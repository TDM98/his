namespace eHCMS.Services.Core
{
    public static class AxCodeGenerator
    {
        private static readonly object InvCodeLockObject = new object();
        private static int _invoiceNumber;
        public static string CreateInvoiceNumber()
        {
            lock (InvCodeLockObject)
            {
                return (++_invoiceNumber).ToString();
            }
        }

        private static readonly object PatientLockObject = new object();
        private static int _patientCode;

        public static string CreatePatientCode()
        {
            lock (PatientLockObject)
            {
                return (++_patientCode).ToString();
            }
        }

        private static readonly object RegistrationLockObject = new object();
        private static int _registrationCode;

        public static string CreateRegistrationCode()
        {
            lock (RegistrationLockObject)
            {
                return (++_registrationCode).ToString();
            }
        }
    }
}
