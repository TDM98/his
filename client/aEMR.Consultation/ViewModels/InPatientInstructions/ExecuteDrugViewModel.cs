using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using System.Threading;
using aEMR.ServiceClient;
using CommonServiceProxy;
using aEMR.Infrastructure;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Castle.Windsor;
using eHCMSLanguage;
using aEMR.Controls;
using System.Linq;
using aEMR.Common;
using System.ServiceModel;
using aEMR.DataContracts;
using DataEntities.MedicalInstruction;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure.Events;

/*
 * 20190816 #001 TBL: BM 0013175: Lưu Thông tin theo dõi theo y lệnh vào bảng mới.
 * 20191109 #002 TTM: BM 0017401: [Y lệnh] Bổ sung ComboBox thời gian theo dõi y lệnh.
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IExecuteDrug)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ExecuteDrugViewModel : ViewModelBase, IExecuteDrug
    {
        private long _ExecuteDrugID = 0;
        public long ExecuteDrugID
        {
            get
            {
                return _ExecuteDrugID;
            }
            set
            {
                _ExecuteDrugID = value;
                NotifyOfPropertyChange(() => ExecuteDrugID);
            }
        }

        private long _ExecuteDrugDetailID = 0;
        public long ExecuteDrugDetailID
        {
            get
            {
                return _ExecuteDrugDetailID;
            }
            set
            {
                _ExecuteDrugDetailID = value;
                NotifyOfPropertyChange(() => ExecuteDrugDetailID);
            }
        }

        private long _StaffID = 0;
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                _StaffID = value;
                NotifyOfPropertyChange(() => StaffID);
            }
        }

        private IMinHourDateControl _DateExcuteContent;
        public IMinHourDateControl DateExcuteContent
        {
            get { return _DateExcuteContent; }
            set
            {
                _DateExcuteContent = value;
                NotifyOfPropertyChange(() => DateExcuteContent);
            }
        }

        [ImportingConstructor]
        public ExecuteDrugViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            DateExcuteContent = Globals.GetViewModel<IMinHourDateControl>();
            DateExcuteContent.DateTime = Globals.GetCurServerDateTime();
            LoadDoctorStaffCollection();
        }

        private Staff _gSelectedDoctorStaff;
        public Staff gSelectedDoctorStaff
        {
            get
            {
                return _gSelectedDoctorStaff;
            }
            set
            {
                if (_gSelectedDoctorStaff == value) return;
                _gSelectedDoctorStaff = value;
                NotifyOfPropertyChange(() => gSelectedDoctorStaff);
            }
        }
        public void DoctorStaff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void DoctorStaff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            gSelectedDoctorStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
        }

        private ObservableCollection<Staff> _DoctorStaffs;
        public ObservableCollection<Staff> DoctorStaffs
        {
            get
            {
                return _DoctorStaffs;
            }
            set
            {
                if (_DoctorStaffs != value)
                {
                    _DoctorStaffs = value;
                    NotifyOfPropertyChange(() => DoctorStaffs);
                }
            }
        }

        private void LoadDoctorStaffCollection()
        {
            long CurrentDeptID = Globals.DeptLocation.DeptID;
            if (Globals.ServerConfigSection.ConsultationElements.FilterDoctorByDeptResponsibilitiesInPt && !Globals.IsUserAdmin)
            {
                DoctorStaffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => (x.StaffCatgID == (long)StaffCatg.DieuDuong || x.StaffCatgID == (long)StaffCatg.NvHanhChanh
                                                                                    || (x.RefStaffCategory != null && (x.RefStaffCategory.V_StaffCatType == (long)V_StaffCatType.BacSi)))
                                                                                    && x.ListDeptResponsibilities != null && ((x.ListDeptResponsibilities.Contains(CurrentDeptID.ToString()) || CurrentDeptID == 0))
                                                                                    && (!x.IsStopUsing)).ToList());
                if (StaffID == 0)
                {
                    gSelectedDoctorStaff = DoctorStaffs.FirstOrDefault(x => x.StaffID == Globals.LoggedUserAccount.StaffID);
                }
                else
                {
                    gSelectedDoctorStaff = DoctorStaffs.FirstOrDefault(x => x.StaffID == StaffID);
                }
            }
            else
            {
                DoctorStaffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => (x.StaffCatgID == (long)StaffCatg.DieuDuong || x.StaffCatgID == (long)StaffCatg.NvHanhChanh
                                                                                    || (x.RefStaffCategory != null && (x.RefStaffCategory.V_StaffCatType == (long)V_StaffCatType.BacSi)))
                                                                                    && (!x.IsStopUsing)).ToList());
                if (StaffID == 0)
                {
                    gSelectedDoctorStaff = DoctorStaffs.FirstOrDefault(x => x.StaffID == Globals.LoggedUserAccount.StaffID);
                }
                else
                {
                    gSelectedDoctorStaff = DoctorStaffs.FirstOrDefault(x => x.StaffID == StaffID);
                }
            }
        }

        public void SaveCmd()
        {
            this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSaveExecuteDrug(ExecuteDrugID
                            , ExecuteDrugDetailID
                            , gSelectedDoctorStaff.StaffID
                            , Globals.LoggedUserAccount.StaffID.Value
                            , DateExcuteContent.DateTime.GetValueOrDefault()
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool results = contract.EndSaveExecuteDrug(asyncResult);
                                if (results)
                                {
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                    Globals.EventAggregator.Publish(new SaveExecuteDrugCompleted());
                                }
                                else
                                {
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.Z0477_G1_LuuKhongThanhCong);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                                TryClose();
                            }
                        }),
                        null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        private Dictionary<string, string> _ShortHandDictionaryObj;
        public Dictionary<string, string> ShortHandDictionaryObj
        {
            get => _ShortHandDictionaryObj; set
            {
                _ShortHandDictionaryObj = value;
                NotifyOfPropertyChange(() => ShortHandDictionaryObj);
            }
        }
    }
}
