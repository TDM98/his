namespace aEMR.ViewContracts
{
    public interface IcwdPatientAdmisInfo
    {
        long PtRegistrationID { get; set; }
        IInPatientDeptListing InPatientDeptListingContent { get; set; }
        
        IPatientSummaryInfoV2 PatientSummaryInfoContent { get; set; }

        IInPatientAdmissionInfo InPatientAdmissionInfoContent { get; set; }
        IInPatientBedPatientAllocListing PatientAllocListingContent { get; set; }
    }
}
