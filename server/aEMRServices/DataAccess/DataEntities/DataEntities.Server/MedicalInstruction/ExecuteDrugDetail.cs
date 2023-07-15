using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities.MedicalInstruction
{
    public partial class ExecuteDrugDetail: NotifyChangedBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public long ExecuteDrugID
        {
            get
            {
                return _ExecuteDrugID;
            }
            set
            {
                if (_ExecuteDrugID != value)
                {
                    _ExecuteDrugID = value;
                    RaisePropertyChanged("ExecuteDrugID");
                }
            }
        }
        private long _ExecuteDrugID;
        [DataMemberAttribute()]
        public long ExecuteDrugDetailID
        {
            get
            {
                return _ExecuteDrugDetailID;
            }
            set
            {
                if (_ExecuteDrugDetailID != value)
                {
                    _ExecuteDrugDetailID = value;
                    RaisePropertyChanged("ExecuteDrugDetailID");
                }
            }
        }
        private long _ExecuteDrugDetailID;
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
        [DataMemberAttribute()]
        public Staff NurseStaff
        {
            get
            {
                return _NurseStaff;
            }
            set
            {
                if (_NurseStaff != value)
                {
                    _NurseStaff = value;
                    RaisePropertyChanged("NurseStaff");
                }
            }
        }
        private Staff _NurseStaff;
        [DataMemberAttribute()]
        public bool MarkAsDeleted
        {
            get
            {
                return _MarkAsDeleted;
            }
            set
            {
                _MarkAsDeleted = value;
                RaisePropertyChanged("MarkAsDeleted");
            }
        }
        private bool _MarkAsDeleted;
        [DataMemberAttribute()]
        public DateTime DateExecute
        {
            get
            {
                return _DateExecute;
            }
            set
            {
                _DateExecute = value;
                RaisePropertyChanged("DateExecute");
            }
        }
        private DateTime _DateExecute;
        #endregion

        public static ExecuteDrugDetail CreateExecuteDrugDetail()
        {
            ExecuteDrugDetail ExecuteDrugDetail = new ExecuteDrugDetail();
            return ExecuteDrugDetail;
        }
    }
}
