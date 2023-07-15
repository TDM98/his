using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IUCServiceListsSelection
    {
        ConsultationRoomStaffAllocationServiceList ConfirmedServiceList { get; set; }
        bool IsConfirmed { get; set; }
        ServiceItemCollectionLoadCompleted ServiceItemCollectionLoadCompletedCallback { get; set; }
        void ApplySelectedServiceCollection(long ConsultationRoomStaffAllocationServiceListID);
    }
}