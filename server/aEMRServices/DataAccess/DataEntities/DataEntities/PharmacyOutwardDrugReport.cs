using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text;

namespace DataEntities
{
    public partial class PharmacyOutwardDrugReport : NotifyChangedBase
    {
        #region Factory Method

        /// Create a new PharmacyOutwardDrugReport object.
        /// <param name="PharmacyOutRepID">Initial value of the PharmacyOutRepID property.</param>

        public static PharmacyOutwardDrugReport CreatePharmacyOutwardDrugReport(long PharmacyOutRepID)
        {
            PharmacyOutwardDrugReport PharmacyOutwardDrugReport = new PharmacyOutwardDrugReport();
            PharmacyOutwardDrugReport.PharmacyOutRepID = PharmacyOutRepID;
            return PharmacyOutwardDrugReport;
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long PharmacyOutRepID
        {
            get
            {
                return _PharmacyOutRepID;
            }
            set
            {
                if (_PharmacyOutRepID != value)
                {
                    OnPharmacyOutRepIDChanging(value);
                    _PharmacyOutRepID = value;
                    RaisePropertyChanged("PharmacyOutRepID");
                    RaisePropertyChanged("CanSave");
                    RaisePropertyChanged("CanPrint");
                    OnPharmacyOutRepIDChanged();
                }
            }
        }
        private long _PharmacyOutRepID;
        partial void OnPharmacyOutRepIDChanging(long value);
        partial void OnPharmacyOutRepIDChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> ReportDate
        {
            get
            {
                return _ReportDate;
            }
            set
            {
                OnReportDateChanging(value);
                _ReportDate = value;
                RaisePropertyChanged("ReportDate");
                OnReportDateChanged();
            }
        }
        private Nullable<DateTime> _ReportDate = DateTime.Now;
        partial void OnReportDateChanging(Nullable<DateTime> value);
        partial void OnReportDateChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                OnRecCreatedDateChanging(value);
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
                OnRecCreatedDateChanged();
            }
        }
        private Nullable<DateTime> _RecCreatedDate = DateTime.Now;
        partial void OnRecCreatedDateChanging(Nullable<DateTime> value);
        partial void OnRecCreatedDateChanged();

        [DataMemberAttribute()]
        public Int64 RepStaffID
        {
            get
            {
                return _RepStaffID;
            }
            set
            {
                OnRepStaffIDChanging(value);
                _RepStaffID = value;
                RaisePropertyChanged("RepStaffID");
                OnRepStaffIDChanged();
            }
        }
        private Int64 _RepStaffID;
        partial void OnRepStaffIDChanging(Int64 value);
        partial void OnRepStaffIDChanged();

        [DataMemberAttribute()]
        public Int64 V_PharmacyOutRepType
        {
            get
            {
                return _V_PharmacyOutRepType;
            }
            set
            {
                if (_V_PharmacyOutRepType != value)
                {
                    OnV_PharmacyOutRepTypeChanging(value);
                    _V_PharmacyOutRepType = value;
                    RaisePropertyChanged("V_PharmacyOutRepType");
                    OnV_PharmacyOutRepTypeChanged();
                }
            }
        }
        private Int64 _V_PharmacyOutRepType;
        partial void OnV_PharmacyOutRepTypeChanging(Int64 value);
        partial void OnV_PharmacyOutRepTypeChanged();

        [DataMemberAttribute()]
        public string V_PharmacyOutRepTypeName
        {
            get
            {
                return _V_PharmacyOutRepTypeName;
            }
            set
            {
                if (_V_PharmacyOutRepTypeName != value)
                {
                    OnV_PharmacyOutRepTypeNameChanging(value);
                    _V_PharmacyOutRepTypeName = value;
                    RaisePropertyChanged("V_PharmacyOutRepTypeName");
                    OnV_PharmacyOutRepTypeNameChanged();
                }
            }
        }
        private string _V_PharmacyOutRepTypeName;
        partial void OnV_PharmacyOutRepTypeNameChanging(string value);
        partial void OnV_PharmacyOutRepTypeNameChanged();


