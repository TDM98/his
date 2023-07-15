using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using PCLsProxy;
using aEMR.Controls;

namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(ISieuAmTim_Droppler)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SieuAmTim_DropplerViewModel : Conductor<object>, ISieuAmTim_Droppler

    //, IHandle<ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SieuAmTim_DropplerViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            Globals.EventAggregator.Subscribe(this);
            GetV_ValveOpen();
            if (Globals.PatientPCLReqID_Imaging > 0)
            {
                PatientPCLReqID = Globals.PatientPCLReqID_Imaging;
                //LoadInfo();
            }
            var vm = Globals.GetViewModel<IEnumListing>();
            vm.EnumType = typeof(AllLookupValues.ChoiceEnum);
            vm.AddSelectOneItem = true;
            vm.LoadData();
            Mitral_EaContent = vm;

            /*==== #001 ====*/
            EaValueArrays.Add(new V_MitralEa(0, ""));
            EaValueArrays.Add(new V_MitralEa(0, ">1"));
            EaValueArrays.Add(new V_MitralEa(1, "<1"));
            EaValueArrays.Add(new V_MitralEa(2, "=1"));
            /*==== #001 ====*/
        }

        private long _PatientPCLReqID;
        public long PatientPCLReqID
        {
            get { return _PatientPCLReqID; }
            set
            {
                if (_PatientPCLReqID != value)
                {
                    _PatientPCLReqID = value;
                    NotifyOfPropertyChange(() => PatientPCLReqID);
                }
            }
        }

        public void SetResultDetails(object resDetails)
        {
            curUltraResParams_EchoCardiography = (UltraResParams_EchoCardiography)resDetails;
        }


        #region properties

        private IEnumListing _Mitral_EaContent;
        public IEnumListing Mitral_EaContent
        {
            get { return _Mitral_EaContent; }
            set
            {
                _Mitral_EaContent = value;
                NotifyOfPropertyChange(() => Mitral_EaContent);
            }
        }

        private ObservableCollection<Lookup> _allValveOpen;
        private UltraResParams_EchoCardiography _curUltraResParams_EchoCardiography;

        public UltraResParams_EchoCardiography curUltraResParams_EchoCardiography
        {
            get { return _curUltraResParams_EchoCardiography; }
            set
            {
                if (_curUltraResParams_EchoCardiography == value)
                    return;
                _curUltraResParams_EchoCardiography = value;
                NotifyOfPropertyChange(() => curUltraResParams_EchoCardiography);
                /*==== #001 ====*/
                if (_curUltraResParams_EchoCardiography != null)
                {
                    if (curUltraResParams_EchoCardiography.V_DOPPLER_Mitral_Ea != 3)
                        SelectedEaValue = new V_MitralEa(curUltraResParams_EchoCardiography.V_DOPPLER_Mitral_Ea, curUltraResParams_EchoCardiography.DOPPLER_Mitral_Ea);
                    else if (cboEa != null)
                    {
                        curUltraResParams_EchoCardiography.V_DOPPLER_Mitral_Ea = 3;
                        cboEa.Text = curUltraResParams_EchoCardiography.DOPPLER_Mitral_Ea;
                    }
                }
                /*==== #001 ====*/
            }
        }

        public ObservableCollection<Lookup> allValveOpen
        {
            get { return _allValveOpen; }
            set
            {
                if (_allValveOpen == value)
                    return;
                _allValveOpen = value;
                NotifyOfPropertyChange(() => allValveOpen);
            }
        }

        #endregion

        #region fuction

        private ValidationSummary validationSummary { get; set; }

        public void butSave()
        {
            if (PatientPCLReqID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0301_G1_Msg_InfoChonBN);
                return;
            }

            curUltraResParams_EchoCardiography.TabIndex = 2;

            if (validationSummary.DisplayedErrors.Count > 0)
            {
                MessageBox.Show(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe);
                return;
            }

            if (curUltraResParams_EchoCardiography.UltraResParams_EchoCardiographyID > 0)
            {
                UpdateUltraResParams_EchoCardiography(curUltraResParams_EchoCardiography);
            }
            else
            {
                curUltraResParams_EchoCardiography.PCLImgResultID = PatientPCLReqID;
                curUltraResParams_EchoCardiography.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                AddUltraResParams_EchoCardiography(curUltraResParams_EchoCardiography);
            }

        }

        public void butUpdate()
        {

        }

        public void ValidationSummary_Loaded(object sender, RoutedEventArgs e)
        {
            validationSummary = sender as ValidationSummary;
        }

        #endregion

        #region method

        private void AddUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginAddUltraResParams_EchoCardiography(entity,null,null,null,
                                                                     Globals.DispatchCallback(
                                                                         (asyncResult) =>
                                                                         {
                                                                             try
                                                                             {
                                                                                 bool res =contract.EndAddUltraResParams_EchoCardiography(asyncResult);
                                                                                 if (res)
                                                                                 {
                                                                                     MessageBox.Show(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
                                                                                 }
                                                                                 else
                                                                                 {
                                                                                     MessageBox.Show("Thêm mới thất bại!");
                                                                                 }
                                                                             }
                                                                             catch (Exception ex)
                                                                             {
                                                                                 MessageBox.Show(ex.Message);
                                                                             }
                                                                             finally
                                                                             {
                                                                                 Globals.IsBusy =false;
                                                                             }
                                                                         }), null);
                }
            });

            t.Start();
        }

        private void UpdateUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginUpdateUltraResParams_EchoCardiography(entity,null,null,null,
                                                                        Globals.DispatchCallback(
                                                                            (asyncResult) =>
                                                                            {
                                                                                try
                                                                                {
                                                                                    bool res=contract.EndUpdateUltraResParams_EchoCardiography(asyncResult);
                                                                                    if (res)
                                                                                    {
                                                                                        MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail);
                                                                                    }
                                                                                }
                                                                                catch (Exception ex)
                                                                                {
                                                                                    MessageBox.Show(ex.Message);
                                                                                }
                                                                                finally
                                                                                {
                                                                                    Globals.IsBusy=false;
                                                                                }
                                                                            }), null);
                }
            });

            t.Start();
        }

        private void GetUltraResParams_EchoCardiographyByID(long UltraResParams_EchoCardiographyID, long PCLImgResultID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsClient())
                {
                    IPCLs contract = serviceFactory.ServiceInstance;
                    contract.BeginGetUltraResParams_EchoCardiography(UltraResParams_EchoCardiographyID, PCLImgResultID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var item = contract.EndGetUltraResParams_EchoCardiography(asyncResult);
                            if (item != null)
                            {
                                curUltraResParams_EchoCardiography = item;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            Globals.IsBusy =
                                false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private void GetV_ValveOpen()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllLookupValuesByType(LookupValues.V_ValveOpen
                                                           ,
                                                           Globals.DispatchCallback(
                                                               (asyncResult) =>
                                                               {
                                                                   try
                                                                   {
                                                                       IList<Lookup> results
                                                                           =
                                                                           contract.
                                                                               EndGetAllLookupValuesByType
                                                                               (asyncResult);
                                                                       if (results != null)
                                                                       {
                                                                           if (
                                                                               allValveOpen ==
                                                                               null)
                                                                           {
                                                                               allValveOpen =
                                                                                   new ObservableCollection
                                                                                       <
                                                                                           Lookup
                                                                                           >();
                                                                           }
                                                                           else
                                                                           {
                                                                               allValveOpen.
                                                                                   Clear();
                                                                           }
                                                                           foreach (
                                                                               Lookup p in
                                                                                   results)
                                                                           {
                                                                               allValveOpen.
                                                                                   Add(p);
                                                                           }
                                                                           NotifyOfPropertyChange
                                                                               (() =>
                                                                                allValveOpen);
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
                                                               }), null);
                }
            });

            t.Start();
        }

        #endregion


        public void cbxSelectionChanged(object sender,RoutedEventArgs e) 
        {
            if (curUltraResParams_EchoCardiography != null)
            {
                curUltraResParams_EchoCardiography.DOPPLER_Mitral_Ea = (string)((ComboBoxItem)((ComboBox)sender).SelectedValue).Content;                
            }
        }

        /*==== #001 ====*/
        private ObservableCollection<V_MitralEa> _EaValueArrays = new ObservableCollection<V_MitralEa>();
        public ObservableCollection<V_MitralEa> EaValueArrays
        {
            get { return _EaValueArrays; }
            set
            {
                _EaValueArrays = value;
                NotifyOfPropertyChange(() => EaValueArrays);
            }
        }
        private V_MitralEa _SelectedEaValue;
        public V_MitralEa SelectedEaValue
        {
            get { return _SelectedEaValue; }
            set
            {
                _SelectedEaValue = value;
                NotifyOfPropertyChange(() => SelectedEaValue);
                if (SelectedEaValue != null && curUltraResParams_EchoCardiography != null)
                {
                    curUltraResParams_EchoCardiography.V_DOPPLER_Mitral_Ea = SelectedEaValue.ID;
                    curUltraResParams_EchoCardiography.DOPPLER_Mitral_Ea = SelectedEaValue.Value;
                }
                else
                {
                    curUltraResParams_EchoCardiography.V_DOPPLER_Mitral_Ea = 0;
                    curUltraResParams_EchoCardiography.DOPPLER_Mitral_Ea = "";
                }
            }
        }
        AutoCompleteBox cboEa;
        public void cboEa_Loaded(object sender, RoutedEventArgs e)
        {
            cboEa = sender as AutoCompleteBox;
            if (curUltraResParams_EchoCardiography != null && curUltraResParams_EchoCardiography.V_DOPPLER_Mitral_Ea == 3)
                cboEa.Text = curUltraResParams_EchoCardiography.DOPPLER_Mitral_Ea;
        }
        public void cboEa_TextChanged(object sender, RoutedEventArgs e)
        {
            if (SelectedEaValue == null && curUltraResParams_EchoCardiography != null && cboEa != null && !string.IsNullOrEmpty(cboEa.Text))
            {
                curUltraResParams_EchoCardiography.V_DOPPLER_Mitral_Ea = 3;
                curUltraResParams_EchoCardiography.DOPPLER_Mitral_Ea = cboEa.Text;
            }
        }
        private string _SelectedEaText;
        public string SelectedEaText
        {
            get { return _SelectedEaText; }
            set
            {
                _SelectedEaText = value;
                NotifyOfPropertyChange(() => SelectedEaText);
            }
        }
        /*==== #001 ====*/
    }
}
/*==== #001 ====*/
public class V_MitralEa
{
    public V_MitralEa(int ID, string Value)
    {
        this.ID = ID;
        this.Value = Value;
    }
    public string Value { get; set; }
    public int ID { get; set; }
}
/*==== #001 ====*/
//public void Handle(DbClickSelectedObjectEvent<PatientPCLRequest> message)
// {
//     if (message != null)
//     {
//         PatientPCLReqID = message.Result.PatientPCLReqID;
//         LoadInfo();
//     }
// }

// public void Handle(ReaderInfoPatientFromPatientPCLReqEvent<PatientPCLRequest> message)
// {
//     if (message != null)
//     {
//         PatientPCLReqID = message.PCLRequest.PatientPCLReqID;
//         LoadInfo();
//     }
// }

// private void LoadInfo()
// {
//     return;

//     curUltraResParams_EchoCardiography = new UltraResParams_EchoCardiography();
//     GetUltraResParams_EchoCardiographyByID(0, PatientPCLReqID);
// }