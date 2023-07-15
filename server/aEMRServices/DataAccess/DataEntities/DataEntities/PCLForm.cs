using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PCLForm : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PCLForm object.

        /// <param name="pCLFormID">Initial value of the PCLFormID property.</param>
        /// <param name="pCLFormName">Initial value of the PCLFormName property.</param>
        public static PCLForm CreatePCLForm(long pCLFormID, String pCLFormName)
        {
            PCLForm pCLForm = new PCLForm();
            pCLForm.PCLFormID = pCLFormID;
            pCLForm.PCLFormName = pCLFormName;
            return pCLForm;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long PCLFormID
        {
            get
            {
                return _PCLFormID;
            }
            set
            {
                if (_PCLFormID != value)
                {
                    OnPCLFormIDChanging(value);
                    _PCLFormID = value;
                    RaisePropertyChanged("PCLFormID");
                    OnPCLFormIDChanged();
                }
            }
        }
        private long _PCLFormID;
        partial void OnPCLFormIDChanging(long value);
        partial void OnPCLFormIDChanged();

        [DataMemberAttribute()]
        [Required(ErrorMessage = "Nhập Tên PCLForm!")]
        [StringLength(64, MinimumLength = 0, ErrorMessage = "Tên PCLForm Phải <= 64 Ký Tự")]
        public String PCLFormName
        {
            get
            {
                return _PCLFormName;
            }
            set
            {
                OnPCLFormNameChanging(value);
                ValidateProperty("PCLFormName", value);
                _PCLFormName = value;
                RaisePropertyChanged("PCLFormName");
                OnPCLFormNameChanged();
            }
        }
        private String _PCLFormName;
        partial void OnPCLFormNameChanging(String value);
        partial void OnPCLFormNameChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> ApplicatorDay
        {
            get
            {
                return _ApplicatorDay;
            }
            set
            {
                OnApplicatorDayChanging(value);
                _ApplicatorDay = value;
                RaisePropertyChanged("ApplicatorDay");
                OnApplicatorDayChanged();
            }
        }
        private Nullable<DateTime> _ApplicatorDay;
        partial void OnApplicatorDayChanging(Nullable<DateTime> value);
        partial void OnApplicatorDayChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> ExpiredDay
        {
            get
            {
                return _ExpiredDay;
            }
            set
            {
                OnExpiredDayChanging(value);
                _ExpiredDay = value;
                RaisePropertyChanged("ExpiredDay");
                OnExpiredDayChanged();
            }
        }
        private Nullable<DateTime> _ExpiredDay;
        partial void OnExpiredDayChanging(Nullable<DateTime> value);
        partial void OnExpiredDayChanged();


        public Int64 V_PCLMainCategory
        {
            get { return _V_PCLMainCategory; }
            set
            {
                if (_V_PCLMainCategory != value)
                {
                    OnV_PCLMainCategoryChanging(value);
                    _V_PCLMainCategory = value;
                    RaisePropertyChanged("V_PCLMainCategory");
                    OnV_PCLMainCategoryChanged();
                }
            }
        }
        private Int64 _V_PCLMainCategory;
        partial void OnV_PCLMainCategoryChanging(Int64 value);
        partial void OnV_PCLMainCategoryChanged();



        public Lookup ObjV_PCLMainCategory
        {
            get
            {
                return _ObjV_PCLMainCategory;
            }
            set
            {
                if (_ObjV_PCLMainCategory != value)
                {
                    OnObjV_PCLMainCategoryChanging(value);
                    _ObjV_PCLMainCategory = value;
                    RaisePropertyChanged("ObjV_PCLMainCategory");
                    OnObjV_PCLMainCategoryChanged();
                }
            }
        }
        private Lookup _ObjV_PCLMainCategory;
        partial void OnObjV_PCLMainCategoryChanging(Lookup value);
        partial void OnObjV_PCLMainCategoryChanged();


        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public ObservableCollection<PatientPCLRequest> PatientPCLAppointments
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<PCLForExternalRef> PCLRequestForms
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<PCLSection> PCLSections
        {
            get;
            set;
        }

        #endregion

        public override bool Equals(object obj)
        {
            PCLForm SelectedPCLForm = obj as PCLForm;
            if (SelectedPCLForm == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PCLFormID == SelectedPCLForm.PCLFormID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
