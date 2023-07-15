using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPharmacySellingItemPrices_Item)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacySellingItemPrices_ItemViewModel : Conductor<object>, IPharmacySellingItemPrices_Item
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PharmacySellingItemPrices_ItemViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
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

        private DataEntities.RefGenericDrugDetail _ObjDrug_Current;
        public DataEntities.RefGenericDrugDetail ObjDrug_Current
        {
            get { return _ObjDrug_Current; }
            set
            {
                _ObjDrug_Current = value;
                NotifyOfPropertyChange(() => ObjDrug_Current);
            }
        }

        private DataEntities.PharmacySellingItemPrices _ObjPharmacySellingItemPrices_Current;
        public DataEntities.PharmacySellingItemPrices ObjPharmacySellingItemPrices_Current
        {
            get { return _ObjPharmacySellingItemPrices_Current; }
            set
            {
                _ObjPharmacySellingItemPrices_Current = value;
                NotifyOfPropertyChange(() => ObjPharmacySellingItemPrices_Current);
            }
        }

        public void InitializeNewItem()
        {
            //hien khong xai ham nay nua
            ObjPharmacySellingItemPrices_Current = new PharmacySellingItemPrices();
            ObjPharmacySellingItemPrices_Current.DrugID = ObjDrug_Current.DrugID;
            ObjPharmacySellingItemPrices_Current.NormalPrice = 0;
            ObjPharmacySellingItemPrices_Current.PriceForHIPatient = 0;
            ObjPharmacySellingItemPrices_Current.HIAllowedPrice = 0;
            ObjPharmacySellingItemPrices_Current.EffectiveDate = Globals.ServerDate;

            ObjPharmacySellingItemPrices_Current.StaffID = (long)Globals.LoggedUserAccount.Staff.StaffID;

            ObjPharmacySellingItemPrices_Current.ApprovedStaffID =(long) ObjPharmacySellingItemPrices_Current.StaffID;

        }

        public void btSave()
        {
            if (CheckValid(ObjPharmacySellingItemPrices_Current))
            {
                if (CheckGiaChenhLech())
                {
                    PharmacySellingItemPrices_Save();
                }
            }
        }

        private bool CheckGiaChenhLech()
        {
            if (ObjPharmacySellingItemPrices_Current.NormalPrice >= 1)
            {
                if (ObjPharmacySellingItemPrices_Current.NormalPrice < ObjPharmacySellingItemPrices_Current.PriceForHIPatient)
                {
                    MessageBox.Show(eHCMSResources.Z0573_G1_GiaBNBHNhoHonDGia);
                    return false;
                }
                else
                {
                    if (ObjPharmacySellingItemPrices_Current.HIAllowedPrice > 0)
                    {
                        if (ObjPharmacySellingItemPrices_Current.HIAllowedPrice > ObjPharmacySellingItemPrices_Current.PriceForHIPatient)
                        {
                            MessageBox.Show(eHCMSResources.T1982_G1_GiaBHChoPhep2);
                            return false;
                        }
                    }
                }

            }
            else
            {
                MessageBox.Show(eHCMSResources.Z0644_G1_DGiaPhaiLonHonBang1);
                return false;
            }
            return true;
        }



        public void PharmacySellingItemPrices_Save()
        {
            string Result = "";

            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0645_G1_DangGhi) });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPharmacySellingItemPrices_Item_Save(ObjPharmacySellingItemPrices_Current, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndPharmacySellingItemPrices_Item_Save(out Result, asyncResult);
                            if (string.IsNullOrEmpty(Result))
                            {
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                TryClose();
                            }
                            else
                            {
                                MessageBox.Show(Result);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

       
        public bool CheckValid(object temp)
        {
            DataEntities.PharmacySellingItemPrices p = temp as DataEntities.PharmacySellingItemPrices;
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

        #region "LostFocus"
        public void LostFocus_NormalPrice(object NormalPrice)
        {
            if (ObjPharmacySellingItemPrices_Current.PharmacySellingItemPriceID <= 0)
            {
                decimal V = 0;

                if (IsNumeric(NormalPrice) == false)
                {
                    decimal.TryParse(NormalPrice.ToString(), out V);
                }
                else
                {
                    V = ObjPharmacySellingItemPrices_Current.NormalPrice;
                }

                ObjPharmacySellingItemPrices_Current.NormalPrice = V;
                ObjPharmacySellingItemPrices_Current.PriceForHIPatient = V;
            }
        }

        public void LostFocus_PriceForHIPatient(object PriceForHIPatient)
        {
            if (PriceForHIPatient != null)
            {
                decimal V = 0;

                if (IsNumeric(PriceForHIPatient) == false)
                {
                    decimal.TryParse(PriceForHIPatient.ToString(), out V);
                }
                else
                {
                    V = ObjPharmacySellingItemPrices_Current.PriceForHIPatient;
                }

                ObjPharmacySellingItemPrices_Current.PriceForHIPatient = V;

            }
            else
            {
                ObjPharmacySellingItemPrices_Current.PriceForHIPatient = 0;
            }
        }


        public void LostFocus_HIAllowedPrice(object HIAllowedPrice)
        {
            if (HIAllowedPrice != null)
            {
                decimal V = 0;

                if (IsNumeric(HIAllowedPrice) == false)
                {
                    decimal.TryParse(HIAllowedPrice.ToString(), out V);
                }
                else
                {
                    V = ObjPharmacySellingItemPrices_Current.HIAllowedPrice;
                }

                ObjPharmacySellingItemPrices_Current.HIAllowedPrice = V;
            }
            else
            {
                ObjPharmacySellingItemPrices_Current.HIAllowedPrice = 0;
            }
        }

        public void LostFocus_EffectiveDate(object EffectiveDate)
        {
            if (EffectiveDate != null)
            {

                DateTime V = Globals.ServerDate.Value;
                DateTime.TryParse(EffectiveDate.ToString(), out V);
            }
            else
            {
                ObjPharmacySellingItemPrices_Current.EffectiveDate = Globals.ServerDate.Value;
            }
        }
        public void EffectiveDate_Loaded(object sender)
        {
            
        }

        private static bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;
            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }


        #endregion
    }
}

