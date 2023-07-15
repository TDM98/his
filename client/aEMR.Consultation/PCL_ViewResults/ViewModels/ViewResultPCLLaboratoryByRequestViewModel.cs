using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using System.Collections.ObjectModel;
using Castle.Windsor;
using aEMR.Common.BaseModel;
using System.Windows.Controls;
using aEMR.Controls;
/*
* 20180923 #001 TTM:
* 20181216 #002 TNHX: [BM0005424] Add button "View/Print" report "Phieu Ket qua Xet Nghiem"
* 20190815 #003 TTM:   BM 0013133: Không load lại kết quả CLS khi tìm đăng ký mới
*/
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IViewResultPCLLaboratoryByRequest)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ViewResultPCLLaboratoryByRequestViewModel : ViewModelBase, IViewResultPCLLaboratoryByRequest
    {
        //private ObservableCollection<Lookup> _RequestStatusList;
        //public ObservableCollection<Lookup> RequestStatusList
        //{
        //    get { return _RequestStatusList; }
        //    set
        //    {
        //        _RequestStatusList = value;
        //        NotifyOfPropertyChange(() => RequestStatusList);
        //    }
        //}
        private string _PCLExamTypeName;
        public string PCLExamTypeName
        {
            get { return _PCLExamTypeName; }
            set
            {
                if (_PCLExamTypeName != value)
                {
                    _PCLExamTypeName = value;
                    NotifyOfPropertyChange(() => PCLExamTypeName);
                }
            }
        }
        private ObservableCollection<PatientPCLLaboratoryResultDetail> _allPatientPCLLaboratoryResultDetail;
        public ObservableCollection<PatientPCLLaboratoryResultDetail> allPatientPCLLaboratoryResultDetail
        {
            get
            {
                return _allPatientPCLLaboratoryResultDetail;

            }
            set
            {
                if (_allPatientPCLLaboratoryResultDetail == value)
                    return;
                _allPatientPCLLaboratoryResultDetail = value;
                NotifyOfPropertyChange(() => allPatientPCLLaboratoryResultDetail);
            }
        }

        [ImportingConstructor]
        public ViewResultPCLLaboratoryByRequestViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);
            SearchCriteria = new PatientPCLRequestSearchCriteria();
            SearchCriteria.PatientFindBy = AllLookupValues.PatientFindBy.CAHAI;
            SearchCriteria.FromDate = Globals.ServerDate.Value.AddDays(-365);
            SearchCriteria.ToDate = Globals.ServerDate;
            ObjPatientPCLRequest_SearchPaging = new PagedSortableCollectionView<PatientPCLRequest>();
            ObjPatientPCLRequest_SearchPaging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPatientPCLRequest_SearchPaging_OnRefresh);
            ObjPatientPCLRequest_SearchPaging_Selected = new PatientPCLRequest();
            //Coroutine.BeginExecute(GetRequestStatusList());
        }
        //▼====== #001:     Tạo 1 hàm InitPatientInfo để pass thông tin bệnh nhân xuống cho các ContentControl để nhận thông tin Patient
        //                  Vì màn hình này bây giờ vừa là màn hình chính, vừa là màn hình con. 
        //                  Trường hợp:  1. Màn hình chính: Được khởi tạo lại nên lấy đc PatientID từ Globals do màn hình thông tin chung pass vào Globals.
        //                               2. Màn hình con: Không được khởi tạo nên không lấy PatientID đc 
        //      
        public void InitPatientInfo(Patient patientInfo)
        {
            if (patientInfo != null)
            {
                SearchCriteria.PatientID = patientInfo.PatientID;
            }
            else
            {
                SearchCriteria.PatientID = 0;
            }
        }

        //▲====== #001
        public void InitData()
        {
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                DoPatientPCLRequest_SearchPaging(ObjPatientPCLRequest_SearchPaging.PageIndex, ObjPatientPCLRequest_SearchPaging.PageSize);
            }
        }

        public override void DeactivateItem(object item, bool close)
        {
            base.DeactivateItem(item, close);
            Globals.EventAggregator.Unsubscribe(this);
        }
        //private IEnumerator<IResult> GetRequestStatusList()
        //{
        //    var results = new LoadLookupListTask(LookupValues.V_PCLRequestStatus, false, true);
        //    yield return results;
        //    RequestStatusList = results.LookupList;
        //    yield break;
        //}
        void ObjPatientPCLRequest_SearchPaging_OnRefresh(object sender, RefreshEventArgs e)
        {
            DoPatientPCLRequest_SearchPaging(ObjPatientPCLRequest_SearchPaging.PageIndex, ObjPatientPCLRequest_SearchPaging.PageSize);
        }

        private PatientPCLRequestSearchCriteria _SearchCriteria;
        public PatientPCLRequestSearchCriteria SearchCriteria
        {
            get { return _SearchCriteria; }
            set
            {
                if (_SearchCriteria != value)
                {
                    _SearchCriteria = value;
                    NotifyOfPropertyChange(() => SearchCriteria);
                }
            }
        }

        private DataEntities.PatientPCLRequest _ObjPatientPCLRequest_SearchPaging_Selected;
        public DataEntities.PatientPCLRequest ObjPatientPCLRequest_SearchPaging_Selected
        {
            get { return _ObjPatientPCLRequest_SearchPaging_Selected; }
            set
            {
                _ObjPatientPCLRequest_SearchPaging_Selected = value;
                NotifyOfPropertyChange(() => ObjPatientPCLRequest_SearchPaging_Selected);
            }
        }

        private PagedSortableCollectionView<DataEntities.PatientPCLRequest> _ObjPatientPCLRequest_SearchPaging;
        public PagedSortableCollectionView<DataEntities.PatientPCLRequest> ObjPatientPCLRequest_SearchPaging
        {
            get { return _ObjPatientPCLRequest_SearchPaging; }
            set
            {
                _ObjPatientPCLRequest_SearchPaging = value;
                NotifyOfPropertyChange(() => ObjPatientPCLRequest_SearchPaging);
            }
        }
        private void CallShowBusyIndicator(string BusyContent)
        {
            if (this.IsDialogView)
            {
                this.ShowBusyIndicator(BusyContent);
            }
            else
            {
                this.DlgShowBusyIndicator(BusyContent);
            }
        }
        private void CallHideBusyIndicator()
        {
            if (this.IsDialogView)
            {
                this.HideBusyIndicator();
            }
            else
            {
                this.DlgHideBusyIndicator();
            }
        }
        private void DoPatientPCLRequest_SearchPaging(int PageIndex, int PageSize, bool CountTotal = false)
        {
            CallShowBusyIndicator(eHCMSResources.Z1125_G1_DangTimKiem);
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPatientPCLRequest_ViewResult_SearchPaging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<PatientPCLRequest> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndPatientPCLRequest_ViewResult_SearchPaging(out totalCount, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                            }
                            finally
                            {
                                CallHideBusyIndicator();
                            }

                            if (bOK)
                            {
                                ObjPatientPCLRequest_SearchPaging.Clear();
                                ObjPatientPCLRequest_SearchPaging.TotalItemCount = totalCount;
                                if (allItems != null && allItems.Count > 0)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjPatientPCLRequest_SearchPaging.Add(item);
                                    }
                                }
                                CanBtnViewPrint = false;
                                NotifyOfPropertyChange(() => CanBtnViewPrint);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                    CallHideBusyIndicator();
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }

        public void btSearch()
        {
            if (Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                ObjPatientPCLRequest_SearchPaging.PageIndex = 0;
                allPatientPCLLaboratoryResultDetail = new ObservableCollection<PatientPCLLaboratoryResultDetail>();
                DoPatientPCLRequest_SearchPaging(0, 10000);
            }
            else
            {
                MessageBox.Show(eHCMSResources.K0290_G1_ChonBN);
            }
        }

        public void btClear()
        {
            SearchCriteria.FromDate = null;
            SearchCriteria.ToDate = null;
            SearchCriteria.PatientCode = "";
            SearchCriteria.FullName = "";
            SearchCriteria.PCLRequestNumID = "";
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            PatientPCLRequest p = eventArgs.Value as PatientPCLRequest;
            LoadPCLRequestResult(p);
        }
        public void LoadPCLRequestResult(PatientPCLRequest aPCLRequest)
        {
            PCLLaboratoryResults_With_ResultOld(aPCLRequest.PatientID, aPCLRequest.PatientPCLReqID, (long)aPCLRequest.V_PCLRequestType);
            GetPCLExamTypeName(aPCLRequest.PatientID, aPCLRequest.PatientPCLReqID, (long)aPCLRequest.V_PCLRequestType);
            PatientID = aPCLRequest.PatientID;
            PatientPCLReqID = aPCLRequest.PatientPCLReqID;
            PCLRequestTypeID = aPCLRequest.V_PCLRequestType > 0 ? (long)aPCLRequest.V_PCLRequestType : 0;
        }

        public AllLookupValues.PatientFindBy PatientFindBy { get; set; }

        public void rdtNgoaiTru_Checked(RoutedEventArgs e)
        {
            SearchCriteria.PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
        }

        public void rdtNoiTru_Checked(RoutedEventArgs e)
        {
            SearchCriteria.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
        }

        public void rdtAll_Checked(RoutedEventArgs e)
        {
            SearchCriteria.PatientFindBy = AllLookupValues.PatientFindBy.CAHAI;
        }

        private void PCLLaboratoryResults_With_ResultOld(long PatientID, long PatientPCLReqID, long V_PCLRequestType)
        {
            CallShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        // HPT 21/03/2017: Hiện tại đường dẫn xem kết quả xét nghiệm chỉ có trong ngoại trú nên tạm để const = ngoại trú
                        contract.BeginPCLLaboratoryResults_With_ResultOld(PatientID, PatientPCLReqID, V_PCLRequestType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                IList<PatientPCLLaboratoryResultDetail> results = contract.EndPCLLaboratoryResults_With_ResultOld(asyncResult);
                                allPatientPCLLaboratoryResultDetail = results.ToObservableCollection();
                                //▼====== #002
                                CanBtnViewPrint = true;
                                NotifyOfPropertyChange(() => CanBtnViewPrint);
                                //▲====== #002
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                CallHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    CallHideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void GetPCLExamTypeName(long PatientID, long PatientPCLReqID, long V_PCLRequestType)
        {
            PCLExamTypeName = "";
            CallShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetPCLExamTypeName(PatientID, PatientPCLReqID, V_PCLRequestType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                PCLExamTypeName = contract.EndGetPCLExamTypeName(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                CallHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    CallHideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }
        //▼====== #002
        private bool _CanBtnViewPrint = false;
        public bool CanBtnViewPrint
        {
            get
            {
                return _CanBtnViewPrint;
            }
            set
            {
                if (_CanBtnViewPrint != value)
                {
                    _CanBtnViewPrint = value;
                    NotifyOfPropertyChange(() => CanBtnViewPrint);
                }
            }
        }

        public void BtnViewPrint()
        {
            void onInitDlg(ICommonPreviewView proAlloc)
            {
                proAlloc.PatientID = PatientID;
                proAlloc.PatientPCLReqID = (int)PatientPCLReqID;
                proAlloc.V_PCLRequestType = PCLRequestTypeID;
                proAlloc.FindPatient = PCLRequestTypeID > 0 && PCLRequestTypeID == (long)AllLookupValues.V_PCLRequestType.NOI_TRU ? (int)AllLookupValues.PatientFindBy.NOITRU : (int)AllLookupValues.PatientFindBy.NGOAITRU;
                proAlloc.eItem = ReportName.PCLDEPARTMENT_LABORATORY_RESULT;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        private long PatientID = 0;
        private long PatientPCLReqID = 0;
        private long PCLRequestTypeID = 0;
        //▲====== #002
        //▼===== #003
        public void setDefaultWhenRenewPatient()
        {
            allPatientPCLLaboratoryResultDetail = new ObservableCollection<PatientPCLLaboratoryResultDetail>();
            ObjPatientPCLRequest_SearchPaging = new PagedSortableCollectionView<PatientPCLRequest>();
        }
        //▲===== #003
        private bool _IsDialogView = false;
        public bool IsDialogView
        {
            get
            {
                return _IsDialogView;
            }
            set
            {
                if (_IsDialogView == value)
                {
                    return;
                }
                _IsDialogView = value;
                NotifyOfPropertyChange(() => IsDialogView);
            }
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            if (!IsDialogView)
            {
                InitData();
            }
        }
        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
                if (Registration_DataStorage == null)
                {
                    InitPatientInfo(null);
                }
                else
                {
                    InitPatientInfo(Registration_DataStorage.CurrentPatient);
                }
            }
        }
        private bool _IsShowCheckTestItem = false;
        public bool IsShowCheckTestItem
        {
            get
            {
                return _IsShowCheckTestItem;
            }
            set
            {
                if (_IsShowCheckTestItem == value)
                {
                    return;
                }
                _IsShowCheckTestItem = value;
                NotifyOfPropertyChange(() => IsShowCheckTestItem);
            }
        }
        public void gvTestItems_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (IsShowCheckTestItem && sender != null && sender is DataGrid)
            {
                if ((sender as DataGrid).GetColumnByName("clIsChecked") != null)
                {
                    (sender as DataGrid).GetColumnByName("clIsChecked").Visibility = Visibility.Visible;
                }
            }
        }
    }
}