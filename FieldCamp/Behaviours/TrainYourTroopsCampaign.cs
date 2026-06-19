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
    public class TrainYourTroopsCampaign
    {
        private static bool _IsFirstTime = true;
        private int ContadorHourlyTick;
        private static int _Aporte;
        private static int _InfoExperienciaSoldados;
        private static int _InfoExperienciaJugador;
        private static bool _ExitoEntrenamiento;
        public static void OnHourlyTick()
        {
            if (!QuestManager._IsCamping || !QuestManager._IsTrainingCampaing)
                return;

            _ExitoEntrenamiento = FormulaExitoEntrenamiento();
            if(_ExitoEntrenamiento)
            {
                DarExperienciaSoldados();
                DarExperienciaJugador();
            }            
            LanzarMensajeAmbiental();

        }
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

        public static void DesactivarCampamento()
        {
            QuestManager._IsCamping = false;
            QuestManager._IsTrainingCampaing = false;
            _IsFirstTime = true;
            CampVisualPatch.RemoveTent();
            MobileParty.MainParty.Party.SetVisualAsDirty();
        }
    }
}
