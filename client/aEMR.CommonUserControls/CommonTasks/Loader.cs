using System;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;

namespace aEMR.CommonTasks
{
    public class Loader : IResult
    {
        private readonly string _message;
        private readonly bool _hide;

        public Loader(string message)
        {
            this._message = message;
        }

        public Loader(bool hide)
        {
            this._hide = hide;
        }
        public void Execute(ActionExecutionContext context)
        {
            if (_hide)
            {
                Globals.IsBusy = false;
            }
            else
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = _message
                });
            }
            Completed(this, new ResultCompletionEventArgs());
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;

        public static IResult Show(string message = null)
        {
            return new Loader(message);
        }
        public static IResult Hide()
        {
            return new Loader(true);
        }
    }

}
