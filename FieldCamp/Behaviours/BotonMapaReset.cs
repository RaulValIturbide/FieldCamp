using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar;
using TaleWorlds.Library;

namespace FieldCamp.Behaviours
{
    public class BotonMapaReset
    {
        [HarmonyPatch(typeof(MapTimeControlVM), "ExecuteTimeControlChange")]
        public class MapTimeControlVM_ExecuteTimeControlChange_Patch
        {
            static void Prefix(int selectedTimeSpeed)
            {

                if (!MySubModule.IsFastForwardX4Active) return;

                MySubModule.IsFastForwardX4Active = false;
                if (Campaign.Current != null)
                {
                    Campaign.Current.SpeedUpMultiplier = 4f;
                }
            }
        }
    }
}
