using DataEntities;
using System;
using System.Collections.ObjectModel;
namespace aEMR.ViewContracts
{
    public interface IInsertUpdateTrainingForSubOrg
    {
        string TitleForm { get; set; }
        bool ISAdd { get; set; }
        TrainingForSubOrg TrainingForSubOrg_Current { get; set; }
    }
}
