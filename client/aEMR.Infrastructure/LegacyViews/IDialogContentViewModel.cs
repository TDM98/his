using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Caliburn.Micro;
using aEMR.Common.Enums;

namespace aEMR.ViewContracts
{
    public interface IDialogContentViewModel
    {

        string BusyContent { get; set; }
        bool IsBusy { get; set; }

        Action<MsgBoxOptions, IScreen> CallbackResult { get; set; }
        TViewModel DisplayContent_V2<TViewModel>(
            Action<TViewModel> initAction
            , Action<TViewModel> onClose
            , MsgBoxOptions msgBoxOptions = MsgBoxOptions.Ok
            , Action<MsgBoxOptions, IScreen> callbackResult = null);

        void DisplayContent<TViewModel>(
            Action<TViewModel> initAction
            , Action<TViewModel> onClose
            , MsgBoxOptions msgBoxOptions = MsgBoxOptions.Ok
            , Action<MsgBoxOptions, IScreen> callbackResult = null);
        
        void DisplayContent<TViewModel>(IScreen viewModel, Action<TViewModel> initAction = null
            , MsgBoxOptions msgBoxOptions = MsgBoxOptions.Ok
            , Action<MsgBoxOptions, IScreen> callbackResult = null
            
            ) where TViewModel : IScreen;


        void DisplayContent<TViewModel>(Type viewModel, Action<TViewModel> initAction = null
            , MsgBoxOptions msgBoxOptions = MsgBoxOptions.Ok
            , Action<MsgBoxOptions, IScreen> callbackResult = null
                
            ) where TViewModel : IScreen;
        
        string Message { get; set; }
        MsgBoxOptions Option { get; set; }

        void Select(MsgBoxOptions option);

        void OkCommand();
        void CancelCommand();
        void YesCommand();
        void NoCommand();
        void NoneCommand();

    }
}
