using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace aEMR.Infrastructure.Events
{
    public class InsuranceBenefitCategories_Event
    {
    }

    public class dgListDblClickSelectInsuranceBenefitCategories_Event
    {
        public object InsuranceBenefitCategories_Current { get; set; }
    }

    public class dgInsuranceBenefitCategoriesListClickSelectionChanged_Event
    {
        public object InsuranceBenefitCategories_Current { get; set; }
    }

    public class InsuranceBenefitCategories_Event_Save
    {
        public object Result { get; set; }
    }
}