        [DataMemberAttribute()]
        public DateTime RepDateFrom
        {
            get
            {
                return _RepDateFrom;
            }
            set
            {
                OnRepDateFromChanging(value);
                _RepDateFrom = value;
                RaisePropertyChanged("RepDateFrom");
                OnRepDateFromChanged();
            }
        }
        private DateTime _RepDateFrom = DateTime.Now;
        partial void OnRepDateFromChanging(DateTime value);
        partial void OnRepDateFromChanged();

        [DataMemberAttribute()]
        public DateTime RepDateTo
        {
            get
            {
                return _RepDateTo;
            }
            set
            {
                OnRepDateToChanging(value);
                _RepDateTo = value;
                RaisePropertyChanged("RepDateTo");
                OnRepDateToChanged();
            }
        }
        private DateTime _RepDateTo = DateTime.Now;
        partial void OnRepDateToChanging(DateTime value);
        partial void OnRepDateToChanged();

        [DataMemberAttribute()]
        public String Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
                RaisePropertyChanged("Title");
            }
        }
        private String _Title;

        [DataMemberAttribute()]
        public String FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                OnFullNameChanging(value);
                _FullName = value;
                RaisePropertyChanged("FullName");
                OnFullNameChanged();
            }
        }
        private String _FullName;
        partial void OnFullNameChanging(String value);
        partial void OnFullNameChanged();


        [DataMemberAttribute()]
        private decimal _OutAmount;
        public decimal OutAmount
        {
            get
            {
                return _OutAmount;
            }
            set
            {
                _OutAmount = value;
                RaisePropertyChanged("OutAmount");
            }
        }

        [DataMemberAttribute()]
        private decimal _OutHIRebate;
        public decimal OutHIRebate
        {
            get
            {
                return _OutHIRebate;
            }
            set
            {
                _OutHIRebate = value;
                RaisePropertyChanged("OutHIRebate");
            }
        }

        [DataMemberAttribute()]
        private decimal _PatientPayment;
        public decimal PatientPayment
        {
            get
            {
                return _PatientPayment;
            }
            set
            {
                _PatientPayment = value;
                RaisePropertyChanged("PatientPayment");
            }
        }

        [DataMemberAttribute()]
        private decimal _AmountCoPay;
        public decimal AmountCoPay
        {
            get
            {
                return _AmountCoPay;
            }
            set
            {
                _AmountCoPay = value;
                RaisePropertyChanged("AmountCoPay");
            }
        }

        [DataMemberAttribute()]
        private decimal _ThucThu;
        public decimal ThucThu
        {
            get
            {
                return _ThucThu;
            }
            set
            {
                _ThucThu = value;
                RaisePropertyChanged("ThucThu");
            }
        }

        [DataMemberAttribute()]
        private decimal _ThucNop;
        public decimal ThucNop
        {
            get
            {
                return _ThucNop;
            }
            set
            {
                _ThucNop = value;
                RaisePropertyChanged("ThucNop");
            }
        }


        [DataMemberAttribute()]
        private bool _Checked;
        public bool Checked
        {
            get
            {
                return _Checked;
            }
            set
            {
                _Checked = value;
                RaisePropertyChanged("Checked");
            }
        }
        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        private ObservableCollection<PharmacyOutwardDrugReportDetail> _PharmacyOutwardDrugReportDetails;
        public ObservableCollection<PharmacyOutwardDrugReportDetail> PharmacyOutwardDrugReportDetails
        {
            get
            {
                return _PharmacyOutwardDrugReportDetails;
            }
            set
            {
                if (_PharmacyOutwardDrugReportDetails != value)
                {
                    _PharmacyOutwardDrugReportDetails = value;
                    RaisePropertyChanged("PharmacyOutwardDrugReportDetails");
                }
            }
        }
        #endregion

        #region Convert XML
        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_PharmacyOutwardDrugReportDetails);
        }
        public string ConvertDetailsListToXml(IEnumerable<PharmacyOutwardDrugReportDetail> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<PharmacyOutwardDrugReportDetails>");
                foreach (PharmacyOutwardDrugReportDetail details in items)
                {
                    if (details.IsChecked && details.OutiID > 0)
                    {
                        //int EntityState = (int)details.EntityState;
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<PharmacyOutRepDetID>{0}</PharmacyOutRepDetID>", details.PharmacyOutRepDetID);
                        sb.AppendFormat("<PharmacyOutRepID>{0}</PharmacyOutRepID>", details.PharmacyOutRepID);
                        sb.AppendFormat("<OutiID>{0}</OutiID>", details.OutiID);
                        //sb.AppendFormat("<EntityState>{0}</EntityState>", EntityState);
                        sb.Append("</RecInfo>");
                    }
                }
                sb.Append("</PharmacyOutwardDrugReportDetails>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        #endregion

        public override bool Equals(object obj)
        {
            PharmacyOutwardDrugReport seletedStore = obj as PharmacyOutwardDrugReport;
            if (seletedStore == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PharmacyOutRepID == seletedStore.PharmacyOutRepID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool CanSave
        {
            get { return PharmacyOutRepID <= 0; }
        }
        public bool CanPrint
        {
            get { return PharmacyOutRepID > 0; }
        }

        [DataMemberAttribute]
        public long StoreID { get => _StoreID; set
            {
                _StoreID = value;
                RaisePropertyChanged("StoreID");
            }
        }
        private long _StoreID;
    }

    public partial class PharmacyOutwardDrugReportDetail : NotifyChangedBase
    {
        #region Factory Method
        /// Create a new PharmacyOutwardDrugReportDetail object.
        /// <param name="PharmacyOutRepDetID">Initial value of the PharmacyOutRepDetID property.</param>
        public static PharmacyOutwardDrugReportDetail CreatePharmacyOutwardDrugReportDetail(Int64 PharmacyOutRepDetID)
        {
            PharmacyOutwardDrugReportDetail PharmacyOutwardDrugReportDetail = new PharmacyOutwardDrugReportDetail();
            PharmacyOutwardDrugReportDetail.PharmacyOutRepDetID = PharmacyOutRepDetID;
            return PharmacyOutwardDrugReportDetail;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 PharmacyOutRepDetID
        {
            get
            {
                return _PharmacyOutRepDetID;
            }
            set
            {
                if (_PharmacyOutRepDetID != value)
                {
                    OnPharmacyOutRepDetIDChanging(value);
                    _PharmacyOutRepDetID = value;
                    RaisePropertyChanged("PharmacyOutRepDetID");
                    OnPharmacyOutRepDetIDChanged();
                }
            }
        }
        private Int64 _PharmacyOutRepDetID;
        partial void OnPharmacyOutRepDetIDChanging(Int64 value);
        partial void OnPharmacyOutRepDetIDChanged();


        [DataMemberAttribute()]
        public Int64 PharmacyOutRepID
        {
            get
            {
                return _PharmacyOutRepID;
            }
            set
            {
                if (_PharmacyOutRepID != value)
                {
                    OnPharmacyOutRepIDChanging(value);
                    _PharmacyOutRepID = value;
                    RaisePropertyChanged("PharmacyOutRepID");
                    OnPharmacyOutRepIDChanged();

                }
            }
        }
        private Int64 _PharmacyOutRepID;
        partial void OnPharmacyOutRepIDChanging(Int64 value);
        partial void OnPharmacyOutRepIDChanged();

        [DataMemberAttribute()]
        public Int64 OutiID
        {
            get
            {
                return _OutiID;
            }
            set
            {
                if (_OutiID != value)
                {
                    OnOutiIDChanging(value);
                    _OutiID = value;
                    RaisePropertyChanged("OutiID");
                    OnOutiIDChanged();
                }
            }
        }
        private Int64 _OutiID;
        partial void OnOutiIDChanging(Int64 value);
        partial void OnOutiIDChanged();

        #endregion

        #region ext member

        [DataMemberAttribute()]
        public String OutInvID
        {
            get
            {
                return _OutInvID;
            }
            set
            {
                OnOutInvIDChanging(value);
                _OutInvID = value;
                RaisePropertyChanged("OutInvID");
                OnOutInvIDChanged();
            }
        }
        private String _OutInvID;
        partial void OnOutInvIDChanging(String value);
        partial void OnOutInvIDChanged();

        [DataMemberAttribute()]
        public String OutInvIDString
        {
            get
            {
                return _OutInvIDString;
            }
            set
            {
                _OutInvIDString = value;
                RaisePropertyChanged("OutInvIDString");
            }
        }
        private String _OutInvIDString;

        [DataMemberAttribute()]
        public Nullable<long> StoreID
        {
            get
            {
                return _StoreID;
            }
            set
            {
                OnStoreIDChanging(value);
                _StoreID = value;
                RaisePropertyChanged("StoreID");
                OnStoreIDChanged();
            }
        }
        private Nullable<long> _StoreID;
        partial void OnStoreIDChanging(Nullable<long> value);
        partial void OnStoreIDChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private Nullable<Int64> _StaffID;
        partial void OnStaffIDChanging(Nullable<Int64> value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public String FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                OnFullNameChanging(value);
                _FullName = value;
                RaisePropertyChanged("FullName");
                OnFullNameChanged();
            }
        }
        private String _FullName;
        partial void OnFullNameChanging(String value);
        partial void OnFullNameChanged();

        [DataMemberAttribute()]
        public String CustomerName
        {
            get
            {
                return _CustomerName;
            }
            set
            {
                OnCustomerNameChanging(value);
                _CustomerName = value;
                RaisePropertyChanged("CustomerName");
                OnCustomerNameChanged();
            }
        }
        private String _CustomerName;
        partial void OnCustomerNameChanging(String value);
        partial void OnCustomerNameChanged();

        [DataMemberAttribute()]
        private decimal _TotalCost;
        public decimal TotalCost
        {
            get
            {
                return _TotalCost;
            }
            set
            {
                _TotalCost = value;
                RaisePropertyChanged("TotalCost");
            }
        }

        [DataMemberAttribute()]
        private double _PtInsuranceBenefit;
        public double PtInsuranceBenefit
        {
            get
            {
                return _PtInsuranceBenefit;
            }
            set
            {
                _PtInsuranceBenefit = value;
                RaisePropertyChanged("PtInsuranceBenefit");
            }
        }

        [DataMemberAttribute()]
        private decimal _OutAmount;
        public decimal OutAmount
        {
            get
            {
                return _OutAmount;
            }
            set
            {
                _OutAmount = value;
                RaisePropertyChanged("OutAmount");
            }
        }

        [DataMemberAttribute()]
        private decimal _OutHIRebate;
        public decimal OutHIRebate
        {
            get
            {
                return _OutHIRebate;
            }
            set
            {
                _OutHIRebate = value;
                RaisePropertyChanged("OutHIRebate");
            }
        }

        [DataMemberAttribute()]
        private decimal _PatientPayment;
        public decimal PatientPayment
        {
            get
            {
                return _PatientPayment;
            }
            set
            {
                _PatientPayment = value;
                RaisePropertyChanged("PatientPayment");
            }
        }

        [DataMemberAttribute()]
        private decimal _AmountCoPay;
        public decimal AmountCoPay
        {
            get
            {
                return _AmountCoPay;
            }
            set
            {
                _AmountCoPay = value;
                RaisePropertyChanged("AmountCoPay");
            }
        }

        [DataMemberAttribute()]
        private bool _IsChecked = true;
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
        #endregion
    }
}
