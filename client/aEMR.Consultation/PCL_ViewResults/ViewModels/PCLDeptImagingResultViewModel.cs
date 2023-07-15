using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using PCLsProxy;
using PCLsService;
using System.Windows.Media.Imaging;
using aEMR.CommonTasks;
using aEMR.Common.Collections;
using Castle.Windsor;
using aEMR.Common.BaseModel;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IPCLDeptImagingResult_Consultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLDeptImagingResultViewModel : ViewModelBase, IPCLDeptImagingResult_Consultation
        , IHandle<PCLDeptImagingResultLoadEvent>
    {
        #region Indicator Member

        private bool _isLoadingConfig = false;
        public bool isLoadingConfig
        {
            get { return _isLoadingConfig; }
            set
            {
                if (_isLoadingConfig != value)
                {
                    _isLoadingConfig = value;
                    NotifyOfPropertyChange(() => isLoadingConfig);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingParent = false;
        public bool isLoadingParent
        {
            get { return _isLoadingParent; }
            set
            {
                if (_isLoadingParent != value)
                {
                    _isLoadingParent = value;
                    NotifyOfPropertyChange(() => isLoadingParent);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingDetail = false;
        public bool isLoadingDetail
        {
            get { return _isLoadingDetail; }
            set
            {
                if (_isLoadingDetail != value)
                {
                    _isLoadingDetail = value;
                    NotifyOfPropertyChange(() => isLoadingDetail);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingOperator = false;
        public bool isLoadingOperator
        {
            get { return _isLoadingOperator; }
            set
            {
                if (_isLoadingOperator != value)
                {
                    _isLoadingOperator = value;
                    NotifyOfPropertyChange(() => isLoadingOperator);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }


        public bool IsLoading
        {
            get { return (isLoadingParent || isLoadingDetail || isLoadingOperator || isLoadingConfig); }
        }

        #endregion


        private string ImagePool;
        private string ImageStore;
        private string ImageThumbTemp;
        private string _FolderListTextSelect;
        private ObservableCollection<String> _Folders;

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _currentView = view as IPCLDeptImagingResultView_Consultation;
        }
        IPCLDeptImagingResultView_Consultation _currentView;

        /// <summary>
        /// Các Biến Giá Trị
        /// </summary>

        private ObservableCollection<PCLResultFileStorageDetail> _ObjGetPCLResultFileStoreDetails;
        private ObservableCollection<PCLExamType> _ObjPCLExamTypes_ByPatientPCLReqID;
        private ObservableCollection<PatientPCLRequest> _ObjPatientPCLRequest_ByPtRegIDV_PCLCategory;
        private int _OptionValue;
        private PCLResultFileStorageDetail _PCLResultFileStorageDetailSelected;
        private long _V_ResultType;
        private string _cboListFolderSelectedValue;

        #region Property Enable

        private bool _HasPatient;

        public bool HasPatient
        {
            get { return _HasPatient; }
            set
            {
                if (_HasPatient != value)
                {
                    _HasPatient = value;
                    NotifyOfPropertyChange(() => HasPatient);
                }
            }
        }

        #endregion
        [ImportingConstructor]
        public PCLDeptImagingResultViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            ObjPatientPCLRequest_ByPtRegIDV_PCLCategory = new ObservableCollection<PatientPCLRequest>();
            ObjPCLExamTypes_ByPatientPCLReqID = new ObservableCollection<PCLExamType>();
            ObjGetPCLResultFileStoreDetails = new ObservableCollection<PCLResultFileStorageDetail>();

            FolderListTextSelect = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon);

            GetTestingAgency_All();

            CheckFormStartValid();

            KhoiTao();

            GetAppConfigValue();
            //khong hieu vi sao lai lam viec voi thang Global o day rat nguy hiem
            //if (Globals.PatientPCLRequest_Imaging != null && Globals.PatientPCLRequest_Imaging.PatientPCLReqID > 0)
            //{
            //    LoadDataCoroutineEx(Globals.PatientPCLRequest_Imaging);
            //}
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            Globals.EventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

        public void Handle(PCLDeptImagingResultLoadEvent message)
        {
            if (message != null
                && message.PatientPCLRequest_Imaging != null
                && message.PatientPCLRequest_Imaging.PatientPCLReqID > 0)
            {
                LoadDataCoroutineEx(message.PatientPCLRequest_Imaging);
            }
        }

        private PatientPCLRequest_Ext _curPatientPCLRequest_Ext;
        public PatientPCLRequest_Ext curPatientPCLRequest_Ext
        {
            get { return _curPatientPCLRequest_Ext; }
            set
            {
                _curPatientPCLRequest_Ext = value;
                NotifyOfPropertyChange(() => curPatientPCLRequest_Ext);
            }
        }

        public ObservableCollection<PatientPCLRequest> ObjPatientPCLRequest_ByPtRegIDV_PCLCategory
        {
            get { return _ObjPatientPCLRequest_ByPtRegIDV_PCLCategory; }
            set
            {
                _ObjPatientPCLRequest_ByPtRegIDV_PCLCategory = value;
                NotifyOfPropertyChange(() => ObjPatientPCLRequest_ByPtRegIDV_PCLCategory);
            }
        }

        public ObservableCollection<PCLExamType> ObjPCLExamTypes_ByPatientPCLReqID
        {
            get { return _ObjPCLExamTypes_ByPatientPCLReqID; }
            set
            {
                _ObjPCLExamTypes_ByPatientPCLReqID = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypes_ByPatientPCLReqID);
            }
        }

        public ObservableCollection<PCLResultFileStorageDetail> ObjGetPCLResultFileStoreDetails
        {
            get { return _ObjGetPCLResultFileStoreDetails; }
            set
            {
                _ObjGetPCLResultFileStoreDetails = value;
                NotifyOfPropertyChange(() => ObjGetPCLResultFileStoreDetails);
            }
        }

        public string FolderListTextSelect
        {
            get { return _FolderListTextSelect; }
            set
            {
                if (_FolderListTextSelect != value)
                {
                    _FolderListTextSelect = value;
                    NotifyOfPropertyChange(() => FolderListTextSelect);
                }
            }
        }

        public ObservableCollection<String> FolderList
        {
            get { return _Folders; }
            set
            {
                if (_Folders != value)
                {
                    _Folders = value;
                    NotifyOfPropertyChange(() => FolderList);
                }
            }
        }

        public int OptionValue
        {
            get { return _OptionValue; }
            set
            {
                if (_OptionValue != value)
                {
                    _OptionValue = value;
                    NotifyOfPropertyChange(() => OptionValue);
                }
            }
        }

        public string cboListFolderSelectedValue
        {
            get { return _cboListFolderSelectedValue; }
            set
            {
                if (_cboListFolderSelectedValue != value)
                {
                    _cboListFolderSelectedValue = value;
                    NotifyOfPropertyChange(() => cboListFolderSelectedValue);
                }
            }
        }

        public long V_ResultType
        {
            get { return _V_ResultType; }
            set
            {
                if (_V_ResultType != value)
                {
                    _V_ResultType = value;
                    NotifyOfPropertyChange(() => V_ResultType);
                }
            }
        }

        public PCLResultFileStorageDetail PCLResultFileStorageDetailSelected
        {
            get { return _PCLResultFileStorageDetailSelected; }
            set
            {
                if (_PCLResultFileStorageDetailSelected != value)
                {
                    _PCLResultFileStorageDetailSelected = value;
                    NotifyOfPropertyChange(() => PCLResultFileStorageDetailSelected);
                }
            }
        }



        public int GetType(object item)
        {
            int type = 0;
            if (item != null)
            {
                PCLResultFileStorageDetail p = item as PCLResultFileStorageDetail;
                if (p.V_ResultType == (long)AllLookupValues.FileStorageResultType.IMAGES)
                {
                    type = 1;
                }
                else if (p.V_ResultType == (long)AllLookupValues.FileStorageResultType.VIDEO_RECORDING)
                {
                    type = 2;
                }
            }
            return type;
        }

        private bool _IsEdit = true;
        public bool IsEdit
        {
            get { return _IsEdit; }
            set
            {
                if (_IsEdit != value)
                {
                    _IsEdit = value;
                    NotifyOfPropertyChange(() => IsEdit);
                }
            }
        }
        //==== 20161010 CMN Begin: Change Image View to WriteableBitmap
        //private BitmapImage _ObjBitmapImage;
        //public BitmapImage ObjBitmapImage
        private WriteableBitmap _ObjBitmapImage;
        public WriteableBitmap ObjBitmapImage
        //==== 20161010 CMN End.
        {
            get { return _ObjBitmapImage; }
            set
            {
                if (_ObjBitmapImage != value)
                {
                    _ObjBitmapImage = value;
                    NotifyOfPropertyChange(() => ObjBitmapImage);
                }
            }
        }

        private Stream _VideoAndImageStream;
        public Stream VideoAndImageStream
        {
            get { return _VideoAndImageStream; }
            set
            {
                _VideoAndImageStream = value;
                NotifyOfPropertyChange(() => VideoAndImageStream);
            }
        }

        private Visibility _ControlVideoVisibility;
        public Visibility ControlVideoVisibility
        {
            get { return _ControlVideoVisibility; }
            set
            {
                if (_ControlVideoVisibility != value)
                {
                    _ControlVideoVisibility = value;
                    NotifyOfPropertyChange(() => ControlVideoVisibility);
                }
            }
        }

        private Visibility _ControlImgVisibility;
        public Visibility ControlImgVisibility
        {
            get { return _ControlImgVisibility; }
            set
            {
                if (_ControlImgVisibility != value)
                {
                    _ControlImgVisibility = value;
                    NotifyOfPropertyChange(() => ControlImgVisibility);
                }
            }
        }

        private void SetVisibleForVideoControls(Visibility pV)
        {
            ControlVideoVisibility = pV;
        }
        private void SetVisibleForImgControls(System.Windows.Visibility pV)
        {
            ControlImgVisibility = pV;
        }

        private void DisplayResultFile(Stream pStream, int itype)
        {
            VideoAndImageStream = pStream;

            if (pStream != null)
            {
                if (itype == 1)/*Images*/
                {
                    //==== 20161010 CMN Begin: Change Image View to WriteableBitmap
                    //System.Windows.Media.Imaging.BitmapImage imgsource = new System.Windows.Media.Imaging.BitmapImage();
                    //imgsource.SetSource(pStream);
                    //ObjBitmapImage = imgsource;
                    ObjBitmapImage = Globals.GetWriteableBitmapFromStream(pStream);
                    //==== 20161010 CMN End.
                    SetVisibleForVideoControls(Visibility.Collapsed);
                    SetVisibleForImgControls(Visibility.Visible);
                }
                else if (itype == 2)/*Video*/
                {
                    SetVisibleForVideoControls(Visibility.Visible);
                    SetVisibleForImgControls(Visibility.Collapsed);

                    if (_currentView != null)
                    {
                        _currentView.SetObjectSource(pStream);
                    }

                }
                else/*Document*/
                {
                    SetVisibleForVideoControls(Visibility.Collapsed);
                    SetVisibleForImgControls(Visibility.Collapsed);
                }
            }
            else
            {
                SetVisibleForVideoControls(Visibility.Collapsed);
                SetVisibleForImgControls(Visibility.Collapsed);
            }
        }

        private PatientPCLRequest copyPatientPCLRequest(PatientPCLRequest_Ext p)
        {
            PatientPCLRequest temp = new PatientPCLRequest();
            temp.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();
            temp.DeptID = p.DeptID;
            temp.PtRegistrationID = p.PtRegistrationID;

            temp.PCLRequestNumID = p.PCLRequestNumID;
            temp.Diagnosis = p.Diagnosis;

            temp.PatientPCLReqID = p.PatientPCLReqExtID;
            temp.PCLExamTypeID = p.PCLExamTypeID == null ? 0 : p.PCLExamTypeID.Value;
            temp.PCLExamTypeName = p.PCLExamTypeName;

            temp.StaffIDName = p.FullName;
            temp.StaffID = p.StaffID;
            temp.DoctorStaffID = p.DoctorStaffID;

            temp.ReqFromDeptLocID = p.ReqFromDeptLocID;

            temp.DoctorComments = p.DoctorComments;
            temp.IsCaseOfEmergency = p.IsCaseOfEmergency;

            temp.IsExternalExam = p.IsExternalExam;
            temp.IsImported = p.IsImported;

            temp.PatientServiceRecord = new PatientServiceRecord();
            temp.PatientServiceRecord.ServiceRecID = p.PatientServiceRecord.ServiceRecID;

            temp.PatientServiceRecord.StaffID = p.PatientServiceRecord.StaffID;
            temp.V_PCLRequestStatus = p.V_PCLRequestStatus;
            return temp;
        }

        private void GetPCLRequestExtPK(long PatientPCLReqExtID)
        {
            Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = eHCMSResources.K3035_G1_DSPh });

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetPCLRequestExtPK(PatientPCLReqExtID, Globals.DispatchCallback((asyncResult) =>
                        {
                            curPatientPCLRequest_Ext = client.EndGetPCLRequestExtPK(asyncResult);
                            LoadDataCoroutineEx(copyPatientPCLRequest(curPatientPCLRequest_Ext));
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }

        private void GetPatientPCLImagingResults_ByID(long ptPCLReqID, long PCLExamTypeID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format("{0}...", eHCMSResources.Z0528_G1_DSKQua) });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsImportClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetPatientPCLImagingResults_ByID(ptPCLReqID, PCLExamTypeID,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var item = contract.EndGetPatientPCLImagingResults_ByID(asyncResult);
                                if (item != null && item.PatientPCLReqID.GetValueOrDefault() > 0)
                                {

                                    ObjPatientPCLImagingResult.DiagnoseOnPCLExam = item.DiagnoseOnPCLExam;
                                    ObjPatientPCLImagingResult.ResultExplanation = item.ResultExplanation;
                                    ObjPatientPCLImagingResult.TemplateResultDescription = item.TemplateResultDescription;
                                    ObjPatientPCLImagingResult.TemplateResult = item.TemplateResult;
                                }
                                else
                                {
                                    if (ObjPatientPCLImagingResult != null)
                                    {
                                        ObjPatientPCLImagingResult.DiagnoseOnPCLExam = "";
                                        ObjPatientPCLImagingResult.ResultExplanation = "";
                                        ObjPatientPCLImagingResult.TemplateResultDescription = "";
                                        ObjPatientPCLImagingResult.TemplateResult = "";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                Globals.IsBusy = false;
                            }
                        }), null);
                }
            });
            t.Start();
        }

        private void GetPCLResultFileStoreDetails(long ptID, long? ptPCLReqID, long? PCLGroupID, long? PCLExamTypeID)
        {
            //  Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = string.Format("{0}...", eHCMSResources.Z0528_G1_DSKQua)});
            isLoadingDetail = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;

                    contract.BeginGetPCLResultFileStoreDetails(ptID, ptPCLReqID, PCLGroupID, PCLExamTypeID,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                IList<PCLResultFileStorageDetail> items = contract.EndGetPCLResultFileStoreDetails(asyncResult);
                                if (items != null)
                                {
                                    ObjGetPCLResultFileStoreDetails = new ObservableCollection<PCLResultFileStorageDetail>(items);

                                }
                                else
                                {
                                    ObjGetPCLResultFileStoreDetails = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                isLoadingDetail = false;
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                isLoadingDetail = false;
                                // Globals.IsBusy =false;
                            }
                        }), null);
                }
            });
            t.Start();
        }

        private void GetAppConfigValue()
        {
            //ImagePool = Globals.ConfigList[(int)ConfigItemKey.PCLResourcePool];
            //ImageStore = Globals.ConfigList[(int)ConfigItemKey.PCLStorePool];
            //ImageThumbTemp = Globals.ConfigList[(int)ConfigItemKey.PCLThumbTemp];

            // Txd 25/05/2014 Replaced ConfigList
            ImagePool = Globals.ServerConfigSection.Hospitals.PCLResourcePool;
            ImageStore = Globals.ServerConfigSection.Hospitals.PCLStorePool;
            ImageThumbTemp = Globals.ServerConfigSection.Hospitals.PCLThumbTemp;

            GetFolderList(ImagePool);
        }

        private void GetFolderList(string Path)
        {
            // Globals.EventAggregator.Publish(new BusyEvent {IsBusy = true, Message = "FolderList..."});
            isLoadingConfig = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsImportClient())
                {
                    IPCLsImport contract = serviceFactory.ServiceInstance;

                    contract.BeginGetFolderList(Path, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            IList<string> items = contract.EndGetFolderList(asyncResult);
                            if (items != null)
                            {
                                FolderList = new ObservableCollection<string>(items);
                                //Item Default
                                string ItemDefault = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon);
                                FolderList.Insert(0, ItemDefault);
                                //Item Default
                            }
                            else
                            {
                                FolderList = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            isLoadingConfig = false;
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingConfig = false;
                            // Globals.IsBusy=false;
                        }
                    }), null);
                }
            });
            t.Start();
        }

        #region Thông tin bệnh viện ngoài

        private ObservableCollection<TestingAgency> _ObjTestingAgencyList;

        public ObservableCollection<TestingAgency> ObjTestingAgencyList
        {
            get { return _ObjTestingAgencyList; }
            set
            {
                if (_ObjTestingAgencyList != value)
                {
                    _ObjTestingAgencyList = value;
                    NotifyOfPropertyChange(() => ObjTestingAgencyList);
                }
            }
        }

        private void GetTestingAgency_All()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format("{0}...", eHCMSResources.Z0535_G1_DSBVNgoai) });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsImportClient())
                {
                    IPCLsImport contract = serviceFactory.ServiceInstance;

                    contract.BeginGetTestingAgency_All(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            IList
                                <
                                    TestingAgency
                                    >
                                items
                                    =
                                    contract
                                        .
                                        EndGetTestingAgency_All
                                        (asyncResult);
                            if (
                                items !=
                                null)
                            {
                                ObjTestingAgencyList
                                    =
                                    new ObservableCollection
                                        <
                                            TestingAgency
                                            >
                                        (items);

                                //Item Default
                                var
                                    ItemDefault
                                        =
                                        new TestingAgency
                                            ();
                                ItemDefault
                                    .
                                    AgencyID
                                    =
                                    -1;
                                ItemDefault
                                    .
                                    AgencyName
                                    =
                                    string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon);
                                ObjTestingAgencyList
                                    .
                                    Insert
                                    (0,
                                    ItemDefault);
                                //Item Default
                            }
                            else
                            {
                                ObjTestingAgencyList
                                    =
                                    null;
                            }
                        }
                        catch (
                            Exception
                                ex
                            )
                        {
                            Globals.ShowMessage
                                (ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals
                                .
                                IsBusy
                                =
                                false;
                        }
                    }), null);
                }
            });
            t.Start();
        }

        #endregion

        #region InitObject

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


        private void CheckFormStartValid()
        {
            //▼===== 20190927 TTM:  Vì hàm này được gọi trước khi giá trị của Registration_DataStorage đc set => null exception. Nên cần kiểm tra lại
            //                      Trước đây là xài Globals.PatientAllDetails nên kiểm tra Globals.PatientAllDetails.PatientInfo != null đã bao gồm điều kiện vừa thêm vào 
            //                      => Không lỗi.
            if (Registration_DataStorage == null)
            {
                return;
            }
            //▲===== 
            if (Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                HasPatient = true;
            }
            else
            {
                HasPatient = false;
            }
        }

        private void KhoiTao()
        {
            ObjPatientPCLImagingResult = new PatientPCLImagingResult();
            ObjPatientPCLImagingResult.StaffID = Globals.LoggedUserAccount.Staff != null ? Globals.LoggedUserAccount.Staff.StaffID : (Globals.LoggedUserAccount.StaffID.HasValue ? Globals.LoggedUserAccount.StaffID.Value : -1);
            ObjPatientPCLImagingResult.PatientPCLReqID = -1;
            ObjPatientPCLImagingResult.PCLExamTypeID = -1;

            ObjPatientPCLImagingResult.PCLExamDate = Globals.ServerDate.GetValueOrDefault(DateTime.Now);
            ObjPatientPCLImagingResult.PCLExamForOutPatient = true;
            ObjPatientPCLImagingResult.IsExternalExam = false;
            OptionValue = 1;

            PCLResultFileStorageDetailSelected = new PCLResultFileStorageDetail();
            cboListFolderSelectedValue = "Images";
            V_ResultType = (long)AllLookupValues.FileStorageResultType.IMAGES;
        }

        public void Reset()
        {
            ObjPatientPCLImagingResult = new PatientPCLImagingResult();
            ObjGetPCLResultFileStoreDetails = new ObservableCollection<PCLResultFileStorageDetail>();
            ObjPatientPCLImagingResult = new PatientPCLImagingResult();
        }


        #endregion


        public void LoadData(PatientPCLRequest message)
        {
            if (ObjPatientPCLImagingResult == null)
            {
                KhoiTao();
            }

            ObjPatientPCLImagingResult.PatientPCLRequest = message;
            ObjPatientPCLImagingResult.PatientPCLReqID = message.PatientPCLReqID;
            ObjPatientPCLImagingResult.PCLExamTypeID = message.PCLExamTypeID.GetValueOrDefault();
            ObjPatientPCLImagingResult.PCLExamType = new PCLExamType
            {
                PCLExamTypeID = message.PCLExamTypeID.GetValueOrDefault(),
                PCLExamTypeName = message.PCLExamTypeName
            };

            GetPCLResultFileStoreDetails(message.PatientID,
                                          ObjPatientPCLImagingResult.PatientPCLReqID, null,
                                          ObjPatientPCLImagingResult.PCLExamTypeID);

            GetPatientPCLImagingResults_ByID(ObjPatientPCLImagingResult.PatientPCLReqID.GetValueOrDefault(), ObjPatientPCLImagingResult.PCLExamTypeID);
        }


        public void LoadDataFromPK(long PatientPCLReqExtID)
        {
            GetPCLRequestExtPK(PatientPCLReqExtID);
        }

        public void LoadDataCoroutineEx(PatientPCLRequest message)
        {
            Coroutine.BeginExecute(LoadDataCoroutine(message));
        }

        private IEnumerator<IResult> LoadDataCoroutine(PatientPCLRequest p)
        {
            isLoadingDetail = true;

            if (ObjPatientPCLImagingResult == null)
            {
                KhoiTao();
            }
            ObjPatientPCLImagingResult.PatientPCLRequest = p;
            ObjPatientPCLImagingResult.IsExternalExam = p.IsExternalExam;
            ObjPatientPCLImagingResult.PCLExamType = new PCLExamType
            {
                PCLExamTypeID = p.PCLExamTypeID.Value,
                PCLExamTypeName = p.PCLExamTypeName
            };
            var pclresSearch = new PCLResultFileStorageDetailSearchCriteria
            {
                PatientPCLReqID = p.PatientPCLReqID,
                PCLExamTypeID = p.PCLExamTypeID.Value,
                IsExternalExam = p.IsExternalExam,
                V_PCLRequestType = (long)p.V_PCLRequestType
            };
            var ResultFileStoreDetails = new LoadPCLResultFileStoreDetailsExtTask(pclresSearch);
            yield return ResultFileStoreDetails;
            ObjGetPCLResultFileStoreDetails = ResultFileStoreDetails.ObjGetPCLResultFileStoreDetails.ToObservableCollection();

            var LoadPCLImagingResultsID = new LoadPatientPCLImagingResults_ByIDExtTask(pclresSearch);
            yield return LoadPCLImagingResultsID;
            if (LoadPCLImagingResultsID.ObjPatientPCLImagingResult != null && LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PatientPCLReqID.GetValueOrDefault() > 0)
            {
                ObjPatientPCLImagingResult.DiagnoseOnPCLExam = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.DiagnoseOnPCLExam;
                ObjPatientPCLImagingResult.ResultExplanation = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.ResultExplanation;
                ObjPatientPCLImagingResult.TestingAgency = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.TestingAgency;
                ObjPatientPCLImagingResult.TemplateResultDescription = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.TemplateResultDescription;
                ObjPatientPCLImagingResult.TemplateResult = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.TemplateResult;

                ObjPatientPCLImagingResult.HIRepResourceCode = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.HIRepResourceCode;
                ObjPatientPCLImagingResult.TemplateResultString = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.TemplateResultString;
                ObjPatientPCLImagingResult.NumberOfFilmsReceived = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.NumberOfFilmsReceived;
                ObjPatientPCLImagingResult.PtRegistrationCode = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PtRegistrationCode;
                ObjPatientPCLImagingResult.Staff = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.Staff;
                ObjPatientPCLImagingResult.PerformedDate = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PerformedDate;
                ObjPatientPCLImagingResult.PerformStaffID = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PerformStaffID;
                ObjPatientPCLImagingResult.PerformStaffFullName = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PerformStaffFullName;
                ObjPatientPCLImagingResult.PatientPCLRequest.ResultDate = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PCLExamDate;
                ObjPatientPCLImagingResult.Suggest = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.Suggest;
                ObjPatientPCLImagingResult.ResultStaffID = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.ResultStaffID;
                ObjPatientPCLImagingResult.ResultStaffFullName = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.ResultStaffFullName;
                ObjPatientPCLImagingResult.PCLImgResultID = LoadPCLImagingResultsID.ObjPatientPCLImagingResult.PCLImgResultID;
            }
            else
            {
                if (ObjPatientPCLImagingResult != null)
                {
                    ObjPatientPCLImagingResult.DiagnoseOnPCLExam = "";
                    ObjPatientPCLImagingResult.ResultExplanation = "";
                    ObjPatientPCLImagingResult.TemplateResultDescription = "";
                    ObjPatientPCLImagingResult.TemplateResult = "";

                    ObjPatientPCLImagingResult.NumberOfFilmsReceived = ObjPatientPCLImagingResult.PatientPCLRequest.DefaultNumFilmsRecv;
                    ObjPatientPCLImagingResult.TemplateResultString = null;
                    ObjPatientPCLImagingResult.PerformStaffID = null;
                    ObjPatientPCLImagingResult.PerformStaffFullName = null;
                    ObjPatientPCLImagingResult.Suggest = null;
                    ObjPatientPCLImagingResult.ResultStaffID = null;
                    ObjPatientPCLImagingResult.ResultStaffFullName = null;
                }
            }
            Globals.EventAggregator.Publish(new ItemSelected<PatientPCLImagingResult, ObservableCollection<PCLResultFileStorageDetail>> { Sender = ObjPatientPCLImagingResult, Item = ObjGetPCLResultFileStoreDetails });
            isLoadingDetail = false;
            yield break;

        }

        private PCLResultFileStorageDetail _ObjPCLResultFileStorageDetailSelected;
        public PCLResultFileStorageDetail ObjPCLResultFileStorageDetailSelected
        {
            get
            {
                return _ObjPCLResultFileStorageDetailSelected;
            }
            set
            {
                if (_ObjPCLResultFileStorageDetailSelected != value)
                {
                    _ObjPCLResultFileStorageDetailSelected = value;
                    NotifyOfPropertyChange(() => ObjPCLResultFileStorageDetailSelected);
                }
            }
        }

        public void lnkView_Click(object sender, RoutedEventArgs e)
        {
            if (ObjPCLResultFileStorageDetailSelected != null)
            {
                int iType = GetType(ObjPCLResultFileStorageDetailSelected);
                //==== 20161011 CMN Begin: Add file return to selected image for load
                //if (ObjPCLResultFileStorageDetailSelected.PCLResultFileItemID > 0)
                //{
                //    //20161006 CMN Begin: Change for new SaveFile Method
                //    //ObjPCLResultFileStorageDetailSelected.FullPath = ImageStore + @"\" + ObjPCLResultFileStorageDetailSelected.PCLResultLocation + @"\" + ObjPCLResultFileStorageDetailSelected.PCLResultFileName;
                //    ObjPCLResultFileStorageDetailSelected.FullPath = ObjPCLResultFileStorageDetailSelected.PCLResultLocation + @"\" + ObjPCLResultFileStorageDetailSelected.PCLResultFileName;
                //    //20161006 CMN End
                //}
                //GetVideoAndImage(ObjPCLResultFileStorageDetailSelected.FullPath, iType);

                if (ObjPCLResultFileStorageDetailSelected.PCLResultFileItemID > 0 && ObjPCLResultFileStorageDetailSelected.File == null)
                    GetVideoAndImage(ObjPCLResultFileStorageDetailSelected.FullPath, iType);
                else
                {
                    ObjGetVideoAndImage = new MemoryStream(ObjPCLResultFileStorageDetailSelected.File);
                    DisplayResultFile(ObjGetVideoAndImage, iType);
                }
                //==== 20161011 CMN End.
            }
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ObjPCLResultFileStorageDetailSelected != null)
            {
                ObjGetPCLResultFileStoreDetails.Remove(ObjPCLResultFileStorageDetailSelected);
            }
        }

        #region Control media
        public void btPlay()
        {
            if (_currentView != null)
            {
                _currentView.btPlay();
            }
        }
        public void btPause()
        {
            if (_currentView != null)
            {
                _currentView.btPause();
            }
        }

        public void btStop()
        {
            if (_currentView != null)
            {
                _currentView.btStop();
            }
        }

        public void btMute(object sender, RoutedEventArgs e)
        {
            if (_currentView != null)
            {
                _currentView.btMute();
            }
        }

        public void btVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_currentView != null)
            {
                _currentView.btVolume_ValueChanged(sender, e);
            }
        }
        #endregion

        private Stream _ObjGetVideoAndImage;
        public Stream ObjGetVideoAndImage
        {
            get { return _ObjGetVideoAndImage; }
            set
            {
                if (_ObjGetVideoAndImage != value)
                {
                    _ObjGetVideoAndImage = value;
                    NotifyOfPropertyChange(() => ObjGetVideoAndImage);
                }
            }
        }

        private void GetVideoAndImage(string path, int itype)
        {
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Tải File..." });
            //isLoadingOperator = true;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetVideoAndImage(path, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetVideoAndImage(asyncResult);
                            if (items != null)
                            {
                                //==== 20161011 CMN Begin: Add file return to selected image for load
                                ObjPCLResultFileStorageDetailSelected.File = items;
                                //==== 20161011 CMN End.
                                ObjGetVideoAndImage = new MemoryStream(items);
                                DisplayResultFile(ObjGetVideoAndImage, itype);
                            }
                            else
                            {
                                ObjGetVideoAndImage = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            //isLoadingOperator = false;
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingOperator = false;
                            this.HideBusyIndicator();
                            //Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        public void hplDiagnoticsVideo()
        {
            if (VideoAndImageStream != null)
            {
                Action<IVideoDiagnosticExplorer> onInitDlg = delegate (IVideoDiagnosticExplorer typeInfo)
                {
                    typeInfo.VideoStream = VideoAndImageStream;
                };
                GlobalsNAV.ShowDialog<IVideoDiagnosticExplorer>(onInitDlg);
            }
        }

        public void hplDiagnoticsImg()
        {
            if (ObjBitmapImage != null)
            {
                Action<IImageDisgnosticExplorer> onInitDlg = delegate (IImageDisgnosticExplorer typeInfo)
                {
                    //==== 20161010 CMN Begin: Change Image View to WriteableBitmap
                    //typeInfo.ObjBitmapImage = ObjBitmapImage;
                    typeInfo.TypeOfBitmapImage = 1;
                    typeInfo.ObjWBitmapImage = ObjBitmapImage;
                    //==== 20161010 CMN End.
                };
                GlobalsNAV.ShowDialog<IImageDisgnosticExplorer>(onInitDlg);
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
            }
        }
    }
}