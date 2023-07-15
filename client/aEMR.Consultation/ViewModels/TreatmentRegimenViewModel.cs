using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Controls;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(ITreatmentRegimen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class TreatmentRegimenViewModel : Conductor<object>, ITreatmentRegimen
    {
        [ImportingConstructor]
        public TreatmentRegimenViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
        }
        private ObservableCollection<RefTreatmentRegimen> _RefTreatmentRegimenCollection;
        public ObservableCollection<RefTreatmentRegimen> RefTreatmentRegimenCollection
        {
            get => _RefTreatmentRegimenCollection; set
            {
                _RefTreatmentRegimenCollection = value;
                NotifyOfPropertyChange(() => RefTreatmentRegimenCollection);
            }
        }
        private ObservableCollection<RefTreatmentRegimen> _RefTreatmentRegimenCollectionForCombobox;
        public ObservableCollection<RefTreatmentRegimen> RefTreatmentRegimenCollectionForCombobox
        {
            get => _RefTreatmentRegimenCollectionForCombobox; set
            {
                _RefTreatmentRegimenCollectionForCombobox = value;
                NotifyOfPropertyChange(() => RefTreatmentRegimenCollectionForCombobox);
            }
        }
        private RefTreatmentRegimen _SelectedRefTreatmentRegimen;
        public RefTreatmentRegimen SelectedRefTreatmentRegimen
        {
            get => _SelectedRefTreatmentRegimen; set
            {
                _SelectedRefTreatmentRegimen = value;
                NotifyOfPropertyChange(() => SelectedRefTreatmentRegimen);
            }
        }
        private CollectionViewSource CV_RefTreatmentRegimenDrugDetailSource;
        private CollectionViewSource CV_RefTreatmentRegimenPCLDetailSource;
        private CollectionViewSource CV_RefTreatmentRegimenServiceDetailSource;
        private CollectionViewSource CV_RefTreatmentRegimenSource;
        public CollectionView CV_RefTreatmentRegimenDrugDetail { get; set; }
        public CollectionView CV_RefTreatmentRegimenPCLDetail { get; set; }
        public CollectionView CV_RefTreatmentRegimenServiceDetail { get; set; }
        public CollectionView CV_RefTreatmentRegimen { get; set; }
        private ObservableCollection<DrugClass> _GenericClasses;
        public ObservableCollection<DrugClass> GenericClasses
        {
            get => _GenericClasses; set
            {
                _GenericClasses = value;
                NotifyOfPropertyChange(() => GenericClasses);
            }
        }
        private long? _PtRegDetailID = null;
        public long? PtRegDetailID
        {
            get => _PtRegDetailID; set
            {
                _PtRegDetailID = value;
                NotifyOfPropertyChange(() => PtRegDetailID);
            }
        }
        private bool _IsEditView = true;
        public bool IsEditView
        {
            get => _IsEditView; set
            {
                _IsEditView = value;
                NotifyOfPropertyChange(() => IsEditView);
            }
        }
        private List<string> _listICD10Codes = null;
        public List<string> listICD10Codes
        {
            get
            {
                return _listICD10Codes;
            }
            set
            {
                if (_listICD10Codes != value)
                {
                    _listICD10Codes = value;
                }
                NotifyOfPropertyChange(() => listICD10Codes);
            }
        }
        #region Methods
        private void LoadTreatmentRegimenCollection()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRefTreatmentRegimensAndDetail(PtRegDetailID, listICD10Codes, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                RefTreatmentRegimenCollection = contract.EndGetRefTreatmentRegimensAndDetail(asyncResult).ToObservableCollection();
                                CV_RefTreatmentRegimenSource = new CollectionViewSource { Source = RefTreatmentRegimenCollection };
                                CV_RefTreatmentRegimen = (CollectionView)CV_RefTreatmentRegimenSource.View;
                                NotifyOfPropertyChange(() => CV_RefTreatmentRegimen);
                                if (!IsEditView)
                                {
                                    RefTreatmentRegimenCollectionForCombobox = RefTreatmentRegimenCollection.DeepCopy();
                                    SelectedRefTreatmentRegimen = RefTreatmentRegimenCollectionForCombobox.FirstOrDefault();
                                    RefTreatmentRegimenCollectionForCombobox.Insert(0, new RefTreatmentRegimen
                                    {
                                        TreatmentRegimenID = 0,
                                        TreatmentRegimenName = eHCMSResources.T0822_G1_TatCa,
                                        RefTreatmentRegimenDrugDetails = RefTreatmentRegimenCollection.SelectMany(x => x.RefTreatmentRegimenDrugDetails).ToObservableCollection(),
                                        RefTreatmentRegimenPCLDetails = RefTreatmentRegimenCollection.SelectMany(x => x.RefTreatmentRegimenPCLDetails).ToObservableCollection(),
                                        RefTreatmentRegimenServiceDetails = RefTreatmentRegimenCollection.SelectMany(x => x.RefTreatmentRegimenServiceDetails).ToObservableCollection()
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
            });
            t.Start();
        }
        private void LoadGenericClasses()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetSearchDrugDeptClasses(null, (long)AllLookupValues.MedProductType.HOAT_CHAT, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var mResults = contract.EndGetSearchDrugDeptClasses(asyncResult);
                                GenericClasses = mResults.ToList().Where(x => x.DrugClassID != x.ParDrugClassID && !mResults.Any(y => y.ParDrugClassID == x.DrugClassID)).ToObservableCollection();
                                foreach (var item in GenericClasses)
                                {
                                    var mParent = mResults.FirstOrDefault(x => x.DrugClassID == item.ParDrugClassID);
                                    if (mParent != null) item.ParentText = mParent.DrugClassCode + " - " + mParent.FaName;
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        #endregion
        #region Events
        public void gvRefTreatmentRegimen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CV_RefTreatmentRegimenDrugDetailSource = new CollectionViewSource { Source = SelectedRefTreatmentRegimen == null ? new ObservableCollection<RefTreatmentRegimenDrugDetail>() : SelectedRefTreatmentRegimen.RefTreatmentRegimenDrugDetails.ToList().OrderBy(x => x.V_TreatmentPeriodic).ToObservableCollection() };
            CV_RefTreatmentRegimenDrugDetail = (CollectionView)CV_RefTreatmentRegimenDrugDetailSource.View;
            CV_RefTreatmentRegimenDrugDetail.GroupDescriptions.Add(new PropertyGroupDescription("TreatmentPeriodic"));
            CV_RefTreatmentRegimenDrugDetail.Filter = mItem =>
            {
                RefTreatmentRegimenDrugDetail SItem = mItem as RefTreatmentRegimenDrugDetail;
                return SItem != null && !SItem.IsDeleted;
            };
            NotifyOfPropertyChange(() => CV_RefTreatmentRegimenDrugDetail);

            CV_RefTreatmentRegimenPCLDetailSource = new CollectionViewSource { Source = SelectedRefTreatmentRegimen == null ? new ObservableCollection<RefTreatmentRegimenPCLDetail>() : SelectedRefTreatmentRegimen.RefTreatmentRegimenPCLDetails.ToList().OrderBy(x => x.V_TreatmentPeriodic).ToObservableCollection() };
            CV_RefTreatmentRegimenPCLDetail = (CollectionView)CV_RefTreatmentRegimenPCLDetailSource.View;
            CV_RefTreatmentRegimenPCLDetail.GroupDescriptions.Add(new PropertyGroupDescription("TreatmentPeriodic"));
            CV_RefTreatmentRegimenPCLDetail.Filter = mItem =>
            {
                RefTreatmentRegimenPCLDetail SItem = mItem as RefTreatmentRegimenPCLDetail;
                return SItem != null && !SItem.IsDeleted;
            };
            NotifyOfPropertyChange(() => CV_RefTreatmentRegimenPCLDetail);

            CV_RefTreatmentRegimenServiceDetailSource = new CollectionViewSource { Source = SelectedRefTreatmentRegimen == null ? new ObservableCollection<RefTreatmentRegimenServiceDetail>() : SelectedRefTreatmentRegimen.RefTreatmentRegimenServiceDetails.ToList().OrderBy(x => x.V_TreatmentPeriodic).ToObservableCollection() };
            CV_RefTreatmentRegimenServiceDetail = (CollectionView)CV_RefTreatmentRegimenServiceDetailSource.View;
            CV_RefTreatmentRegimenServiceDetail.GroupDescriptions.Add(new PropertyGroupDescription("TreatmentPeriodic"));
            CV_RefTreatmentRegimenServiceDetail.Filter = mItem =>
            {
                RefTreatmentRegimenServiceDetail SItem = mItem as RefTreatmentRegimenServiceDetail;
                return SItem != null && !SItem.IsDeleted;
            };
            NotifyOfPropertyChange(() => CV_RefTreatmentRegimenServiceDetail);

            if (sender is DataGrid && (sender as DataGrid).Columns.Count > 0)
                (sender as DataGrid).CurrentCell = new DataGridCellInfo((sender as DataGrid).CurrentItem, (sender as DataGrid).Columns[0]);
        }
        public void clEditClick(object sender, RoutedEventArgs e)
        {
            ITreatmentRegimenEdit aView = Globals.GetViewModel<ITreatmentRegimenEdit>();
            aView.gRefTreatmentRegimen = (sender as FrameworkElement).DataContext as RefTreatmentRegimen;
            aView.GenericClasses = this.GenericClasses;
            GlobalsNAV.ShowDialog_V3<ITreatmentRegimenEdit>(aView, null, null, false, true);
            RefTreatmentRegimenCollection[RefTreatmentRegimenCollection.IndexOf((sender as FrameworkElement).DataContext as RefTreatmentRegimen)] = aView.gRefTreatmentRegimen;
            SelectedRefTreatmentRegimen = aView.gRefTreatmentRegimen;
            CV_RefTreatmentRegimen.Refresh();
            CV_RefTreatmentRegimenDrugDetail.Refresh();
        }
        public void clDelete(object sender, RoutedEventArgs e)
        {
            RefTreatmentRegimen mRefTreatmentRegimen = (sender as FrameworkElement).DataContext as RefTreatmentRegimen;
            mRefTreatmentRegimen.IsDeleted = true;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginEditRefTreatmentRegimen(mRefTreatmentRegimen, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                RefTreatmentRegimen OutRefTreatmentRegimen;
                                if (contract.EndEditRefTreatmentRegimen(out OutRefTreatmentRegimen, asyncResult))
                                {
                                    mRefTreatmentRegimen = OutRefTreatmentRegimen;
                                    RefTreatmentRegimenCollection.Remove((sender as FrameworkElement).DataContext as RefTreatmentRegimen);
                                    CV_RefTreatmentRegimen.Refresh();
                                    MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void btnAddNew()
        {
            ITreatmentRegimenEdit aView = Globals.GetViewModel<ITreatmentRegimenEdit>();
            aView.gRefTreatmentRegimen = new RefTreatmentRegimen {
                      RefTreatmentRegimenDrugDetails = new ObservableCollection<RefTreatmentRegimenDrugDetail>()
                    , RefTreatmentRegimenPCLDetails = new ObservableCollection<RefTreatmentRegimenPCLDetail>()
                    , RefTreatmentRegimenServiceDetails = new ObservableCollection<RefTreatmentRegimenServiceDetail>() };

            aView.GenericClasses = this.GenericClasses;
            GlobalsNAV.ShowDialog_V3<ITreatmentRegimenEdit>(aView, null, null, false, true);
            if (aView.gRefTreatmentRegimen.TreatmentRegimenID > 0)
            {
                RefTreatmentRegimenCollection.Add(aView.gRefTreatmentRegimen);
                SelectedRefTreatmentRegimen = aView.gRefTreatmentRegimen;
                CV_RefTreatmentRegimen.Refresh();
                CV_RefTreatmentRegimenDrugDetail.Refresh();
            }
        }
        public void RefTreatmentRegimens_Populating(object sender, PopulatingEventArgs e)
        {
            AxAutoComplete cboContext = sender as AxAutoComplete;
            e.Cancel = true;
            if (string.IsNullOrEmpty(cboContext.SearchText))
            {
                CV_RefTreatmentRegimen.Filter = null;
            }
            else
            {
                CV_RefTreatmentRegimen.Filter = mItem =>
                {
                    RefTreatmentRegimen SItem = mItem as RefTreatmentRegimen;
                    return SItem != null && !SItem.IsDeleted && (SItem.ICD10Code.ToUpper().Contains(cboContext.SearchText.ToUpper()) || Globals.RemoveVietnameseString(SItem.TreatmentRegimenName).ToUpper().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToUpper()));
                };
            }
            CV_RefTreatmentRegimen.Refresh();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            if (PtRegDetailID > 0)
                IsEditView = false;
            if (IsEditView)
                LoadGenericClasses();
            LoadTreatmentRegimenCollection();
        }
        #endregion
    }
}