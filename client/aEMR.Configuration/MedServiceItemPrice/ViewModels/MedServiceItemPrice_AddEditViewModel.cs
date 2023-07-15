using eHCMSLanguage;
using System;
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
using aEMR.ViewContracts;

namespace aEMR.Configuration.MedServiceItemPrice.ViewModels
{
    [Export(typeof(IMedServiceItemPrice_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MedServiceItemPrice_AddEditViewModel : Conductor<object>, IMedServiceItemPrice_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public MedServiceItemPrice_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
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



        private DataEntities.DeptMedServiceItems _ObjDeptMedServiceItems_Current;
        public DataEntities.DeptMedServiceItems ObjDeptMedServiceItems_Current
        {
            get { return _ObjDeptMedServiceItems_Current;}
            set
            {
                _ObjDeptMedServiceItems_Current = value;
                NotifyOfPropertyChange(() => ObjDeptMedServiceItems_Current);
            }
        }

        private object _IDeptMedServiceItems_Current;
        public object IDeptMedServiceItems_Current
        {
            get { return _IDeptMedServiceItems_Current; }
            set
            {
                _IDeptMedServiceItems_Current = value;
                NotifyOfPropertyChange(() => IDeptMedServiceItems_Current);
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

        public void LoadForm()
        {
            //Thông tin Khoa-Dịch Vụ
            ObjDeptMedServiceItems_Current=(IDeptMedServiceItems_Current as DataEntities.DeptMedServiceItems);
            //Thông tin Khoa-Dịch Vụ
            
            ObjMedServiceItemPrice_Save = (IMedServiceItemPrice_Save as DataEntities.MedServiceItemPrice);
        }

        private DataEntities.MedServiceItemPrice _ObjMedServiceItemPrice_Save;
        public DataEntities.MedServiceItemPrice ObjMedServiceItemPrice_Save
        {
            get { return _ObjMedServiceItemPrice_Save; }
            set { _ObjMedServiceItemPrice_Save = value;
                NotifyOfPropertyChange(()=>ObjMedServiceItemPrice_Save);
            }
        }

        private object _IMedServiceItemPrice_Save;
        public object IMedServiceItemPrice_Save
        {
            get { return _IMedServiceItemPrice_Save; }
            set { _IMedServiceItemPrice_Save = value;
                NotifyOfPropertyChange(()=>IMedServiceItemPrice_Save);
            }
        }

        public void btSave()
        {
            if (CheckValid(ObjMedServiceItemPrice_Save))
            {
                if (CheckGiaChenhLech())
                {
                    MedServiceItemPrice_Save();
                }
            }
        }

    
        public void MedServiceItemPrice_Save()
        {
            string Result = "";

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Ghi..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginMedServiceItemPrice_Save(ObjMedServiceItemPrice_Save, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndMedServiceItemPrice_Save(out Result, asyncResult);
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
                                        MessageBox.Show(eHCMSResources.Z0648_G1_NgGiaApDungNhoHonBangNgHHanh, eHCMSResources.Z0524_G1_ThemGia, MessageBoxButton.OK);
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
                                        MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.K1652_G1_CNhatGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new MedServiceItemPrice_AddEditViewModel_Save_Event() { Result = true });
                                        MessageBox.Show(eHCMSResources.K2782_G1_DaCNhat, eHCMSResources.K1652_G1_CNhatGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0-PriceCurrent-EffectiveDate":
                                    {
                                        MessageBox.Show(eHCMSResources.Z0652_G1_I, eHCMSResources.K1652_G1_CNhatGia, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0-PriceFuture-EffectiveDate":
                                    {
                                        MessageBox.Show(eHCMSResources.A0455_G1_Msg_InfoDaCoGiaApDung, eHCMSResources.K1652_G1_CNhatGia, MessageBoxButton.OK);
                                        break;
                                    }
                                default:
                                    {
                                        if (Result.IndexOf(eHCMSResources.T1794_G1_ID) != -1)
                                        {
                                            Int64 IDInsert = 0;
                                            Int64.TryParse(Result.Replace(eHCMSResources.T1794_G1_ID, ""), out IDInsert);
                                            if (IDInsert > 0)
                                            {
                                                MedServiceItemPrice_ByMedServItemPriceID(IDInsert);
                                            }
                                            else
                                            {
                                                MessageBox.Show(eHCMSResources.Z0654_G1_GhiKgThCong, eHCMSResources.A0464_G1_Msg_Ghi, MessageBoxButton.OK);
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
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        private  void MedServiceItemPrice_ByMedServItemPriceID(Int64 MedServItemPriceID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang refresh..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginMedServiceItemPrice_ByMedServItemPriceID(MedServItemPriceID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            ObjMedServiceItemPrice_Save = contract.EndMedServiceItemPrice_ByMedServItemPriceID(asyncResult);
                            if(ObjMedServiceItemPrice_Save!=null && ObjMedServiceItemPrice_Save.MedServItemPriceID>0)
                            {
                                Globals.EventAggregator.Publish(new MedServiceItemPrice_AddEditViewModel_Save_Event() { Result = true });

                                MessageBox.Show(eHCMSResources.Z0655_G1_DaGhi, eHCMSResources.A0464_G1_Msg_Ghi, MessageBoxButton.OK);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z0654_G1_GhiKgThCong, eHCMSResources.A0464_G1_Msg_Ghi, MessageBoxButton.OK);
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

        public bool CheckValid(object temp)
        {
            DataEntities.MedServiceItemPrice p = temp as DataEntities.MedServiceItemPrice;
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }


        private bool CheckGiaChenhLech()
        {
            if (ObjMedServiceItemPrice_Save.NormalPrice >= 1)
            {
                if (ObjMedServiceItemPrice_Save.NormalPrice < ObjMedServiceItemPrice_Save.PriceForHIPatient)
                {
                    MessageBox.Show(eHCMSResources.Z0573_G1_GiaBNBHNhoHonDGia);
                    return false;
                }
                else
                {
                    if (ObjMedServiceItemPrice_Save.HIAllowedPrice != null && ObjMedServiceItemPrice_Save.HIAllowedPrice > 0)
                    {
                        if (ObjMedServiceItemPrice_Save.HIAllowedPrice > ObjMedServiceItemPrice_Save.PriceForHIPatient)
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

        public void btClose()
        {
            TryClose();
        }

        #region "LostFocus"
        public void LostFocus_NormalPrice(object NormalPrice)
        {
            if (ObjMedServiceItemPrice_Save.MedServItemPriceID <= 0)
            {
                decimal V = 0;

                if (IsNumeric(NormalPrice) == false)
                {
                    decimal.TryParse(NormalPrice.ToString(), out V);
                }
                else
                {
                    V = ObjMedServiceItemPrice_Save.NormalPrice;
                }
                
                ObjMedServiceItemPrice_Save.NormalPrice = V;
                ObjMedServiceItemPrice_Save.PriceForHIPatient = V;

                if (ObjMedServiceItemPrice_Save.HIAllowedPrice != null && ObjMedServiceItemPrice_Save.HIAllowedPrice > 0)
                {
                    if (ObjMedServiceItemPrice_Save.PriceForHIPatient >= ObjMedServiceItemPrice_Save.HIAllowedPrice)
                    {
                        ObjMedServiceItemPrice_Save.PriceDifference = ObjMedServiceItemPrice_Save.PriceForHIPatient -
                                                                      ObjMedServiceItemPrice_Save.HIAllowedPrice;
                    }
                }
                else
                {
                    ObjMedServiceItemPrice_Save.PriceDifference = ObjMedServiceItemPrice_Save.PriceForHIPatient;
                }
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
                    V = ObjMedServiceItemPrice_Save.PriceForHIPatient.Value;
                }
                
                ObjMedServiceItemPrice_Save.PriceForHIPatient = V;

                if (ObjMedServiceItemPrice_Save.HIAllowedPrice != null && ObjMedServiceItemPrice_Save.HIAllowedPrice > 0)
                {
                    if (ObjMedServiceItemPrice_Save.PriceForHIPatient >= ObjMedServiceItemPrice_Save.HIAllowedPrice)
                    {
                        ObjMedServiceItemPrice_Save.PriceDifference = ObjMedServiceItemPrice_Save.PriceForHIPatient - ObjMedServiceItemPrice_Save.HIAllowedPrice;
                    }
                }
                else
                {
                    ObjMedServiceItemPrice_Save.PriceDifference = ObjMedServiceItemPrice_Save.PriceForHIPatient;
                }
            }
            else
            {
                ObjMedServiceItemPrice_Save.PriceForHIPatient = null;
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
                    V = ObjMedServiceItemPrice_Save.VATRate.Value;
                }
                
                ObjMedServiceItemPrice_Save.VATRate = V;
            }
            else
            {
                ObjMedServiceItemPrice_Save.VATRate = null;
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
                    V = ObjMedServiceItemPrice_Save.HIAllowedPrice.Value;
                }
                
                ObjMedServiceItemPrice_Save.HIAllowedPrice = V;

                if (ObjMedServiceItemPrice_Save.HIAllowedPrice != null && ObjMedServiceItemPrice_Save.HIAllowedPrice > 0)
                {
                    if (ObjMedServiceItemPrice_Save.PriceForHIPatient >= ObjMedServiceItemPrice_Save.HIAllowedPrice)
                    {
                        ObjMedServiceItemPrice_Save.PriceDifference = ObjMedServiceItemPrice_Save.PriceForHIPatient - ObjMedServiceItemPrice_Save.HIAllowedPrice;
                    }
                }
            }
            else
            {
                ObjMedServiceItemPrice_Save.HIAllowedPrice = null;
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
                ObjMedServiceItemPrice_Save.EffectiveDate = DateTime.Now;
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
