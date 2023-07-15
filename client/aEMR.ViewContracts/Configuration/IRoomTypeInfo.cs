using System;
using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts.Configuration
{
    public interface IRoomTypeInfo
    {
        string TitleForm { get; set; }
        ObservableCollection<Lookup> ObjV_RoomFunction { get; set; }
        DataEntities.RoomType ObjRoomType_Current { get; set; }
        void InitializeNewItem(Int64 V_RoomFunction_Selected);
    }
}
