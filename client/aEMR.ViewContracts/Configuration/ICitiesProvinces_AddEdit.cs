using System;
using System.Collections.ObjectModel;
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
    public interface ICitiesProvinces_AddEdit
    {
        string TitleForm { get; set; }
        long FormType { get; set; }
        //ObservableCollection<DataEntities.ICD> ObjICD_GetAll{ get; set; }
        DataEntities.CitiesProvince ObjCitiesProvinces_Current { get; set; }
        DataEntities.SuburbNames ObjSuburbNames_Current { get; set; }
        DataEntities.WardNames ObjWardNames_Current { get; set; }
        ObservableCollection<DataEntities.CitiesProvince> Provinces { get; set; }
        ObservableCollection<DataEntities.SuburbNames> SuburbNames { get; set; }
        void InitializeNewItem();
    }
}
