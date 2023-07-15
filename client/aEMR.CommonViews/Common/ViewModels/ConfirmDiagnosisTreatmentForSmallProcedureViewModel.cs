using aEMR.Common.BaseModel;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IConfirmDiagnosisTreatmentForSmallProcedure)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConfirmDiagnosisTreatmentForSmallProcedureViewModel : ViewModelBase, IConfirmDiagnosisTreatmentForSmallProcedure
    {
        #region Properties
        private IDiagnosisTreatmentHistoriesTree _UCDiagnosisTreatmentHistoriesTree;
        public IDiagnosisTreatmentHistoriesTree UCDiagnosisTreatmentHistoriesTree
        {
            get
            {
                return _UCDiagnosisTreatmentHistoriesTree;
            }
            set
            {
                if (_UCDiagnosisTreatmentHistoriesTree == value)
                {
                    return;
                }
                _UCDiagnosisTreatmentHistoriesTree = value;
                NotifyOfPropertyChange(() => UCDiagnosisTreatmentHistoriesTree);
            }
        }
        private IConsultationOld_V3 _UCConsultationOld;
        public IConsultationOld_V3 UCConsultationOld
        {
            get
            {
                return _UCConsultationOld;
            }
            set
            {
                if (_UCConsultationOld == value)
                {
                    return;
                }
                _UCConsultationOld = value;
                NotifyOfPropertyChange(() => UCConsultationOld);
            }
        }
        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
            }
        }
        private SmallProcedure _SmallProcedureObj;
        public SmallProcedure SmallProcedureObj
        {
            get => _SmallProcedureObj; set
            {
                if (_SmallProcedureObj == value)
                {
                    return;
                }
                _SmallProcedureObj = value;
                NotifyOfPropertyChange(() => SmallProcedureObj);
            }
        }
        #endregion
        #region Events
        [ImportingConstructor]
        public ConfirmDiagnosisTreatmentForSmallProcedureViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            UCDiagnosisTreatmentHistoriesTree = Globals.GetViewModel<IDiagnosisTreatmentHistoriesTree>();
            UCConsultationOld = Globals.GetViewModel<IConsultationOld_V3>();
        }
        public void btnConfirm()
        {
            if (UCConsultationOld != null && UCConsultationOld.refIDC10List != null && UCConsultationOld.refIDC10List.Count > 0)
            {
                SmallProcedureObj = new SmallProcedure();
                DiseasesReference CurrentICD = UCConsultationOld.refIDC10List.First(x => x.IsMain).DiseasesReference.DeepCopy();
                string aBeforeDiagTreatment = CurrentICD.DiseaseNameVN;
                CurrentICD.DiseaseNameVN = UCConsultationOld.DiagTrmtItem.DiagnosisFinal;
                SmallProcedureObj.Diagnosis = CurrentICD.DiseaseNameVN;
                SmallProcedureObj.BeforeICD10 = CurrentICD;
                if (UCConsultationOld.DiagTrmtItem != null)
                {
                    SmallProcedureObj.ServiceRecID = UCConsultationOld.DiagTrmtItem.ServiceRecID.GetValueOrDefault();
                }
            }
            TryClose();
        }
        protected override void OnActivate()
        {
            ActivateItem(UCDiagnosisTreatmentHistoriesTree);
            ActivateItem(UCConsultationOld);
            UCDiagnosisTreatmentHistoriesTree.CurrentViewCase = 2;
            if (UCDiagnosisTreatmentHistoriesTree != null && Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID > 0)
            {
                UCDiagnosisTreatmentHistoriesTree.GetPatientServicesTreeView_ByPtRegistrationID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
            }
            base.OnActivate();
        }
        #endregion
    }
}