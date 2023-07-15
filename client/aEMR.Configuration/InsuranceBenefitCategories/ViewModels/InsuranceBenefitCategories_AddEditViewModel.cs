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
using aEMR.Common.BaseModel;

namespace aEMR.Configuration.InsuranceBenefitCategories.ViewModels
{
    [Export(typeof(IInsuranceBenefitCategories_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InsuranceBenefitCategories_AddEditViewModel : ViewModelBase, IInsuranceBenefitCategories_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InsuranceBenefitCategories_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }

        private InsuranceBenefitCategories_Data _ObjInsuranceBenefitCategories_Current;
        public InsuranceBenefitCategories_Data ObjInsuranceBenefitCategories_Current
        {
            get { return _ObjInsuranceBenefitCategories_Current; }
            set
            {
                _ObjInsuranceBenefitCategories_Current = value;
                NotifyOfPropertyChange(() => ObjInsuranceBenefitCategories_Current);
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

        public void InitializeNewItem()
        {
            ObjInsuranceBenefitCategories_Current = new InsuranceBenefitCategories_Data();
        }

        public void btSave()
        {
            if (ObjInsuranceBenefitCategories_Current.HIPCode != "")
            {
                if (CheckValid(ObjInsuranceBenefitCategories_Current))
                {
                    InsuranceBenefitCategories_InsertUpdate(ObjInsuranceBenefitCategories_Current, true);
                }
            }
            else
            {
                MessageBox.Show("Nhập đầy đủ thông tin mã đối tượng và mã quyền lợi!", eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
            }
        }

        public bool CheckValid(object temp)
        {
            InsuranceBenefitCategories_Data p = temp as InsuranceBenefitCategories_Data;
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

        private void InsuranceBenefitCategories_InsertUpdate(InsuranceBenefitCategories_Data Obj, bool SaveToDB)
        {
            this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        Obj.DateModified = Convert.ToDateTime(Globals.ServerDate);
                        Obj.ModifiedLog = Globals.LoggedUserAccount.StaffID.ToString();
                        contract.BeginInsuranceBenefitCategories_Save(Obj, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string Result = "";
                                contract.EndInsuranceBenefitCategories_Save(out Result, asyncResult);
                                switch (Result)
                                {
                                    case "Duplex-Name":
                                        {
                                            MessageBox.Show("Mã quyền lợi bảo hiểm đã tồn tại", eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Update-0":
                                        {
                                            MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Update-1":
                                        {
                                            Globals.EventAggregator.Publish(new InsuranceBenefitCategories_Event_Save() { Result = true });
                                            TitleForm = string.Format("{0} ({1})", eHCMSResources.T1484_G1_HChinh, ObjInsuranceBenefitCategories_Current.HIPCode.Trim());
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
                                            Globals.EventAggregator.Publish(new InsuranceBenefitCategories_Event_Save() { Result = true });
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
