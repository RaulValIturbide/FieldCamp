using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;
using System.Collections.Generic;

namespace FieldCamp.Behaviours
{
    public class Prefab
    {
        // 1. Reemplazar el Center Panel por una versión más ancha (500 en vez de 430)
        [PrefabExtension("MapBar", "descendant::MapCurrentTimeVisualWidget")]
        public class PrefabCenterPanel : PrefabExtensionInsertPatch
        {
            public override InsertType Type => InsertType.ReplaceKeepChildren;

            [PrefabExtensionText(true)]
            public string Text => " <DiscardedRoot>" +
                                  "     <MapCurrentTimeVisualWidget DataSource=\"{MapTimeControl}\" Id=\"CenterPanel\" " +
                                  "     VisualDefinition=\"CenterPanel\" WidthSizePolicy=\"Fixed\" HeightSizePolicy=\"Fixed\" " +
                                  "     SuggestedWidth=\"500\" SuggestedHeight=\"59\" HorizontalAlignment=\"Center\" " +
                                  "     VerticalAlignment=\"Bottom\" Sprite=\"MapBar\\mapbar_center_frame\" " +
                                  "     CurrentTimeState=\"@TimeFlowState\" FastForwardButton=\"FastForwardButton\" " +
                                  "     IsEnabled=\"@IsCenterPanelEnabled\" PauseButton=\"PauseButton\" " +
                                  "     PlayButton=\"PlayButton\">" +
                                  "     </MapCurrentTimeVisualWidget>" +
                                  " </DiscardedRoot>";
        }

        // 2. Reposicionar la fecha por el ensanchado
        [PrefabExtension("MapBar", "descendant::MapCurrentTimeVisualWidget[@Id='CenterPanel']/Children/TextWidget")]
        public class PrefabDate : PrefabExtensionSetAttributePatch
        {
            public override List<Attribute> Attributes => new List<Attribute> { new Attribute("SuggestedWidth", "220"), new Attribute("PositionXOffset", "15") };
        }

        // 3. Añadir nuestro botón a la izquierda de FastForward
        [PrefabExtension("MapBar", "descendant::MapCurrentTimeVisualWidget[@Id='CenterPanel']/Children/ButtonWidget[@Id='FastForwardButton']")]
        public class PrefabCampamentoButton : PrefabExtensionInsertPatch
        {
            public override InsertType Type => InsertType.Prepend;
            [PrefabExtensionText]
            public string Text => " <ButtonWidget Id=\"CampamentoButton\" WidthSizePolicy=\"Fixed\" HeightSizePolicy=\"Fixed\" SuggestedWidth=\"35\" SuggestedHeight=\"24\" HorizontalAlignment=\"Right\" " +
                      " VerticalAlignment=\"Bottom\" PositionXOffset=\"-62\" PositionYOffset=\"-13\" " +
                      " DoNotPassEventsToChildren=\"true\" Command.Click=\"ExecuteCampamento\">" +
                      "     <Children>" +
                      "         <IconBrushWidget DoNotAcceptEvents=\"true\" WidthSizePolicy=\"StretchToParent\" " +
                      "         HeightSizePolicy=\"StretchToParent\" HorizontalAlignment=\"Center\" " +
                      "         VerticalAlignment=\"Center\" IconBrush=\"MapBar.Right.Icons\" " +
                      "         IconID=\"troops\"/>" +
                      "         <HintWidget DataSource=\"{CampamentoHint}\" WidthSizePolicy=\"StretchToParent\" HeightSizePolicy=\"StretchToParent\" " +
                      "         Command.HoverBegin=\"ExecuteBeginHint\" Command.HoverEnd=\"ExecuteEndHint\" IsDisabled=\"true\"/>" +
                      "     </Children>" +
                      " </ButtonWidget>";
        }

        // 4. Reposicionar FastForward
        [PrefabExtension("MapBar", "descendant::MapCurrentTimeVisualWidget[@Id='CenterPanel']/Children/ButtonWidget[@Id='FastForwardButton']")]
        public class PrefabFastForwardButton : PrefabExtensionSetAttributePatch
        {
            public override List<Attribute> Attributes => new List<Attribute> { new Attribute("PositionXOffset", "-105") };
        }

        // 5. Reposicionar Play
        [PrefabExtension("MapBar", "descendant::MapCurrentTimeVisualWidget[@Id='CenterPanel']/Children/ButtonWidget[@Id='PlayButton']")]
        public class PrefabPlayButton : PrefabExtensionSetAttributePatch
        {
            public override List<Attribute> Attributes => new List<Attribute> { new Attribute("PositionXOffset", "-145") };
        }

        // 6. Reposicionar Pause
        [PrefabExtension("MapBar", "descendant::MapCurrentTimeVisualWidget[@Id='CenterPanel']/Children/ButtonWidget[@Id='PauseButton']")]
        public class PrefabPauseButton : PrefabExtensionSetAttributePatch
        {
            public override List<Attribute> Attributes => new List<Attribute> { new Attribute("PositionXOffset", "-185") };
        }
    }
}