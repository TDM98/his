using System.Windows;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IListPCLRequestExt
    {
        PatientPCLRequestExtSearchCriteria SearchCriteria { get; set; }
        void Init();
        //Thực Hiện, Ngưng, Kết Quả
        Visibility dtgCellTemplateThucHien_Visible { get; set; }
        Visibility dtgCellTemplateNgung_Visible { get; set; }
        Visibility dtgCellTemplateKetQua_Visible { get; set; }
        Visibility dtgCellTemplateInputKetQua_Visible { get; set; }
        //Thực Hiện, Ngưng, Kết Quả
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}