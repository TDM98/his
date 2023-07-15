using eHCMS.Services.Core;
using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public partial class PCLExamAccordingICD : NotifyChangedBase, IEditableObject
    {
        public PCLExamAccordingICD()
         : base()
        {

        }
        private PCLExamAccordingICD _tempPCLExamAccordingICD;
        public override bool Equals(object obj)
        {
            PCLExamAccordingICD info = obj as PCLExamAccordingICD;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PCLExamAccordingICDID > 0 && this.PCLExamAccordingICDID == info.PCLExamAccordingICDID;
        }
        public void BeginEdit()
        {
            _tempPCLExamAccordingICD = (PCLExamAccordingICD)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempPCLExamAccordingICD)
                CopyFrom(_tempPCLExamAccordingICD);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(PCLExamAccordingICD p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        [DataMemberAttribute()]
        public long PCLExamAccordingICDID
        {
            get
            {
                return _PCLExamAccordingICDID;
            }
            set
            {
                OnPCLExamAccordingICDIDChanging(value);
                _PCLExamAccordingICDID = value;
                RaisePropertyChanged("PCLExamAccordingICDID");
                OnPCLExamAccordingICDIDChanged();
            }
        }
        private long _PCLExamAccordingICDID;
        partial void OnPCLExamAccordingICDIDChanging(long value);
        partial void OnPCLExamAccordingICDIDChanged();

        [DataMemberAttribute()]
        public long PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                OnPCLExamTypeIDChanging(value);
                _PCLExamTypeID = value;
                RaisePropertyChanged("PCLExamTypeID");
                OnPCLExamTypeIDChanged();
            }
        }
        private long _PCLExamTypeID;
        partial void OnPCLExamTypeIDChanging(long value);
        partial void OnPCLExamTypeIDChanged();

        [DataMemberAttribute()]
        public string ICDList
        {
            get
            {
                return _ICDList;
            }
            set
            {
                OnICDListChanging(value);
                _ICDList = value;
                RaisePropertyChanged("ICDList");
                OnICDListChanged();
            }
        }
        private string _ICDList;
        partial void OnICDListChanging(string value);
        partial void OnICDListChanged();

        [DataMemberAttribute()]
        public int ReminderTime
        {
            get
            {
                return _ReminderTime;
            }
            set
            {
                OnReminderTimeChanging(value);
                _ReminderTime = value;
                RaisePropertyChanged("ReminderTime");
                OnReminderTimeChanged();
            }
        }
        private int _ReminderTime;
        partial void OnReminderTimeChanging(int value);
        partial void OnReminderTimeChanged();

        [DataMemberAttribute()]
        public bool IsDelete
        {
            get
            {
                return _IsDelete;
            }
            set
            {
                OnIsDeleteChanging(value);
                _IsDelete = value;
                RaisePropertyChanged("IsDelete");
                OnIsDeleteChanged();
            }
        }
        private bool _IsDelete;
        partial void OnIsDeleteChanging(bool value);
        partial void OnIsDeleteChanged();

        [DataMemberAttribute()]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                OnIsActiveChanging(value);
                _IsActive = value;
                RaisePropertyChanged("IsActive");
                OnIsActiveChanged();
            }
        }
        private bool _IsActive;
        partial void OnIsActiveChanging(bool value);
        partial void OnIsActiveChanged();
        private DateTime? _MedicalInstructionDate;
        [DataMemberAttribute]
        public DateTime? MedicalInstructionDate
        {
            get
            {
                return _MedicalInstructionDate;
            }
            set
            {
                _MedicalInstructionDate = value;
                RaisePropertyChanged("MedicalInstructionDate");
            }
        }
        private PCLExamType _PCLExamType;
        [DataMemberAttribute]
        public PCLExamType PCLExamType
        {
            get
            {
                return _PCLExamType;
            }
            set
            {
                _PCLExamType = value;
                RaisePropertyChanged("PCLExamType");
            }
        }
        private bool _IsChecked;
        [DataMemberAttribute]
        public bool IsChecked
        {
            get
            {
                return _IsChecked;
            }
            set
            {
                _IsChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }
        private string _MedicalInstructionDateStr;
        [DataMemberAttribute]
        public string MedicalInstructionDateStr
        {
            get
            {
                return _MedicalInstructionDateStr;
            }
            set
            {
                _MedicalInstructionDateStr = value;
                RaisePropertyChanged("MedicalInstructionDateStr");
            }
        }
    }
}
