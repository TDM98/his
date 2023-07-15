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
using System.Threading;
using aEMR.ServiceClient;
using PCLsProxy;
using System.Collections.Generic;
using eHCMSLanguage;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Common.BaseModel;

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof (ISieuAmTimGangSucDobutamineHome)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTimGangSucDobutamineHomeViewModel : ViewModelBase, ISieuAmTimGangSucDobutamineHome
        , IHandle<DbClickSelectedObjectEvent<PatientPCLRequest>>
        , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTimGangSucDobutamineHomeViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
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
            var uc3 = Globals.GetViewModel<ISATGSDobuQuyTrinh>();
            SATGSDobuQuyTrinh = uc3;
            var uc4 = Globals.GetViewModel<ISATGSDobu>();
            SATGSDobu = uc4;
            var uc5 = Globals.GetViewModel<ISATGSDobuBenhSu>();
            SATGSDobuBenhSu = uc5;
            var uc6 = Globals.GetViewModel<ISATGSDobuDienTamDo>();
            SATGSDobuDienTamDo = uc6;
            var uc7 = Globals.GetViewModel<ISATGSDobuKetQua>();
            SATGSDobuKetQua = uc7;
            var uc8 = Globals.GetViewModel<ISATGSDobuHinh>();
            SATGSDobuHinh = uc8;
            ObjPatientPCLImagingResult = new PatientPCLImagingResult
            {
                StaffID = Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : (Globals.LoggedUserAccount.StaffID.HasValue ? Globals.LoggedUserAccount.StaffID.Value : -1),
                PatientPCLReqID = -1,
                PCLExamTypeID = -1,
                PCLExamDate = Globals.ServerDate.GetValueOrDefault(DateTime.Now),
                PCLExamForOutPatient = true,
                IsExternalExam = false
            };
            var uc9 = Globals.GetViewModel<IPatientPCLDeptImagingResult>();
            uc9.ObjPatientPCLImagingResult = ObjPatientPCLImagingResult;
            UCPatientPCLImageResults = uc9;

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
                //#003 Sự kiện ReaderInfoPatientFromPatientPCLReqEvent ở đây đang thực hiện việc tự mình bắn mình, không cần thiết
                //Anh Tuấn bảo chỉ cần gọi không cần phải bắn. Nếu tự mình bắn mình thì tương tự như tự giết mình.
                //Globals.EventAggregator.Publish(new ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>() { PCLRequest = pcl });
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

        //Tạo hàm để gọi thay vì tự bắn chính mình
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

        public object UCPatientPCLImageResults { get; set; }

        public object SATGSDobuBenhSu { get; set; }
        public object SATGSDobuDienTamDo { get; set; }
        public object SATGSDobu { get; set; }
        public object SATGSDobuKetQua { get; set; }
        public object SATGSDobuHinh { get; set; }

        public object SATGSDobuQuyTrinh { get; set; }
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
        //==== 20161202 CMN Begin: Add button save for all pages
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
        private URP_FE_StressDobutamineExam _ObjURP_FE_StressDobutamineExam;
        public URP_FE_StressDobutamineExam ObjURP_FE_StressDobutamineExam
        {
            get { return _ObjURP_FE_StressDobutamineExam; }
            set
            {
                if (_ObjURP_FE_StressDobutamineExam == value)
                    return;
                _ObjURP_FE_StressDobutamineExam = value;
                if (SATGSDobuBenhSu != null)
                {
                    ((ISATGSDobuBenhSu)SATGSDobuBenhSu).SetResultDetails(ObjURP_FE_StressDobutamineExam, ObjURP_FE_Exam);
                }
                NotifyOfPropertyChange(() => ObjURP_FE_StressDobutamineExam);
            }
        }
        private URP_FE_Exam _ObjURP_FE_Exam;
        public URP_FE_Exam ObjURP_FE_Exam
        {
            get { return _ObjURP_FE_Exam; }
            set
            {
                if (_ObjURP_FE_Exam == value)
                    return;
                _ObjURP_FE_Exam = value;
                if (SATGSDobuBenhSu != null)
                {
                    ((ISATGSDobuBenhSu)SATGSDobuBenhSu).SetResultDetails(ObjURP_FE_StressDobutamineExam, ObjURP_FE_Exam);
                }
                NotifyOfPropertyChange(() => ObjURP_FE_Exam);
            }
        }
        private URP_FE_StressDobutamineElectrocardiogram _ObjURP_FE_StressDobutamineElectrocardiogram;
        public URP_FE_StressDobutamineElectrocardiogram ObjURP_FE_StressDobutamineElectrocardiogram
        {
            get { return _ObjURP_FE_StressDobutamineElectrocardiogram; }
            set
            {
                if (_ObjURP_FE_StressDobutamineElectrocardiogram == value)
                    return;
                _ObjURP_FE_StressDobutamineElectrocardiogram = value;
                if (SATGSDobuDienTamDo != null)
                {
                    ((ISATGSDobuDienTamDo)SATGSDobuDienTamDo).SetResultDetails(ObjURP_FE_StressDobutamineElectrocardiogram);
                }
                NotifyOfPropertyChange(() => ObjURP_FE_StressDobutamineElectrocardiogram);
            }
        }
        private URP_FE_StressDobutamine _ObjURP_FE_StressDobutamine;
        public URP_FE_StressDobutamine ObjURP_FE_StressDobutamine
        {
            get { return _ObjURP_FE_StressDobutamine; }
            set
            {
                if (_ObjURP_FE_StressDobutamine == value)
                    return;
                _ObjURP_FE_StressDobutamine = value;
                if (SATGSDobu != null)
                {
                    ((ISATGSDobu)SATGSDobu).SetResultDetails(ObjURP_FE_StressDobutamine);
                }
                NotifyOfPropertyChange(() => ObjURP_FE_StressDobutamine);
            }
        }
        private URP_FE_StressDobutamineResult _ObjURP_FE_StressDobutamineResult;
        public URP_FE_StressDobutamineResult ObjURP_FE_StressDobutamineResult
        {
            get { return _ObjURP_FE_StressDobutamineResult; }
            set
            {
                if (_ObjURP_FE_StressDobutamineResult == value)
                    return;
                _ObjURP_FE_StressDobutamineResult = value;
                if (SATGSDobuKetQua != null)
                {
                    ((ISATGSDobuKetQua)SATGSDobuKetQua).SetResultDetails(ObjURP_FE_StressDobutamineResult);
                }
                NotifyOfPropertyChange(() => ObjURP_FE_StressDobutamineResult);
            }
        }
        private URP_FE_StressDobutamineImages _ObjURP_FE_StressDobutamineImages;
        public URP_FE_StressDobutamineImages ObjURP_FE_StressDobutamineImages
        {
            get { return _ObjURP_FE_StressDobutamineImages; }
            set
            {
                if (_ObjURP_FE_StressDobutamineImages == value)
                    return;
                _ObjURP_FE_StressDobutamineImages = value;
                if (SATGSDobuHinh != null)
                {
                    ((ISATGSDobuHinh)SATGSDobuHinh).SetResultDetails(ObjURP_FE_StressDobutamineImages);
                }
                NotifyOfPropertyChange(() => ObjURP_FE_StressDobutamineImages);
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
        private void GetURP_FE_StressDobutamineExam()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_StressDobutamineExam(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetURP_FE_StressDobutamineExam(asyncResult);
                            if (item != null)
                                ObjURP_FE_StressDobutamineExam = item;
                            else
                                ObjURP_FE_StressDobutamineExam = new URP_FE_StressDobutamineExam();
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
        private void GetURP_FE_Exam()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_Exam(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetURP_FE_Exam(asyncResult);
                            if (item != null)
                                ObjURP_FE_Exam = item;
                            else
                                ObjURP_FE_Exam = new URP_FE_Exam();
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
        private void GetURP_FE_StressDobutamineElectrocardiogram()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_StressDobutamineElectrocardiogram(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetURP_FE_StressDobutamineElectrocardiogram(asyncResult);
                            if (item != null)
                                ObjURP_FE_StressDobutamineElectrocardiogram = item;
                            else
                                ObjURP_FE_StressDobutamineElectrocardiogram = new URP_FE_StressDobutamineElectrocardiogram();
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
        private void GetURP_FE_StressDobutamineResult()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_StressDobutamineResult(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetURP_FE_StressDobutamineResult(asyncResult);
                            if (item != null)
                                ObjURP_FE_StressDobutamineResult = item;
                            else
                                ObjURP_FE_StressDobutamineResult = new URP_FE_StressDobutamineResult();
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
        private void GetURP_FE_StressDobutamineImage()
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
                                ObjURP_FE_StressDobutamineImages = item;
                            else
                                ObjURP_FE_StressDobutamineImages = new URP_FE_StressDobutamineImages();
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
        private void GetURP_FE_StressDobutamine()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_StressDobutamine(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetURP_FE_StressDobutamine(asyncResult);
                            if (item != null)
                                ObjURP_FE_StressDobutamine = item;
                            else
                                ObjURP_FE_StressDobutamine = new URP_FE_StressDobutamine();

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
        int ThreadCount = 0;
        int TotalThread = 6;
        private void LoadInfo()
        {
            ThreadCount = 0;
            this.ShowBusyIndicator();
            GetURP_FE_StressDobutamineExam();
            GetURP_FE_Exam();
            GetURP_FE_StressDobutamineElectrocardiogram();
            GetURP_FE_StressDobutamineResult();
            GetURP_FE_StressDobutamineImage();
            GetURP_FE_StressDobutamine();
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
            if (!(SATGSDobuDienTamDo as ISATGSDobuDienTamDo).CheckValidate()
                || !(SATGSDobuBenhSu as ISATGSDobuBenhSu).CheckValidate()
                || !(SATGSDobu as ISATGSDobu).CheckValidate()
                || !(SATGSDobuKetQua as ISATGSDobuKetQua).CheckValidate()
                || !(SATGSDobuHinh as ISATGSDobuHinh).CheckValidate())
            {
                MessageBox.Show(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe);
                return;
            }
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
            URP_FE_StressDobutamineUltra entity = new URP_FE_StressDobutamineUltra();
            try
            {
                entity.PCLRequestID = PatientPCLReqID;
                //==== #001
                //ObjURP_FE_StressDobutamine.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                ObjURP_FE_StressDobutamineImages.DoctorStaffID = ObjURP_FE_StressDobutamineImages.DoctorStaffID > 0 ? ObjURP_FE_StressDobutamineImages.DoctorStaffID : Globals.LoggedUserAccount.Staff.StaffID;
                ObjURP_FE_StressDobutamineImages.CreateDate = ObjURP_FE_StressDobutamineImages.CreateDate == DateTime.MinValue ? DateTime.Now : ObjURP_FE_StressDobutamineImages.CreateDate;
                //==== #001
                entity.ObjURP_FE_StressDobutamineExam = ObjURP_FE_StressDobutamineExam;
                entity.ObjURP_FE_Exam = ObjURP_FE_Exam;
                entity.ObjURP_FE_StressDobutamineElectrocardiogram = ObjURP_FE_StressDobutamineElectrocardiogram;
                entity.ObjURP_FE_StressDobutamine = ObjURP_FE_StressDobutamine;
                entity.ObjURP_FE_StressDobutamineResult = ObjURP_FE_StressDobutamineResult;
                entity.ObjURP_FE_StressDobutamineImages = ObjURP_FE_StressDobutamineImages;
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
                    contract.BeginAddAndUpdateURP_FE_StressDobutamine(entity, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool res = contract.EndAddAndUpdateURP_FE_StressDobutamine(asyncResult);
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
        //==== 20161202 CMN End.
    }
}