using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;

/*
 * 20200323 #001 TBL: BM 0019715: Cho xem lịch sử khám bệnh theo thời gian
 */

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IDiagnosisTreatmentHistoriesTree)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DiagnosisTreatmentHistoriesTreeViewModel : ViewModelBase, IDiagnosisTreatmentHistoriesTree
    {
        #region Properties
        private long CurrentPatientID { get; set; }
        private ObservableCollection<TreatmentHistory> _TreatmentHistoryCollection;
        public ObservableCollection<TreatmentHistory> TreatmentHistoryCollection
        {
            get
            {
                return _TreatmentHistoryCollection;
            }
            set
            {
                if (_TreatmentHistoryCollection == value)
                {
                    return;
                }
                _TreatmentHistoryCollection = value;
                NotifyOfPropertyChange(() => TreatmentHistoryCollection);
                DiagnosisTreatmentView = CollectionViewSource.GetDefaultView(TreatmentHistoryCollection);
                DiagnosisTreatmentView.GroupDescriptions.Add(new PropertyGroupDescription("TreatmentHistoryGroupContent"));
                DiagnosisTreatmentView.GroupDescriptions.Add(new PropertyGroupDescription("GroupText"));
                DiagnosisTreatmentView.Filter = (x) => FilterDiagnosisTreatment(x);
            }
        }
        private ICollectionView _DiagnosisTreatmentView;
        public ICollectionView DiagnosisTreatmentView
        {
            get
            {
                return _DiagnosisTreatmentView;
            }
            set
            {
                if (_DiagnosisTreatmentView == value)
                {
                    return;
                }
                _DiagnosisTreatmentView = value;
                NotifyOfPropertyChange(() => DiagnosisTreatmentView);
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
                NotifyOfPropertyChange(() => IsInTreatmentProgram);
            }
        }
        private long? _FilterOutPtTreatmentProgramID;
        public long? FilterOutPtTreatmentProgramID
        {
            get
            {
                return _FilterOutPtTreatmentProgramID;
            }
            set
            {
                if (_FilterOutPtTreatmentProgramID != value)
                {
                    _FilterOutPtTreatmentProgramID = value;
                    NotifyOfPropertyChange(() => FilterOutPtTreatmentProgramID);
                }
            }
        }
        public bool IsInTreatmentProgram
        {
            get
            {
                return Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.IsInTreatmentProgram && IsOutPtTreatmentProgram;
            }
        }
        private bool _IsOutPtTreatmentProgram = false;
        public bool IsOutPtTreatmentProgram
        {
            get
            {
                return _IsOutPtTreatmentProgram;
            }
            set
            {
                if (_IsOutPtTreatmentProgram == value)
                {
                    return;
                }
                _IsOutPtTreatmentProgram = value;
                NotifyOfPropertyChange(() => IsOutPtTreatmentProgram);
                NotifyOfPropertyChange(() => IsInTreatmentProgram);
            }
        }
        private DateTime? _ToDate = Globals.GetCurServerDateTime().AddMonths(-6);
        public DateTime? ToDate
        {
            get { return _ToDate; }
            set
            {
                if(_ToDate != value)
                {
                    _ToDate = value;
                    NotifyOfPropertyChange(() => ToDate);
                }
            }
        }
        private DateTime? _FromDate = Globals.GetCurServerDateTime();
        public DateTime? FromDate
        {
            get { return _FromDate; }
            set
            {
                if (_FromDate != value)
                {
                    _FromDate = value;
                    NotifyOfPropertyChange(() => FromDate);
                }
            }
        }
        #endregion
        #region Methods
        //▼====== #001
        public void btnRefresh()
        {
            GetPatientServicesTreeView(CurrentPatientID);
        }
        //▲====== #001
        public void GetPatientServicesTreeView(long aPatientID)
        {
            NotifyViewDataChanged();
            FilterOutPtTreatmentProgramID = null;
            CurrentPatientID = aPatientID;
            TreatmentHistoryCollection.Clear();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetTreatmentHistoriesByPatientID(aPatientID, ToDate, FromDate, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetTreatmentHistoriesByPatientID(asyncResult);
                            TreatmentHistoryCollection = results.ToObservableCollection();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                    }), null);
                }
            });
            t.Start();
        }
        //20200214 TBL: Chỉ lấy lịch sử của bệnh nhân theo đăng ký
        public void GetPatientServicesTreeView_ByPtRegistrationID(long PtRegistrationID)
        {
            TreatmentHistoryCollection.Clear();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetTreatmentHistoriesByPtRegistrationID(PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetTreatmentHistoriesByPtRegistrationID(asyncResult);
                            TreatmentHistoryCollection = results.ToObservableCollection();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private ObservableCollection<PrescriptionDetail> ReadPrescriptionDetailFromString(string XmlString)
        {
            try
            {
                if (string.IsNullOrEmpty(XmlString))
                {
                    return null;
                }
                ObservableCollection<PrescriptionDetail> PrescriptionDetailCollection = new ObservableCollection<PrescriptionDetail>();
                XDocument PrescriptDetailDocument = XDocument.Load(new StringReader(XmlString));
                if (PrescriptDetailDocument.Element("Prescription") == null)
                {
                    return null;
                }
                foreach (var DetailItem in PrescriptDetailDocument.Element("Prescription").Elements("PrescriptionDetails"))
                {
                    PrescriptionDetail CurrentPrescriptionDetail = new PrescriptionDetail();
                    CurrentPrescriptionDetail.PrescriptDetailID = Convert.ToInt64(DetailItem.Element("PrescriptDetailID").Value);
                    CurrentPrescriptionDetail.DrugID = Convert.ToInt64(DetailItem.Element("DrugID").Value);
                    CurrentPrescriptionDetail.Qty = Convert.ToInt32(Convert.ToDecimal(DetailItem.Element("Qty").Value));
                    CurrentPrescriptionDetail.BrandName = Convert.ToString(DetailItem.Element("BrandName").Value);
                    CurrentPrescriptionDetail.Content = DetailItem.Element("Content") == null ? null : Convert.ToString(DetailItem.Element("Content").Value);
                    CurrentPrescriptionDetail.UnitName = DetailItem.Element("UnitName") == null ? null : Convert.ToString(DetailItem.Element("UnitName").Value);
                    CurrentPrescriptionDetail.UnitUse = DetailItem.Element("UnitNameUse") == null ? null : Convert.ToString(DetailItem.Element("UnitNameUse").Value);
                    CurrentPrescriptionDetail.Administration = DetailItem.Element("Administration") == null ? null : Convert.ToString(DetailItem.Element("Administration").Value);
                    CurrentPrescriptionDetail.DayRpts = DetailItem.Element("DayRpts") == null ? 0 : Convert.ToInt32(Convert.ToDecimal(DetailItem.Element("DayRpts").Value));
                    CurrentPrescriptionDetail.DrugInstructionNotes = DetailItem.Element("DrugInstructionNotes") == null ? null : Convert.ToString(DetailItem.Element("DrugInstructionNotes").Value);
                    CurrentPrescriptionDetail.V_DrugType = Convert.ToInt64(DetailItem.Element("V_DrugType").Value);
                    CurrentPrescriptionDetail.MDose = float.Parse(DetailItem.Element("MDose").Value);
                    CurrentPrescriptionDetail.ADose = float.Parse(DetailItem.Element("ADose").Value);
                    CurrentPrescriptionDetail.EDose = float.Parse(DetailItem.Element("EDose").Value);
                    CurrentPrescriptionDetail.NDose = float.Parse(DetailItem.Element("NDose").Value);
                    CurrentPrescriptionDetail.MDoseStr = Convert.ToString(DetailItem.Element("MDoseStr").Value);
                    CurrentPrescriptionDetail.ADoseStr = Convert.ToString(DetailItem.Element("ADoseStr").Value);
                    CurrentPrescriptionDetail.EDoseStr = Convert.ToString(DetailItem.Element("EDoseStr").Value);
                    CurrentPrescriptionDetail.NDoseStr = Convert.ToString(DetailItem.Element("NDoseStr").Value);
                    PrescriptionDetailCollection.Add(CurrentPrescriptionDetail);
                }
                return PrescriptionDetailCollection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void LoadPrescriptionDetailViewByID(long PrescriptID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPrescriptionByID(PrescriptID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetPrescriptionByID(asyncResult);
                            var allPrescription = new ObservableCollection<Prescription>();
                            if (results != null)
                            {
                                foreach (Prescription p in results)
                                {
                                    allPrescription.Add(p);
                                }
                                var CurrentPrescription = allPrescription.FirstOrDefault();
                                CurrentPrescription.PrescriptionDetails = ReadPrescriptionDetailFromString(CurrentPrescription.PrescriptDetailsStr);
                                IDiagnosisTreatmentHistoryDetail mPopupDialog = Globals.GetViewModel<IDiagnosisTreatmentHistoryDetail>();
                                mPopupDialog.CurrentPrescription = CurrentPrescription;
                                this.HideBusyIndicator();
                                GlobalsNAV.ShowDialog_V3(mPopupDialog);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.HideBusyIndicator();

                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private void LoadPrescriptionDetailViewByID_InPt(long ServiceRecID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPrescriptionIssueHistoriesInPtBySerRecID(ServiceRecID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetPrescriptionIssueHistoriesInPtBySerRecID(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                var PrescriptionIssued = results.LastOrDefault();
                                Prescription CurrentPrescription = results.LastOrDefault().Prescription;
                                GetPrescriptionDetailsByPrescriptID_InPt(CurrentPrescription);
                            }
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
            });
            t.Start();
        }
        public void GetPrescriptionDetailsByPrescriptID_InPt(Prescription CurrentPrescription, bool GetRemaining = false)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPrescriptionDetailsByPrescriptID_InPt(CurrentPrescription.PrescriptID, GetRemaining, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetPrescriptionDetailsByPrescriptID_InPt(asyncResult);
                            if (results != null)
                            {
                                CurrentPrescription.PrescriptionDetails = results.ToObservableCollection();
                            }
                            IDiagnosisTreatmentHistoryDetail mPopupDialog = Globals.GetViewModel<IDiagnosisTreatmentHistoryDetail>();
                            mPopupDialog.CurrentPrescription = CurrentPrescription;
                            GlobalsNAV.ShowDialog_V3(mPopupDialog);
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
            });
            t.Start();
        }
        private void DiagnosisIcd10Items_Load(DiagnosisTreatment aDiagnosisTreatment, long? PatientID, bool IsLast)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisIcd10Items_Load(aDiagnosisTreatment.ServiceRecID, PatientID, IsLast, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisIcd10Items_Load(asyncResult);
                            ObservableCollection<DiagnosisIcd10Items> CurrentIcd10Collection = results.ToObservableCollection();
                            if (CurrentIcd10Collection == null)
                            {
                                CurrentIcd10Collection = new ObservableCollection<DiagnosisIcd10Items>();
                            }
                            if (CurrentViewCase == (byte)ViewCase.DiagnosisTreatmentTreeView) //20200214 TBL: Nếu đang ở màn hình khám bệnh thì khi click vào để xem chẩn đoán thì hiện popup lên
                            {
                                IDiagnosisTreatmentHistoryDetail mPopupDialog = Globals.GetViewModel<IDiagnosisTreatmentHistoryDetail>();
                                mPopupDialog.CurrentDiagnosisTreatment = aDiagnosisTreatment;
                                mPopupDialog.CurrentIcd10Collection = CurrentIcd10Collection;
                                GlobalsNAV.ShowDialog_V3(mPopupDialog);
                            }
                            else //20200214 TBL: Nếu đang ở màn hình xác nhận chẩn đoán cho thủ thuật thì bắn sự kiện để hiển thị
                            {
                                Globals.EventAggregator.Publish(new SelectedDiagnosisTreatmentForConfirmSmallProceduce { Icd10Items = CurrentIcd10Collection, DiagnosisTreatment = aDiagnosisTreatment });
                            }
                        }
                        catch (Exception ex)
                        {
                            this.HideBusyIndicator();
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private void LoadDiagnosisTreatmentViewByDTItemID(long DTItemID, long aPatientID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisTreatmentByDTItemID(DTItemID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisTreatmentByDTItemID(asyncResult);
                            if (results != null)
                            {
                                DiagnosisTreatment CurrentDiagnosisTreatment = results;
                                DiagnosisIcd10Items_Load(CurrentDiagnosisTreatment, aPatientID, false);
                            }
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
            });
            t.Start();
        }
        private void DiagnosisIcd10Items_Load_InPt(DiagnosisTreatment aDiagnosisTreatment, long DTItemID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisIcd10Items_Load_InPt(DTItemID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            ObservableCollection<DiagnosisIcd10Items> CurrentIcd10Collection = contract.EndGetDiagnosisIcd10Items_Load_InPt(asyncResult).ToObservableCollection();
                            if (CurrentIcd10Collection == null)
                            {
                                CurrentIcd10Collection = new ObservableCollection<DiagnosisIcd10Items>();
                            }
                            DiagnosisICD9Items_Load_InPt(aDiagnosisTreatment, CurrentIcd10Collection, DTItemID);
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
            });
            t.Start();
        }
        private void DiagnosisICD9Items_Load_InPt(DiagnosisTreatment aDiagnosisTreatment, ObservableCollection<DiagnosisIcd10Items> CurrentIcd10Collection, long DTItemID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisICD9Items_Load_InPt(DTItemID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisICD9Items_Load_InPt(asyncResult);
                            ObservableCollection<DiagnosisICD9Items> CurrentIcd9Collection = new ObservableCollection<DiagnosisICD9Items>();
                            if (results != null)
                            {
                                CurrentIcd9Collection = results.ToObservableCollection();
                            }
                            IDiagnosisTreatmentHistoryDetail mPopupDialog = Globals.GetViewModel<IDiagnosisTreatmentHistoryDetail>();
                            mPopupDialog.CurrentDiagnosisTreatment = aDiagnosisTreatment;
                            mPopupDialog.CurrentIcd10Collection = CurrentIcd10Collection;
                            mPopupDialog.CurrentIcd9Collection = CurrentIcd9Collection;
                            this.HideBusyIndicator();
                            GlobalsNAV.ShowDialog_V3(mPopupDialog);
                        }
                        catch (Exception ex)
                        {
                            this.HideBusyIndicator();
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                    }), null);
                }
            });
            t.Start();
        }
        public void LoadDiagnosisTreatmentViewByDTItemID_InPt(long PtRegistrationID, long DTItemID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var mFactory = new ePMRsServiceClient())
                {
                    var mContract = mFactory.ServiceInstance;
                    mContract.BeginGetDiagnosisTreatment_InPt_ByPtRegID(PtRegistrationID, DTItemID, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = mContract.EndGetDiagnosisTreatment_InPt_ByPtRegID(asyncResult);
                            if (results != null && results.Count == 1)
                            {
                                DiagnosisTreatment CurrentDiagnosisTreatment = results.First();
                                DiagnosisIcd10Items_Load_InPt(CurrentDiagnosisTreatment, DTItemID);
                            }
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
            });
            t.Start();
        }
        private void GetSmallProcedureForPrintPreview(long aSmallProcedureID,bool InPt)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetSmallProcedure(0, aSmallProcedureID, InPt ? (long)AllLookupValues.RegistrationType.NOI_TRU : (long)AllLookupValues.RegistrationType.NGOAI_TRU, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var mSmallProcedure = contract.EndGetSmallProcedure(asyncResult);
                            if (mSmallProcedure != null && mSmallProcedure.SmallProcedureID > 0 && Registration_DataStorage != null
                                && Registration_DataStorage.CurrentPatientRegistration != null)
                            {
                                CommonGlobals.PrintProcedureProcess(this, mSmallProcedure, Registration_DataStorage.CurrentPatientRegistration);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private bool FilterDiagnosisTreatment(object aCurrentTreatmentHistory)
        {
            TreatmentHistory CurrentTreatmentHistory = aCurrentTreatmentHistory as TreatmentHistory;
            if (CurrentTreatmentHistory == null)
            {
                return false;
            }
            return FilterOutPtTreatmentProgramID.GetValueOrDefault(0) == 0 || CurrentTreatmentHistory.OutPtTreatmentProgramID == FilterOutPtTreatmentProgramID;
        }
        public void NotifyViewDataChanged()
        {
            NotifyOfPropertyChange(() => Registration_DataStorage);
            NotifyOfPropertyChange(() => IsInTreatmentProgram);
        }
        public byte CurrentViewCase { get; set; } = (byte)ViewCase.DiagnosisTreatmentTreeView;
        private enum ViewCase : byte
        {
            DiagnosisTreatmentTreeView = 1,
            ConfirmDiagnosisTreatment = 2
        }
        #endregion
        #region Events
        public DiagnosisTreatmentHistoriesTreeViewModel()
        {
            TreatmentHistoryCollection = new ObservableCollection<TreatmentHistory>();
            NotifyViewDataChanged();
        }
        public void gvDiagnosis_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(sender is DataGrid))
            {
                return;
            }
            DataGrid CurrentDataGrid = sender as DataGrid;
            if (CurrentDataGrid.SelectedItem == null || !(CurrentDataGrid.SelectedItem is TreatmentHistory))
            {
                return;
            }
            TreatmentHistory mTreatmentHistory = CurrentDataGrid.SelectedItem as TreatmentHistory;
            if (CurrentViewCase == (byte)ViewCase.DiagnosisTreatmentTreeView) //20200214 TBL: Nếu ở màn hình khám bệnh thì làm như cũ
            {
                if (mTreatmentHistory.PrescriptID > 0 && !mTreatmentHistory.InPt)
                {
                    LoadPrescriptionDetailViewByID(mTreatmentHistory.PrescriptID);
                }
                else if (mTreatmentHistory.PrescriptID > 0 && mTreatmentHistory.ServiceRecID > 0 && mTreatmentHistory.InPt)
                {
                    LoadPrescriptionDetailViewByID_InPt(mTreatmentHistory.ServiceRecID);
                }
                else if (mTreatmentHistory.DTItemID > 0 && !mTreatmentHistory.InPt)
                {
                    LoadDiagnosisTreatmentViewByDTItemID(mTreatmentHistory.DTItemID, CurrentPatientID);
                }
                else if (mTreatmentHistory.DTItemID > 0 && mTreatmentHistory.PtRegistrationID > 0 && mTreatmentHistory.InPt)
                {
                    LoadDiagnosisTreatmentViewByDTItemID_InPt(mTreatmentHistory.PtRegistrationID, mTreatmentHistory.DTItemID);
                }
                else if (mTreatmentHistory.PatientPCLReqID > 0 && mTreatmentHistory.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                {
                    IPatientPCLLaboratoryResult mPopupDialog = Globals.GetViewModel<IPatientPCLLaboratoryResult>();
                    mPopupDialog.IsDialogView = true;
                    mPopupDialog.IsShowSummaryContent = false;
                    mPopupDialog.LoadPCLRequestResult(new PatientPCLRequest
                    {
                        PatientPCLReqID = mTreatmentHistory.PatientPCLReqID,
                        PatientID = CurrentPatientID,
                        V_PCLRequestType = !mTreatmentHistory.InPt ? AllLookupValues.V_PCLRequestType.NGOAI_TRU : AllLookupValues.V_PCLRequestType.NOI_TRU
                    });
                    GlobalsNAV.ShowDialog_V3(mPopupDialog, null, null, false, true, new System.Windows.Size(1000, 700));
                }
                else if (mTreatmentHistory.PatientPCLReqID > 0 && mTreatmentHistory.PCLExamTypeID > 0 && mTreatmentHistory.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
                {
                    IPatientPCLImagingResult_V2 mPopupDialog = Globals.GetViewModel<IPatientPCLImagingResult_V2>();
                    mPopupDialog.InitHTML();
                    mPopupDialog.CheckTemplatePCLResultByReqID(mTreatmentHistory.PatientPCLReqID, mTreatmentHistory.InPt);
                    //mPopupDialog.LoadDataCoroutineEx(new PatientPCLRequest
                    //{
                    //    PatientPCLReqID = mTreatmentHistory.PatientPCLReqID,
                    //    PatientID = CurrentPatientID,
                    //    V_PCLRequestType = !mTreatmentHistory.InPt ? AllLookupValues.V_PCLRequestType.NGOAI_TRU : AllLookupValues.V_PCLRequestType.NOI_TRU,
                    //    PCLExamTypeID = mTreatmentHistory.PCLExamTypeID
                    //});
                    GlobalsNAV.ShowDialog_V3(mPopupDialog, null, null, false, true, new System.Windows.Size(1000, 700));
                }
                else if (mTreatmentHistory.SmallProcedureID.HasValue && mTreatmentHistory.SmallProcedureID.Value > 0)
                {
                    GetSmallProcedureForPrintPreview(mTreatmentHistory.SmallProcedureID.Value, mTreatmentHistory.InPt);
                }
            }
            else //20200214 TBL: Nếu ở màn hình xác nhận chẩn đoán cho thủ thuật thì chỉ cho click vào chẩn đoán
            {
                if (mTreatmentHistory.DTItemID > 0 && !mTreatmentHistory.InPt)
                {
                    LoadDiagnosisTreatmentViewByDTItemID(mTreatmentHistory.DTItemID, CurrentPatientID);
                }
            }
        }
        public void btnPrintTreatmentRecord(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!(sender is Button))
            {
                return;
            }
            Button CurrentButton = sender as Button;
            if (CurrentButton.CommandParameter == null && !(CurrentButton.CommandParameter is TreatmentHistoryGroupContent))
            {
                return;
            }
            TreatmentHistoryGroupContent CurrentTreatmentHistoryGroupContent = CurrentButton.CommandParameter as TreatmentHistoryGroupContent;
            if (CurrentTreatmentHistoryGroupContent == null || CurrentTreatmentHistoryGroupContent.PtRegistrationID == 0
                || CurrentTreatmentHistoryGroupContent.InPt)
            {
                return;
            }
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.eItem = ReportName.MedicalRecord;
                proAlloc.RegistrationID = CurrentTreatmentHistoryGroupContent.PtRegistrationID;
                proAlloc.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }
        public void IsInTreatmentProgramCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (Registration_DataStorage == null || Registration_DataStorage.CurrentPatientRegistration == null || (sender as CheckBox).IsChecked.GetValueOrDefault(false) == false)
            {
                FilterOutPtTreatmentProgramID = null;
            }
            else
            {
                FilterOutPtTreatmentProgramID = Registration_DataStorage.CurrentPatientRegistration.OutPtTreatmentProgramID;
            }
            DiagnosisTreatmentView.Refresh();
        }
        #endregion
    }
}