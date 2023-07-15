using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using DataEntities;
using Castle.Windsor;
using aEMR.ViewContracts;

namespace aEMR.Configuration.PCLGroups.ViewModels
{
    [Export(typeof(IPCLGroups_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLGroups_AddEditViewModel : Conductor<object>, IPCLGroups_AddEdit
    {
        [ImportingConstructor]
        public PCLGroups_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg) { }
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

        private ObservableCollection<Lookup> _ObjV_PCLCategory;
        public ObservableCollection<Lookup> ObjV_PCLCategory
        {
            get { return _ObjV_PCLCategory; }
            set
            {
                _ObjV_PCLCategory = value;
                NotifyOfPropertyChange(() => ObjV_PCLCategory);
            }
        }

        private DataEntities.PCLGroup _ObjPCLGroups_Current;
        public DataEntities.PCLGroup ObjPCLGroups_Current
        {
            get { return _ObjPCLGroups_Current; }
            set
            {
                _ObjPCLGroups_Current = value;
                NotifyOfPropertyChange(() => ObjPCLGroups_Current);
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


        public void InitializeNewItem(Int64 V_PCLCategory_Selected)
        {
            ObjPCLGroups_Current = new DataEntities.PCLGroup();
            ObjPCLGroups_Current.V_PCLCategory = V_PCLCategory_Selected;
        }

        public void btSave()
        {
            if (ObjPCLGroups_Current.V_PCLCategory > 0)
            {
                if (CheckValid(ObjPCLGroups_Current))
                {
                    PCLGroups_Save();
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0330_G1_Msg_InfoChonLoaiPCL, eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
            }

        }

        public bool CheckValid(object temp)
        {
            DataEntities.PCLGroup p = temp as DataEntities.PCLGroup;
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

        private void PCLGroups_Save()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLGroups_Save(ObjPCLGroups_Current, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndPCLGroups_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new SaveEvent<bool>() { Result = true });

                                        TitleForm = string.Format("{0} ({1})", eHCMSResources.T1484_G1_HChinh, ObjPCLGroups_Current.PCLGroupName.Trim());
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-1":
                                    {
                                        Globals.EventAggregator.Publish(new SaveEvent<bool>() { Result = true });

                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
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
                                            msg1 = "Tên PCLSection Này Đã Được Sử Dụng!";
                                        }

                                        if (arrRes[1] != "")
                                        {
                                            msg2 = "Ngày Hết Hạn Phải > Ngày Tạo PCLSections!";
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
