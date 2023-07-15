using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using aEMR.CommonTasks;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using eHCMSLanguage;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.Common.BaseModel;

namespace aEMR.ViewModels
{
    [Export(typeof(IPCLDepartmentOutstandingTask)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLDepartmentOutstandingTaskViewModel : ViewModelBase, IPCLDepartmentOutstandingTask
        //, IHandle<ChangePCLDepartmentEvent>
        , IHandle<InitialPCLImage_Step3_Event>
        , IHandle<LocationSelected>
        , IHandle<ReloadOutStandingStaskPCLRequest>
        , IHandle<AddPCLResultFileStorageDetailsComplete>
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


        private bool _isCompleted = true;
        public bool isCompleted
        {
            get { return _isCompleted; }
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    NotifyOfPropertyChange(() => isCompleted);
                }
            }
        }
        public PCLDepartmentOutstandingTaskViewModel()
        {
            Globals.EventAggregator.Subscribe(this);

            SearchCriteria = new PatientPCLRequestSearchCriteria();
            ObjPatientPCLRequest_SearchPaging_Selected = new PatientPCLRequest();
            ObjPatientPCLRequest_SearchPaging = new PagedSortableCollectionView<PatientPCLRequest>();
            ObjPatientPCLRequest_SearchPaging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPatientPCLRequest_SearchPaging_OnRefresh);
            //ShowList();
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

        private IEnumerator<IResult> DoPatientPCLRequest_SearchPaging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.ShowBusyIndicator();
            
            if (!isCompleted)
            {
                SearchCriteria.V_ExamRegStatus = (long?)V_ExamRegStatus.mHoanTat;
            }
            else
            {
                SearchCriteria.V_ExamRegStatus = (long?)V_ExamRegStatus.mDangKyKham;
            }

            var loadRegInfoTask = new PCLDepartmentSearchPCLRequestTask(SearchCriteria, PageIndex, PageSize, CountTotal);
            yield return loadRegInfoTask;

