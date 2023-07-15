using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using eHCMS.CommonUserControls.CommonTasks;
using System.Linq;
using aEMR.Common.BaseModel;
/*
 * 20220830 #001 TNHX: 2168 Thêm cấu hình thời gian tìm kiếm cho KSK + refactor code + Khi clear dữ liệu thì set lại ngày giờ hiện tại
 */
namespace aEMR.ViewModels
{
    [Export(typeof(ISearchPCLRequestForMedicalExamination)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SearchPCLRequestForMedicalExaminationViewModel : ViewModelBase, ISearchPCLRequestForMedicalExamination
    {
        private ObservableCollection<Lookup> _RequestStatusList;
        public ObservableCollection<Lookup> RequestStatusList
        {
            get { return _RequestStatusList; }
            set
            {
                _RequestStatusList = value;
                NotifyOfPropertyChange(() => RequestStatusList);
            }
        }

        private ObservableCollection<DeptLocation> _locations;
        public ObservableCollection<DeptLocation> Locations
        {
            get
            {
                return _locations;
            }
            set
            {
                _locations = value;
                NotifyOfPropertyChange(() => Locations);
            }
        }

        private bool _isCheckDeptPCL = false;
        public bool isCheckDeptPCL
        {
            get
            {
                return _isCheckDeptPCL;
            }
            set
            {
                if (_isCheckDeptPCL != value)
                {
                    _isCheckDeptPCL = value;
                    if (!_isCheckDeptPCL)
                    {
                        SearchCriteria.PCLExamTypeLocationsDeptLocationID = Globals.DeptLocation.DeptLocationID;
                    }
                }
                NotifyOfPropertyChange(() => isCheckDeptPCL);
            }
        }

        private bool _hasChoiceDept = false;
        public bool hasChoiceDept
        {
            get
            {
                return _hasChoiceDept;
            }
            set
            {
                _hasChoiceDept = value;
                NotifyOfPropertyChange(() => hasChoiceDept);
            }
        }

        public SearchPCLRequestForMedicalExaminationViewModel()
        {
            Globals.EventAggregator.Subscribe(this);

            SearchCriteria = new PatientPCLRequestSearchCriteria();
            ObjPatientPCLRequest_SearchPaging = new PagedSortableCollectionView<PatientPCLRequest>();
            ObjPatientPCLRequest_SearchPaging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPatientPCLRequest_SearchPaging_OnRefresh);
            ObjPatientPCLRequest_SearchPaging_Selected = new PatientPCLRequest();
            GetHospitalClientsData();
            if (Globals.ObjRefDepartment.V_DeptTypeOperation == (long?)V_DeptTypeOperation.KhoaCanLamSang)
            {
                hasChoiceDept = true;
            }
            else
            {
                hasChoiceDept = false;
            }
            Coroutine.BeginExecute(GetRequestStatusList());
        }

        public void LoadData()
        {
            DateTime todayDate = Globals.GetCurServerDateTime();
            SearchCriteria.PCLExamTypeLocationsDeptLocationID = Globals.DeptLocation.DeptLocationID;
            SearchCriteria.V_PCLMainCategory = Globals.PCLDepartment.ObjV_PCLMainCategory != null ? Globals.PCLDepartment.ObjV_PCLMainCategory.LookupID : 0;
            SearchCriteria.PCLExamTypeSubCategoryID = Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID != null ? Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID.PCLExamTypeSubCategoryID : 0;
            SearchCriteria.PCLResultParamImpID = Globals.PCLDepartment.ObjPCLResultParamImpID != null ? Globals.PCLDepartment.ObjPCLResultParamImpID.PCLResultParamImpID : 0;
            //▼====: #001
            SearchCriteria.FromDate = todayDate.AddDays(-Globals.ServerConfigSection.OutRegisElements.NumDayFindOutRegistrationMedicalExamination);
            SearchCriteria.ToDate = todayDate;
            //▲====: #001
            SearchCriteria.PatientFindBy = PatientFindBy;

            ObjPatientPCLRequest_SearchPaging.PageIndex = 0;
            Coroutine.BeginExecute(DoPatientPCLRequest_SearchPaging(0, ObjPatientPCLRequest_SearchPaging.PageSize, true));
        }

        public override void DeactivateItem(object item, bool close)
        {
            base.DeactivateItem(item, close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        private IEnumerator<IResult> GetRequestStatusList()
        {
            var results = new LoadLookupListTask(LookupValues.V_PCLRequestStatus, false, true);
            yield return results;
            RequestStatusList = results.LookupList;

            if (Globals.ObjRefDepartment.V_DeptTypeOperation == (long?)V_DeptTypeOperation.KhoaCanLamSang)
            {
                var deptLoc = new LoadDeptLoctionByIDTask(Globals.ObjRefDepartment.DeptID);
                yield return deptLoc;
                if (Globals.PCLDepartment.ObjV_PCLMainCategory != null
                    && Globals.PCLDepartment.ObjV_PCLMainCategory.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                {
                    Locations = deptLoc.DeptLocations.Where(item => item.Location.RoomType.RmTypeID == Globals.ServerConfigSection.Hospitals.LaboRmTp).ToObservableCollection();
                }
                else
                {
                    Locations = deptLoc.DeptLocations.Where(item => item.Location.RoomType.RmTypeID != Globals.ServerConfigSection.Hospitals.LaboRmTp).ToObservableCollection();
                }
            }
            yield break;
        }

        void ObjPatientPCLRequest_SearchPaging_OnRefresh(object sender, RefreshEventArgs e)
        {
            Coroutine.BeginExecute(DoPatientPCLRequest_SearchPaging(ObjPatientPCLRequest_SearchPaging.PageIndex, ObjPatientPCLRequest_SearchPaging.PageSize, false));
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

        private PagedSortableCollectionView<PatientPCLRequest> _ObjPatientPCLRequest_SearchPaging;
        public PagedSortableCollectionView<PatientPCLRequest> ObjPatientPCLRequest_SearchPaging
        {
            get { return _ObjPatientPCLRequest_SearchPaging; }
            set
            {
                _ObjPatientPCLRequest_SearchPaging = value;
                NotifyOfPropertyChange(() => ObjPatientPCLRequest_SearchPaging);
            }
        }
        private int _AllCasePCL = 0;
        public int AllCasePCL
        {
            get { return _AllCasePCL; }
            set
            {
                _AllCasePCL = value;
                NotifyOfPropertyChange(() => AllCasePCL);
            }
        }


        private void PCLDepartmentSearchPCLRequest_GenTask(GenericCoRoutineTask genTask, object searchCriteria, object pageIndex, object pageSize, object countTotal)
        {
            PatientPCLRequestSearchCriteria SearchCriteria = (PatientPCLRequestSearchCriteria)searchCriteria;
            int PageIndex = (int)pageIndex;
            int PageSize = (int)pageSize;
            bool CountTotal = (bool)countTotal;

            int Total = 0;
            List<PatientPCLRequest> PatientPCLReqList = new List<PatientPCLRequest>();
            this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        bool bActionCompleteOk = false;

                        contract.BeginPatientPCLRequest_SearchPaging_ForMedicalExamination(SearchCriteria, PageIndex, PageSize, "", (bool)countTotal
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    PatientPCLReqList = contract.EndPatientPCLRequest_SearchPaging_ForMedicalExamination(out Total, asyncResult).ToList();
                                    bActionCompleteOk = true;
                                }
                                catch (Exception ex)
                                {
                                    bActionCompleteOk = false;
                                }
                                finally
                                {
                                    genTask.AddResultObj(Total);
                                    genTask.AddResultObj(CountTotal);
                                    genTask.AddResultObj(PatientPCLReqList);
                                    genTask.AddResultObj(CountTotal);
                                    genTask.ActionComplete(bActionCompleteOk);
                                    this.DlgHideBusyIndicator();
                                    if (!bActionCompleteOk)
                                    {
                                        TryClose();
                                    }
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.DlgHideBusyIndicator();
                    genTask.ActionComplete(false);
                    MessageBox.Show(ex.Message, "");
                }
            });

            t.Start();
        }

        private IEnumerator<IResult> DoPatientPCLRequest_SearchPaging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);

            if (HospitalClientSelected != null)
            {
                SearchCriteria.HosClientID = HospitalClientSelected.HosClientID;
            }
            var loadRegInfoTask = new GenericCoRoutineTask(PCLDepartmentSearchPCLRequest_GenTask, SearchCriteria, PageIndex, PageSize, CountTotal);
            yield return loadRegInfoTask;

            int total = Convert.ToInt32(loadRegInfoTask.GetResultObj(0));
            bool countTotal = Convert.ToBoolean(loadRegInfoTask.GetResultObj(1));
            List<PatientPCLRequest> PatientPclRequestList = loadRegInfoTask.GetResultObj(2) as List<PatientPCLRequest>;

            this.DlgHideBusyIndicator();
            ObjPatientPCLRequest_SearchPaging.Clear();

            if (loadRegInfoTask.Error == null)
            {
                if (countTotal)
                {
                    ObjPatientPCLRequest_SearchPaging.TotalItemCount = total;
                    AllCasePCL = total;
                }
                if (PatientPclRequestList != null)
                {
                    foreach (var item in PatientPclRequestList)
                    {
                        ObjPatientPCLRequest_SearchPaging.Add(item);
                    }
                }
            }
            else
            {
                Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = new AxErrorEventArgs(loadRegInfoTask.Error) });
            }
            yield break;
        }

