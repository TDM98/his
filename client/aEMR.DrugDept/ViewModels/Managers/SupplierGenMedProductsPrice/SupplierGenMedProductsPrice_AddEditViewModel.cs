using eHCMSLanguage;
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
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using Castle.Core.Logging;
using DataEntities;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(ISupplierGenMedProductsPrice_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SupplierGenMedProductsPrice_AddEditViewModel : Conductor<object>, ISupplierGenMedProductsPrice_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SupplierGenMedProductsPrice_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            
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

        private Supplier _ObjSupplierCurrent;
        public Supplier ObjSupplierCurrent
        {
            get { return _ObjSupplierCurrent; }
            set
            {
                _ObjSupplierCurrent = value;
                NotifyOfPropertyChange(() => ObjSupplierCurrent);
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

        private SupplierGenMedProductsPrice _ObjDrugCurrent;
        public SupplierGenMedProductsPrice ObjDrugCurrent
        {
            get { return _ObjDrugCurrent; }
            set
            {
                _ObjDrugCurrent = value;
                NotifyOfPropertyChange(() => ObjDrugCurrent);
            }
        }


        private SupplierGenMedProductsPrice _ObjSupplierGenMedProductsPrice_Current;
        public SupplierGenMedProductsPrice ObjSupplierGenMedProductsPrice_Current
        {
            get { return _ObjSupplierGenMedProductsPrice_Current; }
            set
            {   
                _ObjSupplierGenMedProductsPrice_Current = value;
                NotifyOfPropertyChange(()=>ObjSupplierGenMedProductsPrice_Current);
            }
        }


        public void InitializeNewItem()
        {
            ObjSupplierGenMedProductsPrice_Current = new SupplierGenMedProductsPrice();


            ObjSupplierGenMedProductsPrice_Current.StaffID = Globals.LoggedUserAccount.Staff != null
                                                                           ? Globals.LoggedUserAccount.Staff.
                                                                                 StaffID
                                                                           : (Globals.LoggedUserAccount.StaffID.
                                                                                  HasValue
                                                                                  ? Globals.LoggedUserAccount.
                                                                                        StaffID.Value
                                                                                  : -1);
            ObjSupplierGenMedProductsPrice_Current.ApprovedStaffID = ObjSupplierGenMedProductsPrice_Current.StaffID;
            
            ObjSupplierGenMedProductsPrice_Current.UnitPrice = 0;
            ObjSupplierGenMedProductsPrice_Current.PackagePrice = 0;
            ObjSupplierGenMedProductsPrice_Current.VAT = 0;
            ObjSupplierGenMedProductsPrice_Current.EffectiveDate = DateTime.Now;

            ObjSupplierGenMedProductsPrice_Current.SupplierID = ObjSupplierCurrent.SupplierID;
            ObjSupplierGenMedProductsPrice_Current.GenMedProductID = ObjDrugCurrent.GenMedProductID;

        }


        public void btSave()
        {
            if (CheckValid(ObjSupplierGenMedProductsPrice_Current))
            {
                SupplierGenMedProductsPrice_Save(ObjSupplierGenMedProductsPrice_Current);
            }
        }

        public bool CheckValid(object temp)
        {
            SupplierGenMedProductsPrice u = temp as SupplierGenMedProductsPrice;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }



        public void SupplierGenMedProductsPrice_Save(SupplierGenMedProductsPrice Obj)
        {
            string Result = "";

            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0645_G1_DangGhi) });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSupplierGenMedProductsPrice_Save(Obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndSupplierGenMedProductsPrice_Save(out Result, asyncResult);
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
                                        Globals.EventAggregator.Publish(new SaveEvent<bool>() { Result = true });

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
                                                SupplierGenMedProductsPrice_ByPKID(IDInsert);
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

        private void SupplierGenMedProductsPrice_ByPKID(Int64 PKID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0530_G1_DangRefresh) });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSupplierGenMedProductsPrice_ByPKID(PKID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            ObjSupplierGenMedProductsPrice_Current = contract.EndSupplierGenMedProductsPrice_ByPKID(asyncResult);
                            if (ObjSupplierGenMedProductsPrice_Current != null && ObjSupplierGenMedProductsPrice_Current.PKID > 0)
                            {
                                Globals.EventAggregator.Publish(new SaveEvent<bool>() { Result = true });

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
        public void LostFocus_UnitPrice(object UnitPrice)
        {
            decimal V = 0;

            if (IsNumeric(UnitPrice) == false)
            {
                decimal.TryParse(UnitPrice.ToString(), out V);
            }
            else
            {
                V = ObjSupplierGenMedProductsPrice_Current.UnitPrice;
            }
            
            ObjSupplierGenMedProductsPrice_Current.UnitPrice = V;
        }

        public void LostFocus_PackagePrice(object PackagePrice)
        {
            decimal V = 0;

            if (IsNumeric(PackagePrice) == false)
            {
                decimal.TryParse(PackagePrice.ToString(), out V);
            }
            else
            {
                V = ObjSupplierGenMedProductsPrice_Current.PackagePrice;
            }
            
            ObjSupplierGenMedProductsPrice_Current.PackagePrice = V;
        }

        public void LostFocus_VATRate(object VATRate)
        {
            //if (VATRate != null)
            //{
            //    double V = 0;

            //    if (IsNumeric(VATRate) == false)
            //    {
            //        double.TryParse(VATRate.ToString(), out V);
            //    }
            //    else
            //    {
            //        V = ObjSupplierGenMedProductsPrice_Current.VAT.Value;
            //    }
                
            //    ObjSupplierGenMedProductsPrice_Current.VAT= V;
            //}
            //else
            //{
            //    ObjSupplierGenMedProductsPrice_Current.VAT = 0;
            //}
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
                ObjSupplierGenMedProductsPrice_Current.EffectiveDate = DateTime.Now;
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
