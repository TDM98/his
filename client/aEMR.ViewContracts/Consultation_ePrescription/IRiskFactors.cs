using System.Windows;
using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IRiskFactors
    {
        bool isEdit { get; set; }
        long? PatientID { get; set; }
        RiskFactors curRiskFactors { get; set; }
    }
}