            ClearObjPatientPCLRequest();

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
                        ObjPatientPCLRequest_SearchPaging.Add(item);
                    }

                }
            }
            else
            {
                Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = new AxErrorEventArgs(loadRegInfoTask.Error) });
            }
            
            this.HideBusyIndicator();

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
            string errStr = "";
            
            //if (p.CheckTraTien(p, ref errStr, Globals.EffectedPCLHours, Globals.EditDiagDays) == false)
            //    return;

            // Txd 25/05/2014 Replaced ConfigList
            if (p.CheckTraTien(p, ref errStr, Globals.ServerConfigSection.Hospitals.EffectedPCLHours, Globals.ServerConfigSection.Hospitals.EditDiagDays) == false)
                return;

            //lay thong tin benh nhan,roi phat su kien
            Coroutine.BeginExecute(GetPatientByID(p.PatientID, p));
        }

        public void hplChoose_Click(object selectItem)
        {
            if (selectItem != null)
            {
                string errStr = "";
                PatientPCLRequest p = (selectItem as PatientPCLRequest);

                //if (p.CheckTraTien(p, ref errStr, Globals.EffectedPCLHours, Globals.EditDiagDays) == false)
                
                // Txd 25/05/2014 Replaced ConfigList
                if (p.CheckTraTien(p, ref errStr, Globals.ServerConfigSection.Hospitals.EffectedPCLHours, Globals.ServerConfigSection.Hospitals.EditDiagDays) == false)
                {
                    MessageBox.Show(errStr);
                    return;
                }
                //lay thong tin benh nhan,roi phat su kien
                Coroutine.BeginExecute(GetPatientByID(p.PatientID, p));
            }
        }

        private bool CheckTraTien(PatientPCLRequest p)
        {
            if (p.V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                if (p.PaidTime == null)
                {
                    MessageBox.Show(string.Format("{0} (Ngoại trú) chưa trả tiền cho dịch vụ này. Không thể tiến hành xét nghiệm được!", p.FullName.Trim()));
                    return false;
                }
                else
                {
                    if (p.RefundTime != null)
                    {
                        MessageBox.Show(string.Format("{0} (Ngoại trú) đã hoàn lại tiền! Không thể tiến hành xét nghiệm được!", p.FullName.Trim()));
                        return false;
                    }
                    return true;
                }
            }
            else/*VIP,Nội Trú*/
            {
                //if (p.V_RegistrationType == (long)AllLookupValues.RegistrationType.DANGKY_VIP)
                //{
                //    return true;
                //}
                //else
                {
                    if ((p.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.COMPLETED)
                        &&
                        (p.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.INVALID)
                        //&&
                        //(p.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.LOCKED)
                        &&
                        (p.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.PENDING)
                    )
                    {
                        return true;
                    }
                    else
                    {
                        switch (p.V_RegistrationStatus)
                        {
                            case (long)AllLookupValues.RegistrationStatus.COMPLETED:
                                {
                                    MessageBox.Show("'" + p.Patient.FullName.Trim() + eHCMSResources.A0065_G1_Msg_InfoKhongCDDc_DKDaDong);
                                    break;
                                }
                            case (long)AllLookupValues.RegistrationStatus.INVALID:
                                {
                                    MessageBox.Show("'" + p.Patient.FullName.Trim() + eHCMSResources.A0066_G1_Msg_InfoKhongCDDc_DKKhHopLe);
                                    break;
                                }
                            //case (long)AllLookupValues.RegistrationStatus.LOCKED:
                            //    {
                            //        MessageBox.Show("'" + Globals.PatientAllDetails.PatientInfo.FullName.Trim() + string.Format("'({0})", eHCMSResources.T3713_G1_NoiTru) + Environment.NewLine + "Không thể chẩn đoán được vì đăng ký bị khóa");
                            //        break;
                            //    }
                            case (long)AllLookupValues.RegistrationStatus.PENDING:
                                {
                                    MessageBox.Show("'" + p.Patient.FullName.Trim() + eHCMSResources.A0064_G1_Msg_InfoKhongCDDc_DKChuaHTat);
                                    break;
                                }
                        }
                        return false;
                    }
                }
            }

        }

        private void ShowList()
        {
            // TxD 02/08/2014 Use Globals Server date instead
            //GetCurrentDate();
            if (Globals.PCLDepartment.ObjV_PCLMainCategory == null || Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID == null || Globals.PCLDepartment.ObjPCLResultParamImpID == null)
            {
                return;
            }
            DateTime todayDate = Globals.GetCurServerDateTime();
            SearchCriteria.PatientFindBy = GetPatientFindBy();

            SearchCriteria.PCLExamTypeLocationsDeptLocationID = Globals.DeptLocation.DeptLocationID;
            SearchCriteria.V_PCLMainCategory = Globals.PCLDepartment.ObjV_PCLMainCategory.LookupID;
            SearchCriteria.PCLExamTypeSubCategoryID = Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID.PCLExamTypeSubCategoryID;
            SearchCriteria.PCLResultParamImpID = Globals.PCLDepartment.ObjPCLResultParamImpID.PCLResultParamImpID;
            SearchCriteria.FromDate = todayDate;
            SearchCriteria.ToDate = todayDate;

            ObjPatientPCLRequest_SearchPaging.PageIndex = 0;
            Coroutine.BeginExecute(DoPatientPCLRequest_SearchPaging(0, ObjPatientPCLRequest_SearchPaging.PageSize, true));

        }

        public void Handle(InitialPCLImage_Step3_Event message)
        {
            if (message != null)
            {
                ShowList();
            }
        }

        public void hplRefresh()
        {
            ShowList();
        }

        public void Handle(LocationSelected message)
        {
            if (message != null)
            {
                ShowList();
            }
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


        private AllLookupValues.PatientFindBy GetPatientFindBy()
        {
            var activeItem = Globals.GetViewModel<IHome>().ActiveContent;
            ILaboratoryHome LabModule = activeItem as ILaboratoryHome;
            if (LabModule != null)
            {
                if (Globals.PatientFindBy_ForLab != null)
                    return Globals.PatientFindBy_ForLab.Value;
                return AllLookupValues.PatientFindBy.NGOAITRU;
            }
            else
            {
                return Globals.PatientFindBy_ForImaging.Value;
            }
        }

        public void Handle(ReloadOutStandingStaskPCLRequest message)
        {
            ShowList();

        }

        public void Handle(AddPCLResultFileStorageDetailsComplete message)
        {
            ShowList();
        }

        public void ClearObjPatientPCLRequest()
        {
            ObjPatientPCLRequest_SearchPaging.Clear();
        }

        private void GetPatientPCLRequestResultsByReqID(GenericCoRoutineTask aGenTask, object aSearchCriteria)
        {
            if (ObjPatientPCLRequest_SearchPaging_Selected == null || ObjPatientPCLRequest_SearchPaging_Selected.PatientPCLReqID == 0 || !(aSearchCriteria != null && aSearchCriteria is PatientPCLRequestSearchCriteria))
            {
                aGenTask.ActionComplete(false);
                this.HideBusyIndicator();
            }
            PatientPCLRequestSearchCriteria mSearchCriteria = aSearchCriteria == null ? new PatientPCLRequestSearchCriteria() : aSearchCriteria as PatientPCLRequestSearchCriteria;
            mSearchCriteria.PatientPCLReqID = ObjPatientPCLRequest_SearchPaging_Selected.PatientPCLReqID;
            var t = new Thread(() =>
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
                                //Globals.PatientPCLRequest_Result = ObjPatientPCLRequest_SearchPaging_Selected;
                                aGenTask.ActionComplete(true);
                            }
                            else
                            {
                                aGenTask.ActionComplete(false);
                                this.HideBusyIndicator();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            aGenTask.ActionComplete(false);
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }

        private IEnumerator<IResult> GetPatientByID(long patientID, PatientPCLRequest p)
        {
            this.ShowBusyIndicator();

            if (SearchCriteria.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
            {
                yield return GenericCoRoutineTask.StartTask(GetPatientPCLRequestResultsByReqID, SearchCriteria);
            }

            // 1. Load Patient.
            var loadPatients = new LoadPatientTask(patientID);
            yield return loadPatients;

            //Globals.PatientAllDetails.PatientInfo = loadPatients.CurrentPatient;

            Patient curPatient = loadPatients.CurrentPatient;
            ObjPatientPCLRequest_SearchPaging_Selected.Patient = curPatient;
            Globals.PatientPCLRequest_Result = ObjPatientPCLRequest_SearchPaging_Selected;
            if (ObjPatientPCLRequest_SearchPaging_Selected != null && ObjPatientPCLRequest_SearchPaging_Selected.PtRegistrationID > 0)
            {

                // 2. Load Registration
                int Temp_PatientFindBy = (int)AllLookupValues.PatientFindBy.NGOAITRU;
                if (ObjPatientPCLRequest_SearchPaging_Selected.V_PCLRequestType == AllLookupValues.V_PCLRequestType.NOI_TRU)
                {
                    Temp_PatientFindBy = (int)AllLookupValues.PatientFindBy.NOITRU;
                }
                var loadRegistration = new LoadRegistrationSimpleTask(p.PtRegistrationID, Temp_PatientFindBy);
                yield return loadRegistration;

                PatientRegistration curRegistration = loadRegistration.Registration;

                curPatient.CurrentHealthInsurance = curRegistration.HealthInsurance;
                ObjPatientPCLRequest_SearchPaging_Selected.ExamDate = curRegistration.ExamDate;
                // 3. Fire event to show Patient Info.
                Globals.EventAggregator.Publish(new ShowPatientInfoFromPCLOutStandingTask() { Patient = curPatient, PtRegistration = curRegistration });


                //KMx: Chỉ lấy thông tin bệnh nhân để hiển thị, không cần lấy nhiều như vậy và không được set vào Globals (24/05/2014 14:23).
                //int FindPatient = (int)AllLookupValues.V_FindPatientType.NGOAI_TRU;
                //if (ObjPatientPCLRequest_SearchPaging_Selected.V_RegistrationType == (long)AllLookupValues.RegistrationType.NOI_TRU)
                //{
                //    FindPatient = (int)AllLookupValues.V_FindPatientType.NOI_TRU;
                //}

                //var loadRegistration = new LoadRegistrationInfoTask(ObjPatientPCLRequest_SearchPaging_Selected.PtRegistrationID, FindPatient);
                //yield return loadRegistration;

                //Globals.PatientAllDetails.PatientInfo.CurrentHealthInsurance = loadRegistration.Registration.HealthInsurance;

                //PatientRegistrationDetail ObjPatientRegistrationDetail = new PatientRegistrationDetail();

                //ObjPatientRegistrationDetail.PtRegistrationID = loadRegistration.Registration.PtRegistrationID;

                //Globals.PCLDepartment.SetInfo(Globals.PatientAllDetails.PatientInfo, loadRegistration.Registration, ObjPatientRegistrationDetail);

                ////Show Info
                //Globals.EventAggregator.Publish(new ShowPatientInfo<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = Globals.PatientAllDetails.PatientInfo, PtReg = Globals.PatientAllDetails.PtRegistrationInfo, PtRegDetail = Globals.PatientAllDetails.PtRegistrationDetailInfo });
                ////Show Info
                
            }

            var activeItem = Globals.GetViewModel<IHome>().ActiveContent;
            ILaboratoryHome LabModule = activeItem as ILaboratoryHome;
            if (LabModule != null)
            {
                Globals.PatientPCLReqID_LAB = p.PatientPCLReqID;
                Globals.PatientPCLRequest_LAB = p;
                Globals.EventAggregator.Publish(new DbClickSelectedObjectEvent<PatientPCLRequest> { Result = p });
            }
            else
            {
                Globals.PatientPCLReqID_Imaging = p.PatientPCLReqID;
                //LoadContentByParaEnum();
                Globals.PatientPCLRequest_Imaging = p;
                Globals.EventAggregator.Publish(new PCLDeptImagingResultLoadEvent { PatientPCLRequest_Imaging = p });
                Globals.EventAggregator.Publish(new ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest> { PCLRequest = ObjPatientPCLRequest_SearchPaging_Selected });
                Globals.EventAggregator.Publish(new Event_SearchAbUltraRequestCompleted { Patient = curPatient, PCLRequest = p });
            }
            
            this.HideBusyIndicator();
            
            yield break;

        }

    }
}

//public void GetCurrentDate()
//{
//    var t = new Thread(() =>
//    {
//        try
//        {
//            using (var serviceFactory = new CommonServiceClient())
//            {
//                var contract = serviceFactory.ServiceInstance;

//                contract.BeginGetDate(Globals.DispatchCallback((asyncResult) =>
//                {
//                    try
//                    {
//                        DateTime date = contract.EndGetDate(asyncResult);
//                        SearchCriteria.PatientFindBy = GetPatientFindBy();
//                        //sao phai dung cai nay
//                        //Globals.PCLDepartment.ObjPCLExamTypeLocationsDeptLocationID.DeptLocationID
//                        //khong chinh xac
//                        SearchCriteria.PCLExamTypeLocationsDeptLocationID = Globals.DeptLocation.DeptLocationID;
//                        SearchCriteria.V_PCLMainCategory = Globals.PCLDepartment.ObjV_PCLMainCategory.LookupID;
//                        SearchCriteria.PCLExamTypeSubCategoryID = Globals.PCLDepartment.ObjPCLExamTypeSubCategoryID.PCLExamTypeSubCategoryID;
//                        SearchCriteria.PCLResultParamImpID = Globals.PCLDepartment.ObjPCLResultParamImpID.PCLResultParamImpID;
//                        SearchCriteria.FromDate = date;
//                        SearchCriteria.ToDate = date;

//                        ObjPatientPCLRequest_SearchPaging.PageIndex = 0;
//                        Coroutine.BeginExecute(DoPatientPCLRequest_SearchPaging(0, ObjPatientPCLRequest_SearchPaging.PageSize, true));
//                    }
//                    catch (FaultException<AxException> fault)
//                    {
//                        ClientLoggerHelper.LogInfo(fault.ToString());
//                    }
//                    catch (Exception ex)
//                    {
//                        ClientLoggerHelper.LogInfo(ex.ToString());
//                    }

//                }), null);
//            }
//        }
//        catch (Exception ex)
//        {
//            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//        }
//    });
//    t.Start();
//}