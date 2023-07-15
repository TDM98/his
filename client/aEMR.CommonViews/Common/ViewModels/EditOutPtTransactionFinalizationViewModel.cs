using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
/*
 * 20201019 #001 TNHX: Chỉnh thông báo lỗi + Refactor code + Bỏ Email mặc định
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IEditOutPtTransactionFinalization)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class EditOutPtTransactionFinalizationViewModel : ViewModelBase, IEditOutPtTransactionFinalization
    {
        [ImportingConstructor]
        public EditOutPtTransactionFinalizationViewModel(IWindsorContainer aContainer, INavigationService aNavigationService, ISalePosCaching aCaching)
        {
            PaymentModeCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.PAYMENT_MODE).ToObservableCollection();
            DateInvoice = Globals.GetCurServerDateTime();
            LoadCharityOrganizationCollection();
        }

        #region Properties
        private OutPtTransactionFinalization _TransactionFinalizationObj;
        private ObservableCollection<Lookup> _PaymentModeCollection;
        public PatientRegistration Registration { get; set; }
        private OutPtTransactionFinalization BackupTransactionFinalizationObj { get; set; }
        public OutPtTransactionFinalization TransactionFinalizationObj
        {
            get => _TransactionFinalizationObj; set
            {
                _TransactionFinalizationObj = value;
                NotifyOfPropertyChange(() => TransactionFinalizationObj);
                NotifyOfPropertyChange(() => IsUpdating);
            }
        }

        public ObservableCollection<Lookup> PaymentModeCollection
        {
            get => _PaymentModeCollection; set
            {
                _PaymentModeCollection = value;
                NotifyOfPropertyChange(() => PaymentModeCollection);
            }
        }

        public bool IsUpdating
        {
            get
            {
                return TransactionFinalizationObj != null && TransactionFinalizationObj.TranFinalizationID > 0;
            }
        }

        private DateTime? _DateInvoice;
        public DateTime? DateInvoice
        {
            get
            {
                return _DateInvoice;
            }
            set
            {
                _DateInvoice = value;
                NotifyOfPropertyChange(() => DateInvoice);
            }
        }

        private bool _IsCollectDataOnly = false;
        public bool IsCollectDataOnly
        {
            get
            {
                return _IsCollectDataOnly;
            }
            set
            {
                _IsCollectDataOnly = value;
                NotifyOfPropertyChange(() => IsCollectDataOnly);
            }
        }

        private bool _IsNormalCreateFinalizationView = true;
        public bool IsNormalCreateFinalizationView
        {
            get => _IsNormalCreateFinalizationView; set
            {
                _IsNormalCreateFinalizationView = value;
                NotifyOfPropertyChange(() => IsNormalCreateFinalizationView);
                NotifyOfPropertyChange(() => IsEnableInvoicePatern);
            }
        }
        private byte _ViewCase = 0; //0: Xuất hóa đơn, 1: Phát hành hóa đơn điện tử, 2: Xuất hóa đơn chuẩn bị dữ liệu cho phát hành hóa đơn điện tử
        public byte ViewCase
        {
            get => _ViewCase; set
            {
                _ViewCase = value;
                NotifyOfPropertyChange(() => ViewCase);
                IsCollectDataOnly = (ViewCase == 1) || (InvoiceType > 0);
                IsNormalCreateFinalizationView = (ViewCase == 0);
            }
        }

        private bool _IsSaveCompleted = false;
        public bool IsSaveCompleted
        {
            get => _IsSaveCompleted; set
            {
                _IsSaveCompleted = value;
                NotifyOfPropertyChange(() => IsSaveCompleted);
            }
        }

        public bool IsEnableInvoicePatern
        {
            get
            {
                return IsNormalCreateFinalizationView && Globals.ServerConfigSection.CommonItems.UserCanEditInvoicePatern;
            }
        }

        private byte _InvoiceType = 0; //0: Hóa đơn cho BN khám chữa bệnh, 1: Hóa đơn cho phiếu thu tiền khác, 2: Hóa đơn cho bán lẻ thuốc
        public byte InvoiceType
        {
            get
            {
                return _InvoiceType;
            }
            set
            {
                _InvoiceType = value;
                NotifyOfPropertyChange(() => InvoiceType);
                IsCollectDataOnly = (ViewCase == 1) || (InvoiceType > 0);
                NotifyOfPropertyChange(() => IsEditableTaxMemberName);
            }
        }

        private bool _IsEditableTaxMemberName = true;
        public bool IsEditableTaxMemberName
        {
            get
            {
                return _IsEditableTaxMemberName && (InvoiceType != 1);
            }
            set
            {
                _IsEditableTaxMemberName = value;
                NotifyOfPropertyChange(() => IsEditableTaxMemberName);
            }
        }
        private List<CharityOrganization> CharityOrganizationCollection { get; set; }
        #endregion

        #region Events
        public void btnFinalization()
        {
            if (!CheckValid())
            {
                return;
            }
            IErrorBold MessBox = Globals.GetViewModel<IErrorBold>();
            MessBox.isCheckBox = true;
            if (ViewCase == 2)
            {
                MessBox.SetMessage(eHCMSResources.Z2641_G1_DangKyDanhDauHTHDDT, eHCMSResources.K3847_G1_DongY);
            }
            else
            {
                MessBox.SetMessage(eHCMSResources.Z2370_G1_Msg, eHCMSResources.K3847_G1_DongY);
            }
            MessBox.FireOncloseEvent = true;
            GlobalsNAV.ShowDialog_V3(MessBox);
            if (MessBox.IsAccept)
            {
                TransactionFinalizationObj.DateInvoice = DateInvoice;
                AddOrUpdateTransactionFinalization();
            }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            LoadData();
        }

        public void btnPrintFinalization()
        {
            if (TransactionFinalizationObj.TranFinalizationID > 0)
            {
                PrintFinalization();
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            if (ViewCase == 1 || ViewCase == 2 || !Globals.ServerConfigSection.CommonItems.UserCanEditInvoicePatern)
            {
                TransactionFinalizationObj.Denominator = Globals.ServerConfigSection.CommonItems.eInvoicePatern;
                TransactionFinalizationObj.Symbol = Globals.ServerConfigSection.CommonItems.eInvoiceSerial;
            }
            BackupTransactionFinalizationObj = TransactionFinalizationObj.DeepCopy();
        }

        public void btnSave()
        {
            if (CheckValid())
            {
                IsSaveCompleted = true;
                TryClose();
            }
        }

        public void btnExportEInvoice()
        {
            if (InvoiceType > 0)
            {
                btnFinalization();
                return;
            }
            if (TransactionFinalizationObj == null || TransactionFinalizationObj.PtRegistrationID == 0 || Registration == null || Registration.Patient == null || string.IsNullOrEmpty(Registration.Patient.PatientCode))
            {
                return;
            }
            ExportEInvoice();
        }

        public void btnUpdateEInvoice()
        {
            if (TransactionFinalizationObj == null || TransactionFinalizationObj.PtRegistrationID == 0)
            {
                return;
            }
            ExportEInvoice(true);
        }

        public void btnCancelEInvoice()
        {
            if (TransactionFinalizationObj == null)
            {
                return;
            }
            GetRptOutPtTransactionFinalizationDetails(null, GettingCase.Cancel);
        }
        #endregion

        #region Methods
        public void LoadData()
        {
            OutPtTransactionFinalization temp = TransactionFinalizationObj;
            if (TransactionFinalizationObj == null)
            {
                return;
            }
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRptOutPtTransactionFinalization(TransactionFinalizationObj.PtRegistrationID, TransactionFinalizationObj.V_RegistrationType, TransactionFinalizationObj.TranFinalizationID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var retval = contract.EndRptOutPtTransactionFinalization(asyncResult);
                                    if (retval != null && retval.TranFinalizationID > 0)
                                    {
                                        TransactionFinalizationObj = retval;
                                        DateInvoice = TransactionFinalizationObj.DateInvoice;
                                    }
                                    AssignAToB(TransactionFinalizationObj, temp);
                                    BackupTransactionFinalizationObj = TransactionFinalizationObj.DeepCopy();
                                }
                                catch (Exception ex)
                                {
                                    GlobalsNAV.ShowMessagePopup(ex.Message);
                                }
                                finally
                                {
                                    this.DlgHideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        private bool CheckValid()
        {
            if (string.IsNullOrEmpty(TransactionFinalizationObj.Denominator) && IsCollectDataOnly)
            {
                MessageBox.Show(eHCMSResources.Z2446_G1_ChuaNhapMauSo);
                return false;
            }
            if (TransactionFinalizationObj.Symbol == "" || TransactionFinalizationObj.Symbol == null)
            {
                MessageBox.Show(eHCMSResources.Z2448_G1_ChuaNhapKyHieu);
                return false;
            }
            if (string.IsNullOrEmpty(TransactionFinalizationObj.InvoiceNumb) && ViewCase == 0 && Globals.ServerConfigSection.CommonItems.UserCanEditInvoicePatern)
            {
                MessageBox.Show(eHCMSResources.Z2447_G1_ChuaNhapSoHD);
                return false;
            }
            //▼===== 20200728 TTM: Bổ sung điều kiện kiểm tra số lượng ký tự của trường số hoá đơn.
            if (TransactionFinalizationObj.InvoiceNumb != null && TransactionFinalizationObj.InvoiceNumb.Length > 12 && ViewCase == 0 && Globals.ServerConfigSection.CommonItems.UserCanEditInvoicePatern)
            {
                MessageBox.Show(eHCMSResources.Z3051_G1_SoHoaDonVuot12KyTu);
                return false;
            }
            //▲===== 
            return true;
        }

        private void AssignAToB(OutPtTransactionFinalization objectA, OutPtTransactionFinalization objectB)
        {
            if (objectA.TaxMemberName == null)
            {
                objectA.TaxMemberName = objectB.TaxMemberName;
            }
            if (objectA.TaxMemberAddress == null)
            {
                objectA.TaxMemberAddress = objectB.TaxMemberAddress;
            }
            if (objectA.V_PaymentMode == 0)
            {
                objectA.V_PaymentMode = objectB.V_PaymentMode;
            }
            if (objectA.PatientFullName == null)
            {
                objectA.PatientFullName = objectB.PatientFullName;
            }
        }

        private void PrintFinalization(long TransactionFinalizationSummaryInfoID = 0)
        {
            if (TransactionFinalizationObj.V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                GlobalsNAV.ShowDialog<ICommonPreviewView>((ICommonPreviewView aView) =>
                {
                    aView.RegistrationID = TransactionFinalizationObj.PtRegistrationID;
                    aView.eItem = ReportName.RptOutPtTransactionFinalization;
                    aView.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
                    aView.ID = 0;
                    aView.PrimaryID = TransactionFinalizationSummaryInfoID;
                });
            }
            else
            {
                GlobalsNAV.ShowDialog<ICommonPreviewView>((ICommonPreviewView aView) =>
                {
                    aView.RegistrationID = TransactionFinalizationObj.PtRegistrationID;
                    aView.eItem = ReportName.RptOutPtTransactionFinalization;
                    aView.V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
                    aView.ID = 0;
                });
            }
        }

        private void ExportEInvoice(bool aIsUpdate = false)
        {
            if (TransactionFinalizationObj == null || TransactionFinalizationObj.PtRegistrationID == 0 || Registration == null || Registration.Patient == null || string.IsNullOrEmpty(Registration.Patient.PatientCode))
            {
                return;
            }
            if (!PropertyHelper.CompareObject(BackupTransactionFinalizationObj, TransactionFinalizationObj) && TransactionFinalizationObj.TranFinalizationID > 0)
            {
                GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2439_G1_DLDaThayDoiLuuLaiTT);
                return;
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new VNPTAccountingPortalServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BegingetCus(Registration.Patient.PatientCode
                            , Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserName
                            , Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserPass, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string jCustomerInfo = contract.EndgetCus(asyncResult);
                                if (VNPTAccountingPortalServiceClient.ErrorCodeDetails.ContainsKey(jCustomerInfo) && jCustomerInfo.Equals("ERR:3"))
                                {
                                    if (MessageBox.Show(VNPTAccountingPortalServiceClient.ErrorCodeDetails[jCustomerInfo], eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                                    {
                                        return;
                                    }
                                    ImportPatientEInvoice();
                                    return;
                                }
                                if (jCustomerInfo.StartsWith("ERR"))
                                {
                                    GlobalsNAV.ShowMessagePopup((string.Format("{0}-{1}", jCustomerInfo, VNPTAccountingPublishServiceClient.ImportAndPublishInvErrorCodeDetails[jCustomerInfo])));
                                    return;
                                }
                                GetRptOutPtTransactionFinalizationDetails(new VNPTCustomer(jCustomerInfo), (aIsUpdate ? GettingCase.Update : GettingCase.Insert));
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void ImportPatientEInvoice()
        {
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new VNPTAccountingPublishServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    VNPTCustomer mVNPTCustomer = new VNPTCustomer(Registration.Patient, TransactionFinalizationObj.TaxMemberAddress);
                    contract.BeginUpdateCus(mVNPTCustomer.ToXML()
                        , Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserName
                        , Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserPass
                        , 0, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int jUpdateInfo = contract.EndUpdateCus(asyncResult);
                            if (jUpdateInfo <= 0)
                            {
                                if (VNPTAccountingPublishServiceClient.ErrorCodeDetails.ContainsKey(jUpdateInfo))
                                {
                                    GlobalsNAV.ShowMessagePopup(VNPTAccountingPublishServiceClient.ErrorCodeDetails[jUpdateInfo]);
                                }
                                else
                                {
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.T0074_G1_I);
                                }
                                return;
                            }
                            GetRptOutPtTransactionFinalizationDetails(mVNPTCustomer, GettingCase.Insert);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private void GetRptOutPtTransactionFinalizationDetails(VNPTCustomer aCustomer, GettingCase aGettingCase)
        {
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRptOutPtTransactionFinalizationDetails(TransactionFinalizationObj.PtRegistrationID, TransactionFinalizationObj.V_RegistrationType,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    List<RptOutPtTransactionFinalizationDetail> mRptOutPtTransactionFinalizationDetailCollection = contract.EndGetRptOutPtTransactionFinalizationDetails(asyncResult);
                                    if (mRptOutPtTransactionFinalizationDetailCollection != null && mRptOutPtTransactionFinalizationDetailCollection.Count > 0)
                                    {
                                        if (aGettingCase == GettingCase.Update)
                                        {
                                            UpdateEInvoice(aCustomer, mRptOutPtTransactionFinalizationDetailCollection);
                                        }
                                        else if (aGettingCase == GettingCase.Insert)
                                        {
                                            if (mRptOutPtTransactionFinalizationDetailCollection.Any(x => x.CharityOrgID > 0))
                                            {
                                                if (mRptOutPtTransactionFinalizationDetailCollection.Any(x => x.CharityOrgID == 0))
                                                {
                                                    AddEInvoice(aCustomer, mRptOutPtTransactionFinalizationDetailCollection.Where(x => x.CharityOrgID == 0).ToList());
                                                }
                                                foreach (var aCharityOrgID in mRptOutPtTransactionFinalizationDetailCollection.Where(x => x.CharityOrgID > 0).Select(x => x.CharityOrgID).Distinct().ToList())
                                                {
                                                    CharityOrganization mCharityOrg = CharityOrganizationCollection.FirstOrDefault(x => x.CharityOrgID == aCharityOrgID);
                                                    if (string.IsNullOrEmpty(mCharityOrg.TaxCode))
                                                    {
                                                        continue;
                                                    }
                                                    VNPTCustomer CharityOrgCustomer = new VNPTCustomer
                                                    {
                                                        Code = mCharityOrg.TaxCode,
                                                        TaxCode = mCharityOrg.TaxCode,
                                                        Address = mCharityOrg.TaxMemberAddress,
                                                        Name = mCharityOrg.TaxMemberName,
                                                        CusType = VNPTCusType.Personal
                                                        //Email = "CharityOrg@hospital.com"
                                                    };
                                                    AddEInvoice(CharityOrgCustomer, mRptOutPtTransactionFinalizationDetailCollection.Where(x => x.CharityOrgID == aCharityOrgID).ToList(), true, mCharityOrg);
                                                }
                                            }
                                            else
                                            {
                                                AddEInvoice(aCustomer, mRptOutPtTransactionFinalizationDetailCollection);
                                            }
                                        }
                                        else if (aGettingCase == GettingCase.Cancel)
                                        {
                                            if (mRptOutPtTransactionFinalizationDetailCollection == null || mRptOutPtTransactionFinalizationDetailCollection.Count == 0)
                                            {
                                                CancelCurrentEInvoice(TransactionFinalizationObj, true);
                                            }
                                            else
                                            {
                                                if (mRptOutPtTransactionFinalizationDetailCollection.Any(x => x.CharityOrgID > 0))
                                                {
                                                    foreach (var aCharityOrgID in mRptOutPtTransactionFinalizationDetailCollection.Where(x => x.CharityOrgID > 0).Select(x => x.CharityOrgID).Distinct().ToList())
                                                    {
                                                        var CancelTransactionFinalization = TransactionFinalizationObj.DeepCopy();
                                                        CancelTransactionFinalization.eInvoiceKey = CancelTransactionFinalization.InvoiceKey + string.Format("-C-{0}", aCharityOrgID);
                                                        CancelCurrentEInvoice(CancelTransactionFinalization, false);
                                                    }
                                                }
                                                if (mRptOutPtTransactionFinalizationDetailCollection.Any(x => x.CharityOrgID == 0))
                                                {
                                                    CancelCurrentEInvoice(TransactionFinalizationObj, true);
                                                }
                                                else
                                                {
                                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2648_G1_HuyHoaDonThanhCong);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2635_G1_ChuaXuatHoaDon);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                finally
                                {
                                    this.DlgHideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        private void AddEInvoice(VNPTCustomer aCustomer, List<RptOutPtTransactionFinalizationDetail> aRptOutPtTransactionFinalizationDetailCollection, bool aIsCharityOrg = false, CharityOrganization aCharityOrg = null)
        {
            string aPattern = TransactionFinalizationObj.Denominator;
            string aSerial = TransactionFinalizationObj.Symbol;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new VNPTAccountingPublishServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    var ImportTransactionFinalization = TransactionFinalizationObj.DeepCopy();
                    if (aIsCharityOrg && aCharityOrg != null)
                    {
                        ImportTransactionFinalization.IsOrganization = true;
                        ImportTransactionFinalization.TaxCode = aCharityOrg.TaxCode;
                        ImportTransactionFinalization.TaxMemberName = aCharityOrg.TaxMemberName;
                        ImportTransactionFinalization.TaxMemberAddress = aCharityOrg.TaxMemberAddress;
                        ImportTransactionFinalization.eInvoiceKey = ImportTransactionFinalization.InvoiceKey + string.Format("-C-{0}", aRptOutPtTransactionFinalizationDetailCollection.First().CharityOrgID);
                    }
                    contract.BeginImportAndPublishInv(Globals.ServerConfigSection.CommonItems.eInvoiceAccountUserName
                        , Globals.ServerConfigSection.CommonItems.eInvoiceAccountUserPass
                        , new VNPTInvoice().ToXML(aCustomer, ImportTransactionFinalization, aRptOutPtTransactionFinalizationDetailCollection)
                        , Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserName
                        , Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserPass
                        , aPattern, aSerial, 0, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string JResult = contract.EndImportAndPublishInv(asyncResult);
                            if (VNPTAccountingPublishServiceClient.ImportAndPublishInvErrorCodeDetails.ContainsKey(JResult))
                            {
                                GlobalsNAV.ShowMessagePopup(string.Format("{0}-{1}", JResult, VNPTAccountingPublishServiceClient.ImportAndPublishInvErrorCodeDetails[JResult]));
                                if (JResult.Equals("ERR:13"))
                                {
                                    ExportEInvoiceToPdf(TransactionFinalizationObj);
                                }
                            }
                            else
                            {
                                GlobalsNAV.ShowMessagePopup(JResult);
                            }
                            if (JResult.StartsWith("OK:") && !aIsCharityOrg)
                            {
                                string mResultToken = string.Format("OK:{0};{1}-{2}_", TransactionFinalizationObj.Denominator, TransactionFinalizationObj.Symbol, TransactionFinalizationObj.InvoiceKey);
                                if (JResult.StartsWith(mResultToken) && !string.IsNullOrEmpty(JResult.Replace(mResultToken, "")))
                                {
                                    TransactionFinalizationObj.InvoiceNumb = JResult.Replace(mResultToken, "").PadLeft(Globals.ServerConfigSection.CommonItems.MaxEInvoicePaternLength, '0');
                                }
                                TransactionFinalizationObj.eInvoiceToken = JResult;
                                AddOrUpdateTransactionFinalization(true);
                            }
                            else if (aIsCharityOrg)
                            {
                                ExportEInvoiceToPdf(ImportTransactionFinalization);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private void UpdateEInvoice(VNPTCustomer aCustomer, List<RptOutPtTransactionFinalizationDetail> aRptOutPtTransactionFinalizationDetailCollection)
        {
            string aPattern = TransactionFinalizationObj.Denominator;
            string aSerial = TransactionFinalizationObj.Symbol;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new VNPTAccountingBusinessServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginAdjustInvoiceAction(Globals.ServerConfigSection.CommonItems.eInvoiceAccountUserName
                        , Globals.ServerConfigSection.CommonItems.eInvoiceAccountUserPass
                        , new VNPTInvoice().ToXML(aCustomer, TransactionFinalizationObj
                        , aRptOutPtTransactionFinalizationDetailCollection, VNPTUpdateType.AdjInfo)
                        , Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserName
                        , Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserPass
                        , (TransactionFinalizationObj.V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU ? "1-" : "3-") + TransactionFinalizationObj.PtRegistrationID.ToString()
                        , null
                        , 0
                        , aPattern, aSerial, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string JResult = contract.EndAdjustInvoiceAction(asyncResult);
                            if (VNPTAccountingPublishServiceClient.ImportAndPublishInvErrorCodeDetails.ContainsKey(JResult))
                            {
                                GlobalsNAV.ShowMessagePopup(string.Format("{0}-{1}", JResult, VNPTAccountingPublishServiceClient.ImportAndPublishInvErrorCodeDetails[JResult]));
                                if (JResult.Equals("ERR:13"))
                                {
                                    ExportEInvoiceToPdf(TransactionFinalizationObj);
                                }
                            }
                            else
                            {
                                GlobalsNAV.ShowMessagePopup((string.Format("{0}-{1}", JResult, VNPTAccountingPublishServiceClient.ImportAndPublishInvErrorCodeDetails[JResult])));
                            }
                            if (JResult.StartsWith("OK:"))
                            {
                                JResult = JResult.Replace("OK:", "").Replace(string.Format("-{0}_", (TransactionFinalizationObj.V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU ? "1-" : "3-") + TransactionFinalizationObj.PtRegistrationID.ToString()), ";");
                                TransactionFinalizationObj.eInvoiceToken = JResult;
                                AddOrUpdateTransactionFinalization(true);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private void AddOrUpdateTransactionFinalization(bool IsUpdateToken = false, bool IsExportPdf = true)
        {
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddOutPtTransactionFinalization(TransactionFinalizationObj, IsUpdateToken, ViewCase
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long TransactionFinalizationSummaryInfoID = 0;
                                long OutTranFinalizationID = 0;
                                if (!contract.EndAddOutPtTransactionFinalization(out TransactionFinalizationSummaryInfoID, out OutTranFinalizationID, asyncResult))
                                {
                                    MessageBox.Show(eHCMSResources.A0991_G1_Msg_ErrorSystem, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                else
                                {
                                    TransactionFinalizationObj.TranFinalizationID = OutTranFinalizationID;
                                    TryClose();
                                    TransactionFinalizationObj.TransactionFinalizationSummaryInfoID = TransactionFinalizationSummaryInfoID;
                                    BackupTransactionFinalizationObj = TransactionFinalizationObj.DeepCopy();
                                    if (ViewCase == 2)
                                    {
                                    }
                                    else if (InvoiceType > 0 && !IsUpdateToken)
                                    {
                                        IsSaveCompleted = true;
                                    }
                                    else if (!IsUpdateToken)
                                    {
                                        PrintFinalization(TransactionFinalizationSummaryInfoID);
                                    }
                                    else if (IsExportPdf)
                                    {
                                        ExportEInvoiceToPdf(TransactionFinalizationObj);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        private void ExportEInvoiceToPdf(OutPtTransactionFinalization TransactionFinalizationObj)
        {
            if (TransactionFinalizationObj == null || TransactionFinalizationObj.PtRegistrationID == 0)
            {
                return;
            }
            string mFileName = string.Format("{0}.pdf", TransactionFinalizationObj.InvoiceKey);
            string mFilePath = Path.Combine(Path.GetTempPath(), mFileName);
            CommonGlobals.ExportInvoiceToPdfNoPay(TransactionFinalizationObj.InvoiceKey, mFilePath);
        }

        private void LoadCharityOrganizationCollection()
        {
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllCharityOrganization(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CharityOrganizationCollection = contract.EndGetAllCharityOrganization(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        private void CancelCurrentEInvoice(OutPtTransactionFinalization aTransactionFinalizationObj, bool aIsUpdateStatus = true)
        {
            if (MessageBox.Show(eHCMSResources.Z2648_G1_HuyHoaDon, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            var mThread = new Thread(() =>
            {
                using (var mFactory = new VNPTAccountingBusinessServiceClient())
                {
                    var mContract = mFactory.ServiceInstance;
                    mContract.BegincancelInv(Globals.ServerConfigSection.CommonItems.eInvoiceAccountUserName
                        , Globals.ServerConfigSection.CommonItems.eInvoiceAccountUserPass, aTransactionFinalizationObj.InvoiceKey
                        , Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserName
                        , Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserPass, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string jUpdateInfo = mContract.EndcancelInv(asyncResult);
                            if (jUpdateInfo.StartsWith("ERR"))
                            {
                                GlobalsNAV.ShowMessagePopup((string.Format("{0}-{1}", jUpdateInfo, VNPTAccountingPublishServiceClient.ImportAndPublishInvErrorCodeDetails[jUpdateInfo])));
                                return;
                            }
                            if (aIsUpdateStatus)
                            {
                                aTransactionFinalizationObj.eInvoiceToken = null;
                                aTransactionFinalizationObj.InvoiceNumb = null;
                                AddOrUpdateTransactionFinalization(true, false);
                                GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2648_G1_HuyHoaDonThanhCong);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                    }), null);
                }
            });

            mThread.Start();
        }
        #endregion
    }

    public enum GettingCase : byte
    {
        Insert = 0,
        Update = 1,
        Cancel = 2
    }
}
