using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using System.Linq;
using aEMR.Common;
using System.Windows.Controls;
using System.Windows.Input;
using Castle.Windsor;
using aEMR.CommonTasks;
using System.Collections.Generic;
using System.IO;
/*
20161231 #001 CMN: Add variable for VAT
*/
namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IGenericPayment)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class GenericPaymentViewModel : ViewModelBase, IGenericPayment
    {
        //==== #001
        public bool UseVATOnBill
        {
            get
            {
                return Globals.ServerConfigSection.CommonItems.UseVATOnBill;
            }
        }
        public bool VATAlreadyInPrice
        {
            get
            {
                return Globals.ServerConfigSection.CommonItems.VATAlreadyInPrice;
            }
            set
            {
                //TBR: TTM 14072018
                //Do khong co set nen ben xaml dang xai mode TwoWay bi loi, nhung khong biet set gi nen bo trong
            }
        }
        private decimal _BeforeVATAmount;
        public decimal BeforeVATAmount
        {
            get
            {
                return _BeforeVATAmount;
            }
            set
            {
                _BeforeVATAmount = value;
                NotifyOfPropertyChange(() => BeforeVATAmount);
            }
        }
        private void ReCalAmount()
        {
            try
            {
                if (CurGenericPayment.VATPercent >= 1 && CurGenericPayment.VATPercent <= 2)
                {
                    if (VATAlreadyInPrice)
                    {
                        CurGenericPayment.PaymentAmount = BeforeVATAmount;
                        CurGenericPayment.VATAmount = (decimal)Math.Ceiling((double)BeforeVATAmount / CurGenericPayment.VATPercent.GetValueOrDefault() * Math.Round(CurGenericPayment.VATPercent.GetValueOrDefault() - 1, 2));
                    }
                    else
                    {
                        CurGenericPayment.VATAmount = (decimal)Math.Ceiling((double)BeforeVATAmount * CurGenericPayment.VATPercent.GetValueOrDefault() - (double)BeforeVATAmount);
                        CurGenericPayment.PaymentAmount = CurGenericPayment.VATAmount.GetValueOrDefault() + BeforeVATAmount;
                    }
                }
                else
                {
                    if (CurGenericPayment.VATPercent != null)
                    {
                        MessageBox.Show(eHCMSResources.Z1972_G1_CheckVAT, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    }
                    CurGenericPayment.PaymentAmount = BeforeVATAmount;
                    CurGenericPayment.VATAmount = null;
                    CurGenericPayment.VATPercent = null;
                    //CurGenericPayment.VATAmount = 0;
                    //CurGenericPayment.VATPercent = 0;
                }
            }
            catch
            {
                CurGenericPayment.PaymentAmount = 0;
                CurGenericPayment.VATAmount = null;
                CurGenericPayment.VATPercent = null;
                //CurGenericPayment.VATAmount = 0;
                //CurGenericPayment.VATPercent = 0;
                throw new Exception(eHCMSResources.K0263_G1_VATKhongHopLe2);
            }
        }
        public void txtMoney_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            ReCalAmount();
        }
        //==== #001
        [ImportingConstructor]
        public GenericPaymentViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            //==== #001
            CurGenericPayment.VATPercent = Globals.ServerConfigSection.CommonItems.DefaultVATPercent;
            //==== #001
            IsThuTien = true;
            ListReasonByType = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_GenPaymtReasonTK).ToObservableCollection();

            PaymentTypeContent = Globals.GetViewModel<IEnumListing>();
            PaymentTypeContent.EnumType = typeof(AllLookupValues.V_GenericPaymentType);
            PaymentTypeContent.AddSelectOneItem = true;
            PaymentTypeContent.LoadData();

            Globals.EventAggregator.Subscribe(this);
            CurGenericPayment = new GenericPayment { PaymentDate = Globals.GetCurServerDateTime().Date, StaffID = Globals.LoggedUserAccount.StaffID };
            CurGenericPayment.PaymentDate = Globals.GetCurServerDateTime().Date;
        }

        private string _DeptLocTitle;
        public string DeptLocTitle
        {
            get
            {
                return _DeptLocTitle;
            }
            set
            {
                _DeptLocTitle = value;
                NotifyOfPropertyChange(() => DeptLocTitle);
            }
        }

        private IEnumListing _PaymentTypeContent;
        public IEnumListing PaymentTypeContent
        {
            get { return _PaymentTypeContent; }
            set
            {
                _PaymentTypeContent = value;
                NotifyOfPropertyChange(() => PaymentTypeContent);
            }
        }

        public bool IsDecimal(string s)
        {
            foreach (char itemchar in s)
            {
                if (itemchar != '1' && itemchar != '2' && itemchar != '3' && itemchar != '4' && itemchar != '5' && itemchar != '6'
                    && itemchar != '7' && itemchar != '8' & itemchar != '9' && itemchar != '0')
                {
                    return false;
                }
            }
            return true;
        }

        public void txtSearchCode_KeyUp(TextBox sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCode == null || SearchCode.Trim() == "")
                {
                    MessageBox.Show(eHCMSResources.A0874_G1_Msg_InfoNhapMaPhCanTim);
                    return;
                }
                if (!IsDecimal(SearchCode))
                {
                    MessageBox.Show(eHCMSResources.A0814_G1_Msg_InfoMaPhKhHopLe);
                    return;
                }
                GenericPayment_SearchByCode(SearchCode, FindByStaffID);
            }
        }

        private string _V_Status;
        public string V_Status
        {
            get
            {
                return _V_Status;
            }
            set
            {
                _V_Status = value;
                NotifyOfPropertyChange(() => V_Status);
            }
        }

        private bool _IsDoiBienLai;
        public bool IsDoiBienLai
        {
            get
            {
                return _IsDoiBienLai;
            }
            set
            {
                _IsDoiBienLai = value;
                NotifyOfPropertyChange(() => IsDoiBienLai);
            }
        }

        private bool _IsThuTien = true;
        public bool IsThuTien
        {
            get
            {
                return _IsThuTien;
            }
            set
            {
                _IsThuTien = value;
                NotifyOfPropertyChange(() => IsThuTien);
            }
        }

        private DateTime? _FromDate = Globals.GetCurServerDateTime().Date;
        public DateTime? FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }

        private DateTime? _ToDate = Globals.GetCurServerDateTime().Date;
        public DateTime? ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }

        private ObservableCollection<GenericPayment> _ListGenericPayment;
        public ObservableCollection<GenericPayment> ListGenericPayment
        {
            get
            {
                return _ListGenericPayment;
            }
            set
            {
                _ListGenericPayment = value;
                NotifyOfPropertyChange(() => ListGenericPayment);
            }
        }

        public bool CheckGenPaymtBeforeSave()
        {
            if (CurGenericPayment.PersonName == null || CurGenericPayment.PersonName.Trim() == "" || CurGenericPayment.PersonAddress == null || CurGenericPayment.PersonAddress.Trim() == "")
            {
                MessageBox.Show(eHCMSResources.A0902_G1_Msg_InfoNhapTTin1);
                return false;
            }
            if (!CheckValidDOB(CurGenericPayment.DOB))
            {
                return false;
            }
            if (IsThuTien)
            {
                CurGenericPayment.V_GenericPaymentType = (long)AllLookupValues.V_GenericPaymentType.THU_TIEN;
            }
            else if (IsDoiBienLai)
            {
                CurGenericPayment.V_GenericPaymentType = (long)AllLookupValues.V_GenericPaymentType.DOI_BIEN_LAI;
            }
            if (CurGenericPayment.PaymentAmount <= 0)
            {
                MessageBox.Show(string.Format("{0}. {1}", eHCMSResources.A0604_G1_Msg_InfoNhapSoTien, eHCMSResources.Z0427_G1_SoTienPhaiLonHon0));
                return false;
            }
            if (SelectedReason == null || SelectedReason.LookupID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0336_G1_Msg_InfoChonLyDoThuTien);
                return false;
            }
            CurGenericPayment.LookupReasonID = SelectedReason.LookupID;
            if (IsRequiredReasonDetail && (CurGenericPayment.V_GenericPaymentReason == null
                || CurGenericPayment.V_GenericPaymentReason.Trim().Length < 8))
            {
                MessageBox.Show(eHCMSResources.A0872_G1_Msg_InfoNhapLyDoThu);
                return false;
            }
            while (CurGenericPayment.V_GenericPaymentReason != null && CurGenericPayment.V_GenericPaymentReason.Trim() != "" && CurGenericPayment.V_GenericPaymentReason.Contains("\r\r"))
            {
                CurGenericPayment.V_GenericPaymentReason = CurGenericPayment.V_GenericPaymentReason.Replace("\r\r", "\r");
            }
            return true;
        }

        private GenericPayment _CurGenericPayment = new GenericPayment();
        public GenericPayment CurGenericPayment
        {
            get
            {
                return _CurGenericPayment;
            }
            set
            {
                _CurGenericPayment = value;
                NotifyOfPropertyChange(() => CurGenericPayment);
                NotifyOfPropertyChange(() => CanSaveNew);
                //==== #001
                if (VATAlreadyInPrice == true && CurGenericPayment != null)
                {
                    BeforeVATAmount = CurGenericPayment.PaymentAmount;
                }
                else if (CurGenericPayment != null)
                {
                    BeforeVATAmount = CurGenericPayment.PaymentAmount - CurGenericPayment.VATAmount.GetValueOrDefault();
                }
                else
                {
                    BeforeVATAmount = 0;
                    CurGenericPayment.VATPercent = Globals.ServerConfigSection.CommonItems.DefaultVATPercent;
                }
                //==== #001
            }
        }

        private GenericPayment _RefGenericPayment;
        public GenericPayment RefGenericPayment
        {
            get
            {
                return _RefGenericPayment;
            }
            set
            {
                _RefGenericPayment = value;
                NotifyOfPropertyChange(() => RefGenericPayment);
            }
        }

        private bool _IsAllStaff;
        public bool IsAllStaff
        {
            get
            {
                return _IsAllStaff;
            }
            set
            {
                _IsAllStaff = value;
                NotifyOfPropertyChange(() => IsAllStaff);
                NotifyOfPropertyChange(() => FindByStaffID);
            }
        }

        public bool CanSaveNew
        {
            get
            {
                return CurGenericPayment == null || CurGenericPayment.GenericPaymentID == null || CurGenericPayment.GenericPaymentID <= 0;
            }
        }

        public void SaveNewPaymentCmd()
        {
            if (CurGenericPayment.GenericPaymentID > 0)
            {
                V_Status = eHCMSResources.G2105_G1_CNhat;
            }
            else
            {
                V_Status = "Insert";
            }
            if (CheckGenPaymtBeforeSave())
            {
                GenericPayment_FullOperation(CurGenericPayment);
            }
        }

        private void GenericPayment_FullOperation(GenericPayment GenPayment)
        {
            GenPayment.V_Status = V_Status;
            if (GenPayment.StaffID <= 0)
            {
                GenPayment.StaffID = Globals.LoggedUserAccount.StaffID;
            }
            this.ShowBusyIndicator();
            GenericPayment OutGenericPayment = new GenericPayment();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGenericPayment_FullOperation(GenPayment,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var regItem = contract.EndGenericPayment_FullOperation(out OutGenericPayment, asyncResult);

                                    if (!regItem || OutGenericPayment == null)
                                    {
                                        //HPT: không bao giờ được xảy ra trường hợp này! Để test thôi
                                        MessageBox.Show(eHCMSResources.A0796_G1_Msg_InfoLoadLaiPhFail);
                                    }
                                    else
                                    {
                                        if (V_Status == eHCMSResources.K3177_G1_Delete)
                                        {
                                            MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
                                            if (ListGenericPayment.Any(x => x.GenericPaymentID == OutGenericPayment.GenericPaymentID))
                                            {
                                                ListGenericPayment.RemoveAt(ListGenericPayment.IndexOf(ListGenericPayment.Where(x => x.GenericPaymentID == OutGenericPayment.GenericPaymentID).FirstOrDefault()));
                                            }
                                        }
                                        else
                                        {
                                            if (V_Status == "Insert")
                                            {
                                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                            }
                                            else if (V_Status == eHCMSResources.G2105_G1_CNhat)
                                            {
                                                MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                            }
                                            ToDate = FromDate = null;
                                            CurGenericPayment = OutGenericPayment;
                                            ListGenericPayment = new ObservableCollection<GenericPayment> { CurGenericPayment };
                                            PrintNewReceipt();
                                        }
                                        ResetDefaultData();
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                    MessageBox.Show(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    MessageBox.Show(ex.ToString());
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }

            });
            t.Start();
        }

        //Tìm phiếu theo ngày nhập
        private void GenericPayment_GetAll(DateTime? FromDate, DateTime? ToDate, long? V_GenType, long? StaffID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGenericPayment_GetAll(FromDate, ToDate, V_GenType, StaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var regItem = contract.EndGenericPayment_GetAll(asyncResult);
                                if (regItem != null && regItem.Count > 0)
                                {
                                    ListGenericPayment = regItem.ToObservableCollection();
                                }
                                else
                                {
                                    ListGenericPayment = new ObservableCollection<GenericPayment>();

                                    MessageBox.Show(eHCMSResources.Z0428_G1_KgTimThayPhThu);

                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        //Tìm phiếu theo mã phiếu
        private void GenericPayment_SearchByCode(string genPaymtCode, long StaffID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGenericPayment_SearchByCode(genPaymtCode, StaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                GenericPayment result = contract.EndGenericPayment_SearchByCode(asyncResult);

                                if (result != null && result.GenericPaymentID.GetValueOrDefault() > 0)
                                {
                                    ListGenericPayment = new ObservableCollection<GenericPayment> { result };
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.Z0429_G1_KgTimThayPhThuCoMaYC);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private long? _SearchByV_genType;
        public long? SearchByV_genType
        {
            get
            {
                return _SearchByV_genType;
            }
            set
            {
                _SearchByV_genType = value;
                NotifyOfPropertyChange(() => SearchByV_genType);
            }
        }

        //private long _FindByStaffID;
        public long FindByStaffID
        {
            get
            {
                return (IsAllStaff ? 0 : Globals.LoggedUserAccount.StaffID.GetValueOrDefault());
            }
        }

        public void SearchGenericPaymentList()
        {
            ListGenericPayment = new ObservableCollection<GenericPayment>();
            if (CurGenericPayment != null && RefGenericPayment != null && IsChanged)
            {
                MessageBoxResult result = MessageBox.Show(string.Format("{0}. {1}", eHCMSResources.A045_G1_Msg_InfoTTinPhThuChuaLuu, eHCMSResources.A0138_G1_Msg_ConfBoQua), eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            ResetDefaultData();
            if (IsSearchByCreatedDate)
            {
                if (PaymentTypeContent.SelectedItem != null)
                {
                    if (PaymentTypeContent.SelectedItem.Description == eHCMSResources.K3575_G1_DoiBienLai)
                    {
                        SearchByV_genType = (long)AllLookupValues.V_GenericPaymentType.DOI_BIEN_LAI;
                    }
                    else if (PaymentTypeContent.SelectedItem.Description == eHCMSResources.G0729_G1_ThuTien)
                    {
                        SearchByV_genType = (long)AllLookupValues.V_GenericPaymentType.THU_TIEN;
                    }
                    else
                    {
                        SearchByV_genType = null;
                    }
                }
                else
                {
                    SearchByV_genType = null;
                }
                if (FromDate == null || ToDate == null)
                {
                    MessageBox.Show(eHCMSResources.A0371_G1_Msg_InfoChonTu_DenNg);
                    return;
                }
                if (FromDate > ToDate)
                {
                    MessageBox.Show(eHCMSResources.A0850_G1_Msg_InfoNgKhHopLe2);
                    return;
                }
                GenericPayment_GetAll(FromDate.Value, ToDate.Value.AddDays(1), SearchByV_genType, FindByStaffID);
            }
            else
            {
                if (SearchCode == null || SearchCode.Trim() == "")
                {
                    MessageBox.Show(eHCMSResources.A0874_G1_Msg_InfoNhapMaPhCanTim);
                    return;
                }
                if (!IsDecimal(SearchCode))
                {
                    MessageBox.Show(eHCMSResources.A0814_G1_Msg_InfoMaPhKhHopLe);
                    return;
                }
                GenericPayment_SearchByCode(SearchCode, FindByStaffID);
            }

        }

        private string _SearchCode;
        public string SearchCode
        {
            get
            {
                return _SearchCode;
            }
            set
            {
                _SearchCode = value;
                NotifyOfPropertyChange(() => SearchCode);
            }
        }

        public bool IsSearchByCreatedDate
        {
            get
            {
                return !IsSearchByCode;
            }
        }

        private bool _IsSearchByCode;
        public bool IsSearchByCode
        {
            get
            {
                return _IsSearchByCode;
            }
            set
            {
                _IsSearchByCode = value;
                if (_IsSearchByCode == false)
                {
                    SearchCode = null;
                    ToDate = FromDate = Globals.GetCurServerDateTime().Date;
                }
                else
                {
                    ToDate = FromDate = null;
                    PaymentTypeContent.SelectedItem = null;
                }
                NotifyOfPropertyChange(() => IsSearchByCode);
                NotifyOfPropertyChange(() => IsSearchByCreatedDate);
            }
        }
        public bool IsDifferString(string a, string b)
        {
            if (a == null || a.Trim() == "")
            {
                if (b == null || b.Trim() == "")
                {
                    return false;
                }
                return true;
            }
            if (b == null || b.Trim() == "")
            {
                return true;
            }
            while (a.Contains("\r\r"))
            {
                a = a.Replace("\r\r", "\r");
            }
            while (b.Contains("\r\r"))
            {
                b = b.Replace("\r\r", "\r");
            }
            if (a.Trim() != b.Trim())
            {
                return true;
            }
            return false;
        }
        public bool IsChanged
        {
            get
            {
                return (CurGenericPayment != null && RefGenericPayment != null &&
               (SelectedReason != null && SelectedReason.LookupID != RefGenericPayment.LookupReasonID
                            || IsDifferString(CurGenericPayment.V_GenericPaymentReason, RefGenericPayment.V_GenericPaymentReason)
                            || RefGenericPayment.PaymentAmount != CurGenericPayment.PaymentAmount
                            || RefGenericPayment.PaymentDate != CurGenericPayment.PaymentDate || RefGenericPayment.GeneralNote != CurGenericPayment.GeneralNote));
            }
        }

        public void ClearCmd()
        {
            if (IsChanged)
            {
                MessageBoxResult result = MessageBox.Show(string.Format("{0}. {1}", eHCMSResources.A1037_G1_Msg_InfoTTinBiDoi, eHCMSResources.A0138_G1_Msg_ConfBoQua), eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            ResetDefaultData();
        }

        public void ResetDefaultData()
        {
            BeforeVATAmount = 0;
            SelectedReason = null;
            CurGenericPayment = new GenericPayment { PaymentDate = Globals.GetCurServerDateTime().Date, StaffID = Globals.LoggedUserAccount.StaffID };
            RefGenericPayment = new GenericPayment { PaymentDate = Globals.GetCurServerDateTime().Date, StaffID = Globals.LoggedUserAccount.StaffID };
            IsThuTien = true;
        }


        private GenericPayment _SelectedGenPaymt;
        public GenericPayment SelectedGenPaymt
        {
            get
            {
                return _SelectedGenPaymt;
            }
            set
            {
                _SelectedGenPaymt = value;
                NotifyOfPropertyChange(() => SelectedGenPaymt);
            }
        }

        //HPT: Load lại phiếu cũ để in
        public void PrintOldReceipt()
        {
            if (CurGenericPayment == null || CurGenericPayment.GenericPaymentID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0737_G1_Msg_InfoKhTimThayPhThuDeIn);
                return;
            }
            if (IsChanged)
            {
                MessageBox.Show(string.Format("{0}. {1}", eHCMSResources.A1037_G1_Msg_InfoTTinBiDoi, eHCMSResources.A1039_G1_Msg_InfoLuuDeIn));
                return;
            }
            PrintCmd(CurGenericPayment);
        }

        //HPT: In ngay sau khi lưu xong 14/04/2016
        public void PrintNewReceipt()
        {
            if (CurGenericPayment == null || CurGenericPayment.GenericPaymentID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0737_G1_Msg_InfoKhTimThayPhThuDeIn);
                return;
            }
            PrintCmd(CurGenericPayment);
        }

        public void lnkPrint_Click()
        {
            if (SelectedGenPaymt == null || SelectedGenPaymt.GenericPaymentID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0737_G1_Msg_InfoKhTimThayPhThuDeIn);
                return;
            }
            PrintCmd(SelectedGenPaymt);
        }

        public void PrintCmd(GenericPayment GenPayment)
        {
            if (CurGenericPayment.IsDeleted)
            {
                MessageBox.Show(eHCMSResources.A0906_G1_Msg_InfoKhTheInPh);
                return;
            }
            GenericPayment rptGenpayment = GenPayment.DeepCopy();
            rptGenpayment.V_GenericPaymentReason = GenPayment.ReasonDetail;
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView reportVm)
            {
                reportVm.CurGenPaymt = rptGenpayment;
                reportVm.eItem = ReportName.PHIEU_THU_KHAC;
                switch (Globals.ServerConfigSection.CommonItems.ReceiptVersion)
                {
                    case 1:
                        {
                            reportVm.eItem = ReportName.PHIEU_THU_KHAC;
                            break;
                        }
                    case 2:
                        {
                            reportVm.eItem = ReportName.PHIEU_THU_KHAC;
                            break;
                        }
                    case 4:
                        {
                            reportVm.eItem = ReportName.PHIEU_THU_KHAC_V4;
                            break;
                        }
                    default:
                        {
                            reportVm.eItem = ReportName.PHIEU_THU_KHAC_V4;
                            break;
                        }
                }
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }
        public void btnUpdateGenPaymt_Click()
        {
            if (SelectedGenPaymt.IsDeleted)
            {
                MessageBox.Show(eHCMSResources.A0907_G1_Msg_InfoKhTheCNhatPh);
                return;
            }
            if (SelectedGenPaymt.V_GenericPaymentType == (long)AllLookupValues.V_GenericPaymentType.DOI_BIEN_LAI)
            {
                IsDoiBienLai = true;
            }
            else
            {
                IsThuTien = true;
            }
            if (SelectedGenPaymt.LookupReasonID > 0)
            {
                SelectedReason = ListReasonByType.Where(x => x.LookupID == SelectedGenPaymt.LookupReasonID).FirstOrDefault();
            }
            else
            {
                SelectedReason = null;
            }
            CurGenericPayment = SelectedGenPaymt.DeepCopy();
            RefGenericPayment = CurGenericPayment.DeepCopy();
        }

        public void DeleteGenPaymtCmd()
        {
            MessageBoxResult result = MessageBox.Show(string.Format("{0} {1}?", eHCMSResources.A0193_G1_Msg_ConfXoaPh2, SelectedGenPaymt.GenericPaymentCode), eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel)
            {
                return;
            }
            V_Status = eHCMSResources.K3177_G1_Delete;
            GenericPayment_FullOperation(SelectedGenPaymt);
        }

        private ObservableCollection<Lookup> _ListReasonByType;
        public ObservableCollection<Lookup> ListReasonByType
        {
            get
            {
                return _ListReasonByType;
            }
            set
            {
                _ListReasonByType = value;
                NotifyOfPropertyChange(() => ListReasonByType);
            }
        }

        public bool IsRequiredReasonDetail
        {
            get
            {
                return SelectedReason != null && SelectedReason.LookupID > 0 && (SelectedReason.LookupID == (long)AllLookupValues.V_GenPaymtReasonTK.THU_KHAC);
            }
        }

        private Lookup _SelectedReason = new Lookup();
        public Lookup SelectedReason
        {
            get
            {
                return _SelectedReason;
            }
            set
            {
                _SelectedReason = value;
                NotifyOfPropertyChange(() => SelectedReason);
                NotifyOfPropertyChange(() => IsRequiredReasonDetail);
            }
        }

        public bool CheckValidDOB(string DOB)
        {
            if (string.IsNullOrEmpty(DOB) || string.IsNullOrWhiteSpace(DOB))
            {
                return true;
            }
            if (DOB.Count() != 4 && DOB.Count() != 10)
            {
                MessageBox.Show(eHCMSResources.A0824_G1_Msg_InfoNSinhKhHopLe3);
                return false;
            }
            if (DOB.Count() == 4)
            {
                try
                {
                    int tempDOB = Convert.ToInt16(DOB);
                    if (tempDOB < 1900 || tempDOB > Globals.GetCurServerDateTime().Year)
                    {
                        MessageBox.Show(eHCMSResources.A0823_G1_Msg_InfoNSinhKhHopLe2);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format("{0}. {1}. \n{2}: {3}", eHCMSResources.A0822_G1_Msg_InfoNSinhKhHopLe, eHCMSResources.A1028_G1_Msg_YCKtraTTin, eHCMSResources.T0432_G1_Error, ex.Message));
                    return false;
                }
                return true;
            }
            // Đây là trường hợp nhập 10 ký tự theo định dạng DD/MM/YYYY
            try
            {
                DateTime tempDOB2 = Convert.ToDateTime(DOB);
                if (tempDOB2 == null || tempDOB2.Year < 1900 || tempDOB2.Date > Globals.GetCurServerDateTime().Date)
                {
                    MessageBox.Show(eHCMSResources.A0855_G1_Msg_InfoNgSinhKhHopLe2);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}. \n{1}: {2}", eHCMSResources.A0854_G1_Msg_InfoNgSinhKhHopLe, eHCMSResources.T0432_G1_Error, ex.Message));
                return false;
            }
            return true;
        }
        #region Events
        public void btnFinalization()
        {
            if (CurGenericPayment == null || CurGenericPayment.GenericPaymentID == 0)
            {
                return;
            }
            IEditOutPtTransactionFinalization TransactionFinalizationView = Globals.GetViewModel<IEditOutPtTransactionFinalization>();
            TransactionFinalizationView.InvoiceType = 1;
            TransactionFinalizationView.TransactionFinalizationObj = new OutPtTransactionFinalization
            {
                TaxMemberName = !string.IsNullOrEmpty(CurGenericPayment.OrgName) ? CurGenericPayment.OrgName : CurGenericPayment.PersonName,
                TaxMemberAddress = CurGenericPayment.PersonAddress,
                StaffID = Globals.LoggedUserAccount.Staff.StaffID,
                PatientFullName = CurGenericPayment.PersonName,
                V_PaymentMode = (long)AllLookupValues.PaymentMode.TIEN_MAT,
                GenericPaymentID = CurGenericPayment.GenericPaymentID,
                V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU,
                eInvoiceKey = string.Format("GP-{0}", CurGenericPayment.GenericPaymentCode)
            };
            GlobalsNAV.ShowDialog_V3(TransactionFinalizationView);
            if (!TransactionFinalizationView.IsSaveCompleted)
            {
                return;
            }
            Coroutine.BeginExecute(AddOrUpdateTransactionFinalization_Routine(CurGenericPayment, TransactionFinalizationView.TransactionFinalizationObj, false));
        }
        public void btnExportEInvoice()
        {
            if (CurGenericPayment == null || CurGenericPayment.GenericPaymentID == 0)
            {
                return;
            }
            IEditOutPtTransactionFinalization TransactionFinalizationView = Globals.GetViewModel<IEditOutPtTransactionFinalization>();
            TransactionFinalizationView.TransactionFinalizationObj = new OutPtTransactionFinalization
            {
                V_PaymentMode = (long)AllLookupValues.PaymentMode.TIEN_MAT
            };
            TransactionFinalizationView.ViewCase = 1;
            GlobalsNAV.ShowDialog_V3(TransactionFinalizationView);
            if (!TransactionFinalizationView.IsSaveCompleted)
            {
                return;
            }
            Coroutine.BeginExecute(AddOrUpdateTransactionFinalization_Routine(CurGenericPayment, TransactionFinalizationView.TransactionFinalizationObj, false));
        }
        private IEnumerator<IResult> AddOrUpdateTransactionFinalization_Routine(GenericPayment aGenericPayment, OutPtTransactionFinalization TransactionFinalizationObj, bool aIsUpdate)
        {
            TransactionFinalizationObj.eInvoiceKey = string.Format("GP-{0}", aGenericPayment.GenericPaymentCode);
            ILoggerDialog mLogView = Globals.GetViewModel<ILoggerDialog>();
            var mThread = new Thread(() =>
            {
                GlobalsNAV.ShowDialog_V4(mLogView, null, null, false, true);
            });
            mThread.Start();
            Patient mPatient = new Patient(aGenericPayment);
            List<RptOutPtTransactionFinalizationDetail> mTransactionFinalizationDetail = new List<RptOutPtTransactionFinalizationDetail>();
            mTransactionFinalizationDetail.Add(new RptOutPtTransactionFinalizationDetail
            {
                HITypeDescription = string.Format("{0}{1}", aGenericPayment.LookupReasonName, string.IsNullOrEmpty(aGenericPayment.V_GenericPaymentReason) ? aGenericPayment.V_GenericPaymentReason : string.Format(" {0}", aGenericPayment.V_GenericPaymentReason)),
                IsHasVAT = aGenericPayment.VATPercent > 0,
                VATPercent = aGenericPayment.VATPercent.GetValueOrDefault(0),
                TotalPatientPayment = aGenericPayment.PaymentAmount
            });
            mLogView.AppendLogMessage(string.Format("{0}: {1}", eHCMSResources.Z2651_G1_TienHanhThemMoiBN, mPatient.PatientCode));
            yield return GenericCoRoutineTask.StartTask(CommonGlobals.ImportPatientEInvoice, mPatient, TransactionFinalizationObj, mLogView);
            yield return GenericCoRoutineTask.StartTask(CommonGlobals.AddEInvoice, new VNPTCustomer(mPatient), TransactionFinalizationObj, mTransactionFinalizationDetail, mLogView);
            var AddOrUpdateTransactionFinalizationTask = new GenericCoRoutineTask(CommonGlobals.AddOrUpdateTransactionFinalization, true, TransactionFinalizationObj);
            yield return AddOrUpdateTransactionFinalizationTask;
            mLogView.IsFinished = true;
            if (AddOrUpdateTransactionFinalizationTask.Error != null)
            {
                yield break;
            }
            string mFileName = string.Format("{0}.pdf", TransactionFinalizationObj.InvoiceKey);
            string mFilePath = Path.Combine(Path.GetTempPath(), mFileName);
            CommonGlobals.ExportInvoiceToPdfNoPay(TransactionFinalizationObj.InvoiceKey, mFilePath);
        }
        #endregion
    }
}