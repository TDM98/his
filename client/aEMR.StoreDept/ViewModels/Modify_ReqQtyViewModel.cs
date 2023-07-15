using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Linq;
using Service.Core.Common;
using System.Text;
using aEMR.Common.PagedCollectionView;
/*
 * 20181114 #001 TTM: BM 0005259: Chuyển đổi CellEditEnded => CellEditEnding.
 */
namespace aEMR.StoreDept.ViewModels
{
    [Export(typeof(IModify_ReqQty)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Modify_ReqQtyViewModel : Conductor<object>, IModify_ReqQty
    {

        private bool _calByUnitUse;
        public bool CalByUnitUse
        {
            get { return _calByUnitUse; }
            set
            {
                if (_calByUnitUse != value)
                {
                    _calByUnitUse = value;
                    NotifyOfPropertyChange(() => CalByUnitUse);
                }
            }
        }
        
        //Định nghĩa lại biến khai báo trong Interface của ViewModel này
        private RequestDrugInwardClinicDept _RequestDrug;
        public RequestDrugInwardClinicDept RequestDrug
        {
            get
            {
                return _RequestDrug;
            }
            set
            {
                if (_RequestDrug != value)
                {
                    _RequestDrug = value;
                    NotifyOfPropertyChange(() => RequestDrug);
                }
            }
        }
        private PagedCollectionView _ReqOutwardDrugClinicDeptPatientlst;
        public PagedCollectionView ReqOutwardDrugClinicDeptPatientlst
        {
            get { return _ReqOutwardDrugClinicDeptPatientlst; }
            set
            {
                if (_ReqOutwardDrugClinicDeptPatientlst != value)
                {
                    _ReqOutwardDrugClinicDeptPatientlst = value;
                    NotifyOfPropertyChange(() => ReqOutwardDrugClinicDeptPatientlst);
                }
            }
        }
        private ObservableCollection< ReqOutwardDrugClinicDeptPatient> _RequestDetails;
        public ObservableCollection<ReqOutwardDrugClinicDeptPatient> RequestDetails
        {
            get
            {
                return _RequestDetails;
            }
            set
            {
                if (_RequestDetails != value)
                {
                    _RequestDetails = value;
                    //NotifyOfPropertyChange(() => RequestDetails);
                }
            }
        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Modify_ReqQtyViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            //Khởi tạo biến với giá trị rỗng. Biến này sẽ được gán giá trị mỗi khi Interface của ViewModel này được gọi đến
            //Nếu không khởi tạo, mỗi khi được gọi, biến RequestDrug sẽ hiển thị cả dữ liệu của phiếu cũ
            RequestDrug = new RequestDrugInwardClinicDept();
            //RequestDrug.ReqOutwardDetails
        }
        //hàm bắt sự kiện click nút Điều chỉnh tự động
        //public ObservableCollection<ReqOutwardDrugClinicDeptPatient> AutoModifyReqQty()
        //{
        //    foreach (var item in RequestDetails)
        //    {
        //        item.ReqQty = item.PrescribedQty > item.RemainingQty ? item.PrescribedQty - item.RemainingQty : 0;
        //    }

        //    return RequestDetails;
        //}
        public void btnAutoModify()
        {
            RequestDetails = RequestDrug.ReqOutwardDetails;
            foreach (var item in RequestDetails)
            {
                item.ReqQty = item.PrescribedQty > item.RemainingQty ? item.PrescribedQty - item.RemainingQty : 0;
            }
            ReqOutwardDrugClinicDeptPatientlst = new PagedCollectionView(RequestDetails);
        }
        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }              

        #region checking account
        private bool _mPhieuYeuCau_Tim = true;
        private bool _mPhieuYeuCau_Them = true;
        private bool _mPhieuYeuCau_Xoa = true;
        private bool _mPhieuYeuCau_XemIn = true;
        private bool _mPhieuYeuCau_In = true;

        public bool mPhieuYeuCau_Tim
        {
            get
            {
                return _mPhieuYeuCau_Tim;
            }
            set
            {
                if (_mPhieuYeuCau_Tim == value)
                    return;
                _mPhieuYeuCau_Tim = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Tim);
            }
        }

        public bool mPhieuYeuCau_Them
        {
            get
            {
                return _mPhieuYeuCau_Them;
            }
            set
            {
                if (_mPhieuYeuCau_Them == value)
                    return;
                _mPhieuYeuCau_Them = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Them);
            }
        }

