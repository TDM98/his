using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Infrastructure;
using eHCMSLanguage;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ViewContracts;
using aEMR.Common.PagedCollectionView;
using aEMR.Controls;

namespace aEMR.Configuration.PCLExamTypeExamTestPrint.ViewModels
{
    [Export(typeof(IPCLExamTypeExamTestPrintIndex)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTypeExamTestPrintIndexViewModel : Conductor<object>, IPCLExamTypeExamTestPrintIndex
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
        public PCLExamTypeExamTestPrintIndexViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            SearchCriteria =new PCLExamTypeExamTestPrintSearchCriteria();
            ObjPCLExamTypeExamTestPrint_GetList_Paging=new ObservableCollection<DataEntities.PCLExamTypeExamTestPrint>();
            PCLExamTypeExamTestPrint_GetList_Paging();
        }

        private PCLExamTypeExamTestPrintSearchCriteria _SearchCriteria;
        public PCLExamTypeExamTestPrintSearchCriteria SearchCriteria
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

        private PagedCollectionView _ObjPCLExamTypeExamTestPrintlst;
        public PagedCollectionView ObjPCLExamTypeExamTestPrintlst
        {
            get
            {
                return _ObjPCLExamTypeExamTestPrintlst;
            }
            set
            {
                _ObjPCLExamTypeExamTestPrintlst = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypeExamTestPrintlst);
            }
        }

        private ObservableCollection<DataEntities.PCLExamTypeExamTestPrint> _ObjPCLExamTypeExamTestPrint_GetList_Paging;
        public ObservableCollection<DataEntities.PCLExamTypeExamTestPrint> ObjPCLExamTypeExamTestPrint_GetList_Paging
        {
            get
            {
                return _ObjPCLExamTypeExamTestPrint_GetList_Paging;
            }
            set
            {
                _ObjPCLExamTypeExamTestPrint_GetList_Paging = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypeExamTestPrint_GetList_Paging);
            }
        }

        private void LoadDataGrid()
        {
            ObjPCLExamTypeExamTestPrintlst = null;
            ObjPCLExamTypeExamTestPrintlst = new PagedCollectionView(ObjPCLExamTypeExamTestPrint_GetList_Paging);
            btnFilter();
        }
        public void btnFilter()
        {
            ObjPCLExamTypeExamTestPrintlst.Filter = null;
            ObjPCLExamTypeExamTestPrintlst.Filter = new Predicate<object>(DoFilter);
        }

        private string _SearchCode;
        public string SearchCode
        {
            get { return _SearchCode; }
            set
            {
                _SearchCode = value;
                NotifyOfPropertyChange(() => SearchCode);
            }
        }

        private string _SearchName;
        public string SearchName
        {
            get { return _SearchName; }
            set
            {
                _SearchName = value;
                NotifyOfPropertyChange(() => SearchName);
            }
        }

        //Callback method
        private bool DoFilter(object o)
        {
            //it is not a case sensitive search
            DataEntities.PCLExamTypeExamTestPrint emp = o as DataEntities.PCLExamTypeExamTestPrint;
            if (emp != null)
            {
                if (string.IsNullOrEmpty(SearchCode))
                {
                    SearchCode = "";
                }
                if (string.IsNullOrEmpty(SearchName))
                {
                    SearchName = "";
                }
                if (emp.Code.ToLower().IndexOf(SearchCode.Trim().ToLower()) >= 0 && emp.Name.ToLower().IndexOf(SearchName.Trim().ToLower()) >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public void PCLExamTypeExamTestPrint_GetList_Paging()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách ExamType,ExamTest..." });
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypeExamTestPrintIndex_GetAll(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {;
                            var items = contract.EndPCLExamTypeExamTestPrintIndex_GetAll(asyncResult);
                            ObjPCLExamTypeExamTestPrint_GetList_Paging = items.ToObservableCollection();
                            LoadDataGrid();
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


        public void btFind()
        {
            PCLExamTypeExamTestPrint_GetList_Paging();
        }

        public void btSave()
        {
            if(CheckValid())
            {
                PCLExamTypeExamTestPrint_Save();
            }
        }

        private bool CheckValid()
        {
            bool Result = true;

            if (ObjPCLExamTypeExamTestPrint_GetList_Paging.Count == 0)
                return false;

            StringBuilder sb=new StringBuilder();

            foreach (var pclExamTypeExamTestPrint in ObjPCLExamTypeExamTestPrint_GetList_Paging)
            {
                if(string.IsNullOrEmpty(pclExamTypeExamTestPrint.Code))
                {
                    Result = false;
                    sb.AppendLine("- ID: " + pclExamTypeExamTestPrint.ID + ", Phải Nhập Code!");
                }
            }
            if(!string.IsNullOrEmpty(sb.ToString()))
            {
                MessageBox.Show(sb.ToString(), eHCMSResources.G0442_G1_TBao,MessageBoxButton.OK);
            }
            return Result;
        }

        private void PCLExamTypeExamTestPrint_Save()
        {
            string Result = "";

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypeExamTestPrintIndex_Save(ObjPCLExamTypeExamTestPrint_GetList_Paging, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            
                            contract.EndPCLExamTypeExamTestPrintIndex_Save(out Result,asyncResult);
                            switch (Result)
                            {
                                case  "1":
                                    {
                                        MessageBox.Show(eHCMSResources.Z0655_G1_DaGhi, eHCMSResources.A0464_G1_Msg_Ghi, MessageBoxButton.OK);
                                        break;
                                    }
                                case "0":
                                    {
                                        MessageBox.Show(eHCMSResources.Z0654_G1_GhiKgThCong, eHCMSResources.A0464_G1_Msg_Ghi, MessageBoxButton.OK);
                                        break;
                                    }
                                default:
                                    {
                                        MessageBox.Show(eHCMSResources.A0543_G1_Msg_InfoLoi, eHCMSResources.A0464_G1_Msg_Ghi, MessageBoxButton.OK);
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

       public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
       {
           DataEntities.PCLExamTypeExamTestPrint objRows = e.Row.DataContext as DataEntities.PCLExamTypeExamTestPrint;
           if (objRows != null)
           {
               if(objRows.IsPCLExamType)
               {
                   e.Row.Background = new SolidColorBrush(Color.FromArgb(205, 180, 200, 120));
               }
               else
               {
                   e.Row.Background = new SolidColorBrush(Color.FromArgb(255, 248, 248, 248));
               }
           }
       }

       public void SearchCode_KeyUp(object sender, KeyEventArgs e)
       {
           if (e.Key == Key.Enter)
           {
               SearchCode = (sender as TextBox).Text;
               btnFilter();
           }
       }

       public void SearchName_KeyUp(object sender, KeyEventArgs e)
       {
           if (e.Key == Key.Enter)
           {
               SearchName = (sender as TextBox).Text;
               btnFilter();
           }
       }
       private Visibility _VisibilityPaging = Visibility.Collapsed;
       public Visibility VisibilityPaging
       {
           get
           {
               return _VisibilityPaging;
           }
           set
           {
               if (_VisibilityPaging != value)
               {
                   _VisibilityPaging = value;
                   NotifyOfPropertyChange(() => VisibilityPaging);
               }
           }
       }

       private void UnCheckPaging()
       {
           if (PagingChecked != null && ObjPCLExamTypeExamTestPrintlst != null)
           {
               PagingChecked.IsChecked = false;
               VisibilityPaging = Visibility.Collapsed;
           }
       }

       CheckBox PagingChecked;
       public void Paging_Checked(object sender, RoutedEventArgs e)
       {
           //activate datapager
           PagingChecked = sender as CheckBox;
           //pagerSellingList.Source = ObjPCLExamTypeExamTestPrintlst;
           VisibilityPaging = Visibility.Visible;
       }

       public void Paging_Unchecked(object sender, RoutedEventArgs e)
       {
           //deactivate datapager
           pagerSellingList.Source = null;
           VisibilityPaging = Visibility.Collapsed;

           LoadDataGrid();
       }

       DataPager pagerSellingList = null;
       public void pagerSellingList_Loaded(object sender, RoutedEventArgs e)
       {
           pagerSellingList = sender as DataPager;
       }

       private int _PCVPageSize = 30;
       public int PCVPageSize
       {
           get
           {
               return _PCVPageSize;
           }
           set
           {
               if (_PCVPageSize != value)
               {
                   _PCVPageSize = value;
                   NotifyOfPropertyChange(() => PCVPageSize);
               }
           }
       }
       public void cbxPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
       {
           if ((sender as ComboBox) != null && (sender as ComboBox).SelectedItem != null)
           {
               PCVPageSize = Convert.ToInt32(((sender as ComboBox).SelectedItem as ComboBoxItem).Content);
           }
       }
    }
}
