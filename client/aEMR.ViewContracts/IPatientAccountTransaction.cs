using DataEntities;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Xử lý tạm ứng tiền cho bệnh nhân.
    /// </summary>
    public interface IPatientAccountTransaction
    {
        ISearchPatientAndRegistration SearchRegistrationContent { get; set; }
        IPatientSummaryInfoV3 PatientSummaryInfoContent { get; set; }
        IPatientPayment OldPaymentContent { get; set; }
        Patient CurrentPatient { get; set; }
        decimal? PayAmount { get; set; }
        void PayCmd();
        bool CanPayCmd { get;}
        bool IsLoadingPatient { get; set; }
        bool IsLoadingRegistration { get; set; }

        string DeptLocTitle { get; set; }

        AllLookupValues.PatientFindBy PatientFindBy { get; set; }

    }
}
