using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Common.HotKeyManagement;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMS.CommonUserControls.CommonTasks;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Castle.Core.Logging;
using aEMR.Controls;

namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(ICreateOutPtTransactionFinalization)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CreateOutPtTransactionFinalizationViewModel : ViewModelBase, ICreateOutPtTransactionFinalization
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public CreateOutPtTransactionFinalizationViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            base.HasInputBindingCmd = true;
            Coroutine.BeginExecute(LoadDepartments());
            RegistrationTypeCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_RegistrationType && (x.LookupID == (long)AllLookupValues.RegistrationType.NGOAI_TRU || x.LookupID == (long)AllLookupValues.RegistrationType.NOI_TRU)).ToObservableCollection();
            if (RegistrationTypeCollection != null && RegistrationTypeCollection.Count > 0)
            {
                SearchCriteria.V_RegistrationType = RegistrationTypeCollection.First().LookupID;
            }
            _logger = container.Resolve<ILogger>();
            FromDateDateTime = Globals.GetViewModel<IMinHourDateControl>();
            FromDateDateTime.DateTime = _SearchCriteria.FromDate;
            ToDateDateTime = Globals.GetViewModel<IMinHourDateControl>();
            ToDateDateTime.DateTime = _SearchCriteria.ToDate;
            GetLookupEInvoiceStatus();
        }
        #region Properties
        private DateTime _CurrentDateTime = Globals.GetCurServerDateTime();
        public DateTime CurrentDateTime
        {
            get => _CurrentDateTime; set
            {
                _CurrentDateTime = value;
                NotifyOfPropertyChange(() => CurrentDateTime);
            }
        }
        private ObservableCollection<RefDepartment> _RefDepartmentCollection;
        private ObservableCollection<DeptLocation> _LocationCollection;
        private SeachPtRegistrationCriteria _SearchCriteria = new SeachPtRegistrationCriteria { FromDate = Globals.GetCurServerDateTime(), ToDate = Globals.GetCurServerDateTime() };
        private ObservableCollection<PatientRegistration_V2> _PatientRegistrationCollection;
        private ObservableCollection<PatientRegistration_V2> _FinalizedRegistrationCollection;
        private string _OutputErrorMessage;
        private bool _AllChecked;
        private bool _AllCheckedTIeInvoices;
        private bool _IsAllHIReportID = false;
        private ObservableCollection<OutPtTransactionFinalization> _TransactionFinalizationCollection;
        private ComboBox cboRegStatus { get; set; }
        private TabControl gMainTabControl { get; set; }
        private ObservableCollection<Lookup> _RegistrationTypeCollection;
        public ObservableCollection<RefDepartment> RefDepartmentCollection
        {
            get => _RefDepartmentCollection; set
            {
                _RefDepartmentCollection = value;
                NotifyOfPropertyChange(() => RefDepartmentCollection);
            }
        }
        public ObservableCollection<DeptLocation> LocationCollection
        {
            get => _LocationCollection; set
            {
                _LocationCollection = value;
                NotifyOfPropertyChange(() => LocationCollection);
            }
        }
        public SeachPtRegistrationCriteria SearchCriteria
        {
            get => _SearchCriteria; set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }
        public ObservableCollection<PatientRegistration_V2> PatientRegistrationCollection
        {
            get => _PatientRegistrationCollection; set
            {
                _PatientRegistrationCollection = value;
                NotifyOfPropertyChange(() => PatientRegistrationCollection);
            }
        }
        public ObservableCollection<PatientRegistration_V2> FinalizedRegistrationCollection
        {
            get => _FinalizedRegistrationCollection; set
            {
                _FinalizedRegistrationCollection = value;
                NotifyOfPropertyChange(() => FinalizedRegistrationCollection);
            }
        }
        public string OutputErrorMessage
        {
            get => _OutputErrorMessage; set
            {
                _OutputErrorMessage = value;
                NotifyOfPropertyChange(() => OutputErrorMessage);
            }
        }
        public bool AllChecked
        {
            get => _AllChecked; set
            {
                _AllChecked = value;
                NotifyOfPropertyChange(() => AllChecked);
                if (gMainTabControl != null && gMainTabControl.SelectedItem is TabItem && (gMainTabControl.SelectedItem as TabItem).Name.Equals("TIRegistrations") && PatientRegistrationCollection != null && PatientRegistrationCollection.Count > 0)
                {
                    foreach (var item in PatientRegistrationCollection)
                    {
                        item.IsSelected = AllChecked;
                    }
                }
            }
        }
        public bool AllCheckedTIeInvoices
        {
            get => _AllCheckedTIeInvoices; set
            {
                _AllCheckedTIeInvoices = value;
                NotifyOfPropertyChange(() => _AllCheckedTIeInvoices);
                if (gMainTabControl != null && gMainTabControl.SelectedItem is TabItem && (gMainTabControl.SelectedItem as TabItem).Name.Equals("TIeInvoices") && FinalizedRegistrationCollection != null && FinalizedRegistrationCollection.Count > 0)
                {
                    foreach (var item in FinalizedRegistrationCollection)
                    {
                        item.IsSelected = _AllCheckedTIeInvoices;
                    }
                }
            }
        }
        private PatientRegistration_V2 _SelectedPatientRegistration;
        public PatientRegistration_V2 SelectedPatientRegistration
        {
            get => _SelectedPatientRegistration; set
            {
                _SelectedPatientRegistration = value;
                NotifyOfPropertyChange(() => SelectedPatientRegistration);
            }
        }
        private DataGrid gvRegistrations { get; set; }
        public bool IsAllHIReportID
        {
            get => _IsAllHIReportID; set
            {
                _IsAllHIReportID = value;
                NotifyOfPropertyChange(() => IsAllHIReportID);
            }
        }
        public ObservableCollection<OutPtTransactionFinalization> TransactionFinalizationCollection
        {
            get => _TransactionFinalizationCollection; set
            {
                _TransactionFinalizationCollection = value;
                NotifyOfPropertyChange(() => TransactionFinalizationCollection);
            }
        }
        private bool _IsExportEInvoiceView = false;
        public bool IsExportEInvoiceView
        {
            get => _IsExportEInvoiceView; set
            {
                _IsExportEInvoiceView = value;
                NotifyOfPropertyChange(() => IsExportEInvoiceView);
            }
        }
        private string _TitleForm = Globals.TitleForm;
        public string TitleForm
        {
            get => _TitleForm; set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }
        public ObservableCollection<Lookup> RegistrationTypeCollection
        {
            get
            {
                return _RegistrationTypeCollection;
            }
            set
            {
                _RegistrationTypeCollection = value;
                NotifyOfPropertyChange(() => RegistrationTypeCollection);
            }
        }
        #endregion
        #region Methods
        private IEnumerator<IResult> LoadDepartments()
        {
            ObservableCollection<RefDepartment> tempDepartments = new ObservableCollection<RefDepartment>();
            var departmentTask = new LoadDepartmentsByV_DeptTypeOperationTask(new List<long> { (long)V_DeptTypeOperation.KhoaNgoaiTru, (long)V_DeptTypeOperation.KhoaNoi });
            yield return departmentTask;
            RefDepartmentCollection = departmentTask.Departments.Where(x => x.V_DeptTypeOperation != (long)V_DeptTypeOperation.KhoaNgoaiTru || x.DeptID == Globals.ServerConfigSection.Hospitals.KhoaPhongKham).ToObservableCollection();
            if (RefDepartmentCollection == null) RefDepartmentCollection = new ObservableCollection<RefDepartment>();
            RefDepartmentCollection.Insert(0, new RefDepartment { DeptID = 0, DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1997_G1_ChonKhoa) });
            SearchCriteria.DeptID = RefDepartmentCollection.FirstOrDefault().DeptID;
            cboDepartments_SelectionChanged(null, null);
            yield break;
        }
        private IEnumerator<IResult> LoadLocations(long deptId)
        {
            if (deptId > 0)
            {
                var deptLoc = new LoadDeptLoctionByIDTask(deptId);
                yield return deptLoc;
                if (deptLoc.DeptLocations != null)
                {
                    LocationCollection = new ObservableCollection<DeptLocation>(deptLoc.DeptLocations);
                }
                else
                {
                    LocationCollection = new ObservableCollection<DeptLocation>();
                }
            }
            else
            {
                LocationCollection = new ObservableCollection<DeptLocation>();
            }
            LocationCollection.Insert(0, new DeptLocation { DeptLocationID = 0, Location = new Location { LID = 0, LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2094_G1_ChonPg) } });
            SearchCriteria.DeptLocationID = LocationCollection.FirstOrDefault().DeptLocationID;
            yield break;
        }
        private void FocusSelectedView()
        {
            if (PatientRegistrationCollection != null && PatientRegistrationCollection.Count > 1 && SearchCriteria != null && !string.IsNullOrWhiteSpace(SearchCriteria.PatientCode))
            {
                SelectedPatientRegistration = PatientRegistrationCollection.Skip(PatientRegistrationCollection.IndexOf(SelectedPatientRegistration)).FirstOrDefault(x => x.Patient != null && !string.IsNullOrEmpty(x.Patient.PatientCode) && x.Patient.PatientCode.ToLower() == SearchCriteria.PatientCode.ToLower());
                if (SelectedPatientRegistration == null)
                {
                    SelectedPatientRegistration = PatientRegistrationCollection.FirstOrDefault(x => x.Patient != null && !string.IsNullOrEmpty(x.Patient.PatientCode) && x.Patient.PatientCode.ToLower() == SearchCriteria.PatientCode.ToLower());
                }
                if (SelectedPatientRegistration != null)
                {
                    gvRegistrations.ScrollIntoView(SelectedPatientRegistration);
                }
            }
        }
        private void GetTransactionFinalizationSummaryInfos()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetTransactionFinalizationSummaryInfos(SearchCriteria,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    TransactionFinalizationCollection = contract.EndGetTransactionFinalizationSummaryInfos(asyncResult).ToObservableCollection();
                                    if (PatientRegistrationCollection != null && PatientRegistrationCollection.Any(x => x.HIReportID > 0))
                                    {
                                        IsAllHIReportID = true;
                                    }
                                    else
                                    {
                                        IsAllHIReportID = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void SearchRegistrationsForCreateOutPtTransactionFinalization()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchRegistrationsForCreateOutPtTransactionFinalization(SearchCriteria,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    PatientRegistrationCollection = contract.EndSearchRegistrationsForCreateOutPtTransactionFinalization(asyncResult).ToObservableCollection();
                                    if (PatientRegistrationCollection != null && PatientRegistrationCollection.Any(x => x.HIReportID > 0))
                                    {
                                        IsAllHIReportID = true;
                                    }
                                    else
                                    {
                                        IsAllHIReportID = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void SearchRegistrationsForCreateEInvoices(bool aIsHasInvoice = false)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        var mSearchCriteria = SearchCriteria.DeepCopy();
                        mSearchCriteria.IsHasInvoice = aIsHasInvoice;
                        contract.BeginSearchRegistrationsForCreateEInvoices(mSearchCriteria,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    if (aIsHasInvoice)
                                    {
                                        var mFinalizedRegistrationCollection = contract.EndSearchRegistrationsForCreateEInvoices(asyncResult).ToObservableCollection();
                                        TransactionFinalizationCollection = new ObservableCollection<OutPtTransactionFinalization>();
                                        if (mFinalizedRegistrationCollection != null)
                                        {
                                            TransactionFinalizationCollection = mFinalizedRegistrationCollection.Select(x => x.PtTranFinalization).ToObservableCollection();
                                        }
                                    }
                                    else
                                    {
                                        FinalizedRegistrationCollection = contract.EndSearchRegistrationsForCreateEInvoices(asyncResult).ToObservableCollection();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private IEnumerator<IResult> AddOrUpdateTransactionFinalization_Routine(List<PatientRegistration_V2> aFinalizationCollection, bool aIsUpdate)
        {
            ILoggerDialog mLogView = Globals.GetViewModel<ILoggerDialog>();
            var mThread = new Thread(() =>
            {
                GlobalsNAV.ShowDialog_V4(mLogView, null, null, false, true);
            });
            mThread.Start();
            //this.ShowBusyIndicator();
            foreach (var mTransactionFinalization in aFinalizationCollection)
            {
                //OutPtTransactionFinalization mTransactionFinalization = new OutPtTransactionFinalization
                //{
                //    TranFinalizationID = aRegistration.TranFinalizationID,
                //    TaxMemberName = aRegistration.Patient.FullName,
                //    TaxMemberAddress = aRegistration.Patient.PatientStreetAddress,
                //    StaffID = Globals.LoggedUserAccount.Staff.StaffID,
                //    PtRegistrationID = aRegistration.PtRegistrationID,
                //    V_PaymentMode = aTransactionFinalizationObj.V_PaymentMode,
                //    V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU,
                //    Symbol = aTransactionFinalizationObj.Symbol,
                //    Denominator = aTransactionFinalizationObj.Denominator,
                //    DateInvoice = aTransactionFinalizationObj.DateInvoice
                //};
                //if (mTransactionFinalization.TranFinalizationID == 0)
                //{
                //    var AddOrUpdateTransactionFinalizationTask = new GenericCoRoutineTask(AddOrUpdateTransactionFinalization, mTransactionFinalization, false);
                //    yield return AddOrUpdateTransactionFinalizationTask;
                //    long OutTranFinalizationID = (AddOrUpdateTransactionFinalizationTask.GetResultObj(0) as long?).GetValueOrDefault(0);
                //    if (OutTranFinalizationID <= 0)
                //    {
                //        this.HideBusyIndicator();
                //        btnSearch();
                //        yield break;
                //    }
                //    mTransactionFinalization.TranFinalizationID = OutTranFinalizationID;
                //}
                yield return GenericCoRoutineTask.StartTask(CheckEInvoiceCustomerAndImportNew, mTransactionFinalization.Patient, mTransactionFinalization.PtTranFinalization, mLogView);
                yield return GenericCoRoutineTask.StartTask(GetRptOutPtTransactionFinalizationDetails, new VNPTCustomer(mTransactionFinalization.Patient), mTransactionFinalization.PtTranFinalization, mLogView, aIsUpdate);
            }
            //this.HideBusyIndicator();
            btnSearch();
            mLogView.IsFinished = true;
        }
        private void AddOrUpdateTransactionFinalization(GenericCoRoutineTask aGenTask, object aTransactionFinalizationObj, object aIsUpdateToken, object aLogView)
        {
            OutPtTransactionFinalization TransactionFinalizationObj = aTransactionFinalizationObj as OutPtTransactionFinalization;
            ILoggerDialog mLogView = aLogView as ILoggerDialog;
            bool IsUpdateToken = (aIsUpdateToken as bool?) == true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddOutPtTransactionFinalization(TransactionFinalizationObj, IsUpdateToken, 0, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long TransactionFinalizationSummaryInfoID = 0;
                                long OutTranFinalizationID = 0;
                                if (!contract.EndAddOutPtTransactionFinalization(out TransactionFinalizationSummaryInfoID, out OutTranFinalizationID, asyncResult))
                                {
                                    //MessageBox.Show(eHCMSResources.A0991_G1_Msg_ErrorSystem, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    //aGenTask.ActionComplete(false);
                                    //this.HideBusyIndicator();
                                    mLogView.AppendLogMessage(eHCMSResources.A0991_G1_Msg_ErrorSystem);
                                    aGenTask.Error = new Exception(eHCMSResources.A0991_G1_Msg_ErrorSystem);
                                    aGenTask.ActionComplete(true);
                                    //_logger.Error(ex.Message);
                                }
                                else
                                {
                                    TransactionFinalizationObj.TransactionFinalizationSummaryInfoID = TransactionFinalizationSummaryInfoID;
                                }
                                aGenTask.AddResultObj(OutTranFinalizationID);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            finally
                            {
                                aGenTask.ActionComplete(true);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    aGenTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void CheckEInvoiceCustomerAndImportNew(GenericCoRoutineTask aGenTask, object aPatient, object aTransactionFinalizationObj, object aLogView)
        {
            Patient mPatient = aPatient as Patient;
            OutPtTransactionFinalization TransactionFinalizationObj = aTransactionFinalizationObj as OutPtTransactionFinalization;
            ILoggerDialog mLogView = aLogView as ILoggerDialog;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new VNPTAccountingPortalServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    mLogView.AppendLogMessage(string.Format("{0}: {1}", eHCMSResources.K1270_G1_BN, mPatient.PatientCode));
                    contract.BegingetCus(mPatient.PatientCode
                        , Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserName, Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserPass
                        , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string jCustomerInfo = contract.EndgetCus(asyncResult);
                            if (VNPTAccountingPortalServiceClient.ErrorCodeDetails.ContainsKey(jCustomerInfo) && jCustomerInfo.Equals("ERR:3"))
                            {
                                mLogView.AppendLogMessage(string.Format("{0}: {1}", eHCMSResources.Z2651_G1_TienHanhThemMoiBN, mPatient.PatientCode));
                                CommonGlobals.ImportPatientEInvoice(aGenTask, aPatient, TransactionFinalizationObj, mLogView);
                            }
                            else if (jCustomerInfo.StartsWith("ERR"))
                            {
                                mLogView.AppendLogMessage(eHCMSResources.T0074_G1_I + ": " + jCustomerInfo);
                                //aGenTask.ActionComplete(true);
                                aGenTask.Error = new Exception(eHCMSResources.T0074_G1_I + ": " + jCustomerInfo);
                                aGenTask.ActionComplete(true);
                            }
                            else
                            {
                                aGenTask.ActionComplete(true);
                            }
                        }
                        catch (Exception ex)
                        {
                            //Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            mLogView.AppendLogMessage(ex.Message);
                            aGenTask.Error = ex;
                            aGenTask.ActionComplete(true);
                            //aGenTask.ActionComplete(true);
                            //this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private void GetRptOutPtTransactionFinalizationDetails(GenericCoRoutineTask aGenTask, object aCustomer, object aTransactionFinalizationObj, object aLogView, object aIsUpdate)
        {
            VNPTCustomer mCustomer = aCustomer as VNPTCustomer;
            OutPtTransactionFinalization TransactionFinalizationObj = aTransactionFinalizationObj as OutPtTransactionFinalization;
            ILoggerDialog mLogView = aLogView as ILoggerDialog;
            bool mIsUpdate = (aIsUpdate as bool?) == true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRptOutPtTransactionFinalizationDetails(TransactionFinalizationObj.PtRegistrationID, TransactionFinalizationObj.V_RegistrationType,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    List<RptOutPtTransactionFinalizationDetail> mRptOutPtTransactionFinalizationDetailCollection = contract.EndGetRptOutPtTransactionFinalizationDetails(asyncResult);
                                    if (mRptOutPtTransactionFinalizationDetailCollection != null && mRptOutPtTransactionFinalizationDetailCollection.Count > 0)
                                    {
                                        if (mIsUpdate)
                                        {
                                            UpdateEInvoice(aGenTask, mCustomer, TransactionFinalizationObj, mRptOutPtTransactionFinalizationDetailCollection, mLogView);
                                        }
                                        else
                                        {
                                            AddEInvoice(aGenTask, mCustomer, TransactionFinalizationObj, mRptOutPtTransactionFinalizationDetailCollection, mLogView);
                                        }
                                    }
                                    else
                                    {
                                        //MessageBox.Show(eHCMSResources.Z2439_G1_DuLieuDauVaoKhongDungQD, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        //aGenTask.ActionComplete(false);
                                        //this.HideBusyIndicator();
                                        mLogView.AppendLogMessage(eHCMSResources.Z2439_G1_DuLieuDauVaoKhongDungQD);
                                        aGenTask.Error = new Exception(eHCMSResources.Z2439_G1_DuLieuDauVaoKhongDungQD);
                                        aGenTask.ActionComplete(true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    //aGenTask.ActionComplete(false);
                                    //this.HideBusyIndicator();
                                    mLogView.AppendLogMessage(ex.Message);
                                    aGenTask.Error = new Exception(ex.Message);
                                    aGenTask.ActionComplete(true);
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    aGenTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void AddEInvoice(GenericCoRoutineTask aGenTask, object aCustomer, object aTransactionFinalizationObj, object aRptOutPtTransactionFinalizationDetailCollection, object aLogView)
        {
            VNPTCustomer mCustomer = aCustomer as VNPTCustomer;
            OutPtTransactionFinalization TransactionFinalizationObj = aTransactionFinalizationObj as OutPtTransactionFinalization;
            List<RptOutPtTransactionFinalizationDetail> mRptOutPtTransactionFinalizationDetailCollection = aRptOutPtTransactionFinalizationDetailCollection as List<RptOutPtTransactionFinalizationDetail>;
            ILoggerDialog mLogView = aLogView as ILoggerDialog;
            string aPattern = TransactionFinalizationObj.Denominator;
            string aSerial = TransactionFinalizationObj.Symbol;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new VNPTAccountingPublishServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    string xmlInvData = new VNPTInvoice().ToXML(mCustomer, TransactionFinalizationObj, mRptOutPtTransactionFinalizationDetailCollection);
                    contract.BeginImportAndPublishInv(Globals.ServerConfigSection.CommonItems.eInvoiceAccountUserName, Globals.ServerConfigSection.CommonItems.eInvoiceAccountUserPass
                        , xmlInvData, Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserName, Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserPass
                        , aPattern, aSerial, 0, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string JResult = contract.EndImportAndPublishInv(asyncResult);
                            if (VNPTAccountingPublishServiceClient.ImportAndPublishInvErrorCodeDetails.ContainsKey(JResult))
                            {
                                mLogView.AppendLogMessage(string.Format("{0}-{1}", JResult, VNPTAccountingPublishServiceClient.ImportAndPublishInvErrorCodeDetails[JResult]));
                                aGenTask.Error = new Exception(VNPTAccountingPublishServiceClient.ImportAndPublishInvErrorCodeDetails[JResult]);
                                aGenTask.ActionComplete(true);
                            }
                            else if (JResult.StartsWith("ERR"))
                            {
                                mLogView.AppendLogMessage(JResult);
                                aGenTask.Error = new Exception(JResult);
                                aGenTask.ActionComplete(true);
                            }
                            if (JResult.StartsWith("OK:"))
                            {
                                mLogView.AppendLogMessage(string.Format("{0}: {1}", eHCMSResources.Z2651_G1_ThemMoiHoaDonThanhCong, JResult));
                                string mResultToken = string.Format("OK:{0};{1}-{2}_", TransactionFinalizationObj.Denominator, TransactionFinalizationObj.Symbol, TransactionFinalizationObj.InvoiceKey);
                                if (JResult.StartsWith(mResultToken) && !string.IsNullOrEmpty(JResult.Replace(mResultToken, "")))
                                {
                                    TransactionFinalizationObj.InvoiceNumb = JResult.Replace(mResultToken, "").PadLeft(Globals.ServerConfigSection.CommonItems.MaxEInvoicePaternLength, '0');
                                }
                                TransactionFinalizationObj.eInvoiceToken = JResult;
                                AddOrUpdateTransactionFinalization(aGenTask, TransactionFinalizationObj, true, mLogView);
                            }
                        }
                        catch (Exception ex)
                        {
                            mLogView.AppendLogMessage(ex.Message);
                            aGenTask.Error = new Exception(ex.Message);
                            aGenTask.ActionComplete(true);
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private void UpdateEInvoice(GenericCoRoutineTask aGenTask, object aCustomer, object aTransactionFinalizationObj, object aRptOutPtTransactionFinalizationDetailCollection, object aLogView)
        {
            VNPTCustomer mCustomer = aCustomer as VNPTCustomer;
            OutPtTransactionFinalization TransactionFinalizationObj = aTransactionFinalizationObj as OutPtTransactionFinalization;
            List<RptOutPtTransactionFinalizationDetail> mRptOutPtTransactionFinalizationDetailCollection = aRptOutPtTransactionFinalizationDetailCollection as List<RptOutPtTransactionFinalizationDetail>;
            ILoggerDialog mLogView = aLogView as ILoggerDialog;
            string aPattern = TransactionFinalizationObj.Denominator;
            string aSerial = TransactionFinalizationObj.Symbol;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new VNPTAccountingBusinessServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginAdjustInvoiceAction(Globals.ServerConfigSection.CommonItems.eInvoiceAccountUserName, Globals.ServerConfigSection.CommonItems.eInvoiceAccountUserPass
                        , new VNPTInvoice().ToXML(mCustomer, TransactionFinalizationObj, mRptOutPtTransactionFinalizationDetailCollection, VNPTUpdateType.AdjInfo)
                        , Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserName, Globals.ServerConfigSection.CommonItems.eInvoiceAdminUserPass
                        , (TransactionFinalizationObj.V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU ? "1-" : "3-") + TransactionFinalizationObj.PtRegistrationID.ToString()
                        , null
                        , 0
                        , aPattern, aSerial, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string JResult = contract.EndAdjustInvoiceAction(asyncResult);
                            if (VNPTAccountingPublishServiceClient.ImportAndPublishInvErrorCodeDetails.ContainsKey(JResult))
                            {
                                //GlobalsNAV.ShowMessagePopup(string.Format("{0}-{1}", JResult, VNPTAccountingPublishServiceClient.ImportAndPublishInvErrorCodeDetails[JResult]));
                                //aGenTask.ActionComplete(false);
                                //this.HideBusyIndicator();
                                mLogView.AppendLogMessage(string.Format("{0}-{1}", JResult, VNPTAccountingPublishServiceClient.ImportAndPublishInvErrorCodeDetails[JResult]));
                                aGenTask.Error = new Exception(VNPTAccountingPublishServiceClient.ImportAndPublishInvErrorCodeDetails[JResult]);
                                aGenTask.ActionComplete(true);
                            }
                            else if (JResult.StartsWith("ERR"))
                            {
                                //GlobalsNAV.ShowMessagePopup(JResult);
                                //aGenTask.ActionComplete(false);
                                //this.HideBusyIndicator();
                                mLogView.AppendLogMessage(JResult);
                                aGenTask.Error = new Exception(JResult);
                                aGenTask.ActionComplete(true);
                            }
                            if (JResult.StartsWith("OK:"))
                            {
                                mLogView.AppendLogMessage(string.Format("{0}: {1}", eHCMSResources.Z2651_G1_ThemMoiHoaDonThanhCong, JResult));
                                JResult = JResult.Replace("OK:", "").Replace(string.Format("-{0}_", (TransactionFinalizationObj.V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU ? "1-" : "3-") + TransactionFinalizationObj.PtRegistrationID.ToString()), ";");
                                TransactionFinalizationObj.eInvoiceToken = JResult;
                                AddOrUpdateTransactionFinalization(aGenTask, TransactionFinalizationObj, true, mLogView);
                            }
                        }
                        catch (Exception ex)
                        {
                            //Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            //aGenTask.ActionComplete(false);
                            //this.HideBusyIndicator();
                            mLogView.AppendLogMessage(ex.Message);
                            aGenTask.Error = new Exception(ex.Message);
                            aGenTask.ActionComplete(true);
                        }
                    }), null);
                }
            });
            t.Start();
        }
        #endregion
        #region Events
        public void gvRegistrations_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            gvRegistrations = sender as DataGrid;
        }
        public void cboDepartments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Coroutine.BeginExecute(LoadLocations(SearchCriteria.DeptID.GetValueOrDefault(0)));
        }
        public void btnSearch()
        {
            SearchCriteria.FromDate = FromDateDateTime.DateTime;
            SearchCriteria.ToDate = ToDateDateTime.DateTime;
            if (gMainTabControl != null && gMainTabControl.SelectedItem is TabItem && (gMainTabControl.SelectedItem as TabItem).Name.Equals("TIFinalizations") && IsExportEInvoiceView)
            {
                SearchRegistrationsForCreateEInvoices(true);
            }
            else if (gMainTabControl != null && gMainTabControl.SelectedItem is TabItem && (gMainTabControl.SelectedItem as TabItem).Name.Equals("TIFinalizations"))
            {
                GetTransactionFinalizationSummaryInfos();
            }
            else if (gMainTabControl != null && gMainTabControl.SelectedItem is TabItem && (gMainTabControl.SelectedItem as TabItem).Name.Equals("TIeInvoices"))
            {
                SearchRegistrationsForCreateEInvoices();
            }
            else
            {
                if (cboRegStatus != null)
                {
                    SearchCriteria.ViewCase = Convert.ToByte(cboRegStatus.SelectedIndex + 1);
                }
                SearchRegistrationsForCreateOutPtTransactionFinalization();
            }
        }
        public void btnConfirm()
        {
            if (PatientRegistrationCollection == null || !PatientRegistrationCollection.Any(x => x.IsSelected)) return;
            IEditOutPtTransactionFinalization aViewEditOutPtTransactionFinalization = Globals.GetViewModel<IEditOutPtTransactionFinalization>();
            if (IsExportEInvoiceView)
            {
                aViewEditOutPtTransactionFinalization.ViewCase = 2;
            }
            aViewEditOutPtTransactionFinalization.TransactionFinalizationObj = new OutPtTransactionFinalization
            {
                TaxMemberName = Globals.ServerConfigSection.CommonItems.ReportHospitalName,
                TaxMemberAddress = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress,
                StaffID = Globals.LoggedUserAccount.Staff.StaffID,
                CreatedStaff = Globals.LoggedUserAccount.Staff,
                PtRegistrationIDCollection = PatientRegistrationCollection.Where(x => x.IsSelected).Select(x => x.PtRegistrationID).ToList(),
                V_PaymentMode = (long)AllLookupValues.PaymentMode.TIEN_MAT,
                V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU
            };
            GlobalsNAV.ShowDialog_V3<IEditOutPtTransactionFinalization>(aViewEditOutPtTransactionFinalization);
            if (aViewEditOutPtTransactionFinalization.TransactionFinalizationObj.TransactionFinalizationSummaryInfoID > 0 || IsExportEInvoiceView)
            {
                btnSearch();
            }
        }

        public void btnPrint12Template(object source)
        {
            if (source == null)
            {
                return;
            }
            if(source is PatientRegistration_V2)
            {
                PatientRegistration_V2 mPatientRegistration = source as PatientRegistration_V2;
                Action<IReportDocumentPreview> onInitDlg = delegate (IReportDocumentPreview proAlloc)
                {
                    proAlloc.ID = mPatientRegistration.PtRegistrationID;
                    // Ngoại trú thì áp trực tiếp
                    //if (Globals.ServerConfigSection.CommonItems.ApplyTemp12Version6556)
                    //{
                    //    proAlloc.eItem = ReportName.TEMP12_6556;
                    //}
                    //else
                    //{
                    //proAlloc.eItem = ReportName.TEMP12;
                    //}
                    proAlloc.eItem = ReportName.TEMP12;
                    if (Globals.ServerConfigSection.CommonItems.ShowLoginNameOnReport38 && Globals.LoggedUserAccount.Staff != null)
                    {
                        proAlloc.StaffFullName = Globals.LoggedUserAccount.Staff.FullName;
                    }
                    else
                    {
                        proAlloc.StaffFullName = "";
                    }
                    proAlloc.V_RegistrationType = (long)mPatientRegistration.V_RegistrationType;
                };
                GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg);
            }
            else if (source is OutPtTransactionFinalization)
            {
                if((source as OutPtTransactionFinalization).PtRegistrationID == 0)
                {
                    return;
                }
                OutPtTransactionFinalization mPatientRegistration = source as OutPtTransactionFinalization;
                Action<IReportDocumentPreview> onInitDlg = delegate (IReportDocumentPreview proAlloc)
                {
                    proAlloc.ID = mPatientRegistration.PtRegistrationID;
                    // Ngoại trú thì áp trực tiếp
                    //if (Globals.ServerConfigSection.CommonItems.ApplyTemp12Version6556)
                    //{
                    //    proAlloc.eItem = ReportName.TEMP12_6556;
                    //}
                    //else
                    //{
                    //proAlloc.eItem = ReportName.TEMP12;
                    //}
                    proAlloc.eItem = ReportName.TEMP12;
                    if (Globals.ServerConfigSection.CommonItems.ShowLoginNameOnReport38 && Globals.LoggedUserAccount.Staff != null)
                    {
                        proAlloc.StaffFullName = Globals.LoggedUserAccount.Staff.FullName;
                    }
                    else
                    {
                        proAlloc.StaffFullName = "";
                    }
                    proAlloc.V_RegistrationType = (long)mPatientRegistration.V_RegistrationType;
                };
                GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg);
            }
        }

        public void btnPrintFinalization(object source)
        {
            if (source == null || !(source is OutPtTransactionFinalization) || (source as OutPtTransactionFinalization).PtRegistrationID == 0)
            {
                return;
            }
            OutPtTransactionFinalization TransactionFinalizationObj = source as OutPtTransactionFinalization;
            GlobalsNAV.ShowDialog<ICommonPreviewView>((ICommonPreviewView aView) =>
            {
                aView.RegistrationID = TransactionFinalizationObj.PtRegistrationID;
                aView.eItem = ReportName.RptOutPtTransactionFinalization;
                aView.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
                aView.ID = 0;
                aView.PrimaryID = TransactionFinalizationObj.TransactionFinalizationSummaryInfoID;
            });
        }

        public void hbtnExportEInvoice(object source)
        {
            if (source == null || !(source is OutPtTransactionFinalization))
            {
                return;
            }
            OutPtTransactionFinalization TransactionFinalizationObj = source as OutPtTransactionFinalization;
            if (TransactionFinalizationObj == null)
            {
                return;
            }
            string mFileName = string.Format("{0}.pdf", TransactionFinalizationObj.InvoiceKey);
            string mFilePath = Path.Combine(Path.GetTempPath(), mFileName);
            CommonGlobals.ExportInvoiceToPdfNoPay(TransactionFinalizationObj.InvoiceKey, mFilePath);
        }

        public void TabControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            gMainTabControl = sender as TabControl;
        }

        private void ExportEInvoice(bool aIsUpdate = false)
        {
            if (FinalizedRegistrationCollection == null || !FinalizedRegistrationCollection.Any(x => x.IsSelected)) return;
            if (FinalizedRegistrationCollection.Any(x => x.IsSelected && x.TranFinalizationID == 0))
            {
                GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2640_G1_KhongPHHDChoDKChuaQT);
                return;
            }
            if (aIsUpdate && FinalizedRegistrationCollection.Any(x => x.IsSelected && string.IsNullOrEmpty(x.PtTranFinalization.eInvoiceToken)))
            {
                GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2439_G1_DuLieuDauVaoKhongDungQD);
                return;
            }
            IEditOutPtTransactionFinalization TransactionFinalizationView = Globals.GetViewModel<IEditOutPtTransactionFinalization>();
            TransactionFinalizationView.TransactionFinalizationObj = new OutPtTransactionFinalization
            {
                V_PaymentMode = (long)AllLookupValues.PaymentMode.TIEN_MAT
            };
            TransactionFinalizationView.ViewCase = 1;
            GlobalsNAV.ShowDialog_V3(TransactionFinalizationView);
            if (!TransactionFinalizationView.IsSaveCompleted)
            {
                return;
            }
            var mRegistrationCollection = FinalizedRegistrationCollection.Where(x => x.IsSelected).ToList().DeepCopy();
            if (mRegistrationCollection == null || mRegistrationCollection.Count == 0)
            {
                return;
            }
            foreach (var item in mRegistrationCollection)
            {
                item.PtTranFinalization.Denominator = TransactionFinalizationView.TransactionFinalizationObj.Denominator;
                item.PtTranFinalization.Symbol = TransactionFinalizationView.TransactionFinalizationObj.Symbol;
                item.PtTranFinalization.DateInvoice = TransactionFinalizationView.TransactionFinalizationObj.DateInvoice;
                item.PtTranFinalization.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
                item.PtTranFinalization.CreatedStaff = Globals.LoggedUserAccount.Staff;
            }
            Coroutine.BeginExecute(AddOrUpdateTransactionFinalization_Routine(mRegistrationCollection, aIsUpdate));
        }
        public void btnExportEInvoice()
        {
            ExportEInvoice();
        }
        public void btnUpdateEInvoice()
        {
            ExportEInvoice(true);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            if (SearchCriteria == null)
            {
                return;
            }
        }
        public void btnFinalizeReg(object source)
        {
            if (source == null || !(source is PatientRegistration_V2))
            {
                return;
            }
            PatientRegistration_V2 mPatientRegistration = source as PatientRegistration_V2;
            IEditOutPtTransactionFinalization aView = Globals.GetViewModel<IEditOutPtTransactionFinalization>();
            aView.Registration = mPatientRegistration;
            aView.TransactionFinalizationObj = new OutPtTransactionFinalization
            {
                TaxMemberName = mPatientRegistration.Patient.FullName,
                TaxMemberAddress = !string.IsNullOrEmpty(mPatientRegistration.Patient.PatientFullStreetAddress) ? mPatientRegistration.Patient.PatientFullStreetAddress : mPatientRegistration.Patient.PatientStreetAddress,
                StaffID = Globals.LoggedUserAccount.Staff.StaffID,
                PtRegistrationID = mPatientRegistration.PtRegistrationID,
                PatientFullName = mPatientRegistration.Patient == null ? null : mPatientRegistration.Patient.FullName,
                V_PaymentMode = (long)AllLookupValues.PaymentMode.TIEN_MAT,
                V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU,
                Buyer = (mPatientRegistration.Patient != null && !string.IsNullOrEmpty(mPatientRegistration.Patient.PatientEmployer)) ? mPatientRegistration.Patient.PatientEmployer : null,
            };
            GlobalsNAV.ShowDialog_V3<IEditOutPtTransactionFinalization>(aView);
            if (aView.TransactionFinalizationObj.TranFinalizationID > 0)
            {
                PatientRegistrationCollection.Remove(mPatientRegistration);
            }
        }
        public void cboRegStatus_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            cboRegStatus = sender as ComboBox;
        }
        public void btnExportEInvoiceData()
        {
            SearchCriteria.FromDate = FromDateDateTime.DateTime;
            SearchCriteria.ToDate = ToDateDateTime.DateTime;
            Microsoft.Win32.SaveFileDialog mFileDialog = new Microsoft.Win32.SaveFileDialog() { DefaultExt = ".xls", Filter = "Excel xls (*.xls)|*.xls", FilterIndex = 1 };
            if (mFileDialog.ShowDialog() != true)
            {
                return;
            }
            this.ShowBusyIndicator();
            SeachPtRegistrationCriteria mSearchCriteria = SearchCriteria.DeepCopy();
            mSearchCriteria.IsHasInvoice = true;
            ExportToExcelGeneric.Action(mSearchCriteria, mFileDialog, this);
            this.HideBusyIndicator();
        }
        public void btnExportRegistrationsData()
        {
            SearchCriteria.FromDate = FromDateDateTime.DateTime;
            SearchCriteria.ToDate = ToDateDateTime.DateTime;
            Microsoft.Win32.SaveFileDialog mFileDialog = new Microsoft.Win32.SaveFileDialog() { DefaultExt = ".xls", Filter = "Excel xls (*.xls)|*.xls", FilterIndex = 1 };
            if (mFileDialog.ShowDialog() != true)
            {
                return;
            }
            this.ShowBusyIndicator();
            SeachPtRegistrationCriteria mSearchCriteria = SearchCriteria.DeepCopy();
            mSearchCriteria.IsHasInvoice = false;
            ExportToExcelGeneric.Action(mSearchCriteria, mFileDialog, this);
            this.HideBusyIndicator();
        }
        #endregion
        #region KeyHandles
        public override void HandleHotKey_Action_New(object sender, LocalHotKeyEventArgs e)
        {
            foreach (var inputBindingCommand in ListInputBindingCmds)
            {
                if (inputBindingCommand.HotKey_Registered_Name == e.HotKey.Name)
                {
                    inputBindingCommand._executeDelegate.Invoke(this);
                    break;
                }
            }
        }
        protected override IEnumerable<InputBindingCommand> GetInputBindingCommands()
        {
            yield return new InputBindingCommand(() => FocusSelectedView())
            {
                HotKey_Registered_Name = "ghkCloseDetWin",
                GestureModifier = ModifierKeys.Control,
                GestureKey = (Key)Keys.F
            };
        }
        #endregion
        private IMinHourDateControl _FromDateDateTime;
        public IMinHourDateControl FromDateDateTime
        {
            get { return _FromDateDateTime; }
            set
            {
                _FromDateDateTime = value;
                NotifyOfPropertyChange(() => FromDateDateTime);
            }
        }
        private IMinHourDateControl _ToDateDateTime;
        public IMinHourDateControl ToDateDateTime
        {
            get { return _ToDateDateTime; }
            set
            {
                _ToDateDateTime = value;
                NotifyOfPropertyChange(() => ToDateDateTime);
            }
        }
        private ObservableCollection<Lookup> _EInvoiceStatus;
        public ObservableCollection<Lookup> EInvoiceStatus
        {
            get
            {
                return _EInvoiceStatus;
            }
            set
            {
                if (_EInvoiceStatus != value)
                {
                    _EInvoiceStatus = value;
                }
                NotifyOfPropertyChange(() => EInvoiceStatus);
            }
        }
        private Lookup _SelectedEInvoiceStatus;
        public Lookup SelectedEInvoiceStatus
        {
            get
            {
                return _SelectedEInvoiceStatus;
            }
            set
            {
                if (_SelectedEInvoiceStatus != value)
                {
                    _SelectedEInvoiceStatus = value;
                }
                NotifyOfPropertyChange(() => SelectedEInvoiceStatus);
            }
        }
        public void GetLookupEInvoiceStatus()
        {
            EInvoiceStatus = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.V_EInvoiceStatus))
                {
                    EInvoiceStatus.Add(tmpLookup);
                }
            }
            if (EInvoiceStatus.Count > 0)
            {
                SelectedEInvoiceStatus = EInvoiceStatus.FirstOrDefault();
            }
        }
        public void cboEInvoiceStatus_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (SelectedEInvoiceStatus != null)
            {
                SearchCriteria.ViewCase = Convert.ToByte(SelectedEInvoiceStatus.Code);
            }
        }
    }
}