using aEMR.Common.BaseModel;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace aEMR.CommonViews.ViewModels
{
    [Export(typeof(IPCLResultSummary)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLResultSummaryViewModel : ViewModelBase, IPCLResultSummary
    {
        #region Properties
        public IPatientPCLLaboratoryResult UCPatientPCLLaboratoryResult { get; set; }
        public IPatientPCLImagingResult UCPatientPCLImagingResult { get; set; }
        public IPatientInfo UCPatientProfileInfo { get; set; }
        private TabControl TCMain;
        public bool IsHasDescription { get; set; } = false;
        public string CurrentDescription { get; set; }
        public bool IsHtmlContent { get; set; } = false;
        #endregion
        #region Events
        [ImportingConstructor]
        public PCLResultSummaryViewModel(IWindsorContainer aContainer, INavigationService aNavigation, IEventAggregator aEventArg, ISalePosCaching aCaching)
        {
            UCPatientPCLLaboratoryResult = Globals.GetViewModel<IPatientPCLLaboratoryResult>();
            UCPatientPCLLaboratoryResult.IsShowSummaryContent = false;
            UCPatientPCLImagingResult = Globals.GetViewModel<IPatientPCLImagingResult>();
            UCPatientPCLImagingResult.IsShowSummaryContent = false;
            UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();
            UCPatientPCLLaboratoryResult.IsShowCheckTestItem = true;
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(UCPatientPCLLaboratoryResult);
            ActivateItem(UCPatientPCLImagingResult);
            ActivateItem(UCPatientProfileInfo);
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            DeactivateItem(UCPatientPCLLaboratoryResult, close);
            DeactivateItem(UCPatientPCLImagingResult, close);
            DeactivateItem(UCPatientProfileInfo, close);
            Globals.EventAggregator.Unsubscribe(this);
        }
        public void AddPCLDescriptionCmd()
        {
            if (TCMain == null)
            {
                return;
            }
            if ((TCMain.SelectedItem as TabItem).Name == "TIImaging")
            {
                if (UCPatientPCLImagingResult.CurrentPCLImagingResult == null || UCPatientPCLImagingResult.CurrentPCLImagingResult.PatientPCLReqID == 0 ||
                    string.IsNullOrEmpty(UCPatientPCLImagingResult.CurrentPCLImagingResult.TemplateResult) ||
                    UCPatientPCLImagingResult.CurrentPCLImagingResult.PCLExamDate == null ||
                    UCPatientPCLImagingResult.CurrentPCLImagingResult.PatientPCLRequest == null ||
                    UCPatientPCLImagingResult.CurrentPCLImagingResult.PatientPCLRequest.PCLRequestNumID == null ||
                    string.IsNullOrEmpty(UCPatientPCLImagingResult.CurrentPCLImagingResult.PatientPCLRequest.PCLExamTypeName))
                {
                    return;
                }
                CurrentDescription = BuildDescription(UCPatientPCLImagingResult.CurrentPCLImagingResult);
                IsHasDescription = true;
            }
            else
            {
                if (UCPatientPCLLaboratoryResult.PCLLaboratoryResultCollection == null ||
                    !UCPatientPCLLaboratoryResult.PCLLaboratoryResultCollection.Any(x => x.IsChecked))
                {
                    return;
                }
                if (UCPatientPCLLaboratoryResult != null &&
                    UCPatientPCLLaboratoryResult.SelectedPCLLaboratory != null &&
                    UCPatientPCLLaboratoryResult.SelectedPCLLaboratory.MedicalInstructionDate == null)
                {
                    UCPatientPCLLaboratoryResult.SelectedPCLLaboratory.MedicalInstructionDate = UCPatientPCLLaboratoryResult.SelectedPCLLaboratory.CreatedDate;
                }
                if (UCPatientPCLLaboratoryResult.SelectedPCLLaboratory == null || UCPatientPCLLaboratoryResult.SelectedPCLLaboratory.PatientPCLReqID == 0 ||
                    UCPatientPCLLaboratoryResult.SelectedPCLLaboratory.MedicalInstructionDate == null ||
                    UCPatientPCLLaboratoryResult.SelectedPCLLaboratory.PCLRequestNumID == null)
                {
                    return;
                }
                foreach (var aItem in UCPatientPCLLaboratoryResult.PCLLaboratoryResultCollection.Where(x => x.PCLExamTestItemID > 0))
                {
                    if (!UCPatientPCLLaboratoryResult.PCLLaboratoryResultCollection.Any(x => x.PCLExamTestItemID == 0 && x.PCLSectionID == aItem.PCLSectionID))
                    {
                        continue;
                    }
                    aItem.SectionName = UCPatientPCLLaboratoryResult.PCLLaboratoryResultCollection.First(x => x.PCLExamTestItemID == 0 && x.PCLSectionID == aItem.PCLSectionID).PCLExamTestItemName;
                }
                CurrentDescription = BuildDescription(UCPatientPCLLaboratoryResult.SelectedPCLLaboratory, UCPatientPCLLaboratoryResult.PCLLaboratoryResultCollection.Where(x => x.IsChecked).ToList());
                IsHasDescription = true;
            }
            TryClose();
        }
        public void TCMain_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            TCMain = sender as TabControl;
        }
        #endregion
        #region Methods
        public void InitPatientInfo(IRegistration_DataStorage aRegistration_DataStorage)
        {
            UCPatientPCLLaboratoryResult.Registration_DataStorage = aRegistration_DataStorage;
            UCPatientPCLImagingResult.Registration_DataStorage = aRegistration_DataStorage;
            if (aRegistration_DataStorage == null)
            {
                UCPatientProfileInfo.CurrentPatient = null;
            }
            else
            {
                UCPatientProfileInfo.CurrentPatient = aRegistration_DataStorage.CurrentPatient;
            }
        }
        private string BuildDescription(PatientPCLImagingResult aImagingResult)
        {
            aImagingResult.TemplateResult = string.Join("\n", aImagingResult.TemplateResult.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
            aImagingResult.TemplateResult = aImagingResult.TemplateResult.TrimStart(new char[] { ' ', '\n' });
            aImagingResult.TemplateResult = aImagingResult.TemplateResult.Replace("\n", string.Format("\n{0}+ ", (IsHtmlContent ? "&nbsp;" : "\t")));
            return string.Format("{0:dd/MM/yyy} - {1}\n- {2}\n{4}+ {3}\n", aImagingResult.PCLExamDate, IsHtmlContent ? string.Format("<b>{0}</b>", aImagingResult.PatientPCLRequest.PCLRequestNumID) : aImagingResult.PatientPCLRequest.PCLRequestNumID, (IsHtmlContent ? string.Format("<b>{0}</b>", aImagingResult.PatientPCLRequest.PCLExamTypeName) : aImagingResult.PatientPCLRequest.PCLExamTypeName), aImagingResult.TemplateResult, (IsHtmlContent ? "&nbsp;" : "\t"));
        }
        private string BuildDescription(PatientPCLRequest aPCLRequest, IList<PatientPCLLaboratoryResultDetail> aLaboratoryResultCollection)
        {
            string CurrentTemplateResult = string.Join("\n", aLaboratoryResultCollection.GroupBy(x => x.SectionName).Select(x => string.Format("-{0}\n{1}", (IsHtmlContent ? string.Format("<b>{0}</b>", x.Key) : x.Key), string.Join("\n", x.ToList().Select(i => string.Format("{0}+ {1}:{0}{2} {3}", (IsHtmlContent ? "&nbsp;" : "\t"), i.PCLExamTestItemName, i.Value, (string.IsNullOrEmpty(i.PCLExamTestItemUnit) ? "" : string.Format("({0})", i.PCLExamTestItemUnit))))))));
            return string.Format("{0:dd/MM/yyy} - {1}\n{2}", aPCLRequest.MedicalInstructionDate, (IsHtmlContent ? string.Format("<b>{0}</b>", aPCLRequest.PCLRequestNumID) : aPCLRequest.PCLRequestNumID), CurrentTemplateResult);
        }
        #endregion
    }
}