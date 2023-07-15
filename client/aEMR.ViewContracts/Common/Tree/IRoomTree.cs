/*
 * 20170407 #001 CMN: Enable get Department Only
*/
using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IRoomTree
    {
        //RefDepartment DefaultDepartment { get; set; }
        //==== #001
        bool DeptOnly { get; set; }
        //==== #001
    }
}
