using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using aEMR.DataContracts;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Controls;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common.BaseModel;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(ISupplierGenericDrugPrice_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SupplierGenericDrugPrice_AddEditViewModel : ViewModelBase, ISupplierGenericDrugPrice_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SupplierGenericDrugPrice_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

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

        private SupplierGenericDrugPrice _ObjDrugCurrent;
        public SupplierGenericDrugPrice ObjDrugCurrent
        {
            get { return _ObjDrugCurrent; }
            set
            {
                _ObjDrugCurrent = value;
                NotifyOfPropertyChange(() => ObjDrugCurrent);
            }
        }


        private SupplierGenericDrugPrice _ObjSupplierGenericDrugPrice_Current;
        public SupplierGenericDrugPrice ObjSupplierGenericDrugPrice_Current
        {
            get { return _ObjSupplierGenericDrugPrice_Current; }
            set
            {
                _ObjSupplierGenericDrugPrice_Current = value;
                NotifyOfPropertyChange(() => ObjSupplierGenericDrugPrice_Current);
            }
        }


        public void InitializeNewItem()
        {
            ObjSupplierGenericDrugPrice_Current = new SupplierGenericDrugPrice();


            ObjSupplierGenericDrugPrice_Current.StaffID = Globals.LoggedUserAccount.Staff != null
                                                                           ? Globals.LoggedUserAccount.Staff.
                                                                                 StaffID
                                                                           : (Globals.LoggedUserAccount.StaffID.
                                                                                  HasValue
                                                                                  ? Globals.LoggedUserAccount.
                                                                                        StaffID.Value
                                                                                  : -1);
            ObjSupplierGenericDrugPrice_Current.ApprovedStaffID = ObjSupplierGenericDrugPrice_Current.StaffID;

            ObjSupplierGenericDrugPrice_Current.UnitPrice = 0;
            ObjSupplierGenericDrugPrice_Current.PackagePrice = 0;
            ObjSupplierGenericDrugPrice_Current.VAT = 0;
            ObjSupplierGenericDrugPrice_Current.EffectiveDate = Globals.ServerDate.Value;

            ObjSupplierGenericDrugPrice_Current.SupplierID = ObjSupplierCurrent.SupplierID;
            ObjSupplierGenericDrugPrice_Current.DrugID = ObjDrugCurrent.DrugID;

        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

        }
        #region checking account

        private bool _bEdit = true;
        private bool _bAdd = true;
        private bool _bDelete = true;
        private bool _bView = true;
        private bool _bPrint = true;
        private bool _bReport = true;

        public bool bEdit
        {
            get
            {
                return _bEdit;
            }
            set
            {
                if (_bEdit == value)
                    return;
                _bEdit = value;
            }
        }
        public bool bAdd
        {
            get
            {
                return _bAdd;
            }
            set
            {
                if (_bAdd == value)
                    return;
                _bAdd = value;
            }
        }
        public bool bDelete
        {
            get
            {
                return _bDelete;
            }
            set
            {
                if (_bDelete == value)
                    return;
                _bDelete = value;
            }
        }
        public bool bView
        {
            get
            {
                return _bView;
            }
            set
            {
                if (_bView == value)
                    return;
                _bView = value;
            }
        }
        public bool bPrint
        {
            get
            {
                return _bPrint;
            }
            set
            {
                if (_bPrint == value)
                    return;
                _bPrint = value;
            }
        }

        public bool bReport
        {
            get
            {
                return _bReport;
            }
            set
            {
                if (_bReport == value)
                    return;
                _bReport = value;
            }
        }
        #endregion
        #region binding visibilty

        public Button hplMgntDrug { get; set; }

        public void hplMgntDrug_Loaded(object sender)
        {
            hplMgntDrug = sender as Button;
            hplMgntDrug.Visibility = Globals.convertVisibility(bView);
        }

        #endregion


        public void btSave()
        {
            if (CheckValid(ObjSupplierGenericDrugPrice_Current))
            {
                ObjSupplierGenericDrugPrice_Current.StaffID = Globals.LoggedUserAccount.Staff != null
                                                                         ? Globals.LoggedUserAccount.Staff.
                                                                               StaffID
                                                                         : (Globals.LoggedUserAccount.StaffID.
                                                                                HasValue
                                                                                ? Globals.LoggedUserAccount.
                                                                                      StaffID.Value
                                                                                : -1);
                ObjSupplierGenericDrugPrice_Current.ApprovedStaffID = ObjSupplierGenericDrugPrice_Current.StaffID;

                SupplierGenericDrugPrice_Save(ObjSupplierGenericDrugPrice_Current);
            }
        }

        public bool CheckValid(object temp)
        {
            SupplierGenericDrugPrice u = temp as SupplierGenericDrugPrice;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }



        public void SupplierGenericDrugPrice_Save(SupplierGenericDrugPrice Obj)
        {
            string Result = "";

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0645_G1_DangGhi) });
            this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0645_G1_DangGhi));
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacySuppliersServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginSupplierGenericDrugPrice_Save(Obj, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndSupplierGenericDrugPrice_Save(out Result, asyncResult);
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
                                            Globals.EventAggregator.Publish(new SaveEvent<bool>() { Result = true });

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
                                            if (Result.IndexOf("ID") != -1)
                                            {
                                                Int64 IDInsert = 0;
                                                Int64.TryParse(Result.Replace("ID", ""), out IDInsert);
                                                if (IDInsert > 0)
                                                {
                                                    SupplierGenericDrugPrice_ByPKID(IDInsert);
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

        private void SupplierGenericDrugPrice_ByPKID(Int64 PKID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0530_G1_DangRefresh)});
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSupplierGenericDrugPrice_ByPKID(PKID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            ObjSupplierGenericDrugPrice_Current = contract.EndSupplierGenericDrugPrice_ByPKID(asyncResult);
                            if (ObjSupplierGenericDrugPrice_Current != null && ObjSupplierGenericDrugPrice_Current.PKID > 0)
                            {
                                Globals.EventAggregator.Publish(new SaveEvent<bool>() { Result = true });

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
        public void LostFocus_UnitPrice(object sender)
        {
            AxTextBox UnitPrice = sender as AxTextBox;
            if (UnitPrice != null)
            {
                decimal V = 0;

                if (IsNumeric(UnitPrice.Text) == false)
                {
                    decimal.TryParse(UnitPrice.ToString(), out V);
                }
                else
                {
                    V = ObjSupplierGenericDrugPrice_Current.UnitPrice;
                }

                ObjSupplierGenericDrugPrice_Current.PackagePrice = V * (ObjDrugCurrent.ObjRefGenericDrugDetail != null ? ObjDrugCurrent.ObjRefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1) : 1);
                ObjSupplierGenericDrugPrice_Current.UnitPrice = V;
            }
        }

        public void LostFocus_PackagePrice(object sender)
        {
            AxTextBox PackagePrice = sender as AxTextBox;
            if (PackagePrice != null)
            {
                decimal V = 0;

                if (IsNumeric(PackagePrice.Text) == false)
                {
                    decimal.TryParse(PackagePrice.ToString(), out V);
                }
                else
                {
                    V = ObjSupplierGenericDrugPrice_Current.PackagePrice;
                }

                if (ObjDrugCurrent.ObjRefGenericDrugDetail != null && ObjDrugCurrent.ObjRefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1) > 0)
                {
                    ObjSupplierGenericDrugPrice_Current.UnitPrice = ObjSupplierGenericDrugPrice_Current.PackagePrice / ObjDrugCurrent.ObjRefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
                }
                ObjSupplierGenericDrugPrice_Current.PackagePrice = V;
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
                    V = ObjSupplierGenericDrugPrice_Current.VAT.Value;
                }

                ObjSupplierGenericDrugPrice_Current.VAT = V;
            }
            else
            {
                ObjSupplierGenericDrugPrice_Current.VAT = 0;
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
                ObjSupplierGenericDrugPrice_Current.EffectiveDate = Globals.ServerDate.Value;
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
