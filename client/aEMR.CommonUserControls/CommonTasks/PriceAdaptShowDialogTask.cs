using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.GlobalFuncs;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;
using aEMR.ViewContracts.Configuration;
using aEMR.ViewContracts;

namespace aEMR.CommonTasks
{
    public class PriceAdaptShowDialogTask : IResult
    {
        //private Type _screenType;

        string ServiceName { get; set; }
        public string Comments { get; set; }
        decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public bool IsOk { get; set; }

        public static IWindowManager WindowManager;

        public PriceAdaptShowDialogTask(string serviceName, decimal oldPrice)
        {
            ServiceName = serviceName;
            OldPrice = oldPrice;
        }
        
        public void Execute(ActionExecutionContext context)
        {
            //var PriceAdapt = IoC.Get<IPriceAdapt>();
            Action<IPriceAdapt> onInitDlg = delegate (IPriceAdapt PriceAdapt)
            {
                PriceAdapt.OldPrice = OldPrice;
                PriceAdapt.ServiceName = ServiceName;
                PriceAdapt.SetValue(ServiceName, OldPrice);
            };
            //var instance = PriceAdapt as Conductor<object>;

            var instance = GlobalsNAV.ShowDialog_V2(false, null, onInitDlg);
            
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
                        IsOk = ((IPriceAdapt)o).IsOk;
                        NewPrice = ((IPriceAdapt)o).NewPrice;
                        Comments = ((IPriceAdapt)o).Comments;
                        Completed(this, new ResultCompletionEventArgs());
                    }
                });
            }
                          
        }

        void deActive_Deactivated(object sender, DeactivationEventArgs e)
        {
            if (e.WasClosed)
            {
                IsOk = ((IPriceAdapt)sender).IsOk;
                NewPrice = ((IPriceAdapt)sender).NewPrice;
                Comments = ((IPriceAdapt)sender).Comments;
                Completed(this,new ResultCompletionEventArgs());
            }
        }

        public object Dialog { get; private set; }
        public event EventHandler<ResultCompletionEventArgs> Completed;

    }
}
