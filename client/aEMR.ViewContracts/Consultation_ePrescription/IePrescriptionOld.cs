namespace aEMR.ViewContracts
{
    public interface IePrescriptionOld
    {
        bool DuocSi_IsEditingToaThuoc { get; set; }
        void Init();

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
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}