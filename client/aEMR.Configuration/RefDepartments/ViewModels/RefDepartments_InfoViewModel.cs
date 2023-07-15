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

/*
 * 20230520 #001 DatTB: Thêm cột tên tiếng anh các danh mục khoa phòng
*/
namespace aEMR.Configuration.RefDepartments.ViewModels
{
    [Export(typeof(IRefDepartments_Info)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefDepartments_InfoViewModel : Conductor<object>, IRefDepartments_Info
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
        public RefDepartments_InfoViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            Load_V_DeptTypeOperation();
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

        //V_DeptTypeOperation
        private ObservableCollection<Lookup> _ObjV_DeptTypeOperation;
        public ObservableCollection<Lookup> ObjV_DeptTypeOperation
        {
            get { return _ObjV_DeptTypeOperation; }
            set
            {
                _ObjV_DeptTypeOperation = value;
                NotifyOfPropertyChange(() => ObjV_DeptTypeOperation);
            }
        }
        public void Load_V_DeptTypeOperation()
        {
            var t = new Thread(() =>
            {
                IsLoading = true;

                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_DeptTypeOperation,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_DeptTypeOperation = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup();
                                    firstItem.LookupID = -1;
                                    firstItem.ObjectValue = eHCMSResources.A0015_G1_Chon;
                                    ObjV_DeptTypeOperation.Insert(0, firstItem);
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
        //V_DeptTypeOperation


        public void InitializeRefDepartments_New(object NodeTreeParent)
        {
            DataEntities.RefDepartmentsTree ObjParent = (NodeTreeParent as DataEntities.RefDepartmentsTree);

            DataEntities.RefDepartments ObjNew = new DataEntities.RefDepartments();
            ObjRefDepartments_ParDeptID_Current = ConvertRefDepartmentsTree2RefDepartments(NodeTreeParent);
            ObjNew.ParDeptID = ObjParent.NodeID;
            ObjNew.V_DeptType = ObjParent.V_DeptType;
            ObjNew.V_DeptTypeOperation = -1;

            ObjRefDepartments_Current = ObjNew;
        }

        public DataEntities.RefDepartments ConvertRefDepartmentsTree2RefDepartments(object pobjNode)
        {
            RefDepartmentsTree objNode = (pobjNode as RefDepartmentsTree);

            DataEntities.RefDepartments obj = new DataEntities.RefDepartments();
            obj.DeptID = objNode.NodeID;
            obj.ParDeptID = objNode.ParentID;
            obj.V_DeptType = objNode.V_DeptType;
            obj.V_DeptTypeOperation = objNode.V_DeptTypeOperation;
            obj.DeptName = objNode.NodeText;
            //▼==== #001
            obj.DeptNameEng = objNode.DeptNameEng;
            //▲==== #001
            obj.DeptDescription = objNode.Description;
            return obj;
        }

        private object _ObjRefDepartments_Current;
        public object ObjRefDepartments_Current
        {
            get { return _ObjRefDepartments_Current; }
            set
            {
                _ObjRefDepartments_Current = value;
                NotifyOfPropertyChange(() => ObjRefDepartments_Current);
            }
        }

        private object _ObjRefDepartments_ParDeptID_Current;
        public object ObjRefDepartments_ParDeptID_Current
        {
            get { return _ObjRefDepartments_ParDeptID_Current; }
            set
            {
                _ObjRefDepartments_ParDeptID_Current = value;
                NotifyOfPropertyChange(() => ObjRefDepartments_ParDeptID_Current);
            }
        }

        private ObservableCollection<DataEntities.RefDepartments> _ObjRefDepartment_SubtractAllChild_ByDeptID;
        public ObservableCollection<DataEntities.RefDepartments> ObjRefDepartment_SubtractAllChild_ByDeptID
        {
            get { return _ObjRefDepartment_SubtractAllChild_ByDeptID; }
            set
            {
                _ObjRefDepartment_SubtractAllChild_ByDeptID = value;
                NotifyOfPropertyChange(() => ObjRefDepartment_SubtractAllChild_ByDeptID);
            }
        }


        public void RefDepartment_SubtractAllChild_ByDeptID(Int64 DeptID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Khoa..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefDepartment_SubtractAllChild_ByDeptID(DeptID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndRefDepartment_SubtractAllChild_ByDeptID(asyncResult);
                            if (items != null)
                            {
                                ObjRefDepartment_SubtractAllChild_ByDeptID = new ObservableCollection<DataEntities.RefDepartments>(items);
                            }
                            else
                            {
                                ObjRefDepartment_SubtractAllChild_ByDeptID = null;
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

        public void btSave()
        {
            DataEntities.RefDepartments ObjSave = (ObjRefDepartments_Current as DataEntities.RefDepartments);


            if (ObjSave != null)
            {
                if (CheckValid(ObjSave)==false)
                    return;

                if (ObjSave.DeptID > 0)
                {
                    if (ObjSave.ParDeptID > 0)//Kiểm tra Cha
                    {
                        if (!string.IsNullOrEmpty(ObjSave.DeptName))
                        {
                            UpdateRefDepartments();
                        }
                    }
                    else//Sửa Root
                    {
                        if (!string.IsNullOrEmpty(ObjSave.DeptName))
                        {
                            UpdateRefDepartments();
                        }
                    }
                }
                else
                {
                    if (ObjSave.ParDeptID > 0)
                    {
                        if (!string.IsNullOrEmpty(ObjSave.DeptName))
                        {
                            AddNewRefDepartments();
                        }
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0304_G1_Msg_InfoChonCapCha, eHCMSResources.G0276_G1_ThemMoi, MessageBoxButton.OK);
                    }
                }
            }
        }

        private bool CheckValid(DataEntities.RefDepartments ObjSave)
        {
            if (string.IsNullOrEmpty(ObjSave.DeptName))
            {
                MessageBox.Show(eHCMSResources.A0881_G1_Msg_InfoNhapTen);
                return false;
            }

            if(ObjSave.V_DeptTypeOperation<=0)
            {
                MessageBox.Show(eHCMSResources.A0366_G1_Msg_InfoChonThuocLoai);
                return false;
            }
            return true;
        }

        public void SetInfo_ObjRefDepartments_Current(object objNode)
        {
            ObjRefDepartments_Current = ConvertRefDepartmentsTree2RefDepartments(objNode);
        }

        public void btClose()
        {
            TryClose();
            Globals.EventAggregator.Publish(new RefDepartments_SaveEvent());
        }

        public void UpdateRefDepartments()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Ghi..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginUpdateRefDepartments(ObjRefDepartments_Current as DataEntities.RefDepartments, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";

                            contract.EndUpdateRefDepartments(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.T1484_G1_HChinh, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new SaveEvent<bool>() { Result = true });
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T1484_G1_HChinh, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Parent-HasDeptLocation":
                                    {
                                        MessageBox.Show(eHCMSResources.A0266_G1_Msg_InfoKhTheThemKhoaConVaoCapCha, eHCMSResources.T1484_G1_HChinh, MessageBoxButton.OK);
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

        public void AddNewRefDepartments()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Ghi..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginAddNewRefDepartments(ObjRefDepartments_Current as DataEntities.RefDepartments, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            if (contract.EndAddNewRefDepartments(asyncResult))
                            {
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0276_G1_ThemMoi, MessageBoxButton.OK);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z0654_G1_GhiKgThCong, eHCMSResources.G0276_G1_ThemMoi, MessageBoxButton.OK);
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
