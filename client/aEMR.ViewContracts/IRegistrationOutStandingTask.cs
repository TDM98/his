using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Input;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Outstanding task cua trang Dang ky.
    /// Bao gom 2 outstanding task: HEN BENH va DANH SACH CHO DANG KY
    /// </summary>
    public interface IRegistrationOutStandingTask
    {
        //IRegAppointmentOutStandingTask AppointmentOutstandingTaskContent { get; set; }
        object AppointmentOutstandingTaskContent { get; set; }
        IRegOutStandingTask RegOutstandingTaskContent { get; set; }
    }
}
