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
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.Configuration.RefMedicalServiceTypes.ViewModels
{
    [Export(typeof(IRefMedicalServiceTypes_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefMedicalServiceTypes_AddEditViewModel : Conductor<object>, IRefMedicalServiceTypes_AddEdit
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
        public RefMedicalServiceTypes_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            ObjRefMedicalServiceGroups_GetAll = new ObservableCollection<RefMedicalServiceGroups>();
            RefMedicalServiceGroups_GetAll();

            ObjV_RefMedicalServiceInOutOthers=new ObservableCollection<Lookup>();
            Load_V_RefMedicalServiceInOutOthers();
            
            ObjV_RefMedicalServiceTypes=new ObservableCollection<Lookup>();
            Load_V_RefMedicalServiceTypes();

            //ObjHITransactionType_GetListNoParentID=new ObservableCollection<HITransactionType>();
            //HITransactionType_GetListNoParentID();
        }

        //V_RefMedicalServiceInOutOthers
        private ObservableCollection<Lookup> _ObjV_RefMedicalServiceInOutOthers;
        public ObservableCollection<Lookup> ObjV_RefMedicalServiceInOutOthers
        {
            get { return _ObjV_RefMedicalServiceInOutOthers; }
            set
            {
                _ObjV_RefMedicalServiceInOutOthers = value;
                NotifyOfPropertyChange(() => ObjV_RefMedicalServiceInOutOthers);
            }
        }
        public void Load_V_RefMedicalServiceInOutOthers()
        {
            var t = new Thread(() =>
            {
                //Globals.EventAggregator.Publish(new BusyEvent{IsBusy = true,Message ="Danh Sách Chọn..."});

                IsLoading = true;

                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_RefMedicalServiceInOutOthers,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_RefMedicalServiceInOutOthers = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup();
                                    firstItem.LookupID = -1;
                                    firstItem.ObjectValue = eHCMSResources.A0015_G1_Chon;
                                    ObjV_RefMedicalServiceInOutOthers.Insert(0, firstItem);
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
                    IsLoading=false;    
                }
            });
            t.Start();
        }
        //V_RefMedicalServiceInOutOthers


        //RefMedicalServiceGroups
        private ObservableCollection<RefMedicalServiceGroups> _ObjRefMedicalServiceGroups_GetAll;
        public ObservableCollection<RefMedicalServiceGroups> ObjRefMedicalServiceGroups_GetAll
        {
            get { return _ObjRefMedicalServiceGroups_GetAll; }
            set
            {
                _ObjRefMedicalServiceGroups_GetAll = value;
                NotifyOfPropertyChange(() => ObjRefMedicalServiceGroups_GetAll);
            }
        }

        public void RefMedicalServiceGroups_GetAll()
        {
            ObjRefMedicalServiceGroups_GetAll.Clear();

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách RefMedicalServiceGroups..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefMedicalServiceGroups_GetAll(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndRefMedicalServiceGroups_GetAll(asyncResult);

                            if (items != null)
                            {
                                ObjRefMedicalServiceGroups_GetAll = new ObservableCollection<DataEntities.RefMedicalServiceGroups>(items);

                                //ItemDefault
                                DataEntities.RefMedicalServiceGroups ItemDefault = new DataEntities.RefMedicalServiceGroups();
                                ItemDefault.MedicalServiceGroupID = -1;
                                ItemDefault.MedicalServiceGroupName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon);
                                ObjRefMedicalServiceGroups_GetAll.Insert(0, ItemDefault);
                                //ItemDefault
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
        //RefMedicalServiceGroups




        private ObservableCollection<Lookup> _ObjV_RefMedicalServiceTypes;
        public ObservableCollection<Lookup> ObjV_RefMedicalServiceTypes
        {
            get { return _ObjV_RefMedicalServiceTypes; }
            set
            {
                _ObjV_RefMedicalServiceTypes = value;
                NotifyOfPropertyChange(() => ObjV_RefMedicalServiceTypes);
            }
        }
        public void Load_V_RefMedicalServiceTypes()
        {
            var t = new Thread(() =>
            {
                //Globals.EventAggregator.Publish(new BusyEvent{IsBusy = true,Message ="Danh Sách Thuộc Loại ..."});

                IsLoading = true;

                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_RefMedicalServiceTypes,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_RefMedicalServiceTypes = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup();
                                    firstItem.LookupID = -1;
                                    firstItem.ObjectValue = eHCMSResources.A0015_G1_Chon;
                                    ObjV_RefMedicalServiceTypes.Insert(0, firstItem);
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



        
        //HITransactionType
        //private ObservableCollection<HITransactionType> _ObjHITransactionType_GetListNoParentID;
        //public ObservableCollection<HITransactionType> ObjHITransactionType_GetListNoParentID
        //{
        //    get { return _ObjHITransactionType_GetListNoParentID; }
        //    set
        //    {
        //        _ObjHITransactionType_GetListNoParentID = value;
        //        NotifyOfPropertyChange(() => ObjHITransactionType_GetListNoParentID);
        //    }
        //}

        //public void HITransactionType_GetListNoParentID()
        //{
        //    ObjHITransactionType_GetListNoParentID.Clear();

        //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách HITransactionType..." });

        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new ConfigurationManagerServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            contract.BeginHITransactionType_GetListNoParentID(Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    var items = contract.EndHITransactionType_GetListNoParentID(asyncResult);

        //                    if (items != null)
        //                    {
        //                        ObjHITransactionType_GetListNoParentID = new ObservableCollection<DataEntities.HITransactionType>(items);

        //                        //ItemDefault
        //                        DataEntities.HITransactionType ItemDefault = new DataEntities.HITransactionType();
        //                        ItemDefault.HITTypeID = -1;
        //                        ItemDefault.HITypeDescription = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon);
        //                        ObjHITransactionType_GetListNoParentID.Insert(0, ItemDefault);
        //                        //ItemDefault
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    Globals.IsBusy = false;
        //                }
        //            }), null);
        //        }


        //    });
        //    t.Start();
        //}
        //HITransactionType

        private DataEntities.RefMedicalServiceType _ObjRefMedicalServiceTypes_Current;
        public DataEntities.RefMedicalServiceType ObjRefMedicalServiceTypes_Current
        {
            get { return _ObjRefMedicalServiceTypes_Current; }
            set
            {
                _ObjRefMedicalServiceTypes_Current = value;
                NotifyOfPropertyChange(() => ObjRefMedicalServiceTypes_Current);
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

        public void InitializeNewItem()
        {
            ObjRefMedicalServiceTypes_Current = new DataEntities.RefMedicalServiceType();
            ObjRefMedicalServiceTypes_Current.MedicalServiceGroupID = -1;
            ObjRefMedicalServiceTypes_Current.V_RefMedicalServiceInOutOthers = -1;
            ObjRefMedicalServiceTypes_Current.V_RefMedicalServiceTypes = -1;
            
        }

        public void btSave()
        {
            if (ObjRefMedicalServiceTypes_Current!=null)
            {
                if (ObjRefMedicalServiceTypes_Current.V_RefMedicalServiceInOutOthers<= 0)
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.A0346_G1_Msg_InfoChonNoi_NgTru));
                    return;
                }

                if (ObjRefMedicalServiceTypes_Current.MedicalServiceGroupID <= 0)
                {
                    MessageBox.Show(eHCMSResources.A0344_G1_Msg_InfoChonNhom);
                    return;
                }


                if (ObjRefMedicalServiceTypes_Current.V_RefMedicalServiceTypes<= 0)
                {
                    MessageBox.Show(eHCMSResources.A0366_G1_Msg_InfoChonThuocLoai);
                    return;
                }

                if (CheckValid(ObjRefMedicalServiceTypes_Current))
                {
                    RefMedicalServiceTypes_AddEdit(ObjRefMedicalServiceTypes_Current);
                }
                
            }
        }

        public bool CheckValid(object temp)
        {
            DataEntities.RefMedicalServiceType p = temp as DataEntities.RefMedicalServiceType;
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

        private void RefMedicalServiceTypes_AddEdit(DataEntities.RefMedicalServiceType Obj)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefMedicalServiceTypes_AddEdit(Obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndRefMedicalServiceTypes_AddEdit(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Insert-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, eHCMSResources.A0466_G1_Msg_ThemMoiLoaiDV, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-1":
                                    {
                                        Globals.EventAggregator.Publish(new RefMedicalServiceTypes_AddEditEvent_Save() { Result = true });

                                        MessageBox.Show(eHCMSResources.Z0655_G1_DaGhi, eHCMSResources.A0466_G1_Msg_ThemMoiLoaiDV, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.A0276_G1_Msg_CNhatLoaiDV, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new RefMedicalServiceTypes_AddEditEvent_Save() { Result = true });

                                        TitleForm = string.Format("Hiệu Chỉnh Dịch Vụ PCL: ({0})", ObjRefMedicalServiceTypes_Current.MedicalServiceTypeName.Trim());
                                        MessageBox.Show(eHCMSResources.K2782_G1_DaCNhat, "Cập Nhật Dịch Vụ PCL", MessageBoxButton.OK);
                                        break;
                                    }
                                default:
                                    {
                                        string[] arrRes = new string[2];
                                        arrRes = Result.Split('@');

                                        string msg1 = "";
                                        string msg2 = "";

                                        if (arrRes[0] != "")
                                        {
                                            msg1 = "Mã Loại Dịch Vụ Này Đã Được Sử Dụng!";
                                        }

                                        if (arrRes[1] != "")
                                        {
                                            msg2 = "Tên Loại Dịch Vụ Này Đã Được Sử Dụng!";
                                        }

                                        string msg = (msg1 == "" ? "" : "- " + msg1) + (msg2 != "" ? Environment.NewLine + "- " + msg2 : "");

                                        MessageBox.Show(msg + Environment.NewLine + string.Format("{0}!", eHCMSResources.I0946_G1_I), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
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
                            IsLoading = false;    
                        }
                    }), null);
                }


            });
            t.Start();
        }

    }
}

