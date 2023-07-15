using eHCMSLanguage;
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
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ViewContracts;

namespace aEMR.Configuration.RefMedicalServiceItems_IsPCL.ViewModels
{
    [Export(typeof(IRefMedicalServiceItems_IsPCL_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefMedicalServiceItems_IsPCL_AddEditViewModel : Conductor<object>, IRefMedicalServiceItems_IsPCL_AddEdit
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
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RefMedicalServiceItems_IsPCL_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            ObjRefMedicalServiceTypes_ByV_RefMedicalServiceTypes = new ObservableCollection<RefMedicalServiceType>();
            RefMedicalServiceTypes_ByV_RefMedicalServiceTypes();

            ObjV_RefMedServiceItemsUnit=new ObservableCollection<Lookup>();
            LoadV_RefMedServiceItemsUnit();


        }

        //Loại DV
        private ObservableCollection<RefMedicalServiceType> _ObjRefMedicalServiceTypes_ByV_RefMedicalServiceTypes;
        public ObservableCollection<RefMedicalServiceType> ObjRefMedicalServiceTypes_ByV_RefMedicalServiceTypes
        {
            get { return _ObjRefMedicalServiceTypes_ByV_RefMedicalServiceTypes; }
            set
            {
                _ObjRefMedicalServiceTypes_ByV_RefMedicalServiceTypes = value;
                NotifyOfPropertyChange(() => ObjRefMedicalServiceTypes_ByV_RefMedicalServiceTypes);
            }
        }

        public void RefMedicalServiceTypes_ByV_RefMedicalServiceTypes()
        {
            ObjRefMedicalServiceTypes_ByV_RefMedicalServiceTypes.Clear();

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Loại Dịch Vụ..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefMedicalServiceTypes_ByV_RefMedicalServiceTypes((long)AllLookupValues.V_RefMedicalServiceTypes.CANLAMSANG, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndRefMedicalServiceTypes_ByV_RefMedicalServiceTypes(asyncResult);

                            if (items != null)
                            {
                                ObjRefMedicalServiceTypes_ByV_RefMedicalServiceTypes = new ObservableCollection<DataEntities.RefMedicalServiceType>(items);

                                //Item Default
                                RefMedicalServiceType ItemDefault = new RefMedicalServiceType();
                                ItemDefault.MedicalServiceTypeID = -1;
                                ItemDefault.MedicalServiceTypeName = "--Chọn Loại Dịch Vụ--";
                                //Item Default

                                ObjRefMedicalServiceTypes_ByV_RefMedicalServiceTypes.Insert(0, ItemDefault);

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

        //Loại DV


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
                                    firstItem.ObjectValue = "--Chọn Đơn Vị--";
                                    ObjV_RefMedServiceItemsUnit.Insert(0, firstItem);

                                    if(ObjRefMedicalServiceItem_Current.V_RefMedServiceItemsUnit<=0)
                                    {
                                        ObjRefMedicalServiceItem_Current.V_RefMedServiceItemsUnit = -1;
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

        private RefMedicalServiceItem _ObjRefMedicalServiceItem_Current;
        public RefMedicalServiceItem ObjRefMedicalServiceItem_Current
        {
            get { return _ObjRefMedicalServiceItem_Current; }
            set
            {
                _ObjRefMedicalServiceItem_Current = value;
                NotifyOfPropertyChange(()=>ObjRefMedicalServiceItem_Current);
            }
        }
        
        public void InitializeNewItem()
        {
            ObjRefMedicalServiceItem_Current = new RefMedicalServiceItem();
            ObjRefMedicalServiceItem_Current.MedicalServiceTypeID = -1;
            ObjRefMedicalServiceItem_Current.MedServiceCode = "";
            ObjRefMedicalServiceItem_Current.MedServiceName = "";
            ObjRefMedicalServiceItem_Current.ExpiryDate = DateTime.Now.AddYears(20);//Mặc định 20 năm hết hạn
            ObjRefMedicalServiceItem_Current.IsActive = true;
        }

        public void btSave()
        {
            if(ObjRefMedicalServiceItem_Current!=null)
            {
                if (CheckValid(ObjRefMedicalServiceItem_Current))
                {
                    if (ObjRefMedicalServiceItem_Current.MedicalServiceTypeID > 0)
                    {
                        if (ObjRefMedicalServiceItem_Current.V_RefMedServiceItemsUnit > 0)
                        {
                            RefMedicalServiceItems_IsPCL_Save();
                        }
                        else
                        {
                            MessageBox.Show(eHCMSResources.A0310_G1_Msg_InfoChonDVT, eHCMSResources.A0015_G1_Chon, MessageBoxButton.OK);
                        }
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0327_G1_Msg_InfoChonLoaiDV, eHCMSResources.A0015_G1_Chon, MessageBoxButton.OK);
                    }
                }
            }
        }


        public void RefMedicalServiceItems_IsPCL_Save()
        {
            string Result = "";

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Ghi..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefMedicalServiceItems_IsPCL_Save(ObjRefMedicalServiceItem_Current, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndRefMedicalServiceItems_IsPCL_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Insert-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, eHCMSResources.A0465_G1_Msg_ThemMoiGoiDVPCL, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-1":
                                    {
                                        Globals.EventAggregator.Publish(new RefMedicalServiceItems_IsPCL_AddEditViewModel_Save_Event() { Result = true });
                                        MessageBox.Show(eHCMSResources.Z0655_G1_DaGhi, eHCMSResources.A0465_G1_Msg_ThemMoiGoiDVPCL, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.A0275_G1_Msg_CNhatGoiDVPCL, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new RefMedicalServiceItems_IsPCL_AddEditViewModel_Save_Event() { Result = true });
                                        TitleForm = string.Format("Hiệu Chỉnh Gói Dịch Vụ PCL: ({0})", ObjRefMedicalServiceItem_Current.MedServiceName.Trim());
                                        MessageBox.Show(eHCMSResources.K2782_G1_DaCNhat, eHCMSResources.A0275_G1_Msg_CNhatGoiDVPCL, MessageBoxButton.OK);
                                        break;
                                    }
                                //default:
                                //    {
                                //        string[] arrRes = new string[2];
                                //        arrRes = Result.Split('@');

                                //        string msg1 = "";
                                //        string msg2 = "";

                                //        if (arrRes[0] != "")
                                //        {
                                //            msg1 = "Ngày Hết Hạn Của Dịch Vụ Này Phải > Ngày Hiện Hành!";
                                //        }

                                //        if (arrRes[1] != "")
                                //        {
                                //            msg2 = "Mã Gói Dịch Vụ Cận Lâm Sàng Này Đã Được Sử Dụng!";
                                //        }

                                //        string msg = (msg1 == "" ? "" : "- " + msg1) + (msg2 != "" ? Environment.NewLine + "- " + msg2 : "");

                                //        MessageBox.Show(msg + Environment.NewLine + "Vui Lòng Nhập Khác!", "Lưu", MessageBoxButton.OK);
                                //        break;
                                //    }
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

        public bool CheckValid(object temp)
        {
            DataEntities.RefMedicalServiceItem p = temp as DataEntities.RefMedicalServiceItem;
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }


        public void btClose()
        {
            TryClose();
        }

        
    }
}
