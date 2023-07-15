using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using aEMR.CommonTasks;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.BaseModel;
using aEMR.Common;
using eHCMSLanguage;
using Castle.Windsor;
using System.Windows.Controls;
using aEMR.Controls;
using System.Windows.Interop;
using System.Runtime.InteropServices;

/*
 * 20191212 #001 TBL: BM 0019723: Trong cùng 1 ekip chỉ có thể có 1 dịch vụ đầu tiên
 * 20200208 #002 TBL: BM 0022891: Khi thiết lập ekip phải có 1 dịch vụ đầu tiên
 * 20200218 #003 TBL: BM 0022891: Không cho tắt popup, chỉ tắt popup khi lưu
 */

namespace aEMR.Common.ViewModels
{
    [Export(typeof(ISetEkipForMedicalService)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SetEkipForMedicalServiceViewModel : ViewModelBase, ISetEkipForMedicalService

    {
        [ImportingConstructor]
        public SetEkipForMedicalServiceViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            eventArg.Subscribe(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            Lookup firstItem = new Lookup();
            firstItem.LookupID = 0;
            firstItem.ObjectValue = " ";
            RefEkip = new ObservableCollection<Lookup>();
            RefEkip.Insert(0, firstItem);
            RefEkip.Insert(1, Selected_Ekip);
            GetAllEkipIndex();
            GetAllEkip();
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        private bool _SaveOK = false;
        public bool SaveOK
        {
            get { return _SaveOK; }
            set
            {
                if(_SaveOK != value)
                {
                    _SaveOK = value;
                    NotifyOfPropertyChange(() => SaveOK);
                }
            }
        }

        private PatientRegistration _CurrentRegistration;
        public PatientRegistration CurrentRegistration
        {
            get { return _CurrentRegistration; }
            set
            {
                if(_CurrentRegistration != value)
                {
                    _CurrentRegistration = value;
                    NotifyOfPropertyChange(() => CurrentRegistration);
                }
            }
        }

        private ObservableCollection<Lookup> _RefEkip;
        public ObservableCollection<Lookup> RefEkip
        {
            get { return _RefEkip; }
            set
            {
                if (_RefEkip != value)
                {
                    _RefEkip = value;
                    NotifyOfPropertyChange(() => RefEkip);
                }
            }
        }

        private ObservableCollection<Lookup> _RefEkipIndex;
        public ObservableCollection<Lookup> RefEkipIndex
        {
            get { return _RefEkipIndex; }
            set
            {
                if (_RefEkipIndex != value)
                {
                    _RefEkipIndex = value;
                    NotifyOfPropertyChange(() => RefEkipIndex);
                }
            }
        }

        private Lookup _Selected_Ekip;
        public Lookup Selected_Ekip
        {
            get { return _Selected_Ekip; }
            set
            {
                if (_Selected_Ekip != value)
                {
                    _Selected_Ekip = value;
                    NotifyOfPropertyChange(() => Selected_Ekip);
                }
            }
        }

        private Lookup _Selected_EkipIndex;
        public Lookup Selected_EkipIndex
        {
            get { return _Selected_EkipIndex; }
            set
            {
                if (_Selected_EkipIndex != value)
                {
                    _Selected_EkipIndex = value;
                    NotifyOfPropertyChange(() => Selected_EkipIndex);
                }
            }
        }

        private ObservableCollection<PatientRegistrationDetail> _ObsPatientRegistrationDetail;
        public ObservableCollection<PatientRegistrationDetail> ObsPatientRegistrationDetail
        {
            get
            {
                return _ObsPatientRegistrationDetail;
            }
            set
            {
                if (_ObsPatientRegistrationDetail != value)
                {
                    _ObsPatientRegistrationDetail = value;
                    NotifyOfPropertyChange(() => ObsPatientRegistrationDetail);
                }
            }
        }

        public void GetAllEkip()
        {
            RefEkip = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.V_Ekip))
                {
                    RefEkip.Add(tmpLookup);
                }
            }
            Selected_Ekip = new Lookup();
            if (RefEkip != null && RefEkip.Count > 0)
            {
                Selected_Ekip = RefEkip[0];
            }
        }

        public void GetAllEkipIndex()
        {
            RefEkipIndex = new ObservableCollection<Lookup>();
            Lookup firstItem = new Lookup();
            firstItem.LookupID = 0;
            firstItem.ObjectValue = " ";
            RefEkipIndex = new ObservableCollection<Lookup>();
            RefEkipIndex.Insert(0, firstItem);
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.V_EkipIndex))
                {
                    RefEkipIndex.Add(tmpLookup);
                }
            }
        }

        public void cbxRefEkip_Loaded(object sender, RoutedEventArgs e)
        {
            var cbb = sender as KeyEnabledComboBox;
            if (cbb != null)
            {
                cbb.ItemsSource = RefEkip;
            }
        }

        public void cbxRefEkipIndex_Loaded(object sender, RoutedEventArgs e)
        {
            var cbb = sender as KeyEnabledComboBox;
            if (cbb != null)
            {
                cbb.ItemsSource = RefEkipIndex;
            }
        }

        public void SetEkip(ObservableCollection<PatientRegistrationDetail> ObsPatientRegistrationDetail)
        {
            if (ObsPatientRegistrationDetail != null)
            {
                foreach (PatientRegistrationDetail detail in ObsPatientRegistrationDetail)
                {
                    if (detail.V_EkipIndex != null && detail.V_EkipIndex.LookupID > 0)
                    {
                        detail.V_Ekip = Selected_Ekip;
                    }
                    else
                    {
                        detail.V_Ekip = null;
                    }
                }
            }
        }
        //▼====: #001
        private bool CheckEkipIndex(ObservableCollection<PatientRegistrationDetail> ObsPatientRegistrationDetail)
        {
            int count = 0;
            if (ObsPatientRegistrationDetail != null)
            {
                foreach (PatientRegistrationDetail detail in ObsPatientRegistrationDetail)
                {
                    if (detail.V_EkipIndex != null && detail.V_EkipIndex.LookupID == (long)AllLookupValues.V_EkipIndex.DauTien)
                    {
                        count++;
                    }
                }
            }
            if (count > 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        //▲====: #001
        //▼====: #002
        private bool CheckEkipFirst(ObservableCollection<PatientRegistrationDetail> ObsPatientRegistrationDetail)
        {
            int cFirst = 0;
            int cOther = 0;
            if (ObsPatientRegistrationDetail != null)
            {
                foreach (PatientRegistrationDetail detail in ObsPatientRegistrationDetail)
                {
                    if (detail.V_EkipIndex != null && detail.V_EkipIndex.LookupID == (long)AllLookupValues.V_EkipIndex.DauTien)
                    {
                        cFirst++;
                    }
                    else if (detail.V_EkipIndex != null && (detail.V_EkipIndex.LookupID == (long)AllLookupValues.V_EkipIndex.CungEkip || detail.V_EkipIndex.LookupID == (long)AllLookupValues.V_EkipIndex.KhacEkip))
                    {
                        cOther++;
                    }
                }
            }
            if (cFirst == 1 && cOther >= 0) //Chỉ cần có dịch vụ đầu tiên
            {
                return true;
            }
            else if (cFirst == 0 && cOther == 0) //Cách dịch vụ đều không được thêm ekip (trường hợp bỏ hết ekip ra khỏi dịch vụ)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //▲====: #002
        public void SaveSetEkip()
        {
            SetEkip(ObsPatientRegistrationDetail);
            //▼====: #001
            if (!CheckEkipIndex(ObsPatientRegistrationDetail))
            {
                MessageBox.Show(eHCMSResources.Z2941_G1_KhogLuuEkip, eHCMSResources.G0442_G1_TBao);
                return;
            }
            //▲====: #001
            //▼====: #002
            if (!CheckEkipFirst(ObsPatientRegistrationDetail))
            {
                MessageBox.Show(eHCMSResources.Z2980_G1_ErrorEkipIndex, eHCMSResources.G0442_G1_TBao);
                return;
            }
            //▲====: #002
            CurrentRegistration.PatientRegistrationDetails = ObsPatientRegistrationDetail;
            //20200511 TBL: Không lưu thông tin ekip theo hàm SetEkipForService nữa
            //SetEkipForService(CurrentRegistration);
            //20200530 TBL: Khi set ekip thì bắn sự kiện để nút Lưu và trả tiền hiện lên
            Globals.EventAggregator.Publish(new ChangeHIStatus<PatientRegistrationDetail>() { Item = ObsPatientRegistrationDetail[0] });
            SaveOK = true;
            TryClose();
        }

        public void SetEkipForService(PatientRegistration CurrentRegistration)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSetEkipForService(CurrentRegistration, Globals.DispatchCallback(asyncResult =>
                            {
                                bool Result = contract.EndSetEkipForService(asyncResult);
                                if (Result)
                                {
                                    SaveOK = true;
                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao);
                                    TryClose();
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.G0442_G1_TBao);
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

        public void cboEkip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cboEkip = sender as ComboBox;
            if (cboEkip != null)
            {
                ObsPatientRegistrationDetail = new ObservableCollection<PatientRegistrationDetail>(CurrentRegistration.PatientRegistrationDetails.Where(x => (x.V_Ekip != null && (x.V_Ekip.LookupID == Selected_Ekip.LookupID || x.V_Ekip.LookupID == 0) || x.V_Ekip == null)));
            }
        }
        //▼====: #003
        //private const uint MF_BYCOMMAND = 0x00000000;
        //private const uint MF_GRAYED = 0x00000001;
        //private const uint SC_CLOSE = 0xF060;
        //[DllImport("user32.dll")]
        //private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        //[DllImport("user32.dll")]
        //private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
        //public void SetEkip_Loaded(object view, RoutedEventArgs e)
        //{
        //    base.OnViewLoaded(view);
        //    var theWindow = (view as FrameworkElement).GetWindow();
        //    IntPtr hnwdSelPopup = new WindowInteropHelper(theWindow).Handle;

        //    if (hnwdSelPopup != null)
        //    {
        //        IntPtr menuHandle = GetSystemMenu(hnwdSelPopup, false);
        //        if (menuHandle != IntPtr.Zero)
        //        {
        //            EnableMenuItem(menuHandle, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
        //        }
        //    }
        //}
        //▲====: #003
    }
}