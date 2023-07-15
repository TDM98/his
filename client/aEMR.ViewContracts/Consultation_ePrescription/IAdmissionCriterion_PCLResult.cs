﻿using DataEntities;
using System.Collections.ObjectModel;
using System.Windows;

namespace aEMR.ViewContracts
{
    public interface IAdmissionCriterion_PCLResult
    {
        long AdmissionCriterionID { get; set; }
        long PtRegistrationID { get; set; }
        string PatientCode { get; set; }
        void InitInfo();
    }
}