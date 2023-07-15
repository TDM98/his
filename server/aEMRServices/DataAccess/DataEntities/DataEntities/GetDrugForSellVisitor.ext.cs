using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using eHCMS.Services.Core;
using System.ComponentModel;

namespace DataEntities
{
    public partial class GetDrugForSellVisitor
    {
        [DataMemberAttribute()]
        public string Packaging
        {
            get
            {
                return _Packaging;
            }
            set
            {
                _Packaging = value;
                RaisePropertyChanged("Packaging");
            }
        }
        private string _Packaging;

        [DataMemberAttribute()]
        public string Visa
        {
            get
            {
                return _Visa;
            }
            set
            {
                _Visa = value;
                RaisePropertyChanged("Visa");
            }
        }
        private string _Visa;

        [DataMemberAttribute()]
        public string DrugCode
        {
            get
            {
                return _DrugCode;
            }
            set
            {
                _DrugCode = value;
                RaisePropertyChanged("DrugCode");
            }
        }
        private string _DrugCode;

        [DataMemberAttribute()]
        public string HIDrugCode
        {
            get
            {
                return _HIDrugCode;
            }
            set
            {
                _HIDrugCode = value;
                RaisePropertyChanged("HIDrugCode");
            }
        }
        private string _HIDrugCode;
        [DataMemberAttribute()]
        public String SdlDescription
        {
            get
            {
                return _SdlDescription;
            }
            set
            {
                OnSdlDescriptionChanging(value);
                _SdlDescription = value;
                OnSdlDescriptionChanged();
            }
        }
        private String _SdlDescription;
        partial void OnSdlDescriptionChanging(String value);
        partial void OnSdlDescriptionChanged();

        private String _Content;
        [DataMemberAttribute()]
        public String Content
        {
            get
            {
                return _Content;
            }
            set
            {
                _Content = value;
                RaisePropertyChanged("Content");
            }
        }

        private String _Administration;
        [DataMemberAttribute()]
        public String Administration
        {
            get
            {
                return _Administration;
            }
            set
            {
                _Administration = value;
                RaisePropertyChanged("Administration");
            }
        }

        [DataMemberAttribute()]
        public DateTime InExpiryDate
        {
            get
            {
                return _InExpiryDate;
            }
            set
            {
                OnInExpiryDateChanging(value);
                _InExpiryDate = value;
                OnInExpiryDateChanged();
            }
        }
        private DateTime _InExpiryDate;
        partial void OnInExpiryDateChanging(DateTime value);
        partial void OnInExpiryDateChanged();

        [DataMemberAttribute()]
        public int RemainingFirst
        {
            get
            {
                return _RemainingFirst;
            }
            set
            {
                _RemainingFirst = value;
                RaisePropertyChanged("RemainingFirst");
            }
        }
        private int _RemainingFirst;

