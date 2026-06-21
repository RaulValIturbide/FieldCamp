using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace FieldCamp.Behaviours
{
    /// <summary>
    /// Esta clase sirve para gestionar la opción de Entrenar tropas en el mapa de campaña.
    /// El Jugador podrá mandar a entrenar sus tropas,estas recibirán exp y el Jugador mejorará su Liderazgo.    
    /// </summary>
    /// TODO
    /// Tirada de Heridos por entrenamiento
    /// Comprobar que hay suficientes soldados no heridos para entrenar tropas (>=2)
    public class TrainYourTroopsCampaign
    {
        private static bool _IsFirstTime = true;
        private static int _Aporte;
        private static int _InfoExperienciaSoldados;
        private static int _InfoExperienciaJugador;
        private static bool _ExitoEntrenamiento;
        private static int _woundCooldown;

        #region Metodos Publicos
        public static void OnHourlyTick()
        {
            if (!QuestManager._IsCamping || !QuestManager._IsTrainingCampaing)
                return;

            _ExitoEntrenamiento = FormulaExitoEntrenamiento();
            if (_woundCooldown > 0)
            {
                _woundCooldown--;
            }
            if (_ExitoEntrenamiento)//Todo Gucci
            {
                DarExperienciaSoldados();
                DarExperienciaJugador();
            }
            else//Heridos
            {
                HerirSoldado();
            }
            LanzarMensajeAmbiental();
        }
        public static void DesactivarCampamento()
        {
            QuestManager._IsCamping = false;
            QuestManager._IsTrainingCampaing = false;
            QuestManager._resumeCampAfterEncounter = false;
            _IsFirstTime = true;
            CampVisualPatch.RemoveTent();
            MobileParty.MainParty.Party.SetVisualAsDirty();
        }
        #endregion

        #region Metodos Privados
        private static void LanzarMensajeAmbiental()
        {
            TextObject TxtInfoTroopExp = new TextObject("{=train_your_troops_info_troop_exp}The men gain {_InfoExperienciaSoldados} xp.");
            TxtInfoTroopExp.SetTextVariable("_InfoExperienciaSoldados", _InfoExperienciaSoldados);

            TextObject TxtInfoJugadorExp = new TextObject("{=train_your_troops_info_jugador_exp}You gain {_InfoExperienciaJugador} xp.");
            TxtInfoJugadorExp.SetTextVariable("_InfoExperienciaJugador", _InfoExperienciaJugador);

            if (_IsFirstTime)
            {
                InformationManager.DisplayMessage(
                    new InformationMessage(new TextObject(
                        "{=train_your_troops_first_time}The men start to practice the art of war.").ToString()
                        ));
                _IsFirstTime = false;
            }

            if (_ExitoEntrenamiento)
            {
                InformationManager.DisplayMessage(
                    new InformationMessage(TxtInfoJugadorExp.ToString())
                    );

                InformationManager.DisplayMessage(
                    new InformationMessage(TxtInfoTroopExp.ToString())
                    );
            }
            else
            {
                InformationManager.DisplayMessage(
                            new InformationMessage(new TextObject(
                                "{=train_troops_fail_training}The man trained half-heartedly; they haven't learned anything useful.").ToString()
                                ));
            }
        }
        private static int FormulaDeExperienciaTropas()
        {
            //FORMULA: 3 + round(0.4 * Raizde(Liderazgo))
            int liderazgo = Hero.MainHero.GetSkillValue(DefaultSkills.Leadership);
            double raiz = Math.Sqrt(liderazgo);
            int experiencia = 3 + (int)Math.Round(0.4 * raiz);

            return Math.Max(experiencia, 3);
        }
        private static bool FormulaExitoEntrenamiento()
        {
            //Formula :  65 + (LiderazgoJugador * 0.1) - (NumeroTropas * 0.1)
            int liderazgoJugador = Hero.MainHero.GetSkillValue(DefaultSkills.Leadership);
            int numeroSoldados = 0;
            TroopRoster roster = MobileParty.MainParty.MemberRoster;
            for (int i = 0; i < roster.Count; i++)
            {
                TroopRosterElement type = roster.GetElementCopyAtIndex(i);
                if (type.Character.IsHero)
                    continue;
                numeroSoldados += type.Number;
            }
            int aux = Convert.ToInt32(65 + (liderazgoJugador * 0.1) - (numeroSoldados * 0.1));
            int numeroExito = Math.Max(65, aux);

            return MBRandom.RandomInt(0, 100) < numeroExito;
        }
        private static int FormulaDeExperienciaJugador()
        {
            //FORMULA: round(0.05 * aporte
            int formula = (int)Math.Round(0.05 * _Aporte);
            return Math.Max(1, formula);
        }
        private static double FormulaHayHerido()
        {
            ///Hay_herido = random(0,1) < 0.02
            return MBRandom.RandomFloatRanged(0, 1);
        }
        private static void HerirSoldado()
        {           
            if (FormulaHayHerido() >= 0.1)   
                return;
            if (_woundCooldown > 0)   // cooldown activo: aún no puede caer otro herido
                return;
            TroopRoster roster = MobileParty.MainParty.MemberRoster;

            // (3) candidatos válidos: no héroes, entrenables (con upgrade) y con algún sano
            List<int> candidatos = new List<int>();
            for (int i = 0; i < roster.Count; i++)
            {
                TroopRosterElement e = roster.GetElementCopyAtIndex(i);
                if (e.Character.IsHero)
                    continue;
                if (e.Character.UpgradeTargets == null || e.Character.UpgradeTargets.Length == 0)
                    continue;
                if (e.Number - e.WoundedNumber <= 0)   // no quedan sanos en este stack
                    continue;
                candidatos.Add(i);
            }

            if (candidatos.Count == 0)
                return;

            int idx = candidatos[MBRandom.RandomInt(candidatos.Count)];
            TroopRosterElement elegido = roster.GetElementCopyAtIndex(idx);
            CharacterObject soldado = elegido.Character;

            // (2) herir UNO más, sin curar a los que ya estaban heridos
            roster.SetElementWoundedNumber(idx, elegido.WoundedNumber + 1);

            _woundCooldown = 6;   // arma el cooldown

            TextObject TxtInfo = new TextObject("{=train_your_troops_wounded_men}Soldier wounded in training: {soldado}.");
            TxtInfo.SetTextVariable("soldado", soldado.Name);
            InformationManager.DisplayMessage(
                new InformationMessage(TxtInfo.ToString(), new TaleWorlds.Library.Color(1f, 0f, 0f)));
        }
        private static void DarExperienciaSoldados()
        {
            _Aporte = 0;
            TroopRoster roster = MobileParty.MainParty.MemberRoster;

            for (int i = 0; i < roster.Count; i++)
            {
                TroopRosterElement type = roster.GetElementCopyAtIndex(i);
                if (type.Character.IsHero)
                    continue;
                if (type.Character.UpgradeTargets != null && type.Character.UpgradeTargets.Length > 0)
                    _Aporte += type.Character.Tier * (type.Number - type.WoundedNumber);//Se aumenta el aporte con formula => Tier * soldados no heridos
                _InfoExperienciaSoldados = FormulaDeExperienciaTropas();
                roster.AddXpToTroop(type.Character, _InfoExperienciaSoldados);
            }
        }
        private static void DarExperienciaJugador()
        {
            _InfoExperienciaJugador = FormulaDeExperienciaJugador();
            Hero.MainHero.AddSkillXp(DefaultSkills.Leadership, _InfoExperienciaJugador);
        }
        #endregion

    }
}
