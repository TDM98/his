using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using DataEntities;
using aEMR.Common;
using eHCMSLanguage;
using System.Linq;
using Castle.Windsor;
using aEMR.ViewContracts;
using System.Windows.Controls;
using eHCMS.Services.Core;
using aEMR.Common.BaseModel;

/*
 * 20180508 #001 TBLD:  Check HI infomation
 * 20191003 #002 TTM:   BM 0017393: Bổ sung thông tin chuyên khoa cho dịch vụ.
*/
namespace aEMR.Configuration.DeptMedServiceItems.ViewModels
{
    [Export(typeof(IDeptMedServiceItems_EditInfo)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeptMedServiceItems_EditInfoViewModel : ViewModelBase, IDeptMedServiceItems_EditInfo
    {
        private string _TitleForm = "Hiệu Chỉnh Thông Tin Dịch Vụ";
        public string TitleForm
        {
            get { return _TitleForm; }
            set
            {
                if (_TitleForm != value)
                {
                    _TitleForm = value;
                    NotifyOfPropertyChange(() => TitleForm);
                }
            }
        }

        private bool _isUpdate;
        public bool isUpdate
        {
            get { return _isUpdate; }
            set
            {
                if (_isUpdate != value)
                {
                    _isUpdate = value;
                    NotifyOfPropertyChange(() => isUpdate);
                }
            }
        }

        [ImportingConstructor]
        public DeptMedServiceItems_EditInfoViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            if (!isUpdate)
            {
                ObjRefMedicalServiceItem = new RefMedicalServiceItem();
                ObjRefMedicalServiceItem.ExpiryDate = DateTime.Now.AddYears(20);
            }
            ObjV_RefMedServiceItemsUnit = new ObservableCollection<Lookup>();
            LoadV_RefMedServiceItemsUnit();

            /*TMA*/
            ObjV_Surgery_Tips_Type = new ObservableCollection<Lookup>();
            LoadV_Surgery_Tips_Type();
            /*TMA*/

            LoadV_AppointmentType();
            LoadV_SpecialistType();
            ObjHITransactionType_GetListNoParentID = new ObservableCollection<HITransactionType>();
            HITransactionType_GetListNoParentID();
            SetValuesForGender();

            ObjV_DVKTPhanTuyen = new ObservableCollection<Lookup>();
            ObjV_DVKTPhanTuyen = Globals.AllLookupValueList.Where(x => x.ObjectName == "V_DVKTPhanTuyen").ToObservableCollection();
            Lookup firstItem = new Lookup();
            firstItem.LookupID = -1;
            firstItem.ObjectValue = "--Chọn Đơn Vị--";
            ObjV_DVKTPhanTuyen.Insert(0, firstItem);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            GetAllMedicalServiceTypes_SubtractPCL();
        }

        //Đơn Vị Tính
        private Lookup _ObjV_RefMedServiceItemsUnit_Selected;
        public Lookup ObjV_RefMedServiceItemsUnit_Selected
        {
            get { return _ObjV_RefMedServiceItemsUnit_Selected; }
            set
            {
                _ObjV_RefMedServiceItemsUnit_Selected = value;
                NotifyOfPropertyChange(() => ObjV_RefMedServiceItemsUnit_Selected);
            }
        }

        private ObservableCollection<Lookup> _ObjV_RefMedServiceItemsUnit;
        public ObservableCollection<Lookup> ObjV_RefMedServiceItemsUnit
        {
            get { return _ObjV_RefMedServiceItemsUnit; }
            set
            {
                _ObjV_RefMedServiceItemsUnit = value;
                NotifyOfPropertyChange(() => ObjV_RefMedServiceItemsUnit);
            }
        }

        private RefMedicalServiceType _ObjRefMedicalServiceTypeSelected;
        public RefMedicalServiceType ObjRefMedicalServiceTypeSelected
        {
            get
            {
                return _ObjRefMedicalServiceTypeSelected;
            }
            set
            {
                if (_ObjRefMedicalServiceTypeSelected != value)
                {
                    _ObjRefMedicalServiceTypeSelected = value;
                    NotifyOfPropertyChange(() => ObjRefMedicalServiceTypeSelected);
                    NotifyOfPropertyChange(() => cboV_AppointmentTypeIsEnabled);
                }
            }
        }

        public void LoadV_RefMedServiceItemsUnit()
        {
            var t = new Thread(() =>
            {
                this.DlgShowBusyIndicator("Danh Sách Đơn Vị Tính...");
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_RefMedServiceItemsUnit,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_RefMedServiceItemsUnit = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup();
                                    if (ObjRefMedicalServiceItem.V_RefMedServiceItemsUnit >= 0)
                                    {
                                        ObjRefMedicalServiceItem.V_RefMedServiceItemsUnit = ObjRefMedicalServiceItem.ObjV_RefMedServiceItemsUnit.LookupID;
                                    }
                                    else
                                    {
                                        firstItem.LookupID = -1;
                                        firstItem.ObjectValue = "--Chọn Đơn Vị--";
                                        ObjV_RefMedServiceItemsUnit.Insert(0, firstItem);
                                    }

                                    if (ObjDeptMedServiceItems_Save != null && ObjDeptMedServiceItems_Save.V_RefMedServiceItemsUnit <= 0) // TMA bỏ dấu bằng
                                    {
                                        ObjDeptMedServiceItems_Save.V_RefMedServiceItemsUnit = -1;
                                    }
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
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }
        //Đơn Vị Tính

        //HITransactionType
        private ObservableCollection<HITransactionType> _ObjHITransactionType_GetListNoParentID;
        public ObservableCollection<HITransactionType> ObjHITransactionType_GetListNoParentID
        {
            get { return _ObjHITransactionType_GetListNoParentID; }
            set
            {
                _ObjHITransactionType_GetListNoParentID = value;
                NotifyOfPropertyChange(() => ObjHITransactionType_GetListNoParentID);
            }
        }

        public void HITransactionType_GetListNoParentID()
        {
            ObjHITransactionType_GetListNoParentID.Clear();
            this.DlgShowBusyIndicator("Danh Sách HITransactionType...");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginHITransactionType_GetListNoParentID(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndHITransactionType_GetListNoParentID(asyncResult);

                                if (items != null)
                                {
                                    ObjHITransactionType_GetListNoParentID = new ObservableCollection<HITransactionType>(items);
                                    //ItemDefault
                                    HITransactionType ItemDefault = new HITransactionType();
                                    ItemDefault.HITTypeID = -1;
                                    ItemDefault.HITypeDescription = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z1024_G1_TuyChon);
                                    ObjHITransactionType_GetListNoParentID.Insert(0, ItemDefault);
                                    //ItemDefault

                                    if (ObjRefMedicalServiceItem != null &&
                                            (ObjRefMedicalServiceItem.HITTypeID < 0 || ObjRefMedicalServiceItem.HITTypeID == null))
                                    {
                                        ObjRefMedicalServiceItem.HITTypeID = -1;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }
        //HITransactionType

        private ObservableCollection<RefMedicalServiceType> _ObjRefMedicalServiceTypes_GetAll;
        public ObservableCollection<RefMedicalServiceType> ObjRefMedicalServiceTypes_GetAll
        {
            get { return _ObjRefMedicalServiceTypes_GetAll; }
            set
            {
                _ObjRefMedicalServiceTypes_GetAll = value;
                NotifyOfPropertyChange(() => ObjRefMedicalServiceTypes_GetAll);
            }
        }

        private RefMedicalServiceType _SelectedMedicalServiceType;
        public RefMedicalServiceType SelectedMedicalServiceType
        {
            get
            {
                return _SelectedMedicalServiceType;
            }
            set
            {
                _SelectedMedicalServiceType = value;
                if (ObjRefMedicalServiceItem != null)
                {
                    ObjRefMedicalServiceItem.RefMedicalServiceType = SelectedMedicalServiceType;
                }
                if (ObjRefMedicalServiceItem.RefMedicalServiceType != null && ObjRefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH)
                {
                    vV_SpecialistType = true;
                }
                else
                {
                    vV_SpecialistType = false;
                }
                NotifyOfPropertyChange("SelectedMedicalServiceType");
                NotifyOfPropertyChange(() => IsSurgeryTips);        /*TMA*/
                NotifyOfPropertyChange(() => vV_SpecialistType);
                NotifyOfPropertyChange(() => cboV_AppointmentTypeIsEnabled);
                NotifyOfPropertyChange(() => chk_MedicalExaminationIsEnable);
            }
        }

        public void GetAllMedicalServiceTypes_SubtractPCL()
        {
            this.DlgShowBusyIndicator("Danh Sách Loại Dịch Vụ...");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllMedicalServiceTypes_SubtractPCL(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndGetAllMedicalServiceTypes_SubtractPCL(asyncResult);

                                if (items != null)
                                {
                                    ObjRefMedicalServiceTypes_GetAll = new ObservableCollection<DataEntities.RefMedicalServiceType>(items);

                                    //Item Default
                                    /*TMA 07/11/2017*/
                                    RefMedicalServiceType ItemDefault = new RefMedicalServiceType(); /*TMA 07/11/2017*/

                                    //RefMedicalServiceType ItemDefault = new RefMedicalServiceType(); /*TMA 07/11/2017*/
                                    //RefMedicalServiceType
                                    if (ObjRefMedicalServiceTypeSelected != null && ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID > 0)
                                    {
                                        SelectedMedicalServiceType = new DataEntities.RefMedicalServiceType();
                                        SelectedMedicalServiceType = ObjRefMedicalServiceTypeSelected;
                                    }
                                    /*TMA 07/11/2017*/
                                    ItemDefault.MedicalServiceTypeID = -1;
                                    ItemDefault.MedicalServiceTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2027_G1_ChonLoaiDV);
                                    //Item Default

                                    ObjRefMedicalServiceTypes_GetAll.Insert(0, ItemDefault);
                                }
                                else
                                {
                                    ObjRefMedicalServiceTypes_GetAll = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }


        private DataEntities.DeptMedServiceItems _ObjDeptMedServiceItems_Save;
        public DataEntities.DeptMedServiceItems ObjDeptMedServiceItems_Save
        {
            get { return _ObjDeptMedServiceItems_Save; }
            set
            {
                _ObjDeptMedServiceItems_Save = value;
                NotifyOfPropertyChange(() => ObjDeptMedServiceItems_Save);
            }
        }

        private RefMedicalServiceItem _ObjRefMedicalServiceItem;
        public RefMedicalServiceItem ObjRefMedicalServiceItem
        {
            get { return _ObjRefMedicalServiceItem; }
            set
            {
                _ObjRefMedicalServiceItem = value;
                SetRadioButton();
                SetRadioButtonSurgery_Tips_Item();
                if (_ObjRefMedicalServiceItem.V_DVKTPhanTuyen == null)
                {
                    _ObjRefMedicalServiceItem.V_DVKTPhanTuyen = -1;
                }

                NotifyOfPropertyChange(() => ObjRefMedicalServiceItem);
            }
        }

        public void btClose()
        {
            TryClose();
        }

        public void btSave()
        {
            if (CheckValid1(ObjRefMedicalServiceItem))
            {
                if (ObjRefMedicalServiceItem.MedicalServiceTypeID > 0)
                {
                    if (ObjRefMedicalServiceItem.V_RefMedServiceItemsUnit > 0)
                    {
                        /*▼====: #001*/
                        if (ObjRefMedicalServiceItem.HIApproved == true && !CheckHIInfo())
                        {
                            return;
                        }
                        /*▲====: #001*/
                        if (ObjRefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH && ObjRefMedicalServiceItem.V_SpecialistType <= 0)
                        {
                            MessageBox.Show(eHCMSResources.Z2848_G1_NhapChuyenKhoa);
                            return;
                        }
                        if (ObjRefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes != (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH &&
                            ObjRefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes != (long)AllLookupValues.V_RefMedicalServiceTypes.THUTHUAT)
                        {
                            ObjRefMedicalServiceItem.V_AppointmentType = null;
                            ObjRefMedicalServiceItem.IsMedicalExamination = false;
                        }
                        if (isUpdate)
                        {
                            ObjRefMedicalServiceItem.UpdatedTime = Globals.GetCurServerDateTime();
                            ObjRefMedicalServiceItem.UpdatedStaffID = Globals.LoggedUserAccount.StaffID.Value;
                            MedServiceItems_Update(ObjRefMedicalServiceItem);
                        }
                        else
                        {
                            RefMedicalServiceItems_AddNew();
                        }
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0310_G1_Msg_InfoChonDVT, eHCMSResources.K1916_G1_Chon, MessageBoxButton.OK);
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0327_G1_Msg_InfoChonLoaiDV, eHCMSResources.K1916_G1_Chon, MessageBoxButton.OK);
                }

            }
        }
        /*▼====: #001*/
        public bool CheckHIInfo()
        {
            if (string.IsNullOrEmpty(ObjRefMedicalServiceItem.HICode))
            {
                MessageBox.Show(eHCMSResources.Z2204_G1_LoiMaBH);
                return false;
            }
            if (string.IsNullOrEmpty(ObjRefMedicalServiceItem.MedServiceName_Ax))
            {
                MessageBox.Show(eHCMSResources.Z2203_G1_LoiTenBH);
                return false;
            }
            return true;
        }
        /*▲====: #001*/
        public bool CheckValid1(object temp)
        {
            DataEntities.RefMedicalServiceItem p = temp as DataEntities.RefMedicalServiceItem;
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }

        private void RefMedicalServiceItems_EditInfo()
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0645_G1_DangGhi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRefMedicalServiceItems_EditInfo(ObjRefMedicalServiceItem, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string Result = "";
                                contract.EndRefMedicalServiceItems_EditInfo(out Result, asyncResult);
                                switch (Result)
                                {
                                    case "Update-1":
                                        {
                                            Globals.EventAggregator.Publish(new SaveEvent<bool>() { Result = true });

                                            MessageBox.Show(eHCMSResources.Z0655_G1_DaGhi, eHCMSResources.T1484_G1_HChinh, MessageBoxButton.OK);
                                            Globals.EventAggregator.Publish(new RefMedicalServiceItem_AddEditEvent_Save { Result = eHCMSResources.Z1396_G1_Success });
                                            break;
                                        }
                                    case "Update-0":
                                        {
                                            MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T1484_G1_HChinh, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Duplex-Code":
                                        {
                                            MessageBox.Show(eHCMSResources.A0812_G1_Msg_InfoMaDaTonTai, eHCMSResources.T1484_G1_HChinh, MessageBoxButton.OK);
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
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        private void RefMedicalServiceItems_AddNew()
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0343_G1_DangLuu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRefMedicalServiceItems_NotPCL_Insert(ObjRefMedicalServiceItem
                            , Globals.LoggedUserAccount.StaffID.Value
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndRefMedicalServiceItems_NotPCL_Insert(asyncResult);
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                Globals.EventAggregator.Publish(new RefMedicalServiceItem_AddEditEvent_Save { Result = eHCMSResources.Z1396_G1_Success });
                                TryClose();
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        public void MedServiceItems_Update(RefMedicalServiceItem obj)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0343_G1_DangLuu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRefMedicalServiceItems_NotPCL_Update(obj, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (contract.EndRefMedicalServiceItems_NotPCL_Update(asyncResult))
                                {
                                    MessageBox.Show(eHCMSResources.K2782_G1_DaCNhat, eHCMSResources.A0273_G1_Msg_CNhatDV, MessageBoxButton.OK);
                                    Globals.EventAggregator.Publish(new RefMedicalServiceItem_AddEditEvent_Save { Result = eHCMSResources.Z1396_G1_Success });
                                    TryClose();
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.A0273_G1_Msg_CNhatDV, MessageBoxButton.OK);
                                }
                            }
                            catch (Exception ex)
                            {
                                //Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        #region Config Cho phép hẹn bệnh
        //Loại Hẹn
        private ObservableCollection<Lookup> _ObjV_AppointmentType;
        public ObservableCollection<Lookup> ObjV_AppointmentType
        {
            get { return _ObjV_AppointmentType; }
            set
            {
                _ObjV_AppointmentType = value;
                NotifyOfPropertyChange(() => ObjV_AppointmentType);
            }
        }

        public void LoadV_AppointmentType()
        {
            var t = new Thread(() =>
            {
                this.DlgShowBusyIndicator(eHCMSResources.Z0669_G1_DangLayDLieu);
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLookupValuesByType(LookupValues.APPOINTMENT_TYPE,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);
                                    ObjV_AppointmentType = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup();
                                    firstItem.LookupID = -1;
                                    firstItem.ObjectValue = "--Tùy Chọn--";
                                    ObjV_AppointmentType.Insert(0, firstItem);

                                    if (ObjDeptMedServiceItems_Save != null && ObjDeptMedServiceItems_Save.ObjRefMedicalServiceItem.V_AppointmentType <= 0)
                                    {
                                        ObjDeptMedServiceItems_Save.ObjRefMedicalServiceItem.V_AppointmentType = -1;
                                    }
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
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        public bool cboV_AppointmentTypeIsEnabled
        {
            get
            {
                //return ObjRefMedicalServiceTypeSelected.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH;
                return (ObjRefMedicalServiceItem.RefMedicalServiceType != null &&
                    (ObjRefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH
                        || ObjRefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.THUTHUAT));
            }
        }

        public bool chk_MedicalExaminationIsEnable
        {
            get
            {
                //return ObjRefMedicalServiceTypeSelected.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH;
                return (ObjRefMedicalServiceItem.RefMedicalServiceType != null && ObjRefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH);
            }
        }
        #endregion

        #region NewPriceType
        // Không có giá
        private bool _IsUnknown_PriceType;
        public bool IsUnknown_PriceType
        {
            get
            {
                return _IsUnknown_PriceType;
            }
            set
            {
                _IsUnknown_PriceType = value;
                if (_IsUnknown_PriceType)
                {
                    IsFixed_PriceType = !_IsUnknown_PriceType;
                    IsUpdatable_PriceType = !_IsUnknown_PriceType;
                }
                NotifyOfPropertyChange(() => IsUnknown_PriceType);
            }
        }

        // Giá cố định
        private bool _IsFixed_PriceType = true;
        public bool IsFixed_PriceType
        {
            get
            {
                return _IsFixed_PriceType;
            }
            set
            {
                _IsFixed_PriceType = value;
                if (_IsFixed_PriceType)
                {
                    IsUnknown_PriceType = !_IsFixed_PriceType;
                    IsUpdatable_PriceType = !_IsFixed_PriceType;
                }
                NotifyOfPropertyChange(() => IsFixed_PriceType);
            }
        }

        // Giá không cố định
        private bool _IsUpdatable_PriceType;
        public bool IsUpdatable_PriceType
        {
            get
            {
                return _IsUpdatable_PriceType;
            }
            set
            {
                _IsUpdatable_PriceType = value;
                if (_IsUpdatable_PriceType)
                {
                    IsUnknown_PriceType = !_IsUpdatable_PriceType;
                    IsFixed_PriceType = !_IsUpdatable_PriceType;
                }
                NotifyOfPropertyChange(() => IsUpdatable_PriceType);
            }
        }

        // Điều khiển radioButton

        public void rdbNewPriceType_Click(object sender, RoutedEventArgs e)
        {
            if (ObjRefMedicalServiceItem == null)
            {
                return;
            }
            if (IsUnknown_PriceType)
            {
                ObjRefMedicalServiceItem.V_NewPriceType = Convert.ToInt32(AllLookupValues.V_NewPriceType.Unknown_PriceType);
            }
            else if (IsFixed_PriceType)
            {
                ObjRefMedicalServiceItem.V_NewPriceType = Convert.ToInt32(AllLookupValues.V_NewPriceType.Fixed_PriceType);
            }
            else
            {
                ObjRefMedicalServiceItem.V_NewPriceType = Convert.ToInt32(AllLookupValues.V_NewPriceType.Updatable_PriceType);
            }
        }

        public void SetRadioButton()
        {
            if (ObjRefMedicalServiceItem == null || ObjRefMedicalServiceItem.V_NewPriceType <= 0)
            {
                return;
            }
            if (ObjRefMedicalServiceItem.V_NewPriceType == Convert.ToInt32(AllLookupValues.V_NewPriceType.Unknown_PriceType))
            {
                IsUnknown_PriceType = true;
            }
            else if (ObjRefMedicalServiceItem.V_NewPriceType == Convert.ToInt32(AllLookupValues.V_NewPriceType.Fixed_PriceType))
            {
                IsFixed_PriceType = true;
            }
            else
            {
                IsUpdatable_PriceType = true;
            }
        }
        #endregion

        /*TMA*/
        // Phẫu thuật
        private bool _IsSurgeryItem;
        public bool IsSurgeryItem
        {
            get
            {
                return _IsSurgeryItem;
            }
            set
            {
                _IsSurgeryItem = value;
                if (_IsSurgeryItem)
                {
                    IsTipsItem = !_IsSurgeryItem;
                }
                NotifyOfPropertyChange(() => IsSurgeryItem);
            }
        }

        // Thủ thuật
        private bool _IsTipsItem = true;
        public bool IsTipsItem
        {
            get
            {
                return _IsTipsItem;
            }
            set
            {
                _IsTipsItem = value;
                if (_IsTipsItem)
                {
                    IsSurgeryItem = !_IsTipsItem;
                }
                NotifyOfPropertyChange(() => IsTipsItem);
            }
        }

        //rdbSurgery_Tips_Item_Click

        public void rdbSurgery_Tips_Item_Click(object sender, RoutedEventArgs e)
        {
            if (ObjRefMedicalServiceItem == null)
            {
                return;
            }
            if (IsSurgeryItem) //phẫu thuật
            {
                ObjRefMedicalServiceItem.V_Surgery_Tips_Item = (long)AllLookupValues.V_Surgery_Tips_Item.PHAUTHUAT;
            }
            else if (IsTipsItem) //thủ thuật
            {
                ObjRefMedicalServiceItem.V_Surgery_Tips_Item = (long)AllLookupValues.V_Surgery_Tips_Item.THUTHUAT;
            }
        }

        public void SetRadioButtonSurgery_Tips_Item()
        {
            if (ObjRefMedicalServiceItem == null || ObjRefMedicalServiceItem.V_Surgery_Tips_Item <= 0)
            {
                IsTipsItem = false;
                IsSurgeryItem = false;
                return;
            }
            if (ObjRefMedicalServiceItem.V_Surgery_Tips_Item == (long)AllLookupValues.V_Surgery_Tips_Item.PHAUTHUAT)
            {
                IsSurgeryItem = true;
                IsTipsItem = false;
                ObjRefMedicalServiceItem.V_Surgery_Tips_Item = (long)AllLookupValues.V_Surgery_Tips_Item.PHAUTHUAT;
            }
            else if (ObjRefMedicalServiceItem.V_Surgery_Tips_Item == (long)AllLookupValues.V_Surgery_Tips_Item.THUTHUAT)
            {
                IsTipsItem = true;
                IsSurgeryItem = false;
                ObjRefMedicalServiceItem.V_Surgery_Tips_Item = (long)AllLookupValues.V_Surgery_Tips_Item.THUTHUAT;
            }
            else
            {
                IsTipsItem = false;
                IsSurgeryItem = false;
            }
        }

        // Loại Phẫu - thủ thuật
        private Lookup _ObjV_Surgery_Tips_Item_Selected;
        public Lookup ObjV_Surgery_Tips_Item_Selected
        {
            get { return _ObjV_Surgery_Tips_Item_Selected; }
            set
            {
                _ObjV_Surgery_Tips_Item_Selected = value;
                NotifyOfPropertyChange(() => ObjV_Surgery_Tips_Item_Selected);
            }
        }

        private ObservableCollection<Lookup> _ObjV_Surgery_Tips_Item;
        public ObservableCollection<Lookup> ObjV_Surgery_Tips_Item
        {
            get { return _ObjV_Surgery_Tips_Item; }
            set
            {
                _ObjV_Surgery_Tips_Item = value;
                NotifyOfPropertyChange(() => ObjV_Surgery_Tips_Item);
            }
        }

        private Lookup _ObjV_Surgery_Tips_Type_Selected;
        public Lookup ObjV_Surgery_Tips_Type_Selected
        {
            get { return _ObjV_Surgery_Tips_Type_Selected; }
            set
            {
                _ObjV_Surgery_Tips_Type_Selected = value;
                NotifyOfPropertyChange(() => ObjV_Surgery_Tips_Type_Selected);
            }
        }

        private ObservableCollection<Lookup> _ObjV_Surgery_Tips_Type;
        public ObservableCollection<Lookup> ObjV_Surgery_Tips_Type
        {
            get { return _ObjV_Surgery_Tips_Type; }
            set
            {
                _ObjV_Surgery_Tips_Type = value;
                NotifyOfPropertyChange(() => ObjV_Surgery_Tips_Type);
            }
        }

        private ObservableCollection<Lookup> _ObjV_DVKTPhanTuyen;
        public ObservableCollection<Lookup> ObjV_DVKTPhanTuyen
        {
            get { return _ObjV_DVKTPhanTuyen; }
            set
            {
                _ObjV_DVKTPhanTuyen = value;
                NotifyOfPropertyChange(() => ObjV_DVKTPhanTuyen);
            }
        }

        public void LoadV_Surgery_Tips_Type()
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0669_G1_DangLayDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLookupValuesByType(LookupValues.V_Surgery_Tips_Type,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_Surgery_Tips_Type = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup();
                                    /*TMA 03/11/2017*/
                                    if (ObjRefMedicalServiceItem.ObjV_Surgery_Tips_Type != null || ObjRefMedicalServiceItem.V_Surgery_Tips_Type > 0)
                                    {
                                        ObjRefMedicalServiceItem.V_Surgery_Tips_Type = ObjRefMedicalServiceItem.ObjV_Surgery_Tips_Type.LookupID;
                                        if (ObjRefMedicalServiceItem.ObjV_Surgery_Tips_Item != null || ObjRefMedicalServiceItem.V_Surgery_Tips_Item > 0)
                                        {
                                            ObjRefMedicalServiceItem.V_Surgery_Tips_Item = ObjRefMedicalServiceItem.ObjV_Surgery_Tips_Item.LookupID;
                                            SetRadioButtonSurgery_Tips_Item();
                                        }
                                    }
                                    firstItem.LookupID = -1;
                                    firstItem.ObjectValue = eHCMSResources.A0015_G1_Chon;
                                    ObjV_Surgery_Tips_Type.Insert(0, firstItem);

                                    if (ObjDeptMedServiceItems_Save != null && ObjDeptMedServiceItems_Save.V_Surgery_Tips_Type <= 0)
                                    {
                                        ObjDeptMedServiceItems_Save.V_Surgery_Tips_Type = -1;
                                    }
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
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        //IsSurgeryTips
        public bool IsSurgeryTips
        {
            get
            {
                if (ObjRefMedicalServiceTypes_GetAll != null && ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID != 0) // && ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID == -1 
                {
                    if (ObjRefMedicalServiceItem.MedicalServiceTypeID != 0)
                    {
                        var ID = Convert.ToInt32(ObjRefMedicalServiceItem.MedicalServiceTypeID);
                        long Selected = (long)ObjRefMedicalServiceTypes_GetAll.Where(x => x.MedicalServiceTypeID == ID).FirstOrDefault().V_RefMedicalServiceTypes;
                        if (Selected == (long)AllLookupValues.V_RefMedicalServiceTypes.THUTHUAT || Selected == (long)AllLookupValues.V_RefMedicalServiceTypes.KYTHUATCAO)
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else
                {
                    if (ObjRefMedicalServiceTypeSelected != null && (ObjRefMedicalServiceTypeSelected.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.THUTHUAT || ObjRefMedicalServiceTypeSelected.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KYTHUATCAO))
                        return true;
                    else
                        return false;
                }
            }
        }

        //▼===== #002
        ComboBox cboV_SpecialistType;
        ComboBox cboMedicalServiceTypesSubTractPCL;
        private bool _vV_SpecialistType;
        public bool vV_SpecialistType
        {
            get { return _vV_SpecialistType; }
            set
            {
                _vV_SpecialistType = value;
                NotifyOfPropertyChange(() => vV_SpecialistType);
            }
        }

        private ObservableCollection<Lookup> _ObjV_SpecialistType;
        public ObservableCollection<Lookup> ObjV_SpecialistType
        {
            get { return _ObjV_SpecialistType; }
            set
            {
                _ObjV_SpecialistType = value;
                NotifyOfPropertyChange(() => ObjV_SpecialistType);
            }
        }

        private RefMedicalServiceItem _tmpRefMedicalServiceItem;
        public RefMedicalServiceItem tmpRefMedicalServiceItem
        {
            get { return _tmpRefMedicalServiceItem; }
            set
            {
                _tmpRefMedicalServiceItem = value;
                NotifyOfPropertyChange(() => tmpRefMedicalServiceItem);
            }
        }

        public void LoadV_SpecialistType()
        {
            var t = new Thread(() =>
            {
                this.DlgShowBusyIndicator(eHCMSResources.Z0669_G1_DangLayDLieu);
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_SpecialistType,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_SpecialistType = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup();
                                    firstItem.LookupID = -1;
                                    firstItem.ObjectValue = "--Tùy Chọn--";
                                    ObjV_SpecialistType.Insert(0, firstItem);

                                    if (ObjDeptMedServiceItems_Save != null && ObjDeptMedServiceItems_Save.ObjRefMedicalServiceItem.V_SpecialistType <= 0)
                                    {
                                        ObjDeptMedServiceItems_Save.ObjRefMedicalServiceItem.V_SpecialistType = -1;
                                    }
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
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        public void cboV_SpecialistType_Loaded(object sender)
        {
            cboV_SpecialistType = sender as ComboBox;
        }

        public void cboMedicalServiceTypesSubTractPCL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboMedicalServiceTypesSubTractPCL == null)
            {
                return;
            }
            if (tmpRefMedicalServiceItem == null)
            {
                tmpRefMedicalServiceItem = new RefMedicalServiceItem();
                tmpRefMedicalServiceItem.V_SpecialistType = ObjV_SpecialistType[0].LookupID;
            }
            if (cboMedicalServiceTypesSubTractPCL.SelectedItem != null
                && (cboMedicalServiceTypesSubTractPCL.SelectedItem as RefMedicalServiceType).V_RefMedicalServiceTypes != (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH)
            {
                cboV_SpecialistType.SelectedItem = _ObjV_SpecialistType.FirstOrDefault();
            }
            if (cboMedicalServiceTypesSubTractPCL.SelectedItem != null
                && (cboMedicalServiceTypesSubTractPCL.SelectedItem as RefMedicalServiceType).V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH
                && ObjRefMedicalServiceItem != null && ObjV_SpecialistType != null)
            {
                foreach (var tmpObjV_SpecialistType in ObjV_SpecialistType)
                {
                    if (tmpRefMedicalServiceItem.V_SpecialistType == tmpObjV_SpecialistType.LookupID)
                    {
                        cboV_SpecialistType.SelectedItem = tmpObjV_SpecialistType;
                    }
                }
            }
        }

        public void cboMedicalServiceTypesSubTractPCL_Loaded(object sender, RoutedEventArgs e)
        {
            cboMedicalServiceTypesSubTractPCL = sender as ComboBox;
        }
        //▲===== #002

        private ObservableCollection<Gender> _genders;
        public ObservableCollection<Gender> Genders
        {
            get
            {
                return _genders;
            }
            set
            {
                _genders = value;
                NotifyOfPropertyChange(() => Genders);
            }
        }

        private void SetValuesForGender()
        {
            if (Genders == null)
            {
                Genders = new ObservableCollection<Gender>();
            }
            Genders.Add(new Gender("A", "Tất cả"));
            Genders.Add(new Gender("M", "Nam"));
            Genders.Add(new Gender("F", "Nữ"));
            if (ObjRefMedicalServiceItem.GenderType == null)
            {
                ObjRefMedicalServiceItem.GenderType = Genders.First().ID;
            }
        }
    }
}
