using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using DataEntities;
using aEMR.Common;
using eHCMSLanguage;
using Castle.Windsor;
using aEMR.ViewContracts;

namespace aEMR.Configuration.DeptMedServiceItems.ViewModels
{
    [Export(typeof(IDeptMedServiceItems_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeptMedServiceItems_AddEditViewModel : Conductor<object>, IDeptMedServiceItems_AddEdit
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

        
        private string _TitleForm;
        public string TitleForm
        {
            get { return _TitleForm; }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }
        [ImportingConstructor]
        public DeptMedServiceItems_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            ObjRefMedicalServiceTypes_GetAll=new ObservableCollection<RefMedicalServiceType>();
            ObjV_RefMedServiceItemsUnit=new ObservableCollection<Lookup>();
            LoadV_RefMedServiceItemsUnit();

            LoadV_AppointmentType();

            GetAllMedicalServiceTypes_SubtractPCL();

            ObjHITransactionType_GetListNoParentID=new ObservableCollection<HITransactionType>();
            HITransactionType_GetListNoParentID();
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

        public void LoadV_RefMedServiceItemsUnit()
        {
            var t = new Thread(() =>
            {
                //Globals.EventAggregator.Publish(new BusyEvent{IsBusy = true,Message = "Danh Sách Đơn Vị Tính..."});

                IsLoading = true;

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
                                    firstItem.LookupID = -1;
                                    firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, "Chọn đơn vị");
                                    ObjV_RefMedServiceItemsUnit.Insert(0, firstItem);

                                    if (ObjDeptMedServiceItems_Save.V_RefMedServiceItemsUnit <= 0)
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

                            }), null);
                    }
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

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách HITransactionType..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

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
                                ObjHITransactionType_GetListNoParentID = new ObservableCollection<DataEntities.HITransactionType>(items);

                                //ItemDefault
                                DataEntities.HITransactionType ItemDefault = new DataEntities.HITransactionType();
                                ItemDefault.HITTypeID = -1;
                                ItemDefault.HITypeDescription = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z1024_G1_TuyChon);
                                ObjHITransactionType_GetListNoParentID.Insert(0, ItemDefault);
                                //ItemDefault

