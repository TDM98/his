using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.CommonTasks;
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
using System.Xml.Linq;
using System.Linq;
using System.Windows.Data;
using eHCMS.Services.Core;
//using aEMR.Common.PagedCollectionView;
using DevExpress.Xpf.Printing;
using aEMR.ReportModel.ReportModels;
using Castle.Windsor;
using DevExpress.ReportServer.Printing;
/*
 * 20180923 #001 TTM:
 * 20190815 #002 TTM:   BM 0013133: Không load lại kết quả CLS khi tìm đăng ký mới.
 * 20200408 #003 TTM:   BM 0030103: Fix 1 số lỗi ở màn hình xem kết quả xét nghiệm theo test items.
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IViewResultPCLLaboratoryByExamTest)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ViewResultPCLLaboratoryByExamTestViewModel : Conductor<object>, IViewResultPCLLaboratoryByExamTest
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }
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
        [ImportingConstructor]
        public ViewResultPCLLaboratoryByExamTestViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            SearchCriteria = new PatientPCLRequestSearchCriteria();
            SearchCriteria.PatientFindBy = AllLookupValues.PatientFindBy.CAHAI;
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
                //▼===== 20191117 TTM: Khi thay đổi thông tin bệnh nhân sẽ load các test item bệnh nhân đã sử dụng để xem và vẽ biểu đồ.
                //▼===== #003: Không tự động load dữ liệu khi vào màn hình xem kết quả theo test item.
                //if (!IsDialogView)
                //{
                //    InitData();
                //}
                //▲===== #003
                //▲=====
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
                PCLExamTestItems_ByPatientID(Registration_DataStorage.CurrentPatient.PatientID);
            }
            CVS_PCLExamTestItems_SearchPaging = new CollectionViewSource();
            CV_PCLExamTestItems_SearchPaging = (CollectionView)CVS_PCLExamTestItems_SearchPaging.View;
        }

        void PCLExamTestItems_SearchPaging_OnRefresh(object sender, RefreshEventArgs e)
        {
            //PatientPCLLaboratoryResults_ByExamTest_Paging(Registration_DataStorage.CurrentPatient.PatientID, PCLExamTestItemID, PCLExamTypeID, null, null, PCLExamTestItems_SearchPaging.PageIndex, ParaSize);
            PatientPCLLaboratoryResults_ByExamTest_Paging(Registration_DataStorage.CurrentPatient.PatientID, PCLExamTestItemID, PCLExamTypeID, null, null, 0, ParaSize);
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

        private CollectionViewSource CVS_PCLExamTestItems_SearchPaging = null;
        public CollectionView CV_PCLExamTestItems_SearchPaging
        {
            get;
            set;
        }

        private void PCLExamTestItems_ByPatientID(long PatientID)
        {
            //▼===== #003: Do sử dụng biến SearchCriteria nên cần gán FromDate ToDate vào biến SearchCriteria
            if (SearchCriteria != null)
            {
                SearchCriteria.FromDate = FromDate;
                SearchCriteria.ToDate = ToDate;
            }
            //▲===== 

            // Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Đang tìm bệnh nhân..." });

            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    IsLoading = true;
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTestItems_ByPatientID(PatientID, SearchCriteria, Globals.DispatchCallback((asyncResult) =>
                        {

                            try
                            {
                                var Results = client.EndPCLExamTestItems_ByPatientID(asyncResult);
                                if (!string.IsNullOrEmpty(Results))
                                {
                                    LoadData(Results);
                                }

                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                            }

                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    IsLoading = false;
                    Globals.IsBusy = false;
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }

        private void PatientPCLLaboratoryResults_ByExamTest_Paging(long PatientID, long PCLExamTestItemID, long PCLExamTypeID, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    IsLoading = true;
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPatientPCLLaboratoryResults_ByExamTest_Paging(PatientID, PCLExamTestItemID, PCLExamTypeID, FromDate, ToDate, PageIndex, PageSize, (long)PatientFindBy, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int TotalRow = 0;
                                int MaxRow = 0;
                                var Results = client.EndPatientPCLLaboratoryResults_ByExamTest_Paging(out TotalRow, out MaxRow, asyncResult);
                                // PCLExamTestItems_SearchPaging.Clear();
                                // PCLExamTestItems_SearchPaging.TotalItemCount = TotalRow;
                                //if (Results != null && Results.Count > 0)
                                //{
                                //    foreach (var item in Results)
                                //    {
                                //        PCLExamTestItems_SearchPaging.Add(item);
                                //    }
                                this.TotalRow = TotalRow;
                                CVS_PCLExamTestItems_SearchPaging = new CollectionViewSource { Source = Results };
                                CV_PCLExamTestItems_SearchPaging = (CollectionView)CVS_PCLExamTestItems_SearchPaging.View;
                                CV_PCLExamTestItems_SearchPaging.Filter = null;


                                CVS_PCLExamTestItems_SearchPaging.GroupDescriptions.Clear();
                                CVS_PCLExamTestItems_SearchPaging.GroupDescriptions.Add(new PropertyGroupDescription("SamplingDate"));

                                NotifyOfPropertyChange(() => CV_PCLExamTestItems_SearchPaging);
                                //}

                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                            }

                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    IsLoading = false;
                    Globals.IsBusy = false;
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
            //ObjPatientPCLRequest_SearchPaging.PageIndex = 0;
            //DoPatientPCLRequest_SearchPaging(0, ObjPatientPCLRequest_SearchPaging.PageSize, true);
        }

        public void btClear()
        {
            SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddYears(-1);
            SearchCriteria.ToDate = Globals.GetCurServerDateTime();
            SearchCriteria.PatientCode = "";
            SearchCriteria.FullName = "";
            SearchCriteria.PCLRequestNumID = "";
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            PatientPCLRequest p = eventArgs.Value as PatientPCLRequest;
            btPrint(p.PatientPCLReqID);

        }
        private AllLookupValues.PatientFindBy _PatientFindBy = AllLookupValues.PatientFindBy.CAHAI;
        public AllLookupValues.PatientFindBy PatientFindBy
        {
            get
            {
                return _PatientFindBy;
            }
            set
            {
                if (_PatientFindBy != value)
                {
                    _PatientFindBy = value;
                    NotifyOfPropertyChange(() => PatientFindBy);
                }
            }
        }

        #region Report member

        private RemoteDocumentSource _reportModel;
        public RemoteDocumentSource ReportModel
        {
            get { return _reportModel; }
            set
            {
                _reportModel = value;
                NotifyOfPropertyChange(() => ReportModel);
            }
        }

        public void btPrint(long PatientPCLReqID)
        {
            ReportModel = null;
            ReportModel = new PCLDepartmentLaboratoryResultReportModel().PreviewModel;
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            rParams["parPatientPCLReqID"].Value = (int)PatientPCLReqID;

            // ReportModel.AutoShowParametersPanel = false;
            ReportModel.CreateDocument(rParams);
        }

        #endregion

        private List<Category> _categories;
        public List<Category> categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                NotifyOfPropertyChange(() => categories);
            }
        }

        private void LoadData(string strXML)
        {
            categories = new List<Category>();
            XDocument oDoc = XDocument.Parse(strXML);


            categories = (from info in oDoc.Element("Root").Elements("PCLSections")
                          select new Category
                          {
                              Name = Convert.ToString(info.Element("PCLSectionName").Value),
                              NodeID = Convert.ToInt64(info.Element("PCLSectionID").Value),
                              Level = 1,
                              ParentCategory = null,
                              SubCategory = (from info1 in info.Elements("PCLExamTypes")
                                             select new Category
                                             {
                                                 Name = Convert.ToString(info1.Element("PCLExamTypeName").Value),
                                                 NodeID = Convert.ToInt64(info1.Element("PCLExamTypeID").Value),
                                                 Level = 2,
                                                 PCLExamTypeID = Convert.ToInt64(info1.Element("PCLExamTypeID").Value),
                                                 CountItem = Convert.ToInt32(info1.Element("CountItem").Value),
                                                 SubCategory = (from info2 in info1.Elements("PCLExamTestItems")
                                                                select new Category
                                                                {
                                                                    Name = info2.Element("PCLExamTestItemName").Value != null ? info2.Element("PCLExamTestItemName").Value : "",
                                                                    NodeID = Convert.ToInt64(info2.Element("PCLExamTestItemID").Value),
                                                                    Level = 3,
                                                                    CountItem = 1,
                                                                    PCLExamTypeID = Convert.ToInt64(info2.Parent.Element("PCLExamTypeID").Value)

                                                                }).ToList()
                                             }).ToList()
                          }).ToList();
        }

        private int MaxPageSize = 20;
        private int ParaSize = 50;//Dung khi di try van

        private int _TotalRow = 0;
        public int TotalRow
        {
            get { return _TotalRow; }
            set
            {
                _TotalRow = value;
                NotifyOfPropertyChange(() => TotalRow);
                NotifyOfPropertyChange(() => IsEnableNext);
            }
        }

        private int _PageIndex = 0;
        public int PageIndex
        {
            get { return _PageIndex; }
            set
            {
                _PageIndex = value;
                NotifyOfPropertyChange(() => PageIndex);
                NotifyOfPropertyChange(() => IsEnablePrev);
                NotifyOfPropertyChange(() => IsEnableNext);
            }
        }

        private int _PageSize = 20;
        public int PageSize
        {
            get { return _PageSize; }
            set
            {
                _PageSize = value;
                NotifyOfPropertyChange(() => PageSize);
                NotifyOfPropertyChange(() => IsEnableNext);
            }
        }

        public bool IsEnablePrev
        {
            get { return PageIndex > 0; }
        }

        public bool IsEnableNext
        {
            get { return (TotalRow / PageSize > PageIndex); }
        }
        //▼===== #003: Đặt giá trị mặc định là 1 năm khi tìm kiếm
        private DateTime? _fromDate = Globals.GetCurServerDateTime().AddYears(-1);
        public DateTime? FromDate
        {
            get { return _fromDate; }
            set
            {
                _fromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }

        private DateTime? _toDate = Globals.GetCurServerDateTime();
        public DateTime? ToDate
        {
            get { return _toDate; }
            set
            {
                _toDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }
        //▲===== #003
        private void GetPatientPCLLaboratoryResults_ByExamTest()
        {
            if (Registration_DataStorage.CurrentPatient == null)
            {
                MessageBox.Show(eHCMSResources.K0290_G1_ChonBN);
                return;
            }
            if (FromDate != null && ToDate != null)
            {
                if (AxHelper.CompareDate(FromDate.GetValueOrDefault(), ToDate.GetValueOrDefault()) == 1)
                {
                    MessageBox.Show(eHCMSResources.Z0394_G1_NgNhapKgHopLe);
                    return;
                }
            }
            PatientPCLLaboratoryResults_ByExamTest_Paging(Registration_DataStorage.CurrentPatient.PatientID, PCLExamTestItemID, PCLExamTypeID, FromDate, ToDate, PageIndex, ParaSize);
        }

        public void PrevClick()
        {
            if (PageIndex > 0)
            {
                PageIndex = PageIndex - 1;
            }
            GetPatientPCLLaboratoryResults_ByExamTest();
        }

        public void NextClick()
        {
            if (PageIndex < TotalRow / PageSize)
            {
                PageIndex = PageIndex + 1;
            }
            GetPatientPCLLaboratoryResults_ByExamTest();
        }
        long PCLExamTestItemID = 0;
        long PCLExamTypeID = 0;
        public void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            int CountItem = 0;
            if (e != null && e.NewValue != null)
            {
                Category item = e.NewValue as Category;
                if (item.Level == 2)
                {
                    CountItem = item.CountItem + 1;
                    if (CountItem > MaxPageSize)
                    {
                        ParaSize = 1;
                        PageSize = CountItem;
                    }
                    else
                    {
                        ParaSize = MaxPageSize / CountItem;
                        PageSize = CountItem * ParaSize;
                    }
                    PCLExamTestItemID = 0;
                    PCLExamTypeID = item.NodeID;
                    PageIndex = 0;
                    GetPatientPCLLaboratoryResults_ByExamTest();
                }
                else if (item.Level == 3)
                {
                    // CountItem = item.CountItem + 1;
                    PCLExamTestItemID = item.NodeID;
                    PCLExamTypeID = item.PCLExamTypeID;
                    PageIndex = 0;
                    PageSize = MaxPageSize;
                    ParaSize = PageSize;
                    GetPatientPCLLaboratoryResults_ByExamTest();
                }
            }

        }

        List<PCLExamTestItems> GraphTestItems;
        private void InitGraphCollectionData(GenericCoRoutineTask aGenTask, object aPatientID, object aPCLExamTestItemID, object aPCLExamTypeID)
        {
            long PatientID = Convert.ToInt64(aPatientID);
            long PCLExamTestItemID = Convert.ToInt64(aPCLExamTestItemID);
            long PCLExamTypeID = Convert.ToInt64(aPCLExamTypeID);
            var t = new Thread(() =>
            {
                try
                {
                    IsLoading = true;
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPatientPCLLaboratoryResults_ByExamTest_Paging(PatientID, PCLExamTestItemID, PCLExamTypeID, FromDate, ToDate, PageIndex, PageSize, (long)PatientFindBy, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int TotalRow = 0;
                                int MaxRow = 0;
                                var ItemCollection = client.EndPatientPCLLaboratoryResults_ByExamTest_Paging(out TotalRow, out MaxRow, asyncResult);
                                if (ItemCollection != null && ItemCollection.Count > 0)
                                {
                                    GraphTestItems.AddRange(ItemCollection);
                                }
                            }
                            catch
                            {
                            }
                            finally
                            {
                                aGenTask.ActionComplete(true);
                            }
                        }), null);
                    }
                }
                catch
                {
                    aGenTask.ActionComplete(true);
                }
            });
            t.Start();
        }
        private IEnumerator<IResult> BuildGraph(List<Category> CategoryCollection)
        {
            if (Registration_DataStorage.CurrentPatient == null)
            {
                MessageBox.Show(eHCMSResources.K0290_G1_ChonBN);
                yield break;
            }
            if (FromDate != null && ToDate != null)
            {
                if (AxHelper.CompareDate(FromDate.GetValueOrDefault(), ToDate.GetValueOrDefault()) == 1)
                {
                    MessageBox.Show(eHCMSResources.Z0394_G1_NgNhapKgHopLe);
                    yield break;
                }
            }
            var PatientID = Registration_DataStorage.CurrentPatient.PatientID;
            GraphTestItems = new List<PCLExamTestItems>();
            this.ShowBusyIndicator();
            PageIndex = 0;
            PageSize = MaxPageSize;
            ParaSize = PageSize;
            foreach (var aItem in CategoryCollection)
            {
                yield return GenericCoRoutineTask.StartTask(InitGraphCollectionData, PatientID, aItem.NodeID, aItem.PCLExamTypeID);
            }
            this.HideBusyIndicator();
            if (GraphTestItems != null && GraphTestItems.Count > 0)
            {
                GlobalsNAV.ShowDialog<IIPCLResultGraph>((aView) =>
                {
                    aView.PCLExamTestItemCollection = GraphTestItems as IList<PCLExamTestItems>;
                });
            }
        }
        public void GraphClick()
        {
            if (categories == null || categories.Count == 0)
            {
                return;
            }
            var SelectedItemCollection = categories.SelectMany(x => x.SubCategory).SelectMany(x => x.SubCategory).Where(x => x.IsChecked).ToList();
            if (SelectedItemCollection != null && SelectedItemCollection.Count > 0)
            {
                Coroutine.BeginExecute(BuildGraph(SelectedItemCollection));
            }
            else
            {
                GlobalsNAV.ShowDialog<IIPCLResultGraph>((aView) =>
                {
                    aView.PCLExamTestItemCollection = CVS_PCLExamTestItems_SearchPaging.Source as IList<PCLExamTestItems>;
                });
            }
        }
        //▼====== #002
        public void setDefaultWhenRenewPatient()
        {
            CV_PCLExamTestItems_SearchPaging = new CollectionView(new List<PCLExamTestItems>());
            categories = new List<Category>();
        }
        //▲====== #002
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
            Globals.EventAggregator.Subscribe(this);
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
            }
        }
        //▼===== #003
        public void btnSearch ()
        {
            InitData();
        }
        public void rdtNgoaiTru_Checked(RoutedEventArgs e)
        {
            SearchCriteria.PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
            PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
        }
        public void rdtNoiTru_Checked(RoutedEventArgs e)
        {
            SearchCriteria.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
        }
        public void rdtAll_Checked(RoutedEventArgs e)
        {
            SearchCriteria.PatientFindBy = AllLookupValues.PatientFindBy.CAHAI;
            PatientFindBy = AllLookupValues.PatientFindBy.CAHAI;
        }
        //▲=====  #003
    }

    public class Category
    {
        public bool IsChecked { get; set; }
        public bool IsLevel3Item
        {
            get
            {
                return Level == 3;
            }
        }
        public string Name { get; set; }
        public long NodeID { get; set; }
        public long PCLExamTypeID { get; set; }
        public int Level { get; set; }
        public int CountItem { get; set; }
        public List<Category> SubCategory { get; set; }
        public Category ParentCategory { get; set; }
    }
}