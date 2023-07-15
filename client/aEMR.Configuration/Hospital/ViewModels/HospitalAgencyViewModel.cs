using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ViewContracts;

namespace aEMR.Configuration.Hospitals.ViewModels
{
    [Export(typeof(IHospitalAgency)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class HospitalAgencyViewModel : Conductor<object>, IHospitalAgency        
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public HospitalAgencyViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            HospitalList = new PagedSortableCollectionView<DataEntities.Hospital>();
            HospitalList.OnRefresh += new EventHandler<RefreshEventArgs>(HospitalList_OnRefresh);
            LoadHospitalsNew(HospitalList.PageIndex, HospitalList.PageSize, true);
            GetTestingAgency_All();
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private string _HosName;
        public string HosName
        {
            get { return _HosName; }
            set
            {
                if (_HosName != value)
                {
                    _HosName = value;
                    NotifyOfPropertyChange(() => HosName);
                }
            }
        }

        private object _leftContent;
        public object leftContent
        {
            get
            {
                return _leftContent;
            }
            set
            {
                if (_leftContent == value)
                    return;
                _leftContent = value;
                NotifyOfPropertyChange(() => leftContent);
            }
        }

        private DataEntities.Hospital _selectedHospital;
        public DataEntities.Hospital SelectedHospital
        {
            get { return _selectedHospital; }
            set
            {
                _selectedHospital = value;
                NotifyOfPropertyChange(() => SelectedHospital);                
            }
        }

        private TestingAgency _selectedTestingAgency;
        public TestingAgency selectedTestingAgency
        {
            get { return _selectedTestingAgency; }
            set
            {
                _selectedTestingAgency = value;
                NotifyOfPropertyChange(() => selectedTestingAgency);
            }
        }

        private PagedSortableCollectionView<DataEntities.Hospital> _hospitalList;
        public PagedSortableCollectionView<DataEntities.Hospital> HospitalList
        {
            get { return _hospitalList; }
            //TTM 14072018
            //Xaml Binding thi viewmodel ko de private set dc
            //private set
            //{
            //    _hospitalList = value;
            //    NotifyOfPropertyChange(() => HospitalList);
            //}
            set
            {
                _hospitalList = value;
                NotifyOfPropertyChange(() => HospitalList);
            }
        }



        void HospitalList_OnRefresh(object sender, RefreshEventArgs e)
        {
            LoadHospitalsNew(HospitalList.PageIndex, HospitalList.PageSize, true);
        }

        public void btnSearch_KeyUp(object sender, KeyEventArgs e) 
        {
            if(e.Key==Key.Enter)
            {
                btnSearch();
            }
        }

        public void btnSearch() 
        {
            HospitalList.PageIndex = 0;
            LoadHospitalsNew(HospitalList.PageIndex, HospitalList.PageSize, true);
        }

        public void btAddChoose() 
        {
            if(SelectedHospital!=null)
            {
                ObjTestingAgencyList.Add(new TestingAgency { HosID=SelectedHospital.HosID
                    ,AgencyName=SelectedHospital.HosName
                });
            }
        }

        public void DoubleClick() 
        {
            if (SelectedHospital != null)
            {
                ObjTestingAgencyList.Add(new TestingAgency
                {
                    HosID = SelectedHospital.HosID
                    ,
                    AgencyName = SelectedHospital.HosName
                });
            }
        }

        public void butRestore() 
        {
            ObjTestingAgencyList.Reset();
            NotifyOfPropertyChange(() => ObjTestingAgencyList);
        }
        public void hplDelete_Click()
        {
            if (MessageBox.Show(eHCMSResources.Z0554_G1_CoChacMuonXoaDongNayKg, "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ObjTestingAgencyList.Remove(selectedTestingAgency);
            }
        }

        public void butSave() 
        {
            if (ObjTestingAgencyList.DeleteObject.Count > 0) 
            {
                TestingAgency_DeleteXML(ObjTestingAgencyList.DeleteObject.ToList());
            }

            if (ObjTestingAgencyList.NewObject.Count > 0)
            {
                TestingAgency_InsertXML(ObjTestingAgencyList.NewObject.ToList());
            }
        }

        #region Thông tin bệnh viện ngoài
        private ObjectEdit<TestingAgency> _ObjTestingAgencyList;
        public ObjectEdit<TestingAgency> ObjTestingAgencyList
        {
            get
            {
                return _ObjTestingAgencyList;
            }
            set
            {
                if (_ObjTestingAgencyList != value)
                {
                    _ObjTestingAgencyList = value;
                    NotifyOfPropertyChange(() => ObjTestingAgencyList);
                }
            }
        }
        private void GetTestingAgency_All()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsImportClient())
                {
                    //ObjTestingAgencyList = new DataEntities.ObjectEdit<TestingAgency>();
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetTestingAgency_All(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetTestingAgency_All(asyncResult);
                            //if (items != null)
                            {
                                ObjTestingAgencyList = new DataEntities.ObjectEdit<TestingAgency>(items.ToObservableCollection(),"HosID","","");
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        #endregion

        private void LoadHospitalsNew(int pageIndex, int pageSize, bool bCountTotal)
        {
            HospitalSearchCriteria hosSearchCri = new HospitalSearchCriteria();
            var t = new Thread(() =>
            {
                hosSearchCri.HosName = HosName;
                IsLoading = true;
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchHospitalsNew(hosSearchCri, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<DataEntities.Hospital> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSearchHospitalsNew(out totalCount, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                            }

                            HospitalList.Clear();
                            if (bOK)
                            {
                                if (bCountTotal)
                                {
                                    HospitalList.TotalItemCount = totalCount;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        HospitalList.Add(item);
                                    }
                                }
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    IsLoading = false;
                    //Globals.IsBusy = false;
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }

        private void TestingAgency_DeleteXML(List<TestingAgency> Agencys)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsImportClient())
                {
                    //ObjTestingAgencyList = new DataEntities.ObjectEdit<TestingAgency>();
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginTestingAgency_DeleteXML(Agencys,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndTestingAgency_DeleteXML(asyncResult);
                            //if (items != null)
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk,"");
                            GetTestingAgency_All();
                        }
                    }), null);
                }


            });
            t.Start();
        }

        private void TestingAgency_InsertXML(List<TestingAgency> Agencys)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsImportClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginTestingAgency_InsertXML(Agencys, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndTestingAgency_InsertXML(asyncResult);                            
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.ShowMessage(eHCMSResources.A1027_G1_Msg_InfoThemOK, "");
                            GetTestingAgency_All();
                        }
                    }), null);
                }


            });
            t.Start();
        }
    }



}
