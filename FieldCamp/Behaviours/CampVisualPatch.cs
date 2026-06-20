using HarmonyLib;
using SandBox.View.Map.Visuals;
using System.Reflection;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace FieldCamp.Behaviours
{
    [HarmonyPatch(typeof(MobilePartyVisual), "RefreshPartyIcon")]
    public static class CampVisualPatch
    {
        private const string TentTag = "field_camp_tent";

        private static GameEntity? _tentEntity;

        // Método privado del juego que monta la tienda con bandera (AccessTools recorre clases base)
        private static readonly MethodInfo? _addTentMethod =
            AccessTools.Method(typeof(MobilePartyVisual), "AddTentEntityForParty");

        // Busca nuestra tienda entre los hijos del StrategicEntity por su tag (fiable tras reconstrucciones)
        private static GameEntity? FindTent(GameEntity strategicEntity)
        {
            int n = strategicEntity.ChildCount;
            for (int i = 0; i < n; i++)
            {
                var child = strategicEntity.GetChild(i);
                if (child != null && child.HasTag(TentTag))
                    return child;
            }
            return null;
        }

        private static void AddTent(MobilePartyVisual instance, PartyBase party, GameEntity strategicEntity)
        {
            GameEntity? tent = null;

            if (_addTentMethod != null)
            {
                int antes = strategicEntity.ChildCount;
                // El 3er parámetro es 'ref bool'; va en el array y se actualiza tras Invoke.
                object[] parametros = { strategicEntity, party, true };
                _addTentMethod.Invoke(instance, parametros);

                int despues = strategicEntity.ChildCount;
                if (despues > antes)
                    tent = strategicEntity.GetChild(despues - 1); // la tienda recién creada
            }

            // Fallback: tienda manual sin bandera, por si el método del juego no aparece
            if (tent == null)
            {
                tent = GameEntity.CreateEmpty(strategicEntity.Scene);
                tent.AddMultiMesh(MetaMesh.GetCopy("map_icon_siege_camp_tent"));
                MatrixFrame frame = MatrixFrame.Identity;
                frame.rotation.ApplyScaleLocal(1.2f);
                tent.SetFrame(ref frame);
                strategicEntity.AddChild(tent);
                tent.SetVisibilityExcludeParents(true);
            }

            tent.AddTag(TentTag);   // ← la marca que nos deja reconocerla luego
            _tentEntity = tent;
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

            var strategicEntity = __instance.StrategicEntity;
            if (strategicEntity == null) return;

            __instance.HumanAgentVisuals?.GetEntity()?.SetVisibilityExcludeParents(false);
            __instance.MountAgentVisuals?.GetEntity()?.SetVisibilityExcludeParents(false);

            // Solo creamos si no hay ya una tienda colgando ahora mismo
            if (FindTent(strategicEntity) == null)
                AddTent(__instance, mapEntity.MobileParty.Party, strategicEntity);
        }
    }
}