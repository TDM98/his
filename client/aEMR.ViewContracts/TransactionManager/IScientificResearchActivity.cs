using DataEntities;
using System;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IScientificResearchActivity
    {
        //bool IsChiDaoTuyen { get; set; }
        //bool IsNghienCuuKhoaHoc { get; set; }
        string TitleForm { get; set; }
        ScientificResearchActivities ScientificResearchActivity_Current { get; set; }
        //ObservableCollection<RefGeneralUnits> RefGeneralUnits { get; set; }
        //ObservableCollection<RefGeneralUnits> RefGeneralUnits { get; set; }
        //void InsertUpdateActivity(bool ISCDTuyen, bool ISAdd, Activities objActivity);
    }
}
