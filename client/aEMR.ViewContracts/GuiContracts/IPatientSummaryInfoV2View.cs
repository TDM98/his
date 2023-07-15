using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPatientSummaryInfoV2View
    {
        /// <summary>
        /// Thu nho, mo rong form.
        /// </summary>
        /// <param name="isExpanded"></param>
        void Switch(bool isExpanded);
        
    }

    public interface IPatientSummaryInfoV3View
    {
        /// <summary>
        /// Thu nho, mo rong form.
        /// </summary>
        /// <param name="isExpanded"></param>
        void Switch(bool isExpanded);

    }
}
