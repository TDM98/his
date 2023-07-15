using System;
using DataEntities;
using eHCMS.Configurations;

namespace aEMR.DataAccessLayer.Providers
{
    public abstract class FunctionBase : DataAccess
    {
        public int GetStartSequenceNumber(DateTime curDate, ConsultationRoomTarget crt)
        {
            switch (curDate.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return crt.MondayStartSequenceNumber;

                case DayOfWeek.Tuesday:
                    return crt.TuesdayStartSequenceNumber;

                case DayOfWeek.Wednesday:
                    return crt.WednesdayStartSequenceNumber ;

                case DayOfWeek.Thursday:
                    return crt.ThursdayStartSequenceNumber;

                case DayOfWeek.Friday:
                    return crt.FridayStartSequenceNumber;

                case DayOfWeek.Saturday:
                    return crt.SaturdayStartSequenceNumber;

                case DayOfWeek.Sunday:
                    return crt.SundayStartSequenceNumber;
                default: return 0;
            }
        }
        public static void CorrectRegistrationDetails(PatientRegistration CurrentRegistration)
        {
            CurrentRegistration.CorrectRegistrationDetails(Globals.AxServerSettings.HealthInsurances.SpecialRuleForHIConsultationApplied, null, Globals.AxServerSettings.HealthInsurances.HIPercentOnDifDept, Globals.AxServerSettings.HealthInsurances.HiPolicyMinSalary, Globals.AxServerSettings.PharmacyElements.OnlyRoundResultForOutward, Globals.AxServerSettings.HealthInsurances.FullHIBenefitForConfirm, true, Globals.AxServerSettings.CommonItems.AddingServicesPercent, Globals.AxServerSettings.HealthInsurances.FullHIOfServicesForConfirm
                , Globals.AxServerSettings.CommonItems.AddingHIServicesPercent);
        }
        public static void CorrectRegistrationDetails_V2(PatientRegistration CurrentRegistration)
        {
            CurrentRegistration.CorrectRegistrationDetails_V2(Globals.AxServerSettings.HealthInsurances.SpecialRuleForHIConsultationApplied, null,
                Globals.AxServerSettings.HealthInsurances.HIPercentOnDifDept, Globals.AxServerSettings.HealthInsurances.HiPolicyMinSalary,
                Globals.AxServerSettings.PharmacyElements.OnlyRoundResultForOutward, Globals.AxServerSettings.CommonItems.AddingServicesPercent,
                Globals.AxServerSettings.CommonItems.AddingHIServicesPercent, Globals.AxServerSettings.HealthInsurances.FullHIOfServicesForConfirm,
                Globals.AxServerSettings.HealthInsurances.PercentForEkip, Globals.AxServerSettings.HealthInsurances.PercentForOtherEkip);
        }
    }
}