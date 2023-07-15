using System;
using System.Windows;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using DataEntities;
using Caliburn.Micro;
using aEMR.Infrastructure.Events;
using System.Collections.Generic;
using System.Threading;
using aEMR.ServiceClient;
using PCLsProxy;
using eHCMSLanguage;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using aEMR.CommonTasks;
using aEMR.Common.BaseModel;

/*
 * 20180615 #001 TBLD: Kiem tra ma may
 * 20180827 #002 TTM:
 */

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(IAbdominalUltrasoundMain)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AbdominalUltrasoundMainViewModel : ViewModelBase, IAbdominalUltrasoundMain
        , IHandle<Event_SearchAbUltraRequestCompleted>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AbdominalUltrasoundMainViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            //▼====== #002: Đã đem Subscribe xuống OnActive nên Comment lại.
            //eventAggregator.Subscribe(this);
            //▲====== #002
            ObjPatientPCLImagingResult = new PatientPCLImagingResult();
            ObjPatientPCLImagingResult.StaffID = Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : (Globals.LoggedUserAccount.StaffID.HasValue ? Globals.LoggedUserAccount.StaffID.Value : -1);
            ObjPatientPCLImagingResult.PatientPCLReqID = -1;
            ObjPatientPCLImagingResult.PCLExamTypeID = -1;

            ObjPatientPCLImagingResult.PCLExamDate = Globals.ServerDate.GetValueOrDefault(DateTime.Now);
            ObjPatientPCLImagingResult.PCLExamForOutPatient = true;
            ObjPatientPCLImagingResult.IsExternalExam = false;

            UCDoctorProfileInfo = Globals.GetViewModel<ILoginInfo>();

            UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();

            UCAbdominalUltrasoundResult = Globals.GetViewModel<IAbdominalUltrasoundResult>();


            UCSearchPCLRequest = Globals.GetViewModel<IPCLDepartmentSearchPCLRequest>();
            UCSearchPCLRequest.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Imaging;

            UCPatientPCLImageResults = Globals.GetViewModel<IPatientPCLDeptImagingResult>();
            UCPatientPCLImageResults.ObjPatientPCLImagingResult = ObjPatientPCLImagingResult;

        }

        public void SetVideoImageCaptureSourceVM(object theVideoVM)
        {
            UCPatientPCLImageCapture = (IImageCapture_V4)theVideoVM;
        }

        //▼====== #002: Dùng OnActive để xe rác biết khi nào cần hốt khi nào không => tránh tình trạng xe rác không hốt => tạo object mới khi gọi lại viewmodel này => bắn chụp sự kiện lung tung.
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            this.ActivateItem(UCPatientPCLImageResults);
            if (Globals.PatientPCLRequest_Result != null)
            {
                Coroutine.BeginExecute(GetPatient());
            }
            if (UCPatientPCLImageCapture != null)
            {
                UCPatientPCLImageCapture.ClearAllCapturedImage();
            }
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        private IEnumerator<IResult> GetPatient()
        {
            var loadPatients = new LoadPatientTask(Globals.PatientPCLRequest_Result.PatientID);
            yield return loadPatients;
            CurPatient = loadPatients.CurrentPatient;
            if (CurPatient != null)
            {
                Globals.EventAggregator.Publish(new ItemPatient1<Patient>() { Item = CurPatient });
            }
            ReLoadInfoPatientPCLRequest(CurPatient, Globals.PatientPCLRequest_Result);
        }
        //▲====== #002
        private ILoginInfo ucDoctorProfileInfo;

        public ILoginInfo UCDoctorProfileInfo
        {
            get { return ucDoctorProfileInfo; }
            set { ucDoctorProfileInfo = value; }
        }

        private IPatientInfo ucPatientProfileInfo;

        public IPatientInfo UCPatientProfileInfo
        {
            get { return ucPatientProfileInfo; }
            set { ucPatientProfileInfo = value; }
        }

        private IAbdominalUltrasoundResult ucAbdominalUltrasoundResult;

        public IAbdominalUltrasoundResult UCAbdominalUltrasoundResult
        {
            get { return ucAbdominalUltrasoundResult; }
            set { ucAbdominalUltrasoundResult = value; }
        }

        private IPCLDepartmentSearchPCLRequest ucSearchPCLRequest;

        public IPCLDepartmentSearchPCLRequest UCSearchPCLRequest
        {
            get { return ucSearchPCLRequest; }
            set { ucSearchPCLRequest = value; }
        }

        private IPatientPCLDeptImagingResult ucPatientPCLImageResults;

        public IPatientPCLDeptImagingResult UCPatientPCLImageResults
        {
            get { return ucPatientPCLImageResults; }
            set { ucPatientPCLImageResults = value; }
        }

        private IImageCapture_V4 ucPatientPCLImageCapture;

        public IImageCapture_V4 UCPatientPCLImageCapture
        {
            get { return ucPatientPCLImageCapture; }
            set { ucPatientPCLImageCapture = value; }
        }

        private PatientPCLImagingResult _ObjPatientPCLImagingResult;

        public PatientPCLImagingResult ObjPatientPCLImagingResult
        {
            get { return _ObjPatientPCLImagingResult; }
            set
            {
                if (_ObjPatientPCLImagingResult != value)
                {
                    _ObjPatientPCLImagingResult = value;
                    NotifyOfPropertyChange(() => ObjPatientPCLImagingResult);
                }
            }
        }

        private AbdominalUltrasound currentAbUltra;
        public AbdominalUltrasound CurrentAbUltra
        {
            get { return currentAbUltra; }
            set
            {
                if (currentAbUltra == value)
                {
                    return;
                }
                currentAbUltra = value;

                if (UCAbdominalUltrasoundResult != null)
                {
                    UCAbdominalUltrasoundResult.SetResultDetails(CurPatient, currentAbUltra);
                }

                NotifyOfPropertyChange(() => currentAbUltra);
            }
        }

        private Patient _curPatient;

        public Patient CurPatient
        {
            get
            {
                return _curPatient; 
            }
            set 
            {
                _curPatient = value;
                NotifyOfPropertyChange(() => CurPatient);

            }
        }


        private long _PatientPCLReqID;
        public long PatientPCLReqID
        {
            get { return _PatientPCLReqID; }
            set
            {
                if (_PatientPCLReqID != value)
                {
                    _PatientPCLReqID = value;
                    NotifyOfPropertyChange(() => PatientPCLReqID);
                }
            }
        }

        public void Handle(Event_SearchAbUltraRequestCompleted message)
        {
            if (message != null)
            {
                //==== 20161117 CMN Begin: Clear All Captured image after reload
                (UCPatientPCLImageCapture as IImageCapture_V4).ClearAllCapturedImage();
                //==== 20161117 CMN End

                CurPatient = message.Patient;
                UCPatientProfileInfo.CurrentPatient = message.Patient;
                PatientPCLReqID = message.PCLRequest.PatientPCLReqID;
                LoadInfo();
            }
        }
        //▼====== #002: Tạo hàm để load lại thông tin, vì ban đầu thông tin được load khi SearchPCL bắn sự kiện xuống để ViewModel này chụp lại.
        //              Khi chuyển từ viewmodel khác cùng module thì sự kiện từ SearchPCL không được bắn => không load thông tin đc
        public void ReLoadInfoPatientPCLRequest(Patient curPatient, PatientPCLRequest patientPCLrequest)
        {
            if (patientPCLrequest != null && curPatient != null)
            {
                (UCPatientPCLImageCapture as IImageCapture_V4).ClearAllCapturedImage();
                CurPatient = curPatient;
                PatientPCLReqID = patientPCLrequest.PatientPCLReqID;
                ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).GetImageInAnotherViewModel(Globals.PatientPCLRequest_Result);
                Globals.PatientPCLRequest_Result = patientPCLrequest;
                LoadInfo();
            }
        }
        //▲====== #002
        private void LoadInfo()
        {
            CurrentAbUltra = new AbdominalUltrasound();

            GetAbdominalUltrasoundResult(PatientPCLReqID);
        }

        private void GetAbdominalUltrasoundResult(long PatientPCLReqID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAbdominalUltrasoundResult(PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetAbdominalUltrasoundResult(asyncResult);
                            if (item != null)
                            {
                                CurrentAbUltra = item;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
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
        /*▼====: #001*/
        private bool CheckValid()
        {
            if (ObjPatientPCLImagingResult.HIRepResourceCode == null || ObjPatientPCLImagingResult.HIRepResourceCode == string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri))
            {
                MessageBox.Show(eHCMSResources.Z2242_G1_ChuaChonMaMay);
                return false;
            }
            return true;
        }
        /*▲====: #001*/
        public void btnSaveCmd()
        {
            if (PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0301_G1_Msg_InfoChonBN);
                return;
            }
            /*▼====: #001*/
            if (Globals.ServerConfigSection.CommonItems.OrganizationUseSoftware == 0 && !CheckValid())
            {
                return;
            }
            /*▲====: #001*/
            CurrentAbUltra.DoctorStaffID = Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : (Globals.LoggedUserAccount.StaffID.HasValue ? Globals.LoggedUserAccount.StaffID.Value : -1);

            if (CurrentAbUltra.AbdominalUltrasoundID > 0)
            {
                UpdateAbdominalUltrasoundResult(CurrentAbUltra);
            }
            else
            {
                CurrentAbUltra.PatientPCLReqID = PatientPCLReqID;
                InsertAbdominalUltrasoundResult(CurrentAbUltra);
            }
        }


        private void InsertAbdominalUltrasoundResult(AbdominalUltrasound entity)
        {
            this.ShowBusyIndicator();
            //==== 20161013 CMN Begin: Add PCL Image Method
            List<PCLResultFileStorageDetail> FileForDelete = new List<PCLResultFileStorageDetail>();
            List<PCLResultFileStorageDetail> FileForStore = new List<PCLResultFileStorageDetail>();
            try
            {
                string strPatientCode = (UCPatientProfileInfo.CurrentPatient != null ? UCPatientProfileInfo.CurrentPatient.PatientCode : "AUnkCode");
                List<PCLResultFileStorageDetail> mPCLResultFileStorageDetail = UCPatientPCLImageCapture.GetFileForStore(strPatientCode, true,false);
                FileForDelete = ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).FileForDelete;
                FileForStore = mPCLResultFileStorageDetail;
                if ((((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).TotalFile + FileForStore.Count - FileForDelete.Count) > Globals.ServerConfigSection.Pcls.MaxEchogramImageFile)
                {
                    MessageBox.Show(eHCMSResources.K0457_G1_VuotQuaSLgFileToiDaChoPhep);
                    this.HideBusyIndicator();
                    return;
                }
                FileForStore.AddRange(((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).FileForStore);
            }
            catch (Exception ex)
            {
                MessageBox.Show("UpdateUltraResParams_EchoCardiography Get File: " + ex.Message);
            }
            //==== 20161013 CMN End.
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    try
                    {
                        IPCLs contract = serviceFactory.ServiceInstance;
                        contract.BeginInsertAbdominalUltrasoundResult(entity, ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult, FileForStore, FileForDelete,
                                Globals.DispatchCallback(
                                    (asyncResult) =>
                                    {
                                        try
                                        {
                                            bool res = contract.EndInsertAbdominalUltrasoundResult(asyncResult);
                                            if (res)
                                            {
                                                MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
                                                LoadInfo();
                                                //==== 20161013 CMN Begin: Add PCL Image Method
                                                IPatientPCLDeptImagingResult vImagingResult = UCPatientPCLImageResults as IPatientPCLDeptImagingResult;
                                                vImagingResult.LoadData(((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.PatientPCLRequest);
                                                
                                                UCPatientPCLImageCapture.ClearSelectedImage();
                                                //==== 20161013 CMN End.
                                            }
                                            else
                                            {
                                                MessageBox.Show(eHCMSResources.Z0520_G1_ThemMoiThatBai);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.Message);
                                        }
                                        finally
                                        {
                                            this.HideBusyIndicator();
                                        }
                                    }), null);
                    }
                    catch (Exception ex)
                    {
                        this.HideBusyIndicator();
                        MessageBox.Show("UpdateUltraResParams_EchoCardiography BeginCall: " + ex.Message);
                    }
                }
            });

            t.Start();
        }


        private void UpdateAbdominalUltrasoundResult(AbdominalUltrasound entity)
        {
            this.ShowBusyIndicator();
            //==== 20161013 CMN Begin: Add PCL Image Method
            List<PCLResultFileStorageDetail> FileForDelete = new List<PCLResultFileStorageDetail>();
            List<PCLResultFileStorageDetail> FileForStore = new List<PCLResultFileStorageDetail>();
            try
            {
                string strPatientCode = (UCPatientProfileInfo.CurrentPatient != null ? UCPatientProfileInfo.CurrentPatient.PatientCode : "AUnkCode");
                List<PCLResultFileStorageDetail> mPCLResultFileStorageDetail = UCPatientPCLImageCapture.GetFileForStore(strPatientCode, true,false);
                FileForDelete = ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).FileForDelete;
                FileForStore = mPCLResultFileStorageDetail;
                if ((((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).TotalFile + FileForStore.Count - FileForDelete.Count) > Globals.ServerConfigSection.Pcls.MaxEchogramImageFile)
                {
                    MessageBox.Show(eHCMSResources.K0457_G1_VuotQuaSLgFileToiDaChoPhep);
                    this.HideBusyIndicator();
                    return;
                }
                FileForStore.AddRange(((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).FileForStore);
            }
            catch (Exception ex)
            {
                MessageBox.Show("UpdateUltraResParams_EchoCardiography Get File: " + ex.Message);
            }
            //==== 20161013 CMN End.
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    try
                    {
                        IPCLs contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateAbdominalUltrasoundResult(entity, ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult, FileForStore, FileForDelete,
                                Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        bool res = contract.EndUpdateAbdominalUltrasoundResult(asyncResult);
                                        if (res)
                                        {
                                            MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                            LoadInfo();
                                            //==== 20161013 CMN Begin: Add PCL Image Method
                                            IPatientPCLDeptImagingResult vImagingResult = UCPatientPCLImageResults as IPatientPCLDeptImagingResult;
                                            vImagingResult.LoadData(((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.PatientPCLRequest);
                                            
                                            UCPatientPCLImageCapture.ClearSelectedImage();
                                            //==== 20161013 CMN End.
                                        }
                                        else
                                        {
                                            MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("UpdateUltraResParams_EchoCardiography EndCall: " + ex.Message);
                                    }
                                    finally
                                    {
                                        this.HideBusyIndicator();
                                    }
                                }), null);

                    }
                    catch (Exception ex)
                    {
                        this.HideBusyIndicator();
                        MessageBox.Show("UpdateUltraResParams_EchoCardiography BeginCall: " + ex.Message);
                    }
                }
            });

            t.Start();
        }

        public void btnPrintCmd()
        {
            if (PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0468_G1_Chon1YCCDoanHA);
                return;
            }

            if (CurrentAbUltra.AbdominalUltrasoundID <= 0)
            {
                MessageBox.Show("Kết quả Chẩn đoán hình ảnh chưa được lưu!");
                return;
            }

            //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
            //proAlloc.PatientPCLReqID = (int)PatientPCLReqID;
            //proAlloc.eItem = ReportName.ABDOMINAL_ULTRASOUND_RESULT;

            //IPatientPCLDeptImagingResult imVM = UCPatientPCLImageResults as IPatientPCLDeptImagingResult;
            //if (imVM != null && imVM.GetNumOfImageResultFiles() > 0)
            //{
            //    proAlloc.EchoCardioType1ImageResultFile1 = imVM.GetImageResultFileStoragePath(0);
            //}
            //if (imVM != null && imVM.GetNumOfImageResultFiles() > 1)
            //{
            //    proAlloc.EchoCardioType1ImageResultFile2 = imVM.GetImageResultFileStoragePath(1);
            //}

            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
            {
                proAlloc.PatientPCLReqID = (int)PatientPCLReqID;
                proAlloc.eItem = ReportName.ABDOMINAL_ULTRASOUND_RESULT;

                IPatientPCLDeptImagingResult imVM = UCPatientPCLImageResults as IPatientPCLDeptImagingResult;
                if (imVM != null && imVM.GetNumOfImageResultFiles() > 0)
                {
                    proAlloc.EchoCardioType1ImageResultFile1 = imVM.GetImageResultFileStoragePath(0);
                }
                if (imVM != null && imVM.GetNumOfImageResultFiles() > 1)
                {
                    proAlloc.EchoCardioType1ImageResultFile2 = imVM.GetImageResultFileStoragePath(1);
                }
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, null);
        }




    }
}
