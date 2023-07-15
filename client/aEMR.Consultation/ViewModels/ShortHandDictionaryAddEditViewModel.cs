using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.ViewContracts;
using eHCMS.Services.Core;
using System.Collections.Generic;
using aEMR.Common;
using System.Linq;
using aEMR.Controls;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IShortHandDictionaryAddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ShortHandDictionaryAddEditViewModel : Conductor<object>, IShortHandDictionaryAddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ShortHandDictionaryAddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
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

  
       
        private ShortHandDictionary _ObjShortHandDictionary_Current;
        public ShortHandDictionary ObjShortHandDictionary_Current
        {
            get { return _ObjShortHandDictionary_Current; }
            set
            {
                _ObjShortHandDictionary_Current = value;
                NotifyOfPropertyChange(() => ObjShortHandDictionary_Current);
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
        public AxTextBoxFilter.TextBoxFilterType IntNumberFilter
        {
            get
            {
                return AxTextBoxFilter.TextBoxFilterType.Integer;
            }
        }
        public void InitializeNewItem()
        {
            ObjShortHandDictionary_Current = new ShortHandDictionary();
        }

        public void btSave()
        {
            if (!string.IsNullOrEmpty(ObjShortHandDictionary_Current.ShortHandDictionaryKey) && !string.IsNullOrEmpty(ObjShortHandDictionary_Current.ShortHandDictionaryValue))
            {
                if (CheckValid(ObjShortHandDictionary_Current))
                {
                    ShortHandDictionary_InsertUpdate(ObjShortHandDictionary_Current, true);
                }
            }
            else
            {
                MessageBox.Show("Nhập đầy đủ thông tin từ viết tắt và giá trị!", eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
            }

        }
      
        public bool CheckValid(object temp)
        {
            ShortHandDictionary p = temp as ShortHandDictionary;
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
     
        private void ShortHandDictionary_InsertUpdate(ShortHandDictionary Obj, bool SaveToDB)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    //Obj.DateModified = Convert.ToDateTime(Globals.ServerDate);
                    Obj.StaffID = (long)Globals.LoggedUserAccount.StaffID;

                    contract.BeginShortHandDictionary_Save(Obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndShortHandDictionary_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Duplex-Name":
                                    {
                                        MessageBox.Show(string.Format("{0} {1}!", eHCMSResources.A1009_G1_Msg_InfoTenPgDaTonTai, eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new ShortHandDictionary_Event_Save() { Result = true });
                                        //TitleForm = string.Format("{0} ({1})", eHCMSResources.T1484_G1_HChinh, ObjICD_Current.DiseaseNameVN.Trim());
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        TryClose();
                                        break;
                                    }
                                case "Insert-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-1":
                                    {
                                        Globals.EventAggregator.Publish(new ShortHandDictionary_Event_Save() { Result = true });
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        TryClose();
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
