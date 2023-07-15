using System.Windows;
using DataEntities;

namespace aEMR.Infrastructure
{
    public interface IMessageBox
    {
        string Caption { get; set; }

        string MessageBoxText { get; set; }

        MessageBoxOptions Options { get; set; }
        AxMessageBoxResult Result { get; }

        void CloseUsingXButton();
    }
}
