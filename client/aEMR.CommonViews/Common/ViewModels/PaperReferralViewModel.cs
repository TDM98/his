using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using DataEntities;
using System.Linq;
using eHCMSLanguage;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.DataContracts;
using aEMR.CommonTasks;
using aEMR.Common.DataValidation;
using aEMR.Controls;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
/*
 * 20180830 #001 TTM: Fixed cannot show information on dialog paperreferal full on show new
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPaperReferral)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PaperReferralViewModel : Conductor<object>, IPaperReferral
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public PaperReferralViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _eventArg = eventArg;
            authorization();
            var hospitalAutoCompleteVm = Globals.GetViewModel<IHospitalAutoCompleteListing>();
            hospitalAutoCompleteVm.IsPaperReferal = true;
            HospitalAutoCompleteContent = hospitalAutoCompleteVm;
            //HospitalAutoCompleteContent.setDisplayText("");
            HospitalAutoCompleteContent.InitBlankBindingValue();
            CreateNewInUse = false;
            Operation = FormOperationPaper.None;
            _eventArg.Subscribe(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            CreateNewInUse = false;
            authorization();
            _eventArg.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _eventArg.Unsubscribe(this);
        }

        private bool _formLocked;
        public bool FormLocked
        {
            get { return _formLocked; }
            set
            {
                _formLocked = value;
                NotifyOfPropertyChange(() => CanCreateNewPaperReferalCmd);
                NotifyOfPropertyChange(() => CanEditPaperReferalCmd);
                NotifyOfPropertyChange(() => CanUsePaperReferalCmd);
            }
        }

        private bool _IsChildWindow;
        public bool IsChildWindow
        {
            get { return _IsChildWindow; }
            set
            {
                if (_IsChildWindow != value)
                {
                    _IsChildWindow = value;
                }
                NotifyOfPropertyChange(() => IsChildWindow);
            }
        }

        /*TMA*/
        private int _PatientFindBy;
        public int PatientFindBy
        {
            get { return _PatientFindBy; }
            set
            {
                if (_PatientFindBy != value)
                {
                    _PatientFindBy = value;
                }
                NotifyOfPropertyChange(() => PatientFindBy);
            }
        }
        /*TMA*/
        private bool _IsSearchAll = false;
        public bool IsSearchAll
        {
            get { return _IsSearchAll; }
            set
            {
                if (_IsSearchAll != value)
                {
                    _IsSearchAll = value;
                    HospitalAutoCompleteContent.IsSearchAll = value;
                }
                NotifyOfPropertyChange(() => IsSearchAll);
            }
        }

        //public void chkSearchAll_Click(object sender, RoutedEventArgs e)
        //{
        //    HospitalAutoCompleteContent.IsSearchAll = IsSearchAll;
        //}


        private HealthInsurance _currentHiItem;
        public HealthInsurance CurrentHiItem
        {
            get
            {
                return _currentHiItem;
            }
            set
            {
                _currentHiItem = value;
                NotifyOfPropertyChange(() => CurrentHiItem);
                NotifyOfPropertyChange(() => PaperReferalTitle);
                ResetForm();
                IsSearchAll = false;
                GetAllReferrals();
            }
        }

/*tma*/
        private Patient _CurrentPatient;
        public Patient CurrentPatient
        {
            get
            {
                return _CurrentPatient;
            }
            set
            {
                _CurrentPatient = value;
                NotifyOfPropertyChange(() => _CurrentPatient);
                NotifyOfPropertyChange(() => CurrentPatient);
            }
        }

        private IPatientDetails _PatientDetailsContent;
        public IPatientDetails PatientDetailsContent
        {
            get
            {
                return _PatientDetailsContent;
            }
            set
            {
                _PatientDetailsContent = value;
                NotifyOfPropertyChange(() => _PatientDetailsContent);
                NotifyOfPropertyChange(() => PatientDetailsContent);
            }
        }
        //PatientRegistration PtRegistration { get; set; }
        private PatientRegistration _PtRegistration;
        public PatientRegistration PtRegistration
        {
            get
            {
                return _PtRegistration;
            }
            set
            {
                _PtRegistration = value;
                NotifyOfPropertyChange(() => _PtRegistration);
                NotifyOfPropertyChange(() => PtRegistration);
            }
        }
