using System;
using eHCMS.Services.Core.Base;
using System.Runtime.Serialization;
using System.Text;
using eHCMSLanguage;

namespace DataEntities
{
    public class DTDTReport : NotifyChangedBase
    {
        [DataMemberAttribute()]
        public long DTDTReportID
        {
            get
            {
                return _DTDTReportID;
            }
            set
            {
                if (_DTDTReportID != value)
                {
                    _DTDTReportID = value;
                    RaisePropertyChanged("DTDTReportID");
                }
            }
        }
        private long _DTDTReportID;
        [DataMemberAttribute()]
        public long DTDTReportOutPt
        {
            get
            {
                return _DTDTReportOutPt;
            }
            set
            {
                if (_DTDTReportOutPt != value)
                {
                    _DTDTReportOutPt = value;
                    RaisePropertyChanged("DTDTReportOutPt");
                }
            }
        }
        private long _DTDTReportOutPt;
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
        public string CheckSum
        {
            get
            {
                return _CheckSum;
            }
            set
            {
                _CheckSum = value;
                RaisePropertyChanged("CheckSum");
            }
        }
        private string _CheckSum;

        private long _V_RegistrationType;
        [DataMemberAttribute()]
        public long V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
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
    }
}
