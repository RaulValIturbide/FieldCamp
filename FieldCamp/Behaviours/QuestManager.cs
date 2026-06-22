using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace FieldCamp.Behaviours
{
    public class QuestManager : CampaignBehaviorBase
    {
        public static bool _IsCamping;
        public static bool _IsForraging;
        public static bool _IsTrainingCampaing;
        public static bool _IsHiding;
        public static bool _resumeCampAfterEncounter;

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnSessionLaunched));
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(TrainYourTroopsCampaign.OnHourlyTick));
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(Forraje.OnHourlyTick));
            CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(Emboscada.OnHourlyTick));
            CampaignEvents.TickEvent.AddNonSerializedListener(this, OnTick);
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnGameLoaded));
            // Señal temprana y fiable cuando empieza una batalla en la que está el jugador
            CampaignEvents.MapEventStarted.AddNonSerializedListener(this, OnMapEventStarted);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("Field_IsCamping", ref _IsCamping);

            List<float> xs = null, ys = null;
            List<int> ticks = null;

            if (dataStore.IsSaving)
                Forraje.ExportarZonas(out xs, out ys, out ticks);   // lee las estáticas

            dataStore.SyncData("FieldCamp_zona_x", ref xs);
            dataStore.SyncData("FieldCamp_zona_y", ref ys);
            dataStore.SyncData("FieldCamp_zona_ticks", ref ticks);

            if (dataStore.IsLoading)
                Forraje.ImportarZonas(xs, ys, ticks);
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
                , new TextObject("{=game_menu_train_troops_option}Train your men (campaign)").ToString()
                , args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.PracticeFight;
                    args.Tooltip = new TextObject("{=hint_train_troops}Train your troops while you wait.");
                    return true;
                }
                , args =>
                {
                    _IsTrainingCampaing = true;
                    _IsForraging = false;
                    _IsHiding = false;
                    Campaign.Current.TimeControlMode = CampaignTimeControlMode.UnstoppableFastForward;
                    args.MenuContext.SetAmbientSound("event:/map/ambient/node/settlements/2d/arena");
                }
            );
            //OPCION FORRAJEAR
            gameStarter.AddGameMenuOption(
                "my_camp_activate"
                ,"start_forraging"
                ,new TextObject("{=game_menu_forrage}Start forraging.").ToString()
                ,args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Pillage;
                    args.Tooltip = new TextObject("{=hint_forraging}Send the men to forrage the surroundings.");
                    return true;
                }
                ,args =>
                {
                    _IsForraging = true;
                    _IsTrainingCampaing = false;
                    _IsHiding = false;
                    Campaign.Current.TimeControlMode = CampaignTimeControlMode.UnstoppableFastForward;
                }
                );
            //OPCIÓN EMBOSCADA
            gameStarter.AddGameMenuOption(
                "my_camp_activate"
                , "start_ambush"
                , new TextObject("{=game_menu_ambush}Prepare an ambush.").ToString()
                , args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.SneakIn;
                    args.Tooltip = new TextObject("{=hint_ambush}Try to establish an ambush.");
                    return true;
                }
                , args =>
                {
                    _IsForraging = false;
                    _IsTrainingCampaing = false;
                    _IsHiding = true;
                    Emboscada.IntentarOcultarse();
                    Campaign.Current.TimeControlMode = CampaignTimeControlMode.UnstoppablePlay;
                });
            //OPCIÓN SALIR 
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
                    Forraje.DesactivarForraje();
                    GameMenu.ExitToLast();
                },
                true, -1, false);

        }
        private void OnGameLoaded(CampaignGameStarter starter)
        {
            _IsCamping = false;
            _IsForraging = false;
            _IsTrainingCampaing = false;
            _resumeCampAfterEncounter = false;
            // Forraje y entrenamiento resetean sus propios contadores:
            Forraje.DesactivarForraje();          // pone contadorForrajeo=4, _forrajeosEnSitio=0
        }
        private string CulturePlayerOrDefault()
        {
            var validas = new HashSet<string> { "aserai", "battania", "khuzait", "vlandia", "sturgia", "empire","nord" };
            string id = Hero.MainHero.Culture.StringId;
            return validas.Contains(id) ? id : "empire";
        }
        // Si te atacan mientras acampas, marcamos para reanudar (no desmontamos)
        private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
        {
            if (!QuestManager._IsCamping)
                return;
            if (!mapEvent.IsPlayerMapEvent)
                return;

            QuestManager._IsTrainingCampaing = false;
            _resumeCampAfterEncounter = true;
        }
        private void OnTick(float dt)
        {
            if (!QuestManager._IsCamping)
                return;

            // (a) ¿Metidos en un encuentro / batalla / conversación hostil?
            bool enEncuentro =
                PlayerEncounter.Current != null ||
                MapEvent.PlayerMapEvent != null ||
                MobileParty.MainParty.MapEvent != null;

            if (enEncuentro)
            {
                // Algo hostil nos sacó del campamento: pausamos el entreno,
                // marcamos para reanudar y NO desmontamos.
                QuestManager._IsTrainingCampaing = false;
                _resumeCampAfterEncounter = true;
                return;
            }

            // (b) El encuentro terminó y seguimos en el mapa: reabrir el campamento
            if (_resumeCampAfterEncounter)
            {
                _resumeCampAfterEncounter = false;

                bool seguimosVivos =
                    MobileParty.MainParty != null &&
                    MobileParty.MainParty.IsActive &&
                    !Hero.MainHero.IsPrisoner;

                if (seguimosVivos)
                {
                    GameMenu.ActivateGameMenu("my_camp_activate");
                    Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
                }
                else
                {
                    TrainYourTroopsCampaign.DesactivarCampamento();
                }
                return;
            }

            // Salida "normal" (algo cambió el menú sin ser un encuentro): desmontar
            string menuActual = Campaign.Current.CurrentMenuContext?.GameMenu?.StringId;
            if (menuActual != "my_camp_activate")
            {
                TrainYourTroopsCampaign.DesactivarCampamento();
            }
        }
        public static bool CanCamp()
        {
            TroopRoster roster = MobileParty.MainParty.MemberRoster;
            int contador = 0;
            for(int i = 0; i < roster.Count; i++)
            {
                TroopRosterElement elemento = roster.GetElementCopyAtIndex(i);
                if (elemento.Character.IsHero)
                    continue;
                contador += (elemento.Number - elemento.WoundedNumber);
            }
            return contador >= 2;
        }
    }
}