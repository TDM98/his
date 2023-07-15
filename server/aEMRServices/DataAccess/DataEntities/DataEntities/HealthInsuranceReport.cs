using System;
using eHCMS.Services.Core.Base;
using System.Runtime.Serialization;
using System.Text;
using eHCMSLanguage;

namespace DataEntities
{
    public class HealthInsuranceReport : NotifyChangedBase
    {
        [DataMemberAttribute()]
        public long HIReportID
        {
            get
            {
                return _HIReportID;
            }
            set
            {
                if (_HIReportID != value)
                {
                    _HIReportID = value;
                    RaisePropertyChanged("HIReportID");
                }
            }
        }
        private long _HIReportID;
        [DataMemberAttribute()]
        public long HIReportOutPt
        {
            get
            {
                return _HIReportOutPt;
            }
            set
            {
                if (_HIReportOutPt != value)
                {
                    _HIReportOutPt = value;
                    RaisePropertyChanged("HIReportOutPt");
                }
            }
        }
        private long _HIReportOutPt;

        [DataMemberAttribute()]
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }
        private string _Title;

        [DataMemberAttribute()]
        public DateTime FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                if (_FromDate != value)
                {
                    _FromDate = value;
                    RaisePropertyChanged("FromDate");
                }
            }
        }
        private DateTime _FromDate;

        [DataMemberAttribute()]
        public DateTime ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                if (_ToDate != value)
                {
                    _ToDate = value;
                    RaisePropertyChanged("ToDate");
                }
            }
        }
        private DateTime _ToDate;

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
                    RaisePropertyChanged("Staff");
                }
            }
        }
        private Staff _Staff;

        [DataMemberAttribute()]
        public int Month
        {
            get
            {
                return _Month;
            }
            set
            {
                if (_Month != value)
                {
                    _Month = value;
                    RaisePropertyChanged("Month");
                }
            }
        }
        private int _Month;

        [DataMemberAttribute()]
        public int Quarter
        {
            get
            {
                return _Quarter;
            }
            set
            {
                if (_Quarter != value)
                {
                    _Quarter = value;
                    RaisePropertyChanged("Quarter");
                }
            }
        }
        private int _Quarter;

        [DataMemberAttribute()]
        public int Year
        {
            get
            {
                return _Year;
            }
            set
            {
                if (_Year != value)
                {
                    _Year = value;
                    RaisePropertyChanged("Year");
                }
            }
        }
        private int _Year;

        [DataMemberAttribute()]
        public Lookup V_HIReportType
        {
            get
            {
                return _V_HIReportType;
            }
            set
            {
                if (_V_HIReportType != value)
                {
                    _V_HIReportType = value;
                    RaisePropertyChanged("V_HIReportType");
                }
            }
        }
        private Lookup _V_HIReportType;

        [DataMemberAttribute()]
        public bool IncludeBeforeFromDate
        {
            get
            {
                return _IncludeBeforeFromDate;
            }
            set
            {
                if (_IncludeBeforeFromDate != value)
                {
                    _IncludeBeforeFromDate = value;
                    RaisePropertyChanged("IncludeBeforeFromDate");
                }
            }
        }
        private bool _IncludeBeforeFromDate;

        [DataMemberAttribute()]
        public string RegistrationIDList
        {
            get
            {
                return _RegistrationIDList;
            }
            set
            {
                _RegistrationIDList = value;
                RaisePropertyChanged("RegistrationIDList");
            }
        }
        private string _RegistrationIDList;

        private long _V_ReportStatus = 82400;
        [DataMemberAttribute()]
        public long V_ReportStatus
        {
            get
            {
                return _V_ReportStatus;
            }
            set
            {
                _V_ReportStatus = value;
                RaisePropertyChanged("V_ReportStatus");
            }
        }

        [DataMemberAttribute()]
        public string V_ReportStatus_Name
        {
            get
            {
                if (V_ReportStatus == (long)AllLookupValues.V_ReportStatus.Completed)
                {
                    return eHCMSResources.Z2651_G1_ChuyenDuLieuHoanTat;
                }
                else if (V_ReportStatus == (long)AllLookupValues.V_ReportStatus.NotReported)
                {
                    return eHCMSResources.K2239_G1_ChuaBC;
                }
                else
                {
                    return eHCMSResources.Z1116_G1_ChuaXacDinh;
                }
            }
        }
        [DataMemberAttribute()]
        public bool IsTransferCompleted
        {
            get
            {
                if (V_ReportStatus == (long)AllLookupValues.V_ReportStatus.Completed || V_ReportStatus == (long)AllLookupValues.V_ReportStatus.Pending)
                {
                    return true;
                }
                return false;
            }
        }

        private string _ReportAppliedCode;
        [DataMemberAttribute()]
        public string ReportAppliedCode
        {
            get
            {
                return _ReportAppliedCode;
            }
            set
            {
                _ReportAppliedCode = value;
                RaisePropertyChanged("ReportAppliedCode");
            }
        }
        private bool _IsReportAppliedCode;
        [DataMemberAttribute()]
        public bool IsReportAppliedCode
        {
            get
            {
                return _IsReportAppliedCode;
            }
            set
            {
                _IsReportAppliedCode = value;
                RaisePropertyChanged("IsReportAppliedCode");
            }
        }

        private int _ReportAppliedResultCode;
        [DataMemberAttribute()]
        public int ReportAppliedResultCode
        {
            get
            {
                return _ReportAppliedResultCode;
            }
            set
            {
                _ReportAppliedResultCode = value;
                RaisePropertyChanged("ReportAppliedResultCode");
            }
        }

        public static string ConvertIDListToXml(string RegistrationIDList)
        {
            if (!string.IsNullOrEmpty(RegistrationIDList))
            {
                string[] RegistrationIDCollection = RegistrationIDList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                StringBuilder sb = new StringBuilder();
                sb.Append("<RegistrationIDList>");
                foreach (string aRegistrationID in RegistrationIDCollection)
                {
                    if (string.IsNullOrEmpty(aRegistrationID)) continue;
                    string[] RegistrationInf = aRegistrationID.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    if (RegistrationInf.Length == 2)
                    {
                        sb.Append("<RegistrationID>");
                        sb.AppendFormat("<RegistrationType>{0}</RegistrationType>", RegistrationInf[0]);
                        sb.AppendFormat("<RegistrationID>{0}</RegistrationID>", RegistrationInf[1]);
                        sb.Append("</RegistrationID>");
                    }
                    else
                        throw new Exception(eHCMSResources.Z2200_G1_MaDotDieuTriKhongDungDD);
                }
                sb.Append("</RegistrationIDList>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        private bool _IsAutoCreateHIReport;
        [DataMemberAttribute()]
        public bool IsAutoCreateHIReport
        {
            get
            {
                return _IsAutoCreateHIReport;
            }
            set
            {
                _IsAutoCreateHIReport = value;
                RaisePropertyChanged("IsAutoCreateHIReport");
            }
        }
    }
}