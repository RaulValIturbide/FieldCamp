using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace FieldCamp.Behaviours
{
    public class Forraje
    {
        public static int contadorForrajeo = 4;          // horas entre cada forrajeo

        // Cuántos forrajeos seguidos llevamos en el sitio actual
        private static int _forrajeosEnSitio = 0;
        private const int MAX_FORRAJEOS = 4;             // tras 4 forrajeos, la zona se agota

        // Zonas agotadas: centro + ticks que faltan para que se recupere
        private static readonly List<ZonaAgotada> _zonasAgotadas = new();
        private const float RADIO = 7f;                  // radio de la zona (unidades de mapa, a tunear)
        private const int TICKS_RECUPERACION = 48;       // ~2 días para recuperarse

        private class ZonaAgotada
        {
            public CampaignVec2 Centro;
            public int TicksRestantes;
        }

        public static void OnHourlyTick()
        {
            // Las zonas se recuperan con el tiempo, estés forrajeando o no
            RecuperarZonas();

            if (!QuestManager._IsCamping || !QuestManager._IsForraging)
                return;

            if (contadorForrajeo > 0)
            {
                contadorForrajeo--;
                return;
            }
            contadorForrajeo = 4;

            CampaignVec2 pos = MobileParty.MainParty.Position;

            // ¿Ya hemos esquilmado esta zona? No se puede forrajear aquí.
            if (EstaEnZonaAgotada(pos))
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    new TextObject("{=forrage_exhausted}No more food is available in the area for now.").ToString()
                    , new Color(1f, 0.6f, 0f)));
                return;
            }

            DarAlimento();

            // Contamos el forrajeo en este sitio; si pasamos del máximo, agotamos la zona
            _forrajeosEnSitio++;
            if (_forrajeosEnSitio >= MAX_FORRAJEOS)
            {
                AgotarZona(pos);
                _forrajeosEnSitio = 0;
            }
        }
        private static void RecuperarZonas()
        {
            for (int i = _zonasAgotadas.Count - 1; i >= 0; i--)
            {
                _zonasAgotadas[i].TicksRestantes--;
                if (_zonasAgotadas[i].TicksRestantes <= 0)
                    _zonasAgotadas.RemoveAt(i);
            }
        }
        private static bool EstaEnZonaAgotada(CampaignVec2 pos)
        {
            foreach (var zona in _zonasAgotadas)
            {
                if (zona.Centro.Distance(pos) <= RADIO)
                    return true;
            }
            return false;
        }
        private static void AgotarZona(CampaignVec2 centro)
        {
            _zonasAgotadas.Add(new ZonaAgotada
            {
                Centro = centro,
                TicksRestantes = TICKS_RECUPERACION
            });
        }
        public static void DarAlimento()
        {
            CampaignVec2 posicion = MobileParty.MainParty.Position;
            TerrainType terreno = Campaign.Current.MapSceneWrapper.GetTerrainTypeAtPosition(posicion);

            ZonaForrajeDTO zona = AlimentoDeForraje.ListaAlimentoDisponiblesEnTerreno(terreno);

            if (zona.ExitoEnLaBusqueda())
            {
                int cantidadComida = SeleccionarCuantaComida();
                ItemObject comida = SeleccionComida(zona._ListaAlimentoDisponibles);
                if (comida == null) return;
                MobileParty.MainParty.ItemRoster.AddToCounts(comida, cantidadComida);

                TextObject TxtCantidadComida = new TextObject("{=forrage_food_quantity}The men bring {quantity} of {food}.");
                TxtCantidadComida.SetTextVariable("quantity", cantidadComida);
                TxtCantidadComida.SetTextVariable("food", comida.Name);

                InformationManager.DisplayMessage(new InformationMessage(TxtCantidadComida.ToString()));
            }
            else
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    new TextObject("{=forrage_fail}The men found nothing.").ToString()
                    , new Color(1f, 0f, 0f)));
            }
        }
        private static ItemObject SeleccionComida(Dictionary<ItemObject, float> lista)
        {
            float pesoTotal = lista.Values.Sum();
            float dado = MBRandom.RandomFloat * pesoTotal;

            foreach (var item in lista)
            {
                dado -= item.Value;
                if (dado <= 0f)
                    return item.Key;
            }
            return lista.Keys.LastOrDefault();
        }
        private static int SeleccionarCuantaComida()
        {
            float k = 0.12f;
            int maxComida = 4;

            int comida = Convert.ToInt32(k * Math.Sqrt(CantidadSoldadosNoHeridos() * Hero.MainHero.GetSkillValue(DefaultSkills.Scouting)));
            if (comida < 1)
                comida = 1;
            return Math.Min(comida, maxComida);
        }
        private static float CantidadSoldadosNoHeridos()
        {
            TroopRoster roster = MobileParty.MainParty.MemberRoster;
            float cantidadSoldados = 0;
            for (int i = 0; i < roster.Count; i++)
            {
                TroopRosterElement elemento = roster.GetElementCopyAtIndex(i);
                if (elemento.Character.IsHero)
                    continue;
                cantidadSoldados += (elemento.Number - elemento.WoundedNumber);
            }
            return cantidadSoldados;
        }
        public static void DesactivarForraje()
        {
            contadorForrajeo = 4;
            _forrajeosEnSitio = 0;
            QuestManager._IsForraging = false;
        }
        public static void ExportarZonas(out List<float> xs, out List<float> ys, out List<int> ticks)
        {
            xs = new List<float>();
            ys = new List<float>();
            ticks = new List<int>();

            foreach (var zona in _zonasAgotadas)
            {
                xs.Add(zona.Centro.X);
                ys.Add(zona.Centro.Y);
                ticks.Add(zona.TicksRestantes);
            }
        }
        public static void ImportarZonas(List<float> xs, List<float> ys, List<int> ticks)
        {
            _zonasAgotadas.Clear();

            // Si la partida es vieja (sin estos datos) o algo viene null, no reconstruimos nada.
            if (xs == null || ys == null || ticks == null)
                return;

            int n = Math.Min(xs.Count, Math.Min(ys.Count, ticks.Count));
            for (int i = 0; i < n; i++)
            {
                _zonasAgotadas.Add(new ZonaAgotada
                {
                    Centro = new CampaignVec2(new Vec2(xs[i], ys[i]), true),
                    TicksRestantes = ticks[i]
                });
            }
        }
    }
}