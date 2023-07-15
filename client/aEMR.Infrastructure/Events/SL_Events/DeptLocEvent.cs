namespace aEMR.Infrastructure.Events
{
    public class DeptLocEvent
    {
        public object Resource { get; set; }
    }
    public class DeptLocSelectedEvent
    {
        public object curDeptLoc { get; set; }
    }
    public class DeptLocSelectedTranferEvent
    {
        public object curDeptLoc { get; set; }
    }
    public class DeptLocBedSelectedEvent
    {
        public object curDeptLoc { get; set; }
    }
    public class RoomSelectedEvent
    {
        public object curDeptLoc { get; set; }
    }
    public class DepartmentSelectedEvent
    {
        public object curDepartment { get; set; }
    }

    public class AddNewRoomTargetEvent
    {
        
    }

    public class LoadPatientListBySeletedDeptID
    {
        public object DepartmentTree { get; set; }
    }
}
