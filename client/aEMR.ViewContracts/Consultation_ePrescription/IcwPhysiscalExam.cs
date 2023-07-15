using System.Windows;
using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IcwPhysiscalExam
    {
        Visibility IsVisibility { get; set; }
        long? PatientID { get; set; }
        PhysicalExamination PtPhyExamItem { get; set; }
        bool isEdit { get; set; }
    }
}
