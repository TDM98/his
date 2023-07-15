using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using aEMR.Infrastructure.Events;
using DataEntities;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Service.Core.Common;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using System.Collections.Generic;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IAddEditApptPCLRequest)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AddEditApptPCLRequestViewModel : ViewModelBase, IAddEditApptPCLRequest
        , IHandle<SelectedObjectEventWithKey<PCLExamType, String>>
    {
        //private const long IDLABByName = 28888;

        //Bien luu doi tuong truoc khi edit.
        private PatientApptPCLRequests _tempApptPCLRequest;

        private PatientApptPCLRequests _editingApptPCLRequest;
        public PatientApptPCLRequests EditingApptPCLRequest
        {
            get { return _editingApptPCLRequest; }
            private set 
            {
                _editingApptPCLRequest = value; 
                NotifyOfPropertyChange(()=>EditingApptPCLRequest);
                if(PCLRequestDetailsContent != null)
                {
                    PCLRequestDetailsContent.PCLRequest = EditingApptPCLRequest;
                }
            }
        }
        
        private bool _canEdit;
        public bool CanEdit
        {
            get
            {
                return _canEdit;
            }
            private set
            {
                _canEdit = value;
                NotifyOfPropertyChange(() => CanEdit);
            }
        }
        public PatientAppointment CurrentAppointment
        {
            get;
            set;
        }

        public override bool IsProcessing
        {
            get
            {
                return _isLoadingMainCategory 
                    || _isLoadingPCLForms 
                    || _isWaitingLoadPCLExamType
                    || _isWaitingSave
                    || _isWaitingLoadChanDoan
                    || IsLoadingSegmentsPCL
                    || IsSaving;
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
                if(_isWaitingLoadPCLExamType)
                {
                    return eHCMSResources.Z0483_G1_LoadDSXetNghiem;
                }
                if (_isWaitingSave)
                {
                    return eHCMSResources.Z0343_G1_DangLuu;
                }
                if (_isWaitingLoadChanDoan)
                {
                    return eHCMSResources.Z0484_G1_LoadDSChanDoan;
                }
                if(IsLoadingSegmentsPCL)
                {
                    return eHCMSResources.Z1009_G1_LoadTGian;
                }
                if(IsSaving)
                {
                    return eHCMSResources.Z1016_G1_DangLuuCuocHen;
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

        public long? RegistrationID
        {
            get;
            set;
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

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AddEditApptPCLRequestViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            _canEdit = false;
            eventAgr.Subscribe(this);
            authorization();
            rdoLABByForm = true;
            ObjStaff = Globals.LoggedUserAccount.Staff;
            //PCLRequestDetailsContent = Globals.GetViewModel<IPCLRequestDetails>();
            PCLRequestDetailsContent = Globals.GetViewModel<IEditApptPclRequestDetailList>();
            PCLRequestDetailsContent.PCLRequest = EditingApptPCLRequest;
            LoadPclMainCategories();
            SearchCriteriaExamTypeForChoose = new PCLExamTypeSearchCriteria();
            //if (Globals.PatientInfo != null)/*Làm CLS chỉ cần kiểm tra BN !=null*/
            {

                //CurrentPclRequest=new PatientPCLRequest();

                SelectedPclMainCategory = new Lookup { LookupID = -1 };

                InitControlsForExt();

                //KMx: Sau khi kiểm tra, thấy View này không sử dụng View IPatientMedicalRecords_ByPatientID (25/05/2014 14:48).
                //UC Header PMR
                //var uc3 = Globals.GetViewModel<IPatientMedicalRecords_ByPatientID>();
                //UCHeaderInfoPMR = uc3;
                //ActivateItem(uc3);
                //UC Header PMR

                SelectedTimeSegment = new PCLTimeSegment { ParaclinicalTimeSegmentID = -1 };

                LoadAppointmentSegmentsPCL();

                DiagnosisTreatment = new DiagnosisTreatment();
               
            }
        }

        private IEditApptPclRequestDetailList _pclRequestDetailsContent;
        public IEditApptPclRequestDetailList PCLRequestDetailsContent
        {
            get { return _pclRequestDetailsContent; }
            set
            {
                _pclRequestDetailsContent = value;
                NotifyOfPropertyChange(() => PCLRequestDetailsContent);
            }
        }

        //private PatientPCLRequest _currentPclRequest;
        //public PatientPCLRequest CurrentPclRequest
        //{
        //    get
        //    {
        //        return _currentPclRequest;
        //    }
        //    set
        //    {
        //        _currentPclRequest = value;
        //        NotifyOfPropertyChange(() => CurrentPclRequest);
        //        PCLRequestDetailsContent.PCLRequest = _currentPclRequest;
        //    }
        //}
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

        private Staff _objStaff;
        public Staff ObjStaff
        {
            get { return _objStaff; }
            set
            {
                if (_objStaff != value)
                {
                    _objStaff = value;
                }
            }
        }

        //KMx: Sau khi kiểm tra, thấy biến này không được sử dụng nữa (25/05/2014 14:48).
        //public object UCHeaderInfoPMR
        //{
        //    get;
        //    set;
        //}

        //private void InitPclRequestFromApptRequest(PatientApptPCLRequests apptRequest)
        //{
        //    if (apptRequest == null)
        //    {
        //        CurrentPclRequest = null;
        //        return;
        //    }
        //    var diagnosis = string.Empty;
        //    if(DiagnosisTreatment == null)
        //    {
        //        diagnosis = string.Empty;
        //    }
        //    else
        //    {
        //        if(!string.IsNullOrWhiteSpace(DiagnosisTreatment.DiagnosisFinal))
        //        {
        //            diagnosis = DiagnosisTreatment.DiagnosisFinal;
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrWhiteSpace(DiagnosisTreatment.Diagnosis))
        //            {
        //                diagnosis = DiagnosisTreatment.Diagnosis.Trim(); 
        //            }
        //        }
        //    }
            
        //    var request = new PatientPCLRequest
        //    {
        //        IsExternalExam = false,
        //        PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>(),
        //        DeptID = Globals.DeptLocation == null ? default(long?) : Globals.DeptLocation.DeptID,
        //        ReqFromDeptLocID = Globals.DeptLocation == null ? default(long?) : Globals.DeptLocation.DeptID,
        //        StaffIDName = ObjStaff != null ? ObjStaff.FullName : string.Empty,
        //        Diagnosis = diagnosis,
        //        DoctorComments = "",
        //        IsCaseOfEmergency = false,
        //        IsImported = false,
        //    };

        //    if (apptRequest.ObjPatientApptPCLRequestDetailsList != null)
        //    {
        //        for (var index = 0; index < apptRequest.ObjPatientApptPCLRequestDetailsList.Count; index++)
        //        {
        //            var apptReqDetail = apptRequest.ObjPatientApptPCLRequestDetailsList[index];
        //            var reqDetail = new PatientPCLRequestDetail
        //                {
        //                    PCLExamType = apptReqDetail.ObjPCLExamTypes,
        //                    DeptLocation = apptReqDetail.ObjDeptLocID
        //                };
        //            if (index == 0)
        //            {
        //                SelectedTimeSegment = apptReqDetail.ApptTimeSegment;
        //            }
        //            request.PatientPCLRequestIndicators.Add(reqDetail);
        //        }
        //    }
        //    CurrentPclRequest = request;
        //}
        private PCLFormsSearchCriteria _searchCriteria = new PCLFormsSearchCriteria();
        public PCLFormsSearchCriteria SearchCriteria
        {
            get
            {
                return _searchCriteria;
            }
            set
            {
                _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);

            }
        }

        private Lookup _selectedPclMainCategory;
        public Lookup SelectedPclMainCategory
        {
            get { return _selectedPclMainCategory; }
            set
            {
                _selectedPclMainCategory = value;
                NotifyOfPropertyChange(() => SelectedPclMainCategory);
                NotifyOfPropertyChange(() => PclTimeSegmentsByForm);
                NotifyOfPropertyChange(() => CanSelectPclForm);
                NotifyOfPropertyChange(() => SPTheoFormVisibility);
                NotifyOfPropertyChange(() => SPTheoAutoCompleteVisibility);

                if (_selectedPclMainCategory != null && _selectedPclMainCategory.LookupID > 0)
                {
                    if(SPTheoAutoCompleteVisibility)
                    {
                        SPTheoAutoComplete.SearchCriteria.V_PCLMainCategory = _selectedPclMainCategory.LookupID;
                    }
                    SearchCriteria.V_PCLMainCategory = _selectedPclMainCategory.LookupID;

                    ObjPCLItems_ByPCLFormID = null;
                    SelectedItemForChoose = new PCLExamType();

                    PCLForm_GetList(0, 99999, true);//ko phân trang
                }
                EffectBoNut();
            }
        }

        public bool CanSelectPclForm
        {
            get
            {
                return !_byPclName || (SelectedPclMainCategory != null && SelectedPclMainCategory.LookupID <= 0);
            }
        }
        private bool _byPclName;

        public bool ByPclName
        {
            get { return _byPclName; }
            set
            {
                _byPclName = value;
                NotifyOfPropertyChange(() => ByPclName);
                NotifyOfPropertyChange(() => CanSelectPclForm);
                NotifyOfPropertyChange(() => SPTheoAutoCompleteVisibility);
                NotifyOfPropertyChange(() => SPTheoFormVisibility);
                if (SPTheoAutoCompleteVisibility)
                {
                    SPTheoAutoComplete.SearchCriteria.V_PCLMainCategory = _selectedPclMainCategory != null ? _selectedPclMainCategory.LookupID : 0;
                }
            }
        }

        private PCLForm _objPCLFormsGetListSelected;
        public PCLForm ObjPCLForms_GetList_Selected
        {
            get { return _objPCLFormsGetListSelected; }
            set
            {
                _objPCLFormsGetListSelected = value;
                NotifyOfPropertyChange(() => ObjPCLForms_GetList_Selected);
            }
        }

        private ObservableCollection<PCLForm> _objPCLFormsGetList;
        public ObservableCollection<PCLForm> ObjPCLForms_GetList
        {
            get { return _objPCLFormsGetList; }
            set
            {
                _objPCLFormsGetList = value;
                NotifyOfPropertyChange(() => ObjPCLForms_GetList);
            }
        }
        private void PCLForm_GetList(int pageIndex, int pageSize, bool countTotal)
        {
            if (ObjPCLForms_GetList == null)
            {
                ObjPCLForms_GetList = new ObservableCollection<PCLForm>();
            }
            else
            {
                ObjPCLForms_GetList.Clear();
            }

            var t = new Thread(() =>
            {
                IsLoadingPCLForms = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLForms_GetList_Paging(SearchCriteria, pageIndex, pageSize, "", countTotal, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                int total;
                                var allItems = client.EndPCLForms_GetList_Paging(out total, asyncResult);
                                if (allItems != null)
                                {
                                    ObjPCLForms_GetList = new ObservableCollection<PCLForm>(allItems);

                                    //ItemDefault
                                    var itemDefault = new PCLForm {PCLFormID = -1, PCLFormName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2088_G1_ChonPCLForm2)};
                                    ObjPCLForms_GetList.Insert(0, itemDefault);
                                    //ItemDefault

                                    ObjPCLForms_GetList_Selected = itemDefault;
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                    IsLoadingPCLForms = false;
                }
            });
            t.Start();
        }

        private bool _rdoLabByForm;

        public bool rdoLABByForm
        {
            get { return _rdoLabByForm; }
            set
            {
                _rdoLabByForm = value;
                NotifyOfPropertyChange(() => rdoLABByForm);
                NotifyOfPropertyChange(() => SPTheoAutoCompleteVisibility);
                NotifyOfPropertyChange(() => SPTheoFormVisibility);
            }
        }

        private ObservableCollection<Lookup> _pclMainCategories;
        public ObservableCollection<Lookup> PclMainCategories
        {
            get { return _pclMainCategories; }
            set
            {
                _pclMainCategories = value;
                NotifyOfPropertyChange(() => PclMainCategories);
            }
        }

        public void LoadPclMainCategories()
        {
            var objList = new List<Lookup>();

            var item0 = new Lookup {LookupID = -1, ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2034_G1_ChonLoai2)};
            /**/
            objList.Add(item0);

            var item1 = new Lookup
                {LookupID = (long) AllLookupValues.V_PCLMainCategory.Imaging, ObjectValue = "Imaging"};
            objList.Add(item1);

            var item2 = new Lookup { LookupID = (long)AllLookupValues.V_PCLMainCategory.Laboratory, ObjectValue = "Laboratory" };
            objList.Add(item2);

            //var item2 = new Lookup
            //    {LookupID = (long) AllLookupValues.V_PCLMainCategory.Laboratory, ObjectValue = "Laboratory(theo Form)"};
            //objList.Add(item2);

            //var item3 = new Lookup {LookupID = IDLABByName, ObjectValue = "Laboratory(theo Tên Xét Nghiệm)"};
            ///*tự đặt*/
            //objList.Add(item3);

            var item4 = new Lookup
                {LookupID = (long) AllLookupValues.V_PCLMainCategory.Pathology, ObjectValue = "Pathology"};
            objList.Add(item4);
            
            PclMainCategories=new ObservableCollection<Lookup>(objList);
        }

        public void cboPCLForm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox) sender).SelectedItem == null) return;
            var objtmp = (((ComboBox) sender).SelectedItem as PCLForm);
            if (objtmp.PCLFormID > 0)
            {
                SearchCriteriaExamTypeForChoose.V_PCLMainCategory = SelectedPclMainCategory.LookupID;
                PCLItems_ByPCLFormID_HasGroup(HasGroup);
            }
        }


        private PCLExamTypeSearchCriteria _searchCriteriaExamTypeForChoose;
        public PCLExamTypeSearchCriteria SearchCriteriaExamTypeForChoose
        {
            get
            {
                return _searchCriteriaExamTypeForChoose;
            }
            set
            {
                _searchCriteriaExamTypeForChoose = value;
                NotifyOfPropertyChange(() => SearchCriteriaExamTypeForChoose);

            }
        }

        private aEMR.Common.PagedCollectionView.PagedCollectionView _objPCLItemsByPCLFormID;
        public aEMR.Common.PagedCollectionView.PagedCollectionView ObjPCLItems_ByPCLFormID
        {
            get { return _objPCLItemsByPCLFormID; }
            set
            {
                _objPCLItemsByPCLFormID = value;
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

        private void PCLItems_ByPCLFormID_HasGroup(bool isGroup)
        {
            var t = new Thread(() =>
            {
                IsWaitingLoadPCLExamType = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLItems_ByPCLFormID(SearchCriteriaExamTypeForChoose, ObjPCLForms_GetList_Selected.PCLFormID, null, Globals.DispatchCallback(asyncResult =>
                    {
                        try
                        {
                            var items = contract.EndPCLItems_ByPCLFormID(asyncResult);

                            if (items != null)
                            {
                                ObjPCLItems_ByPCLFormID = new aEMR.Common.PagedCollectionView.PagedCollectionView(items);

                                if (isGroup)
                                {
                                    ObjPCLItems_ByPCLFormID.GroupDescriptions.Add(new aEMR.Common.PagedCollectionView.PropertyGroupDescription("PCLSectionName"));
                                }
                            }
                            else
                            {
                                ObjPCLItems_ByPCLFormID = null;
                            }

                            EffectBoNut();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsWaitingLoadPCLExamType = false;
                        }
                    }), null);
                }


            });
            t.Start();

        }

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        #region 3 Nút Add

        private PCLExamType _selectedItemForChoose;
        public PCLExamType SelectedItemForChoose
        {
            get { return _selectedItemForChoose; }
            set
            {
                _selectedItemForChoose = value;
                NotifyOfPropertyChange(() => SelectedItemForChoose);
            }
        }

        private ObservableCollection<PCLExamType> _objPCLItemsByPCLFormIDSelected;
        public ObservableCollection<PCLExamType> ObjPCLItems_ByPCLFormID_Selected
        {
            get { return _objPCLItemsByPCLFormIDSelected; }
            set
            {
                _objPCLItemsByPCLFormIDSelected = value;
                NotifyOfPropertyChange(() => ObjPCLItems_ByPCLFormID_Selected);
            }
        }

        public void StartEditing(long PatientID, PatientApptPCLRequests apptPCLRequest)
        {
            if (apptPCLRequest == null)
            {
                MessageBox.Show(eHCMSResources.A0299_G1_Msg_InfoChon1YCCLS);
                return;
            }

            _tempApptPCLRequest = apptPCLRequest;

            EditingApptPCLRequest = _tempApptPCLRequest.DeepCopy();

            CanEdit = true;

            if (_editingApptPCLRequest != null)
            {
                CurrentAppointment = EditingApptPCLRequest.PatientAppointment;
            }

            //if(EditingApptPCLRequest.PatientPCLReqID==null || EditingApptPCLRequest.PatientPCLReqID<=0)
            //{
                DiagnosisTreatment_GetLast(PatientID,0,0);
            //}

        }
        /// <summary>
        /// Tra ve true neu examType nay da ton tai.
        /// </summary>
        /// <param name="examType"></param>
        /// <returns></returns>
        private bool AddToCurrentPclRequestIfNotExists(PCLExamType examType)
        {
            if (EditingApptPCLRequest == null)
            {
                return false;
            }
            if (EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList == null)
            {
                EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList = new ObservableCollection<PatientApptPCLRequestDetails>();
            }
            var existingItem = EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList.FirstOrDefault(item => item.ObjPCLExamTypes.PCLExamTypeID == examType.PCLExamTypeID && item.EntityState!= EntityState.DELETED_MODIFIED);
            if(existingItem != null)
            {
                //TODO:Tu tu lam.    
            }
            else
            {
                //Lấy ds phòng PCLExamType lên
                switch (examType.V_PCLMainCategory)
                {
                    case (long)AllLookupValues.V_PCLMainCategory.Imaging:
                        examType.ApptPclTimeSegments = _imagingTimeSegments;
                        break;
                    case (long)AllLookupValues.V_PCLMainCategory.Laboratory:
                        examType.ApptPclTimeSegments = _labTimeSegments;
                        break;
                    case (long)AllLookupValues.V_PCLMainCategory.Pathology:
                        examType.ApptPclTimeSegments = _pathologyTimeSegments;
                        break;
                }
                ListDeptLocation_ByPCLExamTypeID(examType);
                //Lấy ds phòng PCLExamType lên   
            }
            return existingItem != null;
        }

        public bool CanbtAdd
        {
            get
            {
                if (ObjPCLItems_ByPCLFormID == null || ObjPCLItems_ByPCLFormID.Count <= 0)
                    return false;
                return true;
            }
        }
        public void btAdd()
        {
            if (ObjPCLItems_ByPCLFormID ==null || ObjPCLItems_ByPCLFormID.Count <= 0)
                return;

            AddOne();
        }

        private void AddOne()
        {
            if (SelectedItemForChoose != null)
            {
                var bExists = AddToCurrentPclRequestIfNotExists(SelectedItemForChoose);

                if (bExists)
                {
                    MessageBox.Show(string.Format("'{0}' {1}", eHCMSResources.A0071_G1_Msg_InfoItemIsSelected, SelectedItemForChoose.PCLExamTypeName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0347_G1_Msg_InfoChonPCLExamType, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        private void EffectBoNut()
        {
            NotifyOfPropertyChange(() => CanbtAdd);
            NotifyOfPropertyChange(() => CanbtAddAll);
            NotifyOfPropertyChange(() => CanbtSubtractAll);
        }

        public bool CanbtAddAll
        {
            get
            {
                if (ObjPCLItems_ByPCLFormID == null || ObjPCLItems_ByPCLFormID.Count <= 0)
                    return false;
                return true;
            }
        }
        public void btAddAll()
        {
            if (ObjPCLItems_ByPCLFormID==null || ObjPCLItems_ByPCLFormID.Count <= 0)
                return;

            var sb = new StringBuilder();

            foreach (PCLExamType itemForChoose in ObjPCLItems_ByPCLFormID)
            {
                var bExists = AddToCurrentPclRequestIfNotExists(itemForChoose);

                if (bExists)
                {
                    sb.AppendLine("'" + itemForChoose.PCLExamTypeName.Trim() + "'");
                }
            }

            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                MessageBox.Show(sb + Environment.NewLine + eHCMSResources.A0071_G1_Msg_InfoItemIsSelected);
            }
        }

        public bool CanbtSubtractAll
        {
            get
            {
                return EditingApptPCLRequest != null && EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList != null && EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList.Count >= 0;
            }
        }

        public void btSubtractAll()
        {
            if (EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList == null || EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList.Count <= 0)
                return;

            if (!EditingApptPCLRequest.CheckNewPCL(EditingApptPCLRequest))
            {
                MessageBox.Show(eHCMSResources.A0637_G1_Msg_InfoKhCoDVMoi);
                return;
            }
            if (MessageBox.Show(string.Format("{0} {1}", eHCMSResources.K0478_G1_XoaHetPCLExamTypeMoiThemTrongDS, eHCMSResources.K0478_G1_BanCoChacKhong), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                //khong the xoa het duoc
                //EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList.Clear();
                if (EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList.Count>0)
                {
                    foreach (var item in EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList)
                    {
                        if (item.PCLReqItemID > 0)
                        {
                            item.EntityState = EntityState.DELETED_MODIFIED;                            
                        }
                    }
                }                          
                //PCLRequestDetailsContent.ResetCollection();
            }
            NotifyOfPropertyChange(() => CanbtSubtractAll);
            NotifyOfPropertyChange(() => EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList);
            PCLRequestDetailsContent.RefreshView();
        }

        #endregion

        #region Lấy chẩn đoán cuối
        private DiagnosisTreatment _diagnosisTreatment;
        public DiagnosisTreatment DiagnosisTreatment
        {
            get
            {
                return _diagnosisTreatment;
            }
            set
            {
                if (_diagnosisTreatment != value)
                {
                    _diagnosisTreatment = value;
                    NotifyOfPropertyChange(() => DiagnosisTreatment);
                }
            }
        }

        private void DiagnosisTreatment_GetLast(long PatientID, long PtRegistrationID, long ServiceRecID)
        {
            IsWaitingLoadChanDoan = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDiagnosisTreatment_GetLast(PatientID, PtRegistrationID, ServiceRecID,Globals.DispatchCallback(asyncResult =>
                    {
                        try
                        {
                            var results = contract.EndDiagnosisTreatment_GetLast(asyncResult);

                            if (DiagnosisTreatment==null)
                                DiagnosisTreatment=new DiagnosisTreatment();

                            DiagnosisTreatment = results;

                            string strDiag = "";
                            if (!string.IsNullOrEmpty(DiagnosisTreatment.DiagnosisFinal))
                            {
                                strDiag = DiagnosisTreatment.DiagnosisFinal;
                            }
                            else
                            {
                                if (DiagnosisTreatment.Diagnosis!=null)
                                {
                                    strDiag = DiagnosisTreatment.Diagnosis;
                                }
                            }

                            EditingApptPCLRequest.Diagnosis = strDiag.Trim();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            IsWaitingLoadChanDoan = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        #endregion

        #region Thông tin bệnh viện ngoài
        private ObservableCollection<TestingAgency> _objTestingAgencyList;
        public ObservableCollection<TestingAgency> ObjTestingAgencyList
        {
            get
            {
                return _objTestingAgencyList;
            }
            set
            {
                if (_objTestingAgencyList != value)
                {
                    _objTestingAgencyList = value;
                    NotifyOfPropertyChange(() => ObjTestingAgencyList);
                }
            }
        }
        #endregion

        public void DoubleClick(object args)
        {
            var eventArgs = args as aEMR.Common.EventArgs<object>;

            var objtmp = eventArgs.Value as PCLExamType;

            if (objtmp != null)
            {
                bool bExists = AddToCurrentPclRequestIfNotExists(objtmp);

                if (bExists)
                {
                    MessageBox.Show(string.Format("'{0}' {1}", objtmp.PCLExamTypeName.Trim(), eHCMSResources.A0071_G1_Msg_InfoItemIsSelected), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0347_G1_Msg_InfoChonPCLExamType, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        #region "Ext chọn theo tên gõ vào autocomplete"

        private IPCLItems_SearchAutoComplete _SPTheoAutoComplete;
        public IPCLItems_SearchAutoComplete SPTheoAutoComplete
        {
            get { return _SPTheoAutoComplete; }
            set
            {
                if (_SPTheoAutoComplete != value)
                {
                    _SPTheoAutoComplete = value;
                    NotifyOfPropertyChange(() => SPTheoAutoComplete);
                }
            }
        }

        private void InitControlsForExt()
        {
            var ucDynamic = Globals.GetViewModel<IPCLItems_SearchAutoComplete>();
            ucDynamic.KeyAction = "AddNew";
            SPTheoAutoComplete = ucDynamic;
            ActivateItem(ucDynamic);
        }

        public bool SPTheoAutoCompleteVisibility
        {
            get
            {
                return SelectedPclMainCategory != null && _byPclName;
            }
        }

        public bool SPTheoFormVisibility
        {
            get
            {
                return SelectedPclMainCategory != null && !_byPclName;
            }
        }

        public void Handle(SelectedObjectEventWithKey<PCLExamType, String> message)
        {
            if (message != null)
            {
                if (GetView() != null)
                {
                    if (message.Key == "AddNew")
                    {
                        SelectedItemForChoose = message.Result;
                        AddOne();
                    }
                }
            }
        }

        private void ListDeptLocation_ByPCLExamTypeID(PCLExamType examType)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginListDeptLocation_ByPCLExamTypeID(examType.PCLExamTypeID, Globals.DispatchCallback(asyncResult =>
                    {
                        try
                        {
                            var results = contract.EndListDeptLocation_ByPCLExamTypeID(asyncResult);

                            examType.ObjDeptLocationList =new ObservableCollection<DeptLocation>(results);
                            if (examType.ObjDeptLocationList != null && examType.ObjDeptLocationList.Count > 0) 
                            {
                                var appReqDetail = new PatientApptPCLRequestDetails
                                {
                                    ApptTimeSegment = SelectedTimeSegment == null || (SelectedTimeSegment != null
                                    && SelectedTimeSegment.ParaclinicalTimeSegmentID <= 0) ? null : SelectedTimeSegment,
                                    ObjDeptLocID = examType.ObjDeptLocationList.Count > 1?
                                    new DeptLocation
                                    {
                                        DeptLocationID = 0,
                                        Location = new Location { LID = 0, LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2094_G1_ChonPg) }
                                    } : examType.ObjDeptLocationList.FirstOrDefault(),
                                    ObjPCLExamTypes = examType
                                };

                                EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList.Add(appReqDetail);
                            }
                            
                            PCLRequestDetailsContent.RefreshView();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                    }), null);

                }

            });

            t.Start();
        }
        //List Phòng
        



        #endregion


        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account

        private bool _mPCL_TaoPhieuMoi_Them = true;
        private bool _mPCL_TaoPhieuMoi_XemIn = true;
        private bool _mPCL_TaoPhieuMoi_In = true;

        public bool mPCL_TaoPhieuMoi_Them
        {
            get
            {
                return _mPCL_TaoPhieuMoi_Them;
            }
            set
            {
                if (_mPCL_TaoPhieuMoi_Them == value)
                    return;
                _mPCL_TaoPhieuMoi_Them = value;
                NotifyOfPropertyChange(() => mPCL_TaoPhieuMoi_Them);
            }
        }


        public bool mPCL_TaoPhieuMoi_XemIn
        {
            get
            {
                return _mPCL_TaoPhieuMoi_XemIn;
            }
            set
            {
                if (_mPCL_TaoPhieuMoi_XemIn == value)
                    return;
                _mPCL_TaoPhieuMoi_XemIn = value;
                NotifyOfPropertyChange(() => mPCL_TaoPhieuMoi_XemIn);
            }
        }


        public bool mPCL_TaoPhieuMoi_In
        {
            get
            {
                return _mPCL_TaoPhieuMoi_In;
            }
            set
            {
                if (_mPCL_TaoPhieuMoi_In == value)
                    return;
                _mPCL_TaoPhieuMoi_In = value;
                NotifyOfPropertyChange(() => mPCL_TaoPhieuMoi_In);
            }
        }



        #endregion

        
        #region Hẹn
        private PCLTimeSegment _selectedTimeSegment;
        public PCLTimeSegment SelectedTimeSegment
        {
            get
            {
                return _selectedTimeSegment;
            }
            set
            {
                if (_selectedTimeSegment != value)
                {
                    _selectedTimeSegment = value;
                    NotifyOfPropertyChange(() => SelectedTimeSegment);
                }
            }
        }
        private ObservableCollection<PCLTimeSegment> _pclTimeSegments;
        public ObservableCollection<PCLTimeSegment> PclTimeSegments
        {
            get
            {
                return _pclTimeSegments;
            }
            set
            {
                if (_pclTimeSegments != value)
                {
                    _pclTimeSegments = value;
                    NotifyOfPropertyChange(() => PclTimeSegments);
                    NotifyOfPropertyChange(() => PclTimeSegmentsByForm);
                }
            }
        }

        public ObservableCollection<PCLTimeSegment> PclTimeSegmentsByForm
        {
            get
            {
                if(_pclTimeSegments == null || _pclTimeSegments.Count == 0)
                {
                    return _pclTimeSegments;
                }
                if(_selectedPclMainCategory == null || _selectedPclMainCategory.LookupID <= 0)
                {
                    return null;
                }
                var list= _pclTimeSegments.Where(item => item.ParaclinicalTimeSegmentID <= 0 ||
                    (long)item.V_PCLMainCategory == _selectedPclMainCategory.LookupID).ToObservableCollection();

                PCLTimeSegment firstItem = new PCLTimeSegment();
                firstItem.ParaclinicalTimeSegmentID = -1;
                firstItem.SegmentName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z1024_G1_TuyChon);
                var gio = new DateTime(1900, 01, 01, 0, 0, 0);
                firstItem.StartTime = gio;
                firstItem.EndTime = gio;

                list.Insert(0, firstItem);

                SelectedTimeSegment = firstItem;

                return list;
            }
        }

        private ObservableCollection<PCLTimeSegment> _imagingTimeSegments;
        private ObservableCollection<PCLTimeSegment> _labTimeSegments;
        private ObservableCollection<PCLTimeSegment> _pathologyTimeSegments;

        private bool _isLoadingSegmentsPCL;
        public bool IsLoadingSegmentsPCL
        {
            get { return _isLoadingSegmentsPCL; }
            set
            {
                if (_isLoadingSegmentsPCL != value)
                {
                    _isLoadingSegmentsPCL = value;
                    NotifyOfPropertyChange(() => IsLoadingSegmentsPCL);
                    NotifyWhenBusy();
                }
            }
        }

        private void LoadAppointmentSegmentsPCL()
        {
            IsLoadingSegmentsPCL = true;

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllPclTimeSegments(Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var results = contract.EndGetAllPclTimeSegments(asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    PclTimeSegments = new ObservableCollection<PCLTimeSegment>(results);

                                    _imagingTimeSegments = _pclTimeSegments.Where(item => item.ParaclinicalTimeSegmentID <= 0 ||
                                        item.V_PCLMainCategory == AllLookupValues.V_PCLMainCategory.Imaging).ToObservableCollection();
                                    _labTimeSegments = _pclTimeSegments.Where(item => item.ParaclinicalTimeSegmentID <= 0 ||
                                        item.V_PCLMainCategory == AllLookupValues.V_PCLMainCategory.Laboratory).ToObservableCollection();
                                    _pathologyTimeSegments = _pclTimeSegments.Where(item => item.ParaclinicalTimeSegmentID <= 0 ||
                                        item.V_PCLMainCategory == AllLookupValues.V_PCLMainCategory.Pathology).ToObservableCollection();

                                    if (EditingApptPCLRequest != null && EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList != null)
                                    {
                                        foreach (var request in EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList)
                                        {
                                            if (request.ObjPCLExamTypes == null)
                                            {
                                                continue;
                                            }
                                            switch (request.ObjPCLExamTypes.V_PCLMainCategory)
                                            {
                                                case (long)AllLookupValues.V_PCLMainCategory.Imaging:
                                                    request.ObjPCLExamTypes.ApptPclTimeSegments = _imagingTimeSegments;
                                                    break;
                                                case (long)AllLookupValues.V_PCLMainCategory.Laboratory:
                                                    request.ObjPCLExamTypes.ApptPclTimeSegments = _labTimeSegments;
                                                    break;
                                                case (long)AllLookupValues.V_PCLMainCategory.Pathology:
                                                    request.ObjPCLExamTypes.ApptPclTimeSegments = _pathologyTimeSegments;
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                    IsLoadingSegmentsPCL = false;
                }
            });

            t.Start();
        }

        public void SetCurrentAppointment(PatientAppointment appt)
        {
            CurrentAppointment= appt;
        }

        private bool _isSaving;
        public bool IsSaving
        {
            get { return _isSaving; }
            set
            {
                if (_isSaving != value)
                {
                    _isSaving = value;
                    NotifyOfPropertyChange(() => IsSaving);
                    NotifyWhenBusy();
                }
            }
        }

        
        #endregion

        public void OkCmd()
        {
            if(_tempApptPCLRequest == null || _editingApptPCLRequest == null)
            {
                MessageBox.Show(eHCMSResources.Z0611_G1_ChuaCoYCCLS);
                return;
            }
            _tempApptPCLRequest.Diagnosis = EditingApptPCLRequest.Diagnosis;
            _tempApptPCLRequest.ObjPatientApptPCLRequestDetailsList = _editingApptPCLRequest.ObjPatientApptPCLRequestDetailsList;
            Globals.EventAggregator.Publish(new ItemEdited<PatientApptPCLRequests>{Item = _tempApptPCLRequest,Source = this});
        }
        public void CancelCmd()
        {
            TryClose();
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
                SPTheoAutoComplete.Registration_DataStorage = Registration_DataStorage;
            }
        }
    }
}
