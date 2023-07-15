using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using DataEntities;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(ICMDrugDeptSellingItemPrices_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CMDrugDeptSellingItemPrices_AddEditViewModel : Conductor<object>, ICMDrugDeptSellingItemPrices_AddEdit
    {
        [ImportingConstructor]
        public CMDrugDeptSellingItemPrices_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {

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

        private RefGenMedProductDetails _ObjDrug_Current;
        public RefGenMedProductDetails ObjDrug_Current
        {
            get { return _ObjDrug_Current; }
            set
            {
                _ObjDrug_Current = value;
                NotifyOfPropertyChange(() => ObjDrug_Current);
            }
        }

        private DrugDeptSellingItemPrices _ObjDrugDeptSellingItemPrices_Current;
        public DrugDeptSellingItemPrices ObjDrugDeptSellingItemPrices_Current
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
            ObjDrugDeptSellingItemPrices_Current.EffectiveDate = Globals.ServerDate.Value;

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
                    MessageBox.Show(eHCMSResources.T1984_G1_GiaBHChoPhep3);
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
                MessageBox.Show(eHCMSResources.A0519_G1_Msg_InfoDGiaLonHon0);
                return false;
            }
            return true;
        }

        public void DrugDeptSellingItemPrices_Save()
        {
            string Result = "";

            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0645_G1_DangGhi) });

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
                                        MessageBox.Show(eHCMSResources.A0454_G1_Msg_InfoKhTheTaoThemGiaApDung, eHCMSResources.Z0524_G1_ThemGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A1025_G1_Msg_InfoThemGiaFail, eHCMSResources.Z0524_G1_ThemGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-0-PriceCurrent-EffectiveDate":
                                    {
                                        MessageBox.Show(eHCMSResources.A0839_G1_Msg_InfoNgApDung2, eHCMSResources.Z0524_G1_ThemGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-0-PriceFuture-Exists":
                                    {
                                        MessageBox.Show(eHCMSResources.A0553_G1_Msg_InfoKhTheTaoGiaMoi, eHCMSResources.Z0524_G1_ThemGia, MessageBoxButton.OK);
                                        break;
                                    }

                                case "Update-0-PriceOld-NotAllowUpdate":
                                    {
                                        MessageBox.Show(eHCMSResources.A0551_G1_Msg_InfoKhDcCNhatGiaCu, eHCMSResources.K1652_G1_CNhatGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.K1652_G1_CNhatGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new ReLoadDataAfterU());
                                        Globals.EventAggregator.Publish(new ReLoadDataAfterCUD());

                                        MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.K1652_G1_CNhatGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0-PriceCurrent-EffectiveDate":
                                    {
                                        MessageBox.Show(eHCMSResources.A0839_G1_Msg_InfoNgApDung, eHCMSResources.K1652_G1_CNhatGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0-PriceFuture-EffectiveDate":
                                    {
                                        MessageBox.Show(eHCMSResources.A0455_G1_Msg_InfoDaCoGiaApDung, eHCMSResources.K1652_G1_CNhatGia, MessageBoxButton.OK);
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
                                                MessageBox.Show(eHCMSResources.A0549_G1_Msg_InfoGhiDataFail, eHCMSResources.A0464_G1_Msg_Ghi, MessageBoxButton.OK);
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
                            Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        private void DrugDeptSellingItemPrices_ByDrugDeptSellingItemPriceID(Int64 DrugDeptSellingItemPricesID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0530_G1_DangRefresh) });

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
                                MessageBox.Show(eHCMSResources.A0457_G1_Msg_InfoDaGhi, eHCMSResources.A0464_G1_Msg_Ghi, MessageBoxButton.OK);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0549_G1_Msg_InfoGhiDataFail, eHCMSResources.A0464_G1_Msg_Ghi, MessageBoxButton.OK);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
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
            if (ObjDrugDeptSellingItemPrices_Current == null) return;
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
            if (ObjDrugDeptSellingItemPrices_Current == null) return;
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
            if (ObjDrugDeptSellingItemPrices_Current == null) return;
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
            if (ObjDrugDeptSellingItemPrices_Current == null) return;
            if (EffectiveDate != null)
            {

                DateTime V = Globals.ServerDate.Value;
                DateTime.TryParse(EffectiveDate.ToString(), out V);
            }
            else
            {
                ObjDrugDeptSellingItemPrices_Current.EffectiveDate = Globals.ServerDate.Value;
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