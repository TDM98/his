using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ViewContracts;

namespace aEMR.Configuration.PCLExamTypeExamTestPrint.ViewModels
{
    [Export(typeof(IPCLExamTypeExamTestPrint)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTypeExamTestPrintViewModel : Conductor<object>, IPCLExamTypeExamTestPrint
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
        public PCLExamTypeExamTestPrintViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
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

        public void PCLExamTypeExamTestPrint_GetList_Paging()
        {
            ObjPCLExamTypeExamTestPrint_GetList_Paging.Clear();

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách ExamType,ExamTest..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypeExamTestPrint_GetList_Paging(SearchCriteria,0,9999,"",true,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int Total=0;
                            var items = contract.EndPCLExamTypeExamTestPrint_GetList_Paging(out Total,asyncResult);

                            if (items != null)
                            {
                                ObjPCLExamTypeExamTestPrint_GetList_Paging = new ObservableCollection<DataEntities.PCLExamTypeExamTestPrint>(items);
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

                    contract.BeginPCLExamTypeExamTestPrint_Save(ObjPCLExamTypeExamTestPrint_GetList_Paging, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            
                            contract.EndPCLExamTypeExamTestPrint_Save(out Result,asyncResult);
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
    }
}
