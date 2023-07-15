using eHCMSLanguage;
using aEMR.ServiceClient;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using DataEntities;
using System.Collections.ObjectModel;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Threading;
using System;
using System.Windows.Controls;
using aEMR.ServiceClient.Consultation_PCLs;
using System.Windows;
using aEMR.Controls;
using System.Linq;
using aEMR.Common.Collections;

/*
 * 20230318 #001 DatTB: Thêm biến lưu theo lần nhập viện
 */

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IInfectionControl)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InfectionControlViewModel : Conductor<object>, IInfectionControl
    {

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isEditMRBacteria;
        public bool isEditMRBacteria
        {
            get
            {
                if (_isEditMRBacteria)
                {
                    TabAddMRBacteriaSelected = true;
                    TabAddHosInfectionSelected = false;
                }
                else
                {
                    curIC_MRBacteria.InfectionControlID = 0;
                }

                return _isEditMRBacteria;
            }
            set
            {
                if (_isEditMRBacteria != value)
                {
                    _isEditMRBacteria = value;

                    NotifyOfPropertyChange(() => isEditMRBacteria);                    
                }
            }
        }

        private bool _isEditHosInfection;
        public bool isEditHosInfection
        {
            get
            {
                if (_isEditHosInfection)
                {
                    TabAddMRBacteriaSelected = false;
                    TabAddHosInfectionSelected = true;
                }
                else
                {
                    curIC_HosInfection.InfectionControlID = 0;
                }

                return _isEditHosInfection;
            }
            set
            {
                if (_isEditHosInfection != value)
                {
                    _isEditHosInfection = value;

                    NotifyOfPropertyChange(() => isEditHosInfection);
                }
            }
        }



        public InfectionControlViewModel()
        {
            curIC_MRBacteria = new InfectionControl();
            curIC_HosInfection = new InfectionControl();

            LoadMRBacteria_Level();
            LoadHosInfection_Type();
        }

        TabControl TCInfectionControlEdit { get; set; }
        public void TCInfectionControlEdit_Loaded(object sender, RoutedEventArgs e)
        {
            TCInfectionControlEdit = sender as TabControl;
        }
        #region Properties member


        private long? _PatientID;
        public long? PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                if (_PatientID != value)
                {
                    _PatientID = value;
                    NotifyOfPropertyChange(()=>PatientID);
                }
            }
        }

        //▼==== #001
        private long? _InPatientAdmDisDetailID;
        public long? InPatientAdmDisDetailID
        {
            get
            {
                return _InPatientAdmDisDetailID;
            }
            set
            {
                if (_InPatientAdmDisDetailID != value)
                {
                    _InPatientAdmDisDetailID = value;
                    NotifyOfPropertyChange(() => InPatientAdmDisDetailID);
                }
            }
        }
        //▲==== #001

        private InfectionControl _curIC_MRBacteria;
        public InfectionControl curIC_MRBacteria
        {
            get
            {
                return _curIC_MRBacteria;
            }
            set
            {
                if (_curIC_MRBacteria != value)
                {
                    _curIC_MRBacteria = value;
                    _curIC_MRBacteria.BacteriaType = 0;

                    if (MRBacteria_Level != null && _curIC_MRBacteria.DefiniteDate == null)
                    {
                        _curIC_MRBacteria.V_Bacteria_LOT = Convert.ToInt64(MRBacteria_Level.FirstOrDefault().LookupID);
                    }
                    NotifyOfPropertyChange(() => curIC_MRBacteria);
                }
            }
        }

        private InfectionControl _curIC_HosInfection;
        public InfectionControl curIC_HosInfection
        {
            get
            {
                return _curIC_HosInfection;
            }
            set
            {
                if (_curIC_HosInfection != value)
                {
                    _curIC_HosInfection = value;
                    _curIC_HosInfection.BacteriaType = 1;

                    if (HosInfection_Type != null && _curIC_HosInfection.DefiniteDate == null)
                    {
                        _curIC_HosInfection.V_Bacteria_LOT = Convert.ToInt64(HosInfection_Type.FirstOrDefault().LookupID);
                    }
                    NotifyOfPropertyChange(() => curIC_HosInfection);
                }
            }
        }

        private ObservableCollection<Lookup> _MRBacteria_Level;
        public ObservableCollection<Lookup> MRBacteria_Level
        {
            get
            {
                return _MRBacteria_Level;
            }
            set
            {
                if (_MRBacteria_Level != value)
                {
                    _MRBacteria_Level = value;
                    NotifyOfPropertyChange(() => MRBacteria_Level);
                }
            }
        }

        private void LoadMRBacteria_Level()
        {
            ObservableCollection<Lookup> tmpLookupList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_MRBacteria_Level).ToObservableCollection();

            if (tmpLookupList == null || tmpLookupList.Count <= 0)
            {
                MessageBox.Show("Không tìm thấy " + eHCMSResources.Z3301_G1_MDDaKhang, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            MRBacteria_Level = tmpLookupList;
        }

        private ObservableCollection<Lookup> _HosInfection_Type;
        public ObservableCollection<Lookup> HosInfection_Type
        {
            get
            {
                return _HosInfection_Type;
            }
            set
            {
                if (_HosInfection_Type != value)
                {
                    _HosInfection_Type = value;
                    NotifyOfPropertyChange(() => HosInfection_Type);
                }
            }
        }

        private void LoadHosInfection_Type()
        {
            ObservableCollection<Lookup> tmpLookupList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_HosInfection_Type).ToObservableCollection();

            if (tmpLookupList == null || tmpLookupList.Count <= 0)
            {
                MessageBox.Show("Không tìm thấy " + eHCMSResources.Z3303_G1_LoaiNKBV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            HosInfection_Type = tmpLookupList;
        }

        #endregion

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        public void OKSaveMRBacteria(object sender, RoutedEventArgs e)
        {
            //add new here             
            if (PatientID == null || PatientID < 1)
            {
                return;
            }
            if (InPatientAdmDisDetailID == null || InPatientAdmDisDetailID < 1)
            {
                return;
            }
            if (CheckValidSaveMRBacteria(curIC_MRBacteria))
            {
                curIC_MRBacteria.PatientID = PatientID.Value;
                curIC_MRBacteria.InPatientAdmDisDetailID = InPatientAdmDisDetailID.Value;
                curIC_MRBacteria.StaffID = Globals.LoggedUserAccount.StaffID.Value;

                AddNewIC_MRBacteria();
            }
        }

        public void OKSaveHosInfection(object sender, RoutedEventArgs e)
        {
            //add new here             
            if (PatientID == null || PatientID<1)
            {
                return;
            }
            if (InPatientAdmDisDetailID == null || InPatientAdmDisDetailID < 1)
            {
                return;
            }
            if (CheckValidSaveHosInfection(curIC_HosInfection))
            {
                curIC_HosInfection.PatientID = PatientID.Value;
                curIC_HosInfection.InPatientAdmDisDetailID = InPatientAdmDisDetailID.Value;
                curIC_HosInfection.StaffID = Globals.LoggedUserAccount.StaffID.Value;

                AddNewIC_HosInfection();
            }
        }

        private bool CheckValidSaveMRBacteria(object temp)
        {
            InfectionControl u = temp as InfectionControl;
            if (u == null)
            {
                return false;
            }
            if (u.DefiniteDate == null)
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.Z3300_G1_NgXacDinh), eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (u.BacteriaName == null)
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.Z3302_G1_TenViKhuan), eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (u.V_Bacteria_LOT == 0)
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.Z3301_G1_MDDaKhang), eHCMSResources.G0442_G1_TBao);
                return false;
            }
            return u.Validate();
        }
        
        private bool CheckValidSaveHosInfection(object temp)
        {
            InfectionControl u = temp as InfectionControl;
            if (u == null)
            {
                return false;
            }
            if (u.DefiniteDate == null)
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.Z3300_G1_NgXacDinh), eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (u.BacteriaName == null)
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.Z3302_G1_TenViKhuan), eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (u.V_Bacteria_LOT == 0)
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.Z3303_G1_LoaiNKBV), eHCMSResources.G0442_G1_TBao);
                return false;
            }
            return u.Validate();
        }

        public void CloseCmd() 
        {
            TryClose();
        }
        #region service method

        private void AddNewIC_MRBacteria()
        {
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSaveInfectionControl(curIC_MRBacteria, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndSaveInfectionControl(asyncResult);
                            //phat ra su kien de load lai 
                            Globals.EventAggregator.Publish(new SaveInfectionControlCompleteEvent { });
                            //theDisp.BeginInvoke(() =>
                            System.Windows.Threading.Dispatcher theDisp = System.Windows.Application.Current.Dispatcher;
                            theDisp.Invoke(() => { TryClose(); });
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }

                    }), null);

                }

            });
            t.Start();
        }

        private void AddNewIC_HosInfection()
        {
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSaveInfectionControl(curIC_HosInfection, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndSaveInfectionControl(asyncResult);
                            //phat ra su kien de load lai 
                            Globals.EventAggregator.Publish(new SaveInfectionControlCompleteEvent { });
                            //theDisp.BeginInvoke(() =>
                            System.Windows.Threading.Dispatcher theDisp = System.Windows.Application.Current.Dispatcher;
                            theDisp.Invoke(() => { TryClose(); });
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }

                    }), null);

                }

            });
            t.Start();
        }

        private bool _TabAddMRBacteriaSelected = true;
        public bool TabAddMRBacteriaSelected
        {
            get
            {
                return _TabAddMRBacteriaSelected;
            }
            set
            {
                if (_TabAddMRBacteriaSelected != value)
                {
                    _TabAddMRBacteriaSelected = value;
                    NotifyOfPropertyChange(() => TabAddMRBacteriaSelected);
                }
            }
        }

        private bool _TabAddHosInfectionSelected = false;
        public bool TabAddHosInfectionSelected
        {
            get
            {
                return _TabAddHosInfectionSelected;
            }
            set
            {
                if (_TabAddHosInfectionSelected != value)
                {
                    _TabAddHosInfectionSelected = value;
                    NotifyOfPropertyChange(() => TabAddHosInfectionSelected);
                }
            }
        }
        #endregion
    }
}
