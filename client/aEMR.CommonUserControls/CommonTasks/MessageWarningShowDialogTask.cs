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
    public class MessageWarningShowDialogTask : IResult, IHandle<WarningConfirmMsgBoxClose>
    {
        //private Type _screenType;

        private bool _IsAccept = false;
        public bool IsAccept
        {
            get { return _IsAccept; }
            set
            {
                _IsAccept = value;                
            }
        }

        private string _message = "";
        public string message
        {
            get { return _message; }
            set
            {
                _message = value;
            }
        }

        private bool _isCheckBox = true;

        private string _checkBoxContent = "";
        public string checkBoxContent
        {
            get { return _checkBoxContent; }
            set
            {
                _checkBoxContent = value;
            }
        }

        public static IWindowManager WindowManager;

        public MessageWarningShowDialogTask(AxErrorEventArgs error, string _checkBoxContent)
        {
            message = error.ServerError.Message;
            checkBoxContent = _checkBoxContent;
            Globals.EventAggregator.Subscribe(this);
        }

        public MessageWarningShowDialogTask(string _message, string _checkBoxContent)
        {
            message = _message;
            checkBoxContent = _checkBoxContent;
            Globals.EventAggregator.Subscribe(this);
        }
        public MessageWarningShowDialogTask(string _message, string _checkBoxContent,bool isCheckBox)
        {
            message = _message;
            checkBoxContent = _checkBoxContent;
            _isCheckBox = isCheckBox;
            Globals.EventAggregator.Subscribe(this);
        }

        public void Execute(ActionExecutionContext context)
        {
            //var ApptCheck = Globals.GetViewModel<IErrorBold>();
            //var ApptCheck = IoC.Get<IErrorBold>();
            Action<IErrorBold> onInitDlg = delegate (IErrorBold ApptCheck)
            {
                ApptCheck.isCheckBox = _isCheckBox;
                ApptCheck.SetMessage(message, checkBoxContent);
                ApptCheck.FireOncloseEvent = true;
            };
            //var instance = ApptCheck as Conductor<object>;
            var instance = GlobalsNAV.ShowDialog_V2(false, null, onInitDlg);

            // TxD 29/06/2018: Commented OUT the following because IDeactivate DOESNOT WORK for WPF somehow
            //                 Instead we added an EventHandler for WarningConfirmMsgBoxClose which is fired by the Dialog itself upon closing
            //var deActive = instance as IDeactivate;
            //if (deActive == null)
            //{
            //    Completed(this, new ResultCompletionEventArgs());
            //}
            //else
            //{
            //    //deActive.Deactivated += new EventHandler<DeactivationEventArgs>(deActive_Deactivated);
            //    deActive.Deactivated += new EventHandler<DeactivationEventArgs>((o, e) =>
            //    {
            //        if (e.WasClosed)
            //        {
            //            IsAccept = ((IErrorBold)o).IsAccept;
            //            Completed(this, new ResultCompletionEventArgs());
            //        }
            //    });
            //}

        }

        void deActive_Deactivated(object sender, DeactivationEventArgs e)
        {
            if (e.WasClosed)
            {
                IsAccept = ((IErrorBold)sender).IsAccept;
                Completed(this,new ResultCompletionEventArgs());
            }
        }

        public object Dialog { get; private set; }
        public event EventHandler<ResultCompletionEventArgs> Completed;

        public void Handle(WarningConfirmMsgBoxClose msg)
        {
            if (msg != null)
            {
                IsAccept = msg.IsConfirmed;
                if (Completed != null)
                    Completed(this, new ResultCompletionEventArgs());
            }
        }

    }
}
