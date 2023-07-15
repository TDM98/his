using System;
using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public enum InPatientRegistrationViewTab
    {
        EDITING_BILLING_INVOICE = 0,
        OLD_BILLING_INVOICES = 1
    }

    public enum PatientInfoTabs
    {
        GENERAL_INFO = 0,
        HEALTH_INSURANCE_INFO = 1,
        PAPER_REFERRAL_INFO = 2,
        IMAGE_STORE = 3
    }

    /// <summary>
    /// Trạng thái của form Thông tin bệnh nhân.
    /// </summary>
    public enum ActivationMode
    {
        CREATE_PATIENT = 0,
        NEW_PATIENT_FOR_NEW_REGISTRATION,
        CONFIRM_HEALTH_INSURANCE,
        CONFIRM_PAPER_REFERAL,
        VIEW_AND_EDIT_PATIENT,
        EDIT_PATIENT_FOR_REGISTRATION,
        EDIT_PATIENT_GENERAL_INFO,
        PATIENT_GENERAL_HI_VIEW,
        REGISTRATION_CONFIRM_HI,
        CREATE_NEW_PATIENT_DETAILS_ONLY 
    }

    /// <summary>
    /// Trang thai cua form dang ky benh nhan
    /// </summary>
    public enum RegistrationFormMode
    {
        PATIENT_NOT_SELECTED = 0,
        OLD_REGISTRATION_OPENED = 1,
        OLD_REGISTRATION_CHANGED = 2,
        NEW_REGISTRATION_OPENED = 3,
        NEW_REGISTRATION_CHANGED = 4,
        REGISTRATION_SAVED = 5,
        REGISTRATION_SAVED_AND_PAID = 6,
        REGISTRATION_PAID = 7,
        PATIENT_SELECTED = 8
    }

    [Flags]
    public enum SearchRegButtonsVisibility
    {
        SHOW_SEARCH_REG_BTN = 1,
        SHOW_SEARCH_PATIENT_BTN = 2,
        SHOW_SEARCH_APPOINTMENT = 4,
        SHOW_NEW_PATIENT_BTN = 8
    }
    public enum SearchRegistrationButtons
    {
        SEARCH_REGISTRATION = 1,
        SEARCH_PATIENT = 2,
        SEARCH_APPOINTMENT = 3,
        CREATE_NEWPATIENT = 4
    }

    public enum ConsultationFormMode
    {
        ModeCRUD = 0,
        ModeRUD=1
    }
}
