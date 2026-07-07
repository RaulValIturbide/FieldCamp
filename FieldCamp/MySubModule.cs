using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using Bannerlord.UIExtenderEx;
using FieldCamp.Behaviours;
using HarmonyLib;

namespace FieldCamp
{
    internal class MySubModule : MBSubModuleBase
    {
        private Harmony _harmony;
        public static bool IsFastForwardX4Active = false;
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            var uiExtender = UIExtender.Create("FieldCamp");
            uiExtender.Register(typeof(MySubModule).Assembly);
            uiExtender.Enable();
            try
            {
                _harmony = new Harmony("com.FieldCamp.campamento");
                _harmony.PatchAll();
            }
            catch (Exception ex)
            {
                InformationManager.DisplayMessage(new InformationMessage("Error con Harmony"));
            }
        }
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            try
            {
                base.OnGameStart(game, gameStarterObject);
                if (!(game.GameType is Campaign))
                {
                    return;
                }
                AddBehaviors(gameStarterObject as CampaignGameStarter);
            }
            catch (Exception e)
            {
                Debug.PrintError(e.Message);
            }
        }
        private void AddBehaviors(CampaignGameStarter gameStarterObject)
        {
            if (gameStarterObject != null)
            {
                gameStarterObject.AddBehavior(new Behaviours.QuestManager()); //QuestManager se trata del nombre que ejecutará nuestro mod, por lo que será diferente en cada caso
            }
        }
    }
}
