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
using DataEntities;

namespace aEMR.ViewContracts.Configuration
{
    public interface IDeptLocation_ByDeptID
    {
        object leftContent { get; set; }
        string TitleForm { get; set; }
        object ObjKhoa_Current { get; set; }
        void RoomType_GetAll();
        void DeptLocation_GetRoomTypeByDeptID(Int64 DeptID);
        
        object Locations_SelectForAdd { get; set; }
        object RoomType_SelectFind { get; set; }
        string LocationName { get; set; }
        void DeptLocation_ByDeptID(Int64 DeptID,Int64 RmTypeID,string LocationName);
        void DeptLocation_XMLInsert(Int64 DeptID, ObservableCollection<Location> objCollect);
        void DeptLocation_MarkDeleted(Int64 DeptLocationID);
    }
}
