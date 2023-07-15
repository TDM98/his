using aEMR.Common;
using aEMR.Common.BaseModel;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace aEMR.Consultation.ViewModels
{
    [Export(typeof(IAdmissionCriterion_PCLResult)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AdmissionCriterion_PCLResultViewModel : ViewModelBase, IAdmissionCriterion_PCLResult
    {
        [ImportingConstructor]
        public AdmissionCriterion_PCLResultViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        private long _AdmissionCriterionID;
        public long AdmissionCriterionID
        {
            get { return _AdmissionCriterionID; }
            set
            {
                if (_AdmissionCriterionID != value)
                {
                    _AdmissionCriterionID = value;
                    NotifyOfPropertyChange(() => AdmissionCriterionID);
                }
            }
        }
        private long _PtRegistrationID;
        public long PtRegistrationID
        {
            get { return _PtRegistrationID; }
            set
            {
                if (_PtRegistrationID != value)
                {
                    _PtRegistrationID = value;
                    NotifyOfPropertyChange(() => PtRegistrationID);
                }
            }
        }
        private string _PatientCode;
        public string PatientCode
        {
            get { return _PatientCode; }
            set
            {
                if (_PatientCode != value)
                {
                    _PatientCode = value;
                    NotifyOfPropertyChange(() => PatientCode);
                }
            }
        }
        private ObservableCollection<GroupPCLs> _allGroupPCLs;
        public ObservableCollection<GroupPCLs> allGroupPCLs
        {
            get
            {
                return _allGroupPCLs;
            }
            set
            {
                if (_allGroupPCLs == value)
                    return;
                _allGroupPCLs = value;
                NotifyOfPropertyChange(() => allGroupPCLs);
            }
        }

        private GroupPCLs _tempSelectedPCL;
        public GroupPCLs tempSelectedPCL
        {
            get
            {
                return _tempSelectedPCL;
            }
            set
            {
                if (_tempSelectedPCL == value)
                    return;
                _tempSelectedPCL = value;
                NotifyOfPropertyChange(() => allGroupPCLs);
            }
        }
      
        public void InitInfo()
        {
            GetGroupPCLListByAdmissionCriterionID(AdmissionCriterionID,PtRegistrationID);
        }
       
        private void GetGroupPCLListByAdmissionCriterionID(long CriterionID,long PtRegistrationID)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGroupPCL_PCLExamType_ByAdmissionCriterionID(CriterionID, PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {

                            IList<GroupPCLs> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGroupPCL_PCLExamType_ByAdmissionCriterionID(asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }

                            //allAdmissionCriterionAttachGroupPCL.Clear();
                            allGroupPCLs = new ObservableCollection<GroupPCLs>();
                            if (bOK)
                            {
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        foreach (var PCLExamType in item.PCLExamTypeItem)
                                        {
                                            PCLExamType.IsSelected = PCLExamType.IsSelected == null ? false: PCLExamType.IsSelected;
                                        }
                                        allGroupPCLs.Add(item);
                                    }
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }
        public void btnChoseFile()
        {
            if (CurrentPCLResult == null)
            {
                MessageBox.Show("Chọn CLS cần lưu kết quả");
                return;
            }

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            if (V_ResultType == (long)AllLookupValues.FileStorageResultType.IMAGES)
            {
                dlg.Filter = "Images (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*";
            }
            else
            {
                dlg.Filter = "Pdf Files|*.pdf";
            }

            bool? retval = dlg.ShowDialog();

            if (retval != null && retval == true )
            {
                Stream stream = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read);
                bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                CurrentPCLResult.V_ResultType = V_ResultType;
                //CurrentPCLResult.ImageResultUrl = dlg.FileName;
                CurrentPCLResult.ImageResultUrl = PatientCode + Guid.NewGuid().ToString() + Path.GetExtension(dlg.FileName);
                CurrentPCLResult.File = bytes;
            }
        }
        byte[] bytes;
        private string _ImagePool;
        public string ImagePool
        {
            get
            {
                return _ImagePool;
            }
            set
            {
                if (_ImagePool == value)
                    return;
                _ImagePool = value;
                NotifyOfPropertyChange(() => ImagePool);
            }
        }
        public void btnSave()
        {
            if(CurrentPCLResult == null)
            {
                MessageBox.Show("Chưa chọn CLS. Vui lòng chọn CLS và file trước khi lưu");
                return;
            }
            if (CurrentPCLResult.File == null)
            {
                MessageBox.Show("Chưa chọn File. Vui lòng chọn CLS và file trước khi lưu");
                return;
            }
            SaveAdmissionCriterionDetail_PCLResult();
        }
        public void ckbPCL_Checked(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox))
            {
                return;
            }
            CheckBox ckbPCL = sender as CheckBox;
            bool? copierChecked = ckbPCL.IsChecked;
            if(CurrentPCLResult == null)
            {
                CurrentPCLResult = new AdmissionCriterionDetail_PCLResult
                {
                    PtRegistrationID = PtRegistrationID,
                    PCLExamTypeID = (ckbPCL.DataContext as PCLExamType).PCLExamTypeID,
                    CreatedStaffID = Globals.LoggedUserAccount.Staff.StaffID,
                    CreatedDate = Globals.GetCurServerDateTime()
                };
            }
            else
            {
                MessageBox.Show("Đã chọn CLS vui lòng chọn file cho CLS đang chọn trước khi thêm CLS khác");
                ckbPCL.IsChecked = !(bool)copierChecked;
            }
        }
        public void ckbPCL_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckBox))
            {
                return;
            }
            CheckBox ckbPCL = sender as CheckBox;
            PCLExamType examType = ckbPCL.DataContext as PCLExamType;
            bool? copierChecked = ckbPCL.IsChecked;
            if (CurrentPCLResult != null && examType.PCLExamTypeID == CurrentPCLResult.PCLExamTypeID)
            {
                CurrentPCLResult = null;
            }
            //else
            //{
            //    ckbPCL.IsChecked = !(bool)copierChecked;
            //}
        }
        private AdmissionCriterionDetail_PCLResult _CurrentPCLResult;
        public AdmissionCriterionDetail_PCLResult CurrentPCLResult
        {
            get
            {
                return _CurrentPCLResult;
            }
            set
            {
                if (_CurrentPCLResult == value)
                    return;
                _CurrentPCLResult = value;
                NotifyOfPropertyChange(() => CurrentPCLResult);
            }
        }
        private long _V_ResultType = (long)AllLookupValues.FileStorageResultType.IMAGES;
        public long V_ResultType
        {
            get { return _V_ResultType; }
            set
            {
                if (_V_ResultType != value)
                {
                    _V_ResultType = value;
                    NotifyOfPropertyChange(() => V_ResultType);
                }
            }
        }
        public void gOption0_Click(object sender, RoutedEventArgs e)
        {
            var Ctr = (sender as RadioButton);
            if (Ctr.IsChecked.Value)
            {
                V_ResultType = (long)AllLookupValues.FileStorageResultType.IMAGES;
            }
        }

        public void gOption1_Click(object sender, RoutedEventArgs e)
        {
            var Ctr = (sender as RadioButton);
            if (Ctr.IsChecked.Value)
            {
                V_ResultType = (long)AllLookupValues.FileStorageResultType.DOCUMENTS;
            }
        }
        private void SaveAdmissionCriterionDetail_PCLResult()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSaveAdmissionCriterionDetail_PCLResult(CurrentPCLResult, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var Result = contract.EndSaveAdmissionCriterionDetail_PCLResult(asyncResult);
                            if (Result)
                            {
                                CurrentPCLResult = null;
                                GetGroupPCLListByAdmissionCriterionID(AdmissionCriterionID, PtRegistrationID);
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            // Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
    }
}
