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
    public interface IICD_ListFind
    {
        //void ICD_GetAll();
        void ICD_MarkDeleted(Int64 LID);

        Visibility hplAddNewVisible { get; set; }
        object ICD_Current { get; set; }
    }
}
