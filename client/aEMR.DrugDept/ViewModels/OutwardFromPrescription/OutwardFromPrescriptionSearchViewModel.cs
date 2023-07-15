using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using eHCMSLanguage;
using aEMR.Common;
using Castle.Windsor;

namespace eHCMS.DrugDept.ViewModels
{
    [Export(typeof(IOutwardFromPrescriptionSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OutwardFromPrescriptionSearchViewModel : Conductor<object>, IOutwardFromPrescriptionSearch
    {
        #region Indicator Member

        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }
        #endregion

        [ImportingConstructor]
        public OutwardFromPrescriptionSearchViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            SearchCriteria = new PrescriptionSearchCriteria();

            PrescriptionList = new PagedSortableCollectionView<Prescription>();
            PrescriptionList.OnRefresh += PrescriptionList_OnRefresh;
            PrescriptionList.PageSize = 20;
        }

        void PrescriptionList_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchPrescription(PrescriptionList.PageIndex, PrescriptionList.PageSize);
        }

        protected override void OnDeactivate(bool close)
        {
            //base.OnDeactivate(close);
            PrescriptionList.Clear();
        }
        #region Properties Member

        private PrescriptionSearchCriteria _SearchCriteria;
        public PrescriptionSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                if (_SearchCriteria != value)
                {
                    _SearchCriteria = value;
                }
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private PagedSortableCollectionView<Prescription> _prescriptions;
        public PagedSortableCollectionView<Prescription> PrescriptionList
        {
            get
            {
                return _prescriptions;
            }
            set
            {
                if (_prescriptions != value)
                {
                    _prescriptions = value;
                }
                NotifyOfPropertyChange(() => PrescriptionList);
            }
        }

        #endregion

        public void btnSearchPrescription(object sender, RoutedEventArgs e)
        {
            PrescriptionList.PageIndex = 0;
            SearchPrescription(PrescriptionList.PageIndex, PrescriptionList.PageSize);
        }
        private void SearchPrescription(int PageIndex, int PageSize)
        {
            int Total = 0;
            IsLoading = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSearchPrescription_InPt(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndSearchPrescription_InPt(out Total, asyncResult);
                            if (results != null)
                            {

                                PrescriptionList.Clear();
                                PrescriptionList.TotalItemCount = Total;
                                foreach (Prescription p in results)
                                {
                                    PrescriptionList.Add(p);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void dataGrid1_DblClick(object sender, EventArgs<object> e)
        {
            TryClose();

            Globals.EventAggregator.Publish(new MedDeptCloseSearchPrescriptionEvent { SelectedPrescription = e.Value });

        }
        public void Search_KeyUp_Pre(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    try
                    {
                        SearchCriteria.PrescriptID = Convert.ToInt64((sender as TextBox).Text);
                    }
                    catch
                    {
                        SearchCriteria.PrescriptID = null;
                    }
                }
                PrescriptionList.PageIndex = 0;
                SearchPrescription(PrescriptionList.PageIndex, PrescriptionList.PageSize);
            }
        }

        public void Search_KeyUp_HICardNo(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.HICardCode = (sender as TextBox).Text;
                }
                PrescriptionList.PageIndex = 0;
                SearchPrescription(PrescriptionList.PageIndex, PrescriptionList.PageSize);
            }
        }

        public void Search_KeyUp_PatientCode(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.PatientCode = (sender as TextBox).Text;
                }
                PrescriptionList.PageIndex = 0;
                SearchPrescription(PrescriptionList.PageIndex, PrescriptionList.PageSize);
            }
        }

        public void Search_KeyUp_FullName(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.FullName = (sender as TextBox).Text;
                }
                PrescriptionList.PageIndex = 0;
                SearchPrescription(PrescriptionList.PageIndex, PrescriptionList.PageSize);
            }
        }
    }
}
