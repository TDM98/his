using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public enum PatientType
    {
        NORMAL_PATIENT=1,
        INSUARED_PATIENT=2,
        TRANSFERRED_PATIENT=3,
        OTHERS=4
    }
}
