using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof (ISieuAmTT_General)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTT_GeneralViewModel : Conductor<object>, ISieuAmTT_General
    {
        public UltraResParams_FetalEchocardiography ObjUltraResParams_FetalEchocardiography
        {
            get { return _ObjUltraResParams_FetalEchocardiography; }
            set
            {
                if (_ObjUltraResParams_FetalEchocardiography == value)
                    return;
                _ObjUltraResParams_FetalEchocardiography = value;
                NotifyOfPropertyChange(() => ObjUltraResParams_FetalEchocardiography);
            }
        }
        UltraResParams_FetalEchocardiography _ObjUltraResParams_FetalEchocardiography;
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTT_GeneralViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);
            GetV_EchographyPosture();
            GetV_MomMedHis();
        }
        public void SetResultDetails(object resDetails)
        {
            ObjUltraResParams_FetalEchocardiography = (UltraResParams_FetalEchocardiography)resDetails;
        }
        private ObservableCollection<Lookup> _AllEchographyPosture;
        public ObservableCollection<Lookup> AllEchographyPosture
        {
            get { return _AllEchographyPosture; }
            set
            {
                if (_AllEchographyPosture == value)
                    return;
                _AllEchographyPosture = value;
                NotifyOfPropertyChange(() => AllEchographyPosture);
            }
        }
        private void GetV_EchographyPosture()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllLookupValuesByType(LookupValues.V_EchographyPosture, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            IList<Lookup> results = contract.EndGetAllLookupValuesByType(asyncResult);
                            if (results != null)
                            {
                                if (AllEchographyPosture == null)
                                {
                                    AllEchographyPosture = new ObservableCollection<Lookup>();
                                }
                                else
                                {
                                    AllEchographyPosture.Clear();
                                }
                                foreach (Lookup p in results)
                                {
                                    AllEchographyPosture.Add(p);
                                }
                                NotifyOfPropertyChange(() => AllEchographyPosture);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }),
                    null);
                }
            });
            t.Start();
        }
        private ObservableCollection<Lookup> _AllMomMedHis;
        public ObservableCollection<Lookup> AllMomMedHis
        {
            get { return _AllMomMedHis; }
            set
            {
                if (_AllMomMedHis == value)
                    return;
                _AllMomMedHis = value;
                NotifyOfPropertyChange(() => AllMomMedHis);
            }
        }
        private void GetV_MomMedHis()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllLookupValuesByType(LookupValues.V_MomMedHis, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            IList<Lookup> results = contract.EndGetAllLookupValuesByType(asyncResult);
                            if (results != null)
                            {
                                if (AllMomMedHis == null)
                                {
                                    AllMomMedHis = new ObservableCollection<Lookup>();
                                }
                                else
                                {
                                    AllMomMedHis.Clear();
                                }
                                foreach (Lookup p in results)
                                {
                                    AllMomMedHis.Add(p);
                                }
                                NotifyOfPropertyChange(() => AllMomMedHis);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }),
                    null);
                }
            });
            t.Start();
        }
    }
}