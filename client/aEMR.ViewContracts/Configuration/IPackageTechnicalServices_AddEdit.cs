using DataEntities;
using DataEntities.MedicalInstruction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aEMR.ViewContracts.Configuration
{
    public interface IPackageTechnicalServices_AddEdit
    {
        PackageTechnicalService ObjPackageTechnicalServices_Current { get; set; }
        string TitleForm { get; set; }
        void InitializeNewItem();
        void InitializeItem();
    }
}
