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
    [Export(typeof (ISieuAmTimGangSucDipyridamoleHome)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTimGangSucDipyridamoleHomeViewModel : ViewModelBase, ISieuAmTimGangSucDipyridamoleHome
        , IHandle<DbClickSelectedObjectEvent<PatientPCLRequest>>
        , IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
    {
        [ImportingConstructor]
        public SieuAmTimGangSucDipyridamoleHomeViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
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
            var uc3 = Globals.GetViewModel<ISATGSDipyQuyTrinh>();
            SATGSDipyQuyTrinh = uc3;
            var uc4 = Globals.GetViewModel<ISATGSDipy>();
            SATGSDipy = uc4;
            var uc5 = Globals.GetViewModel<ISATGSDipyBenhSu>();
            SATGSDipyBenhSu = uc5;
            var uc6 = Globals.GetViewModel<ISATGSDipyDienTamDo>();
            SATGSDipyDienTamDo = uc6;
            var uc7 = Globals.GetViewModel<ISATGSDipyKetQua>();
            SATGSDipyKetQua = uc7;
            var uc8 = Globals.GetViewModel<ISATGSDipyHinh>();
            SATGSDipyHinh = uc8;
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
        //▼====== #003
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

        public object SATGSDipyBenhSu { get; set; }
        public object SATGSDipyDienTamDo { get; set; }
        public object SATGSDipy { get; set; }
        public object SATGSDipyKetQua { get; set; }
        public object SATGSDipyHinh { get; set; }

        public object SATGSDipyQuyTrinh { get; set; }
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
        private URP_FE_StressDipyridamoleExam _ObjURP_FE_StressDipyridamoleExam;
        public URP_FE_StressDipyridamoleExam ObjURP_FE_StressDipyridamoleExam
        {
            get { return _ObjURP_FE_StressDipyridamoleExam; }
            set
            {
                if (_ObjURP_FE_StressDipyridamoleExam == value)
                    return;
                _ObjURP_FE_StressDipyridamoleExam = value;
                if (SATGSDipyBenhSu != null)
                {
                    ((ISATGSDipyBenhSu)SATGSDipyBenhSu).SetResultDetails(ObjURP_FE_StressDipyridamoleExam, ObjURP_FE_Exam);
                }
                NotifyOfPropertyChange(() => ObjURP_FE_StressDipyridamoleExam);
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
                if (SATGSDipyBenhSu != null)
                {
                    ((ISATGSDipyBenhSu)SATGSDipyBenhSu).SetResultDetails(ObjURP_FE_StressDipyridamoleExam, ObjURP_FE_Exam);
                }
                NotifyOfPropertyChange(() => ObjURP_FE_Exam);
            }
        }
        private URP_FE_StressDipyridamoleElectrocardiogram _ObjURP_FE_StressDipyridamoleElectrocardiogram;
        public URP_FE_StressDipyridamoleElectrocardiogram ObjURP_FE_StressDipyridamoleElectrocardiogram
        {
            get { return _ObjURP_FE_StressDipyridamoleElectrocardiogram; }
            set
            {
                if (_ObjURP_FE_StressDipyridamoleElectrocardiogram == value)
                    return;
                _ObjURP_FE_StressDipyridamoleElectrocardiogram = value;
                if (SATGSDipyDienTamDo != null)
                {
                    ((ISATGSDipyDienTamDo)SATGSDipyDienTamDo).SetResultDetails(ObjURP_FE_StressDipyridamoleElectrocardiogram);
                }
                NotifyOfPropertyChange(() => ObjURP_FE_StressDipyridamoleElectrocardiogram);
            }
        }
        private URP_FE_StressDipyridamole _ObjURP_FE_StressDipyridamole;
        public URP_FE_StressDipyridamole ObjURP_FE_StressDipyridamole
        {
            get { return _ObjURP_FE_StressDipyridamole; }
            set
            {
                if (_ObjURP_FE_StressDipyridamole == value)
                    return;
                _ObjURP_FE_StressDipyridamole = value;
                if (SATGSDipy != null)
                {
                    ((ISATGSDipy)SATGSDipy).SetResultDetails(ObjURP_FE_StressDipyridamole);
                }
                NotifyOfPropertyChange(() => ObjURP_FE_StressDipyridamole);
            }
        }
        private URP_FE_StressDipyridamoleResult _ObjURP_FE_StressDipyridamoleResult;
        public URP_FE_StressDipyridamoleResult ObjURP_FE_StressDipyridamoleResult
        {
            get { return _ObjURP_FE_StressDipyridamoleResult; }
            set
            {
                if (_ObjURP_FE_StressDipyridamoleResult == value)
                    return;
                _ObjURP_FE_StressDipyridamoleResult = value;
                if (SATGSDipyKetQua != null)
                {
                    ((ISATGSDipyKetQua)SATGSDipyKetQua).SetResultDetails(ObjURP_FE_StressDipyridamoleResult);
                }
                NotifyOfPropertyChange(() => ObjURP_FE_StressDipyridamoleResult);
            }
        }
        private URP_FE_StressDipyridamoleImage _ObjURP_FE_StressDipyridamoleImage;
        public URP_FE_StressDipyridamoleImage ObjURP_FE_StressDipyridamoleImage
        {
            get { return _ObjURP_FE_StressDipyridamoleImage; }
            set
            {
                if (_ObjURP_FE_StressDipyridamoleImage == value)
                    return;
                _ObjURP_FE_StressDipyridamoleImage = value;
                if (SATGSDipyHinh != null)
                {
                    ((ISATGSDipyHinh)SATGSDipyHinh).SetResultDetails(ObjURP_FE_StressDipyridamoleImage);
                }
                NotifyOfPropertyChange(() => ObjURP_FE_StressDipyridamoleImage);
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
        int ThreadCount = 0;
        int TotalThread = 6;
        private void LoadInfo()
        {
            ThreadCount = 0;
            this.ShowBusyIndicator();
            GetURP_FE_StressDipyridamoleExam();
            GetURP_FE_Exam();
            GetURP_FE_StressDipyridamoleElectrocardiogram();
            GetURP_FE_StressDipyridamoleResult();
            GetURP_FE_StressDipyridamoleImage();
            GetURP_FE_StressDipyridamole();
        }
        private void GetURP_FE_StressDipyridamoleExam()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_StressDipyridamoleExam(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetURP_FE_StressDipyridamoleExam(asyncResult);
                            if (item != null)
                                ObjURP_FE_StressDipyridamoleExam = item;
                            else
                                ObjURP_FE_StressDipyridamoleExam = new URP_FE_StressDipyridamoleExam();
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
        private void GetURP_FE_StressDipyridamoleElectrocardiogram()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_StressDipyridamoleElectrocardiogram(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetURP_FE_StressDipyridamoleElectrocardiogram(asyncResult);
                            if (item != null)
                                ObjURP_FE_StressDipyridamoleElectrocardiogram = item;
                            else
                                ObjURP_FE_StressDipyridamoleElectrocardiogram = new URP_FE_StressDipyridamoleElectrocardiogram();
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
        private void GetURP_FE_StressDipyridamoleResult()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_StressDipyridamoleResult(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetURP_FE_StressDipyridamoleResult(asyncResult);
                            if (item != null)
                                ObjURP_FE_StressDipyridamoleResult = item;
                            else
                                ObjURP_FE_StressDipyridamoleResult = new URP_FE_StressDipyridamoleResult();
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
        private void GetURP_FE_StressDipyridamoleImage()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_StressDipyridamoleImage(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetURP_FE_StressDipyridamoleImage(asyncResult);
                            if (item != null)
                                ObjURP_FE_StressDipyridamoleImage = item;
                            else
                                ObjURP_FE_StressDipyridamoleImage = new URP_FE_StressDipyridamoleImage();
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
        private void GetURP_FE_StressDipyridamole()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetURP_FE_StressDipyridamole(0, PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var item = contract.EndGetURP_FE_StressDipyridamole(asyncResult);
                            if (item != null)
                                ObjURP_FE_StressDipyridamole = item;
                            else
                                ObjURP_FE_StressDipyridamole = new URP_FE_StressDipyridamole();

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
            if (!(SATGSDipyDienTamDo as ISATGSDipyDienTamDo).CheckValidate()
                || !(SATGSDipyBenhSu as ISATGSDipyBenhSu).CheckValidate()
                || !(SATGSDipy as ISATGSDipy).CheckValidate()
                || !(SATGSDipyKetQua as ISATGSDipyKetQua).CheckValidate()
                || !(SATGSDipyHinh as ISATGSDipyHinh).CheckValidate())
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
            URP_FE_StressDipyridamoleUltra entity = new URP_FE_StressDipyridamoleUltra();
            try
            {
                entity.PCLRequestID = PatientPCLReqID;
                //==== #001
                //ObjURP_FE_StressDipyridamole.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                ObjURP_FE_StressDipyridamoleImage.DoctorStaffID = ObjURP_FE_StressDipyridamoleImage.DoctorStaffID > 0 ? ObjURP_FE_StressDipyridamoleImage.DoctorStaffID : Globals.LoggedUserAccount.Staff.StaffID;
                ObjURP_FE_StressDipyridamoleImage.CreateDate = ObjURP_FE_StressDipyridamoleImage.CreateDate == DateTime.MinValue ? DateTime.Now : ObjURP_FE_StressDipyridamoleImage.CreateDate;
                //==== #001
                entity.ObjURP_FE_StressDipyridamoleExam = ObjURP_FE_StressDipyridamoleExam;
                entity.ObjURP_FE_Exam = ObjURP_FE_Exam;
                entity.ObjURP_FE_StressDipyridamoleElectrocardiogram = ObjURP_FE_StressDipyridamoleElectrocardiogram;
                entity.ObjURP_FE_StressDipyridamole = ObjURP_FE_StressDipyridamole;
                entity.ObjURP_FE_StressDipyridamoleResult = ObjURP_FE_StressDipyridamoleResult;
                entity.ObjURP_FE_StressDipyridamoleImage = ObjURP_FE_StressDipyridamoleImage;
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
                    contract.BeginAddAndUpdateURP_FE_StressDipyridamole(entity, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool res = contract.EndAddAndUpdateURP_FE_StressDipyridamole(asyncResult);
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
        //==== 20161129 CMN End.
    }
}