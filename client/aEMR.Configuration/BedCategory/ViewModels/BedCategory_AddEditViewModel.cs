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
using System.Linq;
using aEMR.Common.BaseModel;

namespace aEMR.Configuration.BedCategory.ViewModels
{
    [Export(typeof(IBedCategory_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BedCategory_AddEditViewModel : ViewModelBase, IBedCategory_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public BedCategory_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            GetAllBedType();
        }

        private ObservableCollection<Lookup> _BedTypeList;

        public ObservableCollection<Lookup> BedTypeList
        {
            get { return _BedTypeList; }
            set
            {
                _BedTypeList = value;
                NotifyOfPropertyChange(() => BedTypeList);
            }
        }

        private DataEntities.BedCategory _ObjBedCategory_Current;
        public DataEntities.BedCategory ObjBedCategory_Current
        {
            get { return _ObjBedCategory_Current; }
            set
            {
                _ObjBedCategory_Current = value;
                NotifyOfPropertyChange(() => ObjBedCategory_Current);
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

        public void GetAllBedType()
        {
            BedTypeList = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.V_BedType))
                {
                    BedTypeList.Add(tmpLookup);
                }
            }
        }

        public void InitializeNewItem()
        {
            ObjBedCategory_Current = new DataEntities.BedCategory();
            ObjBedCategory_Current.BedType = BedTypeList.FirstOrDefault();
            ObjBedCategory_Current.V_BedType = BedTypeList.FirstOrDefault().LookupID;
            ObjBedCategory_Current.IsActive = true;
        }

        public void btSave()
        {
            if (ObjBedCategory_Current.HosBedName != "")
            {
                if (CheckValid(ObjBedCategory_Current))
                {
                    BedCategory_InsertUpdate(ObjBedCategory_Current, true);
                }
            }
            else
            {
                MessageBox.Show("Nhập đầy đủ thông tin mã BedCategory và chẩn đoán!", eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
            }
        }

        public bool CheckValid(object temp)
        {
            DataEntities.BedCategory p = temp as DataEntities.BedCategory;
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

        private void BedCategory_InsertUpdate(DataEntities.BedCategory Obj, bool SaveToDB)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginBedCategory_Save(Obj, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string Result = "";
                                contract.EndBedCategory_Save(out Result, asyncResult);
                                switch (Result)
                                {
                                    case "Duplex-Name":
                                        {
                                            MessageBox.Show(string.Format("{0} {1}!", "Tên giường đã tồn tại!", eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Update-0":
                                        {
                                            GlobalsNAV.ShowMessagePopup(eHCMSResources.A0608_G1_Msg_InfoHChinhFail);
                                            break;
                                        }
                                    case "Update-1":
                                        {
                                            Globals.EventAggregator.Publish(new BedCategory_Event_Save() { Result = true });
                                            TitleForm = string.Format("{0} ({1})", eHCMSResources.T1484_G1_HChinh, ObjBedCategory_Current.HosBedName.Trim());
                                            GlobalsNAV.ShowMessagePopup(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                            TryClose();
                                            break;
                                        }
                                    case "Insert-0":
                                        {
                                            GlobalsNAV.ShowMessagePopup(eHCMSResources.A1026_G1_Msg_InfoThemFail);
                                            break;
                                        }
                                    case "Insert-1":
                                        {
                                            Globals.EventAggregator.Publish(new BedCategory_Event_Save() { Result = true });
                                            GlobalsNAV.ShowMessagePopup(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
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
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }
    }
}
