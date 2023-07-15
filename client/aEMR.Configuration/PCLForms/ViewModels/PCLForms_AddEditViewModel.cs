using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using DataEntities;
using aEMR.Common;
using eHCMSLanguage;
using aEMR.ViewContracts;

namespace aEMR.Configuration.PCLForms.ViewModels
{
    [Export(typeof(IPCLForms_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLForms_AddEditViewModel : Conductor<object>, IPCLForms_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PCLForms_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
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

        private DataEntities.PCLForm _ObjPCLForms_Current;
        public DataEntities.PCLForm ObjPCLForms_Current
        {
            get { return _ObjPCLForms_Current; }
            set
            {
                _ObjPCLForms_Current = value;
                NotifyOfPropertyChange(() => ObjPCLForms_Current);
            }
        }

        public void FormLoad()
        {
            LoadV_PCLMainCategory();
        }

        public void InitializeNewItem()
        {
            ObjPCLForms_Current = new DataEntities.PCLForm();
            ObjPCLForms_Current.V_PCLMainCategory = -1;
            ObjPCLForms_Current.ApplicatorDay = DateTime.Now;
        }

        //Main
        private Lookup _ObjV_PCLMainCategory_Selected;
        public Lookup ObjV_PCLMainCategory_Selected
        {
            get { return _ObjV_PCLMainCategory_Selected; }
            set
            {
                _ObjV_PCLMainCategory_Selected = value;
                NotifyOfPropertyChange(() => ObjV_PCLMainCategory_Selected);
            }
        }

        private ObservableCollection<Lookup> _ObjV_PCLMainCategory;
        public ObservableCollection<Lookup> ObjV_PCLMainCategory
        {
            get { return _ObjV_PCLMainCategory; }
            set
            {
                _ObjV_PCLMainCategory = value;
                NotifyOfPropertyChange(() => ObjV_PCLMainCategory);
            }
        }

        public void LoadV_PCLMainCategory()
        {
            var t = new Thread(() =>
            {
                //Globals.EventAggregator.Publish(new BusyEvent{IsBusy = true,Message = "Danh Sách Loại..."});

                IsLoading = true;

                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_PCLMainCategory,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_PCLMainCategory = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup();
                                    firstItem.LookupID = -1;
                                    firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2034_G1_ChonLoai2);
                                    ObjV_PCLMainCategory.Insert(0, firstItem);

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
        //Main

        public void btSave()
        {
            if (ObjPCLForms_Current.V_PCLMainCategory > 0)
            {
                if (CheckValid(ObjPCLForms_Current))
                {
                    PCLForm_Save();
                }
            }
            else
            {
                MessageBox.Show("Chọn Loại PCLForm!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        public bool CheckValid(object temp)
        {
            DataEntities.PCLForm p = temp as DataEntities.PCLForm;
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

        private void PCLForm_Save()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLForms_Save(ObjPCLForms_Current, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndPCLForms_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Duplex-Name":
                                    {
                                        MessageBox.Show("Tên PCLForm Này Đã Được Sử Dụng! Vui Lòng Dùng Tên Khác!", eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Err_ExpiredDay":
                                    {
                                        MessageBox.Show(eHCMSResources.A0842_G1_Msg_InfoNgHetHanKhHopLe, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }

                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new SaveEvent<bool>() { Result = true });

                                        TitleForm = string.Format("{0} ({1})", eHCMSResources.T1484_G1_HChinh, ObjPCLForms_Current.PCLFormName.Trim());
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
                                            msg1 = "Tên PCLForm Này Đã Được Sử Dụng!";
                                        }

                                        if (arrRes[1] != "")
                                        {
                                            msg2 = eHCMSResources.A0842_G1_Msg_InfoNgHetHanKhHopLe;
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


        #region "Lost Focus"
        public void ExpiredDay_LostFocus(object ExpiredDay)
        {
            if (ExpiredDay != null)
            {
                DateTime V = DateTime.Now;
                DateTime.TryParse(ExpiredDay.ToString(), out V);
            }
            else
            {
                ObjPCLForms_Current.ExpiredDay = null;
            }
        }
        #endregion
    }
}

