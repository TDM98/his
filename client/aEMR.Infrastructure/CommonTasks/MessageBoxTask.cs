using System;
using aEMR.Common.Enums;
using aEMR.Infrastructure.Events;
using Caliburn.Micro;

namespace aEMR.Infrastructure.CommonTasks
{
    /// <summary>
    /// Sử dụng lớp này trong Coroutine LƯU Ý:
    /// Mở popup trong 1 viewmodel khác, popup này publish 1 event "ItemSelected<IMessageBox, AxMessageBoxResult>"
    /// Phải bảo đảm hàm Handle(ItemSelected<IMessageBox, AxMessageBoxResult>) được gọi thì mới complete được.
    /// </summary>
    public class MessageBoxTask : IResult, IHandle<ItemSelected<IMessageBox, AxMessageBoxResult>>
    {
        string _text;
        string _caption;
        MessageBoxOptions _options;
        public AxMessageBoxResult Result
        {
            get;
            private set;
        }

        public MessageBoxTask(string text, string caption,
        MessageBoxOptions opts = MessageBoxOptions.Ok)
        {
            _text = text;
            _caption = caption;
            _options = opts;
            Globals.EventAggregator.Subscribe(this);
        }
        ~MessageBoxTask()
        {
            
        }
        public void Execute(ActionExecutionContext context)
        {
            //Result = MessageBox.Show(_text, _caption, _button);
            //var msgVm = Globals.GetViewModel<IMessageBox>();
            //msgVm.Caption = _caption;
            //msgVm.MessageBoxText = _text;
            //msgVm.Options = _options;
            Action<MsgBoxOptions, IScreen> onCloseDlg = delegate (MsgBoxOptions opt, IScreen dlg)
            {
                (dlg as IMessageBox).CloseUsingXButton();
            };
            Action<IMessageBox> onInitDlg = delegate (IMessageBox msgVm)
            {
                msgVm.Caption = _caption;
                msgVm.MessageBoxText = _text;
                msgVm.Options = _options;
            };

            GlobalsNAV.ShowDialog<IMessageBox>(false, onCloseDlg, onInitDlg);
            //Globals.ShowDialogV2(msgVm as Conductor<object>, () => msgVm.CloseUsingXButton());
            //Show dialog xong roi cho bat duoc event select moi Complete
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;

        public void Handle(ItemSelected<IMessageBox, AxMessageBoxResult> message)
        {
            if (Completed != null)
            {
                Result = message.Sender.Result;
                Completed(this, new ResultCompletionEventArgs
                   {
                       Error = null,
                       WasCancelled = false
                   }); 
            }
        }
    }

}
