using System;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using DataEntities;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.Common;
using System.Windows;
using eHCMSLanguage;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using aEMR.Infrastructure.Events;
using System.Windows.Media;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

/*
 * 20180904 #001 TBL: Tim hoi chan
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IFindConsultingDiagnosy)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class FindConsultingDiagnosyViewModel : Conductor<object>, IFindConsultingDiagnosy
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        #region Properties
        private ConsultingDiagnosysSearchCriteria _gSearchCriteria = new ConsultingDiagnosysSearchCriteria();
        public ConsultingDiagnosysSearchCriteria gSearchCriteria
        {
            get
            {
                return _gSearchCriteria;
            }
            set
            {
                _gSearchCriteria = value;
                NotifyOfPropertyChange(() => gSearchCriteria);
            }
        }
        private PagedSortableCollectionView<ConsultingDiagnosys> _ConsultingDiagnosysCollection;
        public PagedSortableCollectionView<ConsultingDiagnosys> ConsultingDiagnosysCollection
        {
            get
            {
                return _ConsultingDiagnosysCollection;
            }
            set
            {
                _ConsultingDiagnosysCollection = value;
                NotifyOfPropertyChange(() => ConsultingDiagnosysCollection);
            }
        }
        private List<Staff> _DoctorStaffs;
        public List<Staff> DoctorStaffs
        {
            get
            {
                return _DoctorStaffs;
            }
            set
            {
                _DoctorStaffs = value;
                NotifyOfPropertyChange(() => DoctorStaffs);
            }
        }
        private Staff _ConsultingDoctor;
        public Staff ConsultingDoctor
        {
            get
            {
                return _ConsultingDoctor;
            }
            set
            {
                _ConsultingDoctor = value;
                NotifyOfPropertyChange(() => ConsultingDoctor);
                if (ConsultingDoctor != null && gSearchCriteria != null)
                {
                    gSearchCriteria.ConsultingDoctorStaffID = ConsultingDoctor.StaffID;
                }
                else
                    gSearchCriteria.ConsultingDoctorStaffID = null;
            }
        }
        private bool _IsConsultingHistoryView = false;
        public bool IsConsultingHistoryView
        {
            get
            {
                return _IsConsultingHistoryView;
            }
            set
            {
                if (_IsConsultingHistoryView == value) return;
                _IsConsultingHistoryView = value;
                NotifyOfPropertyChange(() => IsConsultingHistoryView);
            }
        }
        #endregion
        #region Methods
        private void CreateSearchCriteria()
        {
            if (!IsConsultingHistoryView)
            {
                gSearchCriteria.IsApproved = true;
                gSearchCriteria.IsAllExamCompleted = true;
                gSearchCriteria.IsWaitSurgery = true;
            }
            /*▼====: #001*/
            gSearchCriteria.IsSurgeryCompleted = true;
            /*▲====: #001*/
            gSearchCriteria.IsConsultingHistoryView = this.IsConsultingHistoryView;
            gSearchCriteria.ToDate = DateTime.Now;
            gSearchCriteria.FromDate = gSearchCriteria.ToDate.Value.AddMonths(-1);
            DoctorStaffs = Globals.AllStaffs.Where(x => x.StaffCatgID == (long)StaffCatg.Bs).ToList();
            ConsultingDiagnosysCollection = new PagedSortableCollectionView<ConsultingDiagnosys>();
            ConsultingDiagnosysCollection.PageSize = 10;
            ConsultingDiagnosysCollection.PageIndex = 0;
            gSearchCriteria.PageIndex = ConsultingDiagnosysCollection.PageIndex;
            gSearchCriteria.PageSize = ConsultingDiagnosysCollection.PageSize;
            ConsultingDiagnosysCollection.OnRefresh += ConsultingDiagnosysCollection_OnRefresh;
        }
        private void ConsultingDiagnosysCollection_OnRefresh(object sender, RefreshEventArgs e)
        {
            gSearchCriteria.PageIndex = ConsultingDiagnosysCollection.PageIndex;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetReportConsultingDiagnosys(gSearchCriteria,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                ConsultingDiagnosysCollection.Clear();
                                int TotalItemCount;
                                var GettedReportConsultingDiagnosys = contract.EndGetReportConsultingDiagnosys(out TotalItemCount, asyncResult);
                                ConsultingDiagnosysCollection.TotalItemCount = TotalItemCount;
                                if (GettedReportConsultingDiagnosys != null)
                                {
                                    foreach (var item in GettedReportConsultingDiagnosys)
                                        ConsultingDiagnosysCollection.Add(item);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void cboDoctor_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboDoctor = sender as AutoCompleteBox;
            e.Cancel = true;
            if (!string.IsNullOrEmpty(cboDoctor.SearchText))
            {
                var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => x.FullName.ToLower().Contains(cboDoctor.SearchText.ToLower())).ToList());
                cboDoctor.ItemsSource = AllItemsContext;
                cboDoctor.PopulateComplete();
            }
            else
                gSearchCriteria.ConsultingDoctorStaffID = null;
        }
        #endregion
        #region Events
        [ImportingConstructor]
        public FindConsultingDiagnosyViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            CreateSearchCriteria();
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }
        public void ResetFilterCmd()
        {
            gSearchCriteria.ToDate = DateTime.Now;
            gSearchCriteria.FromDate = gSearchCriteria.ToDate.Value.AddMonths(-1);
            gSearchCriteria.PatientNameString = null;
            gSearchCriteria.FullName = null;
            gSearchCriteria.PatientCode = null;
            ConsultingDoctor = null;
            gSearchCriteria.ConsultingDoctorStaffID = null;
        }
        public void CancelCmd()
        {
            TryClose();
            Globals.EventAggregator.Unsubscribe(this);
        }
        public void SearchCmd()
        {
            gSearchCriteria.PageIndex = 1;
            ConsultingDiagnosysCollection_OnRefresh(null, null);
        }
        public void gvConsultingDiagnosy_DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            ConsultingDiagnosys SelectedConsultingDiagnosy = eventArgs.Value as ConsultingDiagnosys;
            if (SelectedConsultingDiagnosy.PtRegistrationID > 0 && !IsConsultingHistoryView)
            {
                MessageBox.Show(eHCMSResources.Z2148_G1_CaHoiChanDaDuocDK, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                return;
            }
            //if (!IsConsultingHistoryView)
            //{
            //    if (!string.IsNullOrEmpty(Globals.HIRegistrationForm))
            //    {
            //        Coroutine.BeginExecute(Globals.DoMessageBoxHIRegis(), null, (o, e) =>
            //        {
            //            if (Globals.msgb.Result == AxMessageBoxResult.Ok)
            //            {
            //                Globals.EventAggregator.Publish(new ItemSelected<Patient>() { Item = SelectedConsultingDiagnosy.Patient });
            //                Globals.msgb = null;
            //                Globals.HIRegistrationForm = "";
            //                TryClose();
            //            }
            //            else
            //            {
            //                TryClose();
            //            }
            //        });
            //    }
            //    else
            //    {
            //        Globals.EventAggregator.Publish(new ItemSelected<Patient>() { Item = SelectedConsultingDiagnosy.Patient });
            //        TryClose();
            //    }
            //}
            else
            {
                Globals.EventAggregator.Publish(new ConsultingDiagnosys<ConsultingDiagnosys>() { Item = SelectedConsultingDiagnosy });
                TryClose();
            }
        }
        public void gvConsultingDiagnosy_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ConsultingDiagnosys SelectedConsultingDiagnosy = e.Row.DataContext as ConsultingDiagnosys;
            if (SelectedConsultingDiagnosy != null && SelectedConsultingDiagnosy.PtRegistrationID > 0)
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }
        }
        #endregion
    }
}
