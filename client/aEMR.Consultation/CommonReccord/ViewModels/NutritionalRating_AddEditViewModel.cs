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
/*
* 20181023 #001 TTM:   BM 0002173: Thay đổi cách lưu, cập nhật và lấy lên của tình trạng thể chất => tất cả đều dựa vào lần đăng ký.    
* 20200121 #002 TBL:   BM 0021818: Chỉnh sửa lại cách nhập dữ liệu màn hình Theo dõi sinh hiệu - Nội trúx
*/
namespace aEMR.ConsultantEPrescription.CommonRecs.ViewModels
{
    [Export(typeof(INutritionalRating_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class NutritionalRating_AddEditViewModel : ViewModelBase, INutritionalRating_AddEdit
    {
        public void InitializeNewItem()
        {
            CurrentNutritionalRating = new NutritionalRating();
            CurrentNutritionalRating.DeptID = Globals.DeptLocation.DeptID;
            if (IsBMIBelow205)
            {
                CurrentNutritionalRating.ROM_BMI = true;
                CurrentNutritionalRating.RiskOfMalnutrition = true;
            }
        }

        private bool _IsBMIBelow205;
        public bool IsBMIBelow205
        {
            get { return _IsBMIBelow205; }
            set
            {
                _IsBMIBelow205 = value;
                NotifyOfPropertyChange(() => IsBMIBelow205);
            }
        }

        private NutritionalRating _currentNutritionalRating;
        public NutritionalRating CurrentNutritionalRating
        {
            get { return _currentNutritionalRating; }
            set
            {
                if (_currentNutritionalRating != value)
                {
                    _currentNutritionalRating = value;
                    NotifyOfPropertyChange(() => CurrentNutritionalRating);
                }
            }
        }
        private ObservableCollection<Lookup> _V_EatingType;
        public ObservableCollection<Lookup> V_EatingType
        {
            get { return _V_EatingType; }
            set
            {
                _V_EatingType = value;
                NotifyOfPropertyChange(() => V_EatingType);
            }
        }
        private ObservableCollection<Lookup> _V_ExaminationType;
        public ObservableCollection<Lookup> V_ExaminationType
        {
            get { return _V_ExaminationType; }
            set
            {
                _V_ExaminationType = value;
                NotifyOfPropertyChange(() => V_ExaminationType);
            }
        }
        private ObservableCollection<Lookup> _V_SGAType;
        public ObservableCollection<Lookup> V_SGAType
        {
            get { return _V_SGAType; }
            set
            {
                _V_SGAType = value;
                NotifyOfPropertyChange(() => V_SGAType);
            }
        }
        private ObservableCollection<Lookup> _V_NutritionalRequire;
        public ObservableCollection<Lookup> V_NutritionalRequire
        {
            get { return _V_NutritionalRequire; }
            set
            {
                _V_NutritionalRequire = value;
                NotifyOfPropertyChange(() => V_NutritionalRequire);
            }
        }
        private ObservableCollection<Lookup> _V_NutritionalMethods;
        public ObservableCollection<Lookup> V_NutritionalMethods
        {
            get { return _V_NutritionalMethods; }
            set
            {
                _V_NutritionalMethods = value;
                NotifyOfPropertyChange(() => V_NutritionalMethods);
            }
        }
        private bool _IsNutritionalRequireOtherEnable;
        public bool IsNutritionalRequireOtherEnable
        {
            get { return _IsNutritionalRequireOtherEnable; }
            set
            {
                _IsNutritionalRequireOtherEnable = value;
                NotifyOfPropertyChange(() => IsNutritionalRequireOtherEnable);
            }
        }
        private long _PtRegistrationID;
        public long PtRegistrationID
        {
            get { return _PtRegistrationID; }
            set
            {
                _PtRegistrationID = value;
                NotifyOfPropertyChange(() => PtRegistrationID);
            }
        }

        public void btnSave()
        {
            if (ValidateBeforeSave())
            {
                Save(PtRegistrationID, CurrentNutritionalRating);
            }
        }
        public void chkROM_Checked(object sender, RoutedEventArgs e)
        {
            if (CurrentNutritionalRating.ROM_BMI ||
                CurrentNutritionalRating.ROM_ReduceEat ||
                CurrentNutritionalRating.ROM_SevereIllness ||
                CurrentNutritionalRating.ROM_WeightLoss)
            {
                CurrentNutritionalRating.RiskOfMalnutrition = true;
            }
            else
            {
                CurrentNutritionalRating.RiskOfMalnutrition = false;
            }
        }
        public void chkRiskOfMalnutrition_Checked(object sender, RoutedEventArgs e)
        {
            //if (!CurrentNutritionalRating.RiskOfMalnutrition)
            //{
            //    CurrentNutritionalRating.ROM_BMI = false;
            //    CurrentNutritionalRating.ROM_ReduceEat = false;
            //    CurrentNutritionalRating.ROM_SevereIllness = false;
            //    CurrentNutritionalRating.ROM_WeightLoss = false;
            //}
            //else
            //{
            //    MessageBox.Show("Vui lòng chọn các yếu tố nguy cơ");
            //    CurrentNutritionalRating.RiskOfMalnutrition = false;
            //}
        }
        public void chkNutritionalRequire_Checked(object sender, RoutedEventArgs e)
        {
            if (CurrentNutritionalRating.V_NutritionalRequire == (long)AllLookupValues.V_NutritionalRequire.NutritionalRequireOther)
            {
                IsNutritionalRequireOtherEnable = true;
            }
            else
            {
                IsNutritionalRequireOtherEnable = false;
            }
        }
        
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
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public NutritionalRating_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            //Khi khoi tao module thi load menu ben trai luon.
            //Globals.EventAggregator.Subscribe(this);
            //Thay bằng
            Globals.EventAggregator.Subscribe(this);
            authorization();

            _V_EatingType = new ObservableCollection<Lookup>();
            _V_ExaminationType = new ObservableCollection<Lookup>();
            _V_SGAType = new ObservableCollection<Lookup>();
            _V_NutritionalMethods = new ObservableCollection<Lookup>();
            _V_NutritionalRequire = new ObservableCollection<Lookup>();

            LoadEatingType();
            LoadExaminationType();
            LoadSGAType();
            LoadNutritionalMethods();
            LoadNutritionalRequire();

        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }
    


        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mTongQuat_XemThongTin = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtDashboardCommonRecs,
                                                   (int)oConsultationEx.mTongQuat_XemThongTin, (int)ePermission.mView);
            mTongQuat_ChinhSuaThongTin = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtDashboardCommonRecs,
                                                   (int)oConsultationEx.mTongQuat_ChinhSuaThongTin, (int)ePermission.mView);
        }

