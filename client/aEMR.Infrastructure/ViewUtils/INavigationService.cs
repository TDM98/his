using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using aEMR.Common.Enums;

namespace aEMR.Infrastructure.ViewUtils
{
    public interface INavigationService
    {
        IConductor ParentScreen { get; }

        IScreen DefaultScreen { get; set; }

        IScreen CurrentScreen { get; }

        void NavigationTo(IScreen screen);
        void NavigationTo<T>() where  T : class;
        
        void NavigationTo(Type screen);
        void NavigationTo(IScreen screen, Action<IScreen> initAction );
        void NavigationTo<T>(Action<T> initAction) where T : class;
        void NavigationToWithBeforeAction<T>(Action<T> beforeAction) where T : class;
        void NavigationTo(Type screen, Action<IScreen> initAction);

        
        void ShowNotification(string message);
        void ShowMessage(string message);

        void ShowPopup<T>(Action<T> onInitialize = null, Action<MsgBoxOptions, IScreen> onClose = null);

        void ShowPopup(string message, Action<MsgBoxOptions, IScreen> onClose = null);

        void ShowDialog<T>(Action<T> onInitialize = null, Action<MsgBoxOptions, IScreen> onClose = null
            , MsgBoxOptions msgBoxOptions = MsgBoxOptions.None
            , Dictionary<string, object> settings = null 
            );

        void ShowDialog(string message, Action<MsgBoxOptions, IScreen> onClose);
        void ShowDialog(string message, Action<MsgBoxOptions, IScreen> onClose, Dictionary<string, object> settings);
        void ShowDialog(string message, Action<MsgBoxOptions, IScreen> onClose, MsgBoxOptions msgBoxOptions, Dictionary<string, object> settings);

        void CloseCurrentWindow();
        void CloseCurrentWindow(MsgBoxOptions options);

        void ShowBusy(string busyContent);
        void ShowBusy();
        void HideBusy();

        void ShowPopup(IScreen screen);

        void ShowWindow(IScreen screen);

        void GoBack();

    }
}
