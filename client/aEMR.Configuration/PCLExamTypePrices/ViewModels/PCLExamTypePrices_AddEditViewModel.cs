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
using DataEntities;
using aEMR.ViewContracts;

namespace aEMR.Configuration.PCLExamTypePrices.ViewModels
{
    [Export(typeof(IPCLExamTypePrices_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTypePrices_AddEditViewModel : Conductor<object>, IPCLExamTypePrices_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PCLExamTypePrices_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
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


        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void  OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
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

        private DataEntities.PCLExamType _ObjPCLExamType_Current;
        public DataEntities.PCLExamType ObjPCLExamType_Current
        {
            get { return _ObjPCLExamType_Current; }
            set
            {
                _ObjPCLExamType_Current = value;
                NotifyOfPropertyChange(() => ObjPCLExamType_Current);
            }
        }

        private DataEntities.PCLExamTypePrice _ObjPCLExamTypePrice_Current;
        public DataEntities.PCLExamTypePrice ObjPCLExamTypePrice_Current
        {
            get { return _ObjPCLExamTypePrice_Current; }
            set
            {
                _ObjPCLExamTypePrice_Current = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypePrice_Current);
            }
        }

        public void InitializeNewItem()
        {
            ObjPCLExamTypePrice_Current = new PCLExamTypePrice();
            ObjPCLExamTypePrice_Current.PCLExamTypeID = ObjPCLExamType_Current.PCLExamTypeID;
            ObjPCLExamTypePrice_Current.NormalPrice = 0;
            ObjPCLExamTypePrice_Current.PriceForHIPatient = 0;
            ObjPCLExamTypePrice_Current.HIAllowedPrice = 0;
            ObjPCLExamTypePrice_Current.EffectiveDate = DateTime.Now;
            
            ObjPCLExamTypePrice_Current.StaffID =  Globals.LoggedUserAccount.Staff != null
                                                                               ? Globals.LoggedUserAccount.Staff.
                                                                                     StaffID
                                                                               : (Globals.LoggedUserAccount.StaffID.
                                                                                      HasValue
                                                                                      ? Globals.LoggedUserAccount.
                                                                                            StaffID.Value
                                                                                      : -1);

            ObjPCLExamTypePrice_Current.ApprovedStaffID = ObjPCLExamTypePrice_Current.StaffID;
            
        }

        public void btSave()
        {
            if (CheckValid(ObjPCLExamTypePrice_Current))
            {
                if (CheckGiaChenhLech())
                {
                    PCLExamTypePrices_Save();
                }
            }
        }

        private bool CheckGiaChenhLech()
        {
            if (ObjPCLExamTypePrice_Current.NormalPrice >= 1)
            {
                if (ObjPCLExamTypePrice_Current.NormalPrice < ObjPCLExamTypePrice_Current.PriceForHIPatient)
                {
                    MessageBox.Show(eHCMSResources.Z0573_G1_GiaBNBHNhoHonDGia);
                    return false;
                }
                else
                {
                    if (ObjPCLExamTypePrice_Current.HIAllowedPrice != null && ObjPCLExamTypePrice_Current.HIAllowedPrice > 0)
                    {
                        if (ObjPCLExamTypePrice_Current.HIAllowedPrice > ObjPCLExamTypePrice_Current.PriceForHIPatient)
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



        public void PCLExamTypePrices_Save()
        {
            string Result = "";

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Ghi..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypePrices_Save(ObjPCLExamTypePrice_Current, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndPCLExamTypePrices_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Insert-0-PriceCurrent-Exists":
                                    {
                                        MessageBox.Show(string.Format("{0}! ({1}).", eHCMSResources.Z0646_G1_DaCoGiaApDung, eHCMSResources.Z0647_G1_ThayDoiGiaApDung), eHCMSResources.Z0524_G1_ThemGia, MessageBoxButton.OK);
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
                                        Globals.EventAggregator.Publish(new PCLExamTypePricesEvent() { Result_AddEditSave = true });

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
                                                PCLExamTypePrices_ByPCLExamTypePriceID(IDInsert);
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

        private void PCLExamTypePrices_ByPCLExamTypePriceID(Int64 PCLExamTypePriceID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang refresh..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypePrices_ByPCLExamTypePriceID(PCLExamTypePriceID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            ObjPCLExamTypePrice_Current = contract.EndPCLExamTypePrices_ByPCLExamTypePriceID(asyncResult);
                            if (ObjPCLExamTypePrice_Current != null && ObjPCLExamTypePrice_Current.PCLExamTypePriceID > 0)
                            {
                                Globals.EventAggregator.Publish(new PCLExamTypePricesEvent() { Result_AddEditSave = true });

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
            DataEntities.PCLExamTypePrice p = temp as DataEntities.PCLExamTypePrice;
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
             if (ObjPCLExamTypePrice_Current.PCLExamTypePriceID <= 0)
             {
                 decimal V = 0;

                 if (IsNumeric(NormalPrice) == false)
                 {
                     decimal.TryParse(NormalPrice.ToString(), out V);
                 }
                 else
                 {
                     V = ObjPCLExamTypePrice_Current.NormalPrice;
                 }
                 
                 ObjPCLExamTypePrice_Current.NormalPrice = V;
                 ObjPCLExamTypePrice_Current.PriceForHIPatient = V;
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
                    V = ObjPCLExamTypePrice_Current.PriceForHIPatient.Value;
                }
                
                ObjPCLExamTypePrice_Current.PriceForHIPatient = V;

            }
            else
            {
                ObjPCLExamTypePrice_Current.PriceForHIPatient = null;
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
                    V = ObjPCLExamTypePrice_Current.HIAllowedPrice.Value;
                }
                
                ObjPCLExamTypePrice_Current.HIAllowedPrice = V;
            }
            else
            {
                ObjPCLExamTypePrice_Current.HIAllowedPrice = null;
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
                ObjPCLExamTypePrice_Current.EffectiveDate = DateTime.Now;
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