        public void btSearch()
        {
            ObjPatientPCLRequest_SearchPaging.PageIndex = 0;
            Coroutine.BeginExecute(DoPatientPCLRequest_SearchPaging(0, ObjPatientPCLRequest_SearchPaging.PageSize, true));
        }

        public void btCancel()
        {
            TryClose();
        }

        public void btClear()
        {
            //▼====: #001
            SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-Globals.ServerConfigSection.OutRegisElements.NumDayFindOutRegistrationMedicalExamination);            
            SearchCriteria.ToDate = Globals.GetCurServerDateTime();
            //▲====: #001
            SearchCriteria.PatientCode = "";
            SearchCriteria.FullName = "";
            SearchCriteria.PCLRequestNumID = "";
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;

            PatientPCLRequest p = eventArgs.Value as PatientPCLRequest;

            Globals.PatientPCLRequest_Result = p;
            UpdateReceptionTime(p.PatientPCLReqID, Globals.GetCurServerDateTime(), (long)p.V_PCLRequestType);

            ChonPhieuDiXetNghiem(p);
        }

        #region Load Content BY ENUM
        private void LoadContentByParaEnum()
        {
            if (Globals.PCLDepartment.ObjV_PCLMainCategory != null)
            {
                if (Globals.PCLDepartment.ObjV_PCLMainCategory.LookupID !=
                    (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                {
                    switch (Globals.PCLDepartment.ObjPCLResultParamImpID.ParamEnum)
                    {
                        case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_TIMMAU:
                            {
                                var Module = Globals.GetViewModel<IPCLDepartmentContent>();
                                var VM = Globals.GetViewModel<ISieuAmTim>();

                                Module.MainContent = VM;
                                (Module as Conductor<object>).ActivateItem(VM);
                                break;
                            }
                        case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_MACHMAU:
                            {
                                var Module = Globals.GetViewModel<IPCLDepartmentContent>();
                                var VM = Globals.GetViewModel<ISieuAmMachMauHome>();

                                Module.MainContent = VM;
                                (Module as Conductor<object>).ActivateItem(VM);
                                break;
                            }
                        case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_GANGSUC_Dipyridamole:
                            {
                                var Module = Globals.GetViewModel<IPCLDepartmentContent>();
                                var VM = Globals.GetViewModel<ISieuAmTimGangSucDipyridamoleHome>();

                                Module.MainContent = VM;
                                (Module as Conductor<object>).ActivateItem(VM);
                                break;
                            }
                        case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_GANGSUC_Dobutamine:
                            {
                                var Module = Globals.GetViewModel<IPCLDepartmentContent>();
                                var VM = Globals.GetViewModel<ISieuAmTimGangSucDobutamineHome>();

                                Module.MainContent = VM;
                                (Module as Conductor<object>).ActivateItem(VM);
                                break;
                            }
                        case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_TIMTHAI:
                            {
                                var Module = Globals.GetViewModel<IPCLDepartmentContent>();
                                var VM = Globals.GetViewModel<ISieuAmTimThaiHome>();

                                Module.MainContent = VM;
                                (Module as Conductor<object>).ActivateItem(VM);
                                break;
                            }
                        case (int)AllLookupValues.PCLResultParamImpID.SIEUAM_THUCQUAN:
                            {
                                var Module = Globals.GetViewModel<IPCLDepartmentContent>();
                                var VM = Globals.GetViewModel<ISieuAmTimQuaThucQuanHome>();

                                Module.MainContent = VM;
                                (Module as Conductor<object>).ActivateItem(VM);
                                break;
                            }
                        default:
                            {
                                var Module = Globals.GetViewModel<IPCLDepartmentContent>();
                                var VM = Globals.GetViewModel<IPCLDeptImagingResult>();

                                Module.MainContent = VM;
                                (Module as Conductor<object>).ActivateItem(VM);
                                break;
                            }
                    }
                }
            }
        }
        #endregion

        public AllLookupValues.PatientFindBy PatientFindBy { get; set; }

        public void btOK()
        {
            if (ObjPatientPCLRequest_SearchPaging_Selected != null && ObjPatientPCLRequest_SearchPaging_Selected.PatientPCLReqID > 0)
            {
                ChonPhieuDiXetNghiem(ObjPatientPCLRequest_SearchPaging_Selected);
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0351_G1_Msg_InfoChonPh);
            }
        }

        private void ChonPhieuDiXetNghiem(PatientPCLRequest p)
        {
            Coroutine.BeginExecute(DoGetPatientInfo(p));
        }

        private bool isReadOnly = false;

        private IEnumerator<IResult> DoGetPatientInfo(PatientPCLRequest p)
        {
            if (SearchCriteria.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
            {
                yield return GenericCoRoutineTask.StartTask(GetPatientPCLRequestResultsByReqID, SearchCriteria);
                p = ObjPatientPCLRequest_SearchPaging_Selected;
            }

            string errStr = "";
            if (p.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
            {
                var dialog = new MessageWarningShowDialogTask(eHCMSResources.Z1263_G1_KgMoDuocPhBiHuy, "", false);
                yield return dialog;
                yield break;
            }

            if (p.CheckTraTien(p, ref errStr, Globals.ServerConfigSection.Hospitals.EffectedPCLHours, Globals.ServerConfigSection.Hospitals.EditDiagDays) == false)
            {
                isReadOnly = true;
                var dialog = new MessageWarningShowDialogTask(errStr, "Mở Xem Thông Tin");
                yield return dialog;
                if (!dialog.IsAccept)
                {
                    TryClose();
                    yield break;
                }
            }
            else
            {
                isReadOnly = false;
            }

            this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var activeItem = Globals.GetViewModel<IHome>().ActiveContent;
            ILaboratoryHome LabModule = activeItem as ILaboratoryHome;
            int Temp_PatientFindBy = (int)AllLookupValues.PatientFindBy.NGOAITRU;
            if (p.V_PCLRequestType == AllLookupValues.V_PCLRequestType.NOI_TRU)
            {
                Temp_PatientFindBy = (int)AllLookupValues.PatientFindBy.NOITRU;
            }
            var loadPatients = new LoadPatientTask(p.PatientID, (Temp_PatientFindBy == (int)AllLookupValues.PatientFindBy.NOITRU));
            yield return loadPatients;

            Patient curPatient = loadPatients.CurrentPatient;

            p.Patient = curPatient;

            var loadRegistration = new LoadRegistrationSimpleTask(p.PtRegistrationID, Temp_PatientFindBy);
            yield return loadRegistration;

            PatientRegistration curRegistration = loadRegistration.Registration;

            curPatient.CurrentHealthInsurance = curRegistration.HealthInsurance;

            Globals.EventAggregator.Publish(new ShowPatientInfoFromPopUpSearchPCLRequest() { Patient = curPatient, PtRegistration = curRegistration });

            if (LabModule != null)
            {
                Globals.PatientPCLReqID_LAB = p.PatientPCLReqID;
                Globals.PatientPCLRequest_LAB = p;
                Globals.EventAggregator.Publish(new DbClickSelectedObjectEvent<PatientPCLRequest> { Result = p, IsReadOnly = isReadOnly });
            }
            else
            {
                Globals.PatientPCLReqID_Imaging = p.PatientPCLReqID;
                Globals.PatientPCLRequest_Imaging = p;
                Globals.EventAggregator.Publish(new PCLDeptImagingResultLoadEvent { PatientPCLRequest_Imaging = p });
                Globals.EventAggregator.Publish(new ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest> { PCLRequest = p });
                Globals.EventAggregator.Publish(new Event_SearchAbUltraRequestCompleted() { Patient = curPatient, PCLRequest = p });
            }
            this.HideBusyIndicator();
            TryClose();
            yield break;
        }

        private IEnumerator<IResult> DoLoadLocations(long deptId)
        {
            var deptLoc = new LoadDeptLoctionByIDTask(deptId);
            yield return deptLoc;
            if (deptLoc.DeptLocations != null)
            {
                Locations = new ObservableCollection<DeptLocation>(deptLoc.DeptLocations);
            }
            else
            {
                Locations = new ObservableCollection<DeptLocation>();
            }

            var itemDefault = new DeptLocation();
            itemDefault.Location = new Location();
            itemDefault.Location.LID = -1;
            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0116_G1_HayChonPg);
            Locations.Insert(0, itemDefault);

            yield break;
        }

        public void LoadLocations(long? deptId)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0115_G1_LayDSPgBan);
            var list = new List<refModule>();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLocationsByDeptIDOld(deptId, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var allItems = contract.EndGetAllLocationsByDeptIDOld(asyncResult);

                                if (allItems != null)
                                {
                                    Locations = new ObservableCollection<DeptLocation>(allItems);
                                }
                                else
                                {
                                    Locations = new ObservableCollection<DeptLocation>();
                                }
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetPatientPCLRequestResultsByReqID(GenericCoRoutineTask aGenTask, object aSearchCriteria)
        {
            this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            if (Globals.PatientPCLRequest_Result == null || Globals.PatientPCLRequest_Result.PatientPCLReqID == 0 
                || !(aSearchCriteria != null && aSearchCriteria is PatientPCLRequestSearchCriteria))
            {
                aGenTask.ActionComplete(false);
                this.HideBusyIndicator();
            }
            PatientPCLRequestSearchCriteria mSearchCriteria = aSearchCriteria == null ? new PatientPCLRequestSearchCriteria() : aSearchCriteria as PatientPCLRequestSearchCriteria;
            mSearchCriteria.PatientPCLReqID = Globals.PatientPCLRequest_Result.PatientPCLReqID;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetPatientPCLRequestResultsByReqID(mSearchCriteria, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var mPCLRequest = contract.EndGetPatientPCLRequestResultsByReqID(asyncResult);
                                if (mPCLRequest != null && mPCLRequest.PatientPCLReqID != 0)
                                {
                                    ObjPatientPCLRequest_SearchPaging_Selected = mPCLRequest;
                                    Globals.PatientPCLRequest_Result = ObjPatientPCLRequest_SearchPaging_Selected;
                                    aGenTask.ActionComplete(true);
                                }
                                else
                                {
                                    aGenTask.ActionComplete(false);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                                aGenTask.ActionComplete(false);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        #region Tìm kiếm theo tên công ty
        private HospitalClient _HospitalClientSelected;
        public HospitalClient HospitalClientSelected
        {
            get
            {
                return _HospitalClientSelected;
            }
            set
            {
                if (_HospitalClientSelected == value)
                {
                    return;
                }
                _HospitalClientSelected = value;
                NotifyOfPropertyChange(() => HospitalClientSelected);
            }
        }

        private ObservableCollection<HospitalClient> _HospitalClientCollection;
        public ObservableCollection<HospitalClient> HospitalClientCollection
        {
            get
            {
                return _HospitalClientCollection;
            }
            set
            {
                if (_HospitalClientCollection == value)
                {
                    return;
                }
                _HospitalClientCollection = value;
                NotifyOfPropertyChange(() => HospitalClientCollection);
            }
        }

        private void GetHospitalClientsData()
        {
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var mFactory = new AppointmentServiceClient())
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginGetHospitalClients(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var Collection = mContract.EndGetHospitalClients(asyncResult);
                                if (Collection == null || Collection.Count == 0)
                                {
                                    HospitalClientCollection = new ObservableCollection<HospitalClient>();
                                }
                                else
                                {
                                    HospitalClientCollection = new ObservableCollection<HospitalClient>(Collection);
                                    HospitalClient firstItem = new HospitalClient();
                                    firstItem.HosClientID = 0;
                                    firstItem.CompanyName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    HospitalClientCollection.Insert(0, firstItem);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }
        #endregion

        private void UpdateReceptionTime(long PatientPCLReqID, DateTime ReceptionTime, long V_PCLRequestType)
        {
            this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginUpdateReceptionTime(PatientPCLReqID, ReceptionTime, V_PCLRequestType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                client.EndUpdateReceptionTime(asyncResult);
                            }
                            catch (Exception innerEx)
                            {
                                MessageBox.Show(innerEx.Message);
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
                    MessageBox.Show(ex.Message);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }
    }
}
