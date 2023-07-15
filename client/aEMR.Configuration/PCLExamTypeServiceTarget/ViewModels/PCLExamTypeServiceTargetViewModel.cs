using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.Common.Collections;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ViewContracts;

namespace aEMR.Configuration.PCLExamTypeServiceTarget.ViewModels
{
    [Export(typeof(IPCLExamTypeServiceTarget)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTypeServiceTargetViewModel : Conductor<object>, IPCLExamTypeServiceTarget
    , IHandle<PCLExamTypeServiceTargetEvent_AddEditSave>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PCLExamTypeServiceTargetViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
            PCLExamTypeServiceTargetLst = new PagedSortableCollectionView<DataEntities.PCLExamTypeServiceTarget>();
            PCLExamTypeServiceTargetLst.PageSize = Globals.PageSize;
            PCLExamTypeServiceTargetLst.OnRefresh += new EventHandler<RefreshEventArgs>(PCLExamTypeServiceTargetLst_OnRefresh);
            btnSearch();
        }

        void PCLExamTypeServiceTargetLst_OnRefresh(object sender, RefreshEventArgs e)
        {
            PCLPCLExamTypeServiceTarget_Search(PCLExamTypeServiceTargetLst.PageIndex, PCLExamTypeServiceTargetLst.PageSize);
        }

        #region Properties member

        private string _SearchText;
        public string SearchText
        {
            get { return _SearchText; }
            set
            {
                _SearchText = value;
                NotifyOfPropertyChange(() => SearchText);
            }
        }

        private PagedSortableCollectionView<DataEntities.PCLExamTypeServiceTarget> _PCLExamTypeServiceTargetLst;
        public PagedSortableCollectionView<DataEntities.PCLExamTypeServiceTarget> PCLExamTypeServiceTargetLst
        {
            get { return _PCLExamTypeServiceTargetLst; }
            set
            {
                _PCLExamTypeServiceTargetLst = value;
                NotifyOfPropertyChange(() => PCLExamTypeServiceTargetLst);
            }
        }

        #endregion

        #region Function member

        public void hplDelete_Click(object datacontext)
        {
            DataEntities.PCLExamTypeServiceTarget p = (datacontext as DataEntities.PCLExamTypeServiceTarget);

            if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.PCLExamTypeName), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                PCLPCLExamTypeServiceTarget_Delete(p.PCLExamTypeServiceTargetID);
            }
        }

        public void hplAddNewExamTypeCombo_Click()
        {
            //var typeInfo = Globals.GetViewModel<IPCLExamTypeServiceTarget_AddEdit>();
            //typeInfo.TitleForm = eHCMSResources.G0276_G1_ThemMoi;
            //typeInfo.ObjPCLExamTypeServiceTarget_Current = new DataEntities.PCLExamTypeServiceTarget();

            //var instance = typeInfo as Conductor<object>;
            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});

            Action<IPCLExamTypeServiceTarget_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = eHCMSResources.G0276_G1_ThemMoi;
                typeInfo.ObjPCLExamTypeServiceTarget_Current = new DataEntities.PCLExamTypeServiceTarget();
            };
            GlobalsNAV.ShowDialog<IPCLExamTypeServiceTarget_AddEdit>(onInitDlg);
        }

        public void DoubleClick(Common.EventArgs<object> e)
        {
            DataEntities.PCLExamTypeServiceTarget item = e.Value as DataEntities.PCLExamTypeServiceTarget;
            //var typeInfo = Globals.GetViewModel<IPCLExamTypeServiceTarget_AddEdit>();
            //typeInfo.TitleForm = eHCMSResources.K1599_G1_CNhat;
            //typeInfo.IsAddNew = false;
            //typeInfo.ObjPCLExamTypeServiceTarget_Current = item;
            //var instance = typeInfo as Conductor<object>;
            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});

            Action<IPCLExamTypeServiceTarget_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = eHCMSResources.K1599_G1_CNhat;
                typeInfo.IsAddNew = false;
                //20191026 TBL: BM 0018503: Nhấn cập nhật chỉ tiêu chưa lưu nhưng trên màn hình vẫn hiển thị những thứ đã được cập nhật trước đó
                //typeInfo.ObjPCLExamTypeServiceTarget_Current = item;
                typeInfo.ObjPCLExamTypeServiceTarget_Current = ObjectCopier.DeepCopy(item);
            };
            GlobalsNAV.ShowDialog<IPCLExamTypeServiceTarget_AddEdit>(onInitDlg);
        }

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        public void btnSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchText = (sender as TextBox).Text;
                btnSearch();
            }
        }

        public void btnSearch()
        {
            PCLExamTypeServiceTargetLst.PageIndex = 0;
            PCLPCLExamTypeServiceTarget_Search(PCLExamTypeServiceTargetLst.PageIndex, PCLExamTypeServiceTargetLst.PageSize);
        }

        private void PCLPCLExamTypeServiceTarget_Search(int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                //IsLoading = true;

                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypeServiceTarget_GetAll(SearchText, "", PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Total = 0;
                                var allItems = client.EndPCLExamTypeServiceTarget_GetAll(out Total, asyncResult);
                                PCLExamTypeServiceTargetLst.Clear();
                                if (allItems != null && allItems.Count > 0)
                                {
                                    PCLExamTypeServiceTargetLst.TotalItemCount = Total;
                                    foreach (var item in allItems)
                                    {
                                        PCLExamTypeServiceTargetLst.Add(item);
                                    }
                                }
                            }
                            catch (Exception innerEx)
                            {
                                ClientLoggerHelper.LogInfo(innerEx.ToString());
                            }

                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });
            t.Start();
        }

        private void PCLPCLExamTypeServiceTarget_Delete(long ID)
        {
            var t = new Thread(() =>
            {
                //IsLoading = true;

                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypeServiceTarget_Delete(ID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool bOK = client.EndPCLExamTypeServiceTarget_Delete(asyncResult);
                                if (bOK)
                                {
                                    MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK);
                                    btnSearch();
                                }

                            }
                            catch (Exception innerEx)
                            {
                                ClientLoggerHelper.LogInfo(innerEx.ToString());
                            }

                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });
            t.Start();
        }
        #endregion

        public void Handle(PCLExamTypeServiceTargetEvent_AddEditSave message)
        {
            if (message != null)
            {
                btnSearch();
            }
        }
    }
}