        public bool mPhieuYeuCau_Xoa
        {
            get
            {
                return _mPhieuYeuCau_Xoa;
            }
            set
            {
                if (_mPhieuYeuCau_Xoa == value)
                    return;
                _mPhieuYeuCau_Xoa = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Xoa);
            }
        }

        public bool mPhieuYeuCau_XemIn
        {
            get
            {
                return _mPhieuYeuCau_XemIn;
            }
            set
            {
                if (_mPhieuYeuCau_XemIn == value)
                    return;
                _mPhieuYeuCau_XemIn = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_XemIn);
            }
        }

        public bool mPhieuYeuCau_In
        {
            get
            {
                return _mPhieuYeuCau_In;
            }
            set
            {
                if (_mPhieuYeuCau_In == value)
                    return;
                _mPhieuYeuCau_In = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_In);
            }
        }
       
        #endregion

        #region 1. Property


        private ObservableCollection<RefGenericDrugCategory_1> _RefGenericDrugCategory_1s;
        public ObservableCollection<RefGenericDrugCategory_1> RefGenericDrugCategory_1s
        {
            get
            {
                return _RefGenericDrugCategory_1s;
            }
            set
            {
                if (_RefGenericDrugCategory_1s != value)
                {
                    _RefGenericDrugCategory_1s = value;
                    NotifyOfPropertyChange(() => RefGenericDrugCategory_1s);
                }
            }
        }

        private RefGenMedProductDetails _SelectRefGenMedProductDetail;
        public RefGenMedProductDetails SelectRefGenMedProductDetail
        {
            get
            {
                return _SelectRefGenMedProductDetail;
            }
            set
            {
                if (_SelectRefGenMedProductDetail != value)
                {
                    _SelectRefGenMedProductDetail = value;
                    NotifyOfPropertyChange(() => SelectRefGenMedProductDetail);
                }
            }
        }

        private PagedSortableCollectionView<RefGenMedProductDetails> _RefGenMedProductDetails;
        public PagedSortableCollectionView<RefGenMedProductDetails> RefGenMedProductDetails
        {
            get
            {
                return _RefGenMedProductDetails;
            }
            set
            {
                if (_RefGenMedProductDetails != value)
                {
                    _RefGenMedProductDetails = value;
                    NotifyOfPropertyChange(() => RefGenMedProductDetails);
                }
            }
        }       


        
      
        private ReqOutwardDrugClinicDeptPatient _CurrentReqOutwardDrugClinicDeptPatient;
        public ReqOutwardDrugClinicDeptPatient CurrentReqOutwardDrugClinicDeptPatient
        {
            get { return _CurrentReqOutwardDrugClinicDeptPatient; }
            set
            {
                if (_CurrentReqOutwardDrugClinicDeptPatient != value)
                {
                    _CurrentReqOutwardDrugClinicDeptPatient = value;
                    NotifyOfPropertyChange(() => CurrentReqOutwardDrugClinicDeptPatient);
                }
            }
        }
        private ObservableCollection<ReqOutwardDrugClinicDeptPatient> ListRequestDrugDelete;


        private ReqOutwardDrugClinicDeptPatient _SelectedReqOutwardDrugClinicDeptPatient;
        
        public ReqOutwardDrugClinicDeptPatient SelectedReqOutwardDrugClinicDeptPatient
        {
            get { return _SelectedReqOutwardDrugClinicDeptPatient; }
            set
            {
                if (_SelectedReqOutwardDrugClinicDeptPatient != value)
                {
                    _SelectedReqOutwardDrugClinicDeptPatient = value;
                    NotifyOfPropertyChange(() => SelectedReqOutwardDrugClinicDeptPatient);
                }
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
                    NotifyOfPropertyChange(() => CanSelectedRefGenDrugCatID_1);
                }

            }
        }

        public bool CanSelectedRefGenDrugCatID_1
        {
            get { return V_MedProductType == (long)AllLookupValues.MedProductType.THUOC; }
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
        #endregion

        #region 3. Function Member


        private void ischanged(object item)
        {
            ReqOutwardDrugClinicDeptPatient p = item as ReqOutwardDrugClinicDeptPatient;
            if (p != null)
            {
                if (p.EntityState == EntityState.PERSITED)
                {
                    p.EntityState = EntityState.MODIFIED;
                }
            }
        }

        private bool CheckValidationEditor()
        {
            bool result = true;
            StringBuilder st = new StringBuilder();
            if (RequestDrug != null)
            {
                if (RequestDrug.ReqOutwardDetails != null)
                {
                    int nIdx = 0;
                    foreach (ReqOutwardDrugClinicDeptPatient item in RequestDrug.ReqOutwardDetails)
                    {
                        nIdx++;
                        if (item.GenMedProductID != null && item.GenMedProductID != 0)
                        {
                            if (item.ReqQty < 0)
                            {
                                string strErr = "Dữ liệu dòng số (" + item.DisplayGridRowNumber.ToString() + ") : [" + item.RefGenericDrugDetail.BrandNameAndCode + "] Số lượng yêu cầu phải >= 0";
                                st.AppendLine(strErr);
                                result = false;
                            }
                            if (item.PrescribedQty <= 0)
                            {
                                string strErr = "Dữ liệu dòng số (" + item.DisplayGridRowNumber.ToString() + ") : [" + item.RefGenericDrugDetail.BrandNameAndCode + "] Số lượng Chỉ Định phải > 0";
                                st.AppendLine(strErr);
                                result = false;
                            }
                        }
                    }
                }
                if (!result)
                {
                    MessageBox.Show(st.ToString());
                }
            }
            return result;
        }
        
        private void SaveRequest()
        {
            if (RequestDrug.ReqDrugInClinicDeptID > 0)
            {
                if (MessageBox.Show(eHCMSResources.A0138_G1_Msg_ConfLuuThayDoiTrenPhYC, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            
            SaveFullOp();
            
        }
        
        private void SaveFullOp()
        {
            if (ListRequestDrugDelete.Count > 0)
            {
                if (RequestDrug.ReqOutwardDetails == null)
                {
                    RequestDrug.ReqOutwardDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                }
                foreach (ReqOutwardDrugClinicDeptPatient item in ListRequestDrugDelete)
                {
                    RequestDrug.ReqOutwardDetails.Add(item);
                }
            }
        }

        private void RemoveRowBlank()
        {
            try
            {
                int idx = RequestDrug.ReqOutwardDetails.Count;
                if (idx > 0)
                {
                    idx--;
                    ReqOutwardDrugClinicDeptPatient obj = (ReqOutwardDrugClinicDeptPatient)RequestDrug.ReqOutwardDetails[idx];
                    if (obj.GenMedProductID == null || obj.GenMedProductID == 0)
                    {
                        RequestDrug.ReqOutwardDetails.RemoveAt(idx);
                    }
                }
                NotifyOfPropertyChange(() => RequestDrug);
            }
            catch
            { }
        }

        //private ObservableCollection<ReqOutwardDrugClinicDeptPatient> ReqOutwardDrugClinicDeptPatientlstCopy;
        


        private void RefeshRequest()
        {
            RequestDrug.ReqDrugInClinicDeptID = 0;
            RequestDrug.ReqNumCode = "";
            RequestDrug.ReqOutwardDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
            RequestDrug.DaNhanHang = false;
            RequestDrug.IsApproved = false;

            ListRequestDrugDelete.Clear();
        }
        
        private bool CheckValidGrid()
        {            
            bool results = true;

            if (RequestDrug.ReqOutwardDetails != null)
            {
                if (RequestDrug.ReqOutwardDetails.Count == 0)
                {
                    if (RequestDrug.ReqDrugInClinicDeptID > 0)
                    {
                        if (MessageBox.Show(eHCMSResources.A0922_G1_Msg_ConfXoaPhRong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.K0443_G1_NhapSLggYC);
                        return false;
                    }
                }

                for (int i = 0; i < RequestDrug.ReqOutwardDetails.Count; i++)
                {
                    if (RequestDrug.ReqOutwardDetails[i].GenMedProductID > 0)
                    {
                        if (RequestDrug.ReqOutwardDetails[i].PrescribedQty <= 0)
                        {
                            results = false;
                            MessageBox.Show("Dữ liệu dòng số (" + RequestDrug.ReqOutwardDetails[i].DisplayGridRowNumber.ToString() + ") : [" + RequestDrug.ReqOutwardDetails[i].RefGenericDrugDetail.BrandNameAndCode + "] không hợp lệ." + Environment.NewLine + "Số lượng Chỉ Định phải > 0");
                            break;
                        }
                        if (RequestDrug.ReqOutwardDetails[i].ReqQty < 0)
                        {
                            results = false;
                            MessageBox.Show("Dữ liệu dòng số (" + RequestDrug.ReqOutwardDetails[i].DisplayGridRowNumber.ToString() + ") : [" + RequestDrug.ReqOutwardDetails[i].RefGenericDrugDetail.BrandNameAndCode + "] không hợp lệ." + Environment.NewLine + "Số lượng yêu cầu phải >= 0");
                            break;
                        }
                    }
                }
            }

            return results;
        }

        #endregion
        //▼====== #001
        //public void grdReqOutwardDetails_CellEditEnded(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    if ((sender as DataGrid).SelectedItem != null)
        //    {
        //        ischanged((sender as DataGrid).SelectedItem);
        //    }
        //    // TxD 03/04/2015 if PrescribedQty is changed then ReqQty will be changed AS WELL (As requested by Khoa A & NTM)
        //    if (e.Column.DisplayIndex == 5)
        //    {
        //        if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null)
        //        {
        //            ReqOutwardDrugClinicDeptPatient selItem = RequestDrug.ReqOutwardDetails[(sender as DataGrid).SelectedIndex];
        //            selItem.ReqQty = selItem.PrescribedQty;
        //        }
        //    }
        //}
        public void grdReqOutwardDetails_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if ((sender as DataGrid).SelectedItem != null)
            {
                ischanged((sender as DataGrid).SelectedItem);
            }
            if (e.Column.DisplayIndex == 5)
            {
                if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null)
                {
                    ReqOutwardDrugClinicDeptPatient selItem = RequestDrug.ReqOutwardDetails[(sender as DataGrid).SelectedIndex];
                    selItem.ReqQty = selItem.PrescribedQty;
                }
            }
        }
        //▲====== #001

        public DataGridRow GetDataGridRowByDataContext(DataGrid dataGrid, object dataContext)
        {
        if (null != dataContext)
        {
            dataGrid.ScrollIntoView(dataContext, null);

            System.Windows.Automation.Peers.DataGridAutomationPeer automationPeer = (System.Windows.Automation.Peers.DataGridAutomationPeer)System.Windows.Automation.Peers.DataGridAutomationPeer.CreatePeerForElement(dataGrid);

                // Get the DataGridRowsPresenterAutomationPeer so we can find the rows in the data grid...
                //System.Windows.Automation.Peers.DataGridRowsPresenterAutomationPeer dataGridRowsPresenterAutomationPeer = automationPeer.GetChildren().
                //    Where(a => (a is System.Windows.Automation.Peers.DataGridRowsPresenterAutomationPeer)).
                //    Select(a => (a as System.Windows.Automation.Peers.DataGridRowsPresenterAutomationPeer)).
                //    FirstOrDefault();

                //if (null != dataGridRowsPresenterAutomationPeer)
                //{
                //    foreach (var item in dataGridRowsPresenterAutomationPeer.GetChildren())
                //    {
                //        // loop to find the DataGridCellAutomationPeer from which we can interrogate the owner -- which is a DataGridRow
                //        foreach (var subitem in (item as System.Windows.Automation.Peers.DataGridItemAutomationPeer).GetChildren())
                //        {
                //            if ((subitem is System.Windows.Automation.Peers.DataGridCellAutomationPeer))
                //            {
                //                // At last -- the only public method for finding a row....
                //                DataGridRow row = DataGridRow.GetRowContainingElement(((subitem as System.Windows.Automation.Peers.DataGridCellAutomationPeer).Owner as FrameworkElement));

                //                // check this row to see if it is bound to the requested dataContext.
                //                if ((row.DataContext) == dataContext)
                //                {
                //                    return row;
                //                }

                //                break; // Only need to check one cell in each row
                //            }
                //        }
                //    }
                //}
            }

            return null;
        }
        public void grdReqOutwardDetails_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (!CheckValidationEditor())
            {
                // TxD 26/03/2015: get rid of the following because if this Event is Cancel the the Row is still in Edit mode thus prevent the grid from calling Refresh
                //                  and at thie moment there is no need to cancel unless anything comes up later then review                                
                //e.Cancel = true;
            }
        }

        public void btnSave(object sender, RoutedEventArgs e)
        {
            RequestDrug.ReqOutwardDetails = RequestDrug.ReqOutwardDetails;
            
            //if (((ClinicDeptInPtReqFormView)this.GetView()).grdReqOutwardDetails != null && ((ClinicDeptInPtReqFormView)this.GetView()).grdReqOutwardDetails.IsValid)
            //{
            //    if (CheckValidGrid())
            //    {
            //        SaveRequest();
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Dữ liệu không chính xác. Vui lòng xem lại!");
            //}
        }     
   
        //Huyen: Added button Exit
        public void btnExit(object sender, RoutedEventArgs e)
        {
            this.TryClose();
        }        

        #region printing member

        
        #endregion
        public bool ListOutDrugReqFilter(object listObj)
        {
            ReqOutwardDrugClinicDeptPatient outItem = listObj as ReqOutwardDrugClinicDeptPatient;
            if (CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration != null && CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.Patient != null && CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.Patient.PatientID > 0)
            {
                if (outItem.CurPatientRegistration != null && outItem.CurPatientRegistration.Patient != null && outItem.CurPatientRegistration.Patient.PatientID > 0)
                {
                    return (outItem.CurPatientRegistration.Patient.PatientID == CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.Patient.PatientID);
                }
            }
            else
            {
                if (CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration == null && outItem.CurPatientRegistration == null)
                {
                    return true;
                }
            }
            return false;
        }


        private enum DataGridCol
        {
            ColMultiDelete = 0,
            ColDelete = 1,
            MaThuoc = 2,
            TenThuoc = 3
        }

        TextBox tbx = null;
        public void AxTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            tbx = sender as TextBox;
        }

        string txt = "";

        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;

                System.Diagnostics.Debug.WriteLine(" ====> AxTextBox_LostFocus 1 .....");

                if (!string.IsNullOrEmpty(txt))
                {
                    string Code = Globals.FormatCode(V_MedProductType, txt);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(" ====> AxTextBox_LostFocus 2 .....");
                    // TxD 23/12/2014 If Code is blank then clear selected item
                    CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();
                }
            }
        }

        AutoCompleteBox au;
        AutoCompleteBox acbAutoDrug_Text = null;
        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            acbAutoDrug_Text = sender as AutoCompleteBox;
        }

        private RequestDrugInwardForHiStore _RequestDrugHIStore;
        public RequestDrugInwardForHiStore RequestDrugHIStore
        {
            get
            {
                return _RequestDrugHIStore;
            }
            set
            {
                if (_RequestDrugHIStore != value)
                {
                    _RequestDrugHIStore = value;
                    NotifyOfPropertyChange(() => RequestDrugHIStore);
                }
            }
        }
    }
}
