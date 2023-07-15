using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public partial class InsuranceBenefitCategories_Data : NotifyChangedBase
    {
        public static InsuranceBenefitCategories_Data CreateInsuranceBenefitCategories_Data(int IBeID, string HIPCode, decimal RebatePercentage)
        {
            InsuranceBenefitCategories_Data IBE = new InsuranceBenefitCategories_Data();
            IBE.IBeID = IBeID;
            IBE.HIPCode = HIPCode;
            IBE.RebatePercentage = RebatePercentage;
            return IBE;
        }
        [DataMemberAttribute()]
        public int IBeID
        {
            get
            {
                return _IBeID;
            }
            set
            {
                if (_IBeID != value)
                {
                    OnIBeIDChanging(value);
                    ////ReportPropertyChanging("IBID");
                    _IBeID = value;
                    RaisePropertyChanged("IBeID");
                    OnIBeIDChanged();
                }
            }
        }
        private int _IBeID;
        partial void OnIBeIDChanging(int value);
        partial void OnIBeIDChanged();

        [DataMemberAttribute()]
        public string HIPCode
        {
            get
            {
                return _HIPCode;
            }
            set
            {
                if (_HIPCode != value)
                {
                    OnHIPCodeChanging(value);
                    ////ReportPropertyChanging("IBID");
                    _HIPCode = value;
                    RaisePropertyChanged("HIPCode");
                    OnIBeIDChanged();
                }
            }
        }
        private string _HIPCode;
        partial void OnHIPCodeChanging(string value);
        partial void OnHIPCodeChanged();



        private string _BenefitCode;
        [DataMemberAttribute]
        public string BenefitCode
        {
            get
            {
                return _BenefitCode;
            }
            set
            {
                if (_BenefitCode != value)
                {
                    OnBenefitCodeChanging(value);
                    ////ReportPropertyChanging("IBID");
                    _BenefitCode = value;
                    RaisePropertyChanged("HIPCode");
                    OnBenefitCodeChanged();
                }
            }
        }
        partial void OnBenefitCodeChanging(string value);
        partial void OnBenefitCodeChanged();

        private decimal _RebatePercentage;
        [DataMemberAttribute]
        public decimal RebatePercentage
        {
            get
            {
                return _RebatePercentage;
            }
            set
            {
                if (_RebatePercentage != value)
                {
                    OnRebatePercentageChanging(value);
                    ////ReportPropertyChanging("IBID");
                    _RebatePercentage = value;
                    RaisePropertyChanged("HIPCode");
                    OnRebatePercentageChanged();
                }

            }
        }
        partial void OnRebatePercentageChanging(decimal value);
        partial void OnRebatePercentageChanged();

        private bool _IsActive;
        [DataMemberAttribute]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                if (_IsActive != value)
                {
                    OnIsActiveChanging(value);
                    ////ReportPropertyChanging("IBID");
                    _IsActive = value;
                    RaisePropertyChanged("IsActive");
                    OnIsActiveChanged();
                }

            }
        }
        partial void OnIsActiveChanging(bool value);
        partial void OnIsActiveChanged();

        private DateTime _DateModified;
        [DataMemberAttribute]
        public DateTime DateModified
        {
            get
            {
                return _DateModified;
            }
            set
            {
                if (_DateModified != value)
                {
                    OnDateModifiedChanging(value);
                    ////ReportPropertyChanging("IBID");
                    _DateModified = value;
                    RaisePropertyChanged("DateModified");
                    OnDateModifiedChanged();
                }
            }
        }
        partial void OnDateModifiedChanging(DateTime value);
        partial void OnDateModifiedChanged();


        private string _ModifiedLog;
        [DataMemberAttribute]
        public string ModifiedLog
        {
            get
            {
                return _ModifiedLog;
            }
            set
            {
                if (_ModifiedLog != value)
                {
                    OnModifiedLogChanging(value);
                    ////ReportPropertyChanging("IBID");
                    _ModifiedLog = value;
                    RaisePropertyChanged("ModifiedLog");
                    OnModifiedLogChanged();
                }
            }
        }
        partial void OnModifiedLogChanging(string value);
        partial void OnModifiedLogChanged();
        public string Notes { get; set; }
        public string HIPName { get; set; }
        public int HIPGroup { get; set; }
    }
}
