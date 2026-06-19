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
        private static void Postfix(MobileParty party, bool includeDescription, ref ExplainedNumber __result)
        {
            if (!QuestManager._IsCamping) return;
            if (party != MobileParty.MainParty) return;

            __result.AddFactor(0.5f, new TextObject("{=food_consumption_info_training}Training camp."));
        }
    }
}