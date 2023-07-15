using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Controls;
using eHCMSLanguage;
using aEMR.Common.Collections;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ViewContracts;



namespace aEMR.Configuration.RefDepartmentReqCashAdv.ViewModels
{
    [Export(typeof(IRefDepartmentReqCashAdv_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefDepartmentReqCashAdv_AddEditViewModel : Conductor<object>, IRefDepartmentReqCashAdv_AddEdit
        ,IHandle<RefDepartments_BystrV_DeptTypeEvent.TreeSelectionChanged_Event>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RefDepartmentReqCashAdv_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            //Load UC
            var UCRefDepartments_BystrV_DeptTypeViewModel = Globals.GetViewModel<IRefDepartments_BystrV_DeptType>();
            UCRefDepartments_BystrV_DeptTypeViewModel.strV_DeptType = "7000";
            UCRefDepartments_BystrV_DeptTypeViewModel.ShowDeptLocation = false;
            UCRefDepartments_BystrV_DeptTypeViewModel.Parent = this;
            UCRefDepartments_BystrV_DeptTypeViewModel.RefDepartments_Tree();
            leftContent = UCRefDepartments_BystrV_DeptTypeViewModel;
            (this as Conductor<object>).ActivateItem(leftContent);
            //Load UC
        }

        #region Properties member

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

        private DataEntities.RefDepartmentReqCashAdv _ObjRefDepartmentReqCashAdv_Current;
        public DataEntities.RefDepartmentReqCashAdv ObjRefDepartmentReqCashAdv_Current
        {
            get { return _ObjRefDepartmentReqCashAdv_Current; }
            set
            {
                _ObjRefDepartmentReqCashAdv_Current = value;
                NotifyOfPropertyChange(() => ObjRefDepartmentReqCashAdv_Current);
            }
        }


        #endregion

        private bool _isAddNew = true;
        public bool IsAddNew
        {
            get { return _isAddNew; }
            set
            {
                _isAddNew = value;
                NotifyOfPropertyChange(() => IsAddNew);
            }
        }

        private object _leftContent;
        public object leftContent
        {
            get
            {
                return _leftContent;
            }
            set
            {
                if (_leftContent == value)
                    return;
                _leftContent = value;
                NotifyOfPropertyChange(() => leftContent);
            }
        }



        public void btSave()
        {
            if (CheckValidPCLExamType(ObjRefDepartmentReqCashAdv_Current) == false)
                return;
            if (ObjRefDepartmentReqCashAdv_Current == null)
            {
                MessageBox.Show(eHCMSResources.T0074_G1_I);
                return;
            }
            if (ObjRefDepartmentReqCashAdv_Current.DeptID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0095_G1_Msg_InfoChuaChonKhoa);
                return;
            }
            if (ObjRefDepartmentReqCashAdv_Current.CashAdvAmtReq <= 0)
            {
                MessageBox.Show(eHCMSResources.A0985_G1_Msg_InfoSoTienDNTUKhHopLe);
                return;
            }
            RefDepartmentReqCashAdv_Save();
        }

        public bool CheckValidPCLExamType(object temp)
        {
            DataEntities.RefDepartmentReqCashAdv p = temp as DataEntities.RefDepartmentReqCashAdv;
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

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void RefDepartmentReqCashAdv_Save()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Ghi..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new PCLsClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefDepartmentReqCashAdv_Save(ObjRefDepartmentReqCashAdv_Current,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long ID = 0;
                                var bOK = contract.EndRefDepartmentReqCashAdv_Save(out ID, asyncResult);
                                if (bOK)
                                {
                                    Globals.EventAggregator.Publish(new RefDepartmentReqCashAdvEvent_AddEditSave() { Result_AddEditSave = true });
                                    MessageBox.Show(eHCMSResources.Z0655_G1_DaGhi, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    TryClose();
                                    //phat su kien de load lai du lieu

                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0549_G1_Msg_InfoGhiDataFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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


        public void Handle(RefDepartments_BystrV_DeptTypeEvent.TreeSelectionChanged_Event message)
        {
            if (message != null)
            {
                DataEntities.RefDepartmentsTree NodeTree = message.ObjRefDepartments_Current as DataEntities.RefDepartmentsTree;
                if (NodeTree.ParentID != null)
                {
                    ObjRefDepartmentReqCashAdv_Current.DeptID = NodeTree.NodeID;

                    ObjRefDepartmentReqCashAdv_Current.DeptName = NodeTree.NodeText;
                }
            }
        }


    }
}
