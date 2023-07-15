using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Input;
using DataEntities;
using aEMR.Common.Collections;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IAucHoldConsultDoctor
    {
        long StaffID { get; set; }

        string StaffName { get; set; }

        long StaffCatType { get; set; }

        bool IsMultiStaffCatType { get; set; }

        ObservableCollection<long> StaffCatTypeList { get; set; }

        void setDefault(string DefaultStaffName = null);
        string CertificateNumber { get; set; }
        bool IsDoctorOnly { get; set; }
        bool ShowStopUsing { get; set; }
        long CurrentDeptID { get; set; }

        long PCLResultParamImpID { get; set; }
    }
    public interface IDiagnosisTextBox
    {
        PatientPCLRequest CurrentPclRequest { get; set; }
    }
}
