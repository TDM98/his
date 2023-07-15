using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using aEMR.Infrastructure.Events;
using DataEntities;
using aEMR.Infrastructure;
using Service.Core.Common;
using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using System.Collections.Generic;
using eHCMS.CommonUserControls.CommonTasks;
using eHCMSLanguage;
using aEMR.Common.PagedCollectionView;
using Castle.Windsor;

namespace aEMR.ConsultantEPrescription.PCL_ViewResults.ViewModels
{
    [Export(typeof(IPatientPCLRequestEditImageExt)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PatientPCLRequestEditImageExtViewModel : ViewModelBase, IPatientPCLRequestEditImageExt
        , IHandle<ShowPatientInfo_KHAMBENH_CLS_NGOAIVIEN_HINHANH<Patient, PatientRegistration, PatientRegistrationDetail>>
        , IHandle<DbClickPtPCLReqExtEdit<PatientPCLRequest_Ext, string>>
    {

        #region property gen su dung cho busy indicator
        public override bool IsProcessing
        {
            get
            {
                return _isLoadingMainCategory
                    || _isLoadingPCLForms
                    || _isWaitingLoadPCLExamType
                    || _isWaitingSave
                    || _isWaitingLoadPCLInfo
                    || _isWaitingLoadKhoa
                    || _isWaitingLoadPhong
                    || _isWaitingLoadDetailPCLRequest
                    || _isWaitingDeletePCLRequest
                    || _isWaitingLoadChanDoan
                    || _isWaitingLoadRegistration;
            }
        }
        public override string StatusText
        {
            get
            {
                if (_isLoadingMainCategory)
                {
                    return eHCMSResources.Z0340_G1_LoadDSLoaiPCL;
                }
                if (_isLoadingPCLForms)
                {
                    return eHCMSResources.Z0341_G1_LoadDSPCLForm;
                }
                if (_isWaitingLoadPCLExamType)
                {
                    return eHCMSResources.Z0483_G1_LoadDSXetNghiem;
                }
                if (_isWaitingSave)
                {
                    return eHCMSResources.Z0343_G1_DangLuu;
                }
                if (_isWaitingLoadPCLInfo)
                {
                    return eHCMSResources.Z0344_G1_LoadTTinPh;
                }
                if (_isWaitingLoadKhoa)
                {
                    return eHCMSResources.Z0345_G1_LoadKhoa;
                }
                if (_isWaitingLoadPhong)
                {
                    return eHCMSResources.Z0346_G1_LoadPhg;
                }
                if (_isWaitingLoadDetailPCLRequest)
                {
                    return eHCMSResources.Z0347_G1_LoadCTietPhieu;
                }
                if (_isWaitingDeletePCLRequest)
                {
                    return eHCMSResources.Z0348_G1_DangXoaPhieu;
                }
                if (_isWaitingLoadChanDoan)
                {
                    return eHCMSResources.Z0484_G1_LoadDSChanDoan;
                }
                if (_isWaitingLoadRegistration)
                {
                    return eHCMSResources.Z0349_G1_LayTTinDKCuaPh;
                }
                return string.Empty;
            }
        }
        private bool _isLoadingMainCategory;
        public bool IsLoadingMainCategory
        {
            get { return _isLoadingMainCategory; }
            set
            {
                if (_isLoadingMainCategory != value)
                {
                    _isLoadingMainCategory = value;
                    NotifyOfPropertyChange(() => IsLoadingMainCategory);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _isLoadingPCLForms;
        public bool IsLoadingPCLForms
        {
            get { return _isLoadingPCLForms; }
            set
            {
                if (_isLoadingPCLForms != value)
                {
                    _isLoadingPCLForms = value;
                    NotifyOfPropertyChange(() => IsLoadingPCLForms);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _isWaitingLoadPCLExamType;
        public bool IsWaitingLoadPCLExamType
        {
            get { return _isWaitingLoadPCLExamType; }
            set
            {
                if (_isWaitingLoadPCLExamType != value)
                {
                    _isWaitingLoadPCLExamType = value;
                    NotifyOfPropertyChange(() => IsWaitingLoadPCLExamType);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _isWaitingSave;
        public bool IsWaitingSave
        {
            get { return _isWaitingSave; }
            set
            {
                if (_isWaitingSave != value)
                {
                    _isWaitingSave = value;
                    NotifyOfPropertyChange(() => IsWaitingSave);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _isWaitingLoadPCLInfo;
        public bool IsWaitingLoadPCLInfo
        {
            get { return _isWaitingLoadPCLInfo; }
            set
            {
                if (_isWaitingLoadPCLInfo != value)
                {
                    _isWaitingLoadPCLInfo = value;
                    NotifyOfPropertyChange(() => IsWaitingLoadPCLInfo);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _isWaitingLoadKhoa;
        public bool IsWaitingLoadKhoa
        {
            get { return _isWaitingLoadKhoa; }
            set
            {
                if (_isWaitingLoadKhoa != value)
                {
                    _isWaitingLoadKhoa = value;
                    NotifyOfPropertyChange(() => IsWaitingLoadKhoa);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _isWaitingLoadPhong;
        public bool IsWaitingLoadPhong
        {
            get { return _isWaitingLoadPhong; }
            set
            {
                if (_isWaitingLoadPhong != value)
                {
                    _isWaitingLoadPhong = value;
                    NotifyOfPropertyChange(() => IsWaitingLoadPhong);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _isWaitingLoadDetailPCLRequest;
        public bool IsWaitingLoadDetailPCLRequest
        {
            get { return _isWaitingLoadDetailPCLRequest; }
            set
            {
                if (_isWaitingLoadDetailPCLRequest != value)
                {
                    _isWaitingLoadDetailPCLRequest = value;
                    NotifyOfPropertyChange(() => IsWaitingLoadDetailPCLRequest);
                    NotifyWhenBusy();
                }
            }
        }



        private bool _isWaitingDeletePCLRequest;
        public bool IsWaitingDeletePCLRequest
        {
            get { return _isWaitingDeletePCLRequest; }
            set
            {
                if (_isWaitingDeletePCLRequest != value)
                {
                    _isWaitingDeletePCLRequest = value;
                    NotifyOfPropertyChange(() => IsWaitingDeletePCLRequest);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _isWaitingLoadRegistration;
        public bool IsWaitingLoadRegistration
        {
            get { return _isWaitingLoadRegistration; }
            set
            {
                if (_isWaitingLoadRegistration != value)
                {
                    _isWaitingLoadRegistration = value;
                    NotifyOfPropertyChange(() => IsWaitingLoadRegistration);
                    NotifyWhenBusy();
                }
            }
        }

        #endregion

        private long IDLABByName = 28888;
        /// <summary> bien xac dinh la hen benh hay can lam sang
        /// =true la hen benh

        private long? V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Imaging;
        [ImportingConstructor]
        public PatientPCLRequestEditImageExtViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);
            authorization();
            FindPatient = Globals.PatientFindBy_ForConsultation == null ?
                0 : (int)Globals.PatientFindBy_ForConsultation.Value;

            allPatientPCLRequestDetail_Ext = new EntitiesEdit<PatientPCLRequestDetail_Ext>(true);
            btNewIsEnabled = true;
            InitLoadData();
            //▼===== 20191017 TTM:  Hàm này không có gì thay đổi nên không cần phải để ở InitLoadData. Vì để đó mỗi lần chuyển bệnh nhân sẽ lại đi
            //                      query => không cần thiết
            ObjTestingAgencyList = new ObservableCollection<TestingAgency>();
            GetTestingAgency_All();
            //▲===== 20191017
        }

        public void InitPatientInfo(Patient CurrentPatient)
        {
            if (CurrentPatient != null)/*Làm CLS chỉ cần kiểm tra BN !=null*/
            {
                InitControlsForExt();

                FormIsEnabled = true;

                //KMx: Sau khi kiểm tra, thấy View này không sử dụng View IPatientMedicalRecords_ByPatientID (25/05/2014 14:48).
                //UC Header PMR
                //var uc3 = Globals.GetViewModel<IPatientMedicalRecords_ByPatientID>();
                //ActivateItem(uc3);
                //UC Header PMR

                InitPCLRequest();




                if (!Globals.isConsultationStateEdit)
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0391_G1_BNDuocChonTuLSBA));
                    FormIsEnabled = false;
                    return;
                }

                ObjGetDiagnosisTreatmentByPtID = new DiagnosisTreatment();
                if (Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID > 0)
                {
                    GetDiagnosisTreatmentByPtID(Registration_DataStorage.CurrentPatient.PatientID, Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID, "", 1, true, (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType);
                }
                else
                {
                    MessageBox.Show(string.Format("{0} (PtRegistrationID): ", eHCMSResources.A0747_G1_Msg_InfoKhTimThayMaDK) + Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID.ToString(), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
            }
            else
            {
                //FormIsEnabled = false;
            }
        }

        private void InitPCLRequest()
        {
            ObjPatientPCLRequestNew = NewPCLReq();
            ObjPatientPCLRequestNew.AgencyID = -1;
        }

        private void InitLoadData()
        {
            LoadV_PCLMainCategory();

            //▼===== 20191017 TTM: Chuyển TestingAgency về hàm khởi tạo của view.
            //ObjTestingAgencyList = new ObservableCollection<TestingAgency>();
            //GetTestingAgency_All();
            //▲===== 20191017

            ObjPCLForms_GetList = new ObservableCollection<PCLForm>();
            ObjPCLForms_GetList_Selected = new PCLForm();


            SearchCriteriaDetail = new PatientPCLRequestDetailSearchCriteria();

            //PCLForm
            SearchCriteria = new PCLFormsSearchCriteria();
            SearchCriteria.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Imaging;
            SearchCriteria.OrderBy = "";

            SearchCriteriaExamTypeForChoose = new PCLExamTypeSearchCriteria();

            HasGroup = true;
        }



        /// <summary>Content control cho phieu hen can lam sang
        /// content PCLExamtype
        /// </summary>
        private IEditApptPclRequestDetailList _pclApptRequestDetailsContent;
        public IEditApptPclRequestDetailList pclApptRequestDetailsContent
        {
            get { return _pclApptRequestDetailsContent; }
            set
            {
                _pclApptRequestDetailsContent = value;
                NotifyOfPropertyChange(() => pclApptRequestDetailsContent);
            }
        }

        private bool _FormIsEnabled;
        public bool FormIsEnabled
        {
            get { return _FormIsEnabled; }
            set
            {
                if (_FormIsEnabled != value)
                {
                    _FormIsEnabled = value;
                    NotifyOfPropertyChange(() => FormIsEnabled);
                }
            }
        }

        private bool _FormInputIsEnabled;
        public bool FormInputIsEnabled
        {
            get { return _FormInputIsEnabled; }
            set
            {
                if (_FormInputIsEnabled != value)
                {
                    _FormInputIsEnabled = value;
                    NotifyOfPropertyChange(() => FormInputIsEnabled);
                }
            }
        }



        public enum DataGridCol
        {
            SEL = 0,
            EXAM_CODE = 1,
            EXAM_TYPE_NAME = 2,
            QTY = 3,
            NormalPrice = 4,
            PriceForHIPatient = 5,
            PriceDifference = 4,
        }




        private Staff _ObjStaff;
        public Staff ObjStaff
        {
            get { return _ObjStaff; }
            set
            {
                if (_ObjStaff != value)
                {
                    _ObjStaff = value;
                }
            }
        }


        //Phiếu cuối
        private void PatientPCLRequest_RequestLastest()
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format("{0}...", eHCMSResources.Z0534_G1_PhGanNhat) });

            var t = new Thread(() =>
            {
                IsWaitingLoadPCLInfo = true;

                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPatientPCLRequest_RequestLastest(Registration_DataStorage.CurrentPatient.PatientID, (long)AllLookupValues.RegistrationType.NGOAI_TRU, V_PCLMainCategory, Globals.DispatchCallback((asyncResult) =>
                        {
                            var PCLReqLast = client.EndPatientPCLRequest_RequestLastest(asyncResult);

                            if (PCLReqLast != null && PCLReqLast.PatientPCLReqID > 0)
                            {
                                //ObjPatientPCLRequestNew = PCLReqLast;

                                SearchCriteriaDetail.PatientPCLReqID = ObjPatientPCLRequestNew.PatientPCLReqID;

                                PatientPCLRequestDetail_ByPatientPCLReqID();
                            }

                            ObjPatientPCLRequestNew_BackUp = ObjectCopier.DeepCopy(ObjPatientPCLRequestNew);

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    //Globals.IsBusy = false;
                    IsWaitingLoadPCLInfo = false;
                }
            });
            t.Start();
        }

        private ObservableCollection<DeptLocation> _ObjGetAllLocationsByDeptID;
        public ObservableCollection<DeptLocation> ObjGetAllLocationsByDeptID
        {
            get
            {
                return _ObjGetAllLocationsByDeptID;
            }
            set
            {
                _ObjGetAllLocationsByDeptID = value;
                NotifyOfPropertyChange(() => ObjGetAllLocationsByDeptID);
            }
        }

        private IEnumerator<IResult> GetAllLocationsByDeptID(long deptId)
        {
            var deptLoc = new LoadDeptLoctionByIDTask(deptId);
            yield return deptLoc;
            if (deptLoc.DeptLocations != null)
            {
                ObjGetAllLocationsByDeptID = new ObservableCollection<DeptLocation>(deptLoc.DeptLocations);
            }
            else
            {
                ObjGetAllLocationsByDeptID = new ObservableCollection<DeptLocation>();
            }

            var itemDefault = new DeptLocation();
            itemDefault.Location = new Location();
            itemDefault.Location.LID = -1;
            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2094_G1_ChonPg);
            ObjGetAllLocationsByDeptID.Insert(0, itemDefault);

            yield break;
        }
        //public void GetAllLocationsByDeptID(long? deptId)
        //{
        //    //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.K3054_G1_DSPg });

        //    var t = new Thread(() =>
        //    {
        //        IsWaitingLoadPhong = true;

        //        using (var serviceFactory = new ConfigurationManagerServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            contract.BeginGetAllLocationsByDeptID(deptId, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    var allItems = contract.EndGetAllLocationsByDeptID(asyncResult);

        //                    if (allItems != null)
        //                    {
        //                        ObjGetAllLocationsByDeptID = new ObservableCollection<DeptLocation>(allItems);
        //                    }
        //                    else
        //                    {
        //                        ObjGetAllLocationsByDeptID = new ObservableCollection<DeptLocation>();
        //                    }

        //                    var itemDefault = new DeptLocation();
        //                    itemDefault.Location = new Location();
        //                    itemDefault.Location.LID = -1;
        //                    itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2094_G1_ChonPg);
        //                    ObjGetAllLocationsByDeptID.Insert(0, itemDefault);

        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    //Globals.IsBusy = false;
        //                    IsWaitingLoadPhong = false;
        //                }
        //            }), null);
        //        }


        //    });
        //    t.Start();
        //}

        private PCLFormsSearchCriteria _SearchCriteria = new PCLFormsSearchCriteria();
        public PCLFormsSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);

            }
        }

        private DataEntities.PCLForm _ObjPCLForms_GetList_Selected;
        public DataEntities.PCLForm ObjPCLForms_GetList_Selected
        {
            get { return _ObjPCLForms_GetList_Selected; }
            set
            {
                if (_ObjPCLForms_GetList_Selected != value)
                {
                    _ObjPCLForms_GetList_Selected = value;
                    if (_ObjPCLForms_GetList_Selected.PCLFormID > 0)
                    {
                        PCLItems_ByPCLFormID_HasGroup(HasGroup);
                    }
                    NotifyOfPropertyChange(() => ObjPCLForms_GetList_Selected);
                }
            }
        }

        private ObservableCollection<DataEntities.PCLForm> _ObjPCLForms_GetList;
        public ObservableCollection<DataEntities.PCLForm> ObjPCLForms_GetList
        {
            get { return _ObjPCLForms_GetList; }
            set
            {
                _ObjPCLForms_GetList = value;
                NotifyOfPropertyChange(() => ObjPCLForms_GetList);
            }
        }

        private Visibility _VisibilityTemplate = Visibility.Collapsed;
        public Visibility VisibilityTemplate
        {
            get { return _VisibilityTemplate; }
            set
            {
                _VisibilityTemplate = value;
                NotifyOfPropertyChange(() => VisibilityTemplate);
            }
        }
        private Visibility _VisibilityFrom = Visibility.Visible;
        public Visibility VisibilityFrom
        {
            get { return _VisibilityFrom; }
            set
            {
                _VisibilityFrom = value;
                NotifyOfPropertyChange(() => VisibilityFrom);
            }
        }

        private void SetVisibility()
        {

        }

        private void PCLForm_GetList(int PageIndex, int PageSize, bool CountTotal)
        {
            var t = new Thread(() =>
            {
                IsLoadingPCLForms = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLForms_GetList_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;

                            var allItems = client.EndPCLForms_GetList_Paging(out Total, asyncResult);
                            if (allItems != null)
                            {
                                ObjPCLForms_GetList = new ObservableCollection<DataEntities.PCLForm>(allItems);
                                if (ObjPCLForms_GetList.Count > 1)
                                {
                                    DataEntities.PCLForm ItemDefault = new DataEntities.PCLForm();
                                    ItemDefault.PCLFormID = -1;
                                    if (ObjV_PCLMainCategory_Selected != null)
                                    {
                                        if (ObjV_PCLMainCategory_Selected.LookupID == (long)AllLookupValues.V_PCLMainCategory.Imaging)
                                        {
                                            ItemDefault.PCLFormName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0350_G1_ChonCDoanHATDCN);
                                        }
                                    }
                                    else
                                    {
                                        ItemDefault.PCLFormName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon);
                                    }
                                    ObjPCLForms_GetList.Insert(0, ItemDefault);
                                }

                                //ItemDefault

                                ObjPCLForms_GetList_Selected = ObjPCLForms_GetList.FirstOrDefault();
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    //Globals.IsBusy = false;
                    IsLoadingPCLForms = false;
                }
            });
            t.Start();
        }


        //Main
        private Lookup _ObjV_PCLMainCategory_Selected;
        public Lookup ObjV_PCLMainCategory_Selected
        {
            get { return _ObjV_PCLMainCategory_Selected; }
            set
            {
                _ObjV_PCLMainCategory_Selected = value;
                NotifyOfPropertyChange(() => ObjV_PCLMainCategory_Selected);


                if (_ObjV_PCLMainCategory_Selected != null && _ObjV_PCLMainCategory_Selected.LookupID > 0 && _ObjV_PCLMainCategory_Selected.LookupID != IDLABByName)
                {
                    SearchCriteria.V_PCLMainCategory = _ObjV_PCLMainCategory_Selected.LookupID;

                    ObjPCLItems_ByPCLFormID = null;
                    PCLForm_GetList(0, 99999, true);//ko phân trang

                    SetVisibility();

                }

                NotifyOfPropertyChange(() => cboPCLFormIsEnabled);

                NotifyOfPropertyChange(() => SPTheoFormVisibility);
                NotifyOfPropertyChange(() => SPTheoAutoCompleteVisibility);

            }
        }

        private ObservableCollection<Lookup> _ObjV_PCLMainCategory;
        public ObservableCollection<Lookup> ObjV_PCLMainCategory
        {
            get { return _ObjV_PCLMainCategory; }
            set
            {
                _ObjV_PCLMainCategory = value;
                NotifyOfPropertyChange(() => ObjV_PCLMainCategory);
            }
        }

        private ObservableCollection<PCLExamTypeCombo> _PCLExamTypeCombos;
        public ObservableCollection<PCLExamTypeCombo> PCLExamTypeCombos
        {
            get { return _PCLExamTypeCombos; }
            set
            {
                _PCLExamTypeCombos = value;
                NotifyOfPropertyChange(() => PCLExamTypeCombos);
            }
        }


        public void LoadV_PCLMainCategory()
        {
            List<Lookup> ObjList = new List<Lookup>();

            //Lookup Item0 = new Lookup();
            //Item0.LookupID = -1;/**/
            //Item0.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2034_G1_ChonLoai2);
            //ObjList.Add(Item0);

            Lookup Item1 = new Lookup();
            Item1.LookupID = (long)AllLookupValues.V_PCLMainCategory.Imaging;
            Item1.ObjectValue = eHCMSResources.Z0351_G1_CDoanHAVaTDCN;// "Imaging";
            ObjList.Add(Item1);

            ObjV_PCLMainCategory = new ObservableCollection<Lookup>(ObjList);
            ObjV_PCLMainCategory_Selected = ObjV_PCLMainCategory.FirstOrDefault();
        }
        //Main

        public void cboV_PCLMainCategory_SelectionChanged(object selectItem)
        {
            if (selectItem != null)
            {
                Lookup Objtmp = (selectItem as Lookup);

                if (Objtmp != null)
                {
                    SearchCriteria.V_PCLMainCategory = Objtmp.LookupID;

                    ObjPCLItems_ByPCLFormID = null;
                    PCLForm_GetList(0, 99999, true);//ko phân trang
                }
            }
        }

        public void cboPCLForm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if ((sender as ComboBox).SelectedItem != null)
            //{
            //    PCLForm Objtmp = ((sender as ComboBox).SelectedItem as PCLForm);
            //    if (Objtmp.PCLFormID > 0)
            //    {
            //        PCLItems_ByPCLFormID_HasGroup(HasGroup);
            //    }
            //}
        }

        private PCLExamTypeSearchCriteria _SearchCriteriaExamTypeForChoose;
        public PCLExamTypeSearchCriteria SearchCriteriaExamTypeForChoose
        {
            get
            {
                return _SearchCriteriaExamTypeForChoose;
            }
            set
            {
                _SearchCriteriaExamTypeForChoose = value;
                NotifyOfPropertyChange(() => SearchCriteriaExamTypeForChoose);

            }
        }

        private PagedCollectionView _ObjPCLItems_ByPCLFormID;
        public PagedCollectionView ObjPCLItems_ByPCLFormID
        {
            get { return _ObjPCLItems_ByPCLFormID; }
            set
            {
                _ObjPCLItems_ByPCLFormID = value;
                NotifyOfPropertyChange(() => ObjPCLItems_ByPCLFormID);
            }
        }

        private bool _HasGroup;
        public bool HasGroup
        {
            get { return _HasGroup; }
            set
            {
                if (_HasGroup != value)
                {
                    _HasGroup = value;
                    NotifyOfPropertyChange(() => HasGroup);
                }
            }
        }

        private FormState _formState;
        public FormState formState
        {
            get { return _formState; }
            set
            {
                if (_formState != value)
                {
                    _formState = value;
                    NotifyOfPropertyChange(() => formState);
                }
            }
        }

        private FormWork _formWork;
        public FormWork formWork
        {
            get { return _formWork; }
            set
            {
                if (_formWork != value)
                {
                    _formWork = value;
                    NotifyOfPropertyChange(() => formWork);
                    switch (formWork)
                    {
                        case FormWork.Disable:
                            btNewIsEnabled = true;
                            btSaveIsEnabled = false;
                            FormInputIsEnabled = false;
                            break;
                        case FormWork.Edit:
                            btNewIsEnabled = false;
                            btSaveIsEnabled = true;
                            FormInputIsEnabled = true;
                            break;
                    }

                }
            }
        }

        private void PCLItems_ByPCLFormID_HasGroup(bool isGroup)
        {

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.K3014_G1_DSPCLExamType });
            ObjPCLItems_ByPCLFormID = null;
            var t = new Thread(() =>
            {
                IsWaitingLoadPCLExamType = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLItems_ByPCLFormID(SearchCriteriaExamTypeForChoose, ObjPCLForms_GetList_Selected.PCLFormID, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndPCLItems_ByPCLFormID(asyncResult);

                            if (items != null)
                            {
                                TinhGiaChoBenhNhanBaoHiem(items);

                                ObjPCLItems_ByPCLFormID = new PagedCollectionView(items);

                                if (isGroup)
                                {
                                    ObjPCLItems_ByPCLFormID.GroupDescriptions.Add(new aEMR.Common.PagedCollectionView.PropertyGroupDescription("PCLSectionName"));
                                }
                            }
                            else
                            {
                                ObjPCLItems_ByPCLFormID = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsWaitingLoadPCLExamType = false;
                        }
                    }), null);
                }


            });
            t.Start();

        }

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGrid dtgList = (sender as DataGrid);
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        #region "CalcPatientHI"
        private void TinhGiaChoBenhNhanBaoHiem(List<PCLExamType> ObjList)
        {
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null
                && Registration_DataStorage.CurrentPatientRegistration.HisID.GetValueOrDefault(0) > 0)
            {
                foreach (var pclExamType in ObjList)
                {
                    pclExamType.NormalPrice = pclExamType.HIPatientPrice;
                }
            }
            else
            {
                foreach (var pclExamType in ObjList)
                {
                    pclExamType.HIAllowedPrice = 0;
                }
            }
        }
        #endregion


        //Khởi tạo đối tượng

        private PatientPCLRequest_Ext _ObjPatientPCLRequestNew_BackUp;
        public PatientPCLRequest_Ext ObjPatientPCLRequestNew_BackUp
        {
            get
            {
                return _ObjPatientPCLRequestNew_BackUp;
            }
            set
            {
                if (_ObjPatientPCLRequestNew_BackUp != value)
                {
                    _ObjPatientPCLRequestNew_BackUp = value;
                    NotifyOfPropertyChange(() => ObjPatientPCLRequestNew_BackUp);
                }
            }
        }

        private PatientPCLRequest_Ext _ObjPatientPCLRequestNew;
        public PatientPCLRequest_Ext ObjPatientPCLRequestNew
        {
            get
            {
                return _ObjPatientPCLRequestNew;
            }
            set
            {
                if (_ObjPatientPCLRequestNew != value)
                {
                    _ObjPatientPCLRequestNew = value;
                    NotifyOfPropertyChange(() => ObjPatientPCLRequestNew);
                }
            }
        }



        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }


            mPCL_XemSuaPhieuYeuCau_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtPCLRequest,
                                               (int)oConsultationEx.mPCL_XemSuaPhieuYeuCau_ChinhSua, (int)ePermission.mView);
            mPCL_XemSuaPhieuYeuCau_TaoPhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtPCLRequest,
                                               (int)oConsultationEx.mPCL_XemSuaPhieuYeuCau_TaoPhieuMoi, (int)ePermission.mView);
            mPCL_XemSuaPhieuYeuCau_Huy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtPCLRequest,
                                               (int)oConsultationEx.mPCL_XemSuaPhieuYeuCau_Huy, (int)ePermission.mView);
            mPCL_XemSuaPhieuYeuCau_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtPCLRequest,
                                               (int)oConsultationEx.mPCL_XemSuaPhieuYeuCau_XemIn, (int)ePermission.mView);
            mPCL_XemSuaPhieuYeuCau_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtPCLRequest,
                                               (int)oConsultationEx.mPCL_XemSuaPhieuYeuCau_In, (int)ePermission.mView);
        }
        #region checking account

        private bool _mPCL_XemSuaPhieuYeuCau_ChinhSua = true;
        private bool _mPCL_XemSuaPhieuYeuCau_TaoPhieuMoi = true;
        private bool _mPCL_XemSuaPhieuYeuCau_Huy = true;
        private bool _mPCL_XemSuaPhieuYeuCau_XemIn = true;
        private bool _mPCL_XemSuaPhieuYeuCau_In = true;

        public bool mPCL_XemSuaPhieuYeuCau_ChinhSua
        {
            get
            {
                return _mPCL_XemSuaPhieuYeuCau_ChinhSua;
            }
            set
            {
                if (_mPCL_XemSuaPhieuYeuCau_ChinhSua == value)
                    return;
                _mPCL_XemSuaPhieuYeuCau_ChinhSua = value;
                NotifyOfPropertyChange(() => mPCL_XemSuaPhieuYeuCau_ChinhSua);

            }
        }


        public bool mPCL_XemSuaPhieuYeuCau_TaoPhieuMoi
        {
            get
            {
                return _mPCL_XemSuaPhieuYeuCau_TaoPhieuMoi;
            }
            set
            {
                if (_mPCL_XemSuaPhieuYeuCau_TaoPhieuMoi == value)
                    return;
                _mPCL_XemSuaPhieuYeuCau_TaoPhieuMoi = value;
                NotifyOfPropertyChange(() => mPCL_XemSuaPhieuYeuCau_TaoPhieuMoi);
            }
        }


        public bool mPCL_XemSuaPhieuYeuCau_Huy
        {
            get
            {
                return _mPCL_XemSuaPhieuYeuCau_Huy;
            }
            set
            {
                if (_mPCL_XemSuaPhieuYeuCau_Huy == value)
                    return;
                _mPCL_XemSuaPhieuYeuCau_Huy = value;
                NotifyOfPropertyChange(() => mPCL_XemSuaPhieuYeuCau_Huy);
            }
        }


        public bool mPCL_XemSuaPhieuYeuCau_XemIn
        {
            get
            {
                return _mPCL_XemSuaPhieuYeuCau_XemIn;
            }
            set
            {
                if (_mPCL_XemSuaPhieuYeuCau_XemIn == value)
                    return;
                _mPCL_XemSuaPhieuYeuCau_XemIn = value;
                NotifyOfPropertyChange(() => mPCL_XemSuaPhieuYeuCau_XemIn);
            }
        }


        public bool mPCL_XemSuaPhieuYeuCau_In
        {
            get
            {
                return _mPCL_XemSuaPhieuYeuCau_In;
            }
            set
            {
                if (_mPCL_XemSuaPhieuYeuCau_In == value)
                    return;
                _mPCL_XemSuaPhieuYeuCau_In = value;
                NotifyOfPropertyChange(() => mPCL_XemSuaPhieuYeuCau_In);
            }
        }





        #endregion

        #region 3 Nút Add

        private EntitiesEdit<PatientPCLRequestDetail_Ext> _allPatientPCLRequestDetail_Ext;
        public EntitiesEdit<PatientPCLRequestDetail_Ext> allPatientPCLRequestDetail_Ext
        {
            get { return _allPatientPCLRequestDetail_Ext; }
            set
            {
                _allPatientPCLRequestDetail_Ext = value;
                NotifyOfPropertyChange(() => allPatientPCLRequestDetail_Ext);
            }
        }

        private ObservableCollection<PatientPCLRequestDetail_Ext> _allPatientPCLRequestDetail_ExtBackup;
        public ObservableCollection<PatientPCLRequestDetail_Ext> allPatientPCLRequestDetail_ExtBackup
        {
            get { return _allPatientPCLRequestDetail_ExtBackup; }
            set
            {
                _allPatientPCLRequestDetail_ExtBackup = value;
                NotifyOfPropertyChange(() => allPatientPCLRequestDetail_ExtBackup);
            }
        }

        private PatientPCLRequestDetail_Ext _curPatientPCLRequestDetail_Ext;
        public PatientPCLRequestDetail_Ext curPatientPCLRequestDetail_Ext
        {
            get { return _curPatientPCLRequestDetail_Ext; }
            set
            {
                _curPatientPCLRequestDetail_Ext = value;
                NotifyOfPropertyChange(() => curPatientPCLRequestDetail_Ext);
            }
        }

        private PatientPCLRequestDetailSearchCriteria _SearchCriteriaDetail;
        public PatientPCLRequestDetailSearchCriteria SearchCriteriaDetail
        {
            get
            {
                return _SearchCriteriaDetail;
            }
            set
            {
                if (_SearchCriteriaDetail != value)
                {
                    _SearchCriteriaDetail = value;
                    NotifyOfPropertyChange(() => SearchCriteriaDetail);
                }

            }
        }

        public void PatientPCLRequestDetail_ByPatientPCLReqID()
        {

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0531_G1_DSLoaiXN });

            var t = new Thread(() =>
            {
                IsWaitingLoadDetailPCLRequest = true;

                using (var serviceFactory = new PCLsClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPatientPCLRequestDetail_ByPatientPCLReqID(SearchCriteriaDetail, (long)AllLookupValues.RegistrationType.NGOAI_TRU, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndPatientPCLRequestDetail_ByPatientPCLReqID(asyncResult);

                            if (items != null)
                            {
                                //ObjPatientPCLRequestNew.PatientPCLRequestIndicatorsExt = items.ToObservableCollection();
                                //CurrentPclRequest.PatientPCLRequestIndicatorsExt = items.ToObservableCollection();
                                //PCLRequestDetailsContent.PCLRequest = ObjPatientPCLRequestNew;
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsWaitingLoadDetailPCLRequest = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        public void hplDelete_Click(object selectedItem)
        {
            if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, ""), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {


            }

        }

        #endregion

        #region Các Nút Lưu

        private bool _btNewIsEnabled;
        public bool btNewIsEnabled
        {
            get { return _btNewIsEnabled; }
            set
            {
                if (_btNewIsEnabled != value)
                {
                    _btNewIsEnabled = value;
                    NotifyOfPropertyChange(() => btNewIsEnabled);
                }
            }
        }

        public void btNew()
        {
            FormInputIsEnabled = true;
            allPatientPCLRequestDetail_Ext = new EntitiesEdit<PatientPCLRequestDetail_Ext>(true);
            formState = FormState.New;
            formWork = FormWork.Edit;
            CreateNewPCLReq();
        }

        public void btCancel()
        {
            //if (formState == FormState.Edit)
            {
                //allPatientPCLRequestDetail_Ext.Reset();
                ObjPatientPCLRequestNew = ObjectCopier.DeepCopy(ObjPatientPCLRequestNew_BackUp);
                //allPatientPCLRequestDetail_ExtBackup = new ObservableCollection<PatientPCLRequestDetail_Ext>();
                allPatientPCLRequestDetail_Ext = new EntitiesEdit<PatientPCLRequestDetail_Ext>(allPatientPCLRequestDetail_ExtBackup, true);
            }
            //else
            //    if (formState == FormState.New)
            //    {
            //        allPatientPCLRequestDetail_Ext.Reset();
            //    }
            formWork = FormWork.Disable;
        }

        public void btEdit()
        {
            if (ObjPatientPCLRequestNew.PaidTime == null)
            {
                FormInputIsEnabled = true;
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z0392_G1_PhDaTToanKgDuocPhepSua);
            }
            formWork = FormWork.Edit;
        }

        int FindPatient = 0;/*0: Ngoai Tru;1: Noi Tru*/

        public void btSave()
        {
            if (allPatientPCLRequestDetail_Ext.NewObject.Count > 0)
            {
                if (!PCLRequestValid())
                {
                    return;
                }
                if (ObjPatientPCLRequestNew.PatientPCLReqExtID < 1)
                {
                    AddPCLRequestExtWithDetails();
                }
                else
                {
                    PatientPCLRequest_Ext temp = new PatientPCLRequest_Ext();
                    temp.PatientPCLReqExtID = ObjPatientPCLRequestNew.PatientPCLReqExtID;
                    temp.PatientPCLRequestIndicatorsExt = new ObservableCollection<PatientPCLRequestDetail_Ext>();
                    temp.PatientPCLRequestIndicatorsExt = allPatientPCLRequestDetail_Ext.NewObject;
                    AddNewPCLRequestDetailsExt(temp);
                }

            }
            if (allPatientPCLRequestDetail_Ext.DeleteObject.Count > 0)
            {
                DeletePCLRequestDetailExtList(allPatientPCLRequestDetail_Ext.DeleteObject.ToList());
            }
            formWork = FormWork.Disable;
        }
        public bool PCLRequestValid()
        {
            if (ObjPatientPCLRequestNew.AgencyID == null
               || ObjPatientPCLRequestNew.AgencyID.Value < 0)
            {
                MessageBox.Show(eHCMSResources.A0379_G1_Msg_InfoChuaChonBVLamXNNgoaiVien);
                return false;
            }
            if (allPatientPCLRequestDetail_Ext.NewObject.Count < 0)
            {
                MessageBox.Show(eHCMSResources.A0389_G1_Msg_InfoChuaChonLoaiSA);
                return false;
            }

            ObjPatientPCLRequestNew.PatientPCLRequestIndicatorsExt =
                ObjectCopier.DeepCopy(allPatientPCLRequestDetail_Ext.NewObject);
            return true;
        }

        public PatientPCLRequestDetail_Ext createPatientPCLRequestDetail_Ext(PCLExamType item)
        {
            var requestDetail = new PatientPCLRequestDetail_Ext();
            requestDetail.PCLExamType = item;
            requestDetail.NumberOfTest = 1;
            requestDetail.V_ExamRegStatus = (long)AllLookupValues.ExamRegStatus.KHONG_XAC_DINH;

            requestDetail.EntityState = EntityState.DETACHED;
            requestDetail.RecordState = RecordState.DETACHED;
            requestDetail.CreatedDate = DateTime.Now;

            requestDetail.DeptLocation = new DeptLocation();

            requestDetail.patientPCLRequest_Ext = ObjPatientPCLRequestNew;

            return requestDetail;
        }



        private void GetRegistrationForPCLRequest(List<PatientPCLRequest_Ext> listPCLRequest, List<PatientPCLRequest_Ext> lstPCLReqDelete)
        {
            //Lấy thông tin đăng ký đầy đủ để lưu lại trong module Khám Bệnh
            var t = new Thread(() =>
            {
                IsWaitingLoadRegistration = true;

                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetRegistration(ObjPatientPCLRequestNew.PtRegistrationID, FindPatient,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var regInfo = contract.EndGetRegistration(asyncResult);

                                    if (regInfo != null)
                                    {
                                        //AddServicesAndPCLRequests_Update(regInfo, null, listPCLRequest, null, lstPCLReqDelete);
                                    }

                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    IsWaitingLoadRegistration = false;
                }
            });

            t.Start();
        }
        //add cả chì lẫn chài
        private void AddPCLRequestExtWithDetails()
        {
            var t = new Thread(() =>
            {
                IsWaitingLoadRegistration = true;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        long PatientPCLReqExt;
                        contract.BeginAddPCLRequestExtWithDetails(ObjPatientPCLRequestNew,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var regInfo = contract.EndAddPCLRequestExtWithDetails(out PatientPCLReqExt, asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    IsWaitingLoadRegistration = false;
                }
            });

            t.Start();
        }
        //add chài cho chì
        private void AddNewPCLRequestDetailsExt(PatientPCLRequest_Ext objDetailNew)
        {
            var t = new Thread(() =>
            {
                IsWaitingLoadRegistration = true;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddNewPCLRequestDetailsExt(objDetailNew,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var regInfo = contract.EndAddNewPCLRequestDetailsExt(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    IsWaitingLoadRegistration = false;
                }
            });

            t.Start();
        }
        //xóa chài cho chì
        private void DeletePCLRequestDetailExtList(List<PatientPCLRequestDetail_Ext> objDetailDelete)
        {
            var t = new Thread(() =>
            {
                IsWaitingLoadRegistration = true;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeletePCLRequestDetailExtList(objDetailDelete,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var regInfo = contract.EndDeletePCLRequestDetailExtList(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    IsWaitingLoadRegistration = false;
                }
            });

            t.Start();
        }
        //xóa cả chì lẫn chài
        private void DeletePCLRequestExt(long PatientPCLReqExtID)
        {
            var t = new Thread(() =>
            {
                IsWaitingLoadRegistration = true;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeletePCLRequestExt(PatientPCLReqExtID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var regInfo = contract.EndDeletePCLRequestExt(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    IsWaitingLoadRegistration = false;
                }
            });

            t.Start();
        }

        #endregion

        #region Thông tin bệnh viện ngoài
        private ObservableCollection<TestingAgency> _ObjTestingAgencyList;
        public ObservableCollection<TestingAgency> ObjTestingAgencyList
        {
            get
            {
                return _ObjTestingAgencyList;
            }
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
            ObjTestingAgencyList.Clear();

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format("{0}...", eHCMSResources.Z0535_G1_DSBVNgoai) });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsImportClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetTestingAgency_All(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetTestingAgency_All(asyncResult);
                            if (items != null)
                            {
                                ObjTestingAgencyList = new ObservableCollection<TestingAgency>(items);

                                //Item Default
                                DataEntities.TestingAgency ItemDefault = new DataEntities.TestingAgency();
                                ItemDefault.AgencyID = -1;
                                ItemDefault.AgencyName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon);
                                ObjTestingAgencyList.Insert(0, ItemDefault);
                                //Item Default
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        #endregion

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;

            PCLExamType Objtmp = eventArgs.Value as PCLExamType;
            allPatientPCLRequestDetail_Ext.Add(createPatientPCLRequestDetail_Ext(Objtmp));
        }


        public void btDelete()
        {
            if (ObjPatientPCLRequestNew.PaidTime == null)
            {
                if (MessageBox.Show(string.Format("{0} '{1}'", eHCMSResources.A0433_G1_Msg_ConfHuyPh2, ObjPatientPCLRequestNew.PCLRequestNumID.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {

                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z0393_G1_PhDaTToanKgDuocXoa);
            }
        }

        #region "Ext chọn theo tên gõ vào autocomplete"


        private void InitControlsForExt()
        {
            NotifyOfPropertyChange(() => SPTheoFormVisibility);
            NotifyOfPropertyChange(() => SPTheoAutoCompleteVisibility);

            var ucDynamic = Globals.GetViewModel<IPCLItems_SearchAutoComplete>();
            ucDynamic.KeyAction = eHCMSResources.Z0055_G1_Edit;
            ucDynamic.Registration_DataStorage = Registration_DataStorage;
            this.ActivateItem(ucDynamic);
        }

        RadioButton CtrrdoLABByForm;
        RadioButton CtrrdoLABByName;
        public void rdoLABByForm_Loaded(object sender, RoutedEventArgs e)
        {
            CtrrdoLABByForm = sender as RadioButton;
            NotifyOfPropertyChange(() => SPTheoFormVisibility);
        }
        public void rdoLABByName_Loaded(object sender, RoutedEventArgs e)
        {
            CtrrdoLABByName = sender as RadioButton;
            NotifyOfPropertyChange(() => SPTheoAutoCompleteVisibility);
        }

        public bool cboPCLFormIsEnabled
        {
            get
            {
                if (ObjV_PCLMainCategory_Selected != null && ObjV_PCLMainCategory_Selected.LookupID == IDLABByName)
                    return false;
                return true;
            }
        }

        public bool rdoLABByFormIsEnabled
        {
            get
            {
                if (ObjV_PCLMainCategory_Selected != null && ObjV_PCLMainCategory_Selected.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                {
                    return true;
                }
                return false;
            }
        }

        public bool rdoLABByNameIsEnabled
        {
            get
            {
                if (ObjV_PCLMainCategory_Selected != null && ObjV_PCLMainCategory_Selected.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                {
                    return true;
                }
                return false;
            }
        }


        //private bool _SPTheoAutoCompleteVisibility;
        public bool SPTheoAutoCompleteVisibility
        {
            get
            {
                if (ObjV_PCLMainCategory_Selected != null && ObjV_PCLMainCategory_Selected.LookupID == IDLABByName)
                    return true;
                return false;
            }
        }

        //private bool _SPTheoFormVisibility;
        public bool SPTheoFormVisibility
        {
            get
            {
                return !(SPTheoAutoCompleteVisibility);
            }
        }

        public void rdoLABByForm_Click(object sender, RoutedEventArgs e)
        {
            RadioButton Ctr = (sender as RadioButton);
            if (Ctr.IsChecked.Value)
            {
                //SPTheoFormVisibility = true;
                //SPTheoAutoCompleteVisibility = false;

                NotifyOfPropertyChange(() => cboPCLFormIsEnabled);

                NotifyOfPropertyChange(() => SPTheoFormVisibility);
                NotifyOfPropertyChange(() => SPTheoAutoCompleteVisibility);
            }
        }

        public void rdoLABByName_Click(object sender, RoutedEventArgs e)
        {
            RadioButton Ctr = (sender as RadioButton);
            if (Ctr.IsChecked.Value)
            {
                //SPTheoFormVisibility = false;
                //SPTheoAutoCompleteVisibility = true;
                NotifyOfPropertyChange(() => cboPCLFormIsEnabled);

                NotifyOfPropertyChange(() => SPTheoFormVisibility);
                NotifyOfPropertyChange(() => SPTheoAutoCompleteVisibility);
            }
        }

        #endregion


        private bool _btSaveIsEnabled;
        public bool btSaveIsEnabled
        {
            get { return _btSaveIsEnabled; }
            set
            {
                if (_btSaveIsEnabled != value)
                {
                    _btSaveIsEnabled = value;
                    NotifyOfPropertyChange(() => btSaveIsEnabled);
                }
            }
        }

        private bool _btInIsEnabled;
        public bool btInIsEnabled
        {
            get { return _btInIsEnabled; }
            set
            {
                if (_btInIsEnabled != value)
                {
                    _btInIsEnabled = value;
                    NotifyOfPropertyChange(() => btInIsEnabled);
                }
            }
        }


        #region "Tạo mới PCLRequest"

        private bool _isWaitingLoadChanDoan;
        public bool IsWaitingLoadChanDoan
        {
            get { return _isWaitingLoadChanDoan; }
            set
            {
                if (_isWaitingLoadChanDoan != value)
                {
                    _isWaitingLoadChanDoan = value;
                    NotifyOfPropertyChange(() => IsWaitingLoadChanDoan);
                    NotifyWhenBusy();
                }
            }
        }


        private DiagnosisTreatment _ObjGetDiagnosisTreatmentByPtID;
        public DiagnosisTreatment ObjGetDiagnosisTreatmentByPtID
        {
            get
            {
                return _ObjGetDiagnosisTreatmentByPtID;
            }
            set
            {
                if (_ObjGetDiagnosisTreatmentByPtID != value)
                {
                    _ObjGetDiagnosisTreatmentByPtID = value;
                    NotifyOfPropertyChange(() => ObjGetDiagnosisTreatmentByPtID);
                }
            }
        }


        /*opt:-- 0: Query by PatientID, 1: Query by PtRegistrationID, 2: Query By NationalMedicalCode  */
        private void GetDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, bool latest, long? V_RegistrationType)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format("{0}...", eHCMSResources.Z0536_G1_CDoanCuoi) });

            IsWaitingLoadChanDoan = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    long ServiceRecID = Registration_DataStorage.CurrentPatientRegistrationDetail != null ? Registration_DataStorage.CurrentPatientRegistrationDetail.ServiceRecID : 0;
                    contract.BeginGetDiagnosisTreatmentByPtID(patientID, PtRegistrationID, null, opt, true, V_RegistrationType, ServiceRecID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisTreatmentByPtID(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                ObjGetDiagnosisTreatmentByPtID = results.ToObservableCollection()[0];

                                ObjPatientPCLRequestNew.ServiceRecID = ObjGetDiagnosisTreatmentByPtID.ServiceRecID;
                                ObjPatientPCLRequestNew.PatientServiceRecord.ServiceRecID = ObjGetDiagnosisTreatmentByPtID.ServiceRecID.Value;
                                ObjPatientPCLRequestNew.Diagnosis = string.IsNullOrEmpty(ObjGetDiagnosisTreatmentByPtID.DiagnosisFinal) ? ObjGetDiagnosisTreatmentByPtID.Diagnosis.Trim() : ObjGetDiagnosisTreatmentByPtID.DiagnosisFinal.Trim();

                                //Đọc Phiếu YC cuối lên
                                PatientPCLRequest_RequestLastest();
                            }
                            else
                            {

                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsWaitingLoadChanDoan = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private PatientPCLRequest_Ext NewPCLReq()
        {
            ObjStaff = Globals.LoggedUserAccount.Staff;

            var ObjNew = new PatientPCLRequest_Ext();
            ObjNew.PatientPCLRequestIndicatorsExt = new ObservableCollection<PatientPCLRequestDetail_Ext>();
            ObjNew.DeptID = Globals.ObjRefDepartment.DeptID;
            ObjNew.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;


            ObjNew.StaffIDName = ObjStaff.FullName;
            ObjNew.StaffID = ObjStaff.StaffID;
            ObjNew.DoctorStaffID = ObjStaff.StaffID;

            ObjNew.ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID;

            ObjNew.DoctorComments = "";
            ObjNew.IsCaseOfEmergency = false;

            ObjNew.IsExternalExam = false;
            ObjNew.IsImported = false;

            ObjNew.PatientServiceRecord = new PatientServiceRecord();
            if (ObjGetDiagnosisTreatmentByPtID != null
                && ObjGetDiagnosisTreatmentByPtID.ServiceRecID > 0)
            {
                ObjNew.PatientServiceRecord.ServiceRecID = ObjGetDiagnosisTreatmentByPtID.ServiceRecID.Value;
            }

            ObjNew.PatientServiceRecord.StaffID = Globals.LoggedUserAccount.StaffID;
            ObjNew.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN;

            return ObjNew;
        }

        private void CreateNewPCLReq()
        {
            ObjPatientPCLRequestNew = NewPCLReq();
            ObjPatientPCLRequestNew.Diagnosis = string.IsNullOrEmpty(ObjGetDiagnosisTreatmentByPtID.DiagnosisFinal) ? ObjGetDiagnosisTreatmentByPtID.Diagnosis : ObjGetDiagnosisTreatmentByPtID.DiagnosisFinal;
            HasGroup = true;
        }

        #endregion


        public void Handle(ShowPatientInfo_KHAMBENH_CLS_NGOAIVIEN_HINHANH<Patient, PatientRegistration, PatientRegistrationDetail> message)
        {
            InitPatientInfo(message.Pt);
        }

        public void Handle(DbClickPtPCLReqExtEdit<PatientPCLRequest_Ext, string> message)
        {
            if (message != null)
            {
                formState = FormState.Edit;
                ObjPatientPCLRequestNew = message.ObjA;
                ObjPatientPCLRequestNew_BackUp = ObjectCopier.DeepCopy(ObjPatientPCLRequestNew);
                allPatientPCLRequestDetail_ExtBackup = new ObservableCollection<PatientPCLRequestDetail_Ext>(ObjPatientPCLRequestNew.PatientPCLRequestIndicatorsExt);
                formWork = FormWork.Disable;
                ObjV_PCLMainCategory_Selected = ObjV_PCLMainCategory.FirstOrDefault();
                allPatientPCLRequestDetail_Ext = new EntitiesEdit<PatientPCLRequestDetail_Ext>(allPatientPCLRequestDetail_ExtBackup, true);
            }
        }

        public void lnkDeleteClick(RoutedEventArgs e)
        {
            allPatientPCLRequestDetail_Ext.Remove(curPatientPCLRequestDetail_Ext);
        }

        public void lnkDelete_Loaded(object sender, RoutedEventArgs e)
        {
            //((HyperlinkButton)sender).Visibility = Globals.convertVisibilityNot(curPCLExamType.IsSelected);
        }
        public enum FormState
        {
            New = 1,
            Edit = 2,
            Restore = 3
        }
        public enum FormWork
        {
            Disable = 1,
            Edit = 2
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