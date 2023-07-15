using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aEMR.DataAccessLayer.Providers
{
    public static class CommonProvider
    {
        public static LookupProvider Lookups
        {
            get
            {
                return LookupProvider.Instance;
            }
        }

        public static CountryProvider Countries
        {
            get
            {
                return CountryProvider.Instance;
            }
        }
        public static StaffProvider Staffs
        {
            get
            {
                return StaffProvider.Instance;
            }
        }
        public static AppConfigsProvider AppConfigs
        {
            get
            {
                return AppConfigsProvider.Instance;
            }
        }
        public static PaymentProvider Payments
        {
            get
            {
                return PaymentProvider.Instance;
            }
        }
        public static PtRegistrationProvider PatientRg
        {
            get
            {
                return PtRegistrationProvider.Instance;
            }
        }

    }
}
