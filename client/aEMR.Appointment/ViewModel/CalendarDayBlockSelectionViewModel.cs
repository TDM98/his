using aEMR.Common.BaseModel;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;

namespace aEMR.Appointment.ViewModels
{
    [Export(typeof(ICalendarDayBlockSelection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CalendarDayBlockSelectionViewModel : ViewModelBase, ICalendarDayBlockSelection
    {
        #region Properties
        public int BlockQty { get; set; } = 0;
        #endregion
        #region #Events
        [ImportingConstructor]
        public CalendarDayBlockSelectionViewModel() { }
        public void btnOneBlock()
        {
            BlockQty = 1;
            TryClose();
        }
        public void btnTwoBlock()
        {
            BlockQty = 2;
            TryClose();
        }
        public void btnThreeBlock()
        {
            BlockQty = 3;
            TryClose();
        }
        #endregion
    }
}