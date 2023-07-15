using DataEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aEMR.ViewContracts.Configuration
{
    public interface IExemptions_AddEdit
    {
        PromoDiscountProgram ObjExemptions_Current { get; set; }
        string TitleForm { get; set; }
        void InitializeNewItem();
        void InitializeItem();
    }
}
