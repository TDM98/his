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
using System.Collections.Generic;
using eHCMSLanguage;
using System.Threading;
using aEMR.ServiceClient;
using PCLsProxy;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Common.BaseModel;

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof (ISieuAmTimThaiHome)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTimThaiHomeViewModel : ViewModelBase, ISieuAmTimThaiHome
        , IHandle<DbClickSelectedObjectEvent<PatientPCLRequest>>
        , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTimThaiHomeViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            
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
            var uc3 = Globals.GetViewModel<ISieuAmTT_TM2D>();
            UCSieuAmTimTM = uc3;
            var uc5 = Globals.GetViewModel<ISieuAmTT_Doppler>();
            UCSieuAmTimDoppler = uc5;
            var uc7 = Globals.GetViewModel<ISieuAmTT_KetLuan>();
            UCSieuAmTimKetLuan = uc7;
            var uc8 = Globals.GetViewModel<ISieuAmTT_TimKiem>();
            UCSieuAmTimTimKiem = uc8;
            var uc9 = Globals.GetViewModel<ISieuAmTT_SauSinh>();
            ObjPatientPCLImagingResult = new PatientPCLImagingResult
            {
                StaffID = Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : (Globals.LoggedUserAccount.StaffID.HasValue ? Globals.LoggedUserAccount.StaffID.Value : -1),
                PatientPCLReqID = -1,
                PCLExamTypeID = -1,
                PCLExamDate = Globals.ServerDate.GetValueOrDefault(DateTime.Now),
                PCLExamForOutPatient = true,
                IsExternalExam = false
            };
            var uc10 = Globals.GetViewModel<IPatientPCLDeptImagingResult>();
            uc10.ObjPatientPCLImagingResult = ObjPatientPCLImagingResult;
            UCPatientPCLImageResults = uc10;

            var uc12 = Globals.GetViewModel<ISieuAmTT_General>();
            UCSieuAmTimGeneral = uc12;
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
        //▼====== #003 Đem if từ hàm khởi tạo xuống để thực hiện 1 lần khi active mà thôi
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
                //▼====== #003  Sự kiện ReaderInfoPatientFromPatientPCLReqEvent ở đây đang thực hiện việc tự mình bắn mình, không cần thiết
                //              Anh Tuấn bảo chỉ cần gọi không cần phải bắn. Nếu tự mình bắn mình thì tương tự như tự giết mình.
                //Globals.EventAggregator.Publish(new ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>() { PCLRequest = pcl });
                //▲====== #003
                ReLoadInfoPatientPCLRequest(pcl);
            }
            if (UCPatientPCLImageCapture != null)
            {
                ((IImageCapture_V4)UCPatientPCLImageCapture).ClearAllCapturedImage();
            }
        }
        //▲====== #003
        protected override void OnDeactivate(bool close)
        {
            /*▼====: #004*/
            DeActivateSubVM(close);
            /*▲====: #004*/
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }
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
        public object UCSieuAmTimDoppler { get; set; }
        public object UCSieuAmTimKetLuan { get; set; }
        public object UCSieuAmTimTimKiem { get; set; }
        public object UCSieuAmTimSauSinh { get; set; }
        public object UCSieuAmTimGeneral { get; set; }
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
        private UltraResParams_FetalEchocardiography _ObjUltraResParams_FetalEchocardiography;
        public UltraResParams_FetalEchocardiography ObjUltraResParams_FetalEchocardiography
        {
            get { return _ObjUltraResParams_FetalEchocardiography; }
            set
            {
                if (_ObjUltraResParams_FetalEchocardiography == value)
                    return;
                _ObjUltraResParams_FetalEchocardiography = value;
                if (UCSieuAmTimGeneral != null)
                {
                    ((ISieuAmTT_General)UCSieuAmTimGeneral).SetResultDetails(ObjUltraResParams_FetalEchocardiography);
                }
                NotifyOfPropertyChange(() => ObjUltraResParams_FetalEchocardiography);
            }
        }
        private UltraResParams_FetalEchocardiography2D _ObjUltraResParams_FetalEchocardiography2D;
        public UltraResParams_FetalEchocardiography2D ObjUltraResParams_FetalEchocardiography2D
        {
            get { return _ObjUltraResParams_FetalEchocardiography2D; }
            set
            {
                if (_ObjUltraResParams_FetalEchocardiography2D == value)
                    return;
                _ObjUltraResParams_FetalEchocardiography2D = value;
                if (UCSieuAmTimTM != null)
                {
                    ((ISieuAmTT_TM2D)UCSieuAmTimTM).SetResultDetails(ObjUltraResParams_FetalEchocardiography2D);
                }
                NotifyOfPropertyChange(() => ObjUltraResParams_FetalEchocardiography2D);
            }
        }
        private UltraResParams_FetalEchocardiographyDoppler _ObjUltraResParams_FetalEchocardiographyDoppler;
        public UltraResParams_FetalEchocardiographyDoppler ObjUltraResParams_FetalEchocardiographyDoppler
        {
            get { return _ObjUltraResParams_FetalEchocardiographyDoppler; }
            set
            {
                if (_ObjUltraResParams_FetalEchocardiographyDoppler == value)
                    return;
                _ObjUltraResParams_FetalEchocardiographyDoppler = value;
                if (UCSieuAmTimDoppler != null)
                {
                    ((ISieuAmTT_Doppler)UCSieuAmTimDoppler).SetResultDetails(ObjUltraResParams_FetalEchocardiographyDoppler);
                }
                NotifyOfPropertyChange(() => ObjUltraResParams_FetalEchocardiographyDoppler);
            }
        }
        private UltraResParams_FetalEchocardiographyResult _ObjUltraResParams_FetalEchocardiographyResult;
        public UltraResParams_FetalEchocardiographyResult ObjUltraResParams_FetalEchocardiographyResult
        {
            get { return _ObjUltraResParams_FetalEchocardiographyResult; }
            set
            {
                if (_ObjUltraResParams_FetalEchocardiographyResult == value)
                    return;
                _ObjUltraResParams_FetalEchocardiographyResult = value;
                if (UCSieuAmTimKetLuan != null)
                {
                    ((ISieuAmTT_KetLuan)UCSieuAmTimKetLuan).SetResultDetails(ObjUltraResParams_FetalEchocardiographyResult);
                }
                NotifyOfPropertyChange(() => ObjUltraResParams_FetalEchocardiographyResult);
            }
        }
        private UltraResParams_FetalEchocardiographyPostpartum _ObjUltraResParams_FetalEchocardiographyPostpartum;
        public UltraResParams_FetalEchocardiographyPostpartum ObjUltraResParams_FetalEchocardiographyPostpartum
        {
            get { return _ObjUltraResParams_FetalEchocardiographyPostpartum; }
            set
            {
                if (_ObjUltraResParams_FetalEchocardiographyPostpartum == value)
                    return;
                _ObjUltraResParams_FetalEchocardiographyPostpartum = value;
                if (UCSieuAmTimSauSinh != null)
                {
                    ((ISieuAmTT_SauSinh)UCSieuAmTimSauSinh).SetResultDetails(ObjUltraResParams_FetalEchocardiographyPostpartum);
                }
                NotifyOfPropertyChange(() => ObjUltraResParams_FetalEchocardiographyPostpartum);
            }
        }
        public void Handle(ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest> message)
        {
            if (message != null)
            {
                (UCPatientPCLImageCapture as IImageCapture_V4).ClearAllCapturedImage();
                PatientPCLReqID = message.PCLRequest.PatientPCLReqID;
                LoadInfo();
                Globals.PatientPCLRequest_Result = message.PCLRequest;
            }
        }
        //▼====== #003: Tạo mới hàm này để gọi thay vì phải phải tự bắn sự kiện chính mình
        public void ReLoadInfoPatientPCLRequest(PatientPCLRequest message)
        {
            if (message != null)
            {
                (UCPatientPCLImageCapture as IImageCapture_V4).ClearAllCapturedImage();
                PatientPCLReqID = message.PatientPCLReqID;
                LoadInfo();
                ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).GetImageInAnotherViewModel(Globals.PatientPCLRequest_Result);
            }
        }
        //▲====== #003: 
        int ThreadCount = 0;
        int TotalThread = 5;
        private void LoadInfo()
        {
            ThreadCount = 0;
            this.ShowBusyIndicator();
            GetUltraResParams_FetalEchocardiography();
            GetUltraResParams_FetalEchocardiography2D();
            GetUltraResParams_FetalEchocardiographyDopplerByID();
            GetUltraResParams_FetalEchocardiographyResult();
            GetUltraResParams_FetalEchocardiographyPostpartum();
        }
        private void GetUltraResParams_FetalEchocardiography2D()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetUltraResParams_FetalEchocardiography2D(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetUltraResParams_FetalEchocardiography2D(asyncResult);
                            if (item != null)
                                ObjUltraResParams_FetalEchocardiography2D = item;
                            else
                                ObjUltraResParams_FetalEchocardiography2D = new UltraResParams_FetalEchocardiography2D();
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
        private void GetUltraResParams_FetalEchocardiographyDopplerByID()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetUltraResParams_FetalEchocardiographyDopplerByID(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetUltraResParams_FetalEchocardiographyDopplerByID(asyncResult);
                            if (item != null)
                                ObjUltraResParams_FetalEchocardiographyDoppler = item;
                            else
                                ObjUltraResParams_FetalEchocardiographyDoppler = new UltraResParams_FetalEchocardiographyDoppler();
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
        private void GetUltraResParams_FetalEchocardiographyResult()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetUltraResParams_FetalEchocardiographyResultByID(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetUltraResParams_FetalEchocardiographyResultByID(asyncResult);
                            if (item != null)
                                ObjUltraResParams_FetalEchocardiographyResult = item;
                            else
                                ObjUltraResParams_FetalEchocardiographyResult = new UltraResParams_FetalEchocardiographyResult();
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
        private void GetUltraResParams_FetalEchocardiographyPostpartum()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetUltraResParams_FetalEchocardiographyPostpartumByID(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetUltraResParams_FetalEchocardiographyPostpartumByID(asyncResult);
                            if (item != null)
                                ObjUltraResParams_FetalEchocardiographyPostpartum = item;
                            else
                                ObjUltraResParams_FetalEchocardiographyPostpartum = new UltraResParams_FetalEchocardiographyPostpartum();
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
        private void GetUltraResParams_FetalEchocardiography()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetUltraResParams_FetalEchocardiography(PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetUltraResParams_FetalEchocardiography(asyncResult);
                            if (item != null)
                                ObjUltraResParams_FetalEchocardiography = item;
                            else
                                ObjUltraResParams_FetalEchocardiography = new UltraResParams_FetalEchocardiography();
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
            UltraResParams_FetalEchocardiography entity = new UltraResParams_FetalEchocardiography
            {
                UltraResParams_FetalEchocardiographyID = ObjUltraResParams_FetalEchocardiography.UltraResParams_FetalEchocardiographyID,
                FetalAge = ObjUltraResParams_FetalEchocardiography.FetalAge,
                NuchalTranslucency = ObjUltraResParams_FetalEchocardiography.NuchalTranslucency,
                V_EchographyPosture = ObjUltraResParams_FetalEchocardiography.V_EchographyPosture,
                V_MomMedHis = ObjUltraResParams_FetalEchocardiography.V_MomMedHis,
                Notice = ObjUltraResParams_FetalEchocardiography.Notice
            };
            try
            {
                entity.PCLRequestID = PatientPCLReqID;
                //==== #001
                ObjUltraResParams_FetalEchocardiographyPostpartum.DoctorStaffID = ObjUltraResParams_FetalEchocardiographyPostpartum.DoctorStaffID > 0 ? ObjUltraResParams_FetalEchocardiographyPostpartum.DoctorStaffID : Globals.LoggedUserAccount.Staff.StaffID;
                ObjUltraResParams_FetalEchocardiographyPostpartum.CreateDate = ObjUltraResParams_FetalEchocardiographyPostpartum.CreateDate == DateTime.MinValue ? DateTime.Now : ObjUltraResParams_FetalEchocardiographyPostpartum.CreateDate;
                //==== #001
                entity.ObjUltraResParams_FetalEchocardiography2D = ObjUltraResParams_FetalEchocardiography2D;
                entity.ObjUltraResParams_FetalEchocardiographyDoppler = ObjUltraResParams_FetalEchocardiographyDoppler;
                entity.ObjUltraResParams_FetalEchocardiographyPostpartum = ObjUltraResParams_FetalEchocardiographyPostpartum;
                entity.ObjUltraResParams_FetalEchocardiographyResult = ObjUltraResParams_FetalEchocardiographyResult;
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
                    contract.BeginAddAndUpdateUltraResParams_FetalEchocardiography(entity, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool res = contract.EndAddAndUpdateUltraResParams_FetalEchocardiography(asyncResult);
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
        public void btnPrintCmd()
        {
            //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
            //proAlloc.PatientPCLReqID = (int)PatientPCLReqID;
            //proAlloc.eItem = ReportName.FETAL_ECHOCARDIOGRAPHY;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
            {
                proAlloc.PatientPCLReqID = (int)PatientPCLReqID;
                proAlloc.eItem = ReportName.FETAL_ECHOCARDIOGRAPHY;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }
        //==== 20161129 CMN End: Add button save for all pages
    }
}