using System;
using System.Net;
using System.Windows;
using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class PatientPCLRequestSearchCriteria : SearchCriteriaBase
    {
        public PatientPCLRequestSearchCriteria()
        {
        }

        private Int64 _PatientID;
        public Int64 PatientID
        {
            get { return _PatientID; }
            set 
            { 
                _PatientID = value;
                RaisePropertyChanged("PatientID");
            }
        }
     
        private Int64 _PtRegistrationID;
        public Int64 PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }
        
        private AllLookupValues.PatientPCLRequestListType _LoaiDanhSach;
        public AllLookupValues.PatientPCLRequestListType LoaiDanhSach
        {
            get { return _LoaiDanhSach; }
            set
            {
                if(_LoaiDanhSach!=value)
                {
                    _LoaiDanhSach = value;
                    RaisePropertyChanged("LoaiDanhSach");
                }
            }
        }

        private string _OrderBy;
        public string OrderBy
        {
            get { return _OrderBy; }
            set
            {
                _OrderBy = value;
                RaisePropertyChanged("OrderBy");
            }
        }

        #region "Ds phiếu đăng ký làm CLS"
        private Int64 _PCLExamTypeLocationsDeptLocationID;
        public Int64 PCLExamTypeLocationsDeptLocationID
        {
            get { return _PCLExamTypeLocationsDeptLocationID; }
            set
            {
                if (_PCLExamTypeLocationsDeptLocationID != value)
                {
                    _PCLExamTypeLocationsDeptLocationID = value;
                    RaisePropertyChanged("PCLExamTypeLocationsDeptLocationID");
                }
            }
        }


        private Nullable<Int64> _V_PCLMainCategory;
        public Nullable<Int64> V_PCLMainCategory
        {
            get { return _V_PCLMainCategory; }
            set
            {
                if (_V_PCLMainCategory != value)
                {
                    _V_PCLMainCategory = value;
                    RaisePropertyChanged("V_PCLMainCategory");
                }
            }
        }


        private Nullable<Int64> _PCLExamTypeSubCategoryID;
        public Nullable<Int64> PCLExamTypeSubCategoryID
        {
            get { return _PCLExamTypeSubCategoryID; }
            set
            {
                if (_PCLExamTypeSubCategoryID != value)
                {
                    _PCLExamTypeSubCategoryID = value;
                    RaisePropertyChanged("PCLExamTypeSubCategoryID");
                }
            }
        }


        private Nullable<Int64> _PCLResultParamImpID;
        public Nullable<Int64> PCLResultParamImpID
        {
            get { return _PCLResultParamImpID; }
            set
            {
                if (_PCLResultParamImpID != value)
                {
                    _PCLResultParamImpID = value;
                    RaisePropertyChanged("PCLResultParamImpID");
                }
            }
        }


        private Nullable<DateTime> _FromDate;
        public Nullable<DateTime> FromDate
        {
            get { return _FromDate; }
            set
            {
                if (_FromDate != value)
                {
                    _FromDate = value;
                    RaisePropertyChanged("FromDate");
                }
            }
        }

        private Nullable<DateTime> _ToDate;
        public Nullable<DateTime> ToDate
        {
            get { return _ToDate; }
            set
            {
                if (_ToDate != value)
                {
                    _ToDate = value;
                    RaisePropertyChanged("ToDate");
                }
            }
        }


        private Nullable<Int64> _V_PCLRequestStatus;
        public Nullable<Int64> V_PCLRequestStatus
        {
            get { return _V_PCLRequestStatus; }
            set
            {
                if (_V_PCLRequestStatus != value)
                {
                    _V_PCLRequestStatus = value;
                    RaisePropertyChanged("V_PCLRequestStatus");
                }
            }
        }


        private Nullable<Int64> _V_ExamRegStatus;
        public Nullable<Int64> V_ExamRegStatus
        {
            get
            {
                return _V_ExamRegStatus;
            }
            set
            {
                _V_ExamRegStatus = value;
                RaisePropertyChanged("V_ExamRegStatus");
            }
        }

        private String _PatientCode;
        public String PatientCode
        {
            get
            {
                return _PatientCode;
            }
            set
            {
                _PatientCode = value;
                RaisePropertyChanged("PatientCode");
            }
        }


        private String _FullName;
        public String FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                _FullName = value;
                RaisePropertyChanged("FullName");
            }
        }


        private String _PCLRequestNumID;
        public String PCLRequestNumID
        {
            get
            {
                return _PCLRequestNumID;
            }
            set
            {
                _PCLRequestNumID = value;
                RaisePropertyChanged("PCLRequestNumID");
            }
        }
        #endregion


        private AllLookupValues.PatientFindBy _PatientFindBy;
        public AllLookupValues.PatientFindBy PatientFindBy
        {
            get { return _PatientFindBy; }
            set
            {
                if (_PatientFindBy != value)
                {
                    _PatientFindBy = value;
                    RaisePropertyChanged("PatientFindBy");
                }
            }
        }


        private Int64 _V_Param;
        public Int64 V_Param
        {
            get { return _V_Param; }
            set
            {
                _V_Param = value;
                RaisePropertyChanged("V_Param");
            }
        }

        private long? _PatientPCLReqID;
        public long? PatientPCLReqID
        {
            get => _PatientPCLReqID; set
            {
                _PatientPCLReqID = value;
                RaisePropertyChanged("PatientPCLReqID");
            }
        }


        private long _HosClientID;
        public long HosClientID
        {
            get
            {
                return _HosClientID;
            }
            set
            {
                if (_HosClientID != value)
                {
                    _HosClientID = value;
                    RaisePropertyChanged("HosClientID");
                }
            }
        }
        private long _V_TransactionStatus;
        public long V_TransactionStatus
        {
            get { return _V_TransactionStatus; }
            set
            {
                _V_TransactionStatus = value;
                RaisePropertyChanged("V_TransactionStatus");
            }
        }

        private long _V_SendSMSStatus;
        public long V_SendSMSStatus
        {
            get { return _V_SendSMSStatus; }
            set
            {
                _V_SendSMSStatus = value;
                RaisePropertyChanged("V_SendSMSStatus");
            }
        }

        private long _V_LabType;
        public long V_LabType
        {
            get { return _V_LabType; }
            set
            {
                _V_LabType = value;
                RaisePropertyChanged("V_LabType");
            }
        }
    }
}