using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using DataEntities;
using System.Linq;
using System.ComponentModel;
using eHCMSLanguage;
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.CommonTasks;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events.SL_Events;
using aEMR.Common;
using eHCMS.Services.Core;
/*
 * 20230311 #001 QTD:    Đổ lại dữ liệu quốc tịch
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IVacationPrenatalCertificate)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class VacationPrenatalCertificateViewModel : Conductor<object>, IVacationPrenatalCertificate
    {

        public void SetCurrentInformation(BirthCertificates item)
        {
            if (item != null)
            {
                CurrentBirthCertificates = item;
                BirthDate.DateTime= CurrentBirthCertificates.BirthDate;
                IsNew = CurrentBirthCertificates.BirthCertificateID == 0;
                if (IsNew = CurrentBirthCertificates.BirthCertificateID == 0)
                {
                    BirthDate.DateTime = Globals.GetCurServerDateTime();
                } 
            }
        }
        private IMinHourDateControl _BirthDate;
        public IMinHourDateControl BirthDate
        {
            get { return _BirthDate; }
            set
            {
                _BirthDate = value;
                NotifyOfPropertyChange(() => BirthDate);
            }
        }
        private BirthCertificates _CurrentBirthCertificates = new BirthCertificates();
        public BirthCertificates CurrentBirthCertificates
        {
            get
            {
                return _CurrentBirthCertificates;
            }
            set
            {
                _CurrentBirthCertificates = value;
                NotifyOfPropertyChange(() => CurrentBirthCertificates);
            }
        }

        private PatientRegistration _PtRegistration;
        public PatientRegistration PtRegistration
        {
            get
            {
                return _PtRegistration;
            }
            set
            {
                _PtRegistration = value;
                NotifyOfPropertyChange(() => _PtRegistration);
                NotifyOfPropertyChange(() => PtRegistration);
            }
        }

        private string _TitleForm="" ;
        public string TitleForm
        {
            get
            {
                return _TitleForm;
            }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }
        private bool _IsApplyAutoCode = Globals.ServerConfigSection.CommonItems.ApplyAutoCodeForCirculars56;
        public bool IsApplyAutoCode
        {
            get
            {
                return _IsApplyAutoCode;
            }
            set
            {
                _IsApplyAutoCode = value;
                NotifyOfPropertyChange(() => IsApplyAutoCode);
            }
        }
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public VacationPrenatalCertificateViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            BirthDate = Globals.GetViewModel<IMinHourDateControl>();
            BirthDate.DateTime = null;

            _eventArg = eventArg;
            TitleForm = eHCMSResources.Z3115_G1_ChungNhanNDT;
            LoadCountries();
            LoadEthnicsList();
            LoadGenders();
            //▼====: #001
            Coroutine.BeginExecute(DoNationalityListList());
            //▲====: #001
        }

        private ObservableCollection<RefCountry> _countries;
        public ObservableCollection<RefCountry> Countries
        {
            get { return _countries; }
            set
            {
                _countries = value;
                NotifyOfPropertyChange(() => Countries);
            }
        }

        private ObservableCollection<Lookup> _ethnicsList;
        public ObservableCollection<Lookup> EthnicsList
        {
            get { return _ethnicsList; }
            set
            {
                _ethnicsList = value;
                NotifyOfPropertyChange(() => EthnicsList);
            }
        }
        private ObservableCollection<Gender> _genders;
        public ObservableCollection<Gender> Genders
        {
            get { return _genders; }
            set
            {
                _genders = value;
                NotifyOfPropertyChange(() => Genders);
            }
        }
        public void LoadEthnicsList()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.ETHNIC,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    EthnicsList = allItems != null ? new ObservableCollection<Lookup>(allItems) : null;
                                }
                                catch (Exception ex1)
                                {
                                    ClientLoggerHelper.LogInfo(ex1.ToString());
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
        public void LoadGenders()
        {
            if (Globals.allGenders != null)
            {
                Genders = Globals.allGenders.ToObservableCollection();
                return;
            }

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllGenders(
                            Globals.DispatchCallback(asyncResult =>
                            {
                                IList<Gender> allItems = null;
                                try
                                {
                                    allItems = contract.EndGetAllGenders(asyncResult);
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    MessageBox.Show(eHCMSResources.A0692_G1_Msg_InfoKhTheLayDSGioiTinh);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                                if (allItems != null)
                                {
                                    Globals.allGenders = allItems.ToList();
                                    Genders = Globals.allGenders.ToObservableCollection();
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
        public void LoadCountries()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllCountries(Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var allItems = contract.EndGetAllCountries(asyncResult);

                                Countries = allItems != null ? new ObservableCollection<RefCountry>(allItems) : null;
                            }
                            catch (Exception ex1)
                            {
                                ClientLoggerHelper.LogInfo(ex1.ToString());
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


        public void InitValueBeforeSave(bool IsDelete)
        {
            if (CurrentBirthCertificates.PtRegistrationID == 0)
            {
                CurrentBirthCertificates.PtRegistrationID = PtRegistration.PtRegistrationID;
                CurrentBirthCertificates.V_RegistrationType = (long)PtRegistration.V_RegistrationType;
            }
            CurrentBirthCertificates.BirthDate = BirthDate.DateTime.GetValueOrDefault(DateTime.MinValue);
            CurrentBirthCertificates.IsDelete = IsDelete;
            if(PtRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                TitleForm = eHCMSResources.Z3115_G1_ChungNhanTTNgT;
            }
            else
            {
                TitleForm = eHCMSResources.Z3115_G1_ChungNhanTTNT;
            }
        }

        public bool CheckValidBeforeSave()
        {
            return true;
        }
        private void SaveBirthCertificates(bool IsDelete)
        {
            if (CheckValidBeforeSave() == false)
            {
                return;
            }
            InitValueBeforeSave(IsDelete);
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSaveBirthCertificates(CurrentBirthCertificates, (long)Globals.LoggedUserAccount.StaffID, Globals.GetCurServerDateTime().ToString("ddMMyyyy"), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool result = contract.EndSaveBirthCertificates(asyncResult);
                            if (result)
                            {
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                            }
                            IsNew = CurrentBirthCertificates.BirthCertificateID == 0;
                            this.HideBusyIndicator();
                            if (IsDelete && result)
                            {
                                MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
                                TryClose();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            this.HideBusyIndicator();
                        }

                    }), null);
                }
            });
            t.Start();
        }
       
        public void SaveBirthCertificatesCmd()
        {
            SaveBirthCertificates(false);
        }

        public void DeleteBirthCertificatesCmd()
        {
            if (MessageBox.Show(eHCMSResources.K0483_G1_BanCoChacChanMuonXoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SaveBirthCertificates(true);
            }
        }

        public void PrintBirthCertificatesCmd()
        {
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.ID = (long)CurrentBirthCertificates.BirthCertificateID;
                proAlloc.eItem = ReportName.XRpt_GiayChungSinh;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _eventArg.Unsubscribe(this);
        }

        private bool _IsNew = true;
        public bool IsNew
        {
            get
            {
                return _IsNew;
            }
            set
            {
                if (_IsNew == value) return;
                _IsNew = value;
                NotifyOfPropertyChange(() => IsNew);
            }
        }

        //▼====: #001
        private ObservableCollection<RefNationality> _Nationalities;
        public ObservableCollection<RefNationality> Nationalities
        {
            get { return _Nationalities; }
            set
            {
                _Nationalities = value;
                NotifyOfPropertyChange(() => Nationalities);
            }
        }

        private IEnumerator<IResult> DoNationalityListList()
        {
            if (Globals.allNationalities != null)
            {
                Nationalities = Globals.allNationalities.ToObservableCollection();
                yield break;
            }
            var paymentTask = new LoadNationalityListTask(false, false);
            yield return paymentTask;
            Nationalities = paymentTask.RefNationalityList;
            yield break;
        }
        //▲====: #001
    }
}