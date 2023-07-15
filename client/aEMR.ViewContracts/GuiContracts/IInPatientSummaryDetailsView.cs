using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IInPatientSummaryDetailsView
    {
        /// <summary>
        /// Thu nho, mo rong form.
        /// </summary>
        /// <param name="isExpanded"></param>
        void Switch(bool isExpanded);
        
    }
}
