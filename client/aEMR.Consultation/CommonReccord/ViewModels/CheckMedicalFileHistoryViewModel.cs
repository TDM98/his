using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Controls;
using eHCMSLanguage;
using System.Linq;
using aEMR.Common.BaseModel;
using aEMR.Common;
using System.Windows.Media;
/*
* 20210427 #001 TNHX: 
* 20220406 #002 TNHX: 1115 - Chỉnh quy trình gửi hồ sơ cho KHTH chờ kiểm duyệt
* 20220711 #003 DatTB: Đổi màu trạng thái khi gửi lần 2.
* 20220730 #004 DatTB: Đổi vị trí 2 else if (Hồ sơ gửi lần 2 vẫn đổi màu khi DLS xử lí)
*/
namespace aEMR.ConsultantEPrescription.CommonRecs.ViewModels
{
    [Export(typeof(ICheckMedicalFileHistory)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CheckMedicalFileHistoryViewModel : ViewModelBase, ICheckMedicalFileHistory
        , IHandle<ShowPatientInfo_KHAMBENH_TONGQUAT<Patient, PatientRegistration, PatientRegistrationDetail>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public CheckMedicalFileHistoryViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);
            authorization();
        }

        public void Handle(ShowPatientInfo_KHAMBENH_TONGQUAT<Patient, PatientRegistration, PatientRegistrationDetail> message)
        {
            InitPatientInfo();
        }

        public void InitPatientInfo(PatientRegistration CurrentPatientRegistration = null)
        {
            CheckMedicalFileHistoryList = new ObservableCollection<CheckMedicalFiles>();
            if (CurrentPatientRegistration != null)
            {
                LoadCheckMedicalFilesByPtID(CurrentPatientRegistration.PtRegistrationID, (long)CurrentPatientRegistration.V_RegistrationType);
            }
            else if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                LoadCheckMedicalFilesByPtID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID, (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            InitPatientInfo();
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }

        #region property
        private ObservableCollection<CheckMedicalFiles> _CheckMedicalFileHistoryList;
        public ObservableCollection<CheckMedicalFiles> CheckMedicalFileHistoryList
        {
            get
            {
                return _CheckMedicalFileHistoryList;
            }
            set
            {
                if (_CheckMedicalFileHistoryList == value)
                    return;
                _CheckMedicalFileHistoryList = value;
                NotifyOfPropertyChange(() => CheckMedicalFileHistoryList);
            }
        }

        private long _V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
        public long V_RegistrationType
        {
            get { return _V_RegistrationType; }
            set
            {
                if (_V_RegistrationType != value)
                {
                    _V_RegistrationType = value;
                    NotifyOfPropertyChange(() => V_RegistrationType);
                }
            }
        }
        #endregion

        public void grdCMFHistory_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            CheckMedicalFiles item = e.Row.DataContext as CheckMedicalFiles;
            if (item == null)
            {
                return;
            }
            if (item.V_CheckMedicalFilesStatus == (long)AllLookupValues.V_CheckMedicalFilesStatus.Tra_Ho_So)
            {
                e.Row.Background = new SolidColorBrush(Colors.Red);
            }
            else if (item.V_CheckMedicalFilesStatus == (long)AllLookupValues.V_CheckMedicalFilesStatus.Da_Duyet)
            {
                //▼==== #003
                e.Row.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#0D69FF");
                e.Row.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#0D69FF");
                e.Row.Foreground = new SolidColorBrush(Colors.White);
                //▲==== #003
            }
            //▼==== #004
            else if (item.IsDLSChecked || item.DLSReject)
            {
                e.Row.Background = new SolidColorBrush(Colors.PaleGreen);
            }
            else if (item.V_CheckMedicalFilesStatus == (long)AllLookupValues.V_CheckMedicalFilesStatus.Cho_Duyet_Lai)
            {
                e.Row.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FFA200"); // #003
            }
            //▲==== #004
        }

        #region method
        private void LoadCheckMedicalFilesByPtID(long PtRegistrationID, long V_RegistrationType)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetListCheckMedicalFilesByPtID(PtRegistrationID, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CheckMedicalFileHistoryList = contract.EndGetListCheckMedicalFilesByPtID(asyncResult).ToObservableCollection();
                                //▼====: #002
                                if (Registration_DataStorage != null && CheckMedicalFileHistoryList != null && CheckMedicalFileHistoryList.Count() > 0)
                                {
                                    ObservableCollection<CheckMedicalFiles> tempCheckMedicalFileHistoryList = CheckMedicalFileHistoryList.DeepCopy();
                                    CheckMedicalFiles RecentCheckMedicalFiles = tempCheckMedicalFileHistoryList.OrderBy(x => x.CheckMedicalFileID).LastOrDefault();
                                    if (RecentCheckMedicalFiles != null
                                        && (RecentCheckMedicalFiles.V_CheckMedicalFilesStatus != (long)AllLookupValues.V_CheckMedicalFilesStatus.Tra_Ho_So))
                                    {
                                        Registration_DataStorage.CurrentPatientRegistration.CheckMedicalFiles = RecentCheckMedicalFiles;
                                    }
                                }
                                //▲====: #002
                                //--▼-- 20220329 DatTB: Lấy thêm thông tin lịch sử kiểm duyệt hồ sơ
                                //if (CheckMedicalFileHistoryList.FirstOrDefault() != null)
                                //{
                                //    if (CheckMedicalFileHistoryList.FirstOrDefault().DLSReject == true || CheckMedicalFileHistoryList.FirstOrDefault().V_CheckMedicalFilesStatus == 85602)
                                //    {
                                //        CheckMedicalFileHistoryList.FirstOrDefault().CreatedStaff = new Staff();
                                //        CheckMedicalFileHistoryList.FirstOrDefault().KHTH_Staff = new Staff();
                                //    }
                                //}
                                //--▲-- 20220329 DatTB: Lấy thêm thông tin lịch sử kiểm duyệt hồ sơ
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
        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
            }
        }

        private PatientRegistration _CurrentPatientRegistration;
        public PatientRegistration CurrentPatientRegistration
        {
            get
            {
                return _CurrentPatientRegistration;
            }
            set
            {
                if (_CurrentPatientRegistration == value)
                {
                    return;
                }
                _CurrentPatientRegistration = value;
                NotifyOfPropertyChange(() => CurrentPatientRegistration);
            }
        }
    }
}