/*tma*/
        private IHospitalAutoCompleteListing _hospitalAutoCompleteContent;
        public IHospitalAutoCompleteListing HospitalAutoCompleteContent
        {
            get { return _hospitalAutoCompleteContent; }
            set
            {
                _hospitalAutoCompleteContent = value;
                NotifyOfPropertyChange(() => HospitalAutoCompleteContent);
            }
        }

        private PaperReferal _selectedPaperReferal;
        /// <summary>
        /// Thông tin giấy chuyển viện được chọn trong danh sách
        /// </summary>
        public PaperReferal SelectedPaperReferal
        {
            get
            {
                return _selectedPaperReferal;
            }
            set
            {
                _selectedPaperReferal = value;
                NotifyOfPropertyChange(() => SelectedPaperReferal);
                //EditingPaperReferal = ObjectCopier.DeepCopy(_selectedPaperReferal);
                if (Operation == FormOperationPaper.Edit && EditingPaperReferal != null)
                {
                    EditingPaperReferal.CancelEdit();
                }
                if (_selectedPaperReferal != null)
                {
                    Operation = FormOperationPaper.ReadOnly;
                }
                else
                {
                    Operation = FormOperationPaper.None;
                }
            }
        }
        private bool _isLoading = false;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(() => IsLoading);
            }
        }


        private bool _InUsed = true;
        public bool InUsed
        {
            get
            {
                return _InUsed;
            }
            set
            {
                if (_InUsed != value)
                {
                    CreateNewInUse = false;
                    _InUsed = value;
                    if (InUsed)
                    {
                        SelectedPaperReferal = null;
                        EditingPaperReferal = ObjectCopier.DeepCopy(PaperReferalInUse);
                    }
                    else
                    {
                        SelectedPaperReferal = PaperReferals != null && PaperReferals.Count > 0 ?
                            ObjectCopier.DeepCopy(PaperReferals.FirstOrDefault()) : null;
                        EditingPaperReferal = PaperReferals != null && PaperReferals.Count > 0 ?
                            ObjectCopier.DeepCopy(PaperReferals.FirstOrDefault()) : null;
                    }
                    Operation = FormOperationPaper.None;
                    NotifyOfPropertyChange(() => InUsed);
                    NotifyAll();
                }

            }
        }

        private bool _isSaving;
        public bool IsSaving
        {
            get { return _isSaving; }
            set
            {
                _isSaving = value;
                NotifyOfPropertyChange(() => IsSaving);
            }
        }

        private bool _CanEdit = true;
        public bool CanEdit
        {
            get
            {
                return _CanEdit;
            }
            set
            {
                _CanEdit = value;
                NotifyOfPropertyChange(() => CanEdit);
            }
        }

        private bool _infoHasChanged;
        /// <summary>
        /// Danh sách giấy chuyển viện đã thay đổi hay chưa (kể từ lần load từ database)
        /// </summary>
        public bool InfoHasChanged
        {
            get
            {
                return _infoHasChanged;
            }
            set
            {
                if (_infoHasChanged != value)
                {
                    _infoHasChanged = value;
                    NotifyOfPropertyChange(() => InfoHasChanged);
                }
            }
        }
        private ObservableCollection<PaperReferal> _paperReferals;
        public ObservableCollection<PaperReferal> PaperReferals
        {
            get
            {
                return _paperReferals;
            }
            set
            {
                _paperReferals = value;
                NotifyOfPropertyChange(() => PaperReferals);
            }
        }

        private bool _includeDeletedItems = false;
        public bool IncludeDeletedItems
        {
            get
            {
                return _includeDeletedItems;
            }
            set
            {
                if (_includeDeletedItems != value)
                {
                    _includeDeletedItems = value;
                    GetAllReferrals();
                    NotifyOfPropertyChange(() => IncludeDeletedItems);
                }
            }
        }
        public void GetAllReferrals()
        {
            if (_currentHiItem == null || IsLoading)
            {
                return;
            }
            PaperReferalInUse = new PaperReferal();
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                IsLoading = true;
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0974_G1_DangTimGCV)
                });
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;


                        contract.BeginGetAllPaperReferals(_currentHiItem.HIID, IncludeDeletedItems, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                PaperReferal latestItem = null;
                                var items = contract.EndGetAllPaperReferals(out latestItem, asyncResult);
                                HealthInsurance rootHiItem = (HealthInsurance)asyncResult.AsyncState;
                                if (items != null && items.Count > 0)
                                {
                                    PaperReferal activePaperReferal = null;
                                    var lst = new ObservableCollection<PaperReferal>();
                                    foreach (var paperReferal in items)
                                    {
                                        paperReferal.HealthInsurance = rootHiItem;
                                        lst.Add(paperReferal);
                                        if (paperReferal.IsActive)
                                        {
                                            activePaperReferal = paperReferal;
                                        }
                                    }
                                    PaperReferals = lst;
                                    NotifyOfPropertyChange(() => ShowCreateNewInUserPaperReferalCmd);
                                    //ShowAllowAppearingCmd
                                    NotifyOfPropertyChange(() => ShowAllowAppearingCmd); /*TMA*/
                                    if (PaperReferals != null
                                        && PaperReferals.Count > 0)
                                    {
                                        PaperReferalInUse = PaperReferals.FirstOrDefault();
                                        PaperReferalInUse.IsChecked = true;
                                        EditingPaperReferal = ObjectCopier.DeepCopy(PaperReferals.FirstOrDefault());
                                        PaperReferalBackup = ObjectCopier.DeepCopy(PaperReferals.FirstOrDefault());
                                        SelectedPaperReferal = ObjectCopier.DeepCopy(PaperReferals.FirstOrDefault());
                                    }

                                }
                                else
                                {
                                    PaperReferals = null;
                                    PaperReferalInUse = new PaperReferal();
                                    EditingPaperReferal = null;
                                    PaperReferalBackup = null;
                                }
                                NotifyAll();
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                            }

                        }), _currentHiItem);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    IsLoading = false;
                    Globals.IsBusy = false;
                    Globals.EventAggregator.Publish(new HiCardReloadEvent() { });
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }

        public void GetAllReferralsByDelete()
        {
            if (_currentHiItem == null || IsLoading)
            {
                return;
            }
            PaperReferalInUse = new PaperReferal();
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                IsLoading = true;
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0974_G1_DangTimGCV)
                });
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;


                        contract.BeginGetAllPaperReferals(_currentHiItem.HIID, IncludeDeletedItems, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                PaperReferal latestItem = null;
                                var items = contract.EndGetAllPaperReferals(out latestItem, asyncResult);
                                HealthInsurance rootHiItem = (HealthInsurance)asyncResult.AsyncState;
                                if (items != null && items.Count > 0)
                                {
                                    PaperReferal activePaperReferal = null;
                                    var lst = new ObservableCollection<PaperReferal>();
                                    foreach (var paperReferal in items)
                                    {
                                        paperReferal.HealthInsurance = rootHiItem;
                                        lst.Add(paperReferal);
                                        if (paperReferal.IsActive)
                                        {
                                            activePaperReferal = paperReferal;
                                        }
                                    }
                                    PaperReferals = lst;
                                    if (PaperReferals != null
                                        && PaperReferals.Count > 0)
                                    {
                                        PaperReferalInUse = ObjectCopier.DeepCopy(PaperReferals.FirstOrDefault());
                                        PaperReferalInUse.IsChecked = true;
                                        EditingPaperReferal = ObjectCopier.DeepCopy(PaperReferals.FirstOrDefault());
                                        PaperReferalBackup = ObjectCopier.DeepCopy(PaperReferals.FirstOrDefault());
                                        SelectedPaperReferal = ObjectCopier.DeepCopy(PaperReferals.FirstOrDefault());
                                    }

                                }
                                else
                                {
                                    PaperReferals = null;
                                    PaperReferalInUse = new PaperReferal();
                                    EditingPaperReferal = null;
                                    PaperReferalBackup = null;
                                }
                                NotifyAll();
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                            }

                        }), _currentHiItem);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    IsLoading = false;
                    Globals.IsBusy = false;
                    Globals.EventAggregator.Publish(new PaperReferalDeleteEvent() { });
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }

        private bool ValidatePaperReferalList()
        {
            return true;
        }

        private bool _confirmPaperReferalSelected;
        public bool ConfirmPaperReferalSelected
        {
            get { return _confirmPaperReferalSelected; }
            set
            {
                _confirmPaperReferalSelected = value;
                NotifyOfPropertyChange(() => ConfirmPaperReferalSelected);
            }
        }

        public void CancelEditing()
        {
            if (IsEditMode
                || IsChronicEditMode
                || IsAddNewMode)
            {
                if (_editingPaperReferal != null)
                {
                    CancelSavingPaperReferalCmd();
                }
            }
            InfoHasChanged = false;
        }
        ///////////////////////////////////////////////////

        public string PaperReferalTitle
        {
            get
            {
                if (_currentHiItem != null)
                {
                    return string.Format(eHCMSResources.Z0975_G1_GCVCuaThe0, _currentHiItem.HICardNo);
                }
                return "";
            }
        }

        public string AddEditPaperReferalTitle
        {
            get
            {
                if (_editingPaperReferal != null)
                {
                    if (_editingPaperReferal.RefID <= 0)
                    {
                        return eHCMSResources.Z0972_G1_ThemMoiGCV;
                    }
                    return eHCMSResources.Z0976_G1_CNhatGCV;
                }

                return "";
            }
        }

        private bool _isSavingPaperReferal;
        public bool IsSavingPaperReferal
        {
            get
            {
                return _isSavingPaperReferal;
            }
            set
            {
                _isSavingPaperReferal = value;
                NotifyOfPropertyChange(() => IsSavingPaperReferal);

                NotifyOfPropertyChange(() => CanSavePaperReferalCmd);
            }
        }
        private PaperReferal _editingPaperReferal;
        /// <summary>
        /// Giấy chuyển viện đang edit (thêm mới hoặc cập nhật)
        /// </summary>
        public PaperReferal EditingPaperReferal
        {
            get
            {
                return _editingPaperReferal;
            }
            set
            {
                if (_editingPaperReferal == value)
                {
                    return;
                }
                _editingPaperReferal = value;
                NotifyOfPropertyChange(() => EditingPaperReferal);
                NotifyOfPropertyChange(() => AddEditPaperReferalTitle);
                NotifyOfPropertyChange(() => CanSavePaperReferalCmd);
                NotifyOfPropertyChange(() => CanUsePaperReferalCmd);
                if (_editingPaperReferal != null)
                {
                    if (HospitalAutoCompleteContent != null)
                    {
                        //HospitalAutoCompleteContent.setDisplayText(_editingPaperReferal.IssuerLocation, _editingPaperReferal.Hospital, true);
                        HospitalAutoCompleteContent.SetSelHospital(_editingPaperReferal.Hospital);
                    }
                }
                else
                {
                    if (HospitalAutoCompleteContent != null)
                    {
                        //HospitalAutoCompleteContent.setDisplayText("", null);
                        HospitalAutoCompleteContent.InitBlankBindingValue();
                    }
                }
            }
        }

        private PaperReferal _paperReferalInUse;
        /// <summary>
        /// Giấy chuyển viện được sử dụng để confirm
        /// </summary>
        public PaperReferal PaperReferalInUse
        {
            get
            {
                return _paperReferalInUse;
            }
            set
            {
                _paperReferalInUse = value;
                NotifyOfPropertyChange(() => PaperReferalInUse);
                //NotifyAll();
            }
        }

        private PaperReferal _PaperReferalBackup;
        /// <summary>
        /// Giấy chuyển viện được sử dụng để backup
        /// </summary>
        public PaperReferal PaperReferalBackup
        {
            get
            {
                return _PaperReferalBackup;
            }
            set
            {
                _PaperReferalBackup = value;
                NotifyOfPropertyChange(() => PaperReferalBackup);
            }
        }

        public bool ShowSavePaperReferalCmd
        {
            get
            {
                switch (_operation)
                {
                    case FormOperationPaper.None:
                        return false;
                    case FormOperationPaper.AddNew:
                        return true;
                    case FormOperationPaper.Chronic:
                        return true;
                    case FormOperationPaper.Edit:
                        return true;
                    case FormOperationPaper.ReadOnly:
                        return false;
                }
                return false;
            }
        }
        public bool CanSavePaperReferalCmd
        {
            get
            {
                return !_isSavingPaperReferal && _editingPaperReferal != null;
            }
        }
        public void SavePaperReferalCmd()
        {
            if (_currentHiItem.IsActive)
            {
                if (_editingPaperReferal == null)
                {
                    MessageBox.Show(eHCMSResources.A0698_G1_Msg_InfoKhTheLuuGCV);
                    return;
                }

                //KMx: Anh Tuấn kiu bỏ bắt buộc chọn mãn tính khi sử dụng lại giấy chuyển viện.

                //if (Operation == FormOperationPaper.Chronic
                //    && !_editingPaperReferal.IsChronicDisease)
                //{
                //    MessageBox.Show(eHCMSResources.A0377_G1_Msg_InfoChuaChonBenhManTinhChoGCV);
                //    return;
                //}

                if (CreateNewInUse)     // || !HospitalAutoCompleteContent.hasPopulate)
                {
                    if (EditingPaperReferal.HospitalID <= 0 || string.IsNullOrEmpty(EditingPaperReferal.IssuerLocation))
                    {
                        ShowMessageBox(string.Format("{0}.", eHCMSResources.Z1274_G1_NoiCVKgHopLe));
                        return;
                    }
                }
                else
                {
                    EditingPaperReferal.Hospital = HospitalAutoCompleteContent.SelectedHospital;
                    if (EditingPaperReferal.Hospital == null
                        || string.IsNullOrEmpty(EditingPaperReferal.Hospital.HosName)
                        || EditingPaperReferal.Hospital.HosID < 1)
                    {
                        ShowMessageBox(string.Format("{0}.", eHCMSResources.Z1274_G1_NoiCVKgHopLe));
                        return;
                    }
                    else if (Globals.ServerConfigSection.HealthInsurances.IsCheckHICodeInPaperReferal && (EditingPaperReferal.Hospital.HICode == null || EditingPaperReferal.Hospital.HICode.Trim().Length < 5))
                    {
                        ShowMessageBox(eHCMSResources.Z1275_G1_MaBVKgHopLe);
                        return;
                    }
                    else
                    {
                        //gan cac gia tri tu hospital cho editingpaper
                        EditingPaperReferal.HospitalToValue();
                    }
                }


                if (_editingPaperReferal.RefID <= 0)
                {
                    _editingPaperReferal.HealthInsurance = _currentHiItem;
                }

                //KMx: Nếu như ngày ký giấy chuyển viện < ngày bắt đầu Bảo Hiểm thì hiện messagebox hỏi người dùng có thêm không, chứ không được chặn.
                //Nhưng hiện giờ THÔNG BÁO LỚN phức tạp quá nên A.Tuấn dặn comment code này ra, khi nào tìm được cách làm THÔNG BÁO LỚN rồi làm lại.

                ObservableCollection<ValidationResult> validationResults;
                if (!ValidatePaperReferal(_editingPaperReferal, out validationResults))
                {
                    Action<IValidationError> onInitDlg = delegate (IValidationError errorVm)
                    {
                        errorVm.SetErrors(validationResults);
                    };
                    GlobalsNAV.ShowDialog<IValidationError>(onInitDlg);
                    return;
                }

                if (_editingPaperReferal.RefID > 0)
                {
                    UpdatePaperReferal();
                }
                else
                {
                    AddPaperReferal();
                }

                //▼===== 20200605 TTM: Khi tạo mới giấy chuyển viện hoặc sử dụng lại sẽ bắn thông tin thẻ về
                //                     Cho màn hình đăng ký số thứ tự của CSKH.
                if (Globals.ServerConfigSection.CommonItems.UseQMSSystem && CurrentHiItem != null)
                {
                    Globals.EventAggregator.Publish(new NotifyAddCurHealthInsuranceToQMS { Item = CurrentHiItem });
                }
                //▲===== 
            }
            else
            {
                ShowMessageBox(string.Format("{0}.", eHCMSResources.Z0977_G1_KgTheLuuGCV));
            }
        }
        public void AddPaperReferal()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                IsSavingPaperReferal = true;
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0978_G1_DangLuuTTinGCV)
                });
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginAddPaperReferal(_editingPaperReferal,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                bool bOK = false;
                                CreateNewInUse = false;
                                PaperReferal addedItem = null;
                                try
                                {
                                    contract.EndAddPaperReferal(out addedItem, asyncResult);
                                    addedItem.isBrandNew = true;
                                    bOK = true;
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    bOK = false;
                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {
                                    bOK = false;
                                    error = new AxErrorEventArgs(ex);
                                }
                                if (bOK)
                                {
                                    if (addedItem.IsActive)
                                    {
                                        PaperReferalInUse = addedItem;
                                        PaperReferalInUse.IsChecked = true;
                                    }
                                    Operation = FormOperationPaper.None;
                                    GetAllReferrals();
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    IsSavingPaperReferal = false;
                    Globals.IsBusy = false;
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }
        public void UpdatePaperReferal()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                IsSavingPaperReferal = true;
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0978_G1_DangLuuTTinGCV)
                });
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginUpdatePaperReferal(_editingPaperReferal,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                bool bOK = false;
                                try
                                {
                                    var referal = asyncResult.AsyncState as PaperReferal;
                                    contract.EndUpdatePaperReferal(asyncResult);
                                    bOK = true;
                                    if (referal.IsActive)
                                    {
                                        PaperReferalInUse = referal;
                                        if (PaperReferalInUse != null)
                                        {
                                            PaperReferalInUse.IsChecked = true;
                                        }
                                    }
                                    GetAllReferrals();
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    bOK = false;
                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {
                                    bOK = false;
                                    error = new AxErrorEventArgs(ex);
                                }
                                if (bOK)
                                {
                                    //EditingPaperReferal = SelectedPaperReferal;
                                    Operation = FormOperationPaper.None;
                                }
                            }), _editingPaperReferal);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    IsSavingPaperReferal = false;
                    Globals.IsBusy = false;
                    this.HideBusyIndicator();
                }

                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }
        public void DeletePaper()
        {
            if (MessageBox.Show(eHCMSResources.A0160_G1_Msg_ConfXoaGCV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (IsChildWindow)
                {
                    Coroutine.BeginExecute(WarningResult(eHCMSResources.Z0979_G1_XoaGCVXNhanLaiBH, eHCMSResources.G2617_G1_Xoa));
                }
                else
                {
                    DeletePaper(PaperReferalInUse);
                }
            }
        }
        public IEnumerator<IResult> WarningResult(string message, string checkBoxContent)
        {
            var dialog = new MessageWarningShowDialogTask(message, checkBoxContent);
            yield return dialog;
            if (dialog.IsAccept)
            {
                DeletePaper(PaperReferalInUse);
            }
            yield break;
        }
        public void DeletePaper(PaperReferal PaperReferalDelete)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0980_G1_DangXoaGCV)
                });
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginDeletePaperReferal(PaperReferalDelete,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    contract.EndDeletePaperReferal(asyncResult);
                                    GetAllReferralsByDelete();
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {
                                    error = new AxErrorEventArgs(ex);
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    IsSavingPaperReferal = false;
                    Globals.IsBusy = false;
                    this.HideBusyIndicator();
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }
        private bool ValidatePaperReferal(PaperReferal referal, out ObservableCollection<ValidationResult> validationResults)
        {
            //KMx: Nếu như ngày ký giấy chuyển viện < ngày bắt đầu Bảo Hiểm thì hiện messagebox hỏi người dùng có thêm không, chứ không được chặn.
            //Nhưng hiện giờ THÔNG BÁO LỚN phức tạp quá nên A.Tuấn dặn comment code này ra, khi nào tìm được cách làm THÔNG BÁO LỚN rồi làm lại.


            bool bValid = true;
            validationResults = null;
            if (referal != null)
            {
                bValid = ValidationExtensions.ValidateObject<PaperReferal>(referal, out validationResults);

                if (referal.RefCreatedDate.HasValue && referal.RefCreatedDate.Value.Date > referal.HealthInsurance.ValidDateTo.Value.Date)
                {
                    if (Globals.ServerConfigSection.InRegisElements.AllowChildUnder6YearsOldUseHIOverDate && Globals.CanRegHIChildUnder6YearsOld(CurrentHiItem.Patient.Age.GetValueOrDefault()))
                    {
                        /* HPT_20160610: 
                         * Từ 10/06/2016: sửa đổi theo luật Bảo hiểm cho phép trẻ em dưới 6 tuổi sử dụng thẻ BHYT hết hạn để đăng ký trước ngày 30/9
                         * Nếu giữ nguyên ràng buộc này khi tạo giấy chuyển viện cho thẻ BHYT đã hết hạn sẽ không thể lưu được
                         * (Anh Tuấn đã review: 10/06/2016)
                         */
                    }
                    else
                    {
                        ValidationResult item = new ValidationResult(string.Format("Ngày ký giấy phải nhỏ hơn ngày hết hạn bảo hiểm ({0})", referal.HealthInsurance.ValidDateTo.Value.Date.ToString("dd/MM/yyyy")), new string[] { "RefCreatedDate" });
                        if (validationResults == null)
                        {
                            validationResults = new ObservableCollection<ValidationResult>();
                        }
                        validationResults.Add(item);
                        bValid = false;
                    }
                }
                if (referal.AcceptedDate.HasValue && referal.AcceptedDate.Value.Date > referal.HealthInsurance.ValidDateTo.Value.Date)
                {
                    if (Globals.ServerConfigSection.InRegisElements.AllowChildUnder6YearsOldUseHIOverDate && Globals.CanRegHIChildUnder6YearsOld(CurrentHiItem.Patient.Age.GetValueOrDefault()))
                    {
                        /* HPT_20160610: 
                         * Từ 10/06/2016: sửa đổi theo luật Bảo hiểm cho phép trẻ em dưới 6 tuổi sử dụng thẻ BHYT hết hạn để đăng ký trước ngày 30/9
                         * Nếu giữ nguyên ràng buộc này khi tạo giấy chuyển viện cho thẻ BHYT đã hết hạn sẽ không thể lưu được
                         * (Anh Tuấn đã review: 10/06/2016)
                         */
                    }
                    else
                    {
                        ValidationResult item = new ValidationResult(string.Format("Ngày nộp giấy phải nhỏ hơn ngày hết hạn bảo hiểm ({0})", referal.HealthInsurance.ValidDateTo.Value.Date.ToString("dd/MM/yyyy")), new string[] { "AcceptedDate" });
                        if (validationResults == null)
                        {
                            validationResults = new ObservableCollection<ValidationResult>();
                        }
                        validationResults.Add(item);
                        bValid = false;
                    }
                }
                //if (HospitalAutoCompleteContent != null && HospitalAutoCompleteContent.SelectedHospital != null && (HospitalAutoCompleteContent.SelectedHospital.HICode == null || HospitalAutoCompleteContent.SelectedHospital.HICode.Trim().Length < 5))
                return bValid;

                //if (referal.RefCreatedDate.HasValue
                //        && (referal.RefCreatedDate.Value.Date < referal.HealthInsurance.ValidDateFrom.Value.Date
                //        || referal.RefCreatedDate.Value.Date > referal.HealthInsurance.ValidDateTo.Value.Date
                //        ))
                //{
                //    ValidationResult item = new ValidationResult(string.Format("Ngày ký giấy phải nằm trong khoảng thời gian từ {0} đến {1}",
                //        referal.HealthInsurance.ValidDateFrom.Value.Date.ToString("dd/MM/yyyy"), referal.HealthInsurance.ValidDateTo.Value.Date.ToString("dd/MM/yyyy")), new string[] { "RefCreatedDate" });
                //    if (validationResults == null)
                //    {
                //        validationResults = new ObservableCollection<ValidationResult>();
                //    }
                //    validationResults.Add(item);
                //    bValid = false;
                //}
                //if (referal.AcceptedDate.HasValue
                //        && (referal.AcceptedDate.Value.Date < referal.HealthInsurance.ValidDateFrom.Value.Date
                //        || referal.AcceptedDate.Value.Date > referal.HealthInsurance.ValidDateTo.Value.Date
                //        ))
                //{
                //    ValidationResult item = new ValidationResult(string.Format("Ngày nộp giấy phải nằm trong khoảng thời gian từ {0} đến {1}",
                //        referal.HealthInsurance.ValidDateFrom.Value.Date.ToString("dd/MM/yyyy"), referal.HealthInsurance.ValidDateTo.Value.Date.ToString("dd/MM/yyyy")), new string[] { "AcceptedDate" });
                //    if (validationResults == null)
                //    {
                //        validationResults = new ObservableCollection<ValidationResult>();
                //    }
                //    validationResults.Add(item);
                //    bValid = false;
                //}
            }
            return true;
        }

        public bool ShowCreateNewPaperReferalCmd
        {
            get
            {
                switch (_operation)
                {
                    case FormOperationPaper.None:
                        return true && InUsed && (PaperReferalInUse == null ? true
                            : PaperReferalInUse.isBrandNew == false);
                    //them vao dieu kien chua co giay chuyen vien hoac la giay chuyen vien cu
                    case FormOperationPaper.AddNew:
                        return false;
                    case FormOperationPaper.Chronic:
                        return false;
                    case FormOperationPaper.Edit:
                        return false;
                    case FormOperationPaper.ReadOnly:
                        return true && InUsed && (PaperReferalInUse == null ? true
                            : PaperReferalInUse.isBrandNew == false);
                }
                return false;
            }
        }

        public bool CanCreateNewPaperReferalCmd
        {
            get
            {
                return !FormLocked;
            }
        }

        public void AxDataGrid_SelectionChanged(object sender)
        {
            if (PaperReferals != null && PaperReferals.Count > 0
                && ((AxDataGrid)sender).SelectedIndex > -1)
            {
                EditingPaperReferal = ObjectCopier.DeepCopy(PaperReferals[((AxDataGrid)sender).SelectedIndex]);
            }
        }

        MessageBoxTask msgTask;
        public void CreateNewPaperReferalCmd()
        {
            //HospitalAutoCompleteContent.Reset();
            PaperReferalBackup = ObjectCopier.DeepCopy(EditingPaperReferal);

            if (_currentHiItem == null)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0362_G1_Msg_InfoChonTheBHYT));
                return;
            }

            if (_currentHiItem.MarkAsDeleted)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0710_G1_Msg_InfoKhTheThemGCVChoTheBHDaXoa));
                return;
            }

            if (_currentHiItem.IsActive)
            {
                var newItem = new PaperReferal();

                newItem.HealthInsurance = _currentHiItem;
                newItem.IsActive = true;
                //newItem.Hospital = ObjectCopier.DeepCopy(SelectedPaperReferal.Hospital);
                EditingPaperReferal = newItem;
                Operation = FormOperationPaper.AddNew;
                
                //HospitalAutoCompleteContent.setDisplayText("", null);
                HospitalAutoCompleteContent.InitBlankBindingValue();
            }
            else
            {
                ShowMessageBox(string.Format("{0}.", eHCMSResources.Z0982_G1_KgTheThemMoiGCV));
            }
        }

        public PaperReferal CopyPaperReferal(PaperReferal target)
        {
            var newItem = new PaperReferal();
            newItem.Hospital = ObjectCopier.DeepCopy(target.Hospital);
            newItem.Hospital.HosName = target.IssuerLocation;
            newItem.HospitalID = target.HospitalID;
            newItem.IssuerLocation = target.IssuerLocation;
            newItem.IssuerCode = target.IssuerCode;
            newItem.RefCreatedDate = target.RefCreatedDate;
            newItem.AcceptedDate = target.AcceptedDate;
            newItem.CityProvinceName = target.CityProvinceName;
/*TMA*/
            newItem.TransferFormID = target.TransferFormID;
            newItem.TransferNum = target.TransferNum;
            newItem.IsReUse = true;
/*TMA*/
            //return ObjectCopier.DeepCopy(newItem);
            return newItem;
        }

        private bool CreateNewInUse { get; set; }

        public void CreateNewInUserPaperReferalCmd()
        {
            if (_currentHiItem.IsActive)
            {
                var newItem = new PaperReferal();
                CreateNewInUse = true;
                newItem.HealthInsurance = _currentHiItem;
                newItem.IsActive = true;
                newItem.IsReUse = true;/*TMA*/
                if (InUsed)
                {
                    newItem = CopyPaperReferal(EditingPaperReferal); /*TMA*/
                    //newItem = CopyPaperReferal(PaperReferalInUse);
                }
                else
                {
                    if (SelectedPaperReferal != null)
                    {
                        newItem = CopyPaperReferal(SelectedPaperReferal);
                    }
                }
                EditingPaperReferal = newItem;
                Operation = FormOperationPaper.Chronic;
            }
            else
            {
                ShowMessageBox(string.Format("{0}.", eHCMSResources.Z0993_G1_KgTheThemGCV));
            }
        }

        private void ShowMessageBox(string msg)
        {
            msgTask = new MessageBoxTask(msg, eHCMSResources.G0442_G1_TBao, Infrastructure.MessageBoxOptions.Ok);
            List<IResult> lst = new List<IResult>() { msgTask };
            Coroutine.BeginExecute(lst.GetEnumerator());
        }

        public bool ShowEditPaperReferalCmd
        {
            get
            {
                switch (_operation)
                {
                    //KMx: A. Tuấn cho sửa giấy chuyển viện thoải mái.
                    //     Khi nào muốn chặn "Sửa" lại thì bỏ comment ở dưới ra.
                    case FormOperationPaper.None:
                        //return true && InUsed && (PaperReferalInUse == null ? false : PaperReferalInUse.isBrandNew);
                        if (EditingPaperReferal == null)
                        {
                            return false;
                        }
                        else
                        {
                            return EditingPaperReferal.CityProvinceName == null ? false : true;
                        }
                    case FormOperationPaper.AddNew:
                        return false;
                    case FormOperationPaper.Chronic:
                        return false;
                    case FormOperationPaper.Edit:
                        return false;
                    case FormOperationPaper.ReadOnly:
                        //return true && InUsed && (PaperReferalInUse == null ? false : PaperReferalInUse.isBrandNew);
                        if (EditingPaperReferal == null)
                        {
                            return false;
                        }
                        else
                        {
                            return EditingPaperReferal.CityProvinceName == null ? false : true;
                        }
                }
                return false;
            }
        }
        public bool CanEditPaperReferalCmd
        {
            get
            {
                return !FormLocked;
            }
        }
        public void EditPaperReferalCmd()
        {
            //HospitalAutoCompleteContent.Reset();
            PaperReferalBackup = ObjectCopier.DeepCopy(EditingPaperReferal);
            /*TMA*/
            if (EditingPaperReferal.IsReUse)
            {
                ShowMessageBox(string.Format("{0}.", eHCMSResources.Z2117_G1_MSG));
                return;
            }
            /*TMA*/
            if (_currentHiItem.IsActive)
            {
                if (EditingPaperReferal == null
                    || EditingPaperReferal.RefID < 1)
                {
                    EditingPaperReferal = ObjectCopier.DeepCopy(PaperReferalInUse);
                }
                NotifyOfPropertyChange(() => EditingPaperReferal);
                Operation = FormOperationPaper.Edit;
                _editingPaperReferal.BeginEdit();
            }
            else
            {   
                ShowMessageBox(string.Format("{0}.", eHCMSResources.Z0994_G1_KgTheSuaGCV));
            }
        }
        public bool CanSelectPaperReferalItem
        {
            get
            {
                return Operation == FormOperationPaper.None || Operation == FormOperationPaper.ReadOnly;
            }
        }

        public bool ShowCancelSavingPaperReferalCmd
        {
            get
            {
                switch (_operation)
                {
                    case FormOperationPaper.None:
                        return false;
                    case FormOperationPaper.AddNew:
                        return true;
                    case FormOperationPaper.Chronic:
                        return true;
                    case FormOperationPaper.Edit:
                        return true;
                    case FormOperationPaper.ReadOnly:
                        return false;
                }
                return false;
            }
        }
        public bool CanCancelSavingPaperReferalCmd
        {
            get
            {
                return true;
            }
        }
        public void CancelSavingPaperReferalCmd()
        {
            CreateNewInUse = false;
            EditingPaperReferal = ObjectCopier.DeepCopy(PaperReferalBackup);

            if (_editingPaperReferal != null)
            {
                _editingPaperReferal.CancelEdit();
                if (_editingPaperReferal.RefID > 0)
                {
                    Operation = FormOperationPaper.ReadOnly;
                }
                else
                {
                    if (SelectedPaperReferal != null)
                    {
                        EditingPaperReferal = ObjectCopier.DeepCopy(SelectedPaperReferal);
                        //HospitalAutoCompleteContent.setDisplayText(SelectedPaperReferal.IssuerLocation, SelectedPaperReferal.Hospital, true);
                        Operation = FormOperationPaper.ReadOnly;
                    }
                    else
                    {
                        EditingPaperReferal = null;
                        Operation = FormOperationPaper.None;
                    }
                }
            }
            else
            {
                Operation = FormOperationPaper.None;
            }
        }

        public bool ShowCreateNewInUserPaperReferalCmd
        {
            get
            {
                switch (_operation)
                {
                    case FormOperationPaper.None:
                        return true && (!InUsed ? PaperReferals != null && PaperReferals.Count > 0 ? true : false
                            : (PaperReferalInUse == null ? false
                            : PaperReferalInUse.RefID > 0 && PaperReferalInUse.isBrandNew == false));
                    //them vao dieu kien la da co giay chuyen vien cu va giay chuyen vien moi chua duoc tao
                    case FormOperationPaper.AddNew:
                        return false;
                    case FormOperationPaper.Chronic:
                        return false;
                    case FormOperationPaper.Edit:
                        return false;
                    case FormOperationPaper.ReadOnly:
                        return true && (!InUsed ? PaperReferals != null && PaperReferals.Count > 0 ? true : false
                            : (PaperReferalInUse == null ? false
                            : PaperReferalInUse.RefID > 0 && PaperReferalInUse.isBrandNew == false));
                }
                return false;
            }
        }
