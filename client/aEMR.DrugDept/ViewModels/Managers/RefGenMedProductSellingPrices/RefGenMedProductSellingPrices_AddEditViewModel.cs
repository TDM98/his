using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;

using eHCMSLanguage;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IRefGenMedProductSellingPrices_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefGenMedProductSellingPrices_AddEditViewModel : Conductor<object>, IRefGenMedProductSellingPrices_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RefGenMedProductSellingPrices_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
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

        private RefGenMedProductSellingPrices _ObjRefGenMedProductDetails_Info;
        public RefGenMedProductSellingPrices ObjRefGenMedProductDetails_Info
        {
            get { return _ObjRefGenMedProductDetails_Info; }
            set
            {
                _ObjRefGenMedProductDetails_Info = value;
                NotifyOfPropertyChange(() => ObjRefGenMedProductDetails_Info);
            }
        }

        private RefGenMedProductSellingPrices _ObjRefGenMedProductSellingPrices_Current;
        public RefGenMedProductSellingPrices ObjRefGenMedProductSellingPrices_Current
        {
            get { return _ObjRefGenMedProductSellingPrices_Current; }
            set
            {
                _ObjRefGenMedProductSellingPrices_Current = value;
                NotifyOfPropertyChange(() => ObjRefGenMedProductSellingPrices_Current);
            }
        }

        public void InitializeNewItem(Int64 GenMedProductID)
        {
            ObjRefGenMedProductSellingPrices_Current = new RefGenMedProductSellingPrices();
            ObjRefGenMedProductSellingPrices_Current.GenMedProductID = GenMedProductID;

            ObjRefGenMedProductSellingPrices_Current.StaffID = Globals.LoggedUserAccount.Staff != null
                                                                               ? Globals.LoggedUserAccount.Staff.
                                                                                     StaffID
                                                                               : (Globals.LoggedUserAccount.StaffID.
                                                                                      HasValue
                                                                                      ? Globals.LoggedUserAccount.
                                                                                            StaffID.Value
                                                                                      : -1);

            ObjRefGenMedProductSellingPrices_Current.ApprovedStaffID = ObjRefGenMedProductSellingPrices_Current.StaffID;
            
            ObjRefGenMedProductSellingPrices_Current.NormalPrice = 0;
            ObjRefGenMedProductSellingPrices_Current.EffectiveDate = DateTime.Now;

        }

        public void btSave()
        {
            if (CheckValid(ObjRefGenMedProductSellingPrices_Current))
            {
                if (CheckGiaChenhLech())
                {
                    RefGenMedProductSellingPrices_Save(ObjRefGenMedProductSellingPrices_Current);
                }
            }
        }

        private bool CheckGiaChenhLech()
        {
            if (ObjRefGenMedProductSellingPrices_Current.NormalPrice >= 1)
            {
                if (ObjRefGenMedProductSellingPrices_Current.NormalPrice < ObjRefGenMedProductSellingPrices_Current.PriceForHIPatient)
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0573_G1_GiaBNBHNhoHonDGia));
                    return false;
                }
                else
                {
                    if (ObjRefGenMedProductSellingPrices_Current.HIAllowedPrice != null && ObjRefGenMedProductSellingPrices_Current.HIAllowedPrice > 0)
                    {
                        if (ObjRefGenMedProductSellingPrices_Current.HIAllowedPrice > ObjRefGenMedProductSellingPrices_Current.PriceForHIPatient)
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


        public bool CheckValid(object temp)
        {
            RefGenMedProductSellingPrices u = temp as RefGenMedProductSellingPrices;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }


        public void RefGenMedProductSellingPrices_Save(RefGenMedProductSellingPrices Obj)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0645_G1_DangGhi) });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefGenMedProductSellingPrices_Save(Obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";

                            contract.EndRefGenMedProductSellingPrices_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Insert-0-PriceCurrent-Exists":
                                    {
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0646_G1_DaCoGiaApDung) + Environment.NewLine + string.Format("({0})", eHCMSResources.Z0647_G1_ThayDoiGiaApDung), eHCMSResources.Z0524_G1_ThemGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A1025_G1_Msg_InfoThemGiaFail, eHCMSResources.Z0524_G1_ThemGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-0-PriceCurrent-EffectiveDate":
                                    {
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0648_G1_NgGiaApDungNhoHonBangNgHHanh), eHCMSResources.Z0524_G1_ThemGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-0-PriceFuture-Exists":
                                    {
                                        MessageBox.Show(eHCMSResources.Z0650_G1_GiaMoiCoRoi, eHCMSResources.Z0524_G1_ThemGia, MessageBoxButton.OK);
                                        break;
                                    }

                                case "Update-0-PriceOld-NotAllowUpdate":
                                    {
                                        MessageBox.Show(eHCMSResources.A0551_G1_Msg_InfoKhDcCNhatGiaCu, eHCMSResources.K1652_G1_CNhatGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0272_G1_Msg_InfoCNhatFail), eHCMSResources.K1652_G1_CNhatGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new RefGenMedProductSellingPrices_AddEditViewModel_Save_Event() { Result = true });

                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.K2782_G1_DaCNhat), eHCMSResources.K1652_G1_CNhatGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0-PriceCurrent-EffectiveDate":
                                    {
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0652_G1_I), eHCMSResources.K1652_G1_CNhatGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0-PriceFuture-EffectiveDate":
                                    {
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0653_G1_DaCoGiaApDung), eHCMSResources.K1652_G1_CNhatGia, MessageBoxButton.OK);
                                        break;
                                    }
                                default:
                                    {
                                        if (Result.IndexOf("ID") != -1)
                                        {
                                            Int64 IDInsert = 0;
                                            Int64.TryParse(Result.Replace("ID", ""), out IDInsert);
                                            if (IDInsert > 0)
                                            {
                                                RefGenMedProductSellingPrices_ByGenMedSellPriceID(IDInsert);
                                            }
                                            else
                                            {
                                                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0654_G1_GhiKgThCong), eHCMSResources.A0464_G1_Msg_Ghi, MessageBoxButton.OK);
                                            }
                                        }
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
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        private void RefGenMedProductSellingPrices_ByGenMedSellPriceID(Int64 GenMedSellPriceID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0530_G1_DangRefresh) });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefGenMedProductSellingPrices_ByGenMedSellPriceID(GenMedSellPriceID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            ObjRefGenMedProductSellingPrices_Current = contract.EndRefGenMedProductSellingPrices_ByGenMedSellPriceID(asyncResult);
                            if (ObjRefGenMedProductSellingPrices_Current != null && ObjRefGenMedProductSellingPrices_Current.GenMedSellPriceID > 0)
                            {
                                Globals.EventAggregator.Publish(new RefGenMedProductSellingPrices_AddEditViewModel_Save_Event() { Result = true });

                                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0655_G1_DaGhi), eHCMSResources.A0464_G1_Msg_Ghi, MessageBoxButton.OK);
                            }
                            else
                            {
                                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0654_G1_GhiKgThCong), eHCMSResources.A0464_G1_Msg_Ghi, MessageBoxButton.OK);
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

        public void btClose()
        {
            TryClose();
        }

        #region "LostFocus"
        public void LostFocus_NormalPrice(object NormalPrice)
        {
            if (ObjRefGenMedProductSellingPrices_Current.GenMedSellPriceID <= 0)
            {
                decimal V = 0;

                if (IsNumeric(NormalPrice) == false)
                {
                    decimal.TryParse(NormalPrice.ToString(), out V);
                }
                else
                {
                    V = ObjRefGenMedProductSellingPrices_Current.NormalPrice;
                }
                
                ObjRefGenMedProductSellingPrices_Current.NormalPrice = V;
                ObjRefGenMedProductSellingPrices_Current.PriceForHIPatient = V;
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
                    V = ObjRefGenMedProductSellingPrices_Current.PriceForHIPatient.Value;   
                }
                
                ObjRefGenMedProductSellingPrices_Current.PriceForHIPatient = V;
            }
            else
            {
                ObjRefGenMedProductSellingPrices_Current.PriceForHIPatient = null;
            }
        }

        public void LostFocus_VATRate(object VATRate)
        {
            if (VATRate != null)
            {
                double V = 0;

                if (IsNumeric(VATRate) == false)
                {
                    double.TryParse(VATRate.ToString(), out V);
                }
                else
                {
                    V = ObjRefGenMedProductSellingPrices_Current.VATRate.Value;
                }
                
                ObjRefGenMedProductSellingPrices_Current.VATRate = V;
            }
            else
            {
                ObjRefGenMedProductSellingPrices_Current.VATRate = null;
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
                    V = ObjRefGenMedProductSellingPrices_Current.HIAllowedPrice.Value;
                }
                
                ObjRefGenMedProductSellingPrices_Current.HIAllowedPrice = V;
            }
            else
            {
                ObjRefGenMedProductSellingPrices_Current.HIAllowedPrice = null;
            }
        }

        public void LostFocus_EffectiveDate(object EffectiveDate)
        {
            if (EffectiveDate != null)
            {

                DateTime V = DateTime.Now;
                DateTime.TryParse(EffectiveDate.ToString(), out V);
            }
            else
            {
                ObjRefGenMedProductSellingPrices_Current.EffectiveDate = DateTime.Now;
            }
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
