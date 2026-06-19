using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace FieldCamp.Behaviours
{
    [ViewModelMixin]
    public class CampamentoMixin : BaseViewModelMixin<MapTimeControlVM>
    {
        #region Propiedades Privadas
        private BasicTooltipViewModel _campamentoHint;
        #endregion

        [DataSourceProperty]
        public BasicTooltipViewModel CampamentoHint
        {
            get => _campamentoHint;
            set
            {
                if (_campamentoHint != value)
                {
                    _campamentoHint = value;
                    ViewModel?.OnPropertyChangedWithValue(value, "CampamentoHint");
                }
            }
        }
        public CampamentoMixin(MapTimeControlVM vm) : base(vm)
        {
            _campamentoHint = new BasicTooltipViewModel(() => new TextObject("{=hint_btn_campamento}Set up your camp here.").ToString());
        }

        [DataSourceMethod]
        public void ExecuteCampamento()
        {
            if (QuestManager._IsCamping || MobileParty.MainParty.IsCurrentlyAtSea)
                return;
            GameMenu.ActivateGameMenu("my_camp_activate");
            Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
        }
    }
}