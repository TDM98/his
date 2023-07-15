/*
 * 20170410 #001 CMN: Add Popup follow the result of patient drug using
*/
namespace aEMR.ViewContracts
{
    public interface IClinicDeptOutPtReqForm
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

}
