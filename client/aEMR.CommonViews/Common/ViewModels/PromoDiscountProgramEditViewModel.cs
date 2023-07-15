using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
/*
* 20181217 #001 TNHX: [BM0005436] Create button to Show report PhieuMienGiam
* 20201002 #002 TNHX: [BM] Thêm loại miễn giảm để tính tiền được miễn giảm: Cùng chi trả/ Chênh lệch/ tất cả + Set default Staff by LoggedStaff
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPromoDiscountProgramEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PromoDiscountProgramEditViewModel : ViewModelBase, IPromoDiscountProgramEdit
    {
        [ImportingConstructor]
        public PromoDiscountProgramEditViewModel(IWindsorContainer aContainer, INavigationService aNavigationService, ISalePosCaching aCaching)
        {
            StaffCollection = Globals.AllStaffs?.ToObservableCollection();
            DiscountReasonCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_DiscountReason).ToObservableCollection();
            //▼====: #002
            DiscountTypeCountCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_DiscountTypeCount).ToObservableCollection();
            //▲====: #022
        }

        #region Properties
        private PromoDiscountProgram _PromoDiscountProgramObj;
        private bool _IsUpdated = false;
        private bool _IsViewOld = false;
        private ObservableCollection<Staff> _StaffCollection;
        private ObservableCollection<Lookup> _DiscountReasonCollection;
        public PromoDiscountProgram PromoDiscountProgramObj
        {
            get => _PromoDiscountProgramObj; set
            {
                _PromoDiscountProgramObj = value;
                //▼====: #002
                if (PromoDiscountProgramObj != null && PromoDiscountProgramObj.ConfirmedStaff == null)
                {
                    if (StaffCollection != null && StaffCollection.Count > 0 && Globals.LoggedUserAccount != null)
                    {
                        PromoDiscountProgramObj.ConfirmedStaff = StaffCollection.Where(x => x.StaffID == Globals.LoggedUserAccount.StaffID).FirstOrDefault().DeepCopy() ?? new Staff();
                    }
                    else
                    {
                        PromoDiscountProgramObj.ConfirmedStaff = new Staff();
                    }
                }
                if (PromoDiscountProgramObj != null && PromoDiscountProgramObj.V_DiscountTypeCount == null && DiscountTypeCountCollection != null)
                {
                    PromoDiscountProgramObj.V_DiscountTypeCount = DiscountTypeCountCollection.Where(x => x.LookupID == (long)AllLookupValues.V_DiscountTypeCount.AmountCoPay).FirstOrDefault();
                }
                //▲====: #002
                NotifyOfPropertyChange(() => PromoDiscountProgramObj);
            }
        }
        public bool IsUpdated { get => _IsUpdated; set => _IsUpdated = value; }
        public bool IsViewOld { get => _IsViewOld; set => _IsViewOld = value; }
        public ObservableCollection<Staff> StaffCollection
        {
            get => _StaffCollection; set
            {
                _StaffCollection = value;
                NotifyOfPropertyChange(() => StaffCollection);
            }
        }

        public ObservableCollection<Lookup> DiscountReasonCollection
        {
            get => _DiscountReasonCollection; set
            {
                _DiscountReasonCollection = value;
                NotifyOfPropertyChange(() => DiscountReasonCollection);
            }
        }
        #endregion

        #region Events
        public void btnSave()
        {
            if (PromoDiscountProgramObj == null)
            {
                TryClose();
            }
            if (PromoDiscountProgramObj.ConfirmedStaff == null || PromoDiscountProgramObj.ConfirmedStaff.StaffID == 0)
            {
                MessageBox.Show(eHCMSResources.Z2348_G1_ThieuThongTinNguoiXN, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }
            if (!PromoDiscountProgramObj.IsOnPriceDiscount && PromoDiscountProgramObj.DiscountPercent <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0135_G1_PhanTramHTroPhaiLonHon0, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (string.IsNullOrEmpty(PromoDiscountProgramObj.PromoDiscName))
            {
                MessageBox.Show(eHCMSResources.Z0494_G1_ChuaChonTen, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (string.IsNullOrEmpty(PromoDiscountProgramObj.ReasonOrNote))
            {
                MessageBox.Show(eHCMSResources.Z2180_G1_VuiLongNhapLyDo, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (PtRegistrationID == 0)
            {
                IsUpdated = true;
                TryClose();
                return;
            }
            EditPromoDiscountProgram();
            //IsUpdated = true;
            //TryClose();
        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (PromoDiscountProgramObj == null || PromoDiscountProgramObj.PromoDiscProgID == 0 || !IsViewOld)
            {
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetPromoDiscountProgramByPromoDiscProgID(PromoDiscountProgramObj.PromoDiscProgID, PromoDiscountProgramObj.V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                PromoDiscountProgramObj = contract.EndGetPromoDiscountProgramByPromoDiscProgID(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        //▼====: #001
        public void BtnViewPrint()
        {
            void onInitDlg(ICommonPreviewView reportVm)
            {
                reportVm.PromoDiscProgID = PromoDiscountProgramObj.PromoDiscProgID;
                reportVm.TotalMienGiam = 0;
                reportVm.RegistrationID = PtRegistrationID;
                reportVm.PatientPCLReqID = PatientPCLReqID;
                reportVm.V_RegistrationType = PromoDiscountProgramObj.V_RegistrationType;
                reportVm.eItem = ReportName.PHIEUMIENGIAM_NGOAITRU_TV;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }
        public bool CanBtnViewPrint { get; set; } = false;
        public long PtRegistrationID { get; set; } = 0;
        public long PatientPCLReqID { get; set; } = 0;
        //▲====: #001
        public void cboIsPriceDiscount_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == null || !(sender is CheckBox))
            {
                return;
            }
            if ((sender as CheckBox).IsChecked.GetValueOrDefault(false))
            {
                PromoDiscountProgramObj.DiscountPercent = 0;
            }
        }

        //▼====: #002
        private ObservableCollection<Lookup> _DiscountTypeCountCollection;
        public ObservableCollection<Lookup> DiscountTypeCountCollection
        {
            get { return _DiscountTypeCountCollection; }
            set
            {
                _DiscountTypeCountCollection = value;
                NotifyOfPropertyChange(() => DiscountTypeCountCollection);
            }
        }
        //▲====: #002        
        #endregion

        #region Methods
        private void EditPromoDiscountProgram()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginEditPromoDiscountProgram(PromoDiscountProgramObj, PtRegistrationID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var mPromoDiscountProgramObj = contract.EndEditPromoDiscountProgram(asyncResult);
                                    if (mPromoDiscountProgramObj != null && mPromoDiscountProgramObj.PromoDiscProgID > 0)
                                    {
                                        mPromoDiscountProgramObj.ConfirmedStaff = StaffCollection.FirstOrDefault(x => mPromoDiscountProgramObj.ConfirmedStaff != null && x.StaffID == mPromoDiscountProgramObj.ConfirmedStaff.StaffID);
                                        PromoDiscountProgramObj = mPromoDiscountProgramObj;
                                        IsUpdated = true;
                                        TryClose();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        #endregion
    }
}
