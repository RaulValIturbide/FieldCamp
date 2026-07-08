using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v1;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace FieldCamp.Behaviours
{
    public class FieldCampSettings : AttributeGlobalSettings<FieldCampSettings>
    {
        //public static FieldCampSettings Instance { get; private set; } = new FieldCampSettings();
        public override string Id => "FieldCamp_v1";
        public override string FolderName => "FieldCamp";
        public override string DisplayName => "Field Camp";
        public override string FormatType => "json";

        #region SPEED TIME MODE
        [SettingPropertyBool("Enable x8 hotkey (4)", Order = 1, RequireRestart = false,
            HintText = "If disabled, pressing 4 won't activate x8 speed. Useful if another mod also uses this key.")]
        [SettingPropertyGroup("SPEED TIME")]
        public bool IsX4HotkeyEnabled { get; set; } = true;
        [SettingPropertyInteger("Time speed", minValue: 8, 99, Order = 2,
            HintText = "Adjust the time speed for the fast forward button", RequireRestart = false)]
        [SettingPropertyGroup("SPEED TIME")]
        public float FastForwardSpeed { get; set; } = 8f;
        #endregion

        #region FIELD CAMP
        [SettingPropertyInteger("Times you can forage until it goes empty", 4, 100, Order = 1, RequireRestart = false,
            HintText = "Max times you can forage in an area until it goes empty (4 default)")]
        [SettingPropertyGroup("FIELD_CAMP")]
        public int MaxForageAttempts { get; set; } = 4;
        #endregion
    }
}
