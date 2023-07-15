using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ViewContracts;
using System.Windows.Controls;
using aEMR.Common.BaseModel;
using System.Linq;
using aEMR.Common.Utilities;
using ObjectCopier = aEMR.Common.ObjectCopier;
using aEMR.CommonTasks;
using Microsoft.Win32;
using System.IO;
using System.Net;
/*
* #001 20180921 TNHX: Apply BusyIndicator, refactor code
*/
namespace aEMR.CommonViews.ViewModels
{
    [Export(typeof(IICD_ListFindForConsultation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ICD_ListFindForConsultationViewModel : ViewModelBase, IICD_ListFindForConsultation
        , IHandle<ICD_Event_Save>
    {
        public event Action<string> PopupRequest;

        protected override void OnActivate()
        {
            authorization();
            base.OnActivate();
          
        }
        protected override void OnDeactivate(bool close)
        {
            Debug.WriteLine("OnDeActivate");
            base.OnDeactivate(close);
        }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
        }
        //public CefSharp.Wpf.ChromiumWebBrowser Browser { get; set; }
    
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ICD_ListFindForConsultationViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();
            Globals.EventAggregator.Subscribe(this);

            SearchCriteria = new ICDSearchCriteria();
            SearchCriteria.ICD10Code = "";
            SearchCriteria.DiseaseNameVN = "";
            SearchCriteria.OrderBy = "";
            SearchCriteria.IsActive = true;

            ObjICD_ByIDCode_Paging = new PagedSortableCollectionView<ICD>();
            ObjICD_ByIDCode_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjICD_ByIDCode_Paging_OnRefresh);

            ICD_ByIDCode_Paging(0, ObjICD_ByIDCode_Paging.PageSize, true);

            //Browser = new CefSharp.Wpf.ChromiumWebBrowser();
            //Browser.Address = Globals.ServerConfigSection.CommonItems.ICDCategorySearchUrl;
            CheckExistRunTime();
        }
        
        void ObjICD_ByIDCode_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            ICD_ByIDCode_Paging(ObjICD_ByIDCode_Paging.PageIndex,
                            ObjICD_ByIDCode_Paging.PageSize, false);
        }

        private Uri _SourceLink = new Uri(Globals.ServerConfigSection.CommonItems.ICDCategorySearchUrl); //Globals.ServerConfigSection.CommonItems.ICDCategorySearchUrl;
        public Uri SourceLink
        {
            get
            {
                return _SourceLink;
            }
            set
            {
                _SourceLink = value;
                NotifyOfPropertyChange(() => SourceLink);
            }
        }

