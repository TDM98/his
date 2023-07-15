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
    public interface ICitiesProvinces_ListFind
    {
        //void ICD_GetAll();
        void CitiesProvinces_MarkDeleted(Int64 CityProvinceID);
        void SuburbNames_MarkDeleted(Int64 SuburbNameID);
        void WardNames_MarkDeleted(Int64 WardNameID);

        Visibility hplAddNewVisible { get; set; }
        void DoubleClick(object args);
        object CitiesProvinces_Current { get; set; }
        object SuburbNames_Current { get; set; }
        object WardNames_Current { get; set; }
    }
}
