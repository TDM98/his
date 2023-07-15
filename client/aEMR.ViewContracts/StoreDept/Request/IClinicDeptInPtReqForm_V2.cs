/*
 * 20170410 #001 CMN: Add Popup follow the result of patient drug using
*/
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IClinicDeptInPtReqForm_V2
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }

        bool mPhieuYeuCau_Tim { get; set; }
        bool mPhieuYeuCau_Them { get; set; }
        bool mPhieuYeuCau_Xoa { get; set; }
        bool mPhieuYeuCau_XemIn { get; set; }
        bool mPhieuYeuCau_In { get; set; }

        bool UsedForRequestingDrug { get; set; }
        bool DoseVisibility { get; set; }

        //==== #001 ====
        bool IsResultView { get; set; }
        //==== #001 ====

        bool IsSearchByGenericName { get; set; }
        bool vIsSearchByGenericName { get; set; }
    }
    public interface IClinicDeptInPtReqForm_V3
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }

        bool mPhieuYeuCau_Tim { get; set; }
        bool mPhieuYeuCau_Them { get; set; }
        bool mPhieuYeuCau_Xoa { get; set; }
        bool mPhieuYeuCau_XemIn { get; set; }
        bool mPhieuYeuCau_In { get; set; }

        bool UsedForRequestingDrug { get; set; }
        bool DoseVisibility { get; set; }

        //==== #001 ====
        bool IsResultView { get; set; }
        //==== #001 ====

        bool IsSearchByGenericName { get; set; }
        bool vIsSearchByGenericName { get; set; }
    }
    public interface IPharmacyHIStoreReqForm
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }
        bool mPhieuYeuCau_Tim { get; set; }
        bool mPhieuYeuCau_Them { get; set; }
        bool mPhieuYeuCau_Xoa { get; set; }
        bool mPhieuYeuCau_XemIn { get; set; }
        bool mPhieuYeuCau_In { get; set; }
        bool UsedForRequestingDrug { get; set; }
        bool DoseVisibility { get; set; }
        bool IsResultView { get; set; }
        bool IsSearchByGenericName { get; set; }
        bool vIsSearchByGenericName { get; set; }
    }

    public interface IClinicDeptInPtReqForm_ForSmallProcedure
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }

        bool mPhieuYeuCau_Tim { get; set; }
        bool mPhieuYeuCau_Them { get; set; }
        bool mPhieuYeuCau_Xoa { get; set; }
        bool mPhieuYeuCau_XemIn { get; set; }
        bool mPhieuYeuCau_In { get; set; }

        bool UsedForRequestingDrug { get; set; }
        bool DoseVisibility { get; set; }

        bool IsResultView { get; set; }

        bool IsSearchByGenericName { get; set; }
        bool vIsSearchByGenericName { get; set; }

        long PtRegDetailID { get; set; }

        ReqOutwardDrugClinicDeptPatient CurrentReqOutwardDrugClinicDeptPatient { get; set; }

        void GetRequestDrugForTechnicalServicePtRegDetailID(long PtRegDetailID);

        bool FormEditorIsEnabled { get; set; }
    }
}
