using System;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IDepartmentTreeEx
    {
        //object ActiveContent { get; set; }
        RefDepartmentsTree CurRefDepartmentsTree { get; set; }
    }
}
