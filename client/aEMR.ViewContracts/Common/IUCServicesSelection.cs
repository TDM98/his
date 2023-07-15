using DataEntities;
using System.Collections.Generic;

namespace aEMR.ViewContracts
{
    public delegate void ServiceItemCollectionLoadCompleted();
    public interface IUCServicesSelection
    {
        IList<RefMedicalServiceItem> SelectedServiceItemCollection { get; set; }
        ServiceItemCollectionLoadCompleted ServiceItemCollectionLoadCompletedCallback { get; set; }
        void ApplySelectedServiceCollection(long[] MedServiceIDCollection);
    }
}