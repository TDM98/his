namespace aEMR.Infrastructure.Events
{
    public class ResourceEvent
    {
        public object Resource { get; set; }
    }
    public class ResourceSelectedEvent
    {
        public object curResource { get; set; }
    }
    public class ResourcePropLocationsEvent
    {
        public object selResourcePropLocations { get; set; }
    }
    public class lstResourcePropLocationsEvent
    {
        public object lstResPropLocations { get; set; }
    }

    public class ResourceCategoryEnumEvent
    {
        public object ResourceCategoryEnum { get; set; }
    }

    public class lstResourcePropLocationsGridToFormEvent
    {
        public object lstResPropLocations { get; set; }
    }
    
    public class PropDeptEvent
    {
        public PropDeptEvent(object _curResource,object _curDeptLoc)
        {
            curResource = _curResource;
            curDeptLoc=_curDeptLoc;
        }

        public object curResource { get; set; }
        public object curDeptLoc { get; set; }
    }
    public class ResourceEditEvent
    {
        
    }
    public class ResourceNewGroupEvent
    {
        public bool isNewGroup { get; set; }
    }
    public class TranferEvent
    {
        
    }
}
