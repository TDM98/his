using System;
using DataEntities;

namespace Ax.ViewContracts.SL
{
    public interface IDepartmentTreeEx
    {
        //object ActiveContent { get; set; }
        RefDepartmentsTree CurRefDepartmentsTree { get; set; }
    }
}
