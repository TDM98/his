using System.Windows;
using DataEntities;
/*
 * 20230318 #001 DatTB: Thêm biến lưu theo lần nhập viện
 */
namespace aEMR.ViewContracts
{
    public interface IInfectionControl
    {
        bool isEditMRBacteria { get; set; }
        bool isEditHosInfection { get; set; }
        long? PatientID { get; set; }
        //▼==== #001
        long? InPatientAdmDisDetailID { get; set; }
        //▲==== #001
        InfectionControl curIC_MRBacteria { get; set; }
        InfectionControl curIC_HosInfection { get; set; }
    }
}
