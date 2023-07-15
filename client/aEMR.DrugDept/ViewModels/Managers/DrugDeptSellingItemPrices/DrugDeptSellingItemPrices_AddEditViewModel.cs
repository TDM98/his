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

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptSellingItemPrices_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDeptSellingItemPrices_AddEditViewModel : Conductor<object>, IDrugDeptSellingItemPrices_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public DrugDeptSellingItemPrices_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        private long _V_MedProductType;
        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                if (_V_MedProductType != value)
                {
                    _V_MedProductType = value;
                    NotifyOfPropertyChange(() => V_MedProductType);
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

        private DataEntities.RefGenMedProductDetails _ObjDrug_Current;
        public DataEntities.RefGenMedProductDetails ObjDrug_Current
        {
            get { return _ObjDrug_Current; }
            set
            {
                _ObjDrug_Current = value;
                NotifyOfPropertyChange(() => ObjDrug_Current);
            }
        }

        private DataEntities.DrugDeptSellingItemPrices _ObjDrugDeptSellingItemPrices_Current;
        public DataEntities.DrugDeptSellingItemPrices ObjDrugDeptSellingItemPrices_Current
        {
            get { return _ObjDrugDeptSellingItemPrices_Current; }
            set
            {
                _ObjDrugDeptSellingItemPrices_Current = value;
                NotifyOfPropertyChange(() => ObjDrugDeptSellingItemPrices_Current);
            }
        }

        public void InitializeNewItem()
        {
            ObjDrugDeptSellingItemPrices_Current = new DrugDeptSellingItemPrices();
            ObjDrugDeptSellingItemPrices_Current.GenMedProductID = ObjDrug_Current.GenMedProductID;
            ObjDrugDeptSellingItemPrices_Current.NormalPrice = 0;
            ObjDrugDeptSellingItemPrices_Current.PriceForHIPatient = 0;
            ObjDrugDeptSellingItemPrices_Current.HIAllowedPrice = 0;
            ObjDrugDeptSellingItemPrices_Current.EffectiveDate = DateTime.Now;

            ObjDrugDeptSellingItemPrices_Current.StaffID = Globals.LoggedUserAccount.Staff.StaffID;

            ObjDrugDeptSellingItemPrices_Current.ApprovedStaffID = ObjDrugDeptSellingItemPrices_Current.StaffID;

        }

        public void btSave()
        {
            if (CheckValid(ObjDrugDeptSellingItemPrices_Current))
            {
                if (CheckGiaChenhLech())
                {
                    DrugDeptSellingItemPrices_Save();
                }
            }
        }

        private bool CheckGiaChenhLech()
        {
            if (ObjDrugDeptSellingItemPrices_Current.NormalPrice >= 1)
            {
                if (ObjDrugDeptSellingItemPrices_Current.NormalPrice < ObjDrugDeptSellingItemPrices_Current.PriceForHIPatient)
                {
                    MessageBox.Show(eHCMSResources.Z0573_G1_GiaBNBHNhoHonDGia);
                    return false;
                }
                else
                {
                    if (ObjDrugDeptSellingItemPrices_Current.HIAllowedPrice > 0)
                    {
                        if (ObjDrugDeptSellingItemPrices_Current.HIAllowedPrice > ObjDrugDeptSellingItemPrices_Current.PriceForHIPatient)
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



        public void DrugDeptSellingItemPrices_Save()
        {
            string Result = "";

            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0645_G1_DangGhi) });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDrugDeptSellingItemPrices_Save(ObjDrugDeptSellingItemPrices_Current, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndDrugDeptSellingItemPrices_Save(out Result, asyncResult);
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
                                        Globals.EventAggregator.Publish(new ReLoadDataAfterU());
                                        Globals.EventAggregator.Publish(new ReLoadDataAfterCUD());

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
                                                DrugDeptSellingItemPrices_ByDrugDeptSellingItemPriceID(IDInsert);
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

        private void DrugDeptSellingItemPrices_ByDrugDeptSellingItemPriceID(Int64 DrugDeptSellingItemPricesID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0530_G1_DangRefresh) });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDrugDeptSellingItemPrices_ByDrugDeptSellingItemPriceID(DrugDeptSellingItemPricesID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            ObjDrugDeptSellingItemPrices_Current = contract.EndDrugDeptSellingItemPrices_ByDrugDeptSellingItemPriceID(asyncResult);
                            if (ObjDrugDeptSellingItemPrices_Current != null && ObjDrugDeptSellingItemPrices_Current.DrugDeptSellingItemPriceID > 0)
                            {
                                Globals.EventAggregator.Publish(new ReLoadDataAfterU());
                                Globals.EventAggregator.Publish(new ReLoadDataAfterCUD());
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

        public bool CheckValid(object temp)
        {
            DataEntities.DrugDeptSellingItemPrices p = temp as DataEntities.DrugDeptSellingItemPrices;
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
            if (ObjDrugDeptSellingItemPrices_Current.DrugDeptSellingItemPriceID <= 0)
            {
                decimal V = 0;

                if (IsNumeric(NormalPrice) == false)
                {
                    decimal.TryParse(NormalPrice.ToString(), out V);
                }
                else
                {
                    V = ObjDrugDeptSellingItemPrices_Current.NormalPrice;
                }

                ObjDrugDeptSellingItemPrices_Current.NormalPrice = V;
                ObjDrugDeptSellingItemPrices_Current.PriceForHIPatient = V;
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
                    V = ObjDrugDeptSellingItemPrices_Current.PriceForHIPatient;
                }

                ObjDrugDeptSellingItemPrices_Current.PriceForHIPatient = V;

            }
            else
            {
                ObjDrugDeptSellingItemPrices_Current.PriceForHIPatient = 0;
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
                    V = ObjDrugDeptSellingItemPrices_Current.HIAllowedPrice;
                }

                ObjDrugDeptSellingItemPrices_Current.HIAllowedPrice = V;
            }
            else
            {
                ObjDrugDeptSellingItemPrices_Current.HIAllowedPrice = 0;
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
                ObjDrugDeptSellingItemPrices_Current.EffectiveDate = DateTime.Now;
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

