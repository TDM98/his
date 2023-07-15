﻿using System;
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
using aEMR.Common;

namespace aEMR.CommonTasks
{
    public class WarningWithConfirmMsgBoxTask : IResult, IHandle<WarningConfirmMsgBoxClose> 
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

        //~WarningWithConfirmMsgBoxTask()
        //{
        //    int x = 10;
        //    ClientLoggerHelper.LogInfo("WarningWithConfirmMsgBoxTask DESTRUCTOR " + x.ToString());
        //}


        public static IWindowManager WindowManager;

        public WarningWithConfirmMsgBoxTask(AxErrorEventArgs error, string _checkBoxContent)
        {
            message = error.ServerError.Message;
            checkBoxContent = _checkBoxContent;
            Globals.EventAggregator.Subscribe(this);
        }

        public WarningWithConfirmMsgBoxTask(string _message, string _checkBoxContent)
        {
            message = _message;
            checkBoxContent = _checkBoxContent;
            Globals.EventAggregator.Subscribe(this);
        }
        public WarningWithConfirmMsgBoxTask(string _message, string _checkBoxContent, bool isCheckBox)
        {
            message = _message;
            checkBoxContent = _checkBoxContent;
            _isCheckBox = isCheckBox;
            Globals.EventAggregator.Subscribe(this);
        }

        
        public void Execute(ActionExecutionContext context)
        {
            Action<IErrorBold> onInitDlg = delegate (IErrorBold confDlg)
            {                
                confDlg.isCheckBox = _isCheckBox;
                confDlg.SetMessage(message, checkBoxContent);
                confDlg.FireOncloseEvent = true;
            };
            //var instance = confDlg as Conductor<object>;
            GlobalsNAV.ShowDialog<IErrorBold>(onInitDlg);
        }

        public void Handle(WarningConfirmMsgBoxClose message)
        {
            //ClientLoggerHelper.LogInfo("WarningWithConfirmMsgBoxTask Handle Event [WarningConfirmMsgBoxClose].");
            Globals.EventAggregator.Unsubscribe(this);
            IsAccept = message.IsConfirmed;
            if (Completed != null)
                Completed(this, new ResultCompletionEventArgs());
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

    }
}
