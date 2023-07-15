using DataEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aEMR.Infrastructure.Events.SL_Events
{
    public class Circulars56Event
    {
        public SummaryMedicalRecords SavedSummaryMedicalRecords { get; set; }
        public PatientTreatmentCertificates SavedPatientTreatmentCertificates_NgT { get; set; }
        public PatientTreatmentCertificates SavedPatientTreatmentCertificates_NT { get; set; }
        public InjuryCertificates SavedInjuryCertificates_NgT { get; set; }
        public InjuryCertificates SavedInjuryCertificates_NT { get; set; }
    }
}
