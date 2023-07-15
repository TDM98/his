using System.Collections.Generic;
using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IeInPrescriptionOldNew
    {
        //bool DuocSi_IsEditingToaThuoc { get; set; }
        //void Init();

        ////Dược Sĩ thì không thấy mấy nút này
        bool btChonChanDoanVisibility { get; set; }
        ////Dược Sĩ thì không thấy mấy nút này

        //Nút Này chỉ dành cho Dược Sĩ
        bool btDuocSiEditVisibility { get; set; }
        //Nút Này chỉ dành cho Dược Sĩ


        bool hasTitle { get; set; }

        bool mToaThuocDaPhatHanh_ThongTin { get; set; }
        bool mToaThuocDaPhatHanh_ChinhSua { get; set; }
        bool mToaThuocDaPhatHanh_TaoToaMoi { get; set; }
        bool mToaThuocDaPhatHanh_PhatHanhLai { get; set; }
        bool mToaThuocDaPhatHanh_In { get; set; }
        bool mToaThuocDaPhatHanh_ChonChanDoan { get; set; }

        bool NumberOfTimesPrintVisibility { get; set; }

        //==== 20161115 CMN Begin: Fix Drug Week Calendar
        void HandleDrugSchedule(ObservableCollection<PrescriptionDetailSchedules> drugSchedule, double numOfDay, string note);
        //==== 20161115 CMN End.
        void GetInitDataInfo();
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        void ApplyDiagnosisTreatmentCollection(IList<DiagnosisTreatment> aDiagnosisTreatmentCollection);
        bool IsUpdateDiagConfirmInPT { get; set; }
        void SetLastDiagnosisForConfirm();
    }
}