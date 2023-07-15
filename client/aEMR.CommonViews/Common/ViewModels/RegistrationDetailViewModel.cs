using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using DataEntities;
using aEMR.Common;
using System.Linq;
using aEMR.Common.BaseModel;
using System.Windows;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IRegistrationDetail)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RegistrationDetailViewModel : ViewModelBase, IRegistrationDetail
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]

        public RegistrationDetailViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }

        private PatientRegistrationDetail _registrationDetail;

        public PatientRegistrationDetail RegistrationDetail
        {
            get { return _registrationDetail; }
            set
            {
                _registrationDetail = value;
                NotifyOfPropertyChange(() => RegistrationDetail);

                if (_registrationDetail == null)
                {
                    DeptLocations = null;
                }
                else
                {
                    SelectedDeptLocation = _registrationDetail.DeptLocation;

                    LoadLocations(_registrationDetail);
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
                NotifyOfPropertyChange(() => SelectedDeptLocation);
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

        public override bool IsProcessing
        {
            get
            {
                return _isLoadingLocations;
            }
        }
        public override string StatusText
        {
            get
            {
                if (_isLoadingLocations)
                {
                    return eHCMSResources.Z0605_G1_DangLayDSPK;
                }
                return "";
            }
        }

        private bool _isLoadingLocations;
        public bool IsLoadingLocations
        {
            get { return _isLoadingLocations; }
            set
            {
                _isLoadingLocations = value;
                NotifyOfPropertyChange(() => IsLoadingLocations);
            }
        }

        private int _ServiceSeqNum;
        public int ServiceSeqNum
        {
            get { return _ServiceSeqNum; }
            set
            {
                _ServiceSeqNum = value;
                NotifyOfPropertyChange(() => ServiceSeqNum);
            }
        }

        private bool _FromAppointment=true;
        public bool FromAppointment
        {
            get { return _FromAppointment; }
            set
            {
                _FromAppointment = value;
                NotifyOfPropertyChange(() => FromAppointment);
            }
        }

        public void LoadLocations(PatientRegistrationDetail registrationDetail)
        {
            if (DeptLocations != null)
            {
                DeptLocations.Clear();
            }
            if (registrationDetail == null)
            {
                return;
            }
            var medServiceID = registrationDetail.RefMedicalServiceItem != null ?
                registrationDetail.RefMedicalServiceItem.MedServiceID : registrationDetail.MedServiceID.GetValueOrDefault(0);
            var t = new Thread(() =>
            {
                IsLoadingLocations = true;
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetLocationsByServiceID(medServiceID,
                        Globals.DispatchCallback(asyncResult =>
                        {
                            IList<DeptLocation> allItem = null;
                            try
                            {
                                allItem = contract.EndGetLocationsByServiceID(asyncResult);

                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }

                            if (allItem != null && allItem.Count > 0)
                            {
                                //DeptLocations = new ObservableCollection<DeptLocation>(allItem.Where(c => CheckDeptLocValid(c.DeptLocationID)));
                                //KMx: Bỏ hàm CheckDeptLocValid(). Vì trường hợp: Quầy đăng ký không gán phòng được khi giờ đăng ký (giờ hiện tại) ngoài giờ "Chỉ tiêu phòng khám".
                                DeptLocations = new ObservableCollection<DeptLocation>(allItem);
                            }
                            else
                            {
                                DeptLocations = new ObservableCollection<DeptLocation>();
                            }

                        }), null);
                }
            });
            t.Start();
        }

        //KMx: Bỏ hàm CheckDeptLocValid(). Vì trường hợp: Quầy đăng ký không gán phòng được khi giờ đăng ký (giờ hiện tại) ngoài giờ "Chỉ tiêu phòng khám".
        // Hàm này không còn sử dụng nữa.
        //bool CheckDeptLocValid(long DeptLocationID)
        //{
        //    foreach (var crt in Globals.ConsultationRoomTarget)
        //    {
        //        if (crt.DeptLocationID == DeptLocationID)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public void CancelCmd()
        {
            TryClose();
        }

        public void OkCmd()
        {
            try
            {
                if (ServiceSeqNum < 1)
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z1090_G1_STTLonHon0));
                    return;
                }
                if (SelectedDeptLocation == null
                    || SelectedDeptLocation.DeptLocationID < 1)
                {
                    MessageBox.Show(eHCMSResources.A0101_G1_Msg_InfoChuaChonPg);
                    return;
                }

                if (SelectedDeptLocation != _registrationDetail.DeptLocation)
                {                    
                    _registrationDetail.DeptLocation = SelectedDeptLocation;                    
                }
                _registrationDetail.FromAppointment = true;
                if (ServiceSeqNum > 0)
                {
                    RegistrationDetail.ServiceSeqNum = ServiceSeqNum;
                }
                if (_registrationDetail.RecordState == RecordState.UNCHANGED)
                {
                    _registrationDetail.RecordState = RecordState.MODIFIED;
                }

                //KMx: Flag để nhận biết dịch vụ được gán STT bằng tay (10/11/2014 14:12).
                _registrationDetail.IsSetSeqNumManually = true;

                Globals.EventAggregator.Publish(new StateChanged<PatientRegistrationDetail> { Item = _registrationDetail });
                TryClose();
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
                MessageBox.Show(eHCMSResources.A0983_G1_Msg_InfoSTTKHHopLe);
            }
        }
    }
}
