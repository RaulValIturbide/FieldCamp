using HarmonyLib;
using SandBox.View.Map.Visuals;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;

namespace FieldCamp.Behaviours
{
    [HarmonyPatch(typeof(MobilePartyVisual), "RefreshPartyIcon")]
    public static class CampVisualPatch
    {
        private static GameEntity? _tentEntity;

        private static void AddTent(GameEntity strategicEntity)
        {
            if (_tentEntity != null) return;   // ← ya hay una, no crear otra
            _tentEntity = GameEntity.CreateEmpty(strategicEntity.Scene);
            _tentEntity.AddMultiMesh(MetaMesh.GetCopy("map_icon_siege_camp_tent"));
            MatrixFrame frame = MatrixFrame.Identity;
            frame.rotation.ApplyScaleLocal(1.2f);
            _tentEntity.SetFrame(ref frame);
            strategicEntity.AddChild(_tentEntity);
            _tentEntity.SetVisibilityExcludeParents(true);
        }
        internal static void RemoveTent()
        {
            if (_tentEntity != null)
            {
                _tentEntity.Remove(111);   // 111 es el reason id que usa el juego base en OnPartyRemoved
                _tentEntity = null;
            }
        }

        private static void Postfix(MobilePartyVisual __instance)
        {
            if (!QuestManager._IsCamping) return;

            var mapEntity = __instance.MapEntity;
            if (mapEntity?.MobileParty == null) return;
            if (mapEntity.MobileParty.IsCurrentlyAtSea) return;
            if (mapEntity.MobileParty != MobileParty.MainParty) return;

            bool flag = true;
            __instance.HumanAgentVisuals?.GetEntity()?.SetVisibilityExcludeParents(false);
            __instance.MountAgentVisuals?.GetEntity()?.SetVisibilityExcludeParents(false);
            AddTent(__instance.StrategicEntity);
        }
    }
}