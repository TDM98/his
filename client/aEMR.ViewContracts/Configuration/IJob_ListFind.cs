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
    public interface IJob_ListFind
    {
        //void Job_GetAll();
        void Job_MarkDeleted(Int64 LID);

        Visibility hplAddNewVisible { get; set; }
        void DoubleClick(object args);
        object Job_Current { get; set; }
    }
}
