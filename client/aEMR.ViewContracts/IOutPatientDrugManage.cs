/*
 * 201470803 #001 CMN: Add HI Store Service
*/
//using System.Collections.ObjectModel;
//using System.Windows.Data;
//using DataEntities;
using aEMR.Common.PagedCollectionView;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Quản lý danh sách Thuốc của bệnh nhân ngoại trú (thêm, xóa)
    /// </summary>
    public interface IOutPatientDrugManage
    {
        PagedCollectionView DrugItems { get; set; }
        bool HiServiceBeingUsed { get; set; }
        //==== #001
        bool IsHIOutPt { get; set; }
        //==== #001
    }
}
