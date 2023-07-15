using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DataEntities;
using aEMR.Common.Collections;
using eHCMS.Services.Core;

namespace aEMR.ViewContracts
{
    public interface IHealthExaminationRecordInfo
    {
        void CancelCmd();
        void OkCmd();
        bool IsLoading { get; set; }
        Patient SelectedPatient { get; set; }

        HosClientContractPatient HosPatient { get; set; }
    }
}