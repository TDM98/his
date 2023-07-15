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
    public interface IICD_AddEdit
    {
        string TitleForm { get; set; }
        //ObservableCollection<DataEntities.ICD> ObjICD_GetAll{ get; set; }
        DataEntities.ICD ObjICD_Current { get; set; }
        DataEntities.DiseaseChapters ObjDiseaseChapters_Current { get; set; }
        DataEntities.Diseases ObjDiseases_Current { get; set; }
        long TypeForm { get; set; }
        void InitializeNewItem();
    }
}
