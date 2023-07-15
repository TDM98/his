using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class TransmissionMonitor : EntityBase, IEditableObject
    {
        public static TransmissionMonitor CreateTransmissionMonitor(Int64 TransmissionMonitorID)
        {
            TransmissionMonitor ac = new TransmissionMonitor();
           
            return ac;
        }
        #region Primitive Properties

        [DataMemberAttribute()]
        public long TransmissionMonitorID
        {
            get
            {
                return _TransmissionMonitorID;
            }
            set
            {
                if (_TransmissionMonitorID == value)
                {
                    return;
                }
                _TransmissionMonitorID = value;
                RaisePropertyChanged("TransmissionMonitorID");
            }
        }
        private long _TransmissionMonitorID;

        [DataMemberAttribute()]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if (_PtRegistrationID == value)
                {
                    return;
                }
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }
        private long _PtRegistrationID;

        [DataMemberAttribute()]
        public long IntPtDiagDrInstructionID
        {
            get
            {
                return _IntPtDiagDrInstructionID;
            }
            set
            {
                if (_IntPtDiagDrInstructionID == value)
                {
                    return;
                }
                _IntPtDiagDrInstructionID = value;
                RaisePropertyChanged("IntPtDiagDrInstructionID");
            }
        }
        private long _IntPtDiagDrInstructionID;

        [DataMemberAttribute()]
        public long OutClinicDeptReqID
        {
            get
            {
                return _OutClinicDeptReqID;
            }
            set
            {
                if (_OutClinicDeptReqID == value)
                {
                    return;
                }
                _OutClinicDeptReqID = value;
                RaisePropertyChanged("OutClinicDeptReqID");
            }
        }
        private long _OutClinicDeptReqID;

        [DataMemberAttribute()]
        public int TransAmount
        {
            get
            {
                return _TransAmount;
            }
            set
            {
                if (_TransAmount == value)
                {
                    return;
                }
                _TransAmount = value;
                RaisePropertyChanged("TransAmount");
            }
        }
        private int _TransAmount;

        [DataMemberAttribute()]
        public int TransSpeed
        {
            get
            {
                return _TransSpeed;
            }
            set
            {
                if (_TransSpeed == value)
                {
                    return;
                }
                _TransSpeed = value;
                RaisePropertyChanged("TransSpeed");
            }
        }
        private int _TransSpeed;

        [DataMemberAttribute()]
        public DateTime StartTime
        {
            get
            {
                return _StartTime;
            }
            set
            {
                if (_StartTime == value)
                {
                    return;
                }
                _StartTime = value;
                RaisePropertyChanged("StartTime");
            }
        }
        private DateTime _StartTime;

        [DataMemberAttribute]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                if (_StaffID == value)
                {
                    return;
                }
                _StaffID = value;
                RaisePropertyChanged("StaffID");
            }
        }
        private long _StaffID;

        [DataMemberAttribute()]
        public DateTime EndTime
        {
            get
            {
                return _EndTime;
            }
            set
            {
                if (_EndTime == value)
                {
                    return;
                }
                _EndTime = value;
                RaisePropertyChanged("EndTime");
            }
        }
        private DateTime _EndTime;

        [DataMemberAttribute]
        public bool IsDelete
        {
            get
            {
                return _IsDelete;
            }
            set
            {
                if (_IsDelete == value)
                {
                    return;
                }
                _IsDelete = value;
                RaisePropertyChanged("IsDelete");
            }
        }
        private bool _IsDelete;

        [DataMemberAttribute]
        public long CreatedStaffID
        {
            get
            {
                return _CreatedStaffID;
            }
            set
            {
                if (_CreatedStaffID == value)
                {
                    return;
                }
                _CreatedStaffID = value;
                RaisePropertyChanged("CreatedStaffID");
            }
        }
        private long _CreatedStaffID;

        [DataMemberAttribute]
        public DateTime CreatedDate
        {
            get
            {
                return _CreatedDate;
            }
            set
            {
                if (_CreatedDate == value)
                {
                    return;
                }
                _CreatedDate = value;
                RaisePropertyChanged("CreatedDate");
            }
        }
        private DateTime _CreatedDate;

        [DataMemberAttribute]
        public long LastUpdatedStaffID
        {
            get
            {
                return _LastUpdatedStaffID;
            }
            set
            {
                if (_LastUpdatedStaffID == value)
                {
                    return;
                }
                _LastUpdatedStaffID = value;
                RaisePropertyChanged("LastUpdatedStaffID");
            }
        }
        private long _LastUpdatedStaffID;

        [DataMemberAttribute]
        public DateTime LastUpdatedDate
        {
            get
            {
                return _LastUpdatedDate;
            }
            set
            {
                if (_LastUpdatedDate == value)
                {
                    return;
                }
                _LastUpdatedDate = value;
                RaisePropertyChanged("LastUpdatedDate");
            }
        }
        private DateTime _LastUpdatedDate;

        [DataMemberAttribute]
        public string BrandName
        {
            get
            {
                return _BrandName;
            }
            set
            {
                if (_BrandName == value)
                {
                    return;
                }
                _BrandName = value;
                RaisePropertyChanged("BrandName");
            }
        }
        private string _BrandName;
         [DataMemberAttribute]
        public double? Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                if (_Qty == value)
                {
                    return;
                }
                _Qty = value;
                RaisePropertyChanged("Qty");
            }
        }
        private double? _Qty;
        [DataMemberAttribute]
        public string InBatchNumber
        {
            get
            {
                return _InBatchNumber;
            }
            set
            {
                if (_InBatchNumber == value)
                {
                    return;
                }
                _InBatchNumber = value;
                RaisePropertyChanged("InBatchNumber");
            }
        }
        private string _InBatchNumber;
        [DataMemberAttribute]
        public string UsageDistance
        {
            get
            {
                return _UsageDistance;
            }
            set
            {
                if (_UsageDistance == value)
                {
                    return;
                }
                _UsageDistance = value;
                RaisePropertyChanged("UsageDistance");
            }
        }
        private string _UsageDistance;

        public void BeginEdit()
        {
            throw new NotImplementedException();
        }

        public void EndEdit()
        {
            throw new NotImplementedException();
        }

        public void CancelEdit()
        {
            throw new NotImplementedException();
        }


        #endregion
        #region Navigation Properties
        [DataMemberAttribute()]
      
        public ObservableCollection<TransmissionMonitor> TransmissionMonitors
        {
            get;
            set;
        }
        #endregion
    }
}