        private ICDSearchCriteria _SearchCriteria;
        public ICDSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private PagedSortableCollectionView<ICD> _ObjICD_ByIDCode_Paging;
        public PagedSortableCollectionView<ICD> ObjICD_ByIDCode_Paging
        {
            get { return _ObjICD_ByIDCode_Paging; }
            set
            {
                _ObjICD_ByIDCode_Paging = value;
                NotifyOfPropertyChange(() => ObjICD_ByIDCode_Paging);
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            //bhplEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                                   , (int)eConfiguration_Management.mDanhMucPhong,
            //                                   (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mEdit);
            //bhplDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                                   , (int)eConfiguration_Management.mDanhMucPhong,
            //                                   (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mDelete);
            //bbtSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                            , (int)eConfiguration_Management.mDanhMucPhong,
            //                            (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mView);
            //bhplAddNew = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                                    , (int)eConfiguration_Management.mDanhMucPhong,
            //                                    (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mAdd);
        }

        #region checking account
        private bool _bhplEdit = true;
        private bool _bhplDelete = true;
        private bool _bbtSearch = true;
        private bool _bhplAddNew = true;
        public bool bhplEdit
        {
            get
            {
                return _bhplEdit;
            }
            set
            {
                if (_bhplEdit == value)
                    return;
                _bhplEdit = value;
            }
        }

        public bool bhplDelete
        {
            get
            {
                return _bhplDelete;
            }
            set
            {
                if (_bhplDelete == value)
                    return;
                _bhplDelete = value;
            }
        }

        public bool bbtSearch
        {
            get
            {
                return _bbtSearch;
            }
            set
            {
                if (_bbtSearch == value)
                    return;
                _bbtSearch = value;
            }
        }

        public bool bhplAddNew
        {
            get
            {
                return _bhplAddNew;
            }
            set
            {
                if (_bhplAddNew == value)
                    return;
                _bhplAddNew = value;
            }
        }
        #endregion

        #region binding visibilty
        public Button hplEdit { get; set; }
        public Button hplDelete { get; set; }

        public void hplEdit_Loaded(object sender)
        {
            hplEdit = sender as Button;
            hplEdit.Visibility = Globals.convertVisibility(bhplEdit);
        }
        public void hplDelete_Loaded(object sender)
        {
            hplDelete = sender as Button;
            hplDelete.Visibility = Globals.convertVisibility(bhplDelete);
        }
        #endregion
        #region Button
        public void btSearchICD()
        {
            ObjICD_ByIDCode_Paging.PageIndex = 0;
            ICD_ByIDCode_Paging(0, ObjICD_ByIDCode_Paging.PageSize, true);
        }
        #endregion

        private object _ICD_Current;
        public object ICD_Current
        {
            get { return _ICD_Current; }
            set
            {
                _ICD_Current = value;
                NotifyOfPropertyChange(() => ICD_Current);
            }
        }
        //cal:Message.Attach="[Event DblClick]=[Action DoubleClick($eventArgs)]; [Event SelectionChanged]=[Action dtgListSelectionChanged($eventArgs)]"
        //public void DoubleClick(object args)
        //{
        //    EventArgs<object> eventArgs = args as EventArgs<object>;
        //    ICD_Current = eventArgs.Value as ICD;
        //    Globals.EventAggregator.Publish(new dgICDListClickSelectionChanged_Event() { ICD_Current = eventArgs.Value });
        //}

        //public void dtgListSelectionChanged(object args)
        //{
        //    if (((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems)).Length > 0)
        //    {
        //        if (((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems))[0] != null)
        //        {
        //            ICD_Current =
        //                ((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems))[0];
        //            var typeInfo = Globals.GetViewModel<IICD_ListFind>();
        //            typeInfo.ICD_Current = (ICD)ICD_Current;

        //            Globals.EventAggregator.Publish(new dgICDListClickSelectionChanged_Event()
        //            {
        //                ICD_Current = ((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems))[0]
        //            });
        //        }
        //    }
        //}

        private void ICD_ByIDCode_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Phòng..." });
            this.DlgShowBusyIndicator("Danh sách ICD");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginICD_ByIDCode_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.ICD> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndICD_ByIDCode_Paging(out Total, asyncResult);
                                bOK = true;
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

                            ObjICD_ByIDCode_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjICD_ByIDCode_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjICD_ByIDCode_Paging.Add(item);
                                    }
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }

        #region Handle
        public void Handle(ICD_Event_Save message)
        {
            ObjICD_ByIDCode_Paging.PageIndex = 0;
            ICD_ByIDCode_Paging(0, ObjICD_ByIDCode_Paging.PageSize, true);
        }


        #endregion
        private void CheckExistRunTime()
        {
            var key64 = Registry.LocalMachine.OpenSubKey(Globals.ServerConfigSection.CommonItems.RuntimeReg64);
            var key32 = Registry.LocalMachine.OpenSubKey(Globals.ServerConfigSection.CommonItems.RuntimeReg32);
            // if you want to check under HKLM
            //var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\GT37\0010");
            if (key64 == null && key32 == null)
            {
                // Key does not exist
                MessageBox.Show("Máy tính chưa hỗ trợ xem danh mục ICD của bộ y tế."+Environment.NewLine+"Vui lòng liên hệ IT để được hỗ trợ", "Thông báo");

                

                //string downloadPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads\";
                //try
                //{
                //    using (var client = new WebClient())
                //    {
                //        client.DownloadFile(Globals.ServerConfigSection.CommonItems.RuntimeUrl + "MicrosoftEdgeWebview2Setup.exe", downloadPath + "MicrosoftEdgeWebview2Setup.exe");
                //    }
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show("Cài đặt không thành công vui lòng liên hệ IT", "Lỗi");
                //}
                //Process myProcess = new Process();
                //myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //myProcess.StartInfo.CreateNoWindow = true;
                //myProcess.StartInfo.UseShellExecute = false;
                //myProcess.StartInfo.FileName = "cmd.exe";

                //myProcess.StartInfo.Arguments = "/c " + downloadPath + "MicrosoftEdgeWebview2Setup.exe"; /*   / silent   */
                //myProcess.EnableRaisingEvents = true;
                //myProcess.Start();
                //myProcess.WaitForExit();

            }
        }
        private void DownloadRuntime()
        {
            //var drives = DriveInfo.GetDrives().Where(d => d.DriveType != DriveType.CDRom && d.Name!="P:\\").ToArray();
            //int lastPath = drives.Length - 1;
            //string localPath = drives[lastPath].RootDirectory + @"\Runtime\";
           
            //if (!Directory.Exists(localPath))
            //{
            //    Directory.CreateDirectory(localPath);
            //}
           
        }
    }
}
