using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar;
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
        private BasicTooltipViewModel _fastForwardX4Hint;
        private bool _isFastForwardX4Selected;
        private bool _isFastForwardNormalSelected;
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
        [DataSourceProperty]
        public BasicTooltipViewModel FastForwardX4Hint
        {
            get => _fastForwardX4Hint;
            set
            {
                if(_fastForwardX4Hint != value)
                {
                    _fastForwardX4Hint = value;
                    ViewModel?.OnPropertyChangedWithValue(value, "FastForwardHint");
                }
            }
        }
        public CampamentoMixin(MapTimeControlVM vm) : base(vm)
        {
            _campamentoHint = new BasicTooltipViewModel(() => new TextObject("{=hint_btn_campamento}Set up your camp here.").ToString());
            _fastForwardX4Hint = new BasicTooltipViewModel(() => new TextObject("{=hint_btn_fastforward_x4}Fast forward time x4[4].").ToString());
        }

        [DataSourceMethod]
        public void ExecuteCampamento()
        {
            if (HayImpedimento())
                return;

            GameMenu.ActivateGameMenu("my_camp_activate");
            Campaign.Current.TimeControlMode = CampaignTimeControlMode.Stop;
        }
       

        [DataSourceProperty]
        public bool IsFastForwardX4Selected
        {
            get => _isFastForwardX4Selected;
            set
            {
                if (_isFastForwardX4Selected != value)
                {
                    _isFastForwardX4Selected = value;
                    ViewModel?.OnPropertyChangedWithValue(value, "IsFastForwardX4Selected");
                }
            }
        }


        [DataSourceMethod]
        public void ExecuteFastForwardX4()
        {
            Campaign campaign = Campaign.Current;
            if (campaign == null) return;

            MySubModule.IsFastForwardX4Active = !MySubModule.IsFastForwardX4Active;
            campaign.SpeedUpMultiplier = MySubModule.IsFastForwardX4Active ? 8f : 4f;
            campaign.SetTimeSpeed(2);
            IsFastForwardX4Selected = MySubModule.IsFastForwardX4Active;
            RefreshFastForwardSelectionState();
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

        private void RefreshFastForwardSelectionState()
        {
            bool isFastForward = Campaign.Current.TimeControlMode == CampaignTimeControlMode.StoppableFastForward
                               || Campaign.Current.TimeControlMode == CampaignTimeControlMode.UnstoppableFastForward;

            IsFastForwardX4Selected = isFastForward && MySubModule.IsFastForwardX4Active;
        }
        public override void OnRefresh()
        {
            base.OnRefresh();
            IsFastForwardX4Selected = MySubModule.IsFastForwardX4Active;
        }
    }
}