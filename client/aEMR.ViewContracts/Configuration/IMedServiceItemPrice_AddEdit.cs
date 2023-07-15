using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace aEMR.ViewContracts.Configuration
{
    public interface IMedServiceItemPrice_AddEdit
    {
        object IDeptMedServiceItems_Current { get; set; }
        string TitleForm { get; set; }
        void LoadForm();
        object IMedServiceItemPrice_Save { get; set; }
    }
}
