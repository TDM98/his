using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;
using System.Threading;
using aEMR.ServiceClient;
using PCLsProxy;
using System;
using System.Collections.Generic;
using aEMR.CommonTasks;
using Castle.Windsor;
using aEMR.Common.BaseModel;
/*
* #001: 20161215 CMN Begin: Add control for choose doctor and date
* #002: 20180615 TBLD: Kiem tra ma may
* #003: 20180827 TTM:
* #004: 20181001 TBL: BM 0000106. Fix PatientInfo không hiển thị thông tin 
*/
namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof (ISieuAmMachMauHome)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmMachMauHomeViewModel : ViewModelBase, ISieuAmMachMauHome
        , IHandle<DbClickSelectedObjectEvent<PatientPCLRequest>>
        , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
    {
        [ImportingConstructor]
        public SieuAmMachMauHomeViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            CreateSubVM();
        }

        public void SetVideoImageCaptureSourceVM(object theVideoVM)
        {
            UCPatientPCLImageCapture = theVideoVM;
        }

        /*▼====: #004*/
        public void CreateSubVM()
        {
            vmSearchPCLRequest = Globals.GetViewModel<IPCLDepartmentSearchPCLRequest>();
            vmSearchPCLRequest.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Imaging;
            var uc1 = Globals.GetViewModel<ILoginInfo>();
            UCDoctorProfileInfo = uc1;
            var uc2 = Globals.GetViewModel<IPatientInfo>();
            UCPatientProfileInfo = uc2;
            var uc3 = Globals.GetViewModel<ISAMMDongMachCanh>();
            UCSAMMDongMachCanh = uc3;
            var uc4 = Globals.GetViewModel<ISAMMDongMachChu>();
            UCSAMMDongMachChu = uc4;
            var uc5 = Globals.GetViewModel<ISAMMChiDuoi>();
            UCSAMMChiDuoi = uc5;
            var uc6 = Globals.GetViewModel<ISAMMKhac>();
            UCKhac = uc6;
            ObjPatientPCLImagingResult = new PatientPCLImagingResult
            {
                StaffID = Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : (Globals.LoggedUserAccount.StaffID.HasValue ? Globals.LoggedUserAccount.StaffID.Value : -1),
                PatientPCLReqID = -1,
                PCLExamTypeID = -1,
                PCLExamDate = Globals.ServerDate.GetValueOrDefault(DateTime.Now),
                PCLExamForOutPatient = true,
                IsExternalExam = false
            };
            var uc7 = Globals.GetViewModel<IPatientPCLDeptImagingResult>();
            uc7.ObjPatientPCLImagingResult = ObjPatientPCLImagingResult;
            UCPatientPCLImageResults = uc7;

        }
        public void ActivateSubVM()
        {
            ActivateItem(UCPatientProfileInfo);
            ActivateItem(UCPatientPCLImageResults);
        }

        public void DeActivateSubVM(bool close)
        {
            DeactivateItem(UCPatientProfileInfo, close);
            DeactivateItem(UCPatientPCLImageResults, close);
        }
        /*▼====: #004*/
        //▼====== #003: 
        protected override void OnActivate()
        {
            /*▼====: #004*/
            ActivateSubVM();
            /*▲====: #004*/
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            //this.ActivateItem(UCPatientPCLImageResults);
            if (Globals.PatientPCLRequest_Result != null)
            {
                Coroutine.BeginExecute(GetPatient());
                PatientPCLRequest pcl = Globals.PatientPCLRequest_Result;
                //▼====== #003 Sự kiện ReaderInfoPatientFromPatientPCLReqEvent ở đây đang thực hiện việc tự mình bắn mình, không cần thiết
                //Anh Tuấn bảo chỉ cần gọi không cần phải bắn. Nếu tự mình bắn mình thì tương tự như tự giết mình.
                //Globals.EventAggregator.Publish(new ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>() { PCLRequest = pcl });
                //
                ReLoadInfoPatientPCLRequest(pcl);
            }
            if (UCPatientPCLImageCapture != null)
            {
                ((IImageCapture_V4)UCPatientPCLImageCapture).ClearAllCapturedImage();
            }
        }
        protected override void OnDeactivate(bool close)
        {
            /*▼====: #004*/
            DeActivateSubVM(close);
            /*▲====: #004*/
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }
        
        public void ReLoadInfoPatientPCLRequest(PatientPCLRequest patientPCLrequest)
        {
            if (patientPCLrequest != null)
            {
                (UCPatientPCLImageCapture as IImageCapture_V4).ClearAllCapturedImage();
                PatientPCLReqID = patientPCLrequest.PatientPCLReqID;
                LoadInfo();
                ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).GetImageInAnotherViewModel(Globals.PatientPCLRequest_Result);
            }
        }
        //▲====== #003
        private IEnumerator<IResult> GetPatient()
        {
            var loadPatients = new LoadPatientTask(Globals.PatientPCLRequest_Result.PatientID);
            yield return loadPatients;
            Patient CurrentPatient = loadPatients.CurrentPatient;
            if (CurrentPatient != null)
            {
                Globals.EventAggregator.Publish(new ItemPatient1<Patient>() { Item = CurrentPatient });
            }
        }

        private IPCLDepartmentSearchPCLRequest _vmSearchPCLRequest;
        public IPCLDepartmentSearchPCLRequest vmSearchPCLRequest
        {
            get
            {
                return _vmSearchPCLRequest;
            }
            set
            {
                _vmSearchPCLRequest = value;
                NotifyOfPropertyChange(() => vmSearchPCLRequest);
            }
        }

        public object UCDoctorProfileInfo { get; set; }
        public object UCPatientProfileInfo { get; set; }

        public object UCPCLDepartmentSearchPCLRequest { get; set; }


        public object UCSieuAmTimTM { get; set; }
        public object UCSAMMDongMachCanh { get; set; }

        public object UCSAMMDongMachChu { get; set; }
        public object UCSAMMChiDuoi { get; set; }
        public object UCKhac { get; set; }
        //==== 20161129 CMN Begin: Add Imagecapture
        public object UCPatientPCLImageResults { get; set; }
        public object UCPatientPCLImageCapture { get; set; }
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
        //==== 20161129 CMN End: Add Imagecapture
        public TabItem TabFirst { get; set; }
        public void TabFirst_Loaded(object sender, RoutedEventArgs e)
        {
            TabFirst = sender as TabItem;
        }

        public void Handle(DbClickSelectedObjectEvent<PatientPCLRequest> message)
        {
            if (message != null)
            {
                TabFirst.IsSelected = true;
            }
        }
        //==== 20161129 CMN Begin: Add button save for all pages
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
        private URP_FE_VasculaireAnother _curURP_FE_VasculaireAnother;
        public URP_FE_VasculaireAnother curURP_FE_VasculaireAnother
        {
            get { return _curURP_FE_VasculaireAnother; }
            set
            {
                if (_curURP_FE_VasculaireAnother == value)
                    return;
                _curURP_FE_VasculaireAnother = value;
                if (UCKhac != null)
                {
                    ((ISAMMKhac)UCKhac).SetResultDetails(curURP_FE_VasculaireAnother);
                }
                NotifyOfPropertyChange(() => curURP_FE_VasculaireAnother);
            }
        }
        private URP_FE_StressDobutamineImages _curURP_FE_StressDobutamineImages;
        public URP_FE_StressDobutamineImages curURP_FE_StressDobutamineImages
        {
            get { return _curURP_FE_StressDobutamineImages; }
            set
            {
                if (_curURP_FE_StressDobutamineImages == value)
                    return;
                _curURP_FE_StressDobutamineImages = value;
                if (UCSAMMChiDuoi != null)
                {
                    ((ISAMMChiDuoi)UCSAMMChiDuoi).SetResultDetails(curURP_FE_StressDobutamineImages);
                }
                NotifyOfPropertyChange(() => curURP_FE_StressDobutamineImages);
            }
        }
        private URP_FE_VasculaireAorta _curURP_FE_VasculaireAorta;
        public URP_FE_VasculaireAorta curURP_FE_VasculaireAorta
        {
            get { return _curURP_FE_VasculaireAorta; }
            set
            {
                if (_curURP_FE_VasculaireAorta == value)
                    return;
                _curURP_FE_VasculaireAorta = value;
                if (UCSAMMDongMachChu != null)
                {
                    ((ISAMMDongMachChu)UCSAMMDongMachChu).SetResultDetails(curURP_FE_VasculaireAorta);
                }
                NotifyOfPropertyChange(() => curURP_FE_VasculaireAorta);
            }
        }
        private URP_FE_VasculaireCarotid _curURP_FE_VasculaireCarotid;
        public URP_FE_VasculaireCarotid curURP_FE_VasculaireCarotid
        {
            get { return _curURP_FE_VasculaireCarotid; }
            set
            {
                if (_curURP_FE_VasculaireCarotid == value)
                    return;
                _curURP_FE_VasculaireCarotid = value;
                if (UCSAMMDongMachCanh != null)
                {
                    ((ISAMMDongMachCanh)UCSAMMDongMachCanh).SetResultDetails(curURP_FE_VasculaireCarotid, curURP_FE_VasculaireExam);
                }
                NotifyOfPropertyChange(() => curURP_FE_VasculaireCarotid);
            }
        }
        private URP_FE_VasculaireExam _curURP_FE_VasculaireExam;
        public URP_FE_VasculaireExam curURP_FE_VasculaireExam
        {
            get { return _curURP_FE_VasculaireExam; }
            set
            {
                if (_curURP_FE_VasculaireExam == value)
                    return;
                _curURP_FE_VasculaireExam = value;
                if (UCSAMMDongMachCanh != null)
                {
                    ((ISAMMDongMachCanh)UCSAMMDongMachCanh).SetResultDetails(curURP_FE_VasculaireCarotid, curURP_FE_VasculaireExam);
                }
                NotifyOfPropertyChange(() => curURP_FE_VasculaireExam);
            }
        }
        public void Handle(ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest> message)
        {
            if (message != null)
            {
                //==== 20161117 CMN Begin: Clear All Captured image after reload
                (UCPatientPCLImageCapture as IImageCapture_V4).ClearAllCapturedImage();
                //==== 20161117 CMN End.
                PatientPCLReqID = message.PCLRequest.PatientPCLReqID;
                LoadInfo();
                Globals.PatientPCLRequest_Result = message.PCLRequest;
            }
        }
        int ThreadCount = 0;
        int TotalThread = 5;
        private void LoadInfo()
        {
            ThreadCount = 0;
            this.ShowBusyIndicator();
            LoadVasculaireAnother();
            LoadStressDobutamineImages();
            LoadVasculaireAorta();
            LoadVasculaireCarotid();
            LoadVasculaireExam();
        }
        private void LoadVasculaireAnother()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_VasculaireAnother(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetURP_FE_VasculaireAnother(asyncResult);
                            if (item != null)
                                curURP_FE_VasculaireAnother = item;
                            else
                                curURP_FE_VasculaireAnother = new URP_FE_VasculaireAnother();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            if (++ThreadCount == TotalThread)
                                this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private void LoadStressDobutamineImages()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_StressDobutamineImages(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetURP_FE_StressDobutamineImages(asyncResult);
                            if (item != null)
                                curURP_FE_StressDobutamineImages = item;
                            else
                                curURP_FE_StressDobutamineImages = new URP_FE_StressDobutamineImages();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            if (++ThreadCount == TotalThread)
                                this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private void LoadVasculaireAorta()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_VasculaireAorta(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var item = contract.EndGetURP_FE_VasculaireAorta(asyncResult);
                            if (item != null)
                                curURP_FE_VasculaireAorta = item;
                            else
                                curURP_FE_VasculaireAorta = new URP_FE_VasculaireAorta();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            if (++ThreadCount == TotalThread)
                                this.HideBusyIndicator();
                        }
                    }), null);
                }
            });

            t.Start();
        }
        private void LoadVasculaireCarotid()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_VasculaireCarotid(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetURP_FE_VasculaireCarotid(asyncResult);
                            if (item != null)
                                curURP_FE_VasculaireCarotid = item;
                            else
                                curURP_FE_VasculaireCarotid = new URP_FE_VasculaireCarotid();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            if (++ThreadCount == TotalThread)
                                this.HideBusyIndicator();
                        }
                    }), null);
                }
            });

            t.Start();
        }
        private void LoadVasculaireExam()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_VasculaireExam(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetURP_FE_VasculaireExam(asyncResult);
                            if (item != null)
                                curURP_FE_VasculaireExam = item;
                            else
                                curURP_FE_VasculaireExam = new URP_FE_VasculaireExam();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            if (++ThreadCount == TotalThread)
                                this.HideBusyIndicator();
                        }
                    }), null);
                }
            });

            t.Start();
        }
        /*▼====: #002*/
        private bool CheckValid()
        {
            if (ObjPatientPCLImagingResult.HIRepResourceCode == null)
            {
                MessageBox.Show(eHCMSResources.Z2242_G1_ChuaChonMaMay);
                return false;
            }
            return true;
        }
        /*▲====: #002*/
        public void btnSaveCmd()
        {
            /*▼====: #002*/
            if (!CheckValid() && Globals.ServerConfigSection.CommonItems.OrganizationUseSoftware == 0)
            {
                return;
            }
            /*▲====: #002*/
            this.ShowBusyIndicator();
            List<PCLResultFileStorageDetail> FileForDelete = new List<PCLResultFileStorageDetail>();
            List<PCLResultFileStorageDetail> FileForStore = new List<PCLResultFileStorageDetail>();
            try
            {
                string strPatientCode = (((IPatientInfo)UCPatientProfileInfo).CurrentPatient != null ? ((IPatientInfo)UCPatientProfileInfo).CurrentPatient.PatientCode : "AUnkCode");
                List<PCLResultFileStorageDetail> mPCLResultFileStorageDetail = ((IImageCapture_V4)UCPatientPCLImageCapture).GetFileForStore(strPatientCode, true,false);
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
                MessageBox.Show(ex.Message);
            }
            URP_FE_Vasculaire entity = new URP_FE_Vasculaire();
            try
            {
                entity.PCLRequestID = PatientPCLReqID;
                //==== #001
                //curURP_FE_VasculaireExam.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                curURP_FE_VasculaireAnother.DoctorStaffID = curURP_FE_VasculaireAnother.DoctorStaffID > 0 ? curURP_FE_VasculaireAnother.DoctorStaffID : Globals.LoggedUserAccount.Staff.StaffID;
                curURP_FE_VasculaireAnother.CreateDate = curURP_FE_VasculaireAnother.CreateDate == DateTime.MinValue ? DateTime.Now : curURP_FE_VasculaireAnother.CreateDate;
                //==== #001
                entity.curURP_FE_VasculaireCarotid = curURP_FE_VasculaireCarotid;
                entity.curURP_FE_VasculaireExam = curURP_FE_VasculaireExam;
                entity.curURP_FE_VasculaireAorta = curURP_FE_VasculaireAorta;
                entity.curURP_FE_StressDobutamineImages = curURP_FE_StressDobutamineImages;
                entity.curURP_FE_VasculaireAnother = curURP_FE_VasculaireAnother;
                entity.ObjPatientPCLImagingResult = ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult;
                entity.FileForStore = FileForStore;
                entity.FileForDelete = FileForDelete;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.HideBusyIndicator();
                return;
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginAddAndUpdateFE_Vasculaire(entity, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool res = contract.EndAddAndUpdateFE_Vasculaire(asyncResult);
                            if (res)
                            {
                                MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                LoadInfo();
                                IPatientPCLDeptImagingResult vImagingResult = UCPatientPCLImageResults as IPatientPCLDeptImagingResult;
                                vImagingResult.LoadData(((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.PatientPCLRequest);
                                IImageCapture_V4 vImageCapture = UCPatientPCLImageCapture as IImageCapture_V4;
                                vImageCapture.ClearSelectedImage();
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail);
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
        //==== 20161129 CMN End: Add button save for all pages
    }
}