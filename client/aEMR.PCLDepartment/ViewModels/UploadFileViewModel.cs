//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel.Composition;
//using System.IO;
//using System.Threading;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Media.Imaging;
//using aEMR.Infrastructure;
//using aEMR.Infrastructure.Events;
//using aEMR.ServiceClient;
//using aEMR.ViewContracts;
//using aEMR.Infrastructure.CachingUtils;
//using Castle.Windsor;
//using Castle.Core.Logging;
//using Caliburn.Micro;
//using DataEntities;
//using PCLsService;
//using eHCMSLanguage;
//using Microsoft.Win32;

//namespace aEMR.PCLDepartment.ViewModels
//{
//    [Export(typeof(IUploadFile)), PartCreationPolicy(CreationPolicy.NonShared)]
//    public class UploadFileViewModel : Conductor<object>, IUploadFile
//    {
//        private readonly INavigationService _navigationService;
//        private readonly ISalePosCaching _salePosCaching;
//        private readonly ILogger _logger;

//        [ImportingConstructor]
//        public UploadFileViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
//        {
//            _navigationService = navigationService;
//            _logger = container.Resolve<ILogger>();
//            _salePosCaching = salePosCaching;
//        }

//        private Visibility _ControlImgVisibility;
//        private Visibility _ControlVideoVisibility;
//        private FolderTree _NodeSelected;
//        private BitmapImage _ObjBitmapImage;
//        private ObservableCollection<FolderTree> _ObjFillDirectoryAll;
//        private Stream _ObjGetVideoAndImage;
//        private PCLResultFileStorageDetail _ObjPCLResultFileStorageDetailSelected;
//        private bool _btChooseFileResultForPCLIsEnabled;
//        private IFileExplorerView _currentView;
//        private string _dir;

//        private int _itype;

//        public PCLResultFileStorageDetail ObjPCLResultFileStorageDetailSelected
//        {
//            get { return _ObjPCLResultFileStorageDetailSelected; }
//            set
//            {
//                if (_ObjPCLResultFileStorageDetailSelected != value)
//                {
//                    _ObjPCLResultFileStorageDetailSelected = value;
//                    NotifyOfPropertyChange(() => ObjPCLResultFileStorageDetailSelected);
//                }
//            }
//        }


//        public FolderTree NodeSelected
//        {
//            get { return _NodeSelected; }
//            set
//            {
//                if (_NodeSelected != value)
//                {
//                    _NodeSelected = value;
//                    NotifyOfPropertyChange(() => NodeSelected);
//                }
//            }
//        }


//        public ObservableCollection<FolderTree> ObjFillDirectoryAll
//        {
//            get { return _ObjFillDirectoryAll; }
//            set
//            {
//                if (_ObjFillDirectoryAll != value)
//                {
//                    _ObjFillDirectoryAll = value;
//                    NotifyOfPropertyChange(() => ObjFillDirectoryAll);
//                }
//            }
//        }

//        public Stream ObjGetVideoAndImage
//        {
//            get { return _ObjGetVideoAndImage; }
//            set
//            {
//                if (_ObjGetVideoAndImage != value)
//                {
//                    _ObjGetVideoAndImage = value;
//                    NotifyOfPropertyChange(() => ObjGetVideoAndImage);
//                }
//            }
//        }

//        public BitmapImage ObjBitmapImage
//        {
//            get { return _ObjBitmapImage; }
//            set
//            {
//                if (_ObjBitmapImage != value)
//                {
//                    _ObjBitmapImage = value;
//                    NotifyOfPropertyChange(() => ObjBitmapImage);
//                }
//            }
//        }

//        public Visibility ControlVideoVisibility
//        {
//            get { return _ControlVideoVisibility; }
//            set
//            {
//                if (_ControlVideoVisibility != value)
//                {
//                    _ControlVideoVisibility = value;
//                    NotifyOfPropertyChange(() => ControlVideoVisibility);
//                }
//            }
//        }

//        public Visibility ControlImgVisibility
//        {
//            get { return _ControlImgVisibility; }
//            set
//            {
//                if (_ControlImgVisibility != value)
//                {
//                    _ControlImgVisibility = value;
//                    NotifyOfPropertyChange(() => ControlImgVisibility);
//                }
//            }
//        }

//        public bool btChooseFileResultForPCLIsEnabled
//        {
//            get { return _btChooseFileResultForPCLIsEnabled; }
//            set
//            {
//                if (_btChooseFileResultForPCLIsEnabled != value)
//                {
//                    _btChooseFileResultForPCLIsEnabled = value;
//                    NotifyOfPropertyChange(() => btChooseFileResultForPCLIsEnabled);
//                }
//            }
//        }

//        #region IFileExplorer Members

//        public string dir
//        {
//            get { return _dir; }
//            set
//            {
//                if (_dir != value)
//                {
//                    _dir = value;
//                    NotifyOfPropertyChange(() => dir);
//                }
//            }
//        }

//        public int itype
//        {
//            get { return _itype; }
//            set
//            {
//                if (_itype != value)
//                {
//                    _itype = value;
//                    NotifyOfPropertyChange(() => itype);
//                }
//            }
//        }

//        public void InitPCLResultFileStorageDetail(string pPCLResultLocation, long pV_ResultType)
//        {
//            ObjPCLResultFileStorageDetailSelected = new PCLResultFileStorageDetail();
//            ObjPCLResultFileStorageDetailSelected.PCLResultLocation = pPCLResultLocation;
//            ObjPCLResultFileStorageDetailSelected.V_ResultType = pV_ResultType;
//        }

//        #endregion

//        protected override void OnViewLoaded(object view)
//        {
//            base.OnViewLoaded(view);
//            _currentView = view as IFileExplorerView;
//        }

//        protected override void OnActivate()
//        {
//            base.OnActivate();

//            btChooseFileResultForPCLIsEnabled = false;

//            ObjFillDirectoryAll = new ObservableCollection<FolderTree>();
//            FillDirectoryAll(dir, itype);

//            NodeSelected = new FolderTree();
//        }

//        private void FillDirectoryAll(string pdir, int ptype)
//        {
//            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "FolderList..." });

//            var t = new Thread(() =>
//                                   {
//                                       using (var serviceFactory = new PCLsImportClient())
//                                       {
//                                           IPCLsImport contract = serviceFactory.ServiceInstance;

//                                           contract.BeginFillDirectoryAll(pdir, ptype,
//                                                                          Globals.DispatchCallback((asyncResult) =>
//                                                                                                       {
//                                                                                                           try
//                                                                                                           {
//                                                                                                               IList
//                                                                                                                   <
//                                                                                                                       FolderTree
//                                                                                                                       >
//                                                                                                                   items
//                                                                                                                       =
//                                                                                                                       contract
//                                                                                                                           .
//                                                                                                                           EndFillDirectoryAll
//                                                                                                                           (asyncResult);
//                                                                                                               if (
//                                                                                                                   items !=
//                                                                                                                   null)
//                                                                                                               {
//                                                                                                                   ObjFillDirectoryAll
//                                                                                                                       =
//                                                                                                                       new ObservableCollection
//                                                                                                                           <
//                                                                                                                               FolderTree
//                                                                                                                               >
//                                                                                                                           (items);
//                                                                                                               }
//                                                                                                               else
//                                                                                                               {
//                                                                                                                   ObjFillDirectoryAll
//                                                                                                                       =
//                                                                                                                       null;
//                                                                                                               }
//                                                                                                           }
//                                                                                                           catch (
//                                                                                                               Exception
//                                                                                                                   ex)
//                                                                                                           {
//                                                                                                               Globals.
//                                                                                                                   ShowMessage
//                                                                                                                   (ex.
//                                                                                                                        Message,
//                                                                                                                    eHCMSResources.T0432_G1_Error);
//                                                                                                           }
//                                                                                                           finally
//                                                                                                           {
//                                                                                                               Globals.
//                                                                                                                   IsBusy
//                                                                                                                   =
//                                                                                                                   false;
//                                                                                                           }
//                                                                                                       }), null);
//                                       }
//                                   });
//            t.Start();
//        }

//        public void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
//        {
//            var treeView1 = (sender as TreeView);

//            string txt_path = GetPathFolder(treeView1.SelectedItem);
//            int index = 0;
//            if (txt_path.LastIndexOf(".") > 0)
//            {
//                index = txt_path.LastIndexOf(".");
//            }

//            NodeSelected = (treeView1.SelectedItem) as FolderTree;
//            string txt_filename = Globals.PatientAllDetails.PatientInfo.PatientID.ToString() + "-" + Guid.NewGuid().ToString() +
//                                  txt_path.Substring(index, txt_path.Length - index);

//            SetValuePCLResultFileStorageSelected(NodeSelected, txt_filename);

//            if (!string.IsNullOrEmpty(txt_path))
//            {
//                GetVideoAndImage(txt_path);
//            }
//        }

//        public string GetPathFolder(object Item)
//        {
//            string value = "";
//            if (Item != null)
//            {
//                var p = Item as FolderTree;
//                if (p.ParentFolderName != null)
//                {
//                    value = p.ParentFolderName;
//                }
//            }
//            return value;
//        }

//        private void GetVideoAndImage(string path)
//        {
//            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Tải File..." });

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new CommonService_V2Client())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    contract.BeginGetVideoAndImage(path, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            byte[] items = contract.EndGetVideoAndImage(asyncResult);
//                            if (items != null)
//                            {
//                                ObjGetVideoAndImage = new MemoryStream(items);
//                                CheckKetQuaReturn(ObjGetVideoAndImage);
//                            }
//                            else
//                            {
//                                ObjGetVideoAndImage = null;
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {
//                            Globals.IsBusy = false;
//                        }
//                    }), null);
//                }
//            });
//            t.Start();
//        }

//        private void CheckKetQuaReturn(Stream pStream)
//        {
//            if (pStream != null)
//            {
//                if (itype == 1) /*Images*/
//                {
//                    var imgsource = new BitmapImage();
//                    //imgsource.SetSource(pStream);
//                    ObjBitmapImage = imgsource;
//                    SetVisibleForVideoControls(Visibility.Collapsed);
//                    SetVisibleForImgControls(Visibility.Visible);
//                }
//                else if (itype == 2) /*Video*/
//                {
//                    SetVisibleForVideoControls(Visibility.Visible);
//                    SetVisibleForImgControls(Visibility.Collapsed);

//                    if (_currentView != null)
//                    {
//                        _currentView.SetObjectSource(pStream);
//                    }
//                }
//                else /*Document*/
//                {
//                    SetVisibleForVideoControls(Visibility.Collapsed);
//                    SetVisibleForImgControls(Visibility.Collapsed);
//                }

//                btChooseFileResultForPCLIsEnabled = true;
//            }
//            else
//            {
//                SetVisibleForVideoControls(Visibility.Collapsed);
//                SetVisibleForImgControls(Visibility.Collapsed);

//                btChooseFileResultForPCLIsEnabled = false;
//            }
//        }

//        private void SetVisibleForVideoControls(Visibility pV)
//        {
//            ControlVideoVisibility = pV;
//        }

//        private void SetVisibleForImgControls(Visibility pV)
//        {
//            ControlImgVisibility = pV;
//        }

//        private void SetValuePCLResultFileStorageSelected(FolderTree Node, string FileNameSave)
//        {
//            ObjPCLResultFileStorageDetailSelected.PCLResultFileName = FileNameSave;
//            ObjPCLResultFileStorageDetailSelected.FullPath = Node.ParentFolderName;
//        }


//        public void btChooseFileResultForPCL()
//        {
//            //Phát sự kiện chọn file, đóng lại
//            if (ObjPCLResultFileStorageDetailSelected != null && ObjPCLResultFileStorageDetailSelected.FullPath != null)
//            {
//                Globals.EventAggregator.Publish(new btChooseFileResultForPCL_Click<PCLResultFileStorageDetail, int, Stream>
//                        {
//                            File = ObjPCLResultFileStorageDetailSelected,
//                            TypeFile = itype,
//                            StreamFile = ObjGetVideoAndImage
//                        });
//                TryClose();
//            }
//            else
//            {
//                MessageBox.Show(eHCMSResources.A0312_G1_Msg_InfoChonFile, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//            }
//        }

//        public void btCancel()
//        {
//            TryClose();
//        }


//        public void hplDiagnoticsImg()
//        {
//            if (ObjBitmapImage != null)
//            {
//                //var typeInfo = Globals.GetViewModel<IImageDisgnosticExplorer>();
//                //typeInfo.ObjBitmapImage = ObjBitmapImage;
//                //var instance = typeInfo as Conductor<object>;

//                //Globals.ShowDialog(instance, (o) =>
//                //                                 {
//                //                                     //lam gi do
//                //                                 });

//                Action<IImageDisgnosticExplorer> onInitDlg = (typeInfo) =>
//                {
//                    typeInfo.ObjBitmapImage = ObjBitmapImage;
//                };
//                GlobalsNAV.ShowDialog<IImageDisgnosticExplorer>(onInitDlg);
//            }
//        }

//        public void hplDiagnoticsVideo()
//        {
//            if (ObjGetVideoAndImage != null)
//            {
//                //var typeInfo = Globals.GetViewModel<IVideoDiagnosticExplorer>();
//                //typeInfo.VideoStream = ObjGetVideoAndImage;
//                //var instance = typeInfo as Conductor<object>;

//                //Globals.ShowDialog(instance, (o) =>
//                //                                 {
//                //                                     //lam gi do
//                //                                 });

//                Action<IVideoDiagnosticExplorer> onInitDlg = (typeInfo) =>
//                {
//                    typeInfo.VideoStream = ObjGetVideoAndImage;
//                };
//                GlobalsNAV.ShowDialog<IVideoDiagnosticExplorer>(onInitDlg);
//            }
//        }

//        #region Control media

//        public void btPlay()
//        {
//            if (_currentView != null)
//            {
//                _currentView.btPlay();
//            }
//        }

//        public void btPause()
//        {
//            if (_currentView != null)
//            {
//                _currentView.btPause();
//            }
//        }

//        public void btStop()
//        {
//            if (_currentView != null)
//            {
//                _currentView.btStop();
//            }
//        }

//        public void btMute(object sender, RoutedEventArgs e)
//        {
//            if (_currentView != null)
//            {
//                _currentView.btMute();
//            }
//        }

//        public void btVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
//        {
//            if (_currentView != null)
//            {
//                _currentView.btVolume_ValueChanged(sender, e);
//            }
//        }

//        #endregion

//        public void btUploadFile()
//        {
//            OpenFileDialog dlg = new OpenFileDialog();
//            dlg.Multiselect = false;
//            //dlg.Filter = "All files (*.*)|*.*|PNG Images (*.png)|*.png";
//            dlg.Filter = "Images (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp";

//            bool? retval = dlg.ShowDialog();

//            //if (retval != null && retval == true)
//            //{
//            //    Stream stream = (Stream)dlg.File.OpenRead();
//            //    byte[] bytes = new byte[stream.Length];
//            //    stream.Read(bytes, 0, (int)stream.Length);
//            //    SendFile(dlg.File.Name,bytes,false,dir);
//            //}
//        }

//        void SendFile(string name,byte[] contents, bool append,string dir)
//        {
//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new CommonService_V2Client())
//                {
//                    var contract = serviceFactory.ServiceInstance;

//                    contract.BeginDoUpload(name, contents, append,dir, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            var Result = contract.EndDoUpload(asyncResult);
//                            MessageBox.Show(eHCMSResources.K0258_G1_Uploaded);
//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {
//                            // Globals.IsBusy = false;
//                        }
//                    }), null);
//                }


//            });
//            t.Start();
//        }

//    }
//}