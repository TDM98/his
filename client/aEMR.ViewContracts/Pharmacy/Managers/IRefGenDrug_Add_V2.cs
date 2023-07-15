using System.Collections.ObjectModel;
using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IRefGenDrug_Add_V2
    {
        bool IsAddFinishClosed { get; set; }
        RefGenericDrugDetail NewDrug { get; set; }
        bool IsAdd { get; set; }
        bool IsEdit { get; set; }
        string TitleForm { get; set; }
        bool CanEdit { get; set; }

        #region properties reference member

        ObservableCollection<Supplier> Suppliers { get; set; }
        ObservableCollection<Hospital> Hospitals { get; set; }
        ObservableCollection<RefGenDrugBHYT_Category> AllRefGenDrugBHYT_Categorys { get; set; }
        ObservableCollection<RefGenericDrugCategory_2> RefGenericDrugCategory_2s { get; set; }
        ObservableCollection<RefCountry> AllCountries { get; set; }
        ObservableCollection<RefUnit> Units { get; set; }
        ObservableCollection<PharmaceuticalCompany> AllPharmaceuticalCompanies { get; set; }
        ObservableCollection<DrugClass> AllFamilyTherapies { get; set; }

        #endregion
    }

    public interface IRefGenDrug_Add_V2New
    {
        bool IsAddFinishClosed { get; set; }
        RefGenericDrugDetail NewDrug { get; set; }
        bool IsAdd { get; set; }
        bool IsEdit { get; set; }
        string TitleForm { get; set; }
        bool CanEdit { get; set; }

        #region properties reference member

        ObservableCollection<Supplier> Suppliers { get; set; }
        ObservableCollection<Hospital> Hospitals { get; set; }
        ObservableCollection<RefGenDrugBHYT_Category> AllRefGenDrugBHYT_Categorys { get; set; }
        ObservableCollection<RefGenericDrugCategory_2> RefGenericDrugCategory_2s { get; set; }
        ObservableCollection<RefCountry> AllCountries { get; set; }
        ObservableCollection<RefUnit> Units { get; set; }
        ObservableCollection<PharmaceuticalCompany> AllPharmaceuticalCompanies { get; set; }
        ObservableCollection<DrugClass> AllFamilyTherapies { get; set; }

        #endregion
    }
}
