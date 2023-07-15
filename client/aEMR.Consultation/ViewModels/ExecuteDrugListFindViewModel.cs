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
using DataEntities.MedicalInstruction;
using DevExpress.ReportServer.Printing;
using aEMR.ReportModel.ReportModels;
/*
* 20210427 #001 TNHX: 
*/
namespace aEMR.Consultation.Views
{
    [Export(typeof(IExecuteDrugListFind)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ExecuteDrugListFindViewModel : ViewModelBase, IExecuteDrugListFind
        , IHandle<ShowPatientInfo_KHAMBENH_TONGQUAT<Patient, PatientRegistration, PatientRegistrationDetail>>
        , IHandle<SaveExecuteDrugCompleted>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ExecuteDrugListFindViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
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
            TicketCareList = new ObservableCollection<TicketCare>();
            if (CurrentPatientRegistration != null)
            {
                //LoadTiketCareListByPtID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
                LoadExecuteDrugListByPtID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
            }
            else if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                //LoadTiketCareListByPtID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
                LoadExecuteDrugListByPtID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
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
        private ObservableCollection<ExecuteDrug> _ExecuteDrugList;
        public ObservableCollection<ExecuteDrug> ExecuteDrugList
        {
            get
            {
                return _ExecuteDrugList;
            }
            set
            {
                if (_ExecuteDrugList == value)
                    return;
                _ExecuteDrugList = value;
                NotifyOfPropertyChange(() => ExecuteDrugList);
            }
        }

        private ObservableCollection<TicketCare> _TicketCareList;
        public ObservableCollection<TicketCare> TicketCareList
        {
            get
            {
                return _TicketCareList;
            }
            set
            {
                if (_TicketCareList == value)
                    return;
                _TicketCareList = value;
                NotifyOfPropertyChange(() => TicketCareList);
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
        #endregion

        #region Ticket Care method
        //private void LoadTiketCareListByPtID(long PtRegistrationID)
        //{
        //    this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new CommonService_V2Client())
        //            {
        //                var contract = serviceFactory.ServiceInstance;
        //                contract.BeginGetTicketCareListForRegistration(PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        TicketCareList = contract.EndGetTicketCareListForRegistration(asyncResult).ToObservableCollection();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                    }
        //                    finally
        //                    {
        //                        this.HideBusyIndicator();
        //                    }
        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //            this.HideBusyIndicator();
        //        }
        //    });

        //    t.Start();
        //}

        //public void AddTicketCareCmd()
        //{
        //    if (Registration_DataStorage.CurrentPatientRegistration == null)
        //    {
        //        return;
        //    }
        //    ITicketCare mDialogView = Globals.GetViewModel<ITicketCare>();
        //    mDialogView.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
        //    if (Registration_DataStorage.CurrentPatientRegistration.InPatientInstruction != null)
        //    {
        //        mDialogView.V_LevelCare = Registration_DataStorage.CurrentPatientRegistration.InPatientInstruction.V_LevelCare.LookupID;
        //        mDialogView.IntPtDiagDrInstructionID = Registration_DataStorage.CurrentPatientRegistration.InPatientInstruction.IntPtDiagDrInstructionID;
        //    }
        //    mDialogView.GetTicketCare(0);
        //    GlobalsNAV.ShowDialog_V3(mDialogView);
        //}

        //public void PreviewCmd()
        //{
        //    if (Registration_DataStorage.CurrentPatientRegistration == null)
        //    {
        //        return;
        //    }
        //    void onInitDlg(ICommonPreviewView proAlloc)
        //    {
        //        proAlloc.eItem = ReportName.XRpt_PhieuChamSoc;
        //        proAlloc.RegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
        //    }
        //    GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        //}
        #endregion
        #region Execute Drugs method
        private void LoadExecuteDrugListByPtID(long PtRegistrationID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetExecuteDrugListForRegistration(PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                ExecuteDrugList = contract.EndGetExecuteDrugListForRegistration(asyncResult).ToObservableCollection();
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

        public void RemoveItemCmd(object source, object eventArgs)
        {
            var sender = source as Button;
            if (sender != null)
            {
                var ctx = sender.DataContext as ExecuteDrugDetail;

                if (ctx == null || ctx.ExecuteDrugDetailID == 0)
                {
                    return;
                }
                this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new CommonService_V2Client())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginDeleteExecuteDrug(ctx.ExecuteDrugDetailID
                                , Globals.LoggedUserAccount.StaffID.Value
                                , Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        bool results = contract.EndDeleteExecuteDrug(asyncResult);
                                        if (results)
                                        {
                                            GlobalsNAV.ShowMessagePopup(eHCMSResources.K0537_G1_XoaOk);
                                        }
                                        else
                                        {
                                            GlobalsNAV.ShowMessagePopup(eHCMSResources.K0484_G1_XoaFail);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }
                                    finally
                                    {
                                        this.HideBusyIndicator();
                                    }
                                }),
                            null);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        this.HideBusyIndicator();
                    }
                });

                t.Start();
            }
        }

