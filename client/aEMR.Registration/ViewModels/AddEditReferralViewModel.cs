using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using Service.Core.Common;
using aEMR.Common;
using Castle.Windsor;

namespace aEMR.Registration.ViewModels
{
    /// <summary>
    /// Dung de them 1 giay chuyen vien vao collection (khong them vao database).
    /// </summary>
    [Export(typeof(IAddEditReferral)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class AddEditReferralViewModel : Conductor<object>, IAddEditReferral
    {
        [ImportingConstructor]
        public AddEditReferralViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
        }
        private Patient _currentPatient;
        public Patient CurrentPatient
        {
            get
            {
                return _currentPatient;
            }
            set
            {
                _currentPatient = value;
                NotifyOfPropertyChange(() => CurrentPatient);
            }
        }
        private PaperReferal _currentPaperReferral;
        public PaperReferal CurrentPaperReferral
        {
            get { return _currentPaperReferral; }
            set
            {
                _currentPaperReferral = value;
                NotifyOfPropertyChange(()=>CurrentPaperReferral);
                if(_currentPaperReferral != null)
                {
                    _currentPaperReferral.PropertyChanged += new WeakEventHandler<PropertyChangedEventArgs>(_currentPaperReferral_PropertyChanged).Handler;
                }
                NotifyOfPropertyChange(()=>ShowAddButton);
                NotifyOfPropertyChange(() => ShowUpdateButton);
            }
        }

        private FormOperation _curFormOperation;
        public Visibility ShowAddButton
        {
            get
            {
                
                //if(_currentPaperReferral != null && _currentPaperReferral.RefID <= 0 && _currentPaperReferral.EntityState == EntityState.DETACHED)
                if (_curFormOperation == FormOperation.AddNew)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        public Visibility ShowUpdateButton
        {
            get
            {
                //if (_currentPaperReferral != null && _currentPaperReferral.RefID > 0)
                if (_curFormOperation == FormOperation.Edit)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        public void _currentPaperReferral_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InfoChanged = true;
        }
        public bool CanAddReferalCmd
        {
            get { return InfoChanged; }
        }
        public bool CanUpdateReferalCmd
        {
            get { return InfoChanged; }
        }
        public void AddReferalCmd()
        {
            //Kiem tra hop le du lieu.
            ObservableCollection<ValidationResult> validationResults;
            if (!Validate(out validationResults))
            {
                Action<IValidationError> onInitDlg = delegate (IValidationError errorVm)
                {
                    errorVm.SetErrors(validationResults);
                };
                GlobalsNAV.ShowDialog<IValidationError>(onInitDlg);
                return;
            }
            if(CurrentPaperReferral.RefID <= 0)
            {
                Globals.EventAggregator.Publish(new PaperReferalAttached() { Item = CurrentPaperReferral, PatientID = CurrentPatient == null ? -1 : CurrentPatient.PatientID });   
            }
            InfoChanged = false;
            TryClose();
        }
        public void UpdateReferalCmd()
        {
            //Kiem tra hop le du lieu.
            ObservableCollection<ValidationResult> validationResults;
            if (!Validate(out validationResults))
            {
                Action<IValidationError> onInitDlg = delegate (IValidationError errorVm)
                {
                    errorVm.SetErrors(validationResults);
                };
                GlobalsNAV.ShowDialog<IValidationError>(onInitDlg);
                return;
            }

            Globals.EventAggregator.Publish(new PaperReferalEdited(_sourceObject) { Item = CurrentPaperReferral, PatientID = CurrentPatient.PatientID });
            InfoChanged = false;
            TryClose();
        }
        public void OkCmd()
        {
            InfoChanged = false;
            TryClose();
        }

        public void CancelCmd()
        {
            InfoChanged = false;
            TryClose();
        }

        public void CreateNewPaperReferal()
        {
            _curFormOperation = FormOperation.AddNew;
            CurrentPaperReferral = new PaperReferal();
            CurrentPaperReferral.EntityState = EntityState.DETACHED;
        }

        private PaperReferal _sourceObject = null;
        public void EditPaperReferal(PaperReferal referal)
        {
            _curFormOperation = FormOperation.Edit;
            _sourceObject = referal;
            CurrentPaperReferral = referal.DeepCopy();
        }

        private bool _infoChanged;
        public bool InfoChanged
        {
            get { return _infoChanged; }
            set 
            { 
                _infoChanged = value;
                NotifyOfPropertyChange(()=>InfoChanged);
                NotifyOfPropertyChange(() => CanAddReferalCmd);
                NotifyOfPropertyChange(() => CanUpdateReferalCmd);
            }
        }

        private bool Validate(out ObservableCollection<ValidationResult> result)
        {
            result = new ObservableCollection<ValidationResult>();
            //Se lay ngay tren server.
            DateTime today = DateTime.Now.Date;
            if (!_currentPaperReferral.AdmissionDate.HasValue)
            {
                ValidationResult item = new ValidationResult("Chưa nhập giá trị Ngày nhập viện", new string[] { "AdmissionDate" });
                result.Add(item);
            }
            else if(_currentPaperReferral.AdmissionDate.Value.Date > today)
            {
                ValidationResult item = new ValidationResult("Ngày nhập viện không hợp lệ", new string[] { "AdmissionDate" });
                result.Add(item);
            }

            if (!_currentPaperReferral.ValidDateFrom.HasValue)
            {
                ValidationResult item = new ValidationResult("Chưa nhập giá trị Ngày bắt đầu", new string[] { "ValidDateFrom" });
                result.Add(item);
            }
            else if(_currentPaperReferral.ValidDateFrom.Value.Date > today)
            {
                ValidationResult item = new ValidationResult("Ngày bắt đầu không hợp lệ", new string[] { "ValidDateFrom" });
                result.Add(item);
            }

            if (!_currentPaperReferral.ValidDateTo.HasValue)
            {
                ValidationResult item = new ValidationResult("Chưa nhập giá trị Ngày kết thúc", new string[] { "ValidDateTo" });
                result.Add(item);
            }
            else if (_currentPaperReferral.ValidDateTo.Value.Date < today)
            {
                ValidationResult item = new ValidationResult("Ngày kết thúc không hợp lệ", new string[] { "ValidDateTo" });
                result.Add(item);
            }

            if(string.IsNullOrEmpty(_currentPaperReferral.FromHospital))
            {
                ValidationResult item = new ValidationResult("Chưa nhập giá trị Từ bệnh viện", new string[] { "FromHospital" });
                result.Add(item);
            }
            if (string.IsNullOrEmpty(_currentPaperReferral.TreatmentFaculty))
            {
                ValidationResult item = new ValidationResult("Chưa nhập giá trị Khoa điều trị", new string[] { "TreatmentFaculty" });
                result.Add(item);
            }

            if (string.IsNullOrEmpty(_currentPaperReferral.GeneralDiagnoses))
            {
                ValidationResult item = new ValidationResult("Chưa nhập giá trị Chẩn đoán", new string[] { "GeneralDiagnoses" });
                result.Add(item);
            }

            if (string.IsNullOrEmpty(_currentPaperReferral.CurrentStatusOfPt))
            {
                ValidationResult item = new ValidationResult("Chưa nhập giá trị Tình trạng bệnh nhân", new string[] { "CurrentStatusOfPt" });
                result.Add(item);
            }

            if (string.IsNullOrEmpty(_currentPaperReferral.DischargeReason))
            {
                ValidationResult item = new ValidationResult("Chưa nhập giá trị Lý do vào viện", new string[] { "DischargeReason" });
                result.Add(item);
            }

            if(result.Count > 0)
            {
                return false;
            }
            return true;
        }
    }
}
