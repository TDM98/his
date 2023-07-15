using System;
using System.IO;
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
    public interface IVideoDiagnosticExplorer
    {
        Stream VideoStream { get; set; }
        PatientPCLImagingResult ObjPatientPCLImagingResult { get; set; }
    }

}
