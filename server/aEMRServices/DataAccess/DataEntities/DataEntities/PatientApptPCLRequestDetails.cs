using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using Service.Core.Common;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PatientApptPCLRequestDetails : EntityBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public long PCLReqItemID
        {
            get
            {
                return _PCLReqItemID;
            }
            set
            {
                if (_PCLReqItemID != value)
                {
                    OnPCLReqItemIDChanging(value);
                    _PCLReqItemID = value;
                    RaisePropertyChanged("PCLReqItemID");
                    OnPCLReqItemIDChanged();
                }
            }
        }
        private long _PCLReqItemID;
        partial void OnPCLReqItemIDChanging(long value);
        partial void OnPCLReqItemIDChanged();


        [DataMemberAttribute()]
        public long PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                if (_PCLExamTypeID != value)
                {
                    OnPCLExamTypeIDChanging(value);
                    _PCLExamTypeID = value;
                    RaisePropertyChanged("PCLExamTypeID");
                    OnPCLExamTypeIDChanged();
                }
            }
        }
        private long _PCLExamTypeID;
        partial void OnPCLExamTypeIDChanging(long value);
        partial void OnPCLExamTypeIDChanged();


        [DataMemberAttribute()]
        public long PatientPCLReqID
        {
            get
            {
                return _PatientPCLReqID;
            }
            set
            {
                if (_PatientPCLReqID != value)
                {
                    OnPatientPCLReqIDChanging(value);
                    _PatientPCLReqID = value;
                    RaisePropertyChanged("PatientPCLReqID");
                    OnPatientPCLReqIDChanged();
                }
            }
        }
        private long _PatientPCLReqID;
        partial void OnPatientPCLReqIDChanging(long value);
        partial void OnPatientPCLReqIDChanged();


        private PCLExamType _ObjPCLExamTypes;
        [DataMemberAttribute()]
        public PCLExamType ObjPCLExamTypes
        {
            get
            {
                return _ObjPCLExamTypes;
            }
            set
            {
                if (_ObjPCLExamTypes != value)
                {
                    _ObjPCLExamTypes = value;
                    RaisePropertyChanged("ObjPCLExamTypes");
                }
            }
        }


        [DataMemberAttribute()]
        public Int16 ApptTimeSegmentID
        {
            get
            {
                return _ApptTimeSegmentID;
            }
            set
            {
                if (_ApptTimeSegmentID != value)
                {
                    _ApptTimeSegmentID = value;
                    RaisePropertyChanged("ApptTimeSegmentID");
                }
            }
        }
        private Int16 _ApptTimeSegmentID;


        //[DataMemberAttribute()]
        //public ConsultationTimeSegments ObjApptTimeSegmentID
        //{
        //    get
        //    {
        //        return _ObjApptTimeSegmentID;
        //    }
        //    set
        //    {
        //        if (_ObjApptTimeSegmentID != value)
        //        {
        //            _ObjApptTimeSegmentID = value;
        //            RaisePropertyChanged("ObjApptTimeSegmentID");
        //        }
        //    }
        //}
        //private ConsultationTimeSegments _ObjApptTimeSegmentID;

        [DataMemberAttribute()]
        public PCLTimeSegment ApptTimeSegment
        {
            get
            {
                return _apptTimeSegment;
            }
            set
            {
                if (_apptTimeSegment != value)
                {
                    _apptTimeSegment = value;
                    RaisePropertyChanged("ApptTimeSegment");
                }
            }
        }
        private PCLTimeSegment _apptTimeSegment;

        [DataMemberAttribute()]
        public Int16 ServiceSeqNum
        {
            get
            {
                return _ServiceSeqNum;
            }
            set
            {
                if (_ServiceSeqNum != value)
                {
                    _ServiceSeqNum = value;
                    RaisePropertyChanged("ServiceSeqNum");
                }
            }
        }
        private Int16 _ServiceSeqNum;


        [DataMemberAttribute()]
        public Byte ServiceSeqNumType
        {
            get
            {
                return _ServiceSeqNumType;
            }
            set
            {
                if (_ServiceSeqNumType != value)
                {
                    _ServiceSeqNumType = value;
                    RaisePropertyChanged("ServiceSeqNumType");
                }
            }
        }
        private Byte _ServiceSeqNumType;


        [DataMemberAttribute()]
        public long DeptLocID
        {
            get
            {
                return _DeptLocID;
            }
            set
            {
                if (_DeptLocID != value)
                {
                    _DeptLocID = value;
                    RaisePropertyChanged("DeptLocID");
                }
            }
        }
        private long _DeptLocID;



        [DataMemberAttribute()]
        public DeptLocation ObjDeptLocID
        {
            get
            {
                return _ObjDeptLocID;
            }
            set
            {
                if (_ObjDeptLocID != value)
                {
                    _ObjDeptLocID = value;
                    RaisePropertyChanged("ObjDeptLocID");
                    if (_ObjDeptLocID != null
                        && _ObjDeptLocID.DeptLocationID > 0)
                    {
                        DeptLocID = _ObjDeptLocID.DeptLocationID;
                    }
                }
            }
        }
        private DeptLocation _ObjDeptLocID;


        //NY cung de tach phong
        [DataMemberAttribute()]
        public ObservableCollection<DeptLocation> ObjDeptLocIDList
        {
            get
            {
                return _ObjDeptLocIDList;
            }
            set
            {
                if (_ObjDeptLocIDList != value)
                {
                    _ObjDeptLocIDList = value;
                    RaisePropertyChanged("ObjDeptLocIDList");
                }
            }
        }
        private ObservableCollection<DeptLocation> _ObjDeptLocIDList;



        #endregion

        public override bool Equals(object obj)
        {
            PatientApptPCLRequestDetails info = obj as PatientApptPCLRequestDetails;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PCLReqItemID > 0 && this.PCLReqItemID == info.PCLReqItemID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private EntityState _EntityState = EntityState.NEW;
        [DataMemberAttribute()]
        public override EntityState EntityState
        {
            get
            {
                return _EntityState;
            }
            set
            {
                _EntityState = value;
                RaisePropertyChanged("EntityState");
            }
        }

        private long? _ClientContractSvcPtID;
        [DataMemberAttribute]
        public long? ClientContractSvcPtID
        {
            get
            {
                return _ClientContractSvcPtID;
            }
            set
            {
                if (_ClientContractSvcPtID == value)
                {
                    return;
                }
                _ClientContractSvcPtID = value;
                RaisePropertyChanged("ClientContractSvcPtID");
            }
        }
        [DataMemberAttribute()]
        public bool IsCountHI
        {
            get
            {
                return _IsCountHI;
            }
            set
            {
                if (_IsCountHI != value)
                {
                    _IsCountHI = value;
                    RaisePropertyChanged("IsCountHI");
                }
            }
        }
        private bool _IsCountHI;
    }
}