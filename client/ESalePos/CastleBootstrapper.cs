using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using Caliburn.Micro;
using Castle.Core.Logging;
using Castle.Facilities.Logging;
using Castle.Facilities.TypedFactory;
using Castle.Facilities.WcfIntegration;
using Castle.Windsor;
using aEMR.Infrastructure;
using aEMRClient.Installers;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using aEMR.ViewContracts;

namespace aEMRClient
{
    public class CastleBootstrapper : Bootstrapper<IShellViewModel>, IDisposable
    {
        private CompositionContainer _container;       
        private IWindsorContainer _windsorcontainer;

        private static ILogger _logger;

        protected override void Configure()
        {            
            try
            {
                var batch = new CompositionBatch();
                var catalog = new AggregateCatalog
                (
                    new AggregateCatalog
                    (
                        AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()
                    )
                );

                _container = new CompositionContainer(catalog);
                var eventAggregator = new EventAggregator();

                var windowManager = new WindowManager();
                var cacheManager = CacheFactory.GetCacheManager("Cache Manager");

                batch.AddExportedValue<IWindowManager>(windowManager);
                batch.AddExportedValue<IEventAggregator>(eventAggregator);
                batch.AddExportedValue<INavigationService>(new NavigationService(windowManager, eventAggregator));
                batch.AddExportedValue<ICacheManager>(cacheManager);
                batch.AddExportedValue(_windsorcontainer);
                
                batch.AddExportedValue(_container);
                batch.AddExportedValue(catalog);
                _container.Compose(batch);

            }
            catch (Exception ex)
            {
                _logger.Error("Loading component errors. ", ex);
            }            
        }

        protected override object GetInstance(Type service, string key)
        {            
            try
            {
                var contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(service) : key;

                var exports = _container.GetExportedValues<object>(contract);

                if (exports.Any())
                {
                    return exports.First();
                }
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.Error(ex.Message, ex);
                }

                MessageBox.Show(ex.Message, "aEMRClient", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            //throw new Exception("Can not locate instance of contact: " + service.AssemblyQualifiedName);
            return null;
        }


        protected override IEnumerable<object> GetAllInstances(Type service)
        {            
            return _container.GetExportedValues<object>(AttributedModelServices.GetContractName(service));
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {            
            InitContainer();
            
            IList<Assembly> asmList = new List<Assembly> { Assembly.GetExecutingAssembly() };

            try
            {
                foreach (var assemblyName in Globals.AssemblyList)
                {
                    asmList.Add(Assembly.LoadFrom(assemblyName));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                MessageBox.Show(ex.Message);
            }
            
            return asmList;
        }

        protected override void BuildUp(object instance)
        {           
            _container.SatisfyImportsOnce(instance);            
        }

        private void InitContainer()
        {
            try
            {                
                _windsorcontainer = new WindsorContainer();
                _windsorcontainer.AddFacility<WcfFacility>();
                _windsorcontainer.AddFacility<LoggingFacility>(
                        f => f.UseNLog()
                        .WithConfig(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));

                _windsorcontainer.AddFacility<TypedFactoryFacility>();
                _logger = _windsorcontainer.Resolve<ILogger>();

                _logger.InfoFormat(" Loging configuration Successfully {0} ", "Nlog");

                //_windsorcontainer.Install(new ServiceClientInstaller());
                _windsorcontainer.Install(new CommonComponentInstaller());                
            }
            catch (Exception ex)
            {                
                _logger.Error(ex.Message);
            }
        }

        public void Dispose()
        {
            _container.Dispose();
            _windsorcontainer.Dispose();
        }
    }
}
