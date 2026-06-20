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
            if (HayImpedimento())
                return;

            GameMenu.ActivateGameMenu("my_camp_activate");
            Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
        }
        private bool HayImpedimento()
        {
            bool hayImpedimento = true;
            if (QuestManager._IsCamping)
            {
                InformationManager.DisplayMessage(
                    new InformationMessage(new TextObject("{=my_camp_already_camping_error}The camp is already established.").ToString()
                    ));
            }
            else if (MobileParty.MainParty.IsCurrentlyAtSea)
            {
                InformationManager.DisplayMessage(
                    new InformationMessage(new TextObject("{=my_camp_at_sea_error}The camp cannot be established at sea.").ToString()
                    ));
            }
            else if (!QuestManager.CanCamp())
            {
                InformationManager.DisplayMessage(
                    new InformationMessage(new TextObject("{=my_camp_no_mans_error}The camp cannot be established with less than 2 soldiers.").ToString()
                    ));
            }
            else if (MobileParty.MainParty.Army != null)
            {
                InformationManager.DisplayMessage(
                    new InformationMessage(new TextObject("{=my_camp_at_army_error}The camp cannot be established in an army.").ToString()
                    ));
            }
            else if (PlayerCaptivity.IsCaptive)
            {
                InformationManager.DisplayMessage(
                    new InformationMessage(new TextObject("{=my_camp_is_captive_error}The camp cannot be established if you are captive.").ToString()
                    ));
            }
            else
            {
                hayImpedimento = false;
            }
            return hayImpedimento;
        }
    }
}