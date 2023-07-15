using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.ServiceClient;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using PCLsProxy;
using System.Collections.Generic;
using aEMR.CommonTasks;
using aEMR.Common.BaseModel;
using System.Linq;
/*
* 20170505 #001 CMN: Added InPt print able
* 20180613 #002 TBLD: Kiem tra ma may truoc khi luu hoac cap nhat
* 20182508 #003 TTM:
* 20181001 #004 TBL: BM 0000106. Fix PatientInfo không hiển thị thông tin
*/
namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof (ISieuAmTim)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTimViewModel : ViewModelBase, ISieuAmTim
        , IHandle<DbClickSelectedObjectEvent<PatientPCLRequest>>
        /*#003: Vẫn giữ để chụp lấy sự kiện load thông tin từ view Search đưa xuống.*/
        , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
        , IHandle<ReloadEchoCardiographyResult>
        , IHandle<LoadPatientPCLImagingResultDataCompletedEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTimViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Initialize();

            /*▼====: #004*/
            CreateSubVM();
            /*▲====: #004*/
            //▼====== #003 Comment Subscrible đem xuống OnActive
            //eventAggregator.Subscribe(this);
            //▲====== #003
            
        }

        public void SetVideoImageCaptureSourceVM(object theVideoVM)
        {
            UCPatientPCLImageCapture = theVideoVM;
        }

        //▼====== #003 Đem if từ Initialize lên để thực hiện 1 lần khi active mà thôi
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
        private void Initialize()
        {
            ObjPatientPCLImagingResult = new PatientPCLImagingResult();
            ObjPatientPCLImagingResult.StaffID = Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : (Globals.LoggedUserAccount.StaffID.HasValue ? Globals.LoggedUserAccount.StaffID.Value : -1);
            ObjPatientPCLImagingResult.PatientPCLReqID = -1;
            ObjPatientPCLImagingResult.PCLExamTypeID = -1;
            ObjPatientPCLImagingResult.PCLExamDate = Globals.ServerDate.GetValueOrDefault(DateTime.Now);
            ObjPatientPCLImagingResult.PCLExamForOutPatient = true;
            ObjPatientPCLImagingResult.IsExternalExam = false;            

            vmSearchPCLRequest = Globals.GetViewModel<IPCLDepartmentSearchPCLRequest>();
            vmSearchPCLRequest.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Imaging;
        }

        /*▼====: #004*/
        public void CreateSubVM()
        {
            UCDoctorProfileInfo = Globals.GetViewModel<ILoginInfo>();

            UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();

            UCSieuAmTimTM = Globals.GetViewModel<ISieuAmTim_TM>();

            UCSieuAmTimTwoD = Globals.GetViewModel<ISieuAmTim_2D>();

            UCSieuAmTimDoppler = Globals.GetViewModel<ISieuAmTim_Droppler>();

            UCSieuAmTimKetLuan = Globals.GetViewModel<ISieuAmTim_KetLuan>();

            var uc9 = Globals.GetViewModel<IPatientPCLDeptImagingResult>();
            uc9.ObjPatientPCLImagingResult = ObjPatientPCLImagingResult;
            UCPatientPCLImageResults = uc9;

            //var uc10 = Globals.GetViewModel<IImageCapture_V3>();
            //uc10.ObjPatientPCLImagingResult = ObjPatientPCLImagingResult;
            //UCPatientPCLImageCapture = uc10;

            UCPatientPCLGeneralResult = Globals.GetViewModel<IPatientPCLGeneralResult>();
        }
        public void ActivateSubVM()
        {
            ActivateItem(UCDoctorProfileInfo);
            ActivateItem(UCPatientProfileInfo);
            ActivateItem(UCSieuAmTimTM);
            ActivateItem(UCSieuAmTimTwoD);
            ActivateItem(UCSieuAmTimDoppler);
            ActivateItem(UCSieuAmTimKetLuan);
            ActivateItem(UCPatientPCLImageResults);
            //ActivateItem(UCPatientPCLImageCapture);
            ActivateItem(UCPatientPCLGeneralResult);
        }

        public void DeActivateSubVM(bool close)
        {
            DeactivateItem(UCDoctorProfileInfo, close);
            DeactivateItem(UCPatientProfileInfo, close);
            DeactivateItem(UCSieuAmTimTM, close);
            DeactivateItem(UCSieuAmTimTwoD, close);
            DeactivateItem(UCSieuAmTimDoppler, close);
            DeactivateItem(UCSieuAmTimKetLuan, close);
            DeactivateItem(UCPatientPCLImageResults, close);
            //DeactivateItem(UCPatientPCLImageCapture, close);

        }
        /*▲====: #004*/
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
        /*==== #001 ====*/
        private DataEntities.AllLookupValues.V_PCLRequestType PCLRequestType { get; set; }
        /*==== #001 ====*/

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

        public object UCPatientPCLImageCapture { get; set; }

        public object UCSieuAmTimTM { get; set; }

        public object UCSieuAmTimTwoD { get; set; }
        public object UCSieuAmTimDoppler { get; set; }
        public object UCSieuAmTimKetLuan { get; set; }
        public object UCHinhAnhSieuAm { get; set; }


        public TabItem gTabFirst { get; set; }
        public void TabFirst_Loaded(object sender, RoutedEventArgs e)
        {
            gTabFirst = sender as TabItem;
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

        private UltraResParams_EchoCardiography _curUltraResParams_EchoCardiography;
        public UltraResParams_EchoCardiography curUltraResParams_EchoCardiography
        {
            get { return _curUltraResParams_EchoCardiography; }
            set
            {
                if (_curUltraResParams_EchoCardiography == value)
                    return;
                _curUltraResParams_EchoCardiography = value;

                if (UCSieuAmTimTM != null)
                {
                    ((ISieuAmTim_TM)UCSieuAmTimTM).SetResultDetails(_curUltraResParams_EchoCardiography);
                }
                if (UCSieuAmTimTwoD != null)
                {
                    ((ISieuAmTim_2D)UCSieuAmTimTwoD).SetResultDetails(_curUltraResParams_EchoCardiography);
                }
                if (UCSieuAmTimDoppler != null)
                {
                    ((ISieuAmTim_Droppler)UCSieuAmTimDoppler).SetResultDetails(_curUltraResParams_EchoCardiography);
                }
                if (UCSieuAmTimKetLuan != null)
                {
                    ((ISieuAmTim_KetLuan)UCSieuAmTimKetLuan).SetResultDetails(_curUltraResParams_EchoCardiography);
                }

                NotifyOfPropertyChange(() => curUltraResParams_EchoCardiography);
            }
        }
       

        public void Handle(DbClickSelectedObjectEvent<PatientPCLRequest> message)
        {
            if (message != null)
            {   
                gTabFirst.IsSelected = true;
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
                /*==== #001 ====*/
                PCLRequestType = message.PCLRequest.V_PCLRequestType;
                /*==== #001 ====*/
                LoadInfo();
                Globals.PatientPCLRequest_Result = message.PCLRequest;
            }
        }
        //▼====== #003 Tạo hàm để gọi thay vì tự bắn chính mình
        public void ReLoadInfoPatientPCLRequest(PatientPCLRequest patientPCLrequest)
        {
            if (patientPCLrequest != null)
            {
                (UCPatientPCLImageCapture as IImageCapture_V4).ClearAllCapturedImage();
                PatientPCLReqID = patientPCLrequest.PatientPCLReqID;
                PCLRequestType = patientPCLrequest.V_PCLRequestType;
                LoadInfo();
                ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).GetImageInAnotherViewModel(Globals.PatientPCLRequest_Result);
            }
        }
        //▲====== #003
        public void Handle(ReloadEchoCardiographyResult message)
        {
            LoadInfo();
        }

        private void LoadInfo()
        {
            curUltraResParams_EchoCardiography = new UltraResParams_EchoCardiography();
            // TxD 29/06/2015 : This is a Hot Fix to set default value for Doppler Mitral EA this should not affect any new values set later.
            //curUltraResParams_EchoCardiography.DOPPLER_Mitral_Ea = ">1"; 
            curUltraResParams_EchoCardiography.DOPPLER_Mitral_Ea = " "; 

            GetUltraResParams_EchoCardiographyByID(0, PatientPCLReqID);
        }

        private void GetUltraResParams_EchoCardiographyByID(long UltraResParams_EchoCardiographyID, long PCLImgResultID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetUltraResParams_EchoCardiography(UltraResParams_EchoCardiographyID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetUltraResParams_EchoCardiography(asyncResult);
                            if (item != null)
                            {
                                curUltraResParams_EchoCardiography = item;
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
            if (PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0468_G1_Chon1YCCDoanHA);
                return;
            }

            if (curUltraResParams_EchoCardiography.UltraResParams_EchoCardiographyID <= 0)
            {
                MessageBox.Show("Kết quả Chẩn đoán hình ảnh chưa được lưu!");
                return;
            }

            //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
            //proAlloc.PatientPCLReqID = (int)PatientPCLReqID;
            ///*==== #001 ====*/
            //proAlloc.FindPatient = PCLRequestType == AllLookupValues.V_PCLRequestType.NOI_TRU ? 1 : 0;
            ///*==== #001 ====*/
            //proAlloc.eItem = ReportName.PCLDEPARTMENT_IMAGERESULT_HEART_ULTRASOUND;

            ////WriteableBitmap bmpImg1 = ((IImageCapture_V2)UCPatientPCLImageCapture).GetCapturedImage();
            ////proAlloc.SetHeartUltraImgageResult1(bmpImg1);

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
                /*==== #001 ====*/
                proAlloc.FindPatient = PCLRequestType == AllLookupValues.V_PCLRequestType.NOI_TRU ? 1 : 0;
                /*==== #001 ====*/
                proAlloc.eItem = ReportName.PCLDEPARTMENT_IMAGERESULT_HEART_ULTRASOUND;

                //WriteableBitmap bmpImg1 = ((IImageCapture_V2)UCPatientPCLImageCapture).GetCapturedImage();
                //proAlloc.SetHeartUltraImgageResult1(bmpImg1);

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
            if(PatientPCLReqID<=0)
            {
                MessageBox.Show(eHCMSResources.A0301_G1_Msg_InfoChonBN);
                return;
            }
            /*▼====: #002*/
            if (Globals.ServerConfigSection.CommonItems.OrganizationUseSoftware == 0 && !CheckValid())
            {
                return;
            }
            /*▲====: #002*/
            curUltraResParams_EchoCardiography.DoctorStaffID = Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : (Globals.LoggedUserAccount.StaffID.HasValue ? Globals.LoggedUserAccount.StaffID.Value : -1);

            if (curUltraResParams_EchoCardiography.UltraResParams_EchoCardiographyID > 0)
            {
                if (curUltraResParams_EchoCardiography.Tab1_TM_Changed)
                    curUltraResParams_EchoCardiography.Tab1_TM_Update_Required = true;
                if (curUltraResParams_EchoCardiography.Tab2_2D_Changed)
                    curUltraResParams_EchoCardiography.Tab2_2D_Update_Required = true;
                if (curUltraResParams_EchoCardiography.Tab3_Doppler_Changed)
                    curUltraResParams_EchoCardiography.Tab3_Doppler_Update_Required = true;
                if (curUltraResParams_EchoCardiography.Tab4_Conclusion_Changed)
                    curUltraResParams_EchoCardiography.Tab4_Conclusion_Update_Required = true;

                //==== 20161013 CMN Begin: Add PCL Image Method
                curUltraResParams_EchoCardiography.Tab1_TM_Update_Required = true;
                //==== 20161013 CMN Begin: End.

                UpdateUltraResParams_EchoCardiography(curUltraResParams_EchoCardiography);
            }
            else
            {
                curUltraResParams_EchoCardiography.PCLImgResultID = PatientPCLReqID;
                
                AddUltraResParams_EchoCardiography(curUltraResParams_EchoCardiography);
            }
        }
        private void AddUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity)
        {
            this.ShowBusyIndicator();
            //==== 20161013 CMN Begin: Add PCL Image Method
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
                MessageBox.Show("UpdateUltraResParams_EchoCardiography Get File: " + ex.Message);
            }
            string TemplateResultDescription;
            string TemplateResult;
            string TemplateResultString = UCPatientPCLGeneralResult.GetBodyValue(FileForStore,FileForDelete,out TemplateResultDescription, out TemplateResult);
            if (UCPatientPCLGeneralResult != null && !string.IsNullOrEmpty(TemplateResultString))
            {
                ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.TemplateResult = TemplateResult;
                ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.TemplateResultDescription = TemplateResultDescription;
                ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.TemplateResultString = TemplateResultString;
            }
            //==== 20161013 CMN End.
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    try
                    {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginAddUltraResParams_EchoCardiography(entity, ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult, FileForStore, FileForDelete,
                            Globals.DispatchCallback(
                                (asyncResult) =>
                                {
                                    try
                                    {
                                        bool res =contract.EndAddUltraResParams_EchoCardiography(asyncResult);
                                        if (res)
                                        {
                                            MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
                                            LoadInfo();
                                            //==== 20161013 CMN Begin: Add PCL Image Method
                                            IPatientPCLDeptImagingResult vImagingResult = UCPatientPCLImageResults as IPatientPCLDeptImagingResult;
                                            vImagingResult.LoadData(((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.PatientPCLRequest);
                                            IImageCapture_V4 vImageCapture = UCPatientPCLImageCapture as IImageCapture_V4;
                                            vImageCapture.ClearSelectedImage();
                                            //==== 20161013 CMN End.
                                        }
                                        else
                                        {
                                            MessageBox.Show("Thêm mới thất bại!");
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

        private void UpdateUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity)
        {
            this.ShowBusyIndicator();
            //==== 20161013 CMN Begin: Add PCL Image Method
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
                MessageBox.Show("UpdateUltraResParams_EchoCardiography Get File: " + ex.Message);
            }
            string TemplateResultDescription;
            string TemplateResult;
            string TemplateResultString = UCPatientPCLGeneralResult.GetBodyValue(FileForStore,FileForDelete,out TemplateResultDescription, out TemplateResult);
            if (UCPatientPCLGeneralResult != null && !string.IsNullOrEmpty(TemplateResultString))
            {
                ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.TemplateResult = TemplateResult;
                ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.TemplateResultDescription = TemplateResultDescription;
                ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.TemplateResultFileName = ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.TemplateResultString;
                ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.TemplateResultString = TemplateResultString;
            }
            //==== 20161013 CMN End.
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    try
                    {
                        IPCLs contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateUltraResParams_EchoCardiography(entity, ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult, FileForStore, FileForDelete,
                                Globals.DispatchCallback((asyncResult) =>
                                    {
                                        try
                                        {
                                            bool res = contract.EndUpdateUltraResParams_EchoCardiography(asyncResult);
                                            if (res)
                                            {
                                                MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                                LoadInfo();
                                                //==== 20161013 CMN Begin: Add PCL Image Method
                                                IPatientPCLDeptImagingResult vImagingResult = UCPatientPCLImageResults as IPatientPCLDeptImagingResult;
                                                vImagingResult.LoadData(((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult.PatientPCLRequest);
                                                IImageCapture_V4 vImageCapture = UCPatientPCLImageCapture as IImageCapture_V4;
                                                vImageCapture.ClearSelectedImage();
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

        private IPatientPCLGeneralResult _UCPatientPCLGeneralResult;
        public IPatientPCLGeneralResult UCPatientPCLGeneralResult
        {
            get => _UCPatientPCLGeneralResult; set
            {
                _UCPatientPCLGeneralResult = value;
                NotifyOfPropertyChange(() => UCPatientPCLGeneralResult);
            }
        }
        public void Handle(LoadPatientPCLImagingResultDataCompletedEvent message)
        {
            if (message != null && UCPatientPCLImageResults != null
                && ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult != null)
            {
                ObjPatientPCLImagingResult = ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).ObjPatientPCLImagingResult;
                // 20200514 TNHX: Lấy cột HiddenFullNameOnReport để ẩn bsy thực hiện kết quả CLS (HA)
                string PerformStaffFullName = ObjPatientPCLImagingResult.Staff == null ? "" : ObjPatientPCLImagingResult.Staff.FullName;
                if (ObjPatientPCLImagingResult.Staff != null)
                {
                    Staff PerfromStaff = Globals.AllStaffs.Where(x => x.StaffID == ObjPatientPCLImagingResult.Staff.StaffID).FirstOrDefault();
                    PerformStaffFullName = PerfromStaff.HiddenFullNameOnReport ? "" : ObjPatientPCLImagingResult.Staff.FullName;
                }
                UCPatientPCLGeneralResult.ApplyElementValues(ObjPatientPCLImagingResult.TemplateResultString, ObjPatientPCLImagingResult.PatientPCLRequest, ((IPatientPCLDeptImagingResult)UCPatientPCLImageResults).PCLResultFileStorageDetailCollection, ObjPatientPCLImagingResult.PtRegistrationCode, PerformStaffFullName, ObjPatientPCLImagingResult.Suggest);
            }
        }
    }
}