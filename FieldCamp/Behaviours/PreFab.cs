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
                                  "     SuggestedWidth=\"600\" SuggestedHeight=\"60\" HorizontalAlignment=\"Center\" " +
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
            public override List<Attribute> Attributes => new List<Attribute> { new Attribute("SuggestedWidth", "220"), new Attribute("PositionXOffset", "20") };
        }

        // 3. Añadir BOTON CAMAPAMENTO a la derecha de FastForward
        [PrefabExtension("MapBar", "descendant::MapCurrentTimeVisualWidget[@Id='CenterPanel']/Children/ButtonWidget[@Id='FastForwardButton']")]
        public class PrefabCampamentoButton : PrefabExtensionInsertPatch
        {
            public override InsertType Type => InsertType.Prepend;
            [PrefabExtensionText]
            public string Text => " <ButtonWidget Id=\"CampamentoButton\" WidthSizePolicy=\"Fixed\" HeightSizePolicy=\"Fixed\" SuggestedWidth=\"35\" SuggestedHeight=\"24\" HorizontalAlignment=\"Right\" " +
                      " VerticalAlignment=\"Bottom\" PositionXOffset=\"-80\" PositionYOffset=\"-13\" " +
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
        // 3.2 Añadir  botón FAST FORWARD X8
        [PrefabExtension("MapBar", "descendant::MapCurrentTimeVisualWidget[@Id='CenterPanel']/Children/ButtonWidget[@Id='FastForwardButton']")]
        public class PrefabFastForwardButtonX4 : PrefabExtensionInsertPatch
        {
            public override InsertType Type => InsertType.Prepend;
            [PrefabExtensionText]
            public string Text => " <ButtonWidget Id=\"FastForwardButtonX4\" WidthSizePolicy=\"Fixed\" HeightSizePolicy=\"Fixed\" SuggestedWidth=\"35\" SuggestedHeight=\"24\" HorizontalAlignment=\"Right\" " +
                      " VerticalAlignment=\"Bottom\" PositionXOffset=\"-120\" PositionYOffset=\"-13\" " +
                      " Brush=\"MapBarFastForwardButton\" IsSelected=\"@IsFastForwardX4Selected\" DoNotPassEventsToChildren=\"true\" Command.Click=\"ExecuteFastForwardX4\">" +
                      "     <Children>" +
                      "         <HintWidget DataSource=\"{FastForwardX4Hint}\" WidthSizePolicy=\"StretchToParent\" HeightSizePolicy=\"StretchToParent\" " +
                      "         Command.HoverBegin=\"ExecuteBeginHint\" Command.HoverEnd=\"ExecuteEndHint\" IsDisabled=\"true\"/>" +
                      "     </Children>" +
                      " </ButtonWidget>";
        }

        //[PrefabExtension("MapBar", "descendant::MapCurrentTimeVisualWidget[@Id='CenterPanel']/Children/ButtonWidget[@Id='FastForwardButton']")]
        //public class PrefabFastForwardButtonSelectedOverride : PrefabExtensionSetAttributePatch
        //{
        //    public override List<Attribute> Attributes => new List<Attribute>
        //    {
        //         new Attribute("IsSelected", "@IsFastForwardNormalSelected")
        //    };
        //}

        // 4. Reposicionar FastForward
        [PrefabExtension("MapBar", "descendant::MapCurrentTimeVisualWidget[@Id='CenterPanel']/Children/ButtonWidget[@Id='FastForwardButton']")]
        public class PrefabFastForwardButton : PrefabExtensionSetAttributePatch
        {
            public override List<Attribute> Attributes => new List<Attribute> { new Attribute("PositionXOffset", "-160") };
        }

        // 5. Reposicionar Play
        [PrefabExtension("MapBar", "descendant::MapCurrentTimeVisualWidget[@Id='CenterPanel']/Children/ButtonWidget[@Id='PlayButton']")]
        public class PrefabPlayButton : PrefabExtensionSetAttributePatch
        {
            public override List<Attribute> Attributes => new List<Attribute> { new Attribute("PositionXOffset", "-200") };
        }

        // 6. Reposicionar Pause
        [PrefabExtension("MapBar", "descendant::MapCurrentTimeVisualWidget[@Id='CenterPanel']/Children/ButtonWidget[@Id='PauseButton']")]
        public class PrefabPauseButton : PrefabExtensionSetAttributePatch
        {
            public override List<Attribute> Attributes => new List<Attribute> { new Attribute("PositionXOffset", "-240") };
        }
    }
}