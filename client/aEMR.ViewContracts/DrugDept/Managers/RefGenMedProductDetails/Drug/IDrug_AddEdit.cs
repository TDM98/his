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
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IDrug_AddEdit
    {
        //IDrugClasses_SearchPaging_Drug UCDrugClassesSearchPagingDrug { get; set; }
        //IRefCountries_SearchPaging UCRefCountriesSearchPaging { get; set; }
        //IRefUnitsCal_SearchPaging UCRefUnitsCalSearchPaging { get; set; }
        //IRefUnitsUse_SearchPaging UCRefUnitsUseSearchPaging { get; set; }

        void InitializeNewItem(Int64 pV_MedProductType);
        DataEntities.RefGenMedProductDetails ObjRefGenMedProductDetails_Current { get; set; }
        string TitleForm { get; set; }
        void SupplierGenMedProduct_LoadDrugIDNotPaging(long GenMedProductID);
        
        //DrugClass ObjDrugClasses_Selected { get; set; }
        //RefCountry ObjRefCountry_Selected { get; set; }
        //RefUnit ObjRefUnitCal_Selected { get; set; }
        //RefUnit ObjRefUnitUse_Selected { get; set; }
        ////Int64 V_MedProductType { get; set; }
    }
}
