using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Common;
using DataEntities;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPCLRequestDetail)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLRequestDetailViewModel : ViewModelBase, IPCLRequestDetail
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PCLRequestDetailViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }

        private PatientPCLRequestDetail _pclRequestDetail;

        public PatientPCLRequestDetail PCLRequestDetail
        {
            get { return _pclRequestDetail; }
            set 
            {
                _pclRequestDetail = value; 
                NotifyOfPropertyChange(() => PCLRequestDetail);

                if(_pclRequestDetail == null)
                {
                    DeptLocations = null;
                }
                else
                {
                    SelectedDeptLocation = _pclRequestDetail.DeptLocation;

                    if(_pclRequestDetail.PCLExamType.PCLExamTypeLocations == null
                        ||_pclRequestDetail.PCLExamType.PCLExamTypeLocations.Count == 0)
                    {
                        LoadPclExamTypeLocations(_pclRequestDetail.PCLExamType);
                    }
                    else
                    {
                        DeptLocations = _pclRequestDetail.PCLExamType.PCLExamTypeLocations.Select(item => item.DeptLocation).ToObservableCollection();
                    }
                }
            }
        }

        private DeptLocation _selectedDeptLocation;
        public DeptLocation SelectedDeptLocation
        {
            get { return _selectedDeptLocation; }
            set
            {
                _selectedDeptLocation = value;
                NotifyOfPropertyChange(()=>SelectedDeptLocation);
            }
        }

        private ObservableCollection<DeptLocation> _deptLocations;

        public ObservableCollection<DeptLocation> DeptLocations
        {
            get { return _deptLocations; }
            set
            {
                _deptLocations = value;
                NotifyOfPropertyChange(() => DeptLocations);
            }
        }

        private PCLExamTypeLocation _selectedExamTypeLocation;
        public PCLExamTypeLocation SelectedExamTypeLocation
        {
            get
            { return _selectedExamTypeLocation; }
            set
            {
                _selectedExamTypeLocation = value;
                NotifyOfPropertyChange(() => SelectedExamTypeLocation);
            }
        }

        public override bool IsProcessing
        {
            get
            {
                return _isLoadingPclExamTypeLocations;
            }
        }
        public override string StatusText
        {
            get
            {
                if (_isLoadingPclExamTypeLocations)
                {
                    return eHCMSResources.Z0605_G1_DangLayDSPK;
                }
                return "";
            }
        }

        private bool _isLoadingPclExamTypeLocations;

        public bool IsLoadingPclExamTypeLocations
        {
            get { return _isLoadingPclExamTypeLocations; }
            set { _isLoadingPclExamTypeLocations = value; 
            NotifyOfPropertyChange(() => IsLoadingPclExamTypeLocations);
            }
        }

        public void LoadPclExamTypeLocations(PCLExamType examType)
        {
            if (DeptLocations != null)
            {
                DeptLocations.Clear();
            }
            if (examType == null)
            {
                return;
            }
            var t = new Thread(() =>
            {
                IsLoadingPclExamTypeLocations = true;
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetDeptLocationsByExamType(examType.PCLExamTypeID, 
                        Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var allItems = contract.EndGetDeptLocationsByExamType(asyncResult);
                                if (allItems != null)
                                {
                                    DeptLocations = allItems.ToObservableCollection();
                                }
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                IsLoadingPclExamTypeLocations = false;
                            }
                        }), null);
                }
            });
            t.Start();
        }

        public void CancelCmd()
        {
            TryClose();
        }

        public void OkCmd()
        {
            if(SelectedDeptLocation != _pclRequestDetail.DeptLocation)
            {
                _pclRequestDetail.DeptLocation = SelectedDeptLocation;
                if(_pclRequestDetail.RecordState == RecordState.UNCHANGED)
                {
                    _pclRequestDetail.RecordState = RecordState.MODIFIED;
                }
                if(_pclRequestDetail.PatientPCLRequest != null && _pclRequestDetail.PatientPCLRequest.RecordState == RecordState.UNCHANGED)
                {
                    _pclRequestDetail.PatientPCLRequest.RecordState = RecordState.MODIFIED;

                    Globals.EventAggregator.Publish(new StateChanged<PatientPCLRequest>{Item = _pclRequestDetail.PatientPCLRequest});
                }
            }
            TryClose();
        }
    }
}
