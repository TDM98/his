using System;
using System.Collections.ObjectModel;
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
/*
 * 20230608 #001 DatTB: Thêm các trường lưu bệnh phẩm xét nghiệm
 */
namespace aEMR.ViewContracts
{
    public interface IPatientPCLImagingItemResult 
    {
        ObservableCollection<PatientPCLImagingResultDetail> allPatientPCLImagingResultDetail{ get; set; }
        IAucHoldConsultDoctor aucHoldConsultDoctor { get; set; }

        PatientPCLImagingResult ObjPatientPCLImagingResult_General { get; set; }

        IAucHoldConsultDoctor aucDoctorResult { get; set; }
        System.Collections.ObjectModel.ObservableCollection<Resources> HIRepResourceCode { get; set; }

        bool MoveInDataGridWithArrow { get; set; }
        //▼==== #001
        string ParamName { get; set; }
        //▲==== #001
    }
}
