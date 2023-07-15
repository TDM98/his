using System.Collections.ObjectModel;
using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IRefGenDrug_Add
    {
        bool IsAddFinishClosed { get; set; }
        RefGenericDrugDetail NewDrug { get; set; }
        bool IsAdd { get; set; }
        bool IsEdit { get; set; }
        string TitleForm { get; set; }

        #region properties reference member

        ObservableCollection<Supplier> Suppliers { get; set; }
        ObservableCollection<Hospital> Hospitals { get; set; }
        ObservableCollection<RefGenDrugBHYT_Category> RefGenDrugBHYT_Categorys { get; set; }
        ObservableCollection<RefGenericDrugCategory_2> RefGenericDrugCategory_2s { get; set; }
        ObservableCollection<RefCountry> Countries { get; set; }
        ObservableCollection<RefUnit> Units { get; set; }
        ObservableCollection<PharmaceuticalCompany> PharmaceuticalCompanies { get; set; }
        ObservableCollection<DrugClass> FamilyTherapies { get; set; }

        #endregion
    }
}
