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

namespace aEMR.Configuration.OutPatientTreatmentType.ViewModels
{
    [Export(typeof(IOutPatientTreatmentTypeAddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OutPatientTreatmentTypeAddEditViewModel : Conductor<object>, IOutPatientTreatmentTypeAddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public OutPatientTreatmentTypeAddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
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
     

        private OutpatientTreatmentType _ObjOutPatientTreatmentType_Current;
        public OutpatientTreatmentType ObjOutPatientTreatmentType_Current
        {
            get { return _ObjOutPatientTreatmentType_Current; }
            set
            {
                _ObjOutPatientTreatmentType_Current = value;
                NotifyOfPropertyChange(() => ObjOutPatientTreatmentType_Current);
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
            ObjOutPatientTreatmentType_Current = new OutpatientTreatmentType();
        }

        public void btSave()
        {
            OutPatientTreatmentTypeSave();
        }
      
        private void OutPatientTreatmentTypeSave()
        {
            if (!ObjOutPatientTreatmentType_Current.CheckValidZeroValue())
            {
                MessageBox.Show("Cấu hình số phải lớn hơn 0");
                return;
            }
            if (!ObjOutPatientTreatmentType_Current.CheckValidMinMaxValue())
            {
                MessageBox.Show("Cấu hình số tối đa phải lớn hơn hoặc bằng số tối thiểu");
                return;
            }
            if (string.IsNullOrEmpty(ObjOutPatientTreatmentType_Current.OutpatientTreatmentName) || string.IsNullOrEmpty(ObjOutPatientTreatmentType_Current.OutpatientTreatmentCode))
            {
                MessageBox.Show("Nhập đầy đủ thông tin tên nhóm bệnh và mã nhóm bệnh!", eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
                return;
            }
            if (CheckValidOutPatientTreatmentType_Current(ObjOutPatientTreatmentType_Current))
            {
                OutPatientTreatmentType_InsertUpdate(ObjOutPatientTreatmentType_Current, true);
            }
        }
        
        public bool CheckValidOutPatientTreatmentType_Current(object temp)
        {
            OutpatientTreatmentType p = temp as OutpatientTreatmentType;
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
        
     
        private void OutPatientTreatmentType_InsertUpdate(OutpatientTreatmentType Obj, bool SaveToDB)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    Obj.Log = Globals.LoggedUserAccount.StaffID.ToString();

                    contract.BeginOutPatientTreatmentType_Save(Obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndOutPatientTreatmentType_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Duplex-Code":
                                    {
                                        MessageBox.Show(string.Format("{0} {1}!", "Mã nhóm bệnh đã tồn tại", eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Duplex-Name":
                                    {
                                        MessageBox.Show(string.Format("{0} {1}!", "Tên nhóm bệnh đã tồn tại", eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new SaveEvent<OutpatientTreatmentType> { Result = ObjOutPatientTreatmentType_Current });
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
                                        Globals.EventAggregator.Publish(new SaveEvent<OutpatientTreatmentType> { Result = ObjOutPatientTreatmentType_Current });
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
