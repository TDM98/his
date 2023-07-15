using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common;
using aEMR.Common.BaseModel;
using eHCMSLanguage;
using Castle.Windsor;
using aEMR.ViewContracts;

namespace aEMR.Configuration.PatientApptLocTargets.ViewModels
{
    [Export(typeof(IPatientApptLocTargets)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientApptLocTargetsViewModel : ViewModelBase, IPatientApptLocTargets
        , IHandle<RefDepartments_BystrV_DeptTypeEvent.TreeSelectionChanged_Event>
    {
        private object _leftContent;
        public object leftContent
        {
            get
            {
                return _leftContent;
            }
            set
            {
                if (_leftContent == value)
                    return;
                _leftContent = value;
                NotifyOfPropertyChange(() => leftContent);
            }
        }
        [ImportingConstructor]
        public PatientApptLocTargetsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);

            CreateNew();
            

            //Load UC
            var UCRefDepartments_BystrV_DeptTypeViewModel = Globals.GetViewModel<IRefDepartments_BystrV_DeptType>();
            UCRefDepartments_BystrV_DeptTypeViewModel.strV_DeptType = "7000";
            UCRefDepartments_BystrV_DeptTypeViewModel.ShowDeptLocation = true;
            UCRefDepartments_BystrV_DeptTypeViewModel.Parent = this;
            UCRefDepartments_BystrV_DeptTypeViewModel.RefDepartments_Tree();
            leftContent = UCRefDepartments_BystrV_DeptTypeViewModel;
            //(this as Conductor<object>).ActivateItem(leftContent);
            ActivateItem(leftContent);
            //Load UC

            FormEditor_Status(false,false,false);
      
        }


        //
        public override bool IsProcessing
        {
            get
            {
                return _IsLoadingSegments
                    ||_IsWaitingPatientApptLocTargetsByDepartmentLocID
                    || _IsWaitingSave
                    ||_IsWaitingDelete;
            }
        }

        public override string StatusText
        {
            get
            {
                if (_IsLoadingSegments)
                {
                    return "Danh sách Ca";
                }
                if(_IsWaitingPatientApptLocTargetsByDepartmentLocID)
                {
                    return "Thông tin chi tiết phòng - ca";
                }
                if(_IsWaitingSave)
                {
                    return "Đang lưu";
                }
                if(_IsWaitingDelete)
                {
                    return "Đang xóa";
                }

                return string.Empty;
            }
        }
        

        private bool _IsLoadingSegments;
        public bool IsLoadingSegments
        {
            get { return _IsLoadingSegments; }
            set
            {
                if (_IsLoadingSegments != value)
                {
                    _IsLoadingSegments = value;
                    NotifyOfPropertyChange(() => IsLoadingSegments);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsWaitingSave;
        public bool IsWaitingSave
        {
            get { return _IsWaitingSave; }
            set
            {
                if (_IsWaitingSave != value)
                {
                    _IsWaitingSave = value;
                    NotifyOfPropertyChange(() => IsWaitingSave);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsWaitingDelete;
        public bool IsWaitingDelete
        {
            get { return _IsWaitingDelete; }
            set
            {
                if (_IsWaitingDelete != value)
                {
                    _IsWaitingDelete = value;
                    NotifyOfPropertyChange(() => IsWaitingDelete);
                    NotifyWhenBusy();
                }
            }
        }


        private ObservableCollection<ConsultationTimeSegments> _ConsultationTimeSegmentsList;
        public ObservableCollection<ConsultationTimeSegments> ConsultationTimeSegmentsList
        {
            get
            {
                return _ConsultationTimeSegmentsList;
            }
            set
            {
                if (_ConsultationTimeSegmentsList != value)
                {
                    _ConsultationTimeSegmentsList = value;
                    NotifyOfPropertyChange(() => ConsultationTimeSegmentsList);
                }
            }
        }


        private bool _IsWaitingPatientApptLocTargetsByDepartmentLocID;
        public bool IsWaitingPatientApptLocTargetsByDepartmentLocID
        {
            get { return _IsWaitingPatientApptLocTargetsByDepartmentLocID; }
            set
            {
                if (_IsWaitingPatientApptLocTargetsByDepartmentLocID != value)
                {
                    _IsWaitingPatientApptLocTargetsByDepartmentLocID = value;
                    NotifyOfPropertyChange(() => IsWaitingPatientApptLocTargetsByDepartmentLocID);
                    NotifyWhenBusy();
                }
            }
        }

        private ObservableCollection<DataEntities.PatientApptLocTargets> _PatientApptLocTargetsList;
        public ObservableCollection<DataEntities.PatientApptLocTargets> PatientApptLocTargetsList
        {
            get
            {
                return _PatientApptLocTargetsList;
            }
            set
            {
                if (_PatientApptLocTargetsList != value)
                {
                    _PatientApptLocTargetsList = value;
                    NotifyOfPropertyChange(() => PatientApptLocTargetsList);
                }
            }
        }

        private void ConsultationTimeSegments_ByDeptLocationID(long DeptLocationID)
        {
            IsLoadingSegments = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ClinicManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginConsultationTimeSegments_ByDeptLocationID(DeptLocationID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndConsultationTimeSegments_ByDeptLocationID(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                ConsultationTimeSegmentsList = new ObservableCollection<ConsultationTimeSegments>(results);

                                //Default
                                ConsultationTimeSegments firstItem = new ConsultationTimeSegments();
                                firstItem.ConsultationTimeSegmentID = -1;
                                firstItem.SegmentName = eHCMSResources.A0015_G1_Chon;
                                DateTime gio = new DateTime(1900, 01, 01, 0, 0, 0);
                                firstItem.StartTime = gio;
                                firstItem.EndTime = gio;

                                ConsultationTimeSegmentsList.Insert(0, firstItem);
                                //Default

                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsLoading = false;
                            IsLoadingSegments = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }


        private RefDepartmentsTree _ObjTreeNodeRefDepartments_Current = new RefDepartmentsTree();
        public RefDepartmentsTree ObjTreeNodeRefDepartments_Current
        {
            get { return _ObjTreeNodeRefDepartments_Current; }
            set
            {
                _ObjTreeNodeRefDepartments_Current = value;
                NotifyOfPropertyChange(() => ObjTreeNodeRefDepartments_Current);
                NotifyOfPropertyChange(() => TextKhoa);
                NotifyOfPropertyChange(() => TextPhong);
            }
        }

        
        public string TextKhoa
        {
            get
            {
                if (ObjTreeNodeRefDepartments_Current.IsDeptLocation)
                {
                    return ObjTreeNodeRefDepartments_Current.Parent.NodeText;
                }
                else
                {
                    return "";
                }
            }
        }

        public string TextPhong
        {
            get
            {
                if (ObjTreeNodeRefDepartments_Current.IsDeptLocation)
                {
                    return ObjTreeNodeRefDepartments_Current.NodeText;
                }
                else
                {
                    return "";
                }
            }
        }

        public void Handle(RefDepartments_BystrV_DeptTypeEvent.TreeSelectionChanged_Event message)
        {
            if (message != null)
            {
                CreateNew();

                ObjTreeNodeRefDepartments_Current = message.ObjRefDepartments_Current as DataEntities.RefDepartmentsTree;
                
                long DeptID = ObjTreeNodeRefDepartments_Current.NodeID;

                if (ObjTreeNodeRefDepartments_Current.IsDeptLocation)
                {
                    ConsultationTimeSegments_ByDeptLocationID(DeptID);

                    PatientApptLocTargetsByDepartmentLocID(DeptID);

                    FormEditor_Status(true,false,true);
                }
                else
                {
                    FormEditor_Status(false, false, false);
                }

            }
        }

        
        private void PatientApptLocTargetsByDepartmentLocID(long DepartmentLocID)
        {
            IsWaitingPatientApptLocTargetsByDepartmentLocID = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new AppointmentServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPatientApptLocTargetsByDepartmentLocID(DepartmentLocID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndPatientApptLocTargetsByDepartmentLocID(asyncResult);
                            
                             PatientApptLocTargetsList = new ObservableCollection<DataEntities.PatientApptLocTargets>(results);
                       
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsLoading = false;
                            IsWaitingPatientApptLocTargetsByDepartmentLocID = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }


        private bool _btCreateIsEnabled;
        public bool btCreateIsEnabled
        {
            get { return _btCreateIsEnabled; }
            set
            {
                if(_btCreateIsEnabled!=value)
                {
                    _btCreateIsEnabled = value;
                    NotifyOfPropertyChange(() => btCreateIsEnabled);
                }
            }
        }

        private bool _btSaveAddNewIsEnabled;
        public bool btSaveAddNewIsEnabled
        {
            get { return _btSaveAddNewIsEnabled; }
            set
            {
                if (_btSaveAddNewIsEnabled != value)
                {
                    _btSaveAddNewIsEnabled = value;
                    NotifyOfPropertyChange(() => btSaveAddNewIsEnabled);
                }
            }
        }

        private bool _btUpdateIsEnabled;
        public bool btUpdateIsEnabled
        {
            get { return _btUpdateIsEnabled; }
            set
            {
                if (_btUpdateIsEnabled != value)
                {
                    _btUpdateIsEnabled = value;
                    NotifyOfPropertyChange(() => btUpdateIsEnabled);
                }
            }
        }

        private bool _btCancelIsEnabled;
        public bool btCancelIsEnabled
        {
            get { return _btCancelIsEnabled; }
            set
            {
                if (_btCancelIsEnabled != value)
                {
                    _btCancelIsEnabled = value;
                    NotifyOfPropertyChange(() => btCancelIsEnabled);
                }
            }
        }

        private bool _EditorIsEnabled;
        public bool EditorIsEnabled
        {
            get { return _EditorIsEnabled; }
            set
            {
                if (_EditorIsEnabled != value)
                {
                    _EditorIsEnabled = value;
                    NotifyOfPropertyChange(() => EditorIsEnabled);
                }
            }
        }


        private void FormEditor_Status(bool isReadyAddNew, bool isReadyEdit, bool isDepLocation)
        {
            EditorIsEnabled = false;

            if (isDepLocation==false)
            {
                btCreateIsEnabled = false;
                btSaveAddNewIsEnabled = false;
                btUpdateIsEnabled = false;
                btCancelIsEnabled = false;
            }
            else
            {
                if (isReadyAddNew)
                {
                    btCreateIsEnabled = true;
                    btSaveAddNewIsEnabled = false;
                    btUpdateIsEnabled = false;
                    btCancelIsEnabled = false;
                }
                if (isReadyEdit)
                {
                    EditorIsEnabled = true;

                    btCreateIsEnabled = false;
                    btSaveAddNewIsEnabled = false;
                    btUpdateIsEnabled = true;
                    btCancelIsEnabled = true;
                }
            }

        }


        #region "Button Click"

        private void SetValueBeforeSave()
        {
            ObjPatientApptLocTargetsCurrent.ObjDepartmentLocID.DeptLocationID = ObjTreeNodeRefDepartments_Current.NodeID;
        }

        private void CreateNew()
        {
            ObjPatientApptLocTargetsCurrent = new DataEntities.PatientApptLocTargets();
            ObjPatientApptLocTargetsCurrent.ObjApptTimeSegmentID = new ConsultationTimeSegments();
            ObjPatientApptLocTargetsCurrent.ObjApptTimeSegmentID.ConsultationTimeSegmentID = -1;

            ObjPatientApptLocTargetsCurrent.ObjDepartmentLocID = new DeptLocation();
        }

        public void btCreate()
        {
            CreateNew();

            EditorIsEnabled = true;

            btCreateIsEnabled = false;
            btSaveAddNewIsEnabled = true;
            btUpdateIsEnabled = false;
            btCancelIsEnabled = true;
        }

        private bool CheckValid()
        {
            if (ObjPatientApptLocTargetsCurrent.ObjApptTimeSegmentID.ConsultationTimeSegmentID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0303_G1_Msg_InfoChonCaKham);
                return false;
            }

            if(string.IsNullOrEmpty(ObjPatientApptLocTargetsCurrent.NumberOfAppt.ToString()))
            {
                MessageBox.Show(eHCMSResources.A0879_G1_Msg_InfoNhapSLg);
                return false;
            }
            else
            {
                Int16 SL = 0;
                Int16.TryParse(ObjPatientApptLocTargetsCurrent.NumberOfAppt.ToString(), out SL);
                if(SL<=0)
                {
                    MessageBox.Show("Số Lượng Không Hợp Lệ!");
                    return false;
                }
            }

            Int16 StartNum = 0;
            Int16 EndNum = 0;
            Int16.TryParse(ObjPatientApptLocTargetsCurrent.StartSequenceNumber.ToString(), out StartNum);
            Int16.TryParse(ObjPatientApptLocTargetsCurrent.EndSequenceNumber.ToString(), out EndNum);

            if(StartNum<=0)
            {
                MessageBox.Show(eHCMSResources.A0211_G1_Msg_InfoBDKhHopLe);
                return false;
            }

            if (EndNum <= 0)
            {
                MessageBox.Show(eHCMSResources.A0617_G1_Msg_InfoKetThucKhHopLe);
                return false;
            }

            if(StartNum>EndNum)
            {
                MessageBox.Show(eHCMSResources.A0212_G1_Msg_InfoBDNhoHonKT);
                return false;
            }

            if((Math.Abs((StartNum-EndNum)) + 1)!=ObjPatientApptLocTargetsCurrent.NumberOfAppt)
            {
                MessageBox.Show(eHCMSResources.A0210_G1_Msg_InfoBD_KT_SLgKhongPhuHop);
                return false;
            }

            return true;
        }

        public void btSaveAddNew()
        {
           if(CheckValid()==false)
               return;

            SetValueBeforeSave();

            PatientApptLocTargets_Save();
        }

        public void btUpdate()
        {
            if (CheckValid() == false)
                return;

            SetValueBeforeSave();

            PatientApptLocTargets_Save();
        }

        public void btCancel()
        {
            EditorIsEnabled = false;

            btCreateIsEnabled = true;
            btSaveAddNewIsEnabled = false;
            btUpdateIsEnabled = false;
            btCancelIsEnabled = false;
        }

        private DataEntities.PatientApptLocTargets _ObjPatientApptLocTargetsCurrent;
        public DataEntities.PatientApptLocTargets ObjPatientApptLocTargetsCurrent
        {
            get { return _ObjPatientApptLocTargetsCurrent; }
            set
            {
                if (_ObjPatientApptLocTargetsCurrent != value)
                {
                    _ObjPatientApptLocTargetsCurrent = value;
                    NotifyOfPropertyChange(() => ObjPatientApptLocTargetsCurrent);
                }
            }

        }

        private void PatientApptLocTargets_Save()
        {
            IsWaitingSave = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new AppointmentServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPatientApptLocTargets_Save(ObjPatientApptLocTargetsCurrent, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndPatientApptLocTargets_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "ApptTimeSegmentID-Had-SetUp":
                                    {
                                        MessageBox.Show(TextPhong + ObjPatientApptLocTargetsCurrent.ObjApptTimeSegmentID.SegmentName + string.Format(" {0}.", eHCMSResources.G0020_G1_DaDuocCHinhRoi), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        FormEditor_Status(false, false, true);

                                        CreateNew();

                                        PatientApptLocTargetsByDepartmentLocID(ObjTreeNodeRefDepartments_Current.NodeID);

                                        EditorIsEnabled = false;

                                        btCreateIsEnabled = true;
                                        btSaveAddNewIsEnabled = false;
                                        btUpdateIsEnabled = false;
                                        btCancelIsEnabled = false;


                                        MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Add-1":
                                    {
                                        FormEditor_Status(false, false, true);

                                        CreateNew();

                                        PatientApptLocTargetsByDepartmentLocID(ObjTreeNodeRefDepartments_Current.NodeID);

                                        EditorIsEnabled = false;

                                        btCreateIsEnabled = true;
                                        btSaveAddNewIsEnabled = false;
                                        btUpdateIsEnabled = false;
                                        btCancelIsEnabled = false;


                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Add-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0701_G1_Msg_InfoLuuFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        break;
                                    }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsWaitingSave = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }


        private void PatientApptLocTargets_Delete(long PatientApptTargetID)
        {
            IsWaitingDelete = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new AppointmentServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPatientApptLocTargets_Delete(PatientApptTargetID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool Res=contract.EndPatientApptLocTargets_Delete(asyncResult);

                            if(Res)
                            {
                                PatientApptLocTargetsByDepartmentLocID(ObjTreeNodeRefDepartments_Current.NodeID);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsWaitingDelete = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        public void hplDelete_Click(object selectedItem)
        {
            DataEntities.PatientApptLocTargets p = (selectedItem as DataEntities.PatientApptLocTargets);

            if (p != null)
            {
                if (p.PatientApptTargetID > 0)
                {
                    if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.ObjApptTimeSegmentID.SegmentName), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        PatientApptLocTargets_Delete(p.PatientApptTargetID);
                    }
                }
            }
        }

        public void hplEdit_Click(object selectedItem)
        {
            DataEntities.PatientApptLocTargets p = (selectedItem as DataEntities.PatientApptLocTargets);

            FormEditor_Status(false,true,true);

            ObjPatientApptLocTargetsCurrent = p.DeepCopy();
        }

        public void tbStart_KeyUp(object sender, KeyEventArgs e)
        {
            string v = (sender as TextBox).Text;
            if(!string.IsNullOrEmpty(v))
            {
                int num = 0;
                int.TryParse(v, out num);

                if (ObjPatientApptLocTargetsCurrent != null && ObjPatientApptLocTargetsCurrent.NumberOfAppt > 0)
                {
                    ObjPatientApptLocTargetsCurrent.EndSequenceNumber = (short)((num + ObjPatientApptLocTargetsCurrent.NumberOfAppt) - 1);
                }

            }
            
        }

        public void tbNum_KeyUp(object sender, KeyEventArgs e)
        {
            string v = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(v))
            {
                int num = 0;
                int.TryParse(v, out num);

                if (ObjPatientApptLocTargetsCurrent != null && ObjPatientApptLocTargetsCurrent.StartSequenceNumber > 0)
                {
                    ObjPatientApptLocTargetsCurrent.EndSequenceNumber = (short)((ObjPatientApptLocTargetsCurrent.StartSequenceNumber + num) - 1);
                }

            }


        }

        #endregion
    }
}
