using System;
using System.Collections.ObjectModel;
using DataEntities;
/*
* 20230424 #001 DatTB:
* + Gộp view/model thêm mới và chỉnh sửa lại
* + Thay đổi loại thiết bị theo bảng loại TT/PT.
*/

namespace aEMR.ViewContracts
{
    public interface IResourcesEdit
    {
        bool isEdit { get; set; }
        Resources curResource { get; set; }
        long ResourceCategoryEnum { get; set; }
        ObservableCollection<Lookup> refResourceUnit { get; set; }
        ObservableCollection<ResourceGroup> refResourceGroup { get; set; }
        //▼==== #001
        //ObservableCollection<ResourceType> refResourceType { get; set; }
        ObservableCollection<RefMedicalServiceType> refResourceType { get; set; }
        //▲==== #001
        ObservableCollection<Supplier> refSuplier { get; set; }
    }
}
