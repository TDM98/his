/*
 * #001: 20161215 CMN Begin: Add control for choose doctor and date
 * #002: 20180615 TBLD: Kiem tra ma may
 * #003: 20180827 TTM: 
 * #004: 20181001 TBL: BM 0000106. Fix PatientInfo không hiển thị thông tin
*/
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using System;
using eHCMSLanguage;
using System.Threading;
using aEMR.ServiceClient;
using PCLsProxy;
using System.Collections.Generic;
using aEMR.CommonTasks;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common.BaseModel;

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof (ISieuAmTimQuaThucQuanHome)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTimQuaThucQuanHomeViewModel : ViewModelBase, ISieuAmTimQuaThucQuanHome
        ,IHandle<DbClickSelectedObjectEvent<PatientPCLRequest>>
        , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTimQuaThucQuanHomeViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            
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
            var uc3 = Globals.GetViewModel<ISATQuaThucQuanChanDoan>();
            UCChanDoan = uc3;
            var uc4 = Globals.GetViewModel<ISATQuaThucQuanBangKiemTra>();
            UCBangKiemTra = uc4;
            var uc5 = Globals.GetViewModel<ISATQuaThucQuanQuyTrinh>();
            UCQuyTrinh = uc5;
            var uc6 = Globals.GetViewModel<ISATQuaThucQuanCD>();
            UCChanDoanTQ = uc6;
            //==== 20161129 CMN Begin: Add Imagecapture
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
        /*▲====: #004*/
        //▼====== #003: Dùng OnActive để xe rác biết khi nào cần hốt khi nào không => tránh tình trạng xe rác không hốt => tạo object mới khi gọi lại viewmodel này => bắn chụp sự kiện lung tung.
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
                //▲====== #003
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

        public object UCPatientPCLImageResults { get; set; }


        public object UCChanDoan { get; set; }
        public object UCBangKiemTra { get; set; }

        public object UCQuyTrinh { get; set; }
        public object UCChanDoanTQ { get; set; }

        public object UCLinkInputPCLImagingView { get; set; }
        //==== 20161129 CMN Begin: Add Imagecapture
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
        //////public void resetPatientInfo()
        //////{
        //////    Globals.PCLDepartment.SetInfo(new Patient(), new PatientRegistration(), new PatientRegistrationDetail());

        //////    //Show Info
        //////    Globals.EventAggregator.Publish(
        //////        new ShowPatientInfo<Patient, PatientRegistration, PatientRegistrationDetail>
        //////            {
        //////                Pt = new Patient(),
        //////                PtReg = new PatientRegistration(),
        //////                PtRegDetail = new PatientRegistrationDetail()
        //////            });
        //////}


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
        public void Handle(ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest> message)
        {
            //==== 20161117 CMN Begin: Clear All Captured image after reload
            (UCPatientPCLImageCapture as IImageCapture_V4).ClearAllCapturedImage();
            //==== 20161117 CMN End.
            PatientPCLReqID = message.PCLRequest.PatientPCLReqID;
            LoadInfo();
            Globals.PatientPCLRequest_Result = message.PCLRequest;
        }
        //▼====== #003: Tạo mới hàm này để gọi thay vì phải phải tự bắn sự kiện chính mình
        public void ReLoadInfoPatientPCLRequest(PatientPCLRequest patientPCLRequest)
        {
            (UCPatientPCLImageCapture as IImageCapture_V4).ClearAllCapturedImage();
            PatientPCLReqID = patientPCLRequest.PatientPCLReqID;
            LoadInfo();
            ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).GetImageInAnotherViewModel(Globals.PatientPCLRequest_Result);
        }
        //▲====== #003
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
        private URP_FE_OesophagienneCheck _ObjURP_FE_OesophagienneCheck;
        public URP_FE_OesophagienneCheck ObjURP_FE_OesophagienneCheck
        {
            get { return _ObjURP_FE_OesophagienneCheck; }
            set
            {
                if (_ObjURP_FE_OesophagienneCheck == value)
                    return;
                _ObjURP_FE_OesophagienneCheck = value;
                if (UCBangKiemTra != null)
                {
                    ((ISATQuaThucQuanBangKiemTra)UCBangKiemTra).SetResultDetails(ObjURP_FE_OesophagienneCheck);
                }
                NotifyOfPropertyChange(() => ObjURP_FE_OesophagienneCheck);
            }
        }
        private URP_FE_OesophagienneDiagnosis _ObjURP_FE_OesophagienneDiagnosis;
        public URP_FE_OesophagienneDiagnosis ObjURP_FE_OesophagienneDiagnosis
        {
            get { return _ObjURP_FE_OesophagienneDiagnosis; }
            set
            {
                if (_ObjURP_FE_OesophagienneDiagnosis == value)
                    return;
                _ObjURP_FE_OesophagienneDiagnosis = value;
                if (UCChanDoanTQ != null)
                {
                    ((ISATQuaThucQuanCD)UCChanDoanTQ).SetResultDetails(ObjURP_FE_OesophagienneDiagnosis);
                }
                NotifyOfPropertyChange(() => ObjURP_FE_OesophagienneDiagnosis);
            }
        }
        private URP_FE_Oesophagienne _ObjURP_FE_Oesophagienne;
        public URP_FE_Oesophagienne ObjURP_FE_Oesophagienne
        {
            get { return _ObjURP_FE_Oesophagienne; }
            set
            {
                if (_ObjURP_FE_Oesophagienne == value)
                    return;
                _ObjURP_FE_Oesophagienne = value;
                if (UCChanDoan != null)
                {
                    ((ISATQuaThucQuanChanDoan)UCChanDoan).SetResultDetails(ObjURP_FE_Oesophagienne);
                }
                NotifyOfPropertyChange(() => ObjURP_FE_Oesophagienne);
            }
        }
        int ThreadCount = 0;
        int TotalThread = 3;
        private void LoadInfo()
        {
            ThreadCount = 0;
            this.ShowBusyIndicator();
            GetURP_FE_OesophagienneCheck();
            GetURP_FE_OesophagienneDiagnosis();
            GetURP_FE_Oesophagienne();
        }
        private void GetURP_FE_OesophagienneCheck()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_OesophagienneCheck(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetURP_FE_OesophagienneCheck(asyncResult);
                            if (item != null)
                                ObjURP_FE_OesophagienneCheck = item;
                            else
                                ObjURP_FE_OesophagienneCheck = new URP_FE_OesophagienneCheck();

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
        private void GetURP_FE_OesophagienneDiagnosis()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_OesophagienneDiagnosis(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetURP_FE_OesophagienneDiagnosis(asyncResult);
                            if (item != null)
                                ObjURP_FE_OesophagienneDiagnosis = item;
                            else
                                ObjURP_FE_OesophagienneDiagnosis = new URP_FE_OesophagienneDiagnosis();
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
        private void GetURP_FE_Oesophagienne()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_Oesophagienne(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetURP_FE_Oesophagienne(asyncResult);
                            if (item != null)
                                ObjURP_FE_Oesophagienne = item;
                            else
                                ObjURP_FE_Oesophagienne = new URP_FE_Oesophagienne();
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
            if (ObjPatientPCLImagingResult.HIRepResourceCode == null || ObjPatientPCLImagingResult.HIRepResourceCode == string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri))
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
                List<PCLResultFileStorageDetail> mPCLResultFileStorageDetail = ((IImageCapture_V4)UCPatientPCLImageCapture).GetFileForStore(strPatientCode, true, false);

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
            URP_FE_OesophagienneUltra entity = new URP_FE_OesophagienneUltra();
            try
            {
                entity.PCLRequestID = PatientPCLReqID;
                //==== #001
                //ObjURP_FE_Oesophagienne.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                ObjURP_FE_Oesophagienne.DoctorStaffID = ObjURP_FE_Oesophagienne.DoctorStaffID > 0 ? ObjURP_FE_Oesophagienne.DoctorStaffID : Globals.LoggedUserAccount.Staff.StaffID;
                ObjURP_FE_Oesophagienne.CreateDate = ObjURP_FE_Oesophagienne.CreateDate == DateTime.MinValue ? DateTime.Now : ObjURP_FE_Oesophagienne.CreateDate;
                //==== #001
                entity.ObjURP_FE_Oesophagienne = ObjURP_FE_Oesophagienne;
                entity.ObjURP_FE_OesophagienneCheck = ObjURP_FE_OesophagienneCheck;
                entity.ObjURP_FE_OesophagienneDiagnosis = ObjURP_FE_OesophagienneDiagnosis;
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
                    contract.BeginAddAndUpdateURP_FE_Oesophagienne(entity, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool res = contract.EndAddAndUpdateURP_FE_Oesophagienne(asyncResult);
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