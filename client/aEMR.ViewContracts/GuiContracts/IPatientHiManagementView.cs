using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPatientHiManagementView
    {
        /// <summary>
        /// Hoàn thành việc thêm dữ liệu trên lưới thẻ BH. 
        /// Nếu dữ liệu hợp lệ thì commit edit
        /// Nếu không hợp lệ thì cancel edit.
        /// </summary>
        void CommitEditingHiGridIfValid();
       
        void BeginEditingOnGrid();
    }
}