                                if (ObjDeptMedServiceItems_Save.ObjRefMedicalServiceItem.HITTypeID <= 0 || ObjDeptMedServiceItems_Save.ObjRefMedicalServiceItem.HITTypeID==null)
                                {
                                    ObjDeptMedServiceItems_Save.ObjRefMedicalServiceItem.HITTypeID = -1;
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
                            IsLoading = false;
                        }
                    }), null);
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
                NotifyOfPropertyChange(()=>ObjRefMedicalServiceTypes_GetAll);
            }
        }
      
        public void GetAllMedicalServiceTypes_SubtractPCL()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Loại Dịch Vụ..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

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
                                RefMedicalServiceType ItemDefault = new RefMedicalServiceType();
                                ItemDefault.MedicalServiceTypeID = -1;
                                ItemDefault.MedicalServiceTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2027_G1_ChonLoaiDV);
                                //Item Default

                                ObjRefMedicalServiceTypes_GetAll.Insert(0,ItemDefault);
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
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
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
                NotifyOfPropertyChange(()=>ObjDeptMedServiceItems_Save);
            }
        }

        public void Init(DataEntities.DeptMedServiceItems ObjDeptMedServiceItems)
        {
            ObjDeptMedServiceItems_Save = ObjDeptMedServiceItems;

            ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.StaffID = Globals.LoggedUserAccount.StaffID.Value;

            ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.ObjStaffID = new Staff();
            ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.ObjApprovedStaffID = new Staff();

            ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.ObjStaffID.StaffID =
                ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.StaffID;
            ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.ApprovedStaffID =
                ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.StaffID;
            ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.ObjApprovedStaffID.StaffID =
                ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.ApprovedStaffID.Value;
            ObjDeptMedServiceItems_Save.ObjRefMedicalServiceItem = ObjDeptMedServiceItems.ObjRefMedicalServiceItem;

            ObjDeptMedServiceItems_Save.ObjApptService=new ApptService();

            // TxD 02/08/2014 Use Globals Server Date instead
            //GetCurrentDate();

            ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.EffectiveDate = Globals.GetCurServerDateTime();
        }

        public void btSave()
        {
            if (ObjRefMedicalServiceTypeSelected == null || ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0327_G1_Msg_InfoChonLoaiDV, eHCMSResources.K1916_G1_Chon, MessageBoxButton.OK);
                return; 
            }

            ObjDeptMedServiceItems_Save.ObjRefMedicalServiceItem.MedicalServiceTypeID = ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID;

            if (CheckValid(ObjDeptMedServiceItems_Save))
            {
                if(ObjDeptMedServiceItems_Save.ObjRefMedicalServiceItem.MedicalServiceTypeID>0)
                {   

                    if (ObjDeptMedServiceItems_Save.V_RefMedServiceItemsUnit > 0)
                    {
                        if (CheckGiaChenhLech())
                        {
                            RefMedicalServiceItems_NotPCL_Add();
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

        public bool CheckValid1(object temp)
        {
            DataEntities.RefMedicalServiceItem p = temp as DataEntities.RefMedicalServiceItem;
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }
        public bool CheckValid2(object temp)
        {
            DataEntities.MedServiceItemPrice p = temp as DataEntities.MedServiceItemPrice;
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }
        private  bool CheckValid(object p)
        {
            DataEntities.DeptMedServiceItems Objtemp=p as DataEntities.DeptMedServiceItems;

            DataEntities.RefMedicalServiceItem obj1 = Objtemp.ObjRefMedicalServiceItem;
            DataEntities.MedServiceItemPrice obj2 = Objtemp.ObjMedServiceItemPrice;
            
            return (CheckValid1(obj1) && CheckValid2(obj2));
        }

        private  bool CheckGiaChenhLech()
        {
            if (ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.NormalPrice>=1)
            {
                if(ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.NormalPrice<ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceForHIPatient)
                {
                    MessageBox.Show(eHCMSResources.Z0573_G1_GiaBNBHNhoHonDGia);
                    return false;
                }
                else
                {
                    if (ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.HIAllowedPrice != null && ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.HIAllowedPrice > 0)
                    {
                        if (ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.HIAllowedPrice>ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceForHIPatient)
                        {
                            MessageBox.Show(eHCMSResources.T1982_G1_GiaBHChoPhep2);
                            return  false;
                        }
                    }
                }

            }
            else
            {
                MessageBox.Show(eHCMSResources.Z0644_G1_DGiaPhaiLonHonBang1);
                return false;
            }
            return true;
        }

        private void RefMedicalServiceItems_NotPCL_Add()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Ghi..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefMedicalServiceItems_NotPCL_Add(ObjDeptMedServiceItems_Save, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";

                            contract.EndRefMedicalServiceItems_NotPCL_Add(out Result,asyncResult);
                            if(string.IsNullOrEmpty(Result))
                            {
                                 Globals.EventAggregator.Publish(new RefMedicalServiceItems_NotPCL_Add_Event() { Result = true });
                                        MessageBox.Show(eHCMSResources.Z0655_G1_DaGhi, eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
                            }
                            else
                            {
                                 MessageBox.Show(Result,eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
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
      

        public void btClose()
        {
            TryClose();
        }

        // TxD 02/08/2014 : The following method is nolonger required
        //public void GetCurrentDate()
        //{
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new CommonServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;

        //                contract.BeginGetDate(Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        DateTime date = contract.EndGetDate(asyncResult);
        //                        ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.EffectiveDate = date;

        //                    }
        //                    catch (FaultException<AxException> fault)
        //                    {
        //                        ClientLoggerHelper.LogInfo(fault.ToString());
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ClientLoggerHelper.LogInfo(ex.ToString());
        //                    }

        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //        }
        //    });
        //    t.Start();
        //}


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
                IsLoading = true;

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
                                    firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z1024_G1_TuyChon);
                                    ObjV_AppointmentType.Insert(0, firstItem);

                                    if (ObjDeptMedServiceItems_Save.ObjApptService.V_AppointmentType <= 0)
                                    {
                                        ObjDeptMedServiceItems_Save.ObjApptService.V_AppointmentType = -1;
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

                            }), null);
                    }
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
            });
            t.Start();
        }
        //Đơn Vị Tính

        private RefMedicalServiceType _ObjRefMedicalServiceTypeSelected;
        public RefMedicalServiceType ObjRefMedicalServiceTypeSelected
        {
            get
            {
                return _ObjRefMedicalServiceTypeSelected;
            }
            set
            {
                if(_ObjRefMedicalServiceTypeSelected!=value)
                {
                    _ObjRefMedicalServiceTypeSelected = value;

                    NotifyOfPropertyChange(() => ObjRefMedicalServiceTypeSelected);
                    
                    NotifyOfPropertyChange(() => cboV_AppointmentTypeIsEnabled);


                }
            }
        }

        public bool cboV_AppointmentTypeIsEnabled
        {
            get
            {
                return ObjRefMedicalServiceTypeSelected.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH;
            }
        }
       
        #endregion



        #region "LostFocus"
        public void LostFocus_NormalPrice(object NormalPrice)
        {
            if (ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.MedServItemPriceID <= 0)
            {
                decimal V = 0;

                if (IsNumeric(NormalPrice) == false)
                {
                    decimal.TryParse(NormalPrice.ToString(), out V);
                }
                else
                {
                    V = ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.NormalPrice;
                }

                ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.NormalPrice = V;
                    ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceForHIPatient = V;
                    if (ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.HIAllowedPrice != null &&
                        ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.HIAllowedPrice > 0)
                    {
                        if (ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceForHIPatient >=
                            ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.HIAllowedPrice)
                        {
                            ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceDifference =
                                ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceForHIPatient -
                                ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.HIAllowedPrice;
                        }
                    }
                    else
                    {
                        ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceDifference =
                            ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceForHIPatient;
                    }
                
            }
        }

        public void LostFocus_PriceForHIPatient(object PriceForHIPatient)
        {
            if (PriceForHIPatient != null)
            {
                decimal V = 0;

                if (IsNumeric(PriceForHIPatient) == false)
                {
                    decimal.TryParse(PriceForHIPatient.ToString(), out V);
                }
                else
                {
                    V = ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceForHIPatient.Value;
                }

                ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceForHIPatient = V;

                if (ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.HIAllowedPrice != null && ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.HIAllowedPrice> 0)
                {
                    if (ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceForHIPatient >= ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.HIAllowedPrice)
                    {
                        ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceDifference = ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceForHIPatient - ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.HIAllowedPrice;
                    }
                }
                else
                {
                    ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceDifference = ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceForHIPatient;
                }
            }
            else
            {
                ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceForHIPatient = null;
            }
        }

        public void LostFocus_HIAllowedPrice(object HIAllowedPrice)
        {
            if (HIAllowedPrice != null)
            {
                decimal V = 0;


                if (IsNumeric(HIAllowedPrice) == false)
                {
                    decimal.TryParse(HIAllowedPrice.ToString(), out V);
                }
                else
                {
                    V = ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.HIAllowedPrice.Value;
                }

                

                ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.HIAllowedPrice = V;

                if (ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.HIAllowedPrice != null && ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.HIAllowedPrice> 0)
                {
                    if (ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceForHIPatient >= ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.HIAllowedPrice)
                    {
                        ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceDifference = ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceForHIPatient - ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.HIAllowedPrice;
                    }
                }
            }
            else
            {
                ObjDeptMedServiceItems_Save.ObjMedServiceItemPrice.PriceForHIPatient = null;
            }
        }


        private static bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;
            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }
      
        #endregion

     
    }
}
