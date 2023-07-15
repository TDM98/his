using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using DataEntities;
using eHCMSLanguage;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows.Data;

namespace aEMR.Consultation.ViewModels
{
    [Export(typeof(IInfectionCaseStatistics)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InfectionCaseStatisticsViewModel : ViewModelBase, IInfectionCaseStatistics
    {
        #region Properties
        private DateTime _StartDate;
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                if (_StartDate == value)
                {
                    return;
                }
                _StartDate = value;
                NotifyOfPropertyChange(() => StartDate);
            }
        }
        private DateTime _EndDate;
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                if (_EndDate == value)
                {
                    return;
                }
                _EndDate = value;
                NotifyOfPropertyChange(() => EndDate);
            }
        }
        private ICollectionView _InfectionCaseView;
        public ICollectionView InfectionCaseView
        {
            get
            {
                return _InfectionCaseView;
            }
            set
            {
                if (_InfectionCaseView == value)
                {
                    return;
                }
                _InfectionCaseView = value;
                NotifyOfPropertyChange(() => InfectionCaseView);
            }
        }
        private ObservableCollection<AntibioticTreatmentMedProductDetailSummaryContent> _AntibioticTreatmentMedProductDetailSummaryContentCollection;
        public ObservableCollection<AntibioticTreatmentMedProductDetailSummaryContent> AntibioticTreatmentMedProductDetailSummaryContentCollection
        {
            get
            {
                return _AntibioticTreatmentMedProductDetailSummaryContentCollection;
            }
            set
            {
                if (_AntibioticTreatmentMedProductDetailSummaryContentCollection == value)
                {
                    return;
                }
                _AntibioticTreatmentMedProductDetailSummaryContentCollection = value;
                NotifyOfPropertyChange(() => AntibioticTreatmentMedProductDetailSummaryContentCollection);
                InfectionCaseView = CollectionViewSource.GetDefaultView(AntibioticTreatmentMedProductDetailSummaryContentCollection);
                //InfectionCaseView.SortDescriptions.Add(new SortDescription("PtRegistrationID", ListSortDirection.Descending));
                InfectionCaseView.GroupDescriptions.Add(new PropertyGroupDescription("CurrentInfectionCase"));
                InfectionCaseView.GroupDescriptions.Add(new PropertyGroupDescription("CurrentAntibioticTreatment"));
            }
        }
        private ObservableCollection<Lookup> _InfectionCaseStatusCollection;
        public ObservableCollection<Lookup> InfectionCaseStatusCollection
        {
            get
            {
                return _InfectionCaseStatusCollection;
            }
            set
            {
                if (_InfectionCaseStatusCollection == value)
                {
                    return;
                }
                _InfectionCaseStatusCollection = value;
                NotifyOfPropertyChange(() => InfectionCaseStatusCollection);
            }
        }
        private long _V_InfectionCaseStatus;
        public long V_InfectionCaseStatus
        {
            get
            {
                return _V_InfectionCaseStatus;
            }
            set
            {
                if (_V_InfectionCaseStatus == value)
                {
                    return;
                }
                _V_InfectionCaseStatus = value;
                NotifyOfPropertyChange(() => V_InfectionCaseStatus);
            }
        }
        private string _DrugName;
        public string DrugName
        {
            get
            {
                return _DrugName;
            }
            set
            {
                if (_DrugName == value)
                {
                    return;
                }
                _DrugName = value;
                NotifyOfPropertyChange(() => DrugName);
            }
        }
        #endregion
        #region Events
        [ImportingConstructor]
        public InfectionCaseStatisticsViewModel()
        {
            InfectionCaseStatusCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_InfectionCaseStatus).ToObservableCollection();
            InfectionCaseStatusCollection.Insert(0, new Lookup { LookupID = 0, ObjectValue = string.Format("-- {0} --", eHCMSResources.T0822_G1_TatCa) });
            V_InfectionCaseStatus = InfectionCaseStatusCollection.FirstOrDefault().LookupID;
            EndDate = Globals.GetCurServerDateTime().Date;
            StartDate = EndDate.AddMonths(-1);
        }
        public void btnSearch()
        {
            GetInfectionCaseAllContentInfo();
        }
        public void btnExportExcel()
        {
            SaveFileDialog mFileDialog = new SaveFileDialog()
            {
                DefaultExt = ".xls",
                Filter = "CSV file (*.csv)|*.csv",
                FilterIndex = 1
            };
            if (mFileDialog.ShowDialog() != true)
            {
                return;
            }
            ReportParameters ParameterCollection = new ReportParameters();
            ParameterCollection.reportName = ReportName.InfectionCaseStatistics;
            ParameterCollection.ReportType = ReportType.BAOCAO_TONGHOP;
            ParameterCollection.Show = "InfectionCaseStatistics";
            ParameterCollection.IsExportToCSV = true;
            ParameterCollection.FromDate = StartDate;
            ParameterCollection.ToDate = EndDate;
            ParameterCollection.Status = V_InfectionCaseStatus;
            ParameterCollection.BrandName = DrugName;
            ExportToExcelGeneric.Action(ParameterCollection, mFileDialog, this);
        }
        #endregion
        #region Methods
        private void GetInfectionCaseAllContentInfo()
        {
            AntibioticTreatmentMedProductDetailSummaryContentCollection = new ObservableCollection<AntibioticTreatmentMedProductDetailSummaryContent>();
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInfectionCaseAllContentInfo(StartDate, EndDate, V_InfectionCaseStatus, DrugName, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var GettedCollection = contract.EndGetInfectionCaseAllContentInfo(asyncResult);
                                foreach (var aInfectionCase in GettedCollection)
                                {
                                    if (aInfectionCase.AntibioticTreatmentCollection == null || aInfectionCase.AntibioticTreatmentCollection.Count == 0)
                                    {
                                        AntibioticTreatmentMedProductDetailSummaryContent CurrentAntibioticTreatmentMedProductDetailSummaryContent = new AntibioticTreatmentMedProductDetailSummaryContent();
                                        CurrentAntibioticTreatmentMedProductDetailSummaryContent.CurrentInfectionCase = aInfectionCase;
                                        AntibioticTreatmentMedProductDetailSummaryContentCollection.Add(CurrentAntibioticTreatmentMedProductDetailSummaryContent);
                                        continue;
                                    }
                                    foreach (var aAntibioticTreatment in aInfectionCase.AntibioticTreatmentCollection)
                                    {
                                        if (aAntibioticTreatment.AntibioticTreatmentMedProductDetailCollection == null || aAntibioticTreatment.AntibioticTreatmentMedProductDetailCollection.Count == 0)
                                        {
                                            AntibioticTreatmentMedProductDetailSummaryContent CurrentAntibioticTreatmentMedProductDetailSummaryContent = new AntibioticTreatmentMedProductDetailSummaryContent();
                                            CurrentAntibioticTreatmentMedProductDetailSummaryContent.CurrentAntibioticTreatment = aAntibioticTreatment;
                                            CurrentAntibioticTreatmentMedProductDetailSummaryContent.CurrentInfectionCase = aInfectionCase;
                                            AntibioticTreatmentMedProductDetailSummaryContentCollection.Add(CurrentAntibioticTreatmentMedProductDetailSummaryContent);
                                            continue;
                                        }
                                        foreach (var aAntibioticTreatmentMedProductDetail in aAntibioticTreatment.AntibioticTreatmentMedProductDetailCollection)
                                        {
                                            AntibioticTreatmentMedProductDetailSummaryContent CurrentAntibioticTreatmentMedProductDetailSummaryContent = new AntibioticTreatmentMedProductDetailSummaryContent();
                                            PropertyCopierHelper.CopyPropertiesTo(aAntibioticTreatmentMedProductDetail, CurrentAntibioticTreatmentMedProductDetailSummaryContent);
                                            CurrentAntibioticTreatmentMedProductDetailSummaryContent.CurrentAntibioticTreatment = aAntibioticTreatment;
                                            CurrentAntibioticTreatmentMedProductDetailSummaryContent.CurrentInfectionCase = aInfectionCase;
                                            AntibioticTreatmentMedProductDetailSummaryContentCollection.Add(CurrentAntibioticTreatmentMedProductDetailSummaryContent);
                                        }
                                    }
                                }
                                InfectionCaseView.Refresh();
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        #endregion
    }
}