using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace FieldCamp.Behaviours
{
    public class Forraje
    {
        public static bool _IsForraging;
        public static int contadorForrajeo = 4;
        public static void OnHourlyTick()
        {
            if (QuestManager._IsCamping || !QuestManager._IsForraging)
                return;
            if (contadorForrajeo > 0)
            {
                contadorForrajeo--;
                return;
            }
            contadorForrajeo = 4;
            DarAlimento();
        }


        public static void DarAlimento()
        {
            //Buscamos el terreno en el que se encuentra el jugador
            CampaignVec2 posicion = MobileParty.MainParty.Position;
            TerrainType terreno = Campaign.Current.MapSceneWrapper.GetTerrainTypeAtPosition(posicion);

            //Sacamos la zona de forrajeo
            ZonaForrajeDTO zona = AlimentoDeForraje.ListaAlimentoDisponiblesEnTerreno(terreno);

            if(zona.ExitoEnLaBusqueda())
            {
                int cantidadComida = SeleccionarCuantaComida();
                ItemObject comida = SeleccionComida(zona._ListaAlimentoDisponibles);
                MobileParty.MainParty.ItemRoster.AddToCounts(comida,cantidadComida);

                //Mensaje
                TextObject TxtCantidadComida =new TextObject("={forrage_food_quantity}The man bring {quantity} of {food}.");
                TxtCantidadComida.SetTextVariable("quantity", cantidadComida);
                TxtCantidadComida.SetTextVariable("food", comida.GetName());

                InformationManager.DisplayMessage(new InformationMessage(TxtCantidadComida.ToString()));
            }
            else
            {
                //TODO CAMBIAR
                InformationManager.DisplayMessage(new InformationMessage("Los hombres no han encontrado nada."));
            }
        }
        private static ItemObject SeleccionComida(Dictionary<ItemObject,float> lista)
        {
            float pesoTotal = lista.Values.Sum();
            float dado = MBRandom.RandomFloat * pesoTotal;

            foreach(var item in lista)
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
            return Math.Min(comida, maxComida);
        }

        private static float CantidadSoldadosNoHeridos()
        {
            TroopRoster roster = MobileParty.MainParty.MemberRoster;
            float cantidadSoldados = 0;
            for(int i = 0; i<roster.Count; i++)
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
            QuestManager._IsForraging = false;
        }
    }
}
