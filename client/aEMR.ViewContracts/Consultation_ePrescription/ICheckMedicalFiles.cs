
namespace aEMR.ViewContracts
{
    public interface ICheckMedicalFiles
    {
        bool mCheckMedicalFile_Tim { get; set; }
        bool mCheckMedicalFile_DanhMucICD { get; set; }
        bool mCheckMedicalFile_Luu  { get; set; }
        bool mCheckMedicalFile_TraHS  { get; set; }
        bool mCheckMedicalFile_Duyet { get; set; }
        bool mCheckMedicalFile_MoKhoa { get; set; }
        bool mCheckMedicalFile_DLS_Save { get; set; }
        bool mCheckMedicalFile_DLS_Duyet { get; set; }
        bool mCheckMedicalFile_DLS_TraHS { get; set; }
    }
}