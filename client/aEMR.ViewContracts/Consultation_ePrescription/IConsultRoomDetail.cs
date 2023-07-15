using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IConsultRoomDetail
    {
        V_DeptTypeOperation DeptTypeOperation { get; set; }
        void LoadRefDept(V_DeptTypeOperation DeptTypeOperation);
    }
}