        [DataMemberAttribute()]
        public int Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                _Qty = value;
                RaisePropertyChanged("Qty");
            }
        }
        private int _Qty;

        [DataMemberAttribute()]
        public double DayRpts
        {
            get
            {
                return _DayRpts;
            }
            set
            {
                _DayRpts = value;
                RaisePropertyChanged("DayRpts");
            }
        }
        private double _DayRpts;

        //dung cai nay de han che sai so
        [DataMemberAttribute()]
        public double QtyForDay
        {
            get
            {
                return _QtyForDay;
            }
            set
            {
                _QtyForDay = value;
                RaisePropertyChanged("QtyForDay");
            }
        }
        private double _QtyForDay;

        partial void OnRequiredNumberChanging(double value)
        {
            if (value > Remaining)
            {
                if (Remaining == 0)
                {
                    AddError("RequiredNumber", "Thuốc này không còn trong kho", false);
                }
                else
                {
                    AddError("RequiredNumber", "Số lượng xuất phải <= " + Remaining.ToString(), false);
                }
            }
            else
            {
                RemoveError("RequiredNumber", "Thuốc này không còn trong kho");
                RemoveError("RequiredNumber", "Số lượng xuất phải <= " + Remaining.ToString());
            }
        }


        [DataMemberAttribute()]
        public long? PrescriptID
        {
            get
            {
                return _PrescriptID;
            }
            set
            {
                OnPrescriptIDChanging(value);
                _PrescriptID = value;
                RaisePropertyChanged("PrescriptID");
                OnPrescriptIDChanged();
            }
        }
        private long? _PrescriptID;
        partial void OnPrescriptIDChanging(long? value);
        partial void OnPrescriptIDChanged();

        [DataMemberAttribute()]
        public long? IssueID
        {
            get
            {
                return _IssueID;
            }
            set
            {
                OnIssueIDChanging(value);
                _IssueID = value;
                RaisePropertyChanged("IssueID");
                OnIssueIDChanged();
            }
        }
        private long? _IssueID;
        partial void OnIssueIDChanging(long? value);
        partial void OnIssueIDChanged();

        [DataMemberAttribute()]
        public DateTime? IssuedDateTime
        {
            get
            {
                return _IssuedDateTime;
            }
            set
            {
                OnIssuedDateTimeChanging(value);
                _IssuedDateTime = value;
                RaisePropertyChanged("IssuedDateTime");
                OnIssuedDateTimeChanged();
            }
        }
        private DateTime? _IssuedDateTime;
        partial void OnIssuedDateTimeChanging(DateTime? value);
        partial void OnIssuedDateTimeChanged();

        [DataMemberAttribute()]
        public bool Checked
        {
            get
            {
                return _Checked;
            }
            set
            {
                OnIsCheckedChanging(value);
                _Checked = value;
                RaisePropertyChanged("Checked");
                OnIsCheckedChanged();
            }
        }
        private bool _Checked;
        partial void OnIsCheckedChanging(bool value);
        partial void OnIsCheckedChanged();

        [DataMemberAttribute()]
        public long? DrugIDChanged
        {
            get
            {
                return _DrugIDChanged;
            }
            set
            {
                _DrugIDChanged = value;
                RaisePropertyChanged("DrugIDChanged");
            }
        }
        private long? _DrugIDChanged;

        [DataMemberAttribute()]
        public string Precaution_Warn
        {
            get
            {
                return _Precaution_Warn;
            }
            set
            {
                _Precaution_Warn = value;
                RaisePropertyChanged("Precaution_Warn");
            }
        }
        private string _Precaution_Warn;

        [DataMemberAttribute()]
        public bool IsWarningHI
        {
            get
            {
                return _IsWarningHI;
            }
            set
            {
                _IsWarningHI = value;
                RaisePropertyChanged("IsWarningHI");
            }
        }
        private bool _IsWarningHI;

        [DataMemberAttribute()]
        public Int64 V_DrugType
        {
            get
            {
                return _V_DrugType;
            }
            set
            {
                _V_DrugType = value;
            }
        }
        private Int64 _V_DrugType;


        [DataMemberAttribute()]
        public double QtyMaxAllowed
        {
            get
            {
                return _QtyMaxAllowed;
            }
            set
            {
                _QtyMaxAllowed = value;
                RaisePropertyChanged("QtyMaxAllowed");
            }
        }
        private double _QtyMaxAllowed;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedMon
        {
            get
            {
                return _QtySchedMon;
            }
            set
            {
                _QtySchedMon = value;
                RaisePropertyChanged("QtySchedMon");
            }
        }
        private Nullable<Single> _QtySchedMon;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedTue
        {
            get
            {
                return _QtySchedTue;
            }
            set
            {
                _QtySchedTue = value;
                RaisePropertyChanged("QtySchedTue");
            }
        }
        private Nullable<Single> _QtySchedTue;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedWed
        {
            get
            {
                return _QtySchedWed;
            }
            set
            {
                _QtySchedWed = value;
                RaisePropertyChanged("QtySchedWed");
            }
        }
        private Nullable<Single> _QtySchedWed;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedThu
        {
            get
            {
                return _QtySchedThu;
            }
            set
            {
                _QtySchedThu = value;
                RaisePropertyChanged("QtySchedThu");
            }
        }
        private Nullable<Single> _QtySchedThu;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedFri
        {
            get
            {
                return _QtySchedFri;
            }
            set
            {
                _QtySchedFri = value;
                RaisePropertyChanged("QtySchedFri");
            }
        }
        private Nullable<Single> _QtySchedFri;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedSat
        {
            get
            {
                return _QtySchedSat;
            }
            set
            {
                _QtySchedSat = value;
                RaisePropertyChanged("QtySchedSat");
            }
        }
        private Nullable<Single> _QtySchedSat;


        [DataMemberAttribute()]
        public Nullable<Single> QtySchedSun
        {
            get
            {
                return _QtySchedSun;
            }
            set
            {
                _QtySchedSun = value;
                RaisePropertyChanged("QtySchedSun");
            }
        }
        private Nullable<Single> _QtySchedSun;


        [DataMemberAttribute()]
        public Nullable<byte> SchedBeginDOW
        {
            get
            {
                return _SchedBeginDOW;
            }
            set
            {
                _SchedBeginDOW = value;
                RaisePropertyChanged("SchedBeginDOW");
            }
        }
        private Nullable<byte> _SchedBeginDOW;


    }
}
