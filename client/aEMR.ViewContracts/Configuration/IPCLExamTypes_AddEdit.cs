using System.Collections.ObjectModel;
using DataEntities;
/*
 * 01062018 #001 TTM
 */
namespace aEMR.ViewContracts.Configuration
{
    public interface IPCLExamTypes_AddEdit
    {
        ObservableCollection<Lookup> ObjV_PCLMainCategory { get;set;}
        //ObservableCollection<PCLExamTypeSubCategory> ObjPCLExamTypeSubCategory_ByV_PCLMainCategory { get; set; }
        //ObservableCollection<PCLExamTypeSubCategory> ObjPCLExamTypeSubCategory_ByV_PCLMainCategory { get; set; }
        //▼=====#001
        //Thêm tmphiRepEquip để lấy giá trị từ PCLExamType sang PCLExamTypes_AddEdit
        ObservableCollection<Resources> tmphiRepEquip { get; set; }
        //▲=====#001
        void FormLoad();
        string TitleForm { get; set; }
        PCLExamType ObjPCLExamType_Current { get; set; }
        void InitializeNewItem(Lookup pObjV_PCLMainCategory, PCLExamTypeSubCategory pObjPCLExamTypeSubCategory);
     
    }
}