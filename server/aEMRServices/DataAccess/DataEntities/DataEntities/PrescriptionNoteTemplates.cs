using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class PrescriptionNoteTemplates : NotifyChangedBase
    {
        #region Primitive Properties


        [DataMemberAttribute()]
        public long PrescriptNoteTemplateID
        {
            get
            {
                return _PrescriptNoteTemplateID;
            }
            set
            {
                if (_PrescriptNoteTemplateID != value)
                {
                    _PrescriptNoteTemplateID = value;
                    RaisePropertyChanged("PrescriptNoteTemplateID");
                }
            }
        }
        private long _PrescriptNoteTemplateID;
        partial void OnPrescriptNoteTemplateIDChanging(long value);
        partial void OnPrescriptNoteTemplateIDChanged();



        [DataMemberAttribute()]
        public String NoteDetails
        {
            get
            {
                return _NoteDetails;
            }
            set
            {
                if (_NoteDetails != value)
                {
                    OnNoteDetailsChanging(value);
                    _NoteDetails = value;
                    RaisePropertyChanged("NoteDetails");
                    OnNoteDetailsChanged();
                }
            }
        }
        private String _NoteDetails;
        partial void OnNoteDetailsChanging(String value);
        partial void OnNoteDetailsChanged();

        [DataMemberAttribute()]
        public String DetailsTemplate
        {
            get
            {
                return _DetailsTemplate;
            }
            set
            {
                if (_DetailsTemplate != value)
                {
                    OnDetailsTemplateChanging(value);
                    _DetailsTemplate = value;
                    RaisePropertyChanged("DetailsTemplate");
                    OnDetailsTemplateChanged();
                }
            }
        }
        private String _DetailsTemplate = "";
        partial void OnDetailsTemplateChanging(String value);
        partial void OnDetailsTemplateChanged();

        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                if (_RecCreatedDate != value)
                {
                    _RecCreatedDate = value;
                    RaisePropertyChanged("RecCreatedDate");
                }
            }
        }
        private DateTime _RecCreatedDate;
        partial void OnRecCreatedDateChanging(DateTime value);
        partial void OnRecCreatedDateChanged();


        [DataMemberAttribute()]
        public Boolean IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                if (_IsActive != value)
                {
                    _IsActive = value;
                    RaisePropertyChanged("IsActive");
                }
            }
        }
        private Boolean _IsActive;
        partial void OnIsActiveChanging(Boolean value);
        partial void OnIsActiveChanged();

        [DataMemberAttribute()]
        public long V_PrescriptionNoteTempTypeID
        {
            get
            {
                return _V_PrescriptionNoteTempTypeID;
            }
            set
            {
                if (_V_PrescriptionNoteTempTypeID != value)
                {
                    _V_PrescriptionNoteTempTypeID = value;
                    RaisePropertyChanged("V_PrescriptionNoteTempTypeID");
                }
            }
        }
        private long _V_PrescriptionNoteTempTypeID;
        partial void OnV_PrescriptionNoteTempTypeIDChanging(long value);
        partial void OnV_PrescriptionNoteTempTypeIDChanged();

        [DataMemberAttribute()]
        public AllLookupValues.V_PrescriptionNoteTempType V_PrescriptionNoteTempType
        {
            get
            {
                return _V_PrescriptionNoteTempType;
            }
            set
            {
                if (_V_PrescriptionNoteTempType != value)
                {
                    _V_PrescriptionNoteTempType = value;

                    V_PrescriptionNoteTempTypeID = (long)_V_PrescriptionNoteTempType;

                    RaisePropertyChanged("V_PrescriptionNoteTempType");
                }
            }
        }
        private AllLookupValues.V_PrescriptionNoteTempType _V_PrescriptionNoteTempType;
        partial void OnV_PrescriptionNoteTempTypeChanging(AllLookupValues.V_PrescriptionNoteTempType value);
        partial void OnV_PrescriptionNoteTempTypeChanged();


        [DataMemberAttribute()]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                if (_StaffID != value)
                {
                    _StaffID = value;
                    RaisePropertyChanged("StaffID");
                }
            }
        }
        private long _StaffID;
        partial void OnStaffIDChanging(long value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public Staff Staff
        {
            get
            {
                return _Staff;
            }
            set
            {
                if (_Staff != value)
                {
                    _Staff = value;
                    if (Staff != null)
                    {
                        StaffID = Staff.StaffID;
                    }
                    RaisePropertyChanged("Staff");
                }
            }
        }
        private Staff _Staff;
        partial void OnStaffChanging(Staff value);
        partial void OnStaffChanged();


        [DataMemberAttribute()]
        public String NoteDetailsShort
        {
            get
            {
                //ChangedWPF-CMN: Added codition when NoteDetails is null
                if (NoteDetails == null) return "";
                string str = "";
                for (int i = 0; i < NoteDetails.Length; i++)
                {
                    if (NoteDetails[i] != '\n')
                    {
                        str += NoteDetails[i];
                    }
                }

                if (str.Length > 40)
                {
                    str = str.Substring(0, 40) + " ...";
                }

                return str;
            }
        }
        //private String _NoteDetailsShort;
        //partial void OnNoteDetailsShortChanging(String value);
        //partial void OnNoteDetailsShortChanged();

        [DataMemberAttribute()]
        public String NoteDetailsShortWithID
        {
            get
            {
                //return PrescriptNoteTemplateID > 0 ? PrescriptNoteTemplateID.ToString() + ". " + NoteDetailsShort : NoteDetailsShort;
                return NoteDetailsShort;
            }
        }





        #endregion

        public override bool Equals(object obj)
        {
            var selectedItem = obj as PrescriptionNoteTemplates;
            if (selectedItem == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PrescriptNoteTemplateID == selectedItem.PrescriptNoteTemplateID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
