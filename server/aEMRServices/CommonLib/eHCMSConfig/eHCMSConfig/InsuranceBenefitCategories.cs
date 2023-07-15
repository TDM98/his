using System.Collections.Generic;
using System.Runtime.Serialization;

namespace eHCMS.Configurations
{
    [DataContract]
    public class InsuranceBenefitCategories
    {
        private string _HIPCode;
        [DataMemberAttribute]
        public string HIPCode
        {
            get
            {
                return _HIPCode;
            }
            set
            {
                _HIPCode = value;
            }
        }

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
                _BenefitCode = value;
            }
        }

        private float _RebatePercentage;
        [DataMemberAttribute]
        public float RebatePercentage
        {
            get
            {
                return _RebatePercentage;
            }
            set
            {
                _RebatePercentage = value;
            }
        }

        private int _IBeID;
        [DataMemberAttribute]
        public int IBeID
        {
            get
            {
                return _IBeID;
            }
            set
            {
                _IBeID = value;
            }
        }
    }
}
