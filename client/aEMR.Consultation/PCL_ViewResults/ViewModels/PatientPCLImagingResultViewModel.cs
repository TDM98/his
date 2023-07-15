using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Windows;
using System.Windows.Controls;
using DataEntities;
using aEMR.Common;
using aEMR.DataContracts;
using aEMR.CommonTasks;
using System.Collections.Generic;
using aEMR.Common.Collections;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using aEMR.ServiceClient;
using System.Linq;
using aEMR.Controls;
using Castle.Windsor;
using aEMR.Common.BaseModel;
/*
 * 20180923 #001 TTM: 
 * 20190815 #002 TTM:   BM 0013133: Không load lại màn hình kết quả khi tìm đăng ký mới. Dẫn tới có thể gây nhầm lẫn kết quả.
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IPatientPCLImagingResult)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientPCLImagingResultViewModel : ViewModelBase, IPatientPCLImagingResult
        //, IHandle<ShowPatientInfo_KHAMBENH_CLS_KETQUA_HINHANH<Patient, PatientRegistration, PatientRegistrationDetail>>
        , IHandle<InitDataForPtPCLImagingResult>
        , IHandle<ItemSelected<PatientPCLImagingResult, ObservableCollection<PCLResultFileStorageDetail>>>
    {
        #region Properties member

        public object UCDoctorProfileInfo { get; set; }

        public object UCPatientProfileInfo { get; set; }

        private object _UCMainContent;
        public object UCMainContent
        {
            get { return _UCMainContent; }
            set
            {
                _UCMainContent = value;
                NotifyOfPropertyChange(() => UCMainContent);
            }
        }


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


        private PagedSortableCollectionView<DataEntities.PatientPCLRequestDetail> _ObjPatientPCLRequest_SearchPaging;
        public PagedSortableCollectionView<DataEntities.PatientPCLRequestDetail> ObjPatientPCLRequest_SearchPaging
        {
            get { return _ObjPatientPCLRequest_SearchPaging; }
            set
            {
                _ObjPatientPCLRequest_SearchPaging = value;
                NotifyOfPropertyChange(() => ObjPatientPCLRequest_SearchPaging);
            }
        }

        private ObservableCollection<PCLExamTypeSubCategory> _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory;
        public ObservableCollection<PCLExamTypeSubCategory> ObjPCLExamTypeSubCategory_ByV_PCLMainCategory
        {
            get { return _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory; }
            set
            {
                _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypeSubCategory_ByV_PCLMainCategory);
            }
        }

        private ObservableCollection<DataEntities.PCLResultParamImplementations> ObjPCLResultParamImplementations_GetAll_Root;

        private ObservableCollection<DataEntities.PCLResultParamImplementations> _ObjPCLResultParamImplementations_GetAll;
        public ObservableCollection<DataEntities.PCLResultParamImplementations> ObjPCLResultParamImplementations_GetAll
        {
            get { return _ObjPCLResultParamImplementations_GetAll; }
            set
            {
                _ObjPCLResultParamImplementations_GetAll = value;
                NotifyOfPropertyChange(() => ObjPCLResultParamImplementations_GetAll);
            }
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

        private PatientPCLImagingResult _CurrentPCLImagingResult;
        public PatientPCLImagingResult CurrentPCLImagingResult
        {
            get
            {
                return _CurrentPCLImagingResult;
            }
            set
            {
                if (_CurrentPCLImagingResult == value)
                {
                    return;
                }
                _CurrentPCLImagingResult = value;
                NotifyOfPropertyChange(() => CurrentPCLImagingResult);
            }
        }
        #endregion
        [ImportingConstructor]
        public PatientPCLImagingResultViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            CreateSubVM();
            SearchCriteria = new PatientPCLRequestSearchCriteria();
            SearchCriteria.PCLExamTypeLocationsDeptLocationID = -1;
            SearchCriteria.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Imaging;
            SearchCriteria.PCLExamTypeSubCategoryID = null;
            SearchCriteria.PCLResultParamImpID = null;
            SearchCriteria.FromDate = Globals.ServerDate.Value.AddDays(-365);
            SearchCriteria.ToDate = Globals.ServerDate;
            SearchCriteria.PatientFindBy = AllLookupValues.PatientFindBy.CAHAI;
            
            //▼===== 20190927 TTM:  Comment lại vì không còn màn hình nữa mà đã chuyển vào tab nên chỉ có 1 lần đầu tiên init dữ liệu từ hàm khởi tạo mà lúc này chưa có thông tin do chưa
            //                      tìm kiếm bệnh nhân => Dòng code này không còn ý nghĩa set dữ liệu ban đầu nữa.
            SearchCriteria.PatientID = Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null ? Registration_DataStorage.CurrentPatient.PatientID : 0;
            //▲===== 

            ObjPatientPCLRequest_SearchPaging = new PagedSortableCollectionView<PatientPCLRequestDetail>();
            //20180725 TLB: Comment lai vi du lieu bi double
            //ObjPatientPCLRequest_SearchPaging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPatientPCLRequest_SearchPaging_OnRefresh);
            UCPatientPCLGeneralResult = Globals.GetViewModel<IPatientPCLGeneralResult>();
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
                //▼===== #002
                setNewWhenReload();
                //▲===== #002
            }
            else
            {
                SearchCriteria.PatientID = 0;
            }
        }
        //▲====== #001
        // TxD 22/09/2018 Added the following to ALLOW child VM of each Tab to get ACCESS to the CENTRAL Data stored in the MainKB (Main KHAM BENH) Screen
        private ICS_DataStorage _CS_DS = null;
        public ICS_DataStorage CS_DS
        {
            get
            {
                return _CS_DS;
            }
            set
            {
                _CS_DS = value;
                ((IPatientInfo)UCPatientProfileInfo).CS_DS = CS_DS;
            }
        }

        private void CreateSubVM()
        {
            UCDoctorProfileInfo = Globals.GetViewModel<ILoginInfo>();

            UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();
            UCPatientPCLImagingItemResult = Globals.GetViewModel<IPatientPCLImagingItemResult>();
        }

        private void ActivateSubVM()
        {
            ActivateItem(UCDoctorProfileInfo);
            ActivateItem(UCPatientProfileInfo);
            ActivateItem(UCPatientPCLGeneralResult);
            ActivateItem(UCPatientPCLImagingItemResult);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            ActivateSubVM();
            if (!IsDialogView)
            {
                Coroutine.BeginExecute(DoPatientPCLRequest_SearchPaging(ObjPatientPCLRequest_SearchPaging.PageIndex, ObjPatientPCLRequest_SearchPaging.PageSize, true));
                ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = new ObservableCollection<PCLExamTypeSubCategory>();
                PCLExamTypeSubCategory_ByV_PCLMainCategory();
                PCLResultParamImplementations_GetAll();
            }
        }
        protected override void OnDeactivate(bool close)
        {
            Globals.EventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

        void ObjPatientPCLRequest_SearchPaging_OnRefresh(object sender, RefreshEventArgs e)
        {
            Coroutine.BeginExecute(DoPatientPCLRequest_SearchPaging(ObjPatientPCLRequest_SearchPaging.PageIndex, ObjPatientPCLRequest_SearchPaging.PageSize, true));
        }


        private IEnumerator<IResult> DoPatientPCLRequest_SearchPaging(int PageIndex, int PageSize, bool CountTotal = false)
        {
            if (Registration_DataStorage == null || Registration_DataStorage.CurrentPatient == null)
            {
                yield break;
            }

            ObjPatientPCLRequest_SearchPaging.Clear();

            IsLoading = true;

            var loadRegInfoTask = new PCLRequest_ViewResults_SearchTask(SearchCriteria, PageIndex, PageSize, CountTotal);
            yield return loadRegInfoTask;

            IsLoading = false;

            if (loadRegInfoTask.Error == null)
            {
                if (loadRegInfoTask.CountTotal)
                {
                    ObjPatientPCLRequest_SearchPaging.TotalItemCount = loadRegInfoTask.TotalItemCount;
                }
                if (loadRegInfoTask.PatientPclRequestList != null)
                {
                    foreach (var item in loadRegInfoTask.PatientPclRequestList)
                    {
                        PatientPCLRequestDetail itemdetail = new PatientPCLRequestDetail();
                        itemdetail.PCLReqItemID = item.PCLReqItemID.GetValueOrDefault(0);
                        itemdetail.PCLExamTypeID = item.PCLExamTypeID.GetValueOrDefault(0);
                        itemdetail.PatientPCLRequest = item;
                        ObjPatientPCLRequest_SearchPaging.Add(itemdetail);
                    }
                }
            }
            else
            {
                Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = new AxErrorEventArgs(loadRegInfoTask.Error) });
            }
            yield break;
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            PatientPCLRequestDetail p = eventArgs.Value as PatientPCLRequestDetail;
            ChonPhieuDiXetNghiem(p.PatientPCLRequest);
        }

        public void PCLExamTypeSubCategory_ByV_PCLMainCategory()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPCLExamTypeSubCategory_ByV_PCLMainCategory((long)AllLookupValues.V_PCLMainCategory.Imaging, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndPCLExamTypeSubCategory_ByV_PCLMainCategory(asyncResult);
                            if (items != null)
                            {
                                ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = new ObservableCollection<PCLExamTypeSubCategory>(items);
                                PCLExamTypeSubCategory firstItem = new PCLExamTypeSubCategory();
                                firstItem.PCLExamTypeSubCategoryID = -1;
                                firstItem.PCLSubCategoryName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2074_G1_ChonNhom2);
                                ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Insert(0, firstItem);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }

        private void PCLResultParamImplementations_GetAll()
        {
            Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format("{0}...", eHCMSResources.Z0533_G1_DSPCLResultParam) });

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLResultParamImplementations_GetAll(Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<DataEntities.PCLResultParamImplementations> allItems = null;

                            try
                            {
                                allItems = client.EndPCLResultParamImplementations_GetAll(asyncResult);
                                ObjPCLResultParamImplementations_GetAll_Root = new ObservableCollection<DataEntities.PCLResultParamImplementations>(allItems);

                                PCLResultParamImplementations firstItem = new PCLResultParamImplementations();
                                firstItem.PCLResultParamImpID = -1;
                                firstItem.ParamName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2092_G1_ChonPCLresultparam2);
                                ObjPCLResultParamImplementations_GetAll_Root.Insert(0, firstItem);

                                ObjPCLResultParamImplementations_GetAll = ObjPCLResultParamImplementations_GetAll_Root.DeepCopy();

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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }

        #region Load Content BY ENUM

        private void LoadContentByParaEnum(PatientPCLRequest p)
        {
            DeactivateItem(UCMainContent, true);
            switch (p.ParamEnum)
            {
                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_TIMMAU:
                    {
                        var VM = Globals.GetViewModel<ISieuAmTim_Consultation>();
                        UCMainContent = VM;
                        this.ActivateItem(VM);
                        break;
                    }
                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_MACHMAU:
                    {
                        var VM = Globals.GetViewModel<ISieuAmMachMauHome_Consultation>();

                        UCMainContent = VM;
                        this.ActivateItem(VM);
                        break;
                    }
                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_GANGSUC_Dipyridamole:
                    {
                        var VM = Globals.GetViewModel<ISieuAmTimGangSucDipyridamoleHome_Consultation>();

                        UCMainContent = VM;
                        this.ActivateItem(VM);
                        break;
                    }
                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_GANGSUC_Dobutamine:
                    {
                        var VM = Globals.GetViewModel<ISieuAmTimGangSucDobutamineHome_Consultation>();

                        UCMainContent = VM;
                        this.ActivateItem(VM);
                        break;
                    }
                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_TIMTHAI:
                    {
                        var VM = Globals.GetViewModel<ISieuAmTimThaiHome_Consultation>();

                        UCMainContent = VM;
                        this.ActivateItem(VM);
                        break;
                    }
                case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_THUCQUAN:
                    {
                        var VM = Globals.GetViewModel<ISieuAmTimQuaThucQuanHome_Consultation>();

                        UCMainContent = VM;
                        this.ActivateItem(VM);
                        break;
                    }
                default:
                    {
                        var VM = Globals.GetViewModel<IPCLDeptImagingResult_Consultation>();
                        VM.IsEdit = false;
                        VM.Registration_DataStorage = Registration_DataStorage;
                        UCMainContent = VM;
                        this.ActivateItem(VM);
                        break;
                    }
            }
        }

        #endregion

        private void ChonPhieuDiXetNghiem(PatientPCLRequest p)
        {
            CurrentPCLImagingResult = null;
            Globals.PatientPCLReqID_Imaging = (p).PatientPCLReqID;
            Globals.PatientPCLReqID_LAB = (p).PatientPCLReqID;
            Globals.PatientPCLRequest_Imaging = p;
            LoadContentByParaEnum(p);
            p.Patient = CS_DS == null ? null : CS_DS.CurrentPatient;
            Globals.EventAggregator.Publish(new PCLDeptImagingResultLoadEvent { PatientPCLRequest_Imaging = p });
            //Globals.EventAggregator.Publish(new ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest> { PCLRequest = p });
        }
        public void LoadDataCoroutineEx(PatientPCLRequest aPCLRequest)
        {
            ChonPhieuDiXetNghiem(aPCLRequest);
            if (UCMainContent != null && UCMainContent is IPCLDeptImagingResult_Consultation)
            {
                (UCMainContent as IPCLDeptImagingResult_Consultation).LoadDataCoroutineEx(aPCLRequest);
            }
        }
        public void btSearch()
        {
            if (Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                ObjPatientPCLRequest_SearchPaging.PageIndex = 0;
                ObjPatientPCLRequest_SearchPaging.PageSize = 10000;
                Coroutine.BeginExecute(DoPatientPCLRequest_SearchPaging(ObjPatientPCLRequest_SearchPaging.PageIndex, ObjPatientPCLRequest_SearchPaging.PageSize));
                UCMainContent = null;
            }
            else
            {
                MessageBox.Show(eHCMSResources.K0290_G1_ChonBN);
            }
        }

        public void cboPCLExamTypeSubCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AxComboBox cb = sender as AxComboBox;
            if (cb != null)
            {
                long? id = cb.SelectedValueEx as long?;
                if (id > 0)
                {
                    if (ObjPCLResultParamImplementations_GetAll_Root != null)
                    {
                        ObjPCLResultParamImplementations_GetAll = ObjPCLResultParamImplementations_GetAll_Root.Where(x => x.PCLExamTypeSubCategoryID == id).ToObservableCollection().DeepCopy();
                        return;
                    }
                }
            }
            ObjPCLResultParamImplementations_GetAll = ObjPCLResultParamImplementations_GetAll_Root.DeepCopy();
        }

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
        public void Handle(InitDataForPtPCLImagingResult message)
        {
            if (Registration_DataStorage.CurrentPatient != null)
            {
                SearchCriteria.PatientID = Registration_DataStorage.CurrentPatient != null ? Registration_DataStorage.CurrentPatient.PatientID : 0;
                Coroutine.BeginExecute(DoPatientPCLRequest_SearchPaging(ObjPatientPCLRequest_SearchPaging.PageIndex, ObjPatientPCLRequest_SearchPaging.PageSize, true));
            }
        }

        //KMx: Đổi tên event, không sử dụng chung nữa, vì khó debug, event bắn không kiểm soát được (25/05/2014 11:00).
        //public void Handle(ShowPatientInfo<Patient, PatientRegistration, PatientRegistrationDetail> message)
        //{
        //    if (message != null)
        //    {
        //        if (Registration_DataStorage.CurrentPatient != null)
        //        {
        //            SearchCriteria.PatientID = Registration_DataStorage.CurrentPatient != null ? Registration_DataStorage.CurrentPatient.PatientID : 0;
        //            Coroutine.BeginExecute(DoPatientPCLRequest_SearchPaging(ObjPatientPCLRequest_SearchPaging.PageIndex, ObjPatientPCLRequest_SearchPaging.PageSize, true));
        //        }
        //    }
        //}

        //KMx: Đổi tên event, không sử dụng chung nữa, vì khó debug, event bắn không kiểm soát được (25/05/2014 11:00).
        //public void Handle(ShowPatientInfo_KHAMBENH_CLS_KETQUA_HINHANH<Patient, PatientRegistration, PatientRegistrationDetail> message)
        //{
        //    if (message != null)
        //    {
        //        if (Registration_DataStorage.CurrentPatient != null)
        //        {
        //            SearchCriteria.PatientID = Registration_DataStorage.CurrentPatient != null ? Registration_DataStorage.CurrentPatient.PatientID : 0;
        //            Coroutine.BeginExecute(DoPatientPCLRequest_SearchPaging(ObjPatientPCLRequest_SearchPaging.PageIndex, ObjPatientPCLRequest_SearchPaging.PageSize, true));
        //        }
        //    }
        //}

        private bool _IsShowSummaryContent = true;
        private IPatientPCLGeneralResult _UCPatientPCLGeneralResult;
        private long _V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
        public bool IsShowSummaryContent
        {
            get => _IsShowSummaryContent; set
            {
                _IsShowSummaryContent = value;
                NotifyOfPropertyChange(() => IsShowSummaryContent);
            }
        }
        public IPatientPCLGeneralResult UCPatientPCLGeneralResult
        {
            get
            {
                return _UCPatientPCLGeneralResult;
            }
            set
            {
                _UCPatientPCLGeneralResult = value;
                NotifyOfPropertyChange(() => UCPatientPCLGeneralResult);
            }
        }
        public long V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                if (_V_RegistrationType == value)
                {
                    return;
                }
                _V_RegistrationType = value;
                NotifyOfPropertyChange(() => V_RegistrationType);
            }
        }
        private IPatientPCLImagingItemResult _UCPatientPCLImagingItemResult;
        public IPatientPCLImagingItemResult UCPatientPCLImagingItemResult
        {
            get => _UCPatientPCLImagingItemResult; set
            {
                _UCPatientPCLImagingItemResult = value;
                NotifyOfPropertyChange(() => UCPatientPCLImagingItemResult);
            }
        }
        public void Handle(ItemSelected<PatientPCLImagingResult, ObservableCollection<PCLResultFileStorageDetail>> message)
        {
            if (message == null || message.Sender == null || UCPatientPCLGeneralResult == null)
            {
                return;
            }
            CurrentPCLImagingResult = message.Sender;
            UCPatientPCLGeneralResult.HIRepResourceCode = new ObservableCollection<Resources>();
            if (message.Sender != null && message.Sender.HIRepResourceCode != null && message.Sender.PerformStaffID != null && message.Sender.PerformStaffFullName != null)
            {
                UCPatientPCLGeneralResult.HIRepResourceCode = new ObservableCollection<Resources> { new Resources { HIRepResourceCode = message.Sender.HIRepResourceCode } };
                UCPatientPCLGeneralResult.ObjPatientPCLImagingResult_General.HIRepResourceCode = message.Sender.HIRepResourceCode;
                UCPatientPCLGeneralResult.aucHoldConsultDoctor.setDefault(message.Sender.PerformStaffFullName);
                UCPatientPCLGeneralResult.aucHoldConsultDoctor.StaffID = message.Sender.PerformStaffID.GetValueOrDefault(0);
                UCPatientPCLGeneralResult.ObjPatientPCLImagingResult_General.Suggest = message.Sender.Suggest;
            }
            else
            {
                UCPatientPCLGeneralResult.ObjPatientPCLImagingResult_General.Suggest = "";
            }
            if (message.Sender != null && message.Sender.ResultStaffID != null && message.Sender.ResultStaffFullName != null)
            {
                UCPatientPCLGeneralResult.aucDoctorResult.setDefault(message.Sender.ResultStaffFullName);
                UCPatientPCLGeneralResult.aucDoctorResult.StaffID = message.Sender.ResultStaffID.GetValueOrDefault(0);
            }
            else
            {
                UCPatientPCLGeneralResult.aucDoctorResult.setDefault();
                UCPatientPCLGeneralResult.aucDoctorResult.StaffID = 0;
            }
            // 20200514 TNHX: Lấy cột HiddenFullNameOnReport để ẩn bsy thực hiện kết quả CLS (HA)
            Staff PerfromStaff = Globals.AllStaffs.Where(x => x.StaffID == message.Sender.PerformStaffID.GetValueOrDefault(0)).FirstOrDefault();
            string PerformStaffFullName = message.Sender.PerformStaffFullName ?? "";
            if (PerfromStaff != null && PerfromStaff.HiddenFullNameOnReport)
            {
                PerformStaffFullName = "";
            }
            UCPatientPCLGeneralResult.ApplyElementValues(message.Sender.TemplateResultString, message.Sender.PatientPCLRequest, message.Item, message.Sender.PtRegistrationCode, PerformStaffFullName, message.Sender.Suggest);

            if (message.Sender != null && message.Sender.HIRepResourceCode != null)
            {
                UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.HIRepResourceCode = message.Sender.HIRepResourceCode;
            }
            else
            {
                UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.HIRepResourceCode = null;
            }
            UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.Suggest = message.Sender.Suggest;
            if (message.Sender != null && message.Sender.PerformStaffID != null && message.Sender.PerformStaffFullName != null)
            {
                UCPatientPCLImagingItemResult.aucHoldConsultDoctor.setDefault(message.Sender.PerformStaffFullName);
                UCPatientPCLImagingItemResult.aucHoldConsultDoctor.StaffID = (long)message.Sender.PerformStaffID;
            }
            else
            {
                UCPatientPCLImagingItemResult.aucHoldConsultDoctor.setDefault();
                UCPatientPCLImagingItemResult.aucHoldConsultDoctor.StaffID = 0;
            }
            if (message.Sender != null && message.Sender.ResultStaffID != null && message.Sender.ResultStaffFullName != null)
            {
                UCPatientPCLImagingItemResult.aucDoctorResult.setDefault(message.Sender.ResultStaffFullName);
                UCPatientPCLImagingItemResult.aucDoctorResult.StaffID = (long)message.Sender.ResultStaffID;
            }
            else
            {
                UCPatientPCLImagingItemResult.aucDoctorResult.setDefault();
                UCPatientPCLImagingItemResult.aucDoctorResult.StaffID = 0;
            }
            if (message.Sender != null && message.Sender.PCLImgResultID > 0 && !string.IsNullOrEmpty(message.Sender.TemplateResult))
            {
                UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.TemplateResult = message.Sender.TemplateResult;
            }
            else
            {
                UCPatientPCLImagingItemResult.ObjPatientPCLImagingResult_General.TemplateResult = "";
            }
            if (UCPatientPCLImagingItemResult != null && UCPatientPCLImagingItemResult.allPatientPCLImagingResultDetail != null && UCPatientPCLImagingItemResult.allPatientPCLImagingResultDetail.Count > 0)
            {
                UCPatientPCLGeneralResult.ObjPatientPCLImagingResult_General.PatientPCLImagingResultDetail = UCPatientPCLImagingItemResult.allPatientPCLImagingResultDetail.ToList();
            }
        }
        //▼====== #002
        public void setNewWhenReload()
        {
            UCMainContent = null;
            ObjPatientPCLRequest_SearchPaging = new PagedSortableCollectionView<PatientPCLRequestDetail>();
            UCPatientPCLGeneralResult.RenewWebContent();
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
                SearchCriteria.PatientID = (Registration_DataStorage == null || Registration_DataStorage.CurrentPatient == null ? 0 : Registration_DataStorage.CurrentPatient.PatientID);
            }
        }
    }
}