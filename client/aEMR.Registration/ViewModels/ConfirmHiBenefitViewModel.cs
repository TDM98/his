using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.Common.BaseModel;
using Castle.Windsor;
using Caliburn.Micro;
/*
28052018: #001 TxD
*/
namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IConfirmHiBenefit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConfirmHiBenefitViewModel : ViewModelBase, IConfirmHiBenefit
    {
        [ImportingConstructor]
        public ConfirmHiBenefitViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            RebatePercentageLevel1 = Globals.ServerConfigSection.HealthInsurances.RebatePercentageLevel1;            
        }
        public override string ChildWindowTitle
        {
            get
            {
                return eHCMSResources.G2382_G1_XNhanTheBH;
            }
        }
        public override bool HasCloseButton
        {
            get
            {
                return false;
            }
        }
        public long PatientId { get; set; }
        public long HiId { get; set; }

        private string _HIComment;
        public string HIComment
        {
            get { return _HIComment; }
            set
            {
                if(_HIComment != value)
                {
                    _HIComment = value;                    
                }                
                NotifyOfPropertyChange(() => HIComment);
            }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get { return _isEditing && CanEdit; }
            set
            {
                _isEditing = value;
                NotifyOfPropertyChange(() => IsEditing);
                NotifyOfPropertyChange(() => isTextBox);
                NotifyOfPropertyChange(() => CanEditCmd);                
            }
        }

        private bool _CanEdit = true;
        public bool CanEdit
        {
            get 
            {
                    return _CanEdit && (Globals.ServerConfigSection.CommonItems.EditHIBenefit == 1); 
            }
            set
            {
                _CanEdit = value;
                NotifyOfPropertyChange(() => CanEdit);
                NotifyOfPropertyChange(() => IsEditing);
            }
        }

        private bool _CanPressOKButton = true;
        public bool CanPressOKButton
        {
            get
            {
                return _CanPressOKButton;
            }
            set
            {
                _CanPressOKButton = value;
                NotifyOfPropertyChange(() => CanPressOKButton);
            }
        }

        public bool isTextBox
        {
            get { return _isEditing && IsSameRegion; }            
        }

        private bool _HIBenefitChange;
        public bool HIBenefitChange
        {
            get { return _isSameRegion; }
            set
            {
                _HIBenefitChange = value;
                NotifyOfPropertyChange(() => HIBenefitChange);                
            }
        }

        private double _originalHiBenefit;
        public double OriginalHiBenefit
        {
            get { return _originalHiBenefit; }
            set
            {
                _originalHiBenefit = value;
                NotifyOfPropertyChange(() => OriginalHiBenefit);
            }
        }

        private double _hiBenefit;
        public double HiBenefit
        {
            get { return _hiBenefit; }
            set
            {
                _hiBenefit = value;
                NotifyOfPropertyChange(() => HiBenefit);
            }
        }


        private double _RebatePercentageLevel1;
        public double RebatePercentageLevel1
        {
            get { return _RebatePercentageLevel1; }
            set
            {
                _RebatePercentageLevel1 = value;
                NotifyOfPropertyChange(() => RebatePercentageLevel1);
            }
        }

        private bool _isSameRegion;
        public bool IsSameRegion
        {
            get { return _isSameRegion; }
            set
            {
                if (_isSameRegion == value)
                    return;
                _isSameRegion = value;
                if (_isSameRegion)
                {
                    HiBenefit = OriginalHiBenefit;
                }
                NotifyOfPropertyChange(() => IsSameRegion);
                NotifyOfPropertyChange(() => isTextBox);
            }
        }

        private bool _isCrossRegion;
        public bool IsCrossRegion
        {
            get { return _isCrossRegion; }
            set
            {
                if (_isCrossRegion == value)
                    return;
                if(!_isCrossRegion)
                {
                    HiBenefit = RebatePercentageLevel1;                    
                }
                _isCrossRegion = value;
                NotifyOfPropertyChange(() => IsCrossRegion);
                //NotifyOfPropertyChange(() => IsEditing);
            }
        }

        private bool _originalIsCrossRegion;
        public bool OriginalIsCrossRegion
        {
            get { return _originalIsCrossRegion; }
            set
            {
                _originalIsCrossRegion = value;
                NotifyOfPropertyChange(() => OriginalIsCrossRegion);
            }
        }

        public ConfirmHiBenefitEvent confirmHiBenefitEvent { get; set; }

        //private bool _CanOkCmd=true;
        //public bool CanOkCmd 
        //{
        //    get { return !_CanOkCmd; }
        //    set 
        //    {
        //        if(_CanOkCmd != value)
        //        {
        //            _CanOkCmd = value;
        //        }
        //    }
        //}

        public void OkCmd()
        {
            if(IsEditing && (string.IsNullOrEmpty(_HIComment) || _HIComment.Length < 20))
            {
                MessageBox.Show(eHCMSResources.A0265_G1_Msg_InfoNhapGhiChuTruocKhiLuu);
                return;
            }

            Selected_OJR_Option = 0;
            if (IsConfirmReplaceWithAnotherCard)
                Selected_OJR_Option = 1;
            else if (IsConfirmJoiningWithNewCard)
                Selected_OJR_Option = 2;
            else if (IsConfirmRemoveLastAddedCard)
                Selected_OJR_Option = 3;
            else if (IsConfirmNoChangeCard)
                Selected_OJR_Option = 4;

            // **TxD 17/01/2018: If Options are displayed to "Select Types of HI Confirmation" then 
            //                   at least one option should be selected.
            if (ShowOpts_To_OJR_Card > 0 && Selected_OJR_Option == 0)
            {
                MessageBox.Show(eHCMSResources.K3970_G1_ConfirmNoChange);
                return;
            }

            var diff = Math.Abs(HiBenefit - OriginalHiBenefit);
            const double epsilon = 0.001;
            if (diff <= epsilon && _originalIsCrossRegion == IsCrossRegion)
            {
                confirmHiBenefitEvent = new ConfirmHiBenefitEvent { confirmHiBenefitEnum = ConfirmHiBenefitEnum.NoChangeConfirmHiBenefit, HiBenefit = (float)OriginalHiBenefit, HiId = HiId, IsCrossRegion = IsCrossRegion, PatientId = PatientId, HIComment = HIComment, Selected_OJR_Option = Selected_OJR_Option };
                Globals.EventAggregator.Publish(new ConfirmHiBenefitEvent { confirmHiBenefitEnum = ConfirmHiBenefitEnum.NoChangeConfirmHiBenefit, HiBenefit = (float)OriginalHiBenefit, HiId = HiId, IsCrossRegion = IsCrossRegion, PatientId = PatientId, HIComment = HIComment, Selected_OJR_Option = Selected_OJR_Option });
                TryClose();
                return;
            }
            if (HiBenefit < 0.3 || HiBenefit > 1)
            {
                MessageBox.Show(eHCMSResources.A0961_G1_Msg_InGioiHanQLBHYT);
                return;
            }
            if (diff <= epsilon)
            {
                //Chi thay doi dung tuyen hay trai tuyen
                float? benefit = null;
                if (HiBenefit > 0)
                {
                    benefit = (float)HiBenefit;
                }
                confirmHiBenefitEvent = new ConfirmHiBenefitEvent { confirmHiBenefitEnum = ConfirmHiBenefitEnum.ConfirmHiBenefit, HiBenefit = benefit, HiId = HiId, IsCrossRegion = IsCrossRegion, PatientId = PatientId, HIComment = HIComment, Selected_OJR_Option = Selected_OJR_Option };
                Globals.EventAggregator.Publish(new ConfirmHiBenefitEvent { confirmHiBenefitEnum = ConfirmHiBenefitEnum.ConfirmHiBenefit, HiBenefit = benefit, HiId = HiId, IsCrossRegion = IsCrossRegion, PatientId = PatientId, HIComment = HIComment, Selected_OJR_Option = Selected_OJR_Option });
                TryClose();
                return;
            }

            // TxD 28/01/2018: Commented OUT the following because It has NEVER been USED 
            //var t = new Thread(() =>
            //{
            //    try
            //    {
            //        float? benefit = null;
            //        if (HiBenefit > 0)
            //        {
            //            benefit = (float)HiBenefit;
            //        }

            //        var staffID = Globals.LoggedUserAccount.Staff.StaffID;

            //        using (var serviceFactory = new PatientRegistrationServiceClient())
            //        {
            //            var contract = serviceFactory.ServiceInstance;

            //            contract.BeginConfirmHIBenefit(staffID, PatientId, HiId, benefit,
            //                Globals.DispatchCallback(asyncResult =>
            //                {
            //                    bool bOK;
            //                    try
            //                    {
            //                        contract.EndConfirmHIBenefit(asyncResult);
            //                        bOK = true;
            //                    }
            //                    catch (FaultException<AxException> fault)
            //                    {
            //                        ClientLoggerHelper.LogInfo(fault.ToString());
            //                        bOK = false;
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        ClientLoggerHelper.LogInfo(ex.ToString());
            //                        bOK = false;
            //                    }
            //                    if (bOK)
            //                    {
            //                        confirmHiBenefitEvent = new ConfirmHiBenefitEvent { confirmHiBenefitEnum = ConfirmHiBenefitEnum.ConfirmHiBenefit, HiBenefit = benefit, HiId = HiId, IsCrossRegion = IsCrossRegion, PatientId = PatientId, HIComment = HIComment, Selected_OJR_Option = Selected_OJR_Option };
            //                        Globals.EventAggregator.Publish(new ConfirmHiBenefitEvent { confirmHiBenefitEnum = ConfirmHiBenefitEnum.ConfirmHiBenefit, HiBenefit = benefit, HiId = HiId, IsCrossRegion = IsCrossRegion, PatientId = PatientId, HIComment = HIComment, Selected_OJR_Option = Selected_OJR_Option });
            //                        TryClose();
            //                    }

            //                }), null);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //    }
            //});
            //t.Start();

        }

        public bool CanEditCmd
        {
            get
            {
                return !IsEditing && !OriginalIsCrossRegion;
            }
        }

        public void EditCmd()
        {
            IsEditing = true;
        }

        public void CancelCmd()
        {
            confirmHiBenefitEvent = new ConfirmHiBenefitEvent { confirmHiBenefitEnum = ConfirmHiBenefitEnum.RemoveConfirmedHiCard, HiId = HiId };
            Globals.EventAggregator.Publish(new ConfirmHiBenefitEvent { confirmHiBenefitEnum = ConfirmHiBenefitEnum.RemoveConfirmedHiCard , HiId = HiId });
            TryClose();
        }

        public void SetCrossRegion(bool isCrossRegion)
        {
            IsCrossRegion = isCrossRegion;
            IsSameRegion = !isCrossRegion;
        }

        private bool _IsAllowCrossRegion;
        public bool IsAllowCrossRegion
        {
            get
            {
                return _IsAllowCrossRegion;
            }
            set
            {
                _IsAllowCrossRegion = value;
                NotifyOfPropertyChange(() => IsAllowCrossRegion);
            }
        }

        private bool _VisibilityCbxAllowCrossRegion;
        public bool VisibilityCbxAllowCrossRegion
        {
            get
            {
                return _VisibilityCbxAllowCrossRegion;
            }
            set
            {
                _VisibilityCbxAllowCrossRegion = value;
                NotifyOfPropertyChange(() => VisibilityCbxAllowCrossRegion);
            }
        }
        
        public bool ShowOptToRemLastAddedCardFromRegis
        {
            get
            {
                return (ShowOpts_To_OJR_Card == 2);
            }            
        }
        //▼====: #001
        //Thay đổi giá trị đổi, gỡ, nối thẻ (ShowOpts_To_OJR_Card).
        //Ban đầu các giá trị bị chồng chéo và thiếu
        //Hiện tại thay đổi các giá trị cho cụ thể từng trường hợp cụ thể.
        // 1 == Overwrite
        // 2 == Remove ONLY
        // 3 == Join ONLY
        // 4 == Overwrite AND Join 
        public bool ShowOptToAddNewJoiningCardToRegis
        {              
            get
            {
                return (ShowOpts_To_OJR_Card == 3 || ShowOpts_To_OJR_Card == 4);
            }
        }

        public bool ShowOptNoChangeCard
        {
            get
            {
                return true;
            }
        }

        public bool ShowOptToOverwriteConfirmedCard
        {
            get
            {
                return (ShowOpts_To_OJR_Card == 1 || ShowOpts_To_OJR_Card == 4);
            }            
        }

        public bool ShowOptToAddNewCardToRegis
        {
            get
            {
                return (ShowOpts_To_OJR_Card > 0);
            }
        }
        //▲====: #001
        private bool _IsConfirmReplaceWithAnotherCard = false;
        public bool IsConfirmReplaceWithAnotherCard
        {
            get
            {
                return _IsConfirmReplaceWithAnotherCard;
            }
            set
            {
                _IsConfirmReplaceWithAnotherCard = value;
                NotifyOfPropertyChange(() => IsConfirmReplaceWithAnotherCard);
            }
        }

        private bool _IsConfirmJoiningWithNewCard = false;
        public bool IsConfirmJoiningWithNewCard
        {
            get
            {
                return _IsConfirmJoiningWithNewCard;
            }
            set
            {
                _IsConfirmJoiningWithNewCard = value;
                NotifyOfPropertyChange(() => IsConfirmJoiningWithNewCard);
            }
        }

        private bool _IsConfirmRemoveLastAddedCard = false;
        public bool IsConfirmRemoveLastAddedCard
        {
            get
            {
                return _IsConfirmRemoveLastAddedCard;
            }
            set
            {
                _IsConfirmRemoveLastAddedCard = value;
                NotifyOfPropertyChange(() => IsConfirmRemoveLastAddedCard);
            }
        }

        private bool _IsConfirmNoChangeCard = false;
        public bool IsConfirmNoChangeCard
        {
            get
            {
                return _IsConfirmNoChangeCard;
            }
            set
            {
                _IsConfirmNoChangeCard = value;
                NotifyOfPropertyChange(() => IsConfirmNoChangeCard);
            }
        }

        private bool _IsEnabledSelectOptions_N = true;
        public bool IsEnabledSelectOptions_N
        {
            get
            {
                return _IsEnabledSelectOptions_N;
            }
            set
            {
                _IsEnabledSelectOptions_N = value;
                NotifyOfPropertyChange(() => IsEnabledSelectOptions_N);
            }
        }

        private bool _IsEnabledSelectOptions_O = true;
        public bool IsEnabledSelectOptions_O
        {
            get
            {
                return _IsEnabledSelectOptions_O;
            }
            set
            {
                _IsEnabledSelectOptions_O = value;
                NotifyOfPropertyChange(() => IsEnabledSelectOptions_O);
            }
        }
        private bool _IsEnabledSelectOptions_J = true;
        public bool IsEnabledSelectOptions_J
        {
            get
            {
                return _IsEnabledSelectOptions_J;
            }
            set
            {
                _IsEnabledSelectOptions_J = value;
                NotifyOfPropertyChange(() => IsEnabledSelectOptions_J);
            }
        }
        private bool _IsEnabledSelectOptions_R = true;
        public bool IsEnabledSelectOptions_R
        {
            get
            {
                return _IsEnabledSelectOptions_R;
            }
            set
            {
                _IsEnabledSelectOptions_R = value;
                NotifyOfPropertyChange(() => IsEnabledSelectOptions_R);
            }
        }

        // Show Option Box with Radio buttons to Override, Join or Remove 
        // O = Not Showing any options, J = Join, R = Remove a HI Card From Registration 
        private int _ShowOpts_To_OJR_Card = 0;
        public int ShowOpts_To_OJR_Card 
        {
            get
            {
                return _ShowOpts_To_OJR_Card;
            }
            set
            {
                _ShowOpts_To_OJR_Card = value;
            }
        }

        // Selected O or J or R of the Options above
        private int _Selected_OJR_Option = 0;        
        public int Selected_OJR_Option 
        {
            get
            {
                return _Selected_OJR_Option;
            }
            set
            {
                _Selected_OJR_Option = value;
            }
        }

        // Pre-selected (forced selection disallow user to select manually) of O or J or R option (thus all the OJR radio buttons are Read ONLY)
        private int _PreSelected_OJR_Option = 0;
        public int PreSelected_OJR_Option 
        {
            get
            {
                return _PreSelected_OJR_Option;
            }
            set
            {
                _PreSelected_OJR_Option = value;
            }
        }    

    }
}
