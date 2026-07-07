using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapBar;

namespace FieldCamp.Behaviours
{
    public class BotonMapaArreglo
    {
        [HarmonyPatch(typeof(MapCurrentTimeVisualWidget), "OnUpdate")]
        public class MapCurrentTimeVisualWidget_OnUpdate_Patch
        {
            static void Postfix(MapCurrentTimeVisualWidget __instance)
            {
                CampaignTimeControlMode? mode = Campaign.Current?.TimeControlMode;
                bool isFastForward = mode == CampaignTimeControlMode.StoppableFastForward
                                   || mode == CampaignTimeControlMode.UnstoppableFastForward
                                   || mode == CampaignTimeControlMode.UnstoppableFastForwardForPartyWaitTime;

                ButtonWidget x4Button = __instance.FastForwardButton?.ParentWidget?.FindChild("FastForwardButtonX4") as ButtonWidget;

                if (!MySubModule.IsFastForwardX4Active)
                {
                    if (x4Button != null) x4Button.IsSelected = false;
                    return;
                }

                if (!isFastForward)
                {
                    MySubModule.IsFastForwardX4Active = false;
                    if (Campaign.Current != null)
                    {
                        Campaign.Current.SpeedUpMultiplier = 4f;
                    }
                    if (x4Button != null) x4Button.IsSelected = false;
                }
                else
                {
                    if (__instance.FastForwardButton != null)
                    {
                        __instance.FastForwardButton.IsSelected = false;
                    }
                    if (x4Button != null) x4Button.IsSelected = true;
                }
            }
        }
    }
}