using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
/*
 * 20191104 #001 TTM:   BM 0018528: [Left Menu] Bổ sung cây y lệnh chẩn đoán vào màn hình quản lý bệnh nhân nội trú
 * 20221214 #002 QTD:   Thêm y lệnh phiếu chăm sóc từ danh sách y lệnh bên trái
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IDiagnosisTreatmentTree)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DiagnosisTreatmentTreeViewModel : ViewModelBase, IDiagnosisTreatmentTree
        ,IHandle<GlobalCurPatientServiceRecordLoadComplete_Consult_InPt>
    {
        #region Properties
        ObservableCollection<DiagnosisTreatment> _DiagnosisTreatmentCollection;
        public ObservableCollection<DiagnosisTreatment> DiagnosisTreatmentCollection
        {
            get
            {
                return _DiagnosisTreatmentCollection;
            }
            set
            {
                if (_DiagnosisTreatmentCollection == value)
                {
                    return;
                }
                _DiagnosisTreatmentCollection = value;
                NotifyOfPropertyChange(() => DiagnosisTreatmentCollection);
                DiagnosisTreatmentView = CollectionViewSource.GetDefaultView(DiagnosisTreatmentCollection);
                DiagnosisTreatmentView.GroupDescriptions.Add(new PropertyGroupDescription("DeptDetail.GroupText2"));

                //▼===== 20191011 TTM:  Trong DiagnosisTreatment không có InPatientDeptDetailID mà là của DeptDetail (DiagnosisDeptDetail). 
                //                      Nên phải đăt DeptDetail trước InPatientDeptDetailID Mới có thể lấy biding.
                //DiagnosisTreatmentView.SortDescriptions.Add(new SortDescription("InPatientDeptDetailID", ListSortDirection.Descending));
                DiagnosisTreatmentView.SortDescriptions.Add(new SortDescription("DeptDetail.InPatientDeptDetailID", ListSortDirection.Descending));
                //▲===== 
                //20191128 TBL: Sort theo ngày chứ không theo DTItemID vì y lệnh co thể chỉnh sửa giờ được
                //DiagnosisTreatmentView.SortDescriptions.Add(new SortDescription("DTItemID", ListSortDirection.Descending));
                DiagnosisTreatmentView.SortDescriptions.Add(new SortDescription("DiagnosisDate", ListSortDirection.Descending));
                if (gTreatmentCollectionOnChanged != null)
                {
                    gTreatmentCollectionOnChanged(DiagnosisTreatmentCollection);
                }
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
        private bool _FromInPatientAdmView;
        public bool FromInPatientAdmView
        {
            get
            {
                return _FromInPatientAdmView;
            }
            set
            {
                if (_FromInPatientAdmView == value)
                {
                    return;
                }
                _FromInPatientAdmView = value;
                NotifyOfPropertyChange(() => FromInPatientAdmView);
            }
        }
        private bool mChanDoan_tabLanKhamTruoc_HieuChinh;
        private ObservableCollection<DiagnosisIcd10Items> RefICD10List;
        private ObservableCollection<DiagnosisICD9Items> RefICD9List;
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
        public TreatmentCollectionOnChanged gTreatmentCollectionOnChanged { get; set; }
        public byte CurrentViewCase { get; set; } = (byte)ViewCase.DiagnosisTreatmentTreeView;
        private enum ViewCase : byte
        {
            DiagnosisTreatmentTreeView = 1,
            ConfirmDiagnosisTreatment = 2
        }
        #endregion
        #region Events
        [ImportingConstructor]
        public DiagnosisTreatmentTreeViewModel(IWindsorContainer aContainer, INavigationService aNavigation, IEventAggregator aEventArg)
        {
            mChanDoan_tabLanKhamTruoc_HieuChinh = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                , (int)eConsultation.mPtPMRConsultationNew,
                (int)oConsultationEx.mChanDoan_tabLanKhamTruoc_HieuChinh, (int)ePermission.mView) || !Globals.isAccountCheck;
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            Globals.EventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }
        public void Handle(GlobalCurPatientServiceRecordLoadComplete_Consult_InPt message)
        {
            if (Registration_DataStorage.CurrentPatientRegistration == null)
            {
                return;
            }
            LoadData(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
        }
        public void gvDiagnosis_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!mChanDoan_tabLanKhamTruoc_HieuChinh && CurrentViewCase != (byte)ViewCase.ConfirmDiagnosisTreatment)
            {
                Globals.ShowMessage(eHCMSResources.Z0508_G1_ChuaDuocQuyenSua, "");
                return;
            }
            if ((sender as DataGrid).SelectedItem != null)
            {
                var CurrentDiagTrmt = (sender as DataGrid).SelectedItem as DiagnosisTreatment;
                //▼==== #002
                if (CurrentDiagTrmt.IntPtDiagDrInstructionID == -1)
                {
                    addNewInstruction(CurrentDiagTrmt);
                }
                else
                {
                    Coroutine.BeginExecute(DoubleClick_Routine(CurrentDiagTrmt));
                }
                //▲==== #002
            }
        }
        private IEnumerator<IResult> DoubleClick_Routine(DiagnosisTreatment aDiagnosisTreatment)
        {
            if (aDiagnosisTreatment == null)
            {
                yield break;
            }
            this.ShowBusyIndicator();
            RefICD10List = new ObservableCollection<DiagnosisIcd10Items>();
            RefICD9List = new ObservableCollection<DiagnosisICD9Items>();
            //CMN: Khi double click vào một chẩn đoán ngoại trú thì send một event khác và không cần kiểm tra các điều kiện cho Y lệnh
            if (aDiagnosisTreatment.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                yield return new GenericCoRoutineTask(DiagnosisIcd10Items_Load, aDiagnosisTreatment.ServiceRecID);
                Globals.EventAggregator.Publish(new ConsultationDoubleClickEvent { DiagTrmtItem = aDiagnosisTreatment.DeepCopy(), refIDC10List = RefICD10List.DeepCopy() });
                this.HideBusyIndicator();
                yield break;
            }
            yield return new GenericCoRoutineTask(DiagnosisIcd10Items_Load_InPt, aDiagnosisTreatment.DTItemID);
            yield return new GenericCoRoutineTask(DiagnosisICD9Items_Load_InPt, aDiagnosisTreatment.DTItemID);
            Globals.EventAggregator.Publish(new ConsultationDoubleClickEvent_InPt_1());
            if (aDiagnosisTreatment.IntPtDiagDrInstructionID == 0 || CurrentViewCase == (byte)ViewCase.ConfirmDiagnosisTreatment)
            {
                Globals.EventAggregator.Publish(new ConsultationDoubleClickEvent_InPt_2 { DiagTrmtItem = aDiagnosisTreatment.DeepCopy(), refIDC10List = RefICD10List.DeepCopy(), refICD9List = RefICD9List.DeepCopy() });
            }
            this.HideBusyIndicator();
            if (aDiagnosisTreatment.IntPtDiagDrInstructionID > 0 && CurrentViewCase == (byte)ViewCase.DiagnosisTreatmentTreeView)
            {
                GlobalsNAV.ShowDialog<IInPatientInstruction>((aView) =>
                {
                    if (aView is ViewModelBase)
                    {
                        (aView as ViewModelBase).IsDialogView = true;
                    }
                    if (FromInPatientAdmView)
                    {
                        aView.FromInPatientAdmView = FromInPatientAdmView;
                    }
                    aView.Registration_DataStorage = Registration_DataStorage;
                    aView.LoadRegistrationInfo(Registration_DataStorage.CurrentPatientRegistration.DeepCopy(), false); //20191130 TBL: phải DeepCopy vì khi popup hiện lên thì thông tin sẽ thay đổi nên khi tắt popup thì thông tin ở dưới sẽ bị sai
                    aView.ReloadLoadInPatientInstruction(aDiagnosisTreatment.IntPtDiagDrInstructionID);
                }, null, false, true, Globals.GetDefaultDialogViewSize());
                yield break; 
            }
            //▼===== #001 Nếu xem cây chẩn đoán y lệnh ở màn hình quản lý thì đưa popup lên cho người dùng xem
            //            Vì ở menu này không thể đưa thông tin sang màn hình như bên chẩn đoán đc. 
            //            Và cần phải hide toàn bộ nút để người dùng chỉ được xem không được thao tác.
            if (FromInPatientAdmView && aDiagnosisTreatment.DTItemID > 0 && CurrentViewCase == (byte)ViewCase.DiagnosisTreatmentTreeView)
            {
                GlobalsNAV.ShowDialog<IConsultationOld_InPt>((aView) =>
                {
                    if (aView is ViewModelBase)
                    {
                        (aView as ViewModelBase).IsDialogView = true;
                    }
                    aView.Registration_DataStorage = Registration_DataStorage;
                    aView.HideAllButton();
                    aView.UpdateDiagTrmtItemIntoLayout(aDiagnosisTreatment, RefICD10List, RefICD9List);
                }, null, false, true, Globals.GetDefaultDialogViewSize());
                yield break;
            }
            //▲===== #001
        }
        #endregion
        #region Methods
        public void LoadData(long PtRegistrationID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var mFactory = new ePMRsServiceClient())
                {
                    var mContract = mFactory.ServiceInstance;
                    mContract.BeginGetDiagnosisTreatment_InPt_ByPtRegID(PtRegistrationID, null, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var mReturnCollection = mContract.EndGetDiagnosisTreatment_InPt_ByPtRegID(asyncResult);
                            if (mReturnCollection == null)
                            {
                                DiagnosisTreatmentCollection = new ObservableCollection<DiagnosisTreatment>();
                            }
                            else
                            {
                                DiagnosisTreatmentCollection = mReturnCollection.ToObservableCollection();
                            }
                            //▼==== #002
                            var diagnosisDeptDetail = DiagnosisTreatmentCollection[0].DeptDetail;
                            DiagnosisTreatmentCollection.Add(initNullDiagnosisTreatment(diagnosisDeptDetail));
                            //▲==== #002
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
        //CMN: Hàm lấy danh sách ICD10 cho chẩn đoán ngoại trú
        private void DiagnosisIcd10Items_Load(GenericCoRoutineTask aGenTask, object aServiceRecID)
        {
            long ServiceRecID = (long)aServiceRecID;
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new ePMRsServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginGetDiagnosisIcd10Items_Load(ServiceRecID, null, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var GettedCollection = CurrentContract.EndGetDiagnosisIcd10Items_Load(asyncResult);
                            if (GettedCollection != null)
                            {
                                RefICD10List = GettedCollection.ToObservableCollection();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            aGenTask.ActionComplete(true);
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }
        private void DiagnosisIcd10Items_Load_InPt(GenericCoRoutineTask aGenTask, object aDTItemID)
        {
            long DTItemID = (long)aDTItemID;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisIcd10Items_Load_InPt(DTItemID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var mReturnCollection = contract.EndGetDiagnosisIcd10Items_Load_InPt(asyncResult);
                            if (mReturnCollection != null)
                            {
                                RefICD10List = mReturnCollection.ToObservableCollection();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            aGenTask.ActionComplete(true);
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private void DiagnosisICD9Items_Load_InPt(GenericCoRoutineTask aGenTask, object aDTItemID)
        {
            long DTItemID = (long)aDTItemID;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisICD9Items_Load_InPt(DTItemID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var mReturnCollection = contract.EndGetDiagnosisICD9Items_Load_InPt(asyncResult);
                            if (mReturnCollection != null)
                            {
                                RefICD9List = mReturnCollection.ToObservableCollection();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            aGenTask.ActionComplete(true);
                        }
                    }), null);
                }
            });
            t.Start();
        }
        public void ApplyDiagnosisTreatmentCollection(IList<DiagnosisTreatment> aDiagnosisTreatmentCollection)
        {
            if (aDiagnosisTreatmentCollection == null)
            {
                DiagnosisTreatmentCollection = new ObservableCollection<DiagnosisTreatment>();
            }
            else
            {
                DiagnosisTreatmentCollection = aDiagnosisTreatmentCollection.ToObservableCollection();
            }
        }
        #endregion
        //▼====: #002
        public void addNewInstruction(DiagnosisTreatment CurrentDiagTrmt)
        {
            if (CurrentDiagTrmt != null)
            {
                TryClose();
                ITicketCare mDialogView = Globals.GetViewModel<ITicketCare>();
                mDialogView.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                mDialogView.DoctorOrientedTreatment = CurrentDiagTrmt.OrientedTreatment;
                mDialogView.GetTicketCare(0);
                GlobalsNAV.ShowDialog_V3(mDialogView);

            }
        }

        private DiagnosisTreatment initNullDiagnosisTreatment(DiagnosisDeptDetail diagnosisDeptDetail)
        {
            DiagnosisTreatment diagnosis = new DiagnosisTreatment();
            diagnosis.DeptDetail = diagnosisDeptDetail;
            diagnosis.IntPtDiagDrInstructionID = -1;
            diagnosis.DiagnosisFinal = string.Empty;
            return diagnosis;
        }
        //▲====: #002
    }
}