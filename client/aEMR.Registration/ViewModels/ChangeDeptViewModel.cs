using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using System.ServiceModel;
using eHCMS.CommonUserControls.CommonTasks;
using eHCMSLanguage;
using Castle.Windsor;
using aEMR.Common.BaseModel;
using aEMR.CommonTasks;
using aEMR.Common.Collections;
using System.Linq;
/*
* 20190816 #001 TTM:   BM ?: Loại bỏ khoa dược ra khỏi danh sách khoa được chuyển.
* 20190828 #002 TTM:   BM 0013230: Cảnh báo: khi bệnh nhân còn chỉ định hoặc phiếu xuất chưa tạo bill.
* 20220926 #003 DatTB: Thêm biến ẩn/hiện checkbox Bệnh diễn tiến
*/
namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IChangeDept)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ChangeDeptViewModel : ViewModelBase, IChangeDept
    {
        [ImportingConstructor]
        public ChangeDeptViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);

            StartDepartmentContent = Globals.GetViewModel<IDepartmentListing>();
            StartDepartmentContent.AddSelectOneItem = true;


            DestinationDepartmentContent = Globals.GetViewModel<IDepartmentListing>();
            DestinationDepartmentContent.AddSelectOneItem = true;
            
            (StartDepartmentContent as PropertyChangedBase).PropertyChanged += StartDepartmentContent_PropertyChanged;
            (DestinationDepartmentContent as PropertyChangedBase).PropertyChanged += DestinationDepartmentContent_PropertyChanged;
        }

        public void LoadData()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                //▼===== #001: set cờ không load Khoa dược IsLoadDrugDept = false sẽ không thêm khoa dược vào danh sách.
                StartDepartmentContent.LstRefDepartment = LstRefDepartment;
                StartDepartmentContent.IsLoadDrugDept = false;
                StartDepartmentContent.LoadData();
                //DestinationDepartmentContent.LoadData(OriginalDeptLocation.DeptID);
                //▼===== 20200103 TTM: Thêm điều kiện kiểm tra nếu là màn hình chọn khoa để đề nghị chuyển thì không lấy khoa đã xoá.
                //              Vì anh Tuấn nói để lại để xem các trường hợp cũ. Ở chức năng khác.
                if (IsChangedDept)
                {
                    DestinationDepartmentContent.IsNotShowDeptDeleted = true;
                }
                //▲===== 
                DestinationDepartmentContent.IsLoadDrugDept = false;
                DestinationDepartmentContent.LoadData(0, true);
                //▲===== #001
            }
        }


        private long StartDeptID;
        void StartDepartmentContent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                StartDeptID = 0;
                if (StartDepartmentContent.SelectedItem != null)
                {
                    StartDeptID = StartDepartmentContent.SelectedItem.DeptID;

                    //▼==== #003
                    if (StartDepartmentContent.SelectedItem.DeptID.Equals((long)AllLookupValues.DeptID.CapCuu)
                        || StartDepartmentContent.SelectedItem.DeptID.Equals((long)AllLookupValues.DeptID.KhoaNoi)
                        || StartDepartmentContent.SelectedItem.DeptID.Equals((long)AllLookupValues.DeptID.GayMeHoiSuc))
                    {
                        ShowProgressive = true;
                    }
                    else
                    {
                        ShowProgressive = false;
                        IsProgressive = false;
                    }
                    //▲==== #003
                }
            }
        }


        private long EndDeptID;
        void DestinationDepartmentContent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                EndDeptID = 0;
                if (DestinationDepartmentContent.SelectedItem != null)
                {
                    EndDeptID = DestinationDepartmentContent.SelectedItem.DeptID;
                }
            }
        }


        private ObservableCollection<long> _LstRefDepartment;
        public ObservableCollection<long> LstRefDepartment
        {
            get { return _LstRefDepartment; }
            set
            {
                _LstRefDepartment = value;
                NotifyOfPropertyChange(() => LstRefDepartment);
            }
        }

        private RefDepartment _OriginalRefDepartment;
        public RefDepartment OriginalRefDepartment
        {
            get
            {
                return _OriginalRefDepartment;
            }
            set
            {
                if (_OriginalRefDepartment != null)
                {
                    _OriginalRefDepartment= value;
                }
            }
        }

        private IDepartmentListing _startDepartmentContent;
        public IDepartmentListing StartDepartmentContent
        {
            get { return _startDepartmentContent; }
            set
            {
                _startDepartmentContent = value;
                NotifyOfPropertyChange(() => StartDepartmentContent);
            }
        }

        private IDepartmentListing _departmentContent;
        public IDepartmentListing DestinationDepartmentContent
        {
            get { return _departmentContent; }
            set
            {
                _departmentContent = value;
                NotifyOfPropertyChange(() => DestinationDepartmentContent);
            }
        }

        private InPatientTransferDeptReq _curInPatientTransferDeptReq;
        public InPatientTransferDeptReq curInPatientTransferDeptReq
        {
            get { return _curInPatientTransferDeptReq; }
            set
            {
                _curInPatientTransferDeptReq = value;
                NotifyOfPropertyChange(() => curInPatientTransferDeptReq);
            }
        }

        private ObservableCollection<InPatientTransferDeptReq> _allInPatientTransferDeptReq;
        public ObservableCollection<InPatientTransferDeptReq> allInPatientTransferDeptReq
        {
            get { return _allInPatientTransferDeptReq; }
            set
            {
                _allInPatientTransferDeptReq = value;
                NotifyOfPropertyChange(() => allInPatientTransferDeptReq);
            }
        }

        private InPatientAdmDisDetails _currentAdmission;

        public InPatientAdmDisDetails CurrentAdmission
        {
            get { return _currentAdmission; }
            set
            {
                _currentAdmission = value;
                NotifyOfPropertyChange(() => CurrentAdmission);
            }
        }

        private long _CurrentDeptID;
        public long CurrentDeptID
        {
            get { return _CurrentDeptID; }
            set
            {
                if(_CurrentDeptID != value)
                {
                    _CurrentDeptID = value;
                    NotifyOfPropertyChange(() => CurrentDeptID);
                }
            }
        }
        private InPatientTransferDeptReq CreateInPatientTransferDeptReq()
        {
            var item = new InPatientTransferDeptReq();
            item.ReqDate = Globals.GetCurServerDateTime();
            item.reqStaff = Globals.LoggedUserAccount.Staff;
            item.CurDeptID = StartDeptID;
            item.ReqDeptID = EndDeptID;// SelectedLocation.DeptID;
            item.IsProgressive = IsProgressive;
            if (CurrentAdmission != null)
            {
                item.InPatientAdmDisDetailID = CurrentAdmission.InPatientAdmDisDetailID;
            }
            if (UseOnlyDailyDiagnosis)
            {
                item.ConfirmedDTItemID = CurrentDiagnosisTreatment.DTItemID;
            }
            return item;
        }

        public bool ValidateInfo()
        {
            if (_currentAdmission == null)
            {
                return false;
            }
            if (StartDeptID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0369_G1_HayChonKhoaHienTai, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (EndDeptID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0370_G1_ChonKhoaCanChuyenDen, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (StartDeptID == EndDeptID)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0425_G1_KhChuyenDenPhaiKhacKhoaHTai), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (UseOnlyDailyDiagnosis && (CurrentDiagnosisTreatment == null || CurrentDiagnosisTreatment.DTItemID == 0 || CurrentDiagnosisTreatment.IsAdmission))
            {
                MessageBox.Show(eHCMSResources.Z2895_G1_Msg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (UseOnlyDailyDiagnosis)
            {
                if (StartDeptID != CurrentDiagnosisTreatment.Department.DeptID)
                {
                    MessageBox.Show(eHCMSResources.Z2896_G1_Msg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
                string[] ListStr = Globals.ServerConfigSection.ConsultationElements.ConsultMinTimeReqBeforeExit.Split(new char[] { ';' });
                int MinTimeReq = Convert.ToInt16(ListStr[3]);
                if (MinTimeReq > 0 && (Globals.GetCurServerDateTime() - CurrentDiagnosisTreatment.DiagnosisDate).TotalMinutes > MinTimeReq * 60)
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z2896_G1_ChanDoanQuaTGXacNhan, string.Format("{0} {1}", MinTimeReq, eHCMSResources.T1209_G1_GioL)), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }
            return true;
        }
        private ObservableCollection<DeptLocation> _locations;
        public ObservableCollection<DeptLocation> Locations
        {
            get
            {
                return _locations;
            }
            set
            {
                _locations = value;
                NotifyOfPropertyChange(() => Locations);
            }
        }

        private DeptLocation _selectedLocation;
        public DeptLocation SelectedLocation
        {
            get
            {
                return _selectedLocation;
            }
            set
            {
                _selectedLocation = value;
                NotifyOfPropertyChange(() => SelectedLocation);
            }
        }

        private IEnumerator<IResult> DoLoadLocations(long deptId)
        {
            var deptLoc = new LoadDeptLoctionByIDTask(deptId);
            yield return deptLoc;
            if (deptLoc.DeptLocations != null)
            {
                Locations = new ObservableCollection<DeptLocation>(deptLoc.DeptLocations);
            }
            else
            {
                Locations = new ObservableCollection<DeptLocation>();
            }

            var itemDefault = new DeptLocation();
            itemDefault.Location = new Location();
            itemDefault.Location.LID = -1;
            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0116_G1_HayChonPg);
            Locations.Insert(0, itemDefault);
            SelectedLocation = itemDefault;
            yield break;
        }

        public void LoadLocations(long? deptId)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0115_G1_LayDSPgBan });

            var list = new List<refModule>();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllLocationsByDeptIDOld(deptId, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var allItems = contract.EndGetAllLocationsByDeptIDOld(asyncResult);

                            if (allItems != null)
                            {
                                Locations = new ObservableCollection<DeptLocation>(allItems);
                            }
                            else
                            {
                                Locations = new ObservableCollection<DeptLocation>();
                            }

                            var itemDefault = new DeptLocation();
                            itemDefault.DeptID = -1;
                            itemDefault.Location = new Location();
                            itemDefault.Location.LID = -1;
                            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0116_G1_HayChonPg);
                            Locations.Insert(0, itemDefault);

                            SelectedLocation = itemDefault;
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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

        #region COMMANDS
        
        public void ChangeDeptCmd()
        {
            if (!ValidateInfo())
            {
                return;
            }
            Coroutine.BeginExecute(CheckValid_New());
        }

        /*▼====: #001*/
        private bool _IsBusy = false;
        public bool IsBusy
        {
            get
            {
                return _IsBusy;
            }
            set
            {
                _IsBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
            }
        }

        private string _BusyMessage = eHCMSResources.A0501_G1_Msg_InfoDangTHien;
        public string BusyMessage
        {
            get
            {
                return _BusyMessage;
            }
            set
            {
                _BusyMessage = value;
                NotifyOfPropertyChange(() => BusyMessage);
            }
        }

        private void ShowBusy(string BusyMessage)
        {
            this.BusyMessage = BusyMessage;
            IsBusy = true;
        }

        /*▲====: #001*/
        public void InsertInPatientTransferDeptReq(InPatientTransferDeptReq p)
        {
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginInsertInPatientTransferDeptReq(p, Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<PatientTransactionPayment> allItems = new ObservableCollection<PatientTransactionPayment>();
                                try
                                {
                                    var res= contract.EndInsertInPatientTransferDeptReq(asyncResult);
                                    if(res)
                                    {
                                        MessageBox.Show(eHCMSResources.A1053_G1_Msg_InfoTHienOK);
                                        Globals.EventAggregator.Publish(new ReturnItem<InPatientTransferDeptReq, object> { Item = null, Source = this });
                                        this.TryClose();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    ClientLoggerHelper.LogInfo(ex.Message);
                                }
                                finally
                                {
                                    this.DlgHideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    ClientLoggerHelper.LogInfo(ex.Message);
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }

        public void DeleteInPatientTransferDeptReqXML(ObservableCollection<InPatientTransferDeptReq> lstInPtTransDeptReq)
        {

            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0124_G1_DangLayDSTThaiDK)
                });
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginDeleteInPatientTransferDeptReqXML(lstInPtTransDeptReq,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<PatientTransactionPayment> allItems = new ObservableCollection<PatientTransactionPayment>();
                                try
                                {
                                    var res = contract.EndDeleteInPatientTransferDeptReqXML(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                finally
                                {

                                }


                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }

        public void CancelCmd()
        {
            TryClose();
        }

#endregion

        public DeptLocation DestinationDeptLocation
        {
            get
            {
                return SelectedLocation;
            }
        }

        private DeptLocation _originalDeptLocation;

        public DeptLocation OriginalDeptLocation
        {
            get { return _originalDeptLocation; }
            set
            {
                _originalDeptLocation = value;
                NotifyOfPropertyChange(() => OriginalDeptLocation);
            }
        }
        //▼===== #002
        #region Cảnh báo khi xuất khoa mà có chỉ định hoặc phiếu xuất chưa tạo bill.
        private string _errorMessages;
        public string ErrorMessages
        {
            get { return _errorMessages; }
            set
            {
                _errorMessages = value;
                NotifyOfPropertyChange(() => ErrorMessages);
            }
        }

        private string _confirmMessages;
        public string ConfirmMessages
        {
            get { return _confirmMessages; }
            set
            {
                _confirmMessages = value;
                NotifyOfPropertyChange(() => ConfirmMessages);
            }
        }
        WarningWithConfirmMsgBoxTask errorMessageBox = null;
        public IEnumerator<IResult> CheckValid_New()
        {
            yield return GenericCoRoutineTask.StartTask(CheckBeforeChangeDept_Action);

            if (!string.IsNullOrEmpty(ErrorMessages))
            {
                ErrorMessages = string.Format("{0}: ", eHCMSResources.Z2804_G1_BNKgTheXK) + Environment.NewLine + ErrorMessages;

                errorMessageBox = new WarningWithConfirmMsgBoxTask(ErrorMessages, eHCMSResources.Z2827_G1_DongYXuatKhoa, true);
                yield return errorMessageBox;
                if (errorMessageBox.IsAccept)
                {
                    curInPatientTransferDeptReq = CreateInPatientTransferDeptReq();
                    InsertInPatientTransferDeptReq(curInPatientTransferDeptReq);
                }
                yield break;
            }
            curInPatientTransferDeptReq = CreateInPatientTransferDeptReq();
            InsertInPatientTransferDeptReq(curInPatientTransferDeptReq);
        }
        private void CheckBeforeChangeDept_Action(GenericCoRoutineTask genTask)
        {

            ErrorMessages = "";
            ConfirmMessages = "";

            string errorMsg = "";
            string confirmMsg = "";

            this.DlgShowBusyIndicator();

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        bool bContinue = true;

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginCheckBeforeChangeDept(CurrentAdmission.PtRegistrationID, StartDeptID
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var result = contract.EndCheckBeforeChangeDept(out errorMsg, out confirmMsg, asyncResult);

                                    if (result)
                                    {
                                        ErrorMessages = errorMsg;
                                        ConfirmMessages = confirmMsg;
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z2826_G1_InfoKTraXKFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        bContinue = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                    ClientLoggerHelper.LogError(ex.Message);
                                    bContinue = false;
                                    this.DlgHideBusyIndicator();
                                }
                                finally
                                {
                                    if (genTask != null)
                                    {
                                        genTask.ActionComplete(bContinue);
                                    }
                                    this.DlgHideBusyIndicator();
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);

                    if (genTask != null)
                    {
                        genTask.ActionComplete(false);
                    }
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();

        }
        #endregion
        //▲===== #002
        private bool _UseOnlyDailyDiagnosis = Globals.ServerConfigSection.ConsultationElements.UseOnlyDailyDiagnosis;
        public bool UseOnlyDailyDiagnosis
        {
            get
            {
                return _UseOnlyDailyDiagnosis;
            }
            set
            {
                if (_UseOnlyDailyDiagnosis == value)
                {
                    return;
                }
                _UseOnlyDailyDiagnosis = value;
                NotifyOfPropertyChange(() => UseOnlyDailyDiagnosis);
            }
        }
        private DiagnosisTreatment CurrentDiagnosisTreatment { get; set; }
        public void ConfirmDiagnosisTreatmentCmd()
        {
            //20191104 TBL: Kiểm tra khoa đang nằm với khoa chuyển đi có giống nhau không
            if (CurrentDeptID != StartDeptID)
            {
                MessageBox.Show(eHCMSResources.Z2901_G1_BNKhongNamTrongKhoaChuyenDi, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            CurrentDiagnosisTreatment = null;
            IConfirmDiagnosisTreatment DialogView = Globals.GetViewModel<IConfirmDiagnosisTreatment>();
            DialogView.ApplyDiagnosisTreatmentCollection(DiagnosisTreatmentCollection);
            DialogView.DeptID = CurrentDeptID;
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, new Size(1200, 600));
            CurrentDiagnosisTreatment = DialogView.CurrentDiagnosisTreatment;
        }
        
        private IList<DiagnosisTreatment> _DiagnosisTreatmentCollection;
        public IList<DiagnosisTreatment> DiagnosisTreatmentCollection
        {
            get { return _DiagnosisTreatmentCollection; }
            set
            {
                if(_DiagnosisTreatmentCollection != value)
                {
                    _DiagnosisTreatmentCollection = value;
                    NotifyOfPropertyChange(() => DiagnosisTreatmentCollection);
                    //20200426 TBL: Tự động lấy chẩn đoán cuối cùng làm chẩn đoán xuất khoa
                    if (UseOnlyDailyDiagnosis && DiagnosisTreatmentCollection.Count > 0)
                    {
                        CurrentDiagnosisTreatment = DiagnosisTreatmentCollection.ToObservableCollection().OrderBy(x => x.DTItemID).LastOrDefault();
                    }
                }
            }
        }
        private bool _IsChangedDept = false;
        public bool IsChangedDept
        {
            get
            {
                return _IsChangedDept;
            }
            set
            {
                _IsChangedDept = value;
                NotifyOfPropertyChange(() => IsChangedDept);
            }
        }
        private bool _IsProgressive = false;
        public bool IsProgressive
        {
            get
            {
                return _IsProgressive;
            }
            set
            {
                _IsProgressive = value;
                NotifyOfPropertyChange(() => IsProgressive);
            }
        }

        //▼==== #003
        private bool _ShowProgressive = false;
        public bool ShowProgressive
        {
            get
            {
                return _ShowProgressive;
            }
            set
            {
                _ShowProgressive = value;
                NotifyOfPropertyChange(() => ShowProgressive);
            }
        }
        //▲==== #003
    }
}