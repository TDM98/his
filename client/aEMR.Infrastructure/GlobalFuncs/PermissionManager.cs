using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DataEntities;
using Service.Core.Common;

namespace aEMR.Infrastructure.GlobalFuncs
{
    public class PermissionManager
    {
        static PermissionManager()
        {
            Authorization();
        }

        static void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            InsuranceEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mModulesGen
                                               , (int)eModuleGeneral.mInsurance,
                                               (int)oModuleGeneralEX.mInsurance, (int)ePermission.mEdit);

            InsuranceAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mModulesGen
                                               , (int)eModuleGeneral.mInsurance,
                                               (int)oModuleGeneralEX.mInsurance, (int)ePermission.mAdd);

            InsuranceDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mModulesGen
                                               , (int)eModuleGeneral.mInsurance,
                                               (int)oModuleGeneralEX.mInsurance, (int)ePermission.mDelete);

            InsuranceView = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mModulesGen
                                               , (int)eModuleGeneral.mInsurance,
                                               (int)oModuleGeneralEX.mInsurance, (int)ePermission.mView);

            InsurancePrint = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mModulesGen
                                               , (int)eModuleGeneral.mInsurance,
                                               (int)oModuleGeneralEX.mInsurance, (int)ePermission.mPrint);

            InsuranceReport = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mModulesGen
                                               , (int)eModuleGeneral.mInsurance,
                                               (int)oModuleGeneralEX.mInsurance, (int)ePermission.mReport);

            //---mark 11-10-2012
            //RegInfoDelete = Globals.listRefModule[(int)eModules.mPatient].lstFunction[(int)ePatient.mPatientRegistration].lstOperation[(int)oPatientEx.mRegInfo].mOperation != null
            //                    &&
            //                    Globals.listRefModule[(int)eModules.mPatient].lstFunction[(int)ePatient.mPatientRegistration].lstOperation[(int)oPatientEx.mRegInfo].mPermission.pDelete;
        }
        #region checking account

        private static bool _insuranceEdit = true;
        private static bool _insuranceAdd = true;
        private static bool _insuranceDelete = true;
        private static bool _insuranceView = true;
        private static bool _insurancePrint = true;
        private static bool _insuranceReport = true;

        public static bool InsuranceEdit
        {
            get
            {
                return _insuranceEdit;
            }
            private set
            {
                if (_insuranceEdit == value)
                    return;
                _insuranceEdit = value;
            }
        }

        public static bool InsuranceAdd
        {
            get
            {
                return _insuranceAdd;
            }
            private set
            {
                if (_insuranceAdd == value)
                    return;
                _insuranceAdd = value;
            }
        }

        public static bool InsuranceDelete
        {
            get
            {
                return _insuranceDelete;
            }
            private set
            {
                if (_insuranceDelete == value)
                    return;
                _insuranceDelete = value;
            }
        }

        public static bool InsuranceView
        {
            get
            {
                return _insuranceView;
            }
            private set
            {
                if (_insuranceView == value)
                    return;
                _insuranceView = value;
            }
        }

        public static bool InsurancePrint
        {
            get
            {
                return _insurancePrint;
            }
            private set
            {
                if (_insurancePrint == value)
                    return;
                _insurancePrint = value;
            }
        }

        public static bool InsuranceReport
        {
            get
            {
                return _insuranceReport;
            }
            private set
            {
                if (_insuranceReport == value)
                    return;
                _insuranceReport = value;
            }
        }

        private static bool _regInfoDelete = true;
        public static bool RegInfoDelete
        {
            get
            {
                return _regInfoDelete;
            }
            private set
            {
                _regInfoDelete = value;
            }
        }

        #endregion

        public static void ApplyPermissionToRegistrationDetailList(IEnumerable<PatientRegistrationDetail> registrationDetailList)
        {
            if (registrationDetailList != null)
            {
                foreach (var regDetails in registrationDetailList)
                {
                    regDetails.CanDelete = RegInfoDelete & (regDetails.ExamRegStatus == AllLookupValues.ExamRegStatus.DANG_KY_KHAM
                                                        || regDetails.ExamRegStatus == AllLookupValues.ExamRegStatus.KHONG_XAC_DINH);
                }
            }
        }

        public static void ApplyPermissionToPclRequestList(IEnumerable<PatientPCLRequest> pclRequestList)
        {
            foreach (var request in pclRequestList)
            {
                ApplyPermissionToPclRequest(request);
            }
        }

        public static void ApplyPermissionToRegistration(PatientRegistration registration)
        {
            if (registration != null)
            {
                if (registration.PatientRegistrationDetails != null)
                {
                    foreach (var regDetails in registration.PatientRegistrationDetails)
                    {
                        regDetails.CanDelete = RegInfoDelete & (regDetails.ExamRegStatus == AllLookupValues.ExamRegStatus.DANG_KY_KHAM
                                                || regDetails.ExamRegStatus == AllLookupValues.ExamRegStatus.KHONG_XAC_DINH);
                    }
                }

                if (registration.PCLRequests != null)
                {
                    foreach (var request in registration.PCLRequests)
                    {
                        ApplyPermissionToPclRequest(request);
                    }
                }
            }
        }

        public static void ApplyPermissionToPclRequest(PatientPCLRequest request)
        {
            if (request != null)
            {
                //Neu co 1 chi tiet CLS khong the xoa thi request nay cung khong the xoa.
                int num = 0;
                if (request.PatientPCLRequestIndicators != null)
                {
                    foreach (var item in request.PatientPCLRequestIndicators)
                    {
                        item.CanDelete = RegInfoDelete & (item.ExamRegStatus == AllLookupValues.ExamRegStatus.DANG_KY_KHAM 
                                            || item.ExamRegStatus == AllLookupValues.ExamRegStatus.KHONG_XAC_DINH
                                            || item.ExamRegStatus == AllLookupValues.ExamRegStatus.XOA_TRA_TIEN_LAI);
                        if (!item.CanDelete)
                        {
                            num++;
                        }
                    }
                }
                request.CanDelete = (num == 0);
            }
        }

        public static void ApplyPermissionToPclRequestDetail(PatientPCLRequestDetail item)
        {
            if (item != null)
            {
                item.CanDelete = RegInfoDelete & (item.ExamRegStatus == AllLookupValues.ExamRegStatus.DANG_KY_KHAM ||
                                        item.ExamRegStatus == AllLookupValues.ExamRegStatus.KHONG_XAC_DINH);               
            }
        }

        public static void ApplyPermissionToHealthInsuranceList(IEnumerable<HealthInsurance> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    item.CanDelete = InsuranceDelete & item.CanDelete;
                    item.CanEdit = InsuranceEdit & item.CanEdit;
                }
            }
        }
        public static void ApplyPermissionToPaperReferalList(IEnumerable<PaperReferal> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    item.CanDelete = InsuranceDelete & item.CanDelete;
                    item.CanEdit = InsuranceEdit & item.CanEdit;
                }
            }
        }

        public static bool IsEnableRoleUserStatic(long? StaffID)
        {
            //if (StaffID==0 || StaffID == null)
            //{
            //    return true;
            //}
            //else if (Globals.LoggedUserAccount.AccountName == "admin"
            //                       || Globals.LoggedUserAccount.AccountName == "Admin"
            //                       || Globals.LoggedUserAccount.AccountName == "administrator"
            //                       || Globals.LoggedUserAccount.AccountName == "ADMIN"
            //                       || Globals.LoggedUserAccount.AccountName == "ADMINISTRATOR"
            //                       )
            //{
            //    return true;
            //}
            return true;
            //else
            //{
            //    if (Globals.LoggedUserAccount.StaffID == StaffID)
            //    {
            //        return true;
            //    }
            //    return false;
            //}
        }

        public static bool IsAdminUser()
        {
            if (Globals.LoggedUserAccount.AccountName == "admin"
                                     || Globals.LoggedUserAccount.AccountName == "Admin"
                                     || Globals.LoggedUserAccount.AccountName == "administrator"
                                     || Globals.LoggedUserAccount.AccountName == "ADMIN"
                                     || Globals.LoggedUserAccount.AccountName == "ADMINISTRATOR"
                                     )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

       
    }
}
