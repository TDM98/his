using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using System.Collections.Generic;
using aEMR.Common;
using System.Windows.Media;
using aEMR.CommonTasks;
using eHCMSLanguage;
using aEMR.Common.Converters;
using Castle.Windsor;

namespace eHCMS.DrugDept.ViewModels
{
    [Export(typeof(IEditOutwardFromPrescription)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class EditOutwardFromPrescriptionViewModel : Conductor<object>, IEditOutwardFromPrescription
         , IHandle<EditChooseBatchNumberForPrescriptionEvent>
        , IHandle<EditChooseBatchNumberForPrescriptionResetQtyEvent>
    {
        #region Indicator Member

        private bool _isLoadingGetStore = false;
        public bool isLoadingGetStore
        {
            get { return _isLoadingGetStore; }
            set
            {
                if (_isLoadingGetStore != value)
                {
                    _isLoadingGetStore = value;
                    NotifyOfPropertyChange(() => isLoadingGetStore);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingInfoPatient = false;
        public bool isLoadingInfoPatient
        {
            get { return _isLoadingInfoPatient; }
            set
            {
                if (_isLoadingInfoPatient != value)
                {
                    _isLoadingInfoPatient = value;
                    NotifyOfPropertyChange(() => isLoadingInfoPatient);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingFullOperator = false;
        public bool isLoadingFullOperator
        {
            get { return _isLoadingFullOperator; }
            set
            {
                if (_isLoadingFullOperator != value)
                {
                    _isLoadingFullOperator = value;
                    NotifyOfPropertyChange(() => isLoadingFullOperator);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingFullOperator1 = false;
        public bool isLoadingFullOperator1
        {
            get { return _isLoadingFullOperator1; }
            set
            {
                if (_isLoadingFullOperator1 != value)
                {
                    _isLoadingFullOperator1 = value;
                    NotifyOfPropertyChange(() => isLoadingFullOperator1);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingFullOperator2 = false;
        public bool isLoadingFullOperator2
        {
            get { return _isLoadingFullOperator2; }
            set
            {
                if (_isLoadingFullOperator2 != value)
                {
                    _isLoadingFullOperator3 = value;
                    NotifyOfPropertyChange(() => isLoadingFullOperator2);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingFullOperator3 = false;
        public bool isLoadingFullOperator3
        {
            get { return _isLoadingFullOperator3; }
            set
            {
                if (_isLoadingFullOperator3 != value)
                {
                    _isLoadingFullOperator3 = value;
                    NotifyOfPropertyChange(() => isLoadingFullOperator3);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }
        private bool _isLoadingInfoReg = false;
        public bool isLoadingInfoReg
        {
            get { return _isLoadingInfoReg; }
            set
            {
                if (_isLoadingInfoReg != value)
                {
                    _isLoadingInfoReg = value;
                    NotifyOfPropertyChange(() => isLoadingInfoReg);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingGetID = false;
        public bool isLoadingGetID
        {
            get { return _isLoadingGetID; }
            set
            {
                if (_isLoadingGetID != value)
                {
                    _isLoadingGetID = value;
                    NotifyOfPropertyChange(() => isLoadingGetID);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingSearch = false;
        public bool isLoadingSearch
        {
            get { return _isLoadingSearch; }
            set
            {
                if (_isLoadingSearch != value)
                {
                    _isLoadingSearch = value;
                    NotifyOfPropertyChange(() => isLoadingSearch);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingDetail = false;
        public bool isLoadingDetail
        {
            get { return _isLoadingDetail; }
            set
            {
                if (_isLoadingDetail != value)
                {
                    _isLoadingDetail = value;
                    NotifyOfPropertyChange(() => isLoadingDetail);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }




        public bool IsLoading
        {
            get { return (isLoadingGetStore || isLoadingFullOperator || isLoadingFullOperator1 || isLoadingFullOperator2 || isLoadingFullOperator3 || isLoadingInfoReg || isLoadingInfoPatient || isLoadingGetID || isLoadingSearch || isLoadingDetail); }
        }

        #endregion
        [ImportingConstructor]
        public EditOutwardFromPrescriptionViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
            Globals.EventAggregator.Subscribe(this);

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());

            GetStaffLogin();

            RefeshData();
            SetDefaultForStore();
        }
        private void RefeshData()
        {
            SelectedOutInvoice = null;
            SelectedOutInvoice = new OutwardDrugMedDeptInvoice();
            SelectedOutInvoice.OutDate = Globals.ServerDate.Value;

            SearchCriteria = null;
            SearchCriteria = new PrescriptionSearchCriteria();

            SearchInvoiceCriteria = null;
            SearchInvoiceCriteria = new SearchOutwardInfo();
            SearchInvoiceCriteria.TypID = (long)AllLookupValues.RefOutputType.BANTHEOTOA;

            PatientInfo = null;
            CurRegistration = null;
            SelectedPrescription = null;
            SelectedPrescription = new Prescription();
            OutwardDrugsCopy = null;

            if (GetGenMedProductForSellList == null)
            {
                GetGenMedProductForSellList = new ObservableCollection<GetGenMedProductForSell>();
            }
            else
            {
                GetGenMedProductForSellList.Clear();
            }
            if (GetGenMedProductForSellListSum == null)
            {
                GetGenMedProductForSellListSum = new ObservableCollection<GetGenMedProductForSell>();
            }
            else
            {
                GetGenMedProductForSellListSum.Clear();
            }

            if (GetGenMedProductForSellTemp == null)
            {
                GetGenMedProductForSellTemp = new ObservableCollection<GetGenMedProductForSell>();
            }
            else
            {
                GetGenMedProductForSellTemp.Clear();
            }


        }
        private void ClearData()
        {
            OutwardDrugsCopy = null;

            if (GetGenMedProductForSellList == null)
            {
                GetGenMedProductForSellList = new ObservableCollection<GetGenMedProductForSell>();
            }
            else
            {
                GetGenMedProductForSellList.Clear();
            }
            if (GetGenMedProductForSellListSum == null)
            {
                GetGenMedProductForSellListSum = new ObservableCollection<GetGenMedProductForSell>();
            }
            else
            {
                GetGenMedProductForSellListSum.Clear();
            }

            if (GetGenMedProductForSellTemp == null)
            {
                GetGenMedProductForSellTemp = new ObservableCollection<GetGenMedProductForSell>();
            }
            else
            {
                GetGenMedProductForSellTemp.Clear();
            }

        }

        private void SetDefaultForStore()
        {
            if (StoreCbx != null)
            {
                StoreID = StoreCbx.FirstOrDefault().StoreID;
            }
        }

        #region Properties Member

        public class ListDrugAndQtySell
        {
            public long GenMedProductID;
            public int xban;
        }
        private ObservableCollection<ListDrugAndQtySell> ListDrugTemp;

        private string _strNgayDung;
        public string strNgayDung
        {
            get { return _strNgayDung; }
            set
            {
                if (_strNgayDung != value)
                {
                    _strNgayDung = value;
                    NotifyOfPropertyChange(() => strNgayDung);
                }
            }
        }


        private ObservableCollection<RefStorageWarehouseLocation> _StoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> StoreCbx
        {
            get
            {
                return _StoreCbx;
            }
            set
            {
                if (_StoreCbx != value)
                {
                    _StoreCbx = value;
                    NotifyOfPropertyChange(() => StoreCbx);
                }
            }
        }

        private long _StoreID;
        public long StoreID
        {
            get { return _StoreID; }
            set
            {
                if (_StoreID != value)
                {
                    _StoreID = value;
                    NotifyOfPropertyChange(() => StoreID);
                }
            }
        }

        private string _StaffName;
        public string StaffName
        {
            get { return _StaffName; }
            set
            {
                _StaffName = value;
                NotifyOfPropertyChange(() => StaffName);
            }
        }

        private Staff GetStaffLogin()
        {
            StaffName = Globals.LoggedUserAccount.Staff.FullName;
            return Globals.LoggedUserAccount.Staff;
        }

        private SearchOutwardInfo _SearchInvoiceCriteria;
        public SearchOutwardInfo SearchInvoiceCriteria
        {
            get
            {
                return _SearchInvoiceCriteria;
            }
            set
            {
                if (_SearchInvoiceCriteria != value)
                {
                    _SearchInvoiceCriteria = value;
                }
                NotifyOfPropertyChange(() => SearchInvoiceCriteria);
            }
        }

        private PrescriptionSearchCriteria _SearchCriteria;
        public PrescriptionSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                if (_SearchCriteria != value)
                {
                    _SearchCriteria = value;
                }
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private Prescription _SelectedPrescription;
        public Prescription SelectedPrescription
        {
            get
            {
                return _SelectedPrescription;
            }
            set
            {
                if (_SelectedPrescription != value)
                {
                    _SelectedPrescription = value;
                }
                NotifyOfPropertyChange(() => SelectedPrescription);
            }
        }

        private ObservableCollection<OutwardDrugMedDept> _OutwardDrugsCopy;
        public ObservableCollection<OutwardDrugMedDept> OutwardDrugsCopy
        {
            get
            {
                return _OutwardDrugsCopy;
            }
            set
            {
                if (_OutwardDrugsCopy != value)
                {
                    _OutwardDrugsCopy = value;
                }
                NotifyOfPropertyChange(() => OutwardDrugsCopy);
            }
        }

        private OutwardDrugMedDeptInvoice _SelectedOutInvoiceCopy;
        public OutwardDrugMedDeptInvoice SelectedOutInvoiceCopy
        {
            get
            {
                return _SelectedOutInvoiceCopy;
            }
            set
            {
                if (_SelectedOutInvoiceCopy != value)
                {
                    _SelectedOutInvoiceCopy = value;
                }
                NotifyOfPropertyChange(() => SelectedOutInvoiceCopy);
            }
        }

        private OutwardDrugMedDeptInvoice _SelectedOutInvoice;
        public OutwardDrugMedDeptInvoice SelectedOutInvoice
        {
            get
            {
                return _SelectedOutInvoice;
            }
            set
            {
                if (_SelectedOutInvoice != value)
                {
                    _SelectedOutInvoice = value;
                }
                NotifyOfPropertyChange(() => SelectedOutInvoice);
            }
        }

        private Visibility _Visibility = Visibility.Visible;
        public Visibility Visibility
        {
            get
            {
                return _Visibility;
            }
            set
            {
                _Visibility = value;
                if (_Visibility == Visibility.Collapsed)
                {
                    IsVisibility = Visibility.Visible;
                }
                else
                {
                    IsVisibility = Visibility.Collapsed;
                }
                NotifyOfPropertyChange(() => Visibility);
            }
        }

        private Visibility _IsVisibility;
        public Visibility IsVisibility
        {
            get
            {
                return _IsVisibility;
            }
            set
            {
                if (_IsVisibility != value)
                {
                    _IsVisibility = value;
                }
                NotifyOfPropertyChange(() => IsVisibility);
            }
        }

        private Patient _patientInfo;
        public Patient PatientInfo
        {
            get
            {
                return _patientInfo;
            }
            set
            {
                if (_patientInfo != value)
                {
                    _patientInfo = value;
                    NotifyOfPropertyChange(() => PatientInfo);
                }
            }
        }

        private PatientRegistration _curRegistration;
        public PatientRegistration CurRegistration
        {
            get
            {
                return _curRegistration;
            }
            set
            {
                if (_curRegistration != value)
                {
                    _curRegistration = value;
                    NotifyOfPropertyChange(() => CurRegistration);
                }
            }
        }

        private bool _IsOther = false;
        public bool IsOther
        {
            get
            {
                return _IsOther;
            }
            set
            {
                _IsOther = value;
                NotifyOfPropertyChange(() => IsOther);
            }
        }


        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                if (_V_MedProductType != value)
                {
                    _V_MedProductType = value;
                    NotifyOfPropertyChange(() => V_MedProductType);
                }

            }
        }
        #endregion


        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account

        private bool _bEdit = true;
        private bool _bAdd = true;
        private bool _bDelete = true;
        private bool _bView = true;
        private bool _bPrint = true;
        private bool _bReport = true;
        private bool _bTinhTien = true;
        private bool _bLuuTinhTien = true;
        private bool _bSuaToa = true;
        public bool bSuaToa
        {
            get
            {
                return _bSuaToa;
            }
            set
            {
                if (_bSuaToa == value)
                    return;
                _bSuaToa = value;
                NotifyOfPropertyChange(() => bSuaToa);
            }
        }
        public bool bEdit
        {
            get
            {
                return _bEdit;
            }
            set
            {
                if (_bEdit == value)
                    return;
                _bEdit = value;
            }
        }
        public bool bAdd
        {
            get
            {
                return _bAdd;
            }
            set
            {
                if (_bAdd == value)
                    return;
                _bAdd = value;
            }
        }
        public bool bDelete
        {
            get
            {
                return _bDelete;
            }
            set
            {
                if (_bDelete == value)
                    return;
                _bDelete = value;
            }
        }
        public bool bView
        {
            get
            {
                return _bView;
            }
            set
            {
                if (_bView == value)
                    return;
                _bView = value;
            }
        }
        public bool bPrint
        {
            get
            {
                return _bPrint;
            }
            set
            {
                if (_bPrint == value)
                    return;
                _bPrint = value;
            }
        }
        public bool bTinhTien
        {
            get
            {
                return _bTinhTien;
            }
            set
            {
                if (_bTinhTien == value)
                    return;
                _bTinhTien = value;
            }
        }
        public bool bLuuTinhTien
        {
            get
            {
                return _bLuuTinhTien;
            }
            set
            {
                if (_bLuuTinhTien == value)
                    return;
                _bLuuTinhTien = value;
            }
        }
        public bool bReport
        {
            get
            {
                return _bReport;
            }
            set
            {
                if (_bReport == value)
                    return;
                _bReport = value;
            }
        }
        #endregion
        #region binding visibilty

        public Button lnkDelete { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bDelete);
        }

        #endregion

        #region Load Danh Sach Kho Ngoai Tru Member
        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            SetDefaultForStore();
            isLoadingGetStore = false;
            yield break;
        }

        #endregion


        private bool CheckQuantity(object outward)
        {
            try
            {
                OutwardDrugMedDept p = outward as OutwardDrugMedDept;
                if (p.RequestQty == p.OutQuantity)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            if (!CheckQuantity(e.Row.DataContext))
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        public void grdPrescription_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            SumTotalPrice();
        }


        public Nullable<double> ValueQuyenLoiBH
        {
            get
            {
                if (SelectedOutInvoice == null || SelectedOutInvoice.SelectedPrescription == null)
                {
                    return null;
                }
                if (SelectedOutInvoice.outiID == 0)
                {
                    if (SelectedOutInvoice.SelectedPrescription.IsSold)
                    {
                        return 0;
                    }
                    return CurRegistration.PtInsuranceBenefit;
                }
                else
                {
                    if (SelectedOutInvoice.IsHICount.GetValueOrDefault())
                    {
                        return CurRegistration.PtInsuranceBenefit;
                    }
                    return 0;
                }
            }
        }

        int DayRpts = 0;

        //KMx: Khi load phiếu xuất cũ (bán thuốc theo toa) thì mới gọi hàm này. Còn trong phần cập nhật toa thuốc không có load phiếu xuất cũ nên hàm này không có sử dụng.
        //Nhưng do trong interface đã khai báo hàm này nên không comment.
        public void GetClassicationPatientInvoice()
        {
            //Kiên thêm đk SelectedOutInvoice.IsHICount == true.
            //Khi cập nhật thì PrescriptionViewModel sẽ truyền SelectedOutInvoice muốn cập nhật qua đây, phải dựa vào IsHICount của phiếu được truyền qua.
            //Nếu phiếu đó được tính BH thì phiếu sau khi cập nhật sẽ được tính BH, ngược lại thì không.
            if (CurRegistration != null && CurRegistration.HealthInsurance != null && CurRegistration.HealthInsurance.ValidDateTo >= Globals.GetCurServerDateTime().Date
                 && SelectedOutInvoice != null && SelectedOutInvoice.SelectedPrescription != null && SelectedOutInvoice.IsHICount == true)
            {
                TimeSpan t = CurRegistration.HealthInsurance.ValidDateTo.GetValueOrDefault() - Globals.GetCurServerDateTime().Date;
                DayRpts = Convert.ToInt32(t.TotalDays + 1);
                Visibility = Visibility.Collapsed;
                IsHIPatient = true;
            }
            else
            {
                //0:benh nhan thong thuong,1:benh nhan bao hiem
                DayRpts = 0;
                Visibility = Visibility.Visible;
                IsHIPatient = false;
            }
            if (SelectedOutInvoice != null)
            {
                SelectedOutInvoice.IsHICount = IsHIPatient;
            }
        }

        private void HideShowColumns(DataGrid dg)
        {
            //if (dg == null)
            //{
            //    return;
            //}
            //if (!IsHIPatient)
            //{
            //    dg.Columns[1].Visibility = Visibility.Collapsed;
            //    dg.Columns[11].Visibility = Visibility.Collapsed;
            //    dg.Columns[12].Visibility = Visibility.Collapsed;
            //    dg.Columns[14].Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    dg.Columns[1].Visibility = Visibility.Visible;
            //    dg.Columns[11].Visibility = Visibility.Visible;
            //    dg.Columns[12].Visibility = Visibility.Visible;
            //    dg.Columns[14].Visibility = Visibility.Visible;
            //}
        }

        private bool CheckValid()
        {
            bool result = true;
            string strError = "";
            if (SelectedOutInvoice != null)
            {
                if (Services.Core.AxHelper.CompareDate((DateTime)SelectedPrescription.IssuedDateTime.GetValueOrDefault(), SelectedOutInvoice.OutDate.GetValueOrDefault()) == 1)
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0835_G1_NgXuatKgNhoHonNgRaToa), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                if (Services.Core.AxHelper.CompareDate(SelectedOutInvoice.OutDate.GetValueOrDefault(), Globals.GetCurServerDateTime()) == 1)
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0836_G1_NgXuatKgLonHonNgHTai), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                if (SelectedOutInvoice.OutwardDrugMedDepts == null)
                {
                    MessageBox.Show(eHCMSResources.A0928_G1_Msg_InfoPhXuatKhDcDeTrong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                if (SelectedOutInvoice.OutwardDrugMedDepts.Count == 0)
                {
                    MessageBox.Show(eHCMSResources.A0928_G1_Msg_InfoPhXuatKhDcDeTrong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                foreach (OutwardDrugMedDept item in SelectedOutInvoice.OutwardDrugMedDepts)
                {
                    //KMx: Khoa Dược yêu cầu không có giá bán vẫn cho lưu (hàng tặng) (01/07/2016 10:27).
                    //if (item.OutPrice <= 0)
                    //{
                    //    MessageBox.Show(eHCMSResources.A0525_G1_Msg_InfoGiaBanLonHon0, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    //    result = false;
                    //    break;
                    //}
                    if (item.OutQuantity <= 0)
                    {
                        MessageBox.Show(eHCMSResources.Z0837_G1_SLgXuatMoiDongLonHon0, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        result = false;
                        break;
                    }
                    if (item.Validate() == false)
                    {
                        MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0838_G1_DLieuKgHopLe), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        result = false;
                        break;
                    }
                    //neu ngay het han lon hon ngay hien tai
                    if (Services.Core.AxHelper.CompareDate(Globals.GetCurServerDateTime(), item.InExpiryDate.GetValueOrDefault()) == 1)
                    {
                        strError += item.GetGenMedProductForSell.BrandName + string.Format(" {0}.", eHCMSResources.Z0868_G1_DaHetHanDung) + Environment.NewLine;
                    }
                }
                if (!string.IsNullOrEmpty(strError))
                {
                    if (MessageBox.Show(strError + Environment.NewLine + eHCMSResources.I0939_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return result;
        }

        private void SaveDrugs(OutwardDrugMedDeptInvoice Invoice)
        {
            isLoadingFullOperator3 = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginUpdateOutwardInvoice(Invoice, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            OutwardDrugMedDeptInvoice OutInvoice;
                            contract.EndUpdateOutwardInvoice(out OutInvoice, asyncResult);
                            //phat su kien de form o duoi load lai du lieu 
                            if (OutInvoice != null && OutInvoice.outiID > 0)
                            {
                                Globals.EventAggregator.Publish(new MedDeptCloseEditPrescription { SelectedOutwardInvoice = OutInvoice });
                                TryClose();
                            }
                            else
                            {
                                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0272_G1_Msg_InfoCNhatFail));
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            isLoadingFullOperator3 = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        //KMx: 2 hàm GetValueFromAnonymousType(), CalScheduleQty() giống với 2 hàm cùng tên ở PrescriptionViewModel (08/06/2014 17:41).
        //Ban đầu tính đem 2 hàm này sang Globals để 2 ViewModels sử dụng chung. Nhưng đem qua Globals thì bị lỗi ở dòng (T)type.GetProperty(itemKey).GetValue(dataItem, null);
        public static T GetValueFromAnonymousType<T>(object dataItem, string itemKey)
        {
            System.Type type = dataItem.GetType();

            T itemValue = (T)type.GetProperty(itemKey).GetValue(dataItem, null);

            return itemValue;
        }

        public static float CalScheduleQty(object item, int nNumDay)
        {
            if (item == null)
            {
                return 0;
            }

            float[] WeeklySchedule = new float[7];
            byte? SchedBeginDOW;
            double DispenseVolume;

            SchedBeginDOW = GetValueFromAnonymousType<byte?>(item, "SchedBeginDOW");

            DispenseVolume = GetValueFromAnonymousType<double>(item, "DispenseVolume");

            WeeklySchedule[0] += GetValueFromAnonymousType<float>(item, "QtySchedMon");
            WeeklySchedule[1] += GetValueFromAnonymousType<float>(item, "QtySchedTue");
            WeeklySchedule[2] += GetValueFromAnonymousType<float>(item, "QtySchedWed");
            WeeklySchedule[3] += GetValueFromAnonymousType<float>(item, "QtySchedThu");
            WeeklySchedule[4] += GetValueFromAnonymousType<float>(item, "QtySchedFri");
            WeeklySchedule[5] += GetValueFromAnonymousType<float>(item, "QtySchedSat");
            WeeklySchedule[6] += GetValueFromAnonymousType<float>(item, "QtySchedSun");

            return Globals.CalcWeeklySchedulePrescription(SchedBeginDOW, nNumDay, WeeklySchedule, (float)DispenseVolume);
        }


        //KMx: Hàm này được viết lại từ hàm CheckInsurance. Lý do đổi cách tính thuốc lịch (08/06/2014 15:54)
        private bool CheckInsurance_New(int BHValidDays)
        {
            if (SelectedPrescription == null)
            {
                return true;
            }
            if (SelectedOutInvoice == null)
            {
                return true;
            }
            if (ListDrugTemp == null)
            {
                ListDrugTemp = new ObservableCollection<ListDrugAndQtySell>();
            }
            ListDrugTemp.Clear();

            var hhh = from hd in SelectedOutInvoice.OutwardDrugMedDepts
                      where hd.GetGenMedProductForSell.InsuranceCover == true && hd.ChargeableItem.HIAllowedPrice > 0
                      group hd by new
                      {
                          hd.GenMedProductID,
                          hd.DayRpts,
                          hd.V_DrugType,
                          hd.QtyForDay,
                          hd.QtyMaxAllowed,
                          hd.QtySchedMon,
                          hd.QtySchedTue,
                          hd.QtySchedWed,
                          hd.QtySchedThu,
                          hd.QtySchedFri,
                          hd.QtySchedSat,
                          hd.QtySchedSun,
                          hd.SchedBeginDOW,
                          hd.DispenseVolume,
                          hd.GetGenMedProductForSell.InsuranceCover,
                          hd.GetGenMedProductForSell.BrandName
                      } into hdgroup

                      select new
                      {
                          RequestQty = hdgroup.Sum(groupItem => groupItem.RequestQty),
                          OutQuantity = hdgroup.Sum(groupItem => groupItem.OutQuantity),
                          GenMedProductID = hdgroup.Key.GenMedProductID,
                          DayRpts = hdgroup.Key.DayRpts,
                          V_DrugType = hdgroup.Key.V_DrugType,
                          QtyForDay = hdgroup.Key.QtyForDay,

                          QtyMaxAllowed = hdgroup.Key.QtyMaxAllowed,
                          QtySchedMon = hdgroup.Key.QtySchedMon,
                          QtySchedTue = hdgroup.Key.QtySchedTue,
                          QtySchedWed = hdgroup.Key.QtySchedWed,
                          QtySchedThu = hdgroup.Key.QtySchedThu,
                          QtySchedFri = hdgroup.Key.QtySchedFri,
                          QtySchedSat = hdgroup.Key.QtySchedSat,
                          QtySchedSun = hdgroup.Key.QtySchedSun,
                          SchedBeginDOW = hdgroup.Key.SchedBeginDOW,
                          DispenseVolume = hdgroup.Key.DispenseVolume,

                          InsuranceCover = hdgroup.Key.InsuranceCover,
                          BrandName = hdgroup.Key.BrandName
                      };

            string strError = "";
            int xNgayToiDa = 30;

            xNgayToiDa = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NoiTru;

            //Cảnh báo cho thuốc Cần.
            string strDrugsNotHaveDayRpts = "";
            //Những thuốc mà hết hạn bảo hiểm. VD: Bác sĩ kê 10 ngày, nhưng bảo hiểm chỉ còn hiệu lực 5 ngày.
            string strInvalidDrugs = "";

            for (int i = 0; i < hhh.Count(); i++)
            {
                //QtyMaxAllowed: Số lượng tối đa được bán (Minimum của SLYC và SL BH cho phép).
                //QtyHIValidateTo: Số lượng thuốc tính đến ngày BH hết hạn. Nếu SL bán > SL tính đến hết ngày BH hết hạn thì cảnh báo.
                //RequestQty: Số lượng bác sĩ yêu cầu. Bác sĩ yêu cầu bao nhiêu viên thì đưa lên bấy nhiêu.

                int QtyMaxAllowed = 0;
                int QtyHIValidateTo = 0;

                string strReasonCannotSell;

                if (hhh.ToList()[i].RequestQty <= 0)
                {
                    MessageBox.Show(string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + string.Format(" {0}.", eHCMSResources.K0015_G1_ThuocKhongCoSLgYC), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }

                QtyMaxAllowed = Convert.ToInt32(hhh.ToList()[i].QtyMaxAllowed);

                //KMx: Nếu là thuốc Cần thì so sánh "số lượng xuất" và "số lượng được bán tối đa."
                if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN)
                {
                    if (hhh.ToList()[i].OutQuantity > QtyMaxAllowed)
                    {
                        strReasonCannotSell = eHCMSResources.Z0874_G1_SLgBanVuotYC;
                        strError += string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + string.Format(". {0} ", eHCMSResources.Z0875_G1_SLgBSiKe) + hhh.ToList()[i].RequestQty + Environment.NewLine + strReasonCannotSell + Environment.NewLine;
                        ListDrugAndQtySell item = new ListDrugAndQtySell();
                        item.GenMedProductID = hhh.ToList()[i].GenMedProductID.GetValueOrDefault();
                        item.xban = QtyMaxAllowed;
                        ListDrugTemp.Add(item);
                    }
                    else
                    {
                        strDrugsNotHaveDayRpts += hhh.ToList()[i].BrandName + Environment.NewLine;
                    }
                }

                //KMx: Nếu là thuốc Thường hoặc thuốc Lịch.
                else
                {
                    //KMx: QtyHIValidateTo: Số lượng tính đến ngày BH còn hiệu lực.
                    if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG)
                    {
                        //KMx: Phải Round trước rồi mới Ceiling sau, nếu không sẽ bị sai trong trường hợp kết quả có nhiều số lẻ. VD: 5.00001
                        double QtyRounded = Math.Round((float)hhh.ToList()[i].QtyForDay * BHValidDays, 2);
                        QtyHIValidateTo = Convert.ToInt32(Math.Ceiling(QtyRounded));
                    }

                    if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
                    {
                        QtyHIValidateTo = Convert.ToInt32(Math.Ceiling(CalScheduleQty(hhh.ToList()[i], BHValidDays)));
                    }

                    //Nếu số lượng muốn bán > số lượng mà bảo hiểm còn hiệu lực. VD: Bác sĩ kê 10 ngày, nhưng bảo hiểm chỉ còn hiệu lực 5 ngày.
                    if (hhh.ToList()[i].OutQuantity > QtyHIValidateTo)
                    {
                        strInvalidDrugs += hhh.ToList()[i].BrandName + Environment.NewLine;
                    }

                    //Lấy Min của SLYC và SL tối đa để khống chế số lượng thực xuất.
                    int QtyAllowSell = Math.Min((int)hhh.ToList()[i].RequestQty, QtyMaxAllowed);

                    if (hhh.ToList()[i].OutQuantity > QtyAllowSell)
                    {
                        if (hhh.ToList()[i].RequestQty > QtyMaxAllowed)
                        {
                            strReasonCannotSell = string.Format(eHCMSResources.Z0876_G1_BHChiTraToiDa, xNgayToiDa.ToString());
                            strError += string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + string.Format(". {0} ", eHCMSResources.Z0877_G1_SLgDuocBan) + QtyMaxAllowed + Environment.NewLine + strReasonCannotSell + Environment.NewLine;
                        }
                        else
                        {
                            strReasonCannotSell = eHCMSResources.Z0874_G1_SLgBanVuotYC;
                            strError += string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + string.Format(". {0} ", eHCMSResources.Z0875_G1_SLgBSiKe) + hhh.ToList()[i].RequestQty + Environment.NewLine + strReasonCannotSell + Environment.NewLine;
                        }

                        ListDrugAndQtySell item = new ListDrugAndQtySell();
                        item.GenMedProductID = hhh.ToList()[i].GenMedProductID.GetValueOrDefault();
                        item.xban = QtyAllowSell;
                        ListDrugTemp.Add(item);
                    }
                }
            }

            if (!string.IsNullOrEmpty(strError))
            {
                if (MessageBox.Show(strError + Environment.NewLine + eHCMSResources.I0940_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return false;
                }
                //sua lai so luong xuat cho hop li
                for (int i = 0; i < ListDrugTemp.Count; i++)
                {
                    var values = SelectedOutInvoice.OutwardDrugMedDepts.Where(x => x.GenMedProductID == ListDrugTemp[i].GenMedProductID).OrderBy(x => x.InExpiryDate);

                    if (values == null || values.Count() <= 0)
                    {
                        continue;
                    }

                    int xban = ListDrugTemp[i].xban;

                    foreach (OutwardDrugMedDept p in values)
                    {
                        if (xban <= 0)
                        {
                            DeleteOutwardDrugs(p);
                            continue;
                        }

                        if (p.OutQuantity <= xban)
                        {
                            if (p.RequestQty <= xban)
                            {
                                p.OutQuantity = p.RequestQty;
                            }
                            xban = xban - (int)p.OutQuantity;
                        }
                        else //p.OutQuantity > xban
                        {
                            p.OutQuantity = xban;
                            xban = 0;
                        }
                    }
                }

                SumTotalPrice();
                return false;
            }

            if (!string.IsNullOrEmpty(strInvalidDrugs))
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z0878_G1_BHBNConHieuLuc0Ng, BHValidDays) + Environment.NewLine + string.Format("{0}: ", eHCMSResources.Z0879_G1_NhungThuoc) + Environment.NewLine + strInvalidDrugs + string.Format("{0}.", eHCMSResources.Z0880_G1_BanHonSoNgBH) + Environment.NewLine + eHCMSResources.Z0881_G1_CoDongYBanKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(strDrugsNotHaveDayRpts))
            {
                if (MessageBox.Show(string.Format("{0}: ", eHCMSResources.Z0882_G1_I) + Environment.NewLine + Environment.NewLine + strDrugsNotHaveDayRpts + Environment.NewLine + eHCMSResources.Z0881_G1_CoDongYBanKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }

            return true;
        }


        public void btnNgayDung_New()
        {
            if (SelectedPrescription == null)
            {
                return;
            }

            if (SelectedOutInvoice == null)
            {
                return;
            }

            int NgayDung = 0;

            if (!Int32.TryParse(strNgayDung, out NgayDung))
            {
                MessageBox.Show(eHCMSResources.A0836_G1_Msg_InfoNgDungKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (NgayDung <= 0)
            {
                MessageBox.Show(eHCMSResources.A0837_G1_Msg_InfoNgDungLonHon0);
                return;
            }

            if (IsHIPatient) //toa bao hiem
            {
                int xNgayBHToiDa_NoiTru = 5;

                xNgayBHToiDa_NoiTru = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NoiTru;

                //KMx: Lấy thuốc có ngày dùng nhỏ nhất. Nếu ngày dùng muốn cập nhật > ngày dùng nhỏ nhất thì không cho cập nhật.
                var DrugMinDayRpts = SelectedOutInvoice.OutwardDrugMedDepts.Where(x => x.DayRpts > 0).OrderBy(x => x.DayRpts).Take(1);

                if (NgayDung > xNgayBHToiDa_NoiTru)
                {
                    MessageBox.Show(string.Format("{0} ", eHCMSResources.A0207_G1_Msg_InfoBHTraToiDa) + xNgayBHToiDa_NoiTru + string.Format(" {0}.", eHCMSResources.Z0883_G1_NhapNgDung), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return;
                }

                foreach (OutwardDrugMedDept d in DrugMinDayRpts)
                {
                    if (NgayDung > d.DayRpts)
                    {
                        MessageBox.Show(string.Format("{0} ", eHCMSResources.A0080_G1_Msg_SubBacSiYCThuoc) + d.GetGenMedProductForSell.BrandName + " (" + d.DayRpts + string.Format(" {0}).", eHCMSResources.N0045_G1_Ng) + Environment.NewLine + string.Format("{0}.", eHCMSResources.Z0884_G1_KgDuocCNhatNgDung), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        return;
                    }
                }

            }


            if (GetGenMedProductForSellList == null)
            {
                GetGenMedProductForSellList = new ObservableCollection<GetGenMedProductForSell>();
            }
            else
            {
                GetGenMedProductForSellList.Clear();
            }

            //dung de lay ton hien tai va ton ban dau cua tung thuoc theo lo
            ObservableCollection<GetGenMedProductForSell> temp = GetGenMedProductForSellListByPrescriptID.DeepCopy();
            LaySoLuongTheoNgayDung(temp);

            //dung de lay ton hien tai va ton ban dau cua tung thuoc da duoc sum
            var ObjSumRemaining = from hd in GetGenMedProductForSellList
                                  group hd by new { hd.GenMedProductID, hd.BrandName } into hdgroup
                                  select new
                                  {
                                      Remaining = hdgroup.Sum(groupItem => groupItem.Remaining),
                                      RemainingFirst = hdgroup.Sum(groupItem => groupItem.RemainingFirst),
                                      GenMedProductID = hdgroup.Key.GenMedProductID,
                                      BrandName = hdgroup.Key.BrandName
                                  };

            //lay tong so luong yc cua cac thuoc co trong toa (dua vao ngay dung > 0) 
            var hhh = from hd in SelectedOutInvoice.OutwardDrugMedDepts
                      where hd.DayRpts > 0 && hd.V_DrugType != (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN
                      group hd by new
                      {
                          hd.GenMedProductID,
                          hd.DayRpts,
                          hd.V_DrugType,
                          hd.QtyForDay,
                          hd.QtyMaxAllowed,
                          hd.QtySchedMon,
                          hd.QtySchedTue,
                          hd.QtySchedWed,
                          hd.QtySchedThu,
                          hd.QtySchedFri,
                          hd.QtySchedSat,
                          hd.QtySchedSun,
                          hd.SchedBeginDOW,
                          hd.DispenseVolume,
                          hd.GetGenMedProductForSell.BrandName
                      } into hdgroup
                      select new
                      {
                          RequestQty = hdgroup.Sum(groupItem => groupItem.RequestQty),
                          OutQuantity = hdgroup.Sum(groupItem => groupItem.OutQuantity),
                          GenMedProductID = hdgroup.Key.GenMedProductID,
                          DayRpts = hdgroup.Key.DayRpts,
                          V_DrugType = hdgroup.Key.V_DrugType,
                          QtyForDay = hdgroup.Key.QtyForDay,

                          QtyMaxAllowed = hdgroup.Key.QtyMaxAllowed,
                          QtySchedMon = hdgroup.Key.QtySchedMon,
                          QtySchedTue = hdgroup.Key.QtySchedTue,
                          QtySchedWed = hdgroup.Key.QtySchedWed,
                          QtySchedThu = hdgroup.Key.QtySchedThu,
                          QtySchedFri = hdgroup.Key.QtySchedFri,
                          QtySchedSat = hdgroup.Key.QtySchedSat,
                          QtySchedSun = hdgroup.Key.QtySchedSun,
                          SchedBeginDOW = hdgroup.Key.SchedBeginDOW,
                          DispenseVolume = hdgroup.Key.DispenseVolume,

                          BrandName = hdgroup.Key.BrandName
                      };

            string ThongBao = "";
            //Lưu ý: Khi SelectedOutInvoice.OutwardDrugMedDepts add thêm 1 đối tượng (OutwardDrugMedDept) thì hhh.Count() tự động tăng lên 1.
            for (int i = 0; i < hhh.Count(); i++)
            {
                int QtyWillChange = 0;

                if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG)
                {
                    //KMx: Sau khi lấy QtyForDay * NgayDung thì phải Round lại rồi sau đó mới Ceiling.
                    //Nếu không sẽ bị sai trong trường hợp thuốc có QtyForDay = 0.1666667. Khi nhân lên 30 ngày sẽ ra kết quả 5.00001 và Ceiling liền thì sẽ ra 6.
                    double QtyRounded = Math.Round((double)hhh.ToList()[i].QtyForDay * NgayDung, 2);
                    QtyWillChange = Convert.ToInt32(Math.Ceiling(QtyRounded));
                }

                if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
                {
                    QtyWillChange = Convert.ToInt32(Math.Ceiling(CalScheduleQty(hhh.ToList()[i], NgayDung)));
                }

                //KMx: Dòng thuốc có số lượng muốn thay đổi bằng 0 thì sẽ xóa dòng thuốc đó đi (14/06/2014 16:54)
                //if (QtyWillChange <= 0)
                //{
                //    continue;
                //}

                var values = SelectedOutInvoice.OutwardDrugMedDepts.Where(x => x.GenMedProductID == hhh.ToList()[i].GenMedProductID.GetValueOrDefault()).OrderBy(x => x.InExpiryDate);

                if (values == null || values.Count() <= 0)
                {
                    continue;
                }

                //Neu so luong < So luong hien co tren luoi thi chi can ham ben duoi
                foreach (OutwardDrugMedDept p in values)
                {
                    if (QtyWillChange <= 0)
                    {
                        DeleteOutwardDrugs(p);
                    }
                    else if (p.OutQuantity > QtyWillChange)
                    {
                        p.OutQuantity = QtyWillChange;
                        QtyWillChange = 0;
                    }
                    else if (p.OutQuantity <= QtyWillChange)
                    {
                        QtyWillChange = QtyWillChange - (int)p.OutQuantity;
                    }
                }

                if (QtyWillChange <= 0)
                {
                    continue;
                }

                //else neu > so luong hien co tren luoi thi them vao

                var Obj = ObjSumRemaining.Where(x => x.GenMedProductID == hhh.ToList()[i].GenMedProductID.GetValueOrDefault());
                if (Obj != null && Obj.Count() > 0)
                {
                    if (Obj.ToArray()[0].Remaining > 0 && Obj.ToArray()[0].Remaining >= QtyWillChange)
                    {
                        GetGenMedProductForSell item = new GetGenMedProductForSell();
                        item.GenMedProductID = hhh.ToList()[i].GenMedProductID.GetValueOrDefault();
                        item.BrandName = hhh.ToList()[i].BrandName;
                        //KMx: Phải assign Remaining trước RequiredNumber. Nếu không sẽ bị lỗi, vì khi assign RequiredNumber thì nó sẽ so sánh với Remaining.
                        item.Remaining = Obj.ToArray()[0].Remaining;
                        item.RequiredNumber = QtyWillChange;
                        item.Qty = (int)hhh.ToList()[i].RequestQty;
                        //item.Remaining = Obj.ToArray()[0].Remaining;

                        ReCountQtyAndAddList(item);
                    }
                    else
                    {
                        if (Obj.ToArray()[0].RemainingFirst > 0)
                        {
                            ThongBao = ThongBao + string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + Obj.ToArray()[0].BrandName + string.Format(": {0} ", eHCMSResources.Z0885_G1_SLgCanBan) + QtyWillChange.ToString() + string.Format(", {0} ", eHCMSResources.Z0886_G1_SLgConLai) + Obj.ToArray()[0].RemainingFirst.ToString() + Environment.NewLine;
                        }
                        else
                        {
                            ThongBao = ThongBao + string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + Obj.ToArray()[0].BrandName + string.Format(": {0} ", eHCMSResources.Z0885_G1_SLgCanBan) + QtyWillChange.ToString() + string.Format(", {0} ", eHCMSResources.Z0886_G1_SLgConLai) + "0" + Environment.NewLine;
                        }
                    }
                }
                else
                {
                    ThongBao = ThongBao + string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + string.Format(" {0}!", eHCMSResources.Z0887_G1_DaHet) + Environment.NewLine;
                }

            }
            //KMx: Nếu để dòng dưới thì sẽ bị lỗi khi thêm cùng 1 loại thuốc có trong toa nhưng khác lô.
            //SelectedOutInvoice.OutwardDrugMedDepts = SelectedOutInvoice.OutwardDrugMedDepts.ToObservableCollection();
            SumTotalPrice();
            if (!string.IsNullOrEmpty(ThongBao))
            {
                MessageBox.Show(ThongBao);
            }
        }


        private void SaveCmd()
        {
            //phai cho biet day la ham cap nhat hay them moi de khoi cap lai so
            isLoadingFullOperator2 = true;
            //true la save xong roi thu tien, false thi binh thuong : chi luu thoi
            if (SelectedOutInvoiceNew != null)
            {
                SelectedOutInvoiceNew.OutDate = SelectedOutInvoice.OutDate;
                //Kiên thêm ModFromOutiID để biết phiếu phiếu mới vừa cập nhật được tạo ra từ phiếu nào.
                SelectedOutInvoiceNew.ModFromOutiID = SelectedOutInvoice.outiID;
                //tam thoi huy roi them moi lai 
                //SelectedOutInvoiceNew.RecordState = RecordState.DELETED;
                SelectedOutInvoiceNew.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                //KMx: Phải giữ outiID để xuống DB hủy phiếu cũ, tạo phiếu mới (29/11/2014 17:30).
                //SelectedOutInvoiceNew.outiID = 0;
                SelectedOutInvoiceNew.IssueID = SelectedPrescription.IssueID;
                SelectedOutInvoiceNew.SelectedPrescription = SelectedPrescription;

                SaveDrugs(SelectedOutInvoiceNew);
            }
            isLoadingFullOperator2 = false;
        }

        private bool flag = true;
        public void btnClick_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                if (flag)
                {
                    btnSaveMoney();
                }
            }
        }

        private OutwardDrugMedDeptInvoice SelectedOutInvoiceNew;
        private void btnSaveMoney()
        {
            isLoadingFullOperator = true;
            if (CheckValid())
            {
                if (SelectedPrescription != null)
                {
                    SelectedOutInvoice.PrescriptID = SelectedPrescription.PrescriptID;
                    SelectedOutInvoice.IssueID = SelectedPrescription.IssueID;
                    SelectedOutInvoice.PtRegistrationID = SelectedPrescription.PtRegistrationID;
                    SelectedOutInvoice.SelectedStorage = new RefStorageWarehouseLocation();
                    SelectedOutInvoice.SelectedStorage.StoreID = StoreID;
                    SelectedOutInvoice.SelectedStaff = GetStaffLogin();
                    SelectedOutInvoice.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();

                    SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                    if (IsHIPatient)
                    {
                        //if (CheckInsurance(DayRpts))
                        if (CheckInsurance_New(DayRpts))
                        {
                            UpdateInvoice();
                        }
                    }
                    else
                    {
                        UpdateInvoice();
                    }
                }
            }
            isLoadingFullOperator = false;
            flag = true;

        }

        private void UpdateInvoice()
        {
            SelectedOutInvoiceNew = SelectedOutInvoice.DeepCopy();
            //KMx: Khoa Dược không sử dụng STT
            //SelectedOutInvoiceNew.ColectDrugSeqNumType = SelectedOutInvoice.ColectDrugSeqNumType;
            //SelectedOutInvoiceNew.ColectDrugSeqNum = SelectedOutInvoice.ColectDrugSeqNum;
            //SelectedOutInvoiceNew.IsUpdate = true;
            SelectedOutInvoiceNew.SelectedStaff = GetStaffLogin();

            SaveCmd();
        }
        private bool Equal(OutwardDrugMedDept a, OutwardDrugMedDept b)
        {
            if (a.InID == b.InID && a.GenMedProductID == b.GenMedProductID && a.InBatchNumber == b.InBatchNumber && a.InExpiryDate == b.InExpiryDate
                && a.OutPrice == b.OutPrice && a.OutQuantity == b.OutQuantity && a.HIAllowedPrice == b.HIAllowedPrice)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        //Kiên: Thêm sự kiện: Nếu thay đổi ngày thì hiện lên nút "Lưu".
        public bool dateTimePickerChanged()
        {
            //Nếu chọn ngày khác với ngày xuất hoặc thay đổi thuốc, số lượng thì hiện nút "Lưu"
            if (SelectedOutInvoiceCopy.OutDate != SelectedOutInvoice.OutDate || !Compare2Object())
            {
                IsOther = true;
                return true;
            }
            IsOther = false;
            return false;
        }

        private bool Compare2Object()
        {
            if (SelectedOutInvoiceCopy.OutwardDrugMedDepts != null && SelectedOutInvoice.OutwardDrugMedDepts.Count == SelectedOutInvoiceCopy.OutwardDrugMedDepts.Count && SelectedOutInvoice.OutwardDrugMedDepts.Count > 0)
            {
                int icount = 0;
                for (int i = 0; i < SelectedOutInvoiceCopy.OutwardDrugMedDepts.Count; i++)
                {
                    for (int j = 0; j < SelectedOutInvoice.OutwardDrugMedDepts.Count; j++)
                    {
                        if (Equal(SelectedOutInvoiceCopy.OutwardDrugMedDepts[i], SelectedOutInvoice.OutwardDrugMedDepts[j]))
                        {
                            icount++;
                        }
                    }
                }
                if (icount == SelectedOutInvoiceCopy.OutwardDrugMedDepts.Count)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        #region Total Price Member
        private decimal _TotalInvoicePrice;
        public decimal TotalInvoicePrice
        {
            get
            {
                return _TotalInvoicePrice;
            }
            set
            {
                if (_TotalInvoicePrice != value)
                {
                    _TotalInvoicePrice = value;
                    NotifyOfPropertyChange(() => TotalInvoicePrice);
                }
            }
        }

        private decimal _TotalHIPayment;
        public decimal TotalHIPayment
        {
            get
            {
                return _TotalHIPayment;
            }
            set
            {
                if (_TotalHIPayment != value)
                {
                    _TotalHIPayment = value;
                    NotifyOfPropertyChange(() => TotalHIPayment);
                }
            }
        }
        private decimal _TotalPatientPayment;
        public decimal TotalPatientPayment
        {
            get
            {
                return _TotalPatientPayment;
            }
            set
            {
                if (_TotalPatientPayment != value)
                {
                    _TotalPatientPayment = value;
                    NotifyOfPropertyChange(() => TotalPatientPayment);
                }
            }
        }
        private void SumTotalPrice()
        {
            if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugMedDepts == null)
            {
                return;
            }

            if (dateTimePickerChanged() || !Compare2Object())
            {
                IsOther = true;
            }
            else
            {
                IsOther = false;
            }

            //KMx: OnlyRoundResultForOutward = true: Tính tổng tiền BH trả rồi mới làm tròn. Ngược lại thì làm tròn tiền BH trả trên từng dòng thuốc rồi mới tính tổng(02/08/2014 18:24).
            bool onlyRoundResultForOutward = Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward;

            double HIBenefit = 0;
            if (CurRegistration != null)
            {
                HIBenefit = CurRegistration.PtInsuranceBenefit.GetValueOrDefault();
            }
            TotalInvoicePrice = 0;
            TotalHIPayment = 0;
            TotalPatientPayment = 0;

            if (!onlyRoundResultForOutward)
            {
                for (int i = 0; i < SelectedOutInvoice.OutwardDrugMedDepts.Count; i++)
                {
                    if (SelectedOutInvoice.OutwardDrugMedDepts[i].InwardDrugMedDept != null && SelectedOutInvoice.OutwardDrugMedDepts[i].InwardDrugMedDept.HIAllowedPrice.GetValueOrDefault(0) > 0)
                    {
                        SelectedOutInvoice.OutwardDrugMedDepts[i].PriceDifference = SelectedOutInvoice.OutwardDrugMedDepts[i].OutPrice - SelectedOutInvoice.OutwardDrugMedDepts[i].InwardDrugMedDept.HIAllowedPrice.GetValueOrDefault(0);
                    }
                    SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment = SelectedOutInvoice.OutwardDrugMedDepts[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugMedDepts[i].OutQuantity * (decimal)HIBenefit;
                    SelectedOutInvoice.OutwardDrugMedDepts[i].TotalPatientPayment = SelectedOutInvoice.OutwardDrugMedDepts[i].TotalPrice - SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment;
                    if (SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment > 0)
                    {
                        SelectedOutInvoice.OutwardDrugMedDepts[i].TotalCoPayment = SelectedOutInvoice.OutwardDrugMedDepts[i].TotalPrice - SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment - (SelectedOutInvoice.OutwardDrugMedDepts[i].PriceDifference * SelectedOutInvoice.OutwardDrugMedDepts[i].OutQuantity);
                    }

                    TotalInvoicePrice += SelectedOutInvoice.OutwardDrugMedDepts[i].TotalPrice;
                    TotalHIPayment += SelectedOutInvoice.OutwardDrugMedDepts[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugMedDepts[i].OutQuantity * (decimal)HIBenefit;

                }
                TotalPatientPayment = TotalInvoicePrice - TotalHIPayment;
            }
            else
            {
                for (int i = 0; i < SelectedOutInvoice.OutwardDrugMedDepts.Count; i++)
                {
                    //if (SelectedOutInvoice.OutwardDrugMedDepts[i].InwardDrugMedDept != null && SelectedOutInvoice.OutwardDrugMedDepts[i].InwardDrugMedDept.HIAllowedPrice.GetValueOrDefault(0) > 0)
                    //{
                    //    SelectedOutInvoice.OutwardDrugMedDepts[i].PriceDifference = SelectedOutInvoice.OutwardDrugMedDepts[i].OutPrice - SelectedOutInvoice.OutwardDrugMedDepts[i].InwardDrugMedDept.HIAllowedPrice.GetValueOrDefault(0);
                    //}

                    if (SelectedOutInvoice.OutwardDrugMedDepts[i].HIAllowedPrice.GetValueOrDefault() > 0)
                    {
                        SelectedOutInvoice.OutwardDrugMedDepts[i].HIBenefit = HIBenefit;
                    }

                    //KMx: Phải tính TotalPrice, vì bên nhà thuốc, khi set OutQuantity thì chương trình tự tính TotalPrice, còn khoa Dược thì không (29/11/2014 17:21).
                    SelectedOutInvoice.OutwardDrugMedDepts[i].TotalPrice = SelectedOutInvoice.OutwardDrugMedDepts[i].OutPrice * SelectedOutInvoice.OutwardDrugMedDepts[i].OutQuantity;


                    SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment = SelectedOutInvoice.OutwardDrugMedDepts[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugMedDepts[i].OutQuantity * (decimal)HIBenefit;
                    SelectedOutInvoice.OutwardDrugMedDepts[i].TotalPatientPayment = SelectedOutInvoice.OutwardDrugMedDepts[i].TotalPrice - SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment;

                    if (SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment > 0)
                    {
                        SelectedOutInvoice.OutwardDrugMedDepts[i].TotalCoPayment = SelectedOutInvoice.OutwardDrugMedDepts[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugMedDepts[i].OutQuantity - SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment;
                    }

                    TotalInvoicePrice += SelectedOutInvoice.OutwardDrugMedDepts[i].TotalPrice;
                    TotalHIPayment += SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment;

                }
                TotalInvoicePrice = MathExt.Round(TotalInvoicePrice, aEMR.Common.Converters.MidpointRounding.AwayFromZero);
                TotalHIPayment = MathExt.Round(TotalHIPayment, aEMR.Common.Converters.MidpointRounding.AwayFromZero);

                TotalPatientPayment = TotalInvoicePrice - TotalHIPayment;

            }

        }
        #endregion

        DataGrid grdPrescription = null;
        public void grdPrescription_Loaded(object sender, RoutedEventArgs e)
        {
            grdPrescription = sender as DataGrid;
        }

        public void GridInward_Unloaded(object sender, RoutedEventArgs e)
        {
            grdPrescription.SetValue(DataGrid.ItemsSourceProperty, null);
        }


        #region auto Drug For Prescription member

        public void RadioButton1_Checked(object sender, RoutedEventArgs e)
        {
            HI = 0;
        }
        public void RadioButton2_Checked(object sender, RoutedEventArgs e)
        {
            HI = 1;
        }
        public void RadioButton3_Checked(object sender, RoutedEventArgs e)
        {
            HI = 2;
        }

        private string BrandName;
        private byte HI;
        private bool IsHIPatient = false;

        private ObservableCollection<GetGenMedProductForSell> _GetGenMedProductForSellListByPrescriptID;
        public ObservableCollection<GetGenMedProductForSell> GetGenMedProductForSellListByPrescriptID
        {
            get { return _GetGenMedProductForSellListByPrescriptID; }
            set
            {
                _GetGenMedProductForSellListByPrescriptID = value;
                NotifyOfPropertyChange(() => GetGenMedProductForSellListByPrescriptID);
            }
        }

        private ObservableCollection<GetGenMedProductForSell> GetGenMedProductForSellList;

        private ObservableCollection<GetGenMedProductForSell> _GetGenMedProductForSellSum;
        public ObservableCollection<GetGenMedProductForSell> GetGenMedProductForSellListSum
        {
            get { return _GetGenMedProductForSellSum; }
            set
            {
                if (_GetGenMedProductForSellSum != value)
                    _GetGenMedProductForSellSum = value;
                NotifyOfPropertyChange(() => GetGenMedProductForSellListSum);
            }
        }

        private ObservableCollection<GetGenMedProductForSell> GetGenMedProductForSellTemp;

        private GetGenMedProductForSell _SelectedSellVisitor;
        public GetGenMedProductForSell SelectedSellVisitor
        {
            get { return _SelectedSellVisitor; }
            set
            {
                if (_SelectedSellVisitor != value)
                    _SelectedSellVisitor = value;
                NotifyOfPropertyChange(() => SelectedSellVisitor);
            }
        }

        AutoCompleteBox au;
        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            au = sender as AutoCompleteBox;
        }

        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            if (!IsCode.GetValueOrDefault())
            {
                BrandName = e.Parameter;
                SearchGetGenMedProductForSell(e.Parameter, false);
            }
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (au.SelectedItem != null)
            {
                SelectedSellVisitor = au.SelectedItem as GetGenMedProductForSell;
            }
        }

        private void ListDisplayAutoComplete()
        {
            var hhh = from hd in GetGenMedProductForSellList
                      group hd by new
                      { 
                          hd.GenMedProductID,
                          hd.Code,
                          hd.BrandName,
                          hd.UnitName,
                          hd.Qty,
                          hd.DayRpts,

                          hd.QtyForDay,
                          hd.V_DrugType,
                          hd.QtyMaxAllowed,
                          hd.QtySchedMon,
                          hd.QtySchedTue,
                          hd.QtySchedWed,
                          hd.QtySchedThu,
                          hd.QtySchedFri,
                          hd.QtySchedSat,
                          hd.QtySchedSun,
                          hd.SchedBeginDOW,
                          hd.DispenseVolume,

                          hd.HIPaymentPercent
                      } into hdgroup

                      select new
                      {
                          Remaining = hdgroup.Sum(groupItem => groupItem.Remaining),
                          GenMedProductID = hdgroup.Key.GenMedProductID,
                          Code = hdgroup.Key.Code,
                          UnitName = hdgroup.Key.UnitName,
                          BrandName = hdgroup.Key.BrandName,
                          Qty = hdgroup.Key.Qty,
                          DayRpts = hdgroup.Key.DayRpts,

                          V_DrugType = hdgroup.Key.V_DrugType,
                          QtyForDay = hdgroup.Key.QtyForDay,
                          QtyMaxAllowed = hdgroup.Key.QtyMaxAllowed,
                          QtySchedMon = hdgroup.Key.QtySchedMon,
                          QtySchedTue = hdgroup.Key.QtySchedTue,
                          QtySchedWed = hdgroup.Key.QtySchedWed,
                          QtySchedThu = hdgroup.Key.QtySchedThu,
                          QtySchedFri = hdgroup.Key.QtySchedFri,
                          QtySchedSat = hdgroup.Key.QtySchedSat,
                          QtySchedSun = hdgroup.Key.QtySchedSun,
                          SchedBeginDOW = hdgroup.Key.SchedBeginDOW,
                          DispenseVolume = hdgroup.Key.DispenseVolume,

                          HIPaymentPercent = hdgroup.Key.HIPaymentPercent
                      };
            for (int i = 0; i < hhh.Count(); i++)
            {
                GetGenMedProductForSell item = new GetGenMedProductForSell();
                item.GenMedProductID = hhh.ToList()[i].GenMedProductID;
                item.Code = hhh.ToList()[i].Code;
                item.BrandName = hhh.ToList()[i].BrandName;
                item.UnitName = hhh.ToList()[i].UnitName;
                item.Remaining = hhh.ToList()[i].Remaining;
                item.Qty = hhh.ToList()[i].Qty;
                item.DayRpts = hhh.ToList()[i].DayRpts;

                item.V_DrugType = hhh.ToList()[i].V_DrugType;
                item.QtyForDay = hhh.ToList()[i].QtyForDay;
                item.QtyMaxAllowed = hhh.ToList()[i].QtyMaxAllowed;
                item.QtySchedMon = hhh.ToList()[i].QtySchedMon;
                item.QtySchedTue = hhh.ToList()[i].QtySchedTue;
                item.QtySchedWed = hhh.ToList()[i].QtySchedWed;
                item.QtySchedThu = hhh.ToList()[i].QtySchedThu;
                item.QtySchedFri = hhh.ToList()[i].QtySchedFri;
                item.QtySchedSat = hhh.ToList()[i].QtySchedSat;
                item.QtySchedSun = hhh.ToList()[i].QtySchedSun;
                item.SchedBeginDOW = hhh.ToList()[i].SchedBeginDOW;
                item.DispenseVolume = hhh.ToList()[i].DispenseVolume;

                item.HIPaymentPercent = hhh.ToList()[i].HIPaymentPercent;
                GetGenMedProductForSellListSum.Add(item);
            }

            if (IsCode.GetValueOrDefault())
            {
                if (GetGenMedProductForSellListSum != null && GetGenMedProductForSellListSum.Count > 0)
                {
                    //KMx: Lý do comment code bên dưới: Khi người dùng tìm bằng code, nhưng không nhập prefix. VD: Hàng có code = "med0001", nhưng người dùng chỉ nhập "0001" (14/12/2014 14:23).
                    //var item = GetGenMedProductForSellListSum.Where(x => x.Code == txt);
                    //if (item != null && item.Count() > 0)
                    //{
                    //    SelectedSellVisitor = item.ToList()[0];
                    //}
                    //else
                    //{
                    //    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                    //}
                    SelectedSellVisitor = GetGenMedProductForSellListSum[0];
                }
                else
                {
                    SelectedSellVisitor = null;

                    if (tbx != null)
                    {
                        txt = "";
                        tbx.Text = "";
                    }
                    if (au != null)
                    {
                        au.Text = "";
                    }
                    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                }
            }
            else
            {
                if (au != null)
                {
                    au.ItemsSource = GetGenMedProductForSellListSum;
                    au.PopulateComplete();
                }
            }
        }

        private void SearchGetGenMedProductForSell(string name, bool? IsCode)
        {
            if (SelectedOutInvoice == null)
            {
                return;
            }

            //long? IssueID = null;
            //if (SelectedOutInvoice != null)
            //{
            //    IssueID = SelectedOutInvoice.IssueID;
            //}

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetGenMedProductForSellAutoComplete_ForPrescription(HI, IsHIPatient, name, StoreID, SelectedOutInvoice.IssueID, IsCode, V_MedProductType, SelectedOutInvoice.RefGenDrugCatID_1
                        , Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetGenMedProductForSellAutoComplete_ForPrescription(asyncResult);
                            GetGenMedProductForSellList.Clear();
                            GetGenMedProductForSellListSum.Clear();
                            LaySoLuongTheoNgayDung(results);
                            ListDisplayAutoComplete();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            //20180710 TBL: Comment lai vi khi ra ket qua no mat focus vao AutoCompleteBox
                            //if (au != null)
                            //{
                            //    au.Focus();
                            //}
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void LaySoLuongTheoNgayDung(IList<GetGenMedProductForSell> results)
        {
            if (results != null)
            {
                GetGenMedProductForSellTemp = results.ToObservableCollection();
                if (GetGenMedProductForSellTemp == null)
                {
                    GetGenMedProductForSellTemp = new ObservableCollection<GetGenMedProductForSell>();
                }
                if (OutwardDrugsCopy != null && OutwardDrugsCopy.Count > 0)
                {
                    foreach (OutwardDrugMedDept d in OutwardDrugsCopy)
                    {
                        var value = results.Where(x => x.GenMedProductID == d.GenMedProductID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                        if (value.Count() > 0)
                        {
                            foreach (GetGenMedProductForSell s in value.ToList())
                            {
                                s.Remaining = s.Remaining + (int)d.OutQuantityOld;
                                s.RemainingFirst = s.RemainingFirst + (int)d.OutQuantityOld;
                            }
                        }
                        else
                        {
                            GetGenMedProductForSell p = d.GetGenMedProductForSell;
                            p.Remaining = (int)d.OutQuantity;
                            p.RemainingFirst = (int)d.OutQuantity;
                            p.InBatchNumber = d.InBatchNumber;
                            p.SellingPrice = d.OutPrice;
                            p.InID = Convert.ToInt64(d.InID);
                            p.STT = d.STT;
                            GetGenMedProductForSellTemp.Add(p);
                            // d = null;
                        }
                    }
                }
                foreach (GetGenMedProductForSell s in GetGenMedProductForSellTemp)
                {
                    if (SelectedOutInvoice.OutwardDrugMedDepts != null && SelectedOutInvoice.OutwardDrugMedDepts.Count > 0)
                    {
                        foreach (OutwardDrugMedDept d in SelectedOutInvoice.OutwardDrugMedDepts)
                        {
                            if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                            {
                                s.Remaining = s.Remaining - (int)d.OutQuantity;
                            }
                        }
                    }
                    GetGenMedProductForSellList.Add(s);
                }
            }
        }

        private bool CheckValidDrugAuto(GetGenMedProductForSell temp)
        {
            if (temp == null)
            {
                return false;
            }
            return !temp.HasErrors;
        }

        private void CheckBatchNumberExists(OutwardDrugMedDept p)
        {
            bool kq = false;
            if (SelectedOutInvoice.OutwardDrugMedDepts != null)
            {
                foreach (OutwardDrugMedDept p1 in SelectedOutInvoice.OutwardDrugMedDepts)
                {
                    if (p.GenMedProductID == p1.GenMedProductID && p.InBatchNumber == p1.InBatchNumber && p.InID == p1.InID)
                    {
                        p1.OutQuantity += p.OutQuantity;
                        p1.RequestQty += p.RequestQty;
                        kq = true;
                        break;
                    }
                }
                if (!kq)
                {
                    p.HI = p.GetGenMedProductForSell.InsuranceCover;

                    if (p.InwardDrugMedDept == null)
                    {
                        p.InwardDrugMedDept = new InwardDrugMedDept();
                        p.InwardDrugMedDept.InID = p.InID.GetValueOrDefault();
                        p.InwardDrugMedDept.GenMedProductID = p.GenMedProductID;
                    }
                    p.InvoicePrice = p.OutPrice;
                    p.InwardDrugMedDept.NormalPrice = p.OutPrice;
                    p.InwardDrugMedDept.HIPatientPrice = p.OutPrice;
                    p.InwardDrugMedDept.HIAllowedPrice = p.HIAllowedPrice;
                    if (p.HIAllowedPrice.GetValueOrDefault(-1) > 0)
                    {
                        p.PriceDifference = p.OutPrice - p.HIAllowedPrice.GetValueOrDefault(0);
                    }

                    SelectedOutInvoice.OutwardDrugMedDepts.Add(p);
                }
                txt = "";
                SelectedSellVisitor = null;
                if (IsCode.GetValueOrDefault())
                {
                    if (tbx != null)
                    {
                        tbx.Text = "";
                        tbx.Focus();
                    }

                }
                else
                {
                    if (au != null)
                    {
                        au.Text = "";
                        au.Focus();
                    }
                }
            }
        }


        private void ChooseBatchNumber(GetGenMedProductForSell value)
        {
            var items = GetGenMedProductForSellList.Where(x => x.GenMedProductID == value.GenMedProductID).OrderBy(p => p.STT);
            foreach (GetGenMedProductForSell item in items)
            {
                if (item.Remaining <= 0)
                {
                    continue;
                }

                OutwardDrugMedDept p = new OutwardDrugMedDept();

                //KMx: Nếu không new class bên dưới, thì không thể lưu thuốc thêm bằng tay, lỗi khi convert to xml gừi xuống database (29/11/2014 17:20).
                p.RefGenericDrugDetail = new RefGenMedProductDetails();
                p.RefGenericDrugDetail.GenMedProductID = item.GenMedProductID;
                /////////////////////////

                item.GenMedProductIDChanged = value.GenMedProductIDChanged;
                p.GetGenMedProductForSell = item;
                p.GenMedProductID = item.GenMedProductID;
                p.InBatchNumber = item.InBatchNumber;
                p.InID = item.InID;
                p.OutPrice = item.OutPrice;
                p.InExpiryDate = item.InExpiryDate;
                p.SdlDescription = item.SdlDescription;
                p.HIAllowedPrice = item.HIAllowedPrice;

                //KMx: Nếu thuốc muốn thêm vào giống với thuốc đã có trong phiếu phải gán lại những thuộc tính bên dưới.
                //Nếu không gán lại thì hàm CheckInsurance_New(), btnNgayDung_New() và ListDisplayAutoComplete() sẽ sai ở dòng 
                //group hd by new
                //    { 
                //        hd.GenMedProductID, hd.DayRpts, hd.V_DrugType, hd.QtyForDay, hd.QtyMaxAllowed, hd.QtySchedMon, hd.QtySchedTue,
                //        hd.QtySchedWed, hd.QtySchedThu, hd.QtySchedFri, hd.QtySchedSat, hd.QtySchedSun, hd.SchedBeginDOW, hd.DispenseVolume,
                //        hd.GetGenMedProductForSell.InsuranceCover, hd.GetGenMedProductForSell.BrandName
                //    } into hdgroup

                p.V_DrugType = item.V_DrugType;
                p.QtyForDay = (decimal)item.QtyForDay;
                p.DayRpts = item.DayRpts;
                p.QtyMaxAllowed = item.QtyMaxAllowed;
                p.QtySchedMon = item.QtySchedMon;
                p.QtySchedTue = item.QtySchedTue;
                p.QtySchedWed = item.QtySchedWed;
                p.QtySchedThu = item.QtySchedThu;
                p.QtySchedFri = item.QtySchedFri;
                p.QtySchedSat = item.QtySchedSat;
                p.QtySchedSun = item.QtySchedSun;
                p.SchedBeginDOW = item.SchedBeginDOW;
                p.DispenseVolume = item.DispenseVolume;

                p.HIPaymentPercent = item.HIPaymentPercent;

                if (item.Remaining - value.RequiredNumber < 0)
                {
                    if (value.Qty > item.Remaining)
                    {
                        p.RequestQty = item.Remaining;
                        value.Qty = value.Qty - item.Remaining;
                    }
                    else
                    {
                        p.RequestQty = value.Qty;
                        value.Qty = 0;
                    }
                    
                    value.RequiredNumber = value.RequiredNumber - item.Remaining;
                    
                    p.OutQuantity = item.Remaining;

                    CheckBatchNumberExists(p);
                    item.Remaining = 0;
                }
                else
                {
                    p.RequestQty = value.Qty;
                    value.Qty = 0;

                    p.OutQuantity = (int)value.RequiredNumber;
                    CheckBatchNumberExists(p);
                    item.Remaining = item.Remaining - (int)value.RequiredNumber;
                    break;
                }
            }
            SumTotalPrice();
        }


        private void AddListOutwardDrug(GetGenMedProductForSell SelectedDrugForSell)
        {
            if (CheckValidDrugAuto(SelectedDrugForSell))
            {
                ChooseBatchNumber(SelectedDrugForSell);
            }
            else
            {
                string ErrorMessage = string.Format("{0}.", eHCMSResources.Z0888_G1_ThuocThaoTacBiLoi);
                if (SelectedDrugForSell.BrandName != null)
                {
                    ErrorMessage = string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + string.Format(eHCMSResources.Z0915_G1_0DaBiLoiKTraLai, SelectedDrugForSell.BrandName);
                }
                MessageBox.Show(ErrorMessage, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
            }
        }

        private void ReCountQtyRequest(GetGenMedProductForSell SelectedDrugForSell)
        {
            if (SelectedOutInvoice.OutwardDrugMedDepts == null)
            {
                SelectedOutInvoice.OutwardDrugMedDepts = new ObservableCollection<OutwardDrugMedDept>();
            }
            var results1 = SelectedOutInvoice.OutwardDrugMedDepts.Where(x => x.GenMedProductID == SelectedDrugForSell.GenMedProductID);
            if (results1 != null && results1.Count() > 0)
            {
                foreach (OutwardDrugMedDept p in results1)
                {
                    if (p.RequestQty > p.OutQuantity)
                    {
                        p.RequestQty = (int)p.OutQuantity;
                    }
                    SelectedDrugForSell.Qty = SelectedDrugForSell.Qty - (int)p.RequestQty;
                }
            }
        }

        public bool CheckValidQty(GetGenMedProductForSell SelectedDrugForSell)
        {
            if (SelectedOutInvoice != null && SelectedDrugForSell != null)
            {
                int intOutput = 0;
                if (SelectedDrugForSell.RequiredNumber <= 0
                    || !Int32.TryParse(SelectedDrugForSell.RequiredNumber.ToString(), out intOutput) //Nếu số lượng không phải là số nguyên.
                    || SelectedDrugForSell.RequiredNumber > SelectedDrugForSell.Remaining) //Nếu số lượng muốn thêm > số lượng còn trong kho.
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0890_G1_SLgKgHopLe), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                return true;
            }
            else
            {
                MessageBox.Show(eHCMSResources.K0400_G1_ChonThuocCanThem, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return false;
            }
        }

        //Kết hợp 2 hàm ReCountQtyRequest() và AddListOutwardDrug() (09/06/2014 16:40)
        public void ReCountQtyAndAddList(GetGenMedProductForSell SelectedSellVisitor)
        {
            if (!CheckValidQty(SelectedSellVisitor))
                return;
            else
            {
                ReCountQtyRequest(SelectedSellVisitor);
                AddListOutwardDrug(SelectedSellVisitor);
            }
        }

        public void AddItem(object sender, RoutedEventArgs e)
        {
            ReCountQtyAndAddList(SelectedSellVisitor);
        }

        #region Properties member
        private ObservableCollection<GetGenMedProductForSell> BatchNumberListTemp;
        private ObservableCollection<GetGenMedProductForSell> BatchNumberListShow;
        private ObservableCollection<OutwardDrugMedDept> OutwardDrugListByGenMedProductID;
        private ObservableCollection<OutwardDrugMedDept> OutwardDrugListByGenMedProductIDFirst;

        private OutwardDrugMedDept _SelectedOutwardDrug;
        public OutwardDrugMedDept SelectedOutwardDrug
        {
            get { return _SelectedOutwardDrug; }
            set
            {
                if (_SelectedOutwardDrug != value)
                    _SelectedOutwardDrug = value;
                NotifyOfPropertyChange(() => SelectedOutwardDrug);
            }
        }
        #endregion

        private void GetGenMedProductForSellBatchNumber(long GenMedProductID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetInBatchNumberAllDrugDept_ByGenMedProductID_ForPrescription(GenMedProductID, V_MedProductType, StoreID, IsHIPatient, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetInBatchNumberAllDrugDept_ByGenMedProductID_ForPrescription(asyncResult);
                            BatchNumberListTemp = results.ToObservableCollection();
                            if (BatchNumberListTemp != null && BatchNumberListTemp.Count > 0)
                            {
                                UpdateListToShow();
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z0891_G1_KgConLoNaoKhac, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void lnkChooseBatchNumber_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOutInvoice != null)
            {
                Button lnkBatchNumber = sender as Button;
                long GenMedProductID = (long)lnkBatchNumber.CommandParameter;
                OutwardDrugListByGenMedProductID = SelectedOutInvoice.OutwardDrugMedDepts.Where(x => x.GenMedProductID == GenMedProductID).ToObservableCollection();
                if (OutwardDrugsCopy != null)
                {
                    OutwardDrugListByGenMedProductIDFirst = OutwardDrugsCopy.Where(x => x.GenMedProductID == GenMedProductID).ToObservableCollection();
                }
                GetGenMedProductForSellBatchNumber(GenMedProductID);
            }
        }

        public void UpdateListToShow()
        {
            if (OutwardDrugListByGenMedProductIDFirst != null)
            {
                foreach (OutwardDrugMedDept d in OutwardDrugListByGenMedProductIDFirst)
                {
                    var value = BatchNumberListTemp.Where(x => x.GenMedProductID == d.GenMedProductID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                    if (value.Count() > 0)
                    {
                        foreach (GetGenMedProductForSell s in value.ToList())
                        {
                            s.Remaining = s.Remaining + (int)d.OutQuantityOld;
                            s.RemainingFirst = s.RemainingFirst + (int)d.OutQuantityOld;
                        }
                    }
                    else
                    {
                        GetGenMedProductForSell p = d.GetGenMedProductForSell;
                        p.Remaining = (int)d.OutQuantity;
                        p.RemainingFirst = (int)d.OutQuantity;
                        p.InBatchNumber = d.InBatchNumber;
                        p.SellingPrice = d.OutPrice;
                        p.InID = Convert.ToInt64(d.InID);
                        p.STT = d.STT;
                        BatchNumberListTemp.Add(p);
                    }
                }
            }
            foreach (GetGenMedProductForSell s in BatchNumberListTemp)
            {
                if (OutwardDrugListByGenMedProductID.Count > 0)
                {
                    foreach (OutwardDrugMedDept d in OutwardDrugListByGenMedProductID)
                    {
                        //20200422 TBL: Trừ luôn số lượng dòng nhập đang được chọn để đổi lô
                        //if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID && d.InID != SelectedOutwardDrug.InID)
                        if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                        {
                            s.Remaining = s.Remaining - (int)d.OutQuantity;
                        }
                    }
                }
            }

            BatchNumberListShow = BatchNumberListTemp.Where(x => x.Remaining > 0).ToObservableCollection();

            if (BatchNumberListShow != null && BatchNumberListShow.Count > 0)
            {
                Action<IChooseBatchNumberForPrescription> onInitDlg = delegate (IChooseBatchNumberForPrescription proAlloc)
                {
                    proAlloc.FormType = 2;//phat su kien cho form chinh sua bat
                    proAlloc.SelectedOutwardDrug = SelectedOutwardDrug.DeepCopy();
                    if (BatchNumberListShow != null)
                    {
                        proAlloc.BatchNumberListShow = BatchNumberListShow.DeepCopy();
                    }
                    if (OutwardDrugListByGenMedProductID != null)
                    {
                        proAlloc.OutwardDrugListByGenMedProductID = OutwardDrugListByGenMedProductID.DeepCopy();
                    }
                };
                GlobalsNAV.ShowDialog<IChooseBatchNumberForPrescription>(onInitDlg);
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z0891_G1_KgConLoNaoKhac);
            }
        }
        #endregion

        #region delete outward drug detals member
        private void DeleteOutwardDrugs(OutwardDrugMedDept temp)
        {
            OutwardDrugMedDept p = temp.DeepCopy();
            SelectedOutInvoice.OutwardDrugMedDepts.Remove(temp);
            foreach (OutwardDrugMedDept item in SelectedOutInvoice.OutwardDrugMedDepts)
            {
                if (item.GenMedProductID == p.GenMedProductID)
                {
                    item.RequestQty = item.RequestQty + p.RequestQty;
                    break;
                }
            }
        }

        private void DeleteInvoiceDrugInObject()
        {
            OutwardDrugMedDept p = SelectedOutwardDrug.DeepCopy();
            SelectedOutInvoice.OutwardDrugMedDepts.Remove(SelectedOutwardDrug);
            foreach (OutwardDrugMedDept item in SelectedOutInvoice.OutwardDrugMedDepts)
            {
                if (item.GenMedProductID == p.GenMedProductID)
                {
                    item.RequestQty = item.RequestQty + p.RequestQty;
                    break;
                }
            }
            SelectedOutInvoice.OutwardDrugMedDepts = SelectedOutInvoice.OutwardDrugMedDepts.ToObservableCollection();
            SumTotalPrice();
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.Z0892_CoMuonXoaThuocKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (SelectedOutwardDrug != null)
                {
                    DeleteInvoiceDrugInObject();
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0543_G1_Msg_InfoLoi);
                }
            }
        }

        #endregion

        public void btnNew()
        {
            TryClose();
        }
        #region IHandle<EditChooseBatchNumberForPrescriptionEvent> Members

        public void Handle(EditChooseBatchNumberForPrescriptionEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrug.GetGenMedProductForSell.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.GetGenMedProductForSell.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.GetGenMedProductForSell.HIAllowedPrice = message.BatchNumberVisitorSelected.HIAllowedPrice;
                SelectedOutwardDrug.GetGenMedProductForSell.InID = message.BatchNumberVisitorSelected.InID;
                SelectedOutwardDrug.GetGenMedProductForSell.Remaining = message.BatchNumberVisitorSelected.Remaining;
                SelectedOutwardDrug.GetGenMedProductForSell.RemainingFirst = message.BatchNumberVisitorSelected.RemainingFirst;
                SelectedOutwardDrug.GetGenMedProductForSell.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                //SelectedOutwardDrug.GetGenMedProductForSell.SellingPrice = message.BatchNumberVisitorSelected.SellingPrice;

                SelectedOutwardDrug.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.InID = message.BatchNumberVisitorSelected.InID;
                //SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.SellingPrice;
                SelectedOutwardDrug.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                SumTotalPrice();
            }
        }

        #endregion

        #region IHandle<EditChooseBatchNumberForPrescriptionResetQtyEvent> Members

        public void Handle(EditChooseBatchNumberForPrescriptionResetQtyEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrug.GetGenMedProductForSell.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.GetGenMedProductForSell.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.GetGenMedProductForSell.HIAllowedPrice = message.BatchNumberVisitorSelected.HIAllowedPrice;
                SelectedOutwardDrug.GetGenMedProductForSell.InID = message.BatchNumberVisitorSelected.InID;
                SelectedOutwardDrug.GetGenMedProductForSell.Remaining = message.BatchNumberVisitorSelected.Remaining;
                SelectedOutwardDrug.GetGenMedProductForSell.RemainingFirst = message.BatchNumberVisitorSelected.RemainingFirst;
                SelectedOutwardDrug.GetGenMedProductForSell.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                //SelectedOutwardDrug.GetGenMedProductForSell.SellingPrice = message.BatchNumberVisitorSelected.SellingPrice;

                SelectedOutwardDrug.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.InID = message.BatchNumberVisitorSelected.InID;
                //SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.SellingPrice;
                SelectedOutwardDrug.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                SelectedOutwardDrug.OutQuantity = message.BatchNumberVisitorSelected.Remaining;
                SumTotalPrice();
            }
        }

        #endregion

        string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;

                if (!string.IsNullOrEmpty(txt))
                {
                    SearchGetGenMedProductForSell(txt, true);
                }
            }
        }

        public void DrugCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchGetGenMedProductForSell((sender as TextBox).Text, true);
            }
        }

        TextBox tbx = null;
        public void AxTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            tbx = sender as TextBox;
        }

        private bool _VisibilityName = true;
        public bool VisibilityName
        {
            get
            {
                return _VisibilityName;
            }
            set
            {
                _VisibilityName = value;
                _VisibilityCode = !_VisibilityName;
                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);

            }
        }

        private bool _VisibilityCode = false;
        public bool VisibilityCode
        {
            get
            {
                return _VisibilityCode;
            }
            set
            {
                if (_VisibilityCode != value)
                {
                    _VisibilityCode = value;
                    NotifyOfPropertyChange(() => VisibilityCode);
                }
            }
        }

        private bool? IsCode = false;
        public void Code_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsCode = true;
            VisibilityName = false;
        }

        public void Name_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsCode = false;
            VisibilityName = true;
        }

        public void btnUp()
        {
            if (grdPrescription.SelectedItem != null && grdPrescription.SelectedIndex > 0)
            {
                int i = grdPrescription.SelectedIndex.DeepCopy();
                OutwardDrugMedDept Up = SelectedOutInvoice.OutwardDrugMedDepts[i - 1].DeepCopy();
                OutwardDrugMedDept Down = SelectedOutInvoice.OutwardDrugMedDepts[i].DeepCopy();
                SelectedOutInvoice.OutwardDrugMedDepts[i] = Up;
                SelectedOutInvoice.OutwardDrugMedDepts[i - 1] = Down;
                grdPrescription.SelectedIndex = i - 1;
            }
        }

        public void btnDown()
        {
            if (grdPrescription.SelectedItem != null && grdPrescription.SelectedIndex < SelectedOutInvoice.OutwardDrugMedDepts.Count() - 1)
            {
                int i = grdPrescription.SelectedIndex.DeepCopy();
                OutwardDrugMedDept Up = SelectedOutInvoice.OutwardDrugMedDepts[i].DeepCopy();
                OutwardDrugMedDept Down = SelectedOutInvoice.OutwardDrugMedDepts[i + 1].DeepCopy();
                SelectedOutInvoice.OutwardDrugMedDepts[i] = Down;
                SelectedOutInvoice.OutwardDrugMedDepts[i + 1] = Up;
                grdPrescription.SelectedIndex = i + 1;
            }
        }
    }
}