        #region account checking

        private bool _mTongQuat_XemThongTin = true;
        private bool _mTongQuat_ChinhSuaThongTin = true && Globals.isConsultationStateEdit;

        public bool mTongQuat_XemThongTin
        {
            get
            {
                return _mTongQuat_XemThongTin;
            }
            set
            {
                if (_mTongQuat_XemThongTin == value)
                    return;
                _mTongQuat_XemThongTin = value;
            }
        }
        public bool mTongQuat_ChinhSuaThongTin
        {
            get
            {
                return _mTongQuat_ChinhSuaThongTin;
            }
            set
            {
                if (_mTongQuat_ChinhSuaThongTin == value)
                    return;
                _mTongQuat_ChinhSuaThongTin = value && Globals.isConsultationStateEdit;
            }
        }

        #endregion
        #region binding visibilty

        public Button lnkDelete { get; set; }
        public Button lnkEdit { get; set; }
        public Button lnkSave { get; set; }
        public Button lnkCancel { get; set; }
     

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(mTongQuat_ChinhSuaThongTin);
        }
        public void lnkEdit_Loaded(object sender)
        {
            lnkEdit = sender as Button;
            lnkEdit.Visibility = Globals.convertVisibility(mTongQuat_ChinhSuaThongTin);
        }
        public void lnkSave_Loaded(object sender)
        {
            lnkSave = sender as Button;
            lnkSave.Visibility = Globals.convertVisibility(mTongQuat_ChinhSuaThongTin);
        }
        public void lnkCancel_Loaded(object sender)
        {
            lnkCancel = sender as Button;
            lnkCancel.Visibility = Globals.convertVisibility(mTongQuat_ChinhSuaThongTin);
        }
        #endregion
        #region property



        private PhysicalExamination _selectPhyExamList;
        public PhysicalExamination selectPhyExamList
        {
            get
            {
                return _selectPhyExamList;
            }
            set
            {
                if (_selectPhyExamList == value)
                    return;
                _selectPhyExamList = value;
                NotifyOfPropertyChange(() => selectPhyExamList);
            }
        }


        private ObservableCollection<PhysicalExamination> _PtPhyExamList;
        public ObservableCollection<PhysicalExamination> PtPhyExamList
        {
            get
            {
                return _PtPhyExamList;
            }
            set
            {
                if (_PtPhyExamList == value)
                    return;
                _PtPhyExamList = value;
                NotifyOfPropertyChange(() => PtPhyExamList);
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
        #endregion


        public object Delete { get; set; }

        public bool CheckValid(object temp)
        {
            PhysicalExamination u = temp as PhysicalExamination;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }
        public bool CheckNullValue(PhysicalExamination pe)
        {
            if (pe.Alcohol_CurrentHeavy == null
                && pe.Height == null
                && pe.Weight == null
                && pe.DiastolicPressure == null
                && pe.SystolicPressure == null
                && pe.Pulse == null
                && pe.Cholesterol == null
                && pe.RefAlcohol == null
                && pe.RefSmoke == null
                )
            {
                Globals.ShowMessage(eHCMSResources.Z0503_G1_ChonItNhatMotTTTheChat, "");
                return false;
            }
            return true;
        }
        public void btnAdd()
        {

        }
        //▼====== #002
        public void btnCancel()
        {
            selectPhyExamList = new PhysicalExamination();
            isEdit = false;
        }
 
        public void lnkSaveClick(RoutedEventArgs e)
        {
           
        }
        public void lnkCancel_Click(RoutedEventArgs e)
        {
           
        }
        public void lnkDeleteClick(object selectItem)
        {
           
        }

        private bool isEdit = false;
        public void lnkEditClick(RoutedEventArgs e)
        {
            //grdCommonRecord.IsReadOnly = false;
            //grdCommonRecord.EditRecord();
            //grdCommonRecord.BeginEdit();
            //DataTemplate row = this.grdCommonRecord.RowDetailsTemplate;
            //((PhysicalExamination)grdCommonRecord.SelectedItem).isEdit = false;
            isEdit = true;
            
        }

        #region method
        private void Save(long PtRegistrationID, NutritionalRating curNutritionalRating)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSaveNutritionalRating(PtRegistrationID, curNutritionalRating, (long)Globals.LoggedUserAccount.StaffID, Globals.GetCurServerDateTime(),
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var bOK = contract.EndSaveNutritionalRating(out long NutritionalRatingIDNew, asyncResult);
                                    if (bOK)
                                    {
                                        MessageBox.Show("Cập nhật thành công");
                                        CurrentNutritionalRating.NutritionalRatingID = NutritionalRatingIDNew;
                                        Globals.EventAggregator.Publish(new NutritionalRating_Event_Save() { Result = true });
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        public void LoadEatingType()
        {
            var t = new Thread(() =>
            {
                IsLoading = true;
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLookupValuesByType(LookupValues.V_EatingType,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                    if (allItems != null)
                                    {
                                        V_EatingType.Clear();
                                        foreach (var tp in allItems)
                                        {
                                            V_EatingType.Add(tp);
                                        }
                                    }
                                }
                                catch
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
                    IsLoading = false;
                }
            });
            t.Start();
        }
        public void LoadExaminationType()
        {
            var t = new Thread(() =>
            {
                IsLoading = true;
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLookupValuesByType(LookupValues.V_ExaminationType,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                    if (allItems != null)
                                    {
                                        V_ExaminationType.Clear();
                                        foreach (var tp in allItems)
                                        {
                                            V_ExaminationType.Add(tp);
                                        }
                                    }
                                }
                                catch
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
                    IsLoading = false;
                }
            });
            t.Start();
        }
        public void LoadSGAType()
        {
            var t = new Thread(() =>
            {
                IsLoading = true;
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLookupValuesByType(LookupValues.V_SGAType,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                    if (allItems != null)
                                    {
                                        V_SGAType.Clear();
                                        foreach (var tp in allItems)
                                        {
                                            V_SGAType.Add(tp);
                                        }
                                    }
                                }
                                catch
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
                    IsLoading = false;
                }
            });
            t.Start();
        }
        public void LoadNutritionalRequire()
        {
            var t = new Thread(() =>
            {
                IsLoading = true;
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLookupValuesByType(LookupValues.V_NutritionalRequire,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                    if (allItems != null)
                                    {
                                        V_NutritionalRequire.Clear();
                                        foreach (var tp in allItems)
                                        {
                                            V_NutritionalRequire.Add(tp);
                                        }
                                    }
                                }
                                catch
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
                    IsLoading = false;
                }
            });
            t.Start();
        }
        public void LoadNutritionalMethods()
        {
            var t = new Thread(() =>
            {
                IsLoading = true;
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLookupValuesByType(LookupValues.V_NutritionalMethods,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                    if (allItems != null)
                                    {
                                        V_NutritionalMethods.Clear();
                                        foreach (var tp in allItems)
                                        {
                                            V_NutritionalMethods.Add(tp);
                                        }
                                    }
                                }
                                catch
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
                    IsLoading = false;
                }
            });
            t.Start();
        }

        private bool ValidateBeforeSave()
        {
            string ErrorMessage = "";
            if (CurrentNutritionalRating.WeightLossHospitalStay)
            {
                if (CurrentNutritionalRating.WL_Weight == 0)
                {
                    ErrorMessage = ErrorMessage + "Mục \"1. Giá trị \"kg trong\" không được bằng 0.\" bắt buộc phải chọn một giá trị!" + "\n";
                }
                if (CurrentNutritionalRating.WL_Month == 0)
                {
                    ErrorMessage = ErrorMessage + "Mục \"1. Giá trị \"tháng\" không được bằng 0.\" bắt buộc phải chọn một giá trị!" + "\n";
                }
                if (CurrentNutritionalRating.WL_Percent == 0)
                {
                    ErrorMessage = ErrorMessage + "Mục \"1. Giá trị \"%\" không được bằng 0.\" bắt buộc phải chọn một giá trị!" + "\n";
                }
            }
            if (CurrentNutritionalRating.V_EatingType == 0)
            {
                ErrorMessage = ErrorMessage + "Mục \"2. Ăn uống trong một tuần trước vào viện/ Thời gian nằm viện\" bắt buộc phải chọn một giá trị!" + "\n";
            }
            if (CurrentNutritionalRating.AtrophySubcutaneousFatLayer == 0)
            {
                ErrorMessage = ErrorMessage + "Mục \"3. Khám\". Trường \"Teo lớp mỡ dưới da\" bắt buộc phải chọn một giá trị!" + "\n";
            }
            if (CurrentNutritionalRating.AmyotrophicLateralSclerosis == 0)
            {
                ErrorMessage = ErrorMessage + "Mục \"3. Khám\". Trường \"Teo cơ\" bắt buộc phải chọn một giá trị!" + "\n";
            }
            if (CurrentNutritionalRating.PeripheralEdema == 0)
            {
                ErrorMessage = ErrorMessage + "Mục \"3. Khám\". Trường \"Phù ngoại vi\" bắt buộc phải chọn một giá trị!" + "\n";
            }
            if (CurrentNutritionalRating.BellyFlap == 0)
            {
                ErrorMessage = ErrorMessage + "Mục \"3. Khám\". Trường \"Báng bụng\" bắt buộc phải chọn một giá trị!" + "\n";
            }
            if (CurrentNutritionalRating.V_SGAType == 0)
            {
                ErrorMessage = ErrorMessage + "Mục \"4. Phân loại SGA(Sudjective Global Assessment)\" bắt buộc phải chọn một giá trị!" + "\n";
            }
            if (CurrentNutritionalRating.V_NutritionalRequire == 0)
            {
                ErrorMessage = ErrorMessage + "Mục \"5. Xác định nhu cầu dinh dưỡng\" bắt buộc phải chọn một giá trị!" + "\n";
            }
            if (CurrentNutritionalRating.V_NutritionalMethods == 0)
            {
                ErrorMessage = ErrorMessage + "Mục \"6. Phương pháp dinh dưỡng\" bắt buộc phải chọn một giá trị!" + "\n";
            }
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                IErrorBold MessBox = Globals.GetViewModel<IErrorBold>();
                MessBox.FireOncloseEvent = true;
                MessBox.isCheckBox = false;
                MessBox.SetMessage(ErrorMessage + Environment.NewLine, "");
                GlobalsNAV.ShowDialog_V3(MessBox);
                //Globals.ShowMessage(ErrorMessage, eHCMSResources.T0432_G1_Error);
                return false;
            }
            else return true;
        }
        #endregion
    }
}
