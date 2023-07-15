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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Data;
using System.Globalization;
using System.Linq;
using DataEntities;
/*
 * 20180122 #001 CMN: Added RemainingQty for apply request invoice of drugdept
*/
namespace aEMR.Common.Converters
{
    public class DrugDeptConfReqGridGroupHeaderConverter : IValueConverter
    {
          
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            CollectionViewGroup cvg = value as CollectionViewGroup;
            string param = parameter as string;
            if (param == "ReqQty")
            {
                double reqQty = (double)cvg.Items.Sum(x => (x as ReqOutwardDrugClinicDeptPatient).ReqQty);
                return reqQty.ToString("#.###", currentCulture);
            }
            else if (param == "ApprovedQty")
            {
                double appQty = (Double)cvg.Items.Sum(x => (x as ReqOutwardDrugClinicDeptPatient).ApprovedQty);
                return appQty.ToString("#.###", currentCulture); ;
            }
            //▼====: #001
            else if (param == "RemainingQty")
            {
                double appQty = cvg == null || cvg.Items == null || cvg.Items.Count == 0 ? 0 : (Double)(cvg.Items.FirstOrDefault() as ReqOutwardDrugClinicDeptPatient).RemainingQty;
                return appQty.ToString("#.###", currentCulture); ;
            }
            //▲====: #001
            else if (param == "DrugBrandNameAndCode")
            {
                return cvg.Items.Select(x => (x as ReqOutwardDrugClinicDeptPatient).RefGenericDrugDetail.BrandNameAndCode).FirstOrDefault();
            }
            return null;
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