/*TMA*/
        public bool ShowAllowAppearingCmd
        {
            get 
            {
                if (ShowCreateNewInUserPaperReferalCmd == true || ShowEditPaperReferalCmd==true)
                    return true;
                else return false;
            }
        }
/*TMA*/
        public bool ShowUsePaperReferalCmd
        {
            get
            {
                switch (_operation)
                {
                    case FormOperationPaper.None:
                        return false;
                    case FormOperationPaper.AddNew:
                        return false;
                    case FormOperationPaper.Chronic:
                        return false;
                    case FormOperationPaper.Edit:
                        return false;
                    case FormOperationPaper.ReadOnly:
                        return true;
                }
                return false;
            }
        }
        public bool CanUsePaperReferalCmd
        {
            get
            {
                return _editingPaperReferal != null && _editingPaperReferal.RefID > 0 && !FormLocked;
            }
        }
        public void UsePaperReferalCmd(PaperReferal item)
        {
            if (item != null)
            {
                if (!item.IsActive)
                {
                    ShowMessageBox(string.Format("{0}.", eHCMSResources.Z0983_G1_GCVChuaDuocKichHoat));
                    return;
                }
                PaperReferalInUse = item;
                PaperReferalInUse.IsChecked = true;
                return;
            }
            if (_editingPaperReferal != null && _editingPaperReferal.RefID > 0)
            {
                if (!_editingPaperReferal.IsActive)
                {
                    ShowMessageBox(eHCMSResources.Z0983_G1_GCVChuaDuocKichHoat);
                    return;
                }
                PaperReferalInUse = ObjectCopier.DeepCopy(_editingPaperReferal);
                PaperReferalInUse.IsChecked = true;
            }
        }
        public void ResetForm()
        {
            PaperReferalInUse = null;
            EditingPaperReferal = null;
            PaperReferals = null;
            Operation = FormOperationPaper.None;
        }
        private FormOperationPaper _operation;
        public FormOperationPaper Operation
        {
            get
            {
                return _operation;
            }
            set
            {
                _operation = value;
                if (_operation == FormOperationPaper.Edit || _operation == FormOperationPaper.AddNew
                    || _operation == FormOperationPaper.Chronic)
                {
                    InfoHasChanged = true;
                }
                else
                {
                    InfoHasChanged = false;
                }
                NotifyAll();
            }
        }
        public bool IsEditMode
        {
            get
            {
                switch (_operation)
                {
                    case FormOperationPaper.Edit:
                        return true;
                }
                return false;
            }
        }

        public bool IsAddNewMode
        {
            get
            {
                switch (_operation)
                {
                    case FormOperationPaper.AddNew:
                        return true;
                }
                return false;
            }
        }

        public bool IsChronicEditMode
        {
            get
            {
                switch (_operation)
                {
                    case FormOperationPaper.Chronic:
                        return true;
                }
                return false;
            }
        }

        public void NotifyAll()
        {
            NotifyOfPropertyChange(() => Operation);
            NotifyOfPropertyChange(() => ShowCreateNewPaperReferalCmd);
            NotifyOfPropertyChange(() => ShowCreateNewInUserPaperReferalCmd);
            //ShowAllowAppearingCmd
            NotifyOfPropertyChange(() => ShowAllowAppearingCmd);/*TMA*/
            //
            NotifyOfPropertyChange(() => ShowEditPaperReferalCmd);
            NotifyOfPropertyChange(() => ShowCancelSavingPaperReferalCmd);
            NotifyOfPropertyChange(() => ShowSavePaperReferalCmd);
            NotifyOfPropertyChange(() => ShowUsePaperReferalCmd);
            NotifyOfPropertyChange(() => IsEditMode);
            NotifyOfPropertyChange(() => IsAddNewMode);
            NotifyOfPropertyChange(() => IsChronicEditMode);
            NotifyOfPropertyChange(() => CanSelectPaperReferalItem);
        }
        public void hplHospitalAddNew()
        {
            Action<IHospitalAutoCompleteEdit> onInitDlg = delegate (IHospitalAutoCompleteEdit obj)
            {
                obj.IsUpdate = false;
                obj.IsPaperReferal = true;
                obj.Title = eHCMSResources.G0307_G1_ThemMoiNoiCV.ToUpper();
            };
            GlobalsNAV.ShowDialog<IHospitalAutoCompleteEdit>(onInitDlg);
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account

        private bool _TheBH_Edit = true;

        public bool TheBH_Edit
        {
            get
            {
                return _TheBH_Edit;
            }
            set
            {
                if (_TheBH_Edit == value)
                    return;
                _TheBH_Edit = value;
            }
        }
        #endregion

        public enum FormOperationPaper
        {
            AddNew = 1,
            Edit = 2,
            ReadOnly = 3,
            Chronic = 4,
            None = 5,
            GCT = 6 /*TMA*/
        }

        /*TMA*/
        public void FullPaperReferalCmd()
        {
            if (PaperReferalInUse.PtRegistrationID == null)
                MessageBox.Show("Chưa đăng ký cho bệnh nhân này ! Không được phép sử dụng giấy chuyển viện !");
            else
            {
                Action<IPaperReferalFull> onInitDlg = delegate (IPaperReferalFull TransferFromVm)
                {
                    TransferFromVm.IsThisViewDialog = true;
                    TransferFromVm.V_GetPaperReferalFullFromOtherView = true;
                    TransferFromVm.V_TransferFormType = (int)AllLookupValues.V_TransferFormType.CHUYEN_DEN;

                    if (PaperReferalInUse != null && PaperReferalInUse.PtRegistrationID != null)
                    {
                        TransferFromVm.CurrentTransferForm.CurPatientRegistration = new PatientRegistration();
                        TransferFromVm.CurrentTransferForm.CurPatientRegistration.PtRegistrationID = (long)PaperReferalInUse.PtRegistrationID;
                    }
                    if (PaperReferalInUse.HealthInsurance != null && PaperReferalInUse.HealthInsurance.HisID > 0)
                        TransferFromVm.CurrentTransferForm.CurPatientRegistration.HisID = (long)PaperReferalInUse.HealthInsurance.HisID;
                    TransferFromVm.ConfirmedPaperReferal = new PaperReferal();
                    if (EditingPaperReferal != null)
                    {
                        TransferFromVm.ConfirmedPaperReferal = EditingPaperReferal;
                    }
                    this.ActivateItem(TransferFromVm);

                    //▼====== #001:
                    var mEvent = new TransferFormEvent();
                    mEvent.Item = new TransferForm();

                    mEvent.Item.PatientFindBy = PatientFindBy;

                    mEvent.Item.CurPatientRegistration = new PatientRegistration();
                    mEvent.Item.V_TransferFormType = (int)AllLookupValues.V_TransferFormType.CHUYEN_DEN;
                    if (PaperReferalInUse != null)
                    {
                        if (IsActive == true || EditingPaperReferal.TransferFormID > 0)
                            mEvent.Item.TransferFormID = (long)EditingPaperReferal.TransferFormID;
                        mEvent.Item.CurPatientRegistration.PtRegistrationID = (long)EditingPaperReferal.PtRegistrationID;
                        if (EditingPaperReferal != null)
                        {
                            if (EditingPaperReferal.HealthInsurance != null)
                            {
                                mEvent.Item.CurPatientRegistration.HealthInsurance = EditingPaperReferal.HealthInsurance;
                            }

                            if (EditingPaperReferal.HealthInsurance.Patient != null)
                                mEvent.Item.CurPatientRegistration.Patient = EditingPaperReferal.HealthInsurance.Patient;
                            if (EditingPaperReferal.Hospital != null)
                            {
                                mEvent.Item.TransferFromHos = EditingPaperReferal.Hospital;
                            }
                        }
                        mEvent.Item.TransferDate = DateTime.Now;
                        mEvent.Item.FromDate = DateTime.Now;
                        mEvent.Item.ToDate = DateTime.Now;
                    }
                    if (EditingPaperReferal.TransferFormID == 0)
                    {
                        mEvent.Item.TransferToHos = new Hospital();
                        mEvent.Item.V_TransferTypeID = 62601;           //defalut: 1-a
                        mEvent.Item.V_PatientStatusID = 63002;          //defalut: không cấp cứu
                        mEvent.Item.V_TransferReasonID = 62902;         //default: yêu cầu chuyên môn
                        mEvent.Item.V_TreatmentResultID = 62701;        //defalut: thuyên giảm,tiến triển tốt.Ra viện
                        mEvent.Item.V_CMKTID = 62801;
                    }
                    //Globals.EventAggregator.Publish(mEvent);
                    TransferFromVm.SetCurrentInformation(mEvent);
                    //▲====== #001
                };
                GlobalsNAV.ShowDialog<IPaperReferalFull>(onInitDlg);

                //▼====== #001
                //var mEvent = new TransferFormEvent();
                //mEvent.Item = new TransferForm();

                //mEvent.Item.PatientFindBy = PatientFindBy;

                //mEvent.Item.CurPatientRegistration = new PatientRegistration();
                //mEvent.Item.V_TransferFormType = (int)AllLookupValues.V_TransferFormType.CHUYEN_DEN;
                //if (PaperReferalInUse != null)
                //{
                //    if (IsActive == true || EditingPaperReferal.TransferFormID > 0)
                //        mEvent.Item.TransferFormID = (long)EditingPaperReferal.TransferFormID;
                //    mEvent.Item.CurPatientRegistration.PtRegistrationID = (long)EditingPaperReferal.PtRegistrationID;
                //    if (EditingPaperReferal != null)
                //    {
                //        if (EditingPaperReferal.HealthInsurance != null)
                //        {
                //            mEvent.Item.CurPatientRegistration.HealthInsurance = EditingPaperReferal.HealthInsurance;
                //        }

                //        if (EditingPaperReferal.HealthInsurance.Patient != null)
                //            mEvent.Item.CurPatientRegistration.Patient = EditingPaperReferal.HealthInsurance.Patient;
                //        if (EditingPaperReferal.Hospital != null)
                //        {
                //            mEvent.Item.TransferFromHos = EditingPaperReferal.Hospital;
                //        }
                //    }
                //    mEvent.Item.TransferDate = DateTime.Now;
                //    mEvent.Item.FromDate = DateTime.Now;
                //    mEvent.Item.ToDate = DateTime.Now;
                //}
                //if (EditingPaperReferal.TransferFormID == 0)
                //{
                //    mEvent.Item.TransferToHos = new Hospital();
                //    mEvent.Item.V_TransferTypeID = 62601; // defalut : 1-a
                //    mEvent.Item.V_PatientStatusID = 63002;//defalut : không cấp cứu
                //    mEvent.Item.V_TransferReasonID = 62902;//default : yêu cầu chuyên môn
                //    mEvent.Item.V_TreatmentResultID = 62701;//defalut : thuyên giảm,tiến triển tốt.Ra viện
                //    mEvent.Item.V_CMKTID = 62801;
                //}
                //Globals.EventAggregator.Publish(mEvent);
                //▲====== #001
            }
        }
        /*TMA*/
    }
}