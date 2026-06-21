using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace FieldCamp.Behaviours
{
    [HarmonyPatch(typeof(DefaultMobilePartyFoodConsumptionModel), "CalculateDailyFoodConsumptionf")]
    public static class CampFoodPatch
    {
        private static void Postfix(MobileParty party, ref ExplainedNumber __result)
        {
            if (!QuestManager._IsTrainingCampaing) return;
            if (party != MobileParty.MainParty) return;

            __result.AddFactor(0.5f, new TextObject("{=train_your_troops_food_consumption_info_training}Training camp."));
        }
    }
}