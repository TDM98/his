using Caliburn.Micro;
using System;

namespace aEMR.Infrastructure.Events
{
    public class ShowDialogEvent 
    {
        public object DialogViewModel { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public Action<object> Callback { get; set; }

    }

    public class LogOutEvent
    {
    }

   
}
