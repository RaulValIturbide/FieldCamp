using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace FieldCamp.Behaviours
{
    public class QuestManager :CampaignBehaviorBase
    {
        public static bool _IsCamping;
        public static bool _IsTrainingCampaing;
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnSessionLaunched));
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(TrainYourTroopsCampaign.OnHourlyTick));
            CampaignEvents.TickEvent.AddNonSerializedListener(this, OnTick);
        }

        public override void SyncData(IDataStore dataStore)
        {
            
        }
        public void OnSessionLaunched(CampaignGameStarter gameStarter)
        {
            //Menú del Campamento
            gameStarter.AddWaitGameMenu(
                "my_camp_activate"
                , new TextObject("{=game_menu_lore}The men finish to setting the tents. They await your orders.").ToString()
                , args =>
                {
                    MobileParty.MainParty.SetMoveModeHold();
                    args.MenuContext.SetBackgroundMeshName($"encounter_{CulturePlayerOrDefault()}");
                    _IsCamping = true;
                    MobileParty.MainParty.Party.SetVisualAsDirty();
                }
                , args => true
                , null
                , null
                , GameMenu.MenuAndOptionType.WaitMenuHideProgressAndHoursOption
                , GameMenu.MenuOverlayType.None);
            //OPCIÓN ENTRENAR TROPAS (CAMPAÑA)
            gameStarter.AddGameMenuOption(
                  "my_camp_activate"
                , "train_troops_option"
                , new TextObject("{=game_menu_train_troops_option}Train your men(campaign)").ToString()
                , args =>
                {                    
                    args.optionLeaveType = GameMenuOption.LeaveType.Wait;
                    args.Tooltip = new TextObject("{=hint_train_troops}Train your troops while you wait.");
                    return true;
                }
                , args =>
                {
                    _IsTrainingCampaing = true;
                    Campaign.Current.TimeControlMode = CampaignTimeControlMode.UnstoppableFastForward;
                    args.MenuContext.SetAmbientSound("event:/map/ambient/node/settlements/2d/arena");
                }                
            );
            gameStarter.AddGameMenuOption(
                "my_camp_activate"
                , "exit_camp_option"
                , new TextObject("{=game_menu_exit_camp_option}Dismantle the camp.").ToString()
                , args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Leave;
                    args.Tooltip = new TextObject("{=hint_exit_camp}Exit the camp.");
                    return true;
                }
                , args =>
                {
                    TrainYourTroopsCampaign.DesactivarCampamento();
                    GameMenu.ExitToLast();
                },
                true, -1, false);

        }

        private string CulturePlayerOrDefault()
        {
            var validas = new HashSet<string> { "aserai", "battania", "khuzait", "vlandia", "sturgia", "empire" };
            string id = Hero.MainHero.Culture.StringId;
            return validas.Contains(id) ? id : "empire";
        }
        private void OnTick(float dt)
        {
            if (!QuestManager._IsCamping)
                return;

            // Si estoy "acampado" pero ya no estoy en mi menú de campamento,
            // algo me sacó (soborno, batalla, entrar a un asentamiento...)
            string menuActual = Campaign.Current.CurrentMenuContext?.GameMenu?.StringId;
            if (menuActual != "my_camp_activate")
            {
                TrainYourTroopsCampaign.DesactivarCampamento();
            }

        }
    }
}
