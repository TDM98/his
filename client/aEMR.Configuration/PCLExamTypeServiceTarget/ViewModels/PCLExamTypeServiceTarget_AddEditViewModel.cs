using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;
using aEMR.ViewContracts;

namespace aEMR.Configuration.PCLExamTypeServiceTarget.ViewModels
{
    [Export(typeof(IPCLExamTypeServiceTarget_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTypeServiceTarget_AddEditViewModel : Conductor<object>, IPCLExamTypeServiceTarget_AddEdit
        , IHandle<DbClickSelectedObjectEvent<PCLExamType>>
    {
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

        private DataEntities.PCLExamTypeServiceTarget _ObjPCLExamTypeServiceTarget_Current;
        public DataEntities.PCLExamTypeServiceTarget ObjPCLExamTypeServiceTarget_Current
        {
            get { return _ObjPCLExamTypeServiceTarget_Current; }
            set
            {
                _ObjPCLExamTypeServiceTarget_Current = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypeServiceTarget_Current);
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
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PCLExamTypeServiceTarget_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
        }



        public void btSave()
        {
            if (CheckValidPCLExamType(ObjPCLExamTypeServiceTarget_Current) == false)
                return;
            PCLExamTypeServiceTarget_Save();
        }

        public bool CheckValidPCLExamType(object temp)
        {
            DataEntities.PCLExamTypeServiceTarget p = temp as DataEntities.PCLExamTypeServiceTarget;
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

        private void PCLExamTypeServiceTarget_Save()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Ghi..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new PCLsClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypeServiceTarget_Save(ObjPCLExamTypeServiceTarget_Current,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long ID = 0;
                                var bOK = contract.EndPCLExamTypeServiceTarget_Save(out ID, asyncResult);
                                if (bOK)
                                {
                                    MessageBox.Show(eHCMSResources.Z0655_G1_DaGhi, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    Globals.EventAggregator.Publish(new PCLExamTypeServiceTargetEvent_AddEditSave() { Result_AddEditSave = true });
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

        public void ChooseExamTypeCmd()
        {
            //var UCPCLExamTypes = Globals.GetViewModel<IPCLExamTypes_List_Paging>();
            //UCPCLExamTypes.IsChildWindow = true;
            //UCPCLExamTypes.SearchCriteria = new PCLExamTypeSearchCriteria();
            //UCPCLExamTypes.SearchCriteria.IsNotInPCLExamTypeLocations = true;
            //UCPCLExamTypes.SearchCriteria.IsNotInPCLItems = true;
            //UCPCLExamTypes.IsNotInPCLItemsVisibility = Visibility.Collapsed;
            //UCPCLExamTypes.FormLoad();

            //var instance = UCPCLExamTypes as Conductor<object>;
            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});

            Action<IPCLExamTypes_List_Paging> onInitDlg = (UCPCLExamTypes) =>
            {
                UCPCLExamTypes.IsChildWindow = true;
                UCPCLExamTypes.SearchCriteria = new PCLExamTypeSearchCriteria();
                UCPCLExamTypes.SearchCriteria.IsNotInPCLExamTypeLocations = true;
                UCPCLExamTypes.SearchCriteria.IsNotInPCLItems = true;
                UCPCLExamTypes.IsNotInPCLItemsVisibility = Visibility.Collapsed;
                UCPCLExamTypes.FormLoad();
            };
            GlobalsNAV.ShowDialog<IPCLExamTypes_List_Paging>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        public void Handle(DbClickSelectedObjectEvent<PCLExamType> message)
        {
            if (this.IsActive && message != null)
            {
                ObjPCLExamTypeServiceTarget_Current = new DataEntities.PCLExamTypeServiceTarget();
                ObjPCLExamTypeServiceTarget_Current.PCLExamTypeID = message.Result.PCLExamTypeID;
                ObjPCLExamTypeServiceTarget_Current.PCLExamTypeName = message.Result.PCLExamTypeName;
                ObjPCLExamTypeServiceTarget_Current.PCLExamTypeCode = message.Result.PCLExamTypeCode;
            }
        }
    }
}
