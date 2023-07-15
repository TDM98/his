using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations; //validation data
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-10-12
 * Contents: Create Data Entities
/*******************************************************************/
#endregion

namespace DataEntities
{
    public partial class PatientVitalSign : EntityBase, IEditableObject
    {
        public PatientVitalSign()
            : base()
        {
        }

        #region From EF
        #region Factory Method

     
        /// Create a new PatientVitalSign object.
     
        /// <param name="vSignID">Initial value of the VSignID property.</param>
        /// <param name="commonMedRecID">Initial value of the CommonMedRecID property.</param>
        public static PatientVitalSign CreatePatientVitalSign(Byte vSignID, long commonMedRecID)
        {
            PatientVitalSign patientVitalSign = new PatientVitalSign();
            patientVitalSign.VSignID = vSignID;
            patientVitalSign.CommonMedRecID = commonMedRecID;
            return patientVitalSign;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public long PtVSignID
        {
            get
            {
                return _PtVSignID;
            }
            set
            {
                if (_PtVSignID != value)
                {
                    OnPtVSignIDChanging(value);
                    _PtVSignID = value;
                    RaisePropertyChanged("PtVSignID");
                    OnPtVSignIDChanged();
                }
            }
        }
        private long _PtVSignID;
        partial void OnPtVSignIDChanging(long value);
        partial void OnPtVSignIDChanged();


     
        
     
        [DataMemberAttribute()]
        public Byte VSignID
        {
            get
            {
                return _VSignID;
            }
            set
            {
                if (_VSignID != value)
                {
                    OnVSignIDChanging(value);
                    _VSignID = value;
                    RaisePropertyChanged("VSignID");
                    OnVSignIDChanged();
                }
            }
        }
        private Byte _VSignID;
        partial void OnVSignIDChanging(Byte value);
        partial void OnVSignIDChanged();

     
        
     
        [DataMemberAttribute()]
        public long CommonMedRecID
        {
            get
            {
                return _CommonMedRecID;
            }
            set
            {
                if (_CommonMedRecID != value)
                {
                    OnCommonMedRecIDChanging(value);
                    _CommonMedRecID = value;
                    RaisePropertyChanged("CommonMedRecID");
                    OnCommonMedRecIDChanged();
                }
            }
        }
        private long _CommonMedRecID;
        partial void OnCommonMedRecIDChanging(long value);
        partial void OnCommonMedRecIDChanged();

     
        
     
        
        ///[DefaultValue(DateTime.Now)]
        [DataMemberAttribute()]
        //[Required(ErrorMessage = "Examination Date is required")]
        public Nullable<DateTime> VSignExamDate
        {
            get
            {
                return _VSignExamDate;
            }
            set
            {
                if (_VSignExamDate != value)
                {
                    OnVSignExamDateChanging(value);
                    _VSignExamDate = value;
                    RaisePropertyChanged("VSignExamDate");
                    OnVSignExamDateChanged();
                }
            }
        }
        private Nullable<DateTime> _VSignExamDate=DateTime.Now.Date;
        partial void OnVSignExamDateChanging(Nullable<DateTime> value);
        partial void OnVSignExamDateChanged();

     
        [DataMemberAttribute()]
        //[Required(ErrorMessage = "The First Value is required")]
        public String VSignValue1
        {
            get
            {
                return _VSignValue1;
            }
            set
            {
                if (_VSignValue1!=value)
                {
                    OnVSignValue1Changing(value);
                    _VSignValue1 = value;
                    RaisePropertyChanged("VSignValue1");
                    OnVSignValue1Changed();    
                }
                
            }
        }
        private String _VSignValue1;
        partial void OnVSignValue1Changing(String value);
        partial void OnVSignValue1Changed();

        [DataMemberAttribute()]
        public String VSignValue2
        {
            get
            {
                return _VSignValue2;
            }
            set
            {
                if (_VSignValue2!=value)
                {
                    OnVSignValue2Changing(value);
                    _VSignValue2 = value;
                    RaisePropertyChanged("VSignValue2");
                    OnVSignValue2Changed();
                }
            }
        }
        private String _VSignValue2;
        partial void OnVSignValue2Changing(String value);
        partial void OnVSignValue2Changed();

     
        
     
        [DataMemberAttribute()]
        public String VSignNotes
        {
            get
            {
                return _VSignNotes;
            }
            set
            {
                if (_VSignNotes != value)
                {
                    OnVSignNotesChanging(value);
                    _VSignNotes = value;
                    RaisePropertyChanged("VSignNotes");
                    OnVSignNotesChanged();
                }
                
            }
        }
        private String _VSignNotes;
        partial void OnVSignNotesChanging(String value);
        partial void OnVSignNotesChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<Int64> V_VSignContext
        {
            get
            {
                return _V_VSignContext;
            }
            set
            {
                if (_V_VSignContext != value)
                {
                    OnV_VSignContextChanging(value);
                    _V_VSignContext = value;
                    RaisePropertyChanged("V_VSignContext");
                    OnV_VSignContextChanged();    
                }
                
            }
        }
        private Nullable<Int64> _V_VSignContext;
        partial void OnV_VSignContextChanging(Nullable<Int64> value);
        partial void OnV_VSignContextChanged();

        #endregion

        #region Navigation Properties
        
        [DataMemberAttribute()]
        public CommonMedicalRecord CommonMedicalRecord
        {
            get
            {
                return _CommonMedicalRecord;
            }
            set
            {
                if (_CommonMedicalRecord != value)
                {
                    _CommonMedicalRecord = value;
                    RaisePropertyChanged("CommonMedicalRecord");
                }
            }
        }
        private CommonMedicalRecord _CommonMedicalRecord;
     
        
        [DataMemberAttribute()]
        //[Required(ErrorMessage = "The VitalSign is required")]
        public VitalSign VitalSign
        {
            get
            {
                return _VitalSign;
            }
            set
            {
                if (_VitalSign != value)
                {
                    _VitalSign = value;
                    RaisePropertyChanged("VitalSign");    
                }
            }
        }
        private VitalSign _VitalSign;
        

        [DataMemberAttribute()]
        //[Required(ErrorMessage = "The Context is required")]
        public Lookup LookupVSignContext
        {
            get
            {
                return _LookupVSignContext;
            }
            set
            {
                if (_LookupVSignContext != value)
                {
                    _LookupVSignContext = value;
                    RaisePropertyChanged("LookupVSignContext");    
                }
                
            }
        }
        private Lookup _LookupVSignContext;
        #endregion

        #endregion
        
        private PatientVitalSign _tempPatientVitalSign;


        [DataMemberAttribute()]
        private bool _isDeleted = true;
        public bool isDeleted
        {
            get
            {
                return _isDeleted;
            }
            set
            {
                if (_isDeleted == value)
                    return;
                _isDeleted = value;
                RaisePropertyChanged("isDeleted");
            }
        }

        [DataMemberAttribute()]
        private bool _isEdit = true;
        public bool isEdit
        {
            get
            {
                return _isEdit;
            }
            set
            {
                if (_isEdit == value)
                    return;
                _isEdit = value;
                RaisePropertyChanged("isEdit");
                if (isEdit == false)
                {
                    isSave = true;
                    isCancel = true;
                }
                else
                {
                    isSave = false;
                    isCancel = false;
                }

            }
        }

        [DataMemberAttribute()]
        private bool _isCancel = false;
        public bool isCancel
        {
            get
            {
                return _isCancel;
            }
            set
            {
                if (_isCancel == value)
                    return;
                _isCancel = value;
                RaisePropertyChanged("isCancel");
            }
        }

        [DataMemberAttribute()]
        private bool _isSave = false;
        public bool isSave
        {
            get
            {
                return _isSave;
            }
            set
            {
                if (_isSave == value)
                    return;
                _isSave = value;
                RaisePropertyChanged("isSave");
            }
        }
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempPatientVitalSign = (PatientVitalSign)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempPatientVitalSign)
                CopyFrom(_tempPatientVitalSign);
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(PatientVitalSign p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

    }
}