        public void AddItemCmd(object source, object eventArgs)
        {
            var sender = source as Button;
            if (sender != null)
            {
                var ctx = sender.DataContext as ExecuteDrug;

                if (ctx == null)
                {
                    return;
                }
                IExecuteDrug mDialogView = Globals.GetViewModel<IExecuteDrug>();
                mDialogView.ExecuteDrugID = ctx.ExecuteDrugID;
                
                GlobalsNAV.ShowDialog_V3(mDialogView);
            }
        }

        public void EditItemCmd(object source, object eventArgs)
        {
            var sender = source as Button;
            if (sender != null)
            {
                var ctx = sender.DataContext as ExecuteDrugDetail;

                if (ctx == null || ctx.ExecuteDrugDetailID == 0)
                {
                    return;
                }
                IExecuteDrug mDialogView = Globals.GetViewModel<IExecuteDrug>();
                mDialogView.ExecuteDrugID = ctx.ExecuteDrugID;
                mDialogView.ExecuteDrugDetailID = ctx.ExecuteDrugDetailID;
                mDialogView.StaffID = ctx.StaffID;
                
                GlobalsNAV.ShowDialog_V3(mDialogView);
            }
        }

        public void PreviewExecuteDrugCmd()
        {
            if (Registration_DataStorage.CurrentPatientRegistration == null)
            {
                return;
            }
            void onInitDlg(ICommonPreviewView proAlloc)
            {
                proAlloc.eItem = ReportName.XRpt_PhieuThucHienYLenhThuoc;
                proAlloc.RegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
            }
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }
        #endregion
        public void Handle(SaveExecuteDrugCompleted message)
        {
            if (CurrentPatientRegistration != null)
            {
                LoadExecuteDrugListByPtID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
            }
            else if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                LoadExecuteDrugListByPtID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
            }
        }

        //private RemoteDocumentSource _reportModel;
        //public RemoteDocumentSource ReportModel
        //{
        //    get { return _reportModel; }
        //    set
        //    {
        //        _reportModel = value;
        //        NotifyOfPropertyChange(() => ReportModel);
        //    }
        //}
        //private void GetReport(long PtRegistrationID)
        //{
        //    DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
        //    ReportModel = null;
        //    ReportModel = new GenericReportModel("eHCMS.ReportLib.InPatient.Reports.XRptVitalSign_InPt").PreviewModel;
        //    rParams["PtRegistrationID"].Value = PtRegistrationID;
        //    // ReportModel.AutoShowParametersPanel = false;
        //    ReportModel.CreateDocument(rParams);
        //}
        //public void PreviewVitalCmd()
        //{
        //    if (CurrentPatientRegistration != null)
        //    {
        //        GetReport(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
        //    }
        //    else if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
        //    {
        //        GetReport(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
        //    }
        //}
    }
}
