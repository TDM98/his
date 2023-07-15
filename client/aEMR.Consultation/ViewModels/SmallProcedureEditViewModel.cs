using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

/*
 * 20190727 #001 TBL:   BM 0012974: Tạo mới dựa trên cũ cho thủ thuật
 * 20190908 #002 TNHX:  BM 0013263: Add func get field data + complete SmallProcedure
 * 20191220 #003 TBL:   BM 0021738: Fix lỗi khi thay đổi ngày thực hiện và ngày kết thúc khi lưu không thay đổi
 * 20200205 #004 TTM:   BM 0022883: Fix lỗi không hiển thị tên nhân viên thực hiện thủ thuật khi xem in lúc vừa lưu tường trình thủ thuật phẫu thuật.
 * 20200206 #005 TTM:   BM 0022883: Bổ sung thêm event để load tường trình phẫu thuật sau khi load xong thông tin thủ thuật.
 * 20200408 #006 TTM:   BM 0030105: Fix lỗi lưu thông tin thủ thuật nội trú thành thủ thuật ngoại trú
 * 20210712 #007 TNHX:  260 Thêm trường nhập bsi chính thức
 * 20211004 #008 TNHX: Lọc danh sách bsi theo cấu hình trách nhiệm + theo tích tạm khóa
 * 20211229 #009 TNHX: Thêm trường đánh bsi gây mê 2
 * 20220913 #010 BLQ: Thêm trường bác sĩ gây mê chính thức
 * 20230321 #011 BLQ: Thêm chỗ nhập icd 9
 * 20230329 #012 QTD: Dữ liệu 130
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(ISmallProcedureEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SmallProcedureEditViewModel : ViewModelBase, ISmallProcedureEdit
    {
        [ImportingConstructor]
        public SmallProcedureEditViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
            SmallProcedureDateTime = Globals.GetViewModel<IMinHourDateControl>();
            SmallProcedureDateTime.DateTime = null;
            ObjNoteTemplates_GetAll = new ObservableCollection<PrescriptionNoteTemplates>();
            NoteTemplates_GetAllIsActive();
            //▼====== #002
            BeforeRefIDC10Code = new ObservableCollection<DiseasesReference>();
            AfterRefIDC10Code = new ObservableCollection<DiseasesReference>();
            SelectedAfterICD10 = new DiseasesReference();
            SelectedBeforeICD10 = new DiseasesReference();
            SmallProcedureDateOffStitches = Globals.GetViewModel<IMinHourDateControl>();
            SmallProcedureDateOffStitches.DateTime = null;
            SmallProcedureCompleteDateTime = Globals.GetViewModel<IMinHourDateControl>();
            SmallProcedureCompleteDateTime.DateTime = null;
            GetLookupAnesthesiaTypes();
            GetLookupSurgicalModes();
            GetLookupDeathReasons();
            GetLookupCatactropheTypes();
            //▲====== #002
            //▼====== #012
            RefSurgicalSite = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_SurgicalSite).ToObservableCollection();
            //▲====== #012
            //▼====== #011
            DiagTrmtItem = new DiagnosisTreatment();
            pageIDC9 = new PagedSortableCollectionView<RefICD9>();
            pageIDC9.OnRefresh += PageIDC9_OnRefresh;
            pageIDC9.PageSize = Globals.PageSize;
            refICD9List = new ObservableCollection<DiagnosisICD9Items>();
            AddICD9BlankRow();
            //▲====== #011
            IsVisibleICD9 = !IsFromOutOrInDiag;
        }

        #region Properties
        private IMinHourDateControl _SmallProcedureDateTime;
        public IMinHourDateControl SmallProcedureDateTime
        {
            get { return _SmallProcedureDateTime; }
            set
            {
                _SmallProcedureDateTime = value;
                NotifyOfPropertyChange(() => SmallProcedureDateTime);
            }
        }

        private IMinHourDateControl _SmallProcedureCompleteDateTime;
        public IMinHourDateControl SmallProcedureCompleteDateTime
        {
            get { return _SmallProcedureCompleteDateTime; }
            set
            {
                _SmallProcedureCompleteDateTime = value;
                NotifyOfPropertyChange(() => SmallProcedureCompleteDateTime);
            }
        }

        private SmallProcedure BackupSmallProcedure;
        private SmallProcedure _SmallProcedureObj;
        public SmallProcedure SmallProcedureObj
        {
            get => _SmallProcedureObj; set
            {
                _SmallProcedureObj = value;
                NotifyOfPropertyChange(() => SmallProcedureObj);
            }
        }

        private SmallProcedure _SmallProcedureObjBackup;
        public SmallProcedure SmallProcedureObjBackup
        {
            get => _SmallProcedureObjBackup; set
            {
                _SmallProcedureObjBackup = value;
                NotifyOfPropertyChange(() => SmallProcedureObjBackup);
            }
        }
        //▼====: #007
        AxAutoComplete cboProcedureDoctorOfficial { get; set; }
        AxAutoComplete cboProcedureDoctorOfficial2 { get; set; }
        //▲====: #007
        AxAutoComplete cboProcedureDoctor { get; set; }
        AxAutoComplete cboProcedureDoctor2 { get; set; }
        AxAutoComplete cboNarcoticDoctor { get; set; }
        //▼====: #010
        AxAutoComplete cboNarcoticDoctorOfficial { get; set; }
        //▲====: #010
        AxAutoComplete cboNarcoticDoctor2 { get; set; }
        AxAutoComplete cboNurse { get; set; }
        AxAutoComplete cboNurse2 { get; set; }
        AxAutoComplete cboNurse3 { get; set; }
        AxAutoComplete cboCheckRecordDoctor { get; set; }
        private ObservableCollection<PrescriptionNoteTemplates> _ObjNoteTemplates_GetAll;
        public ObservableCollection<PrescriptionNoteTemplates> ObjNoteTemplates_GetAll
        {
            get { return _ObjNoteTemplates_GetAll; }
            set
            {
                _ObjNoteTemplates_GetAll = value;
                NotifyOfPropertyChange(() => ObjNoteTemplates_GetAll);
            }
        }
        private PrescriptionNoteTemplates _ObjNoteTemplates_Selected;
        public PrescriptionNoteTemplates ObjNoteTemplates_Selected
        {
            get { return _ObjNoteTemplates_Selected; }
            set
            {
                if(_ObjNoteTemplates_Selected == value)
                {
                    return;
                }
                _ObjNoteTemplates_Selected = value;
                NotifyOfPropertyChange(() => ObjNoteTemplates_Selected);

                if (_ObjNoteTemplates_Selected != null && _ObjNoteTemplates_Selected.PrescriptNoteTemplateID > 0 && SmallProcedureObj != null)
                {
                    string str = SmallProcedureObj.TrinhTu;
                    if (string.IsNullOrEmpty(str))
                    {
                        str = ObjNoteTemplates_Selected.DetailsTemplate;
                    }
                    else
                    {
                        str = str + Environment.NewLine + ObjNoteTemplates_Selected.DetailsTemplate;
                    }
                    SmallProcedureObj.TrinhTu = str;
                }
            }
        }

        private bool _IsVisibility = false;
        public bool IsVisibility
        {
            get { return _IsVisibility; }
            set
            {
                if (_IsVisibility != value)
                {
                    _IsVisibility = value;
                    NotifyOfPropertyChange(() => IsVisibility);
                }
            }
        }

        private bool _IsVisibilitySkip = false;
        public bool IsVisibilitySkip
        {
            get { return _IsVisibilitySkip; }
            set
            {
                if (_IsVisibilitySkip != value)
                {
                    _IsVisibilitySkip = value;
                    NotifyOfPropertyChange(() => IsVisibilitySkip);
                }
            }
        }

        private ObservableCollection<DiseasesReference> _BeforeRefIDC10Code;
        public ObservableCollection<DiseasesReference> BeforeRefIDC10Code
        {
            get
            {
                return _BeforeRefIDC10Code;
            }
            set
            {
                if (_BeforeRefIDC10Code != value)
                {
                    _BeforeRefIDC10Code = value;
                }
                NotifyOfPropertyChange(() => BeforeRefIDC10Code);
            }
        }

        private ObservableCollection<DiseasesReference> _AfterRefIDC10Code;
        public ObservableCollection<DiseasesReference> AfterRefIDC10Code
        {
            get
            {
                return _AfterRefIDC10Code;
            }
            set
            {
                if (_AfterRefIDC10Code != value)
                {
                    _AfterRefIDC10Code = value;
                }
                NotifyOfPropertyChange(() => AfterRefIDC10Code);
            }
        }

        private IMinHourDateControl _SmallProcedureDateOffStitches;
        public IMinHourDateControl SmallProcedureDateOffStitches
        {
            get { return _SmallProcedureDateOffStitches; }
            set
            {
                _SmallProcedureDateOffStitches = value;
                NotifyOfPropertyChange(() => SmallProcedureDateOffStitches);
            }
        }

        private ObservableCollection<Lookup> _RefAnesthesiaTypes;
        public ObservableCollection<Lookup> RefAnesthesiaTypes
        {
            get
            {
                return _RefAnesthesiaTypes;
            }
            set
            {
                if (_RefAnesthesiaTypes != value)
                {
                    _RefAnesthesiaTypes = value;
                }
                NotifyOfPropertyChange(() => RefAnesthesiaTypes);
            }
        }

        private ObservableCollection<Lookup> _RefDeathReasons;
        public ObservableCollection<Lookup> RefDeathReasons
        {
            get
            {
                return _RefDeathReasons;
            }
            set
            {
                if (_RefDeathReasons != value)
                {
                    _RefDeathReasons = value;
                }
                NotifyOfPropertyChange(() => RefDeathReasons);
            }
        }

        private ObservableCollection<Lookup> _RefSurgicalModes;
        public ObservableCollection<Lookup> RefSurgicalModes
        {
            get
            {
                return _RefSurgicalModes;
            }
            set
            {
                if (_RefSurgicalModes != value)
                {
                    _RefSurgicalModes = value;
                }
                NotifyOfPropertyChange(() => RefSurgicalModes);
            }
        }

        private ObservableCollection<Lookup> _RefCatactropheTypes;
        public ObservableCollection<Lookup> RefCatactropheTypes
        {
            get
            {
                return _RefCatactropheTypes;
            }
            set
            {
                if (_RefCatactropheTypes != value)
                {
                    _RefCatactropheTypes = value;
                }
                NotifyOfPropertyChange(() => RefCatactropheTypes);
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
        private bool _FormEditorIsEnabled = false;
        public bool FormEditorIsEnabled
        {
            get
            {
                return _FormEditorIsEnabled;
            }
            set
            {
                if (_FormEditorIsEnabled == value)
                {
                    return;
                }
                _FormEditorIsEnabled = value;
                NotifyOfPropertyChange(() => FormEditorIsEnabled);
            }
        }
        #endregion

        #region Events
        //▼====: #007
        public void ProcedureDoctorOfficial_Loaded(object sender, RoutedEventArgs e)
        {
            cboProcedureDoctorOfficial = (AxAutoComplete)sender;
        }

        public void ProcedureDoctorOfficial_Populating(object sender, PopulatingEventArgs e)
        {
            var mSearchText = (sender as AxAutoComplete).SearchText;
            if (!string.IsNullOrEmpty(mSearchText))
            {
                mSearchText = mSearchText.ToLower();
                mSearchText = Globals.RemoveVietnameseString(mSearchText);
            }
            (sender as AxAutoComplete).ItemsSource = Globals.AllStaffs.Where(x => x.RefStaffCategory != null
                && (x.RefStaffCategory.V_StaffCatType == (long)V_StaffCatType.BacSi)
                && Globals.RemoveVietnameseString(x.FullName.ToLower()).Contains(mSearchText));
            (sender as AxAutoComplete).PopulateComplete();
        }

        public void ProcedureDoctorOfficial_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (SmallProcedureObj == null)
            {
                return;
            }
            if ((sender as AxAutoComplete).SelectedItem == null)
            {
                SmallProcedureObj.UserOfficialPDStaffID = null;
                return;
            }
            SmallProcedureObj.UserOfficialPDStaffID = ((sender as AxAutoComplete).SelectedItem as Staff).StaffID;
            SmallProcedureObj.UserOfficialPDStaff = ((sender as AxAutoComplete).SelectedItem as Staff);
        }
        public void ProcedureDoctorOfficial2_Loaded(object sender, RoutedEventArgs e)
        {
            cboProcedureDoctorOfficial2 = (AxAutoComplete)sender;
        }

        public void ProcedureDoctorOfficial2_Populating(object sender, PopulatingEventArgs e)
        {
            var mSearchText = (sender as AxAutoComplete).SearchText;
            if (!string.IsNullOrEmpty(mSearchText))
            {
                mSearchText = mSearchText.ToLower();
                mSearchText = Globals.RemoveVietnameseString(mSearchText);
            }
            (sender as AxAutoComplete).ItemsSource = Globals.AllStaffs.Where(x => x.RefStaffCategory != null
                && (x.RefStaffCategory.V_StaffCatType == (long)V_StaffCatType.BacSi)
                && Globals.RemoveVietnameseString(x.FullName.ToLower()).Contains(mSearchText));
            (sender as AxAutoComplete).PopulateComplete();
        }

        public void ProcedureDoctorOfficial2_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (SmallProcedureObj == null)
            {
                return;
            }
            if ((sender as AxAutoComplete).SelectedItem == null)
            {
                SmallProcedureObj.UserOfficialPDStaffID2 = null;
                return;
            }
            SmallProcedureObj.UserOfficialPDStaffID2 = ((sender as AxAutoComplete).SelectedItem as Staff).StaffID;
            SmallProcedureObj.UserOfficialPDStaff2 = ((sender as AxAutoComplete).SelectedItem as Staff);
        }
        //▲====: #007

        public void ProcedureDoctor_Loaded(object sender, RoutedEventArgs e)
        {
            cboProcedureDoctor = (AxAutoComplete)sender;
        }

        public void ProcedureDoctor_Populating(object sender, PopulatingEventArgs e)
        {
            var mSearchText = (sender as AxAutoComplete).SearchText;
            if (!string.IsNullOrEmpty(mSearchText))
            {
                mSearchText = mSearchText.ToLower();
                mSearchText = Globals.RemoveVietnameseString(mSearchText);
            }
            // 20200115 TNHX: change to find with V_StaffCatType for get all Doctor
            //▼====: #008
            (sender as AxAutoComplete).ItemsSource = Globals.AllStaffs.Where(x => x.RefStaffCategory != null
                && (x.RefStaffCategory.V_StaffCatType == (long)V_StaffCatType.BacSi)
                && Globals.RemoveVietnameseString(x.FullName.ToLower()).Contains(mSearchText)
                && !x.IsStopUsing);
            //▲====: #008
            (sender as AxAutoComplete).PopulateComplete();
        }

        public void ProcedureDoctor2_Populating(object sender, PopulatingEventArgs e)
        {
            var mSearchText = (sender as AxAutoComplete).SearchText;
            if (!string.IsNullOrEmpty(mSearchText))
            {
                mSearchText = mSearchText.ToLower();
                mSearchText = Globals.RemoveVietnameseString(mSearchText);
            }
            //▼====: #008
            (sender as AxAutoComplete).ItemsSource = Globals.AllStaffs.Where(x => x.RefStaffCategory != null
                && (x.RefStaffCategory.V_StaffCatType == (long)V_StaffCatType.BacSi || x.RefStaffCategory.V_StaffCatType == (long)V_StaffCatType.PhuTa)
                && Globals.RemoveVietnameseString(x.FullName.ToLower()).Contains(mSearchText)
                && !x.IsStopUsing);
            //▲====: #008
            (sender as AxAutoComplete).PopulateComplete();
        }

        public void ProcedureDoctor_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (SmallProcedureObj == null)
            {
                return;
            }
            if ((sender as AxAutoComplete).SelectedItem == null)
            {
                SmallProcedureObj.ProcedureDoctorStaffID = null;
                return;
            }
            SmallProcedureObj.ProcedureDoctorStaffID = ((sender as AxAutoComplete).SelectedItem as Staff).StaffID;
            //▼===== #004
            SmallProcedureObj.ProcedureDoctorStaff = ((sender as AxAutoComplete).SelectedItem as Staff);
            //▲===== #004
        }

        public void ProcedureDoctor2_Loaded(object sender, RoutedEventArgs e)
        {
            cboProcedureDoctor2 = (AxAutoComplete)sender;
        }

        public void ProcedureDoctor2_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (SmallProcedureObj == null)
            {
                return;
            }
            if ((sender as AxAutoComplete).SelectedItem == null)
            {
                SmallProcedureObj.ProcedureDoctorStaffID2 = null;
                return;
            }
            SmallProcedureObj.ProcedureDoctorStaffID2 = ((sender as AxAutoComplete).SelectedItem as Staff).StaffID;
            //▼===== #004
            SmallProcedureObj.ProcedureDoctorStaff2 = ((sender as AxAutoComplete).SelectedItem as Staff);
            //▲===== #004
        }

        public void NarcoticDoctor_Loaded(object sender, RoutedEventArgs e)
        {
            cboNarcoticDoctor = (AxAutoComplete)sender;
        }

        public void NarcoticDoctor_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (SmallProcedureObj == null)
            {
                return;
            }
            if ((sender as AxAutoComplete).SelectedItem == null)
            {
                SmallProcedureObj.NarcoticDoctorStaffID = null;
                return;
            }
            SmallProcedureObj.NarcoticDoctorStaffID = ((sender as AxAutoComplete).SelectedItem as Staff).StaffID;
            //▼===== #004
            SmallProcedureObj.NarcoticDoctorStaff = ((sender as AxAutoComplete).SelectedItem as Staff);
            //▲===== #004
            SmallProcedureObj.NarcoticDoctorOfficialStaffID = null;
            SmallProcedureObj.NarcoticDoctorOfficialStaff = null;
            cboNarcoticDoctorOfficial.SelectedItem = null;
            cboNarcoticDoctorOfficial.Text = "";
        }
        //▼===== #010
        public void NarcoticDoctorOfficial_Loaded(object sender, RoutedEventArgs e)
        {
            cboNarcoticDoctorOfficial = (AxAutoComplete)sender;
        }
        public void NarcoticDoctorOfficial_Populating(object sender, PopulatingEventArgs e)
        {
            var mSearchText = (sender as AxAutoComplete).SearchText;
            if (!string.IsNullOrEmpty(mSearchText))
            {
                mSearchText = mSearchText.ToLower();
                mSearchText = Globals.RemoveVietnameseString(mSearchText);
            }
            //if (SmallProcedureObj != null && SmallProcedureObj.V_RegistrationType == (long)AllLookupValues.RegistrationType.NOI_TRU)
            //{
            //    (sender as AxAutoComplete).ItemsSource = Globals.AllStaffs.Where(x => (x.StaffCatgID == (long)StaffCatg.DieuDuong || x.StaffCatgID == (long)StaffCatg.NvHanhChanh
            //                                                                    || (x.RefStaffCategory != null && (x.RefStaffCategory.V_StaffCatType == (long)V_StaffCatType.BacSi)))
            //                                                                    && Globals.RemoveVietnameseString(x.FullName.ToLower()).Contains(mSearchText)
            //                                                                    && !x.IsStopUsing);
            //}
            //else
            //{
            //    (sender as AxAutoComplete).ItemsSource = Globals.AllStaffs.Where(x => (x.StaffCatgID == (long)StaffCatg.DieuDuong || x.StaffCatgID == (long)StaffCatg.NvHanhChanh)
            //                                                                    && Globals.RemoveVietnameseString(x.FullName.ToLower()).Contains(mSearchText)
            //                                                                    && !x.IsStopUsing);
            //}
            //(sender as AxAutoComplete).PopulateComplete();

            if (SmallProcedureObj == null || SmallProcedureObj.NarcoticDoctorStaffID == null || SmallProcedureObj.ProcedureDateTime == null)
            {
                return;
            }
            LoadNarcoticDoctorOfficial(mSearchText, (long)SmallProcedureObj.NarcoticDoctorStaffID
                , SmallProcedureDateOffStitches.DateTime.HasValue ? SmallProcedureDateOffStitches.DateTime.Value : SmallProcedureObj.ProcedureDateTime, IsFromOutOrInDiag);
        }
        public void NarcoticDoctorOfficial_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (SmallProcedureObj == null)
            {
                return;
            }
            if ((sender as AxAutoComplete).SelectedItem == null)
            {
                SmallProcedureObj.NarcoticDoctorOfficialStaffID = null;
                return;
            }
            SmallProcedureObj.NarcoticDoctorOfficialStaffID = ((sender as AxAutoComplete).SelectedItem as Staff).StaffID;
            SmallProcedureObj.NarcoticDoctorOfficialStaff = ((sender as AxAutoComplete).SelectedItem as Staff);
        }
        public void LoadNarcoticDoctorOfficial(string SearchName,long NarcoticDoctorStaffID, DateTime ProcedureDateTime, bool IsInPt)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetNarcoticDoctorOfficial(SearchName, NarcoticDoctorStaffID, ProcedureDateTime, IsInPt, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            ObservableCollection<Staff> ListDoctorOfficial = new ObservableCollection<Staff>();
                            var results = contract.EndGetNarcoticDoctorOfficial(asyncResult);
                         
                            if (results != null)
                            {
                                foreach (Staff p in results)
                                {
                                    if (Globals.RemoveVietnameseString(p.FullName.ToLower()).Contains(SearchName))
                                    {
                                        ListDoctorOfficial.Add(p);
                                    }
                                }
                            }
                            cboNarcoticDoctorOfficial.ItemsSource = ListDoctorOfficial;
                            cboNarcoticDoctorOfficial.PopulateComplete();
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
        //▲===== #010
        //▼====: #009
        AxAutoComplete cboNarcoticDoctor3 { get; set; }

        public void NarcoticDoctor3_Loaded(object sender, RoutedEventArgs e)
        {
            cboNarcoticDoctor3 = (AxAutoComplete)sender;
        }

        public void NarcoticDoctor3_Populating(object sender, PopulatingEventArgs e)
        {
            var mSearchText = (sender as AxAutoComplete).SearchText;
            if (!string.IsNullOrEmpty(mSearchText))
            {
                mSearchText = mSearchText.ToLower();
                mSearchText = Globals.RemoveVietnameseString(mSearchText);
            }
            if (SmallProcedureObj != null && SmallProcedureObj.V_RegistrationType == (long)AllLookupValues.RegistrationType.NOI_TRU)
            {
                (sender as AxAutoComplete).ItemsSource = Globals.AllStaffs.Where(x => (x.StaffCatgID == (long)StaffCatg.DieuDuong || x.StaffCatgID == (long)StaffCatg.NvHanhChanh
                                                                                || (x.RefStaffCategory != null && (x.RefStaffCategory.V_StaffCatType == (long)V_StaffCatType.BacSi)))
                                                                                && Globals.RemoveVietnameseString(x.FullName.ToLower()).Contains(mSearchText)
                                                                                && !x.IsStopUsing);
            }
            else
            {
                (sender as AxAutoComplete).ItemsSource = Globals.AllStaffs.Where(x => (x.StaffCatgID == (long)StaffCatg.DieuDuong || x.StaffCatgID == (long)StaffCatg.NvHanhChanh)
                                                                                && Globals.RemoveVietnameseString(x.FullName.ToLower()).Contains(mSearchText)
                                                                                && !x.IsStopUsing);
            }
            (sender as AxAutoComplete).PopulateComplete();
        }

        public void NarcoticDoctor3_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (SmallProcedureObj == null)
            {
                return;
            }
            if ((sender as AxAutoComplete).SelectedItem == null)
            {
                SmallProcedureObj.NarcoticDoctorStaffID3 = null;
                return;
            }
            SmallProcedureObj.NarcoticDoctorStaffID3 = ((sender as AxAutoComplete).SelectedItem as Staff).StaffID;
            SmallProcedureObj.NarcoticDoctorStaff3 = ((sender as AxAutoComplete).SelectedItem as Staff);
        }
        //▲====: #009


        public void NarcoticDoctor2_Loaded(object sender, RoutedEventArgs e)
        {
            cboNarcoticDoctor2 = (AxAutoComplete)sender;
        }

        public void NarcoticDoctor2_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (SmallProcedureObj == null)
            {
                return;
            }
            if ((sender as AxAutoComplete).SelectedItem == null)
            {
                SmallProcedureObj.NarcoticDoctorStaffID2 = null;
                return;
            }
            SmallProcedureObj.NarcoticDoctorStaffID2 = ((sender as AxAutoComplete).SelectedItem as Staff).StaffID;
            //▼===== #004
            SmallProcedureObj.NarcoticDoctorStaff2 = ((sender as AxAutoComplete).SelectedItem as Staff);
            //▲===== #004
        }

        public void Nurse_Populating(object sender, PopulatingEventArgs e)
        {
            var mSearchText = (sender as AxAutoComplete).SearchText;
            if (!string.IsNullOrEmpty(mSearchText))
            {
                mSearchText = mSearchText.ToLower();
                mSearchText = Globals.RemoveVietnameseString(mSearchText);
            }
            //▼====: #00
            if (SmallProcedureObj != null && SmallProcedureObj.V_RegistrationType == (long)AllLookupValues.RegistrationType.NOI_TRU)
            {
                //▼====: #008
                (sender as AxAutoComplete).ItemsSource = Globals.AllStaffs.Where(x => (x.StaffCatgID == (long)StaffCatg.DieuDuong || x.StaffCatgID == (long)StaffCatg.NvHanhChanh
                                                                                || (x.RefStaffCategory != null && (x.RefStaffCategory.V_StaffCatType == (long)V_StaffCatType.BacSi)))
                                                                                && Globals.RemoveVietnameseString(x.FullName.ToLower()).Contains(mSearchText)
                                                                                && !x.IsStopUsing);
                //▲====: #008
            }
            else
            {
                //▼====: #008
                (sender as AxAutoComplete).ItemsSource = Globals.AllStaffs.Where(x => (x.StaffCatgID == (long)StaffCatg.DieuDuong || x.StaffCatgID == (long)StaffCatg.NvHanhChanh)
                                                                                && Globals.RemoveVietnameseString(x.FullName.ToLower()).Contains(mSearchText)
                                                                                && !x.IsStopUsing);
                //▲====: #008
            }
            //▲====: #008
            (sender as AxAutoComplete).PopulateComplete();
        }

        public void Nurse_Loaded(object sender, RoutedEventArgs e)
        {
            cboNurse = (AxAutoComplete)sender;
        }

        public void Nurse_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (SmallProcedureObj == null)
            {
                return;
            }
            if ((sender as AxAutoComplete).SelectedItem == null)
            {
                SmallProcedureObj.NurseStaffID = null;
                return;
            }
            SmallProcedureObj.NurseStaffID = ((sender as AxAutoComplete).SelectedItem as Staff).StaffID;
            //▼===== #004
            SmallProcedureObj.NurseStaff = ((sender as AxAutoComplete).SelectedItem as Staff);
            //▲===== #004
        }

        public void Nurse2_Loaded(object sender, RoutedEventArgs e)
        {
            cboNurse2 = (AxAutoComplete)sender;
        }

        public void Nurse2_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (SmallProcedureObj == null)
            {
                return;
            }
            if ((sender as AxAutoComplete).SelectedItem == null)
            {
                SmallProcedureObj.NurseStaffID2 = null;
                return;
            }
            SmallProcedureObj.NurseStaffID2 = ((sender as AxAutoComplete).SelectedItem as Staff).StaffID;
            //▼===== #004
            SmallProcedureObj.NurseStaff2 = ((sender as AxAutoComplete).SelectedItem as Staff);
            //▲===== #004
        }

        public void Nurse3_Loaded(object sender, RoutedEventArgs e)
        {
            cboNurse3 = (AxAutoComplete)sender;
        }

        public void Nurse3_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (SmallProcedureObj == null)
            {
                return;
            }
            if ((sender as AxAutoComplete).SelectedItem == null)
            {
                SmallProcedureObj.NurseStaffID3 = null;
                return;
            }
            SmallProcedureObj.NurseStaffID3 = ((sender as AxAutoComplete).SelectedItem as Staff).StaffID;
            //▼===== #004
            SmallProcedureObj.NurseStaff3 = ((sender as AxAutoComplete).SelectedItem as Staff);
            //▲===== #004
        }

        public void CheckRecordDoctor_Loaded(object sender, RoutedEventArgs e)
        {
            cboCheckRecordDoctor = (AxAutoComplete)sender;
        }

        public void CheckRecordDoctor_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (SmallProcedureObj == null)
            {
                return;
            }
            if ((sender as AxAutoComplete).SelectedItem == null)
            {
                SmallProcedureObj.CheckRecordDoctorStaffID = null;
                return;
            }
            SmallProcedureObj.CheckRecordDoctorStaffID = ((sender as AxAutoComplete).SelectedItem as Staff).StaffID;
            //▼===== #004
            SmallProcedureObj.CheckRecordDoctorStaff = ((sender as AxAutoComplete).SelectedItem as Staff);
            //▲===== #004
        }

        //▼====== #002
        private DiseasesReference _SelectedAfterICD10;
        public DiseasesReference SelectedAfterICD10
        {
            get
            {
                return _SelectedAfterICD10;
            }
            set
            {
                if (_SelectedAfterICD10 != value)
                {
                    _SelectedAfterICD10 = value;
                }
                NotifyOfPropertyChange(() => SelectedAfterICD10);
            }
        }

        public void CallNotifyOfPropertyChange(string aBeforeDiagTreatment)
        {
            SelectedBeforeICD10 = SmallProcedureObj.BeforeICD10;
            BeforeDiagTreatment = aBeforeDiagTreatment;
            NotifyOfPropertyChange(() => SelectedBeforeICD10);
        }

        private DiseasesReference _SelectedBeforeICD10;
        public DiseasesReference SelectedBeforeICD10
        {
            get
            {
                return _SelectedBeforeICD10;
            }
            set
            {
                if (_SelectedBeforeICD10 != value)
                {
                    _SelectedBeforeICD10 = value;
                }
                NotifyOfPropertyChange(() => SelectedBeforeICD10);
            }
        }

        private string _BeforeDiagTreatment;
        public string BeforeDiagTreatment
        {
            get
            {
                return _BeforeDiagTreatment;
            }
            set
            {
                if (_BeforeDiagTreatment != value)
                {
                    _BeforeDiagTreatment = value;
                }
                NotifyOfPropertyChange(() => BeforeDiagTreatment);
            }
        }

        AutoCompleteBox BeforeAcb_ICD10_Code = null;
        public void BeforeAcbICD10Code_Loaded(object sender, RoutedEventArgs e)
        {
            BeforeAcb_ICD10_Code = (AutoCompleteBox)sender;
        }
        public void BeforeAucICD10_Populating(object sender, PopulatingEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Parameter))
            {
                LoadBeforeRefDiseases(e.Parameter, 0, 0, 100);
            }
        }
        private bool BeforeIsDropDown = false;
        public void BeforeAxAutoComplete_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            BeforeIsDropDown = true;
        }
        public void BeforeAxAutoComplete_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!BeforeIsDropDown)
            {
                return;
            }
            BeforeIsDropDown = false;
            if (BeforeAcb_ICD10_Code != null)
            {
                if (BeforeAcb_ICD10_Code.SelectedItem is DiseasesReference BDiagTreatment)
                {
                    BeforeDiagTreatment = BDiagTreatment.DiseaseNameVN;
                    SelectedBeforeICD10 = BDiagTreatment;
                    SmallProcedureObj.BeforeICD10 = BDiagTreatment;
                }
            }
        }

        private string _AfterDiagTreatment;
        public string AfterDiagTreatment
        {
            get
            {
                return _AfterDiagTreatment;
            }
            set
            {
                if (_AfterDiagTreatment != value)
                {
                    _AfterDiagTreatment = value;
                }
                NotifyOfPropertyChange(() => AfterDiagTreatment);
            }
        }

        AutoCompleteBox AfterAcb_ICD10_Code = null;
        public void AfterAcbICD10Code_Loaded(object sender, RoutedEventArgs e)
        {
            AfterAcb_ICD10_Code = (AutoCompleteBox)sender;
        }
        public void AfterAucICD10_Populating(object sender, PopulatingEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Parameter))
            {
                LoadAfterRefDiseases(e.Parameter, 0, 0, 100);
            }
        }
        private bool AfterIsDropDown = false;
        public void AfterAxAutoComplete_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AfterIsDropDown = true;
        }
        public void AfterAxAutoComplete_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!AfterIsDropDown)
            {
                return;
            }
            AfterIsDropDown = false;
            if (AfterAcb_ICD10_Code != null)
            {
                if (AfterAcb_ICD10_Code.SelectedItem is DiseasesReference BDiagTreatment)
                {
                    AfterDiagTreatment = BDiagTreatment.DiseaseNameVN;
                    SelectedAfterICD10 = BDiagTreatment;
                    SmallProcedureObj.AfterICD10 = BDiagTreatment;
                }
            }
        }
        //▲====== #002
        #endregion

        #region Methods
        public void NoteTemplates_GetAllIsActive()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        var pnt = new PrescriptionNoteTemplates();
                        pnt.V_PrescriptionNoteTempType = AllLookupValues.V_PrescriptionNoteTempType.SmallProceduresSequence;
                        contract.BeginPrescriptionNoteTemplates_GetAllIsActive(pnt, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PrescriptionNoteTemplates> allItems = new ObservableCollection<PrescriptionNoteTemplates>();
                            try
                            {
                                allItems = contract.EndPrescriptionNoteTemplates_GetAllIsActive(asyncResult);
                                ObjNoteTemplates_GetAll = new ObservableCollection<PrescriptionNoteTemplates>(allItems);
                                PrescriptionNoteTemplates firstItem = new PrescriptionNoteTemplates();
                                firstItem.PrescriptNoteTemplateID = -1;
                                firstItem.NoteDetails = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K0616_G1_ChonMau);
                                ObjNoteTemplates_GetAll.Insert(0, firstItem);

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
                }
            });
            t.Start();
        }

        public void ApplySmallProcedure(SmallProcedure aSmallProcedureObj)
        {
            ClearCBB();
            if (aSmallProcedureObj == null)
            {
                aSmallProcedureObj = new SmallProcedure();
            }
            if (aSmallProcedureObj.ProcedureDateTime == DateTime.MinValue)
            {
                aSmallProcedureObj.ProcedureDateTime = Globals.GetCurServerDateTime();
            }
            if (aSmallProcedureObj.CompletedDateTime == DateTime.MinValue)
            {
                aSmallProcedureObj.CompletedDateTime = Globals.GetCurServerDateTime();
            }
            if (aSmallProcedureObj.DateOffStitches == DateTime.MinValue)
            {
                aSmallProcedureObj.DateOffStitches = null;
            }
            cboProcedureDoctorOfficial.SelectedItem = Globals.AllStaffs.Where(x => x.StaffID == aSmallProcedureObj.UserOfficialPDStaffID).FirstOrDefault();
            cboProcedureDoctorOfficial2.SelectedItem = Globals.AllStaffs.Where(x => x.StaffID == aSmallProcedureObj.UserOfficialPDStaffID2).FirstOrDefault();
            cboProcedureDoctor.SelectedItem = Globals.AllStaffs.Where(x => x.StaffID == aSmallProcedureObj.ProcedureDoctorStaffID).FirstOrDefault();
            cboProcedureDoctor2.SelectedItem = Globals.AllStaffs.Where(x => x.StaffID == aSmallProcedureObj.ProcedureDoctorStaffID2).FirstOrDefault();
            cboNarcoticDoctor.SelectedItem = Globals.AllStaffs.Where(x => x.StaffID == aSmallProcedureObj.NarcoticDoctorStaffID).FirstOrDefault();
            //▼===== #010
            cboNarcoticDoctorOfficial.SelectedItem = Globals.AllStaffs.Where(x => x.StaffID == aSmallProcedureObj.NarcoticDoctorOfficialStaffID).FirstOrDefault();
            //▲===== #010
            cboNarcoticDoctor2.SelectedItem = Globals.AllStaffs.Where(x => x.StaffID == aSmallProcedureObj.NarcoticDoctorStaffID2).FirstOrDefault();
            cboNarcoticDoctor3.SelectedItem = Globals.AllStaffs.Where(x => x.StaffID == aSmallProcedureObj.NarcoticDoctorStaffID3).FirstOrDefault();
            cboNurse.SelectedItem = Globals.AllStaffs.Where(x => x.StaffID == aSmallProcedureObj.NurseStaffID).FirstOrDefault();
            cboNurse2.SelectedItem = Globals.AllStaffs.Where(x => x.StaffID == aSmallProcedureObj.NurseStaffID2).FirstOrDefault();
            cboNurse3.SelectedItem = Globals.AllStaffs.Where(x => x.StaffID == aSmallProcedureObj.NurseStaffID3).FirstOrDefault();
            cboCheckRecordDoctor.SelectedItem = Globals.AllStaffs.Where(x => x.StaffID == aSmallProcedureObj.CheckRecordDoctorStaffID).FirstOrDefault();
            SmallProcedureDateTime.DateTime = aSmallProcedureObj.ProcedureDateTime;
            SmallProcedureCompleteDateTime.DateTime = aSmallProcedureObj.CompletedDateTime;
            SmallProcedureDateOffStitches.DateTime = aSmallProcedureObj.DateOffStitches;
            if (aSmallProcedureObj.BeforeICD10 != null)
            {
                if (aSmallProcedureObj.BeforeICD10.ICD10Code != "")
                {
                    LoadRefDiseases(aSmallProcedureObj.BeforeICD10.ICD10Code.Trim(), 0, 0, 100, true);
                }
                BeforeDiagTreatment = aSmallProcedureObj.BeforeICD10.DiseaseNameVN;
            }
            if (aSmallProcedureObj.AfterICD10 != null)
            {
                if(aSmallProcedureObj.AfterICD10.ICD10Code != "")
                {
                    LoadRefDiseases(aSmallProcedureObj.AfterICD10.ICD10Code.Trim(), 0, 0, 100, false);
                }
                AfterDiagTreatment = aSmallProcedureObj.AfterICD10.DiseaseNameVN;
            }
            SmallProcedureObj = aSmallProcedureObj;
            //20200218 TBL: Nếu được gọi từ Xác nhận chẩn đoán thì không cần set lại BackupSmallProcedure vì trường hợp chỉ xác nhận lại chẩn đoán mà không thay đổi gì sẽ không cho lưu
            if (!IsConfirm)
            {
                BackupSmallProcedure = aSmallProcedureObj.DeepCopy();
            }
            //▼===== #006: Set ở đây để khi lưu hoặc cập nhật dữ liệu của 1 ca thủ thuật có sử dụng nút tạo mới và tạo mới dựa trên cũ. Thì nút bỏ qua sẽ không còn nữa.
            IsVisibilitySkip = false;
            //▲=====

            //▼===== #012
            if (aSmallProcedureObj.PtRegDetailID != 0)
            {
                GetResourcesForMedicalServicesListByDeptIDAndTypeID(Globals.DeptLocation.DeptID, aSmallProcedureObj.PtRegDetailID);
            }
            if (aSmallProcedureObj.SmallProcedureID != 0)
            {
                DiagnosisICD9Items_Load(aSmallProcedureObj.SmallProcedureID);
                GetResourcesForMedicalServicesListBySmallProcedureID(aSmallProcedureObj.SmallProcedureID);
            }
            //▲===== #012
        }

        public SmallProcedure UpdatedSmallProcedure
        {
            get
            {
                if (BackupSmallProcedure == null || SmallProcedureObj == null)
                {
                    return null;
                }
                if (cboProcedureDoctor.SelectedItem == null)
                {
                    SmallProcedureObj.ProcedureDoctorStaffID = null;
                }
                if (cboProcedureDoctor2.SelectedItem == null)
                {
                    SmallProcedureObj.ProcedureDoctorStaffID2 = null;
                }
                if (cboNarcoticDoctor.SelectedItem == null)
                {
                    SmallProcedureObj.NarcoticDoctorStaffID = null;
                }
                //▼===== #010
                if (cboNarcoticDoctorOfficial.SelectedItem == null)
                {
                    SmallProcedureObj.NarcoticDoctorOfficialStaffID = null;
                }
                //▲===== #010
                if (cboNarcoticDoctor2.SelectedItem == null)
                {
                    SmallProcedureObj.NarcoticDoctorStaffID2 = null;
                }
                if (cboCheckRecordDoctor.SelectedItem == null)
                {
                    SmallProcedureObj.CheckRecordDoctorStaffID = null;
                }
                if (cboNurse.SelectedItem == null)
                {
                    SmallProcedureObj.NurseStaffID = null;
                }
                if (cboNurse2.SelectedItem == null)
                {
                    SmallProcedureObj.NurseStaffID2 = null;
                }
                if (cboNurse3.SelectedItem == null)
                {
                    SmallProcedureObj.NurseStaffID3 = null;
                }
                SmallProcedureObj.ProcedureDateTime = SmallProcedureDateTime.DateTime.GetValueOrDefault(DateTime.MinValue);
                SmallProcedureObj.CompletedDateTime = SmallProcedureCompleteDateTime.DateTime.GetValueOrDefault(DateTime.MinValue);
                if (SmallProcedureDateOffStitches.DateTime.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue)
                {
                    SmallProcedureObj.DateOffStitches = null;
                }
                else
                {
                    SmallProcedureObj.DateOffStitches = SmallProcedureDateOffStitches.DateTime.GetValueOrDefault(DateTime.MinValue);
                }
                if (BackupSmallProcedure.ProcedureDateTime == SmallProcedureObj.ProcedureDateTime
                    //&& BackupSmallProcedure.Diagnosis == SmallProcedureObj.Diagnosis
                    && BackupSmallProcedure.ProcedureMethod == SmallProcedureObj.ProcedureMethod
                    && BackupSmallProcedure.NarcoticMethod == SmallProcedureObj.NarcoticMethod
                    && BackupSmallProcedure.ProcedureDoctorStaffID == SmallProcedureObj.ProcedureDoctorStaffID
                    && BackupSmallProcedure.ProcedureDoctorStaffID2 == SmallProcedureObj.ProcedureDoctorStaffID2
                    && BackupSmallProcedure.NarcoticDoctorStaffID == SmallProcedureObj.NarcoticDoctorStaffID
                    //▼====== #010
                    && BackupSmallProcedure.NarcoticDoctorOfficialStaffID == SmallProcedureObj.NarcoticDoctorOfficialStaffID
                    //▲====== #010
                    && BackupSmallProcedure.NarcoticDoctorStaffID2 == SmallProcedureObj.NarcoticDoctorStaffID2
                    && BackupSmallProcedure.NurseStaffID == SmallProcedureObj.NurseStaffID
                    && BackupSmallProcedure.NurseStaffID2 == SmallProcedureObj.NurseStaffID2
                    && BackupSmallProcedure.NurseStaffID3 == SmallProcedureObj.NurseStaffID3
                    && BackupSmallProcedure.CheckRecordDoctorStaffID == SmallProcedureObj.CheckRecordDoctorStaffID
                    && ((BackupSmallProcedure.BeforeICD10 != null && BackupSmallProcedure.BeforeICD10.ICD10Code == SmallProcedureObj.BeforeICD10.ICD10Code
                    && BackupSmallProcedure.BeforeICD10.DiseaseNameVN == SmallProcedureObj.BeforeICD10.DiseaseNameVN) || BackupSmallProcedure.BeforeICD10 == null)
                    && ((BackupSmallProcedure.AfterICD10 != null && BackupSmallProcedure.AfterICD10.ICD10Code == SmallProcedureObj.AfterICD10.ICD10Code
                    && BackupSmallProcedure.AfterICD10.DiseaseNameVN == SmallProcedureObj.AfterICD10.DiseaseNameVN) || BackupSmallProcedure.AfterICD10 == null)
                    && BackupSmallProcedure.Notes == SmallProcedureObj.Notes
                    && BackupSmallProcedure.DateOffStitches == SmallProcedureObj.DateOffStitches
                    && BackupSmallProcedure.CompletedDateTime == SmallProcedureObj.CompletedDateTime
                    && BackupSmallProcedure.Drainage == SmallProcedureObj.Drainage
                    && BackupSmallProcedure.V_DeathReason == SmallProcedureObj.V_DeathReason
                    && BackupSmallProcedure.V_SurgicalMode == SmallProcedureObj.V_SurgicalMode
                    && BackupSmallProcedure.V_CatactropheType == SmallProcedureObj.V_CatactropheType
                    && BackupSmallProcedure.V_AnesthesiaType == SmallProcedureObj.V_AnesthesiaType
                    && BackupSmallProcedure.TrinhTu == SmallProcedureObj.TrinhTu
                    && BackupSmallProcedure.V_SurgicalSite == SmallProcedureObj.V_SurgicalSite
                    && Compare2ICD9IsEqualForSave()
                    && CompareResource())
                {
                    return null;
                }
                return SmallProcedureObj;
            }
        }

        public SmallProcedure SmallProcedure_InPt
        {
            get
            {
                //▼====== #003
                //20200206 TTM: Bổ sung thêm điều kiện kiểm tra vì nếu chưa tìm bệnh nhân nào mà bấm vào xem tường trình thì chết chương trình
                //              Do biến SmallProcedureObj = null.
                if (SmallProcedureObj != null)
                {
                    SmallProcedureObj.ProcedureDateTime = SmallProcedureDateTime.DateTime.GetValueOrDefault(DateTime.MinValue);
                    SmallProcedureObj.CompletedDateTime = SmallProcedureCompleteDateTime.DateTime.GetValueOrDefault(DateTime.MinValue);
                }
                //▲====== #003
                return SmallProcedureObj;
            }
        }

        public void GetSmallProcedure(long PtRegDetailID, long V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU)
        {
            ClearCBB();
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetSmallProcedure(PtRegDetailID, null, V_RegistrationType
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var mSmallProcedure = contract.EndGetSmallProcedure(asyncResult);
                                IsVisibility = false;
                                IsVisibilitySkip = false;
                                FormEditorIsEnabled = true;
                                if (mSmallProcedure == null)
                                {
                                    mSmallProcedure = new SmallProcedure();
                                }
                                if (mSmallProcedure.PtRegDetailID == 0)
                                {
                                    mSmallProcedure.PtRegDetailID = PtRegDetailID;
                                }
                                ApplySmallProcedure(mSmallProcedure);
                                //▼===== #005
                                if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NOI_TRU)
                                {
                                    Globals.EventAggregator.Publish(new LoadDataForHtmlEditor() { SmallProcedure = mSmallProcedure });
                                }
                                //▲===== #005
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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

        public void GetLatesSmallProcedure(long PatientID, long MedServiceID, long V_RegistrationType)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetLatesSmallProcedure(PatientID, MedServiceID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var mSmallProcedure = contract.EndGetLatesSmallProcedure(asyncResult);
                            IsVisibility = false;
                            IsVisibilitySkip = false;
                            if (mSmallProcedure == null)
                            {
                                mSmallProcedure = new SmallProcedure();
                                FormEditorIsEnabled = true;
                                GetSmallProcedureTime(MedServiceID);
                            }
                            else
                            {
                                IsVisibility = true;
                                FormEditorIsEnabled = false;
                            }
                            mSmallProcedure.V_RegistrationType = V_RegistrationType;
                            ApplySmallProcedure(mSmallProcedure);
                            //▼===== #005
                            if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NOI_TRU)
                            {
                                Globals.EventAggregator.Publish(new LoadDataForHtmlEditor() { SmallProcedure = mSmallProcedure });
                            }
                            //▲===== #005
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
        public void GetSmallProcedureTime(long MedServiceID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetSmallProcedureTime( MedServiceID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int SmallProcedureTime = contract.EndGetSmallProcedureTime(asyncResult);
                            if (SmallProcedureTime > 0)
                            {
                                SmallProcedureObj.CompletedDateTime = SmallProcedureObj.ProcedureDateTime.AddMinutes(SmallProcedureTime);
                                SmallProcedureCompleteDateTime.DateTime = SmallProcedureObj.CompletedDateTime;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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

        public void ClearCBB()
        {
            cboProcedureDoctor.Text = "";
            cboProcedureDoctor2.Text = "";
            cboCheckRecordDoctor.Text = "";
            cboNarcoticDoctor.Text = "";
            //▼====== #010
            cboNarcoticDoctorOfficial.Text = "";
            //▲===== #010
            cboNarcoticDoctor2.Text = "";
            cboNurse.Text = "";
            cboNurse2.Text = "";
            cboNurse3.Text = "";
            BeforeDiagTreatment = "";
            AfterDiagTreatment = "";
            SelectedAfterICD10 = new DiseasesReference();
            SelectedBeforeICD10 = new DiseasesReference();
            ObjNoteTemplates_Selected = new PrescriptionNoteTemplates();
        }

        //▼====== #001
        public void btnCreateNew()
        {
            FormEditorIsEnabled = true;
            IsVisibilitySkip = true;
            IsVisibility = false;
            SmallProcedureObjBackup = BackupSmallProcedure.DeepCopy();
            SmallProcedureObj = new SmallProcedure();
            ClearCBB();
            SmallProcedureObj.PtRegDetailID = SmallProcedureObjBackup.PtRegDetailID;
            SmallProcedureObj.ProcedureDateTime = Globals.GetCurServerDateTime();            
            SmallProcedureDateTime.DateTime = SmallProcedureObj.ProcedureDateTime;
            SmallProcedureObj.DateOffStitches = null;
            SmallProcedureDateOffStitches.DateTime = SmallProcedureObj.DateOffStitches;
            SmallProcedureObj.CompletedDateTime = Globals.GetCurServerDateTime();
            SmallProcedureCompleteDateTime.DateTime = SmallProcedureObj.CompletedDateTime;
            SmallProcedureObj.Diagnosis = SmallProcedureObjBackup.Diagnosis;
            //▼===== #006
            if (!IsFromOutOrInDiag)
            {
                SmallProcedureObj.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
            }
            else
            {
                SmallProcedureObj.V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
            }
            //▲===== #006
        }

        public void btnCreateOld()
        {
            FormEditorIsEnabled = true;
            IsVisibilitySkip = true;
            IsVisibility = false;
            SmallProcedureObjBackup = BackupSmallProcedure.DeepCopy();
            SmallProcedureObj.SmallProcedureID = 0;
            SmallProcedureObj.ProcedureDateTime = Globals.GetCurServerDateTime();
            SmallProcedureDateTime.DateTime = SmallProcedureObj.ProcedureDateTime;
            SmallProcedureObj.DateOffStitches = null;
            SmallProcedureDateOffStitches.DateTime = SmallProcedureObj.DateOffStitches;
            SmallProcedureObj.CompletedDateTime = Globals.GetCurServerDateTime();
            SmallProcedureCompleteDateTime.DateTime = SmallProcedureObj.CompletedDateTime;
            //▼===== #006
            if (!IsFromOutOrInDiag)
            {
                SmallProcedureObj.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
            }
            else
            {
                SmallProcedureObj.V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
            }
            //▲===== #006
        }

        public void btnSkip()
        {
            FormEditorIsEnabled = false;
            IsVisibilitySkip = false;
            IsVisibility = true;
            SmallProcedureObj = SmallProcedureObjBackup;
            ApplySmallProcedure(SmallProcedureObj);
        }
        //▲====== #001

        //▼====== #002
        public void LoadBeforeRefDiseases(string name, byte type, int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchRefDiseases(name, PageIndex, PageSize, type
                        , 0
                        , Globals.GetCurServerDateTime()
                        , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndSearchRefDiseases(out int Total, asyncResult);
                            BeforeRefIDC10Code.Clear();
                            if (results != null)
                            {
                                foreach (DiseasesReference p in results)
                                {
                                    BeforeRefIDC10Code.Add(p);
                                }
                            }
                            BeforeAcb_ICD10_Code.ItemsSource = BeforeRefIDC10Code;
                            BeforeAcb_ICD10_Code.PopulateComplete();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                        }
                    }), null);
                }
            });

            t.Start();
        }

        public void LoadAfterRefDiseases(string name, byte type, int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchRefDiseases(name, PageIndex, PageSize, type
                        , 0
                        , Globals.GetCurServerDateTime()
                        , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndSearchRefDiseases(out int Total, asyncResult);
                            AfterRefIDC10Code.Clear();
                            if (results != null)
                            {
                                foreach (DiseasesReference p in results)
                                {
                                    AfterRefIDC10Code.Add(p);
                                }
                            }
                            AfterAcb_ICD10_Code.ItemsSource = AfterRefIDC10Code;
                            AfterAcb_ICD10_Code.PopulateComplete();
                            
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                        }
                    }), null);
                }
            });

            t.Start();
        }

        public void GetLookupAnesthesiaTypes()
        {
            RefAnesthesiaTypes = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.V_AnesthesiaType))
                {
                    RefAnesthesiaTypes.Add(tmpLookup);
                }
            }
        }

        public void GetLookupDeathReasons()
        {
            RefDeathReasons = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.V_DeadReason))
                {
                    RefDeathReasons.Add(tmpLookup);
                }
            }
        }

        public void GetLookupSurgicalModes()
        {
            RefSurgicalModes = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.V_SurgicalMode))
                {
                    RefSurgicalModes.Add(tmpLookup);
                }
            }
        }

        public void GetLookupCatactropheTypes()
        {
            RefCatactropheTypes = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.V_CatactropheType))
                {
                    RefCatactropheTypes.Add(tmpLookup);
                }
            }
        }

        public void LoadRefDiseases(string name, byte type, int PageIndex, int PageSize, bool isBefore)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchRefDiseases(name, PageIndex, PageSize, type
                        , 0
                        , Globals.GetCurServerDateTime()
                        , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndSearchRefDiseases(out int Total, asyncResult);
                            AfterRefIDC10Code.Clear();
                            if (results != null && results.Count() > 0)
                            {
                                if (isBefore)
                                {
                                    SelectedBeforeICD10 = results[0];
                                    BeforeDiagTreatment = SelectedBeforeICD10.DiseaseNameVN;
                                }
                                else
                                {
                                    SelectedAfterICD10 = results[0];
                                    AfterDiagTreatment = SelectedAfterICD10.DiseaseNameVN;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                        }
                    }), null);
                }
            });

            t.Start();
        }
        
        //▲====== #002
        private bool _bConfirmVisi = true;
        public bool bConfirmVisi
        {
            get { return _bConfirmVisi; }
            set
            {
                if (_bConfirmVisi != value)
                {
                    _bConfirmVisi = value;
                    NotifyOfPropertyChange(() => bConfirmVisi);
                }
            }
        }
        private bool _IsConfirm;
        public bool IsConfirm
        {
            get { return _IsConfirm; }
            set
            {
                if (_IsConfirm != value)
                {
                    _IsConfirm = value;
                    NotifyOfPropertyChange(() => IsConfirm);
                }
            }
        }
        public void btnConfirmDiagnosisTreatment()
        {
            if (Registration_DataStorage == null || Registration_DataStorage.CurrentPatientRegistration == null || Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID == 0)
            {
                return; 
            }
            IsConfirm = true;
            IConfirmDiagnosisTreatmentForSmallProcedure DialogView = Globals.GetViewModel<IConfirmDiagnosisTreatmentForSmallProcedure>();
            DialogView.Registration_DataStorage = Registration_DataStorage;
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, new Size(1600, 600));
            if (DialogView.SmallProcedureObj != null)
            {
                SmallProcedureObj.Diagnosis = DialogView.SmallProcedureObj.Diagnosis;
                SmallProcedureObj.BeforeICD10 = DialogView.SmallProcedureObj.BeforeICD10;
                SmallProcedureObj.ServiceRecID = DialogView.SmallProcedureObj.ServiceRecID;
                ApplySmallProcedure(SmallProcedureObj);
            }
            IsConfirm = false;
        }
        #endregion
        private bool _IsFromOutOrInDiag = false;
        public bool IsFromOutOrInDiag
        {
            get { return _IsFromOutOrInDiag; }
            set
            {
                if (_IsFromOutOrInDiag != value)
                {
                    _IsFromOutOrInDiag = value;
                    IsVisibleICD9 = !IsFromOutOrInDiag; //Globals.ServerConfigSection.CommonItems.ApplyReport130 &&
                    NotifyOfPropertyChange(() => IsFromOutOrInDiag);
                }
            }
        }

        //▼====== #011
        #region ICD9 control 
        private bool _IsVisibleICD9;
        public bool IsVisibleICD9
        {
            get
            {
                return _IsVisibleICD9;
            }
            set
            {
                if (_IsVisibleICD9 == value)
                {
                    return;
                }
                _IsVisibleICD9 = value;
                NotifyOfPropertyChange(() => IsVisibleICD9);
            }
        }
        private void PageIDC9_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            SearchICD9(procName, ICD9SearchType, pageIDC9.PageIndex, pageIDC9.PageSize);
        }
        bool IsICD9Code = true;

        private DiagnosisICD9Items _refICD9Item;
        public DiagnosisICD9Items refICD9Item
        {
            get
            {
                return _refICD9Item;
            }
            set
            {
                if (_refICD9Item != value)
                {
                    _refICD9Item = value;
                }
                NotifyOfPropertyChange(() => refICD9Item);
            }
        }
        private DiagnosisTreatment _DiagTrmtItem;
        public DiagnosisTreatment DiagTrmtItem
        {
            get
            {
                return _DiagTrmtItem;
            }
            set
            {
                _DiagTrmtItem = value;
                //SetDepartment();
                NotifyOfPropertyChange(() => DiagTrmtItem);
                //NotifyOfPropertyChange(() => btCreateNewByOldIsEnabled);
            }
        }

        private ObservableCollection<DiagnosisICD9Items> _refICD9List;
        public ObservableCollection<DiagnosisICD9Items> refICD9List
        {
            get
            {
                return _refICD9List;
            }
            set
            {
                if (_refICD9List != value)
                {
                    _refICD9List = value;
                }
                NotifyOfPropertyChange(() => refICD9List);
            }
        }

        private void AddICD9BlankRow()
        {
            if (refICD9List != null
                && refICD9List.LastOrDefault() != null
                && refICD9List.LastOrDefault().RefICD9 == null)
            {
                return;
            }
            DiagnosisICD9Items ite = new DiagnosisICD9Items();
            refICD9List.Add(ite);
        }

        private RefICD9 RefICD9Copy = null;
        private string TreatmentOld = "";
        private string TreatmentNew = "";

        int ColumnIndex = 0;

        AxDataGridNyICD10 grdICD9 { get; set; }
        public void grdICD9_Loaded(object sender, RoutedEventArgs e)
        {
            grdICD9 = sender as AxDataGridNyICD10;
        }

        public void grdICD9_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            ColumnIndex = e.Column.DisplayIndex;

            if (refICD9Item != null)
            {
                RefICD9Copy = refICD9Item.RefICD9.DeepCopy();
            }
            if (e.Column.DisplayIndex == 0)
            {
                IsICD9Code = true;
            }
            else
            {
                IsICD9Code = false;
            }
        }


        public void grdICD9_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DiagnosisICD9Items item = e.Row.DataContext as DiagnosisICD9Items;

            if (ColumnIndex == 0 || ColumnIndex == 1)
            {
                if (refICD9Item.RefICD9 == null)
                {
                    if (RefICD9Copy != null)
                    {
                        refICD9Item.RefICD9 = ObjectCopier.DeepCopy(RefICD9Copy);
                        if (CheckExistsICD9(refICD9Item, false))
                        {
                            GetTreatment(refICD9Item.RefICD9);
                        }
                    }
                }
            }
            if (refICD9Item != null && refICD9Item.RefICD9 != null)
            {
                if (CheckExistsICD9(refICD9Item, false))
                {
                    if (e.Row.GetIndex() == (refICD9List.Count - 1) && e.EditAction == DataGridEditAction.Commit)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() => AddICD9BlankRow());
                    }
                }
            }

        }


        private bool CheckExistsICD9(DiagnosisICD9Items Item, bool HasMessage = true)
        {
            int i = 0;
            if (Item.RefICD9 == null)
            {
                return true;
            }
            foreach (DiagnosisICD9Items p in refICD9List)
            {
                if (p.RefICD9 != null)
                {
                    if (Item.RefICD9.ICD9Code == p.RefICD9.ICD9Code)
                    {
                        i++;
                    }
                }
            }
            if (i > 1)
            {
                Item.RefICD9 = null;
                if (HasMessage)
                {
                    //Remind change the message
                    MessageBox.Show(eHCMSResources.Z1909_G1_ICD9DaTonTai);
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public void GetTreatment(RefICD9 refICD9)
        {
            if (refICD9 != null)
            {
                TreatmentNew = refICD9.ProcedureName;
                if (TreatmentOld != "")
                {
                    DiagTrmtItem.Treatment = DiagTrmtItem.Treatment.Replace(TreatmentOld, TreatmentNew);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(DiagTrmtItem.Treatment))
                    {
                        DiagTrmtItem.Treatment += TreatmentNew;
                    }
                    else
                    {
                        DiagTrmtItem.Treatment += "- " + TreatmentNew;
                    }
                }
                TreatmentOld = ObjectCopier.DeepCopy(TreatmentNew);
            }

        }

        public void grdICD9_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DiagnosisICD9Items item = ((DataGrid)sender).SelectedItem as DiagnosisICD9Items;
            if (item != null && item.RefICD9 != null)
            {
                RefICD9Copy = item.RefICD9;
                TreatmentNew = TreatmentOld = ObjectCopier.DeepCopy(item.RefICD9.ProcedureName);
                RefICD9Copy = ObjectCopier.DeepCopy(item.RefICD9);
            }
            else
            {
                TreatmentNew = TreatmentOld = "";
                RefICD9Copy = null;
            }
        }

        public void lnkDeleteICD9_Click(object sender, RoutedEventArgs e)
        {
            if (refICD9Item == null
                || refICD9Item.RefICD9 == null)
            {
                MessageBox.Show(eHCMSResources.Z0405_G1_KgTheXoaDongRong);
                return;
            }

            int nSelIndex = grdICD9.SelectedIndex;
            if (nSelIndex >= refICD9List.Count - 1)
            {
                MessageBox.Show(eHCMSResources.Z0405_G1_KgTheXoaDongRong);
                return;
            }

            var item = refICD9List[nSelIndex];

            if (item != null && item.ICD9Code != null && item.ICD9Code != "")
            {
                if (MessageBox.Show(eHCMSResources.Z1910_G1_BanMuonXoaICD9, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (refICD9Item.RefICD9.ProcedureName != "")
                    {
                        DiagTrmtItem.Treatment = DiagTrmtItem.Treatment.Replace(refICD9Item.RefICD9.ProcedureName, "");
                    }
                    refICD9List.Remove(refICD9List[nSelIndex]);
                }
            }
        }


        #endregion

        #region Autocomplete ICD9-Code

        AutoCompleteBox AutoICD9Code;

        private PagedSortableCollectionView<RefICD9> _pageIDC9;
        public PagedSortableCollectionView<RefICD9> pageIDC9
        {
            get
            {
                return _pageIDC9;
            }
            set
            {
                if (_pageIDC9 != value)
                {
                    _pageIDC9 = value;
                }
                NotifyOfPropertyChange(() => pageIDC9);
            }
        }

        public void aucICD9_Populating(object sender, PopulatingEventArgs e)
        {
            if (IsICD9Code)
            {
                e.Cancel = true;
                procName = e.Parameter;
                ICD9SearchType = 0;
                pageIDC9.PageIndex = 0;
                SearchICD9(e.Parameter, 0, 0, pageIDC9.PageSize);
            }
        }

        public void aucICD9_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!isDropDownICD9)
            {
                return;
            }
            isDropDownICD9 = false;
            if (refICD9Item != null)
            {
                refICD9Item.RefICD9 = ((AutoCompleteBox)sender).SelectedItem as RefICD9;
                if (CheckIsMainForICD9())
                {
                    refICD9Item.IsMain = true;
                }
                if (CheckExistsICD9(refICD9Item))
                {
                    GetTreatment(refICD9Item.RefICD9);
                }
            }
        }

        private bool isDropDownICD9 = false;
        public void aucICD9_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            isDropDownICD9 = true;
        }

        public void SearchICD9(string name, byte type, int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSearchRefICD9(name, PageIndex, PageSize, type, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 10;
                            var results = contract.EndSearchRefICD9(out Total, asyncResult);
                            pageIDC9.Clear();
                            pageIDC9.TotalItemCount = Total;
                            if (results != null)
                            {
                                foreach (RefICD9 p in results)
                                {
                                    pageIDC9.Add(p);
                                }
                            }
                            if (ICD9SearchType == 0)
                            {
                                Acb_ICD9_Code.ItemsSource = pageIDC9;
                                Acb_ICD9_Code.PopulateComplete();
                            }
                            else
                            {
                                Acb_ICD9_Name.ItemsSource = pageIDC9;
                                Acb_ICD9_Name.PopulateComplete();
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {

                        }

                    }), null);

                }

            });

            t.Start();
        }



        #endregion

        #region ICD 9 - Procedure Name

        //AutoCompleteBox auProcedureName;
        private string procName = "";
        private byte ICD9SearchType = 0;
        //public void ProcedureName_Loaded(object sender, RoutedEventArgs e)
        //{
        //    auProcedureName = (AutoCompleteBox)sender;
        //}

        public void ProcedureName_Populating(object sender, PopulatingEventArgs e)
        {
            if (!IsICD9Code && ColumnIndex == 1)
            {
                e.Cancel = true;
                procName = e.Parameter;
                ICD9SearchType = 1;
                pageIDC9.PageIndex = 0;
                SearchICD9(e.Parameter, 1, 0, pageIDC9.PageSize);
            }
        }

        private bool isICD9DropDown = false;
        public void ProcedureName_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!isICD9DropDown)
            {
                return;
            }
            isICD9DropDown = false;
            refICD9Item.RefICD9 = ((AutoCompleteBox)sender).SelectedItem as RefICD9;
            if (CheckIsMainForICD9())
            {
                refICD9Item.IsMain = true;
            }
            if (CheckExistsICD9(refICD9Item))
            {
                GetTreatment(refICD9Item.RefICD9);
            }
        }

        public void ProcedureName_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            isICD9DropDown = true;
        }
        AutoCompleteBox Acb_ICD9_Code = null;
        public void aucICD9_Code_Loaded(object sender, RoutedEventArgs e)
        {
            Acb_ICD9_Code = (AutoCompleteBox)sender;
        }
        AutoCompleteBox Acb_ICD9_Name = null;
        public void aucICD9_Name_Loaded(object sender, RoutedEventArgs e)
        {
            Acb_ICD9_Name = (AutoCompleteBox)sender;
        }
        private bool CheckIsMainForICD9()
        {
            ObservableCollection<DiagnosisICD9Items> temp = refICD9List.Where(x => x.ICD9Code != null).ToObservableCollection();
            if (temp != null && temp.Count > 0)
            {
                int bcount = 0;
                for (int i = 0; i < temp.Count; i++)
                {
                    if (temp[i].IsMain)
                    {
                        bcount++;
                    }
                }
                if (bcount == 0)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
        //▲====== #011

        //▼====== #012
        private ObservableCollection<Lookup> _RefSurgicalSite;
        public ObservableCollection<Lookup> RefSurgicalSite
        {
            get
            {
                return _RefSurgicalSite;
            }
            set
            {
                if (_RefSurgicalSite != value)
                {
                    _RefSurgicalSite = value;
                }
                NotifyOfPropertyChange(() => RefSurgicalSite);
            }
        }

        private ObservableCollection<DiagnosisICD9Items> _refICD9ListCopy = new ObservableCollection<DiagnosisICD9Items>();
        public ObservableCollection<DiagnosisICD9Items> refICD9ListCopy
        {
            get
            {
                return _refICD9ListCopy;
            }
            set
            {
                if (_refICD9ListCopy != value)
                {
                    _refICD9ListCopy = value;
                }
                NotifyOfPropertyChange(() => refICD9ListCopy);
            }
        }

        private bool ICD9Equal(DiagnosisICD9Items a, DiagnosisICD9Items b)
        {
            return a.DiagICD9ItemID == b.DiagICD9ItemID
                && a.DiagnosisICD9ListID == b.DiagnosisICD9ListID
                && a.ICD9Code == b.ICD9Code
                && a.IsMain == b.IsMain
                && a.IsCongenital == b.IsCongenital;
        }

        public long Compare2ICD9List()
        {
            long ListID = 0;
            ObservableCollection<DiagnosisICD9Items> temp = refICD9List.Where(x => x.RefICD9 != null).ToObservableCollection();
            if (refICD9ListCopy != null && refICD9ListCopy.Count > 0 && refICD9ListCopy.Count == temp.Count)
            {
                int icount = 0;
                for (int i = 0; i < refICD9ListCopy.Count; i++)
                {
                    for (int j = 0; j < temp.Count; j++)
                    {
                        if (ICD9Equal(refICD9ListCopy[i], refICD9List[j]))
                        {
                            icount++;
                        }
                    }
                }
                if (icount == refICD9ListCopy.Count)
                {
                    ListID = refICD9ListCopy.FirstOrDefault().DiagnosisICD9ListID;
                    return ListID;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        private bool Compare2ICD9IsEqualForSave()
        {
            bool equal = true;
            ObservableCollection<DiagnosisICD9Items> temp = refICD9List.Where(x => x.RefICD9 != null).ToObservableCollection();
            if ((refICD9ListCopy != null && refICD9ListCopy.Count > 0 && refICD9ListCopy.Count != temp.Count)
                || (refICD9ListCopy.Count == 0 && temp.Count > 0))
            {
                return false;
            }
            else
            {
                for (int i = 0; i < temp.Count; i++)
                {
                    if (!ICD9Equal(refICD9ListCopy[i], temp[i]))
                    {
                        equal = false;
                        break;
                    }
                }
                return equal;
            }
        }

        private bool ResourceEqual(Resources a, Resources b)
        {
            return a.RscrID == b.RscrID
                && a.HIRepResourceCode == b.HIRepResourceCode
                && a.ItemName == b.ItemName
                && a.ItemBrand == b.ItemBrand;
        }

        public bool CompareResource()
        {
            bool equal = true;
            if ((SelectedResourceListCopy != null && SelectedResourceListCopy.Count > 0 && SelectedResourceListCopy.Count != SelectedResourceList.Count) 
                || (SelectedResourceListCopy.Count == 0 && SelectedResourceList.Count > 0))
            {
                return false;
            }
            else
            {
                for (int i = 0; i < SelectedResourceList.Count; i++)
                {
                    if (!ResourceEqual(SelectedResourceListCopy[i], SelectedResourceList[i]))
                    {
                        equal = false;
                        break;
                    }
                }
                return equal;
            }
        }

        public void DiagnosisICD9Items_Load(long DTItemID)
        {
            if (DTItemID <= 0)
            {
                return;
            }

            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetDiagnosisICD9Items_Load(DTItemID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisICD9Items_Load(asyncResult);
                            refICD9List = results.ToObservableCollection();
                            refICD9ListCopy = refICD9List.DeepCopy();
                            AddICD9BlankRow();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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

        public void lnkDeleteHIRepResourceCode_Click(object datacontext)
        {
            Resources SelectedItem = datacontext as Resources;
            if(SelectedItem == null)
            {
                return;
            }
            if (SelectedResourceList != null && SelectedResourceList.Contains(SelectedItem))
            {
                SelectedResourceList.Remove(SelectedItem);
            }
        }

        AxComboBox Cbo;
        public void cboHiRepResourceCode_Loaded(object sender, SelectionChangedEventArgs e)
        {
            Cbo = sender as AxComboBox;
            Cbo.SelectedIndex = -1;
        }
        public void cboHiRepResourceCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem == null)
            {
                return;
            }
            SelectedResource = (sender as ComboBox).SelectedItem as Resources;
            if(SelectedResource != null)
            {
                if(SelectedResourceList != null && SelectedResourceList.Contains(SelectedResource))
                {
                    return;
                }
                else
                {
                    SelectedResourceList.Add(SelectedResource);
                }
            }
        }

        private ObservableCollection<Resources> _ResourceList;
        public ObservableCollection<Resources> ResourceList
        {
            get { return _ResourceList; }
            set
            {
                if (ResourceList != value)
                {
                    _ResourceList = value;
                }
                NotifyOfPropertyChange(() => ResourceList);
            }
        }

        private ObservableCollection<Resources> _SelectedResourceListCopy = new ObservableCollection<Resources>();
        public ObservableCollection<Resources> SelectedResourceListCopy
        {
            get { return _SelectedResourceListCopy; }
            set
            {
                if (SelectedResourceListCopy != value)
                {
                    _SelectedResourceListCopy = value;
                }
                NotifyOfPropertyChange(() => SelectedResourceListCopy);
            }
        }

        private Resources _SelectedResource;
        public Resources SelectedResource
        {
            get { return _SelectedResource; }
            set
            {
                if (_SelectedResource != value)
                {
                    _SelectedResource = value;
                }
                NotifyOfPropertyChange(() => SelectedResource);
            }
        }

        private ObservableCollection<Resources> _SelectedResourceList = new ObservableCollection<Resources>();
        public ObservableCollection<Resources> SelectedResourceList
        {
            get { return _SelectedResourceList; }
            set
            {
                if (SelectedResourceList != value)
                {
                    _SelectedResourceList = value;
                }
                NotifyOfPropertyChange(() => SelectedResourceList);
            }
        }

        AxDataGridNyICD10 grdHIRepResourceCode { get; set; }
        public void grdHIRepResourceCode_Loaded(object sender, RoutedEventArgs e)
        {
            grdHIRepResourceCode = sender as AxDataGridNyICD10;
        }

        public void GetResourcesForMedicalServicesListByDeptIDAndTypeID(long DeptID, long PtRegDetailID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetResourcesForMedicalServicesListByDeptIDAndTypeID(DeptID, PtRegDetailID
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                List<Resources> results = contract.EndGetResourcesForMedicalServicesListByDeptIDAndTypeID(asyncResult);
                                if (results != null)
                                {
                                    if (ResourceList == null)
                                    {
                                        ResourceList = new ObservableCollection<Resources>();
                                    }
                                    else
                                    {
                                        ResourceList.Clear();
                                    }
                                    ResourceList = results.ToObservableCollection();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
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

        public void GetResourcesForMedicalServicesListBySmallProcedureID(long SmallProcedureID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetResourcesForMedicalServicesListBySmallProcedureID(SmallProcedureID
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                List<Resources> results = contract.EndGetResourcesForMedicalServicesListBySmallProcedureID(asyncResult);
                                if (results != null)
                                {
                                    if (SelectedResourceList == null)
                                    {
                                        SelectedResourceList = new ObservableCollection<Resources>();
                                    }
                                    else
                                    {
                                        SelectedResourceList.Clear();
                                    }
                                    SelectedResourceList = results.ToObservableCollection();
                                    SelectedResourceListCopy = results.ToObservableCollection();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
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
        //▲====== #012
    }
}