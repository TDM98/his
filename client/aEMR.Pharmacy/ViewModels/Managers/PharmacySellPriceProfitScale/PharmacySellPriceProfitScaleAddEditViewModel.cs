using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;
using aEMR.Common.BaseModel;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPharmacySellPriceProfitScaleAddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacySellPriceProfitScaleAddEditViewModel : ViewModelBase, IPharmacySellPriceProfitScaleAddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PharmacySellPriceProfitScaleAddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

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

        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private PharmacySellPriceProfitScale _ObjPharmacySellPriceProfitScale_Current;
        public PharmacySellPriceProfitScale ObjPharmacySellPriceProfitScale_Current
        {
            get { return _ObjPharmacySellPriceProfitScale_Current; }
            set
            {
                _ObjPharmacySellPriceProfitScale_Current = value;
                NotifyOfPropertyChange(() => ObjPharmacySellPriceProfitScale_Current);
            }
        }

        public void FormLoad()
        {
         
        }

        public void InitializeNewItem()
        {
            ObjPharmacySellPriceProfitScale_Current = new PharmacySellPriceProfitScale();
            ObjPharmacySellPriceProfitScale_Current.IsActive = true;
        }

        public void btSave()
        {
            if (ObjPharmacySellPriceProfitScale_Current.BuyingCostTo<=0)
            {
                MessageBox.Show(eHCMSResources.A0560_G1_Msg_InfoGVonDenLonHon0);
                return;
            }


            if(ObjPharmacySellPriceProfitScale_Current.BuyingCostFrom>ObjPharmacySellPriceProfitScale_Current.BuyingCostTo)
            {
                MessageBox.Show(eHCMSResources.A0561_G1_Msg_InfoGiaTuNhoHonGiaDen);
                return; 
            }

            PharmacySellPriceProfitScale_AddEdit();
        }

        //public bool CheckValid(object temp)
        //{
        //    DataEntities.PCLForm p = temp as DataEntities.PCLForm;
        //    if (p == null)
        //    {
        //        return false;
        //    }
        //    return p.Validate();
        //}

        public void btClose()
        {
            TryClose();
        }

        private void PharmacySellPriceProfitScale_AddEdit()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0343_G1_DangLuu) });
            this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0343_G1_DangLuu));
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginPharmacySellPriceProfitScale_AddEdit(ObjPharmacySellPriceProfitScale_Current, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string Result = "";
                                contract.EndPharmacySellPriceProfitScale_AddEdit(out Result, asyncResult);
                                switch (Result)
                                {
                                    case "0":
                                        {
                                            MessageBox.Show(eHCMSResources.A0701_G1_Msg_InfoLuuFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "1":
                                        {
                                            Globals.EventAggregator.Publish(new SaveEvent<bool>() { Result = true });
                                            MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                            break;
                                        }
                                    //default:
                                    //    {
                                    //        string[] arrRes = new string[2];
                                    //        arrRes = Result.Split('@');

                                    //        string msg1 = "";
                                    //        string msg2 = "";

                                    //        if (arrRes[0] != "")
                                    //        {
                                    //            msg1 = "Tên PCLForm Này Đã Được Sử Dụng!";
                                    //        }

                                    //        if (arrRes[1] != "")
                                    //        {
                                    //            msg2 = "Ngày Hết Hạn Phải > Ngày Tạo PCLForm!";
                                    //        }

                                    //        string msg = (msg1 == "" ? "" : "- " + msg1) + (msg2 != "" ? Environment.NewLine + "- " + msg2 : "");

                                    //        MessageBox.Show(msg + Environment.NewLine + "Vui Lòng Nhập Khác!", "Lưu", MessageBoxButton.OK);
                                    //        break;
                                    //    }
                            }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                _logger.Info(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                                IsLoading = false;
                                //Globals.IsBusy = false;
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.DlgHideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    _logger.Info(ex.Message);
                }


            });
            t.Start();
        }
    }
}

