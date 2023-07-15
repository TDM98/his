using System.ComponentModel;
using DataEntities;
using System;
namespace aEMR.ViewContracts
{
    public interface IEditApptPclRequestDetailList
    {
        PatientApptPCLRequests PCLRequest { get; set; }
        //ObservableCollection<PatientApptPCLRequestDetails> MaskedDetailList { get; }
        //void AddItemToView(PatientApptPCLRequestDetails requestDetail);
        //void RemoveItemFromView(PatientApptPCLRequestDetails requestDetail);
        //void ResetCollection();
        void RefreshView();
        ICollectionView FilteredCollection { get; }
        event EventHandler detailListChanged;
        bool IsEdit { get; set; }
    }
}
