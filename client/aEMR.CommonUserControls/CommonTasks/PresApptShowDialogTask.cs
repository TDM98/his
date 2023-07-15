using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using Caliburn.Micro;
using aEMR.ViewContracts;

namespace aEMR.CommonTasks
{
    public class PresApptShowDialogTask : IResult, IHandle<WarningConfirmMsgBoxClose>
    {
        //private Type _screenType;

        private bool _HasAppointment = false;
        public bool HasAppointment
        {
            get { return _HasAppointment; }
            set
            {
                _HasAppointment = value;                
            }
        }

        public static IWindowManager WindowManager;

        public PresApptShowDialogTask()
        {
            Globals.EventAggregator.Subscribe(this);
        }

        public void Execute(ActionExecutionContext context)
        {
            //var ApptCheck = Globals.GetViewModel<IPrescriptionApptCheck>();
            //var ApptCheck = IoC.Get<IPrescriptionApptCheck>();
            //var instance = ApptCheck as Conductor<object>;
            var instance = GlobalsNAV.ShowDialog_V2<IPrescriptionApptCheck>();
            //Globals.ShowDialog(ApptCheck as Conductor<object>);


            ////var screen = IoC.GetInstance(screenType, null);
            //var screen = IoC.Get<IPrescriptionApptCheck>() as Conductor<object>;

            //Dialog = screen;

            //WindowManager.ShowDialog(instance);

            var deActive = instance as IDeactivate;
            if (deActive == null)
            {
                Completed(this, new ResultCompletionEventArgs());
            }
            else 
            {
                //deActive.Deactivated += new EventHandler<DeactivationEventArgs>(deActive_Deactivated);
                deActive.Deactivated += new EventHandler<DeactivationEventArgs>((o, e) => 
                {
                    if (e.WasClosed) 
                    {
                        HasAppointment = ((IPrescriptionApptCheck)o).HasAppointment;
                        Completed(this, new ResultCompletionEventArgs());
                    }
                });
            }
        }

        public void Handle(WarningConfirmMsgBoxClose message)
        {
            Globals.EventAggregator.Unsubscribe(this);
            HasAppointment = message.IsConfirmed;
            Completed(this, new ResultCompletionEventArgs());
        }

        void deActive_Deactivated(object sender, DeactivationEventArgs e)
        {
            if (e.WasClosed)
            {
                HasAppointment = ((IPrescriptionApptCheck)sender).HasAppointment;
                Completed(this,new ResultCompletionEventArgs());
            }
        }

        public object Dialog { get; private set; }
        //public event EventHandler Completed = delegate { };
        public event EventHandler<ResultCompletionEventArgs> Completed;

        //public static PresApptShowDialogTask Of()
        //{
        //    return new PresApptShowDialogTask(typeof(T));
        //}
    }
}
