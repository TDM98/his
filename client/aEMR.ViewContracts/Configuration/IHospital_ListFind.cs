using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace aEMR.ViewContracts.Configuration
{
    public interface IHospital_ListFind
    {
        void Hospital_MarkDeleted(Int64 HosID);
        Visibility hplAddNewVisible { get; set; }
        void DoubleClick(object args);
        object Hospital_Current { get; set; }
    }
}
