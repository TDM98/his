using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace aEMR.WPFControls
{
    public class Appointments : ObservableCollection<Appointment>
    {
        public Appointments()
        {
        }
    }
    public static class Filters
    {
        public static IEnumerable<Appointment> ByDate(this IEnumerable<Appointment> appointments, DateTime date)
        {
            var app = from a in appointments
                      where a.StartTime.Date == date.Date
                      select a;
            return app;
        }
    }
}
