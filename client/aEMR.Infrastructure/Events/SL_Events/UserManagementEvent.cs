using System;
using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.Infrastructure.Events
{
    public class ModuleChangeEvent
    {
        public ModulesTree curModuleTree { get; set; }
    }
    public class FunctionChangeEvent
    {
        public Function curFunction { get; set; }
    }
    public class OperationChangeEvent
    {
        public Operation curOperation { get; set; }
    }
    public class PermissionChangeEvent
    {
        public Permission curPermission { get; set; }
    }
    public class ModuleTreeChangeEvent
    {
        public ModulesTree curModulesTree { get; set; }
    }
    public class ModuleTreeOperationChangeEvent
    {
        public ModulesTree curModulesTree { get; set; }
    }
    public class ModuleTreeExChangeEvent
    {
        public ModulesTree curModulesTree { get; set; }
    }
    public class allFunctionChangeEvent
    {
        
    }
    public class allOperationChangeEvent
    {
        
    }
    public class ModuleTreeChangeExEvent
    {
        public ModulesTree curModulesTree { get; set; }
    }
    public class DeleteModuleEvent
    {
        
    }
    public class ModuleTreeLoadBegin
    {
        
    }
    public class ModuleTreeLoadComplete
    {

    }
    public class UpdateModuleEnumEvent
    {

    }

    public class SelectGroupChangeEvent
    {
        public Group SelectedGroup { get; set; }
    }
    public class allRoleChangeEvent
    {
        
    }
    public class allGroupChangeEvent
    {

    }
    public class SelectRoleChangeEvent
    {
        public Role SelectedRole { get; set; }
    }
    public class allStaffChangeEvent
    {

    }
    public class ModuleLoadCompleteEvent
    {
        public ObservableCollection<ModulesTree> allModulesTree { get; set; }
    }
    public class DeleteUserGroupCompletedEvent
    {
        
    }
    public class DeleteGroupRoleCompletedEvent
    {

    }

    public class GroupChangeCompletedEvent
    {

    }
    public class RoleChangeCompletedEvent
    {

    }
    public class FunctionChangeCompletedEvent
    {

    }

    public class OperationChangeCompletedEvent
    {

    }

    public class GetAllUserNameCompletedEvent
    {

    }
    public class ShowWaringSegments
    {

    }
}
