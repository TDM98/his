using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IUpdateDiseaseProgressionView
    {
        DiagnosisTreatment DiagTrmtItem { get; set; }
        bool UpdateOK { get; set; }
    }
}
