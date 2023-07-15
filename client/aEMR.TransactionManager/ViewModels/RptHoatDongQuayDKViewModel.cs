using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using System.Collections.ObjectModel;
using aEMR.CommonTasks;
using aEMR.Common.ExportExcel;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Linq;
using Microsoft.Win32;

namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(IRptHoatDongQuayDK)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RptHoatDongQuayDKViewModel : Conductor<object>, IRptHoatDongQuayDK
    {
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
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RptHoatDongQuayDKViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            // TxD 02/08/2014: Use Globals Server Date instead
            //GetCurrentDate();
            DateTime todayDate = Globals.GetCurServerDateTime();
            FromDate = todayDate;
            ToDate = todayDate;

            GetDeptLocationFuncExt((long)V_DeptTypeOperation.PhongDangKy);
        }

        // TxD 02/08/2014: The following method is nolonger required
        //public void GetCurrentDate()
        //{
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            IsLoading = true;

        //            using (var serviceFactory = new CommonServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;

        //                contract.BeginGetDate(Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        DateTime date = contract.EndGetDate(asyncResult);
        //                        FromDate = date;
        //                        ToDate = date;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                    }
        //                    finally
        //                    {
        //                        IsLoading = false;
        //                    }

        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //        }
        //    });
        //    t.Start();
        //}

        public void GetDeptLocationFuncExt(long DeptTypeOperation)
        {
            var t = new Thread(() =>
            {
                try
                {
                    IsLoading = true;

                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetDeptLocationFuncExt(0,0,DeptTypeOperation,Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Locations = contract.EndGetDeptLocationFuncExt(asyncResult).ToObservableCollection();
                                var itemDefault = new DeptLocation();
                                itemDefault.DeptID = 0;
                                itemDefault.DeptLocationID = 0;
                                itemDefault.Location = new Location();
                                itemDefault.Location.LID = -1;
                                itemDefault.Location.LocationName = "--Các Quầy Đăng Ký--";
                                Locations.Insert(0, itemDefault);
                                SelectedLocation = Locations.FirstOrDefault();
                            }
                            catch (Exception ex)
                            {
                            }
                            finally
                            {
                                IsLoading = false;
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });
            t.Start();
        }

        private Nullable<DateTime> _FromDate;
        public Nullable<DateTime> FromDate
        {
            get { return _FromDate; }
            set
            {
                if (_FromDate != value)
                {
                    _FromDate = value;
                    NotifyOfPropertyChange(() => FromDate);
                }
            }
        }

        private Nullable<DateTime> _ToDate;
        public Nullable<DateTime> ToDate
        {
            get { return _ToDate; }
            set
            {
                if (_ToDate != value)
                {
                    _ToDate = value;
                    NotifyOfPropertyChange(() => ToDate);
                }
            }
        }


        public void btView()
        {
            if (CheckDateValid())
            {
                //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
                //proAlloc.TieuDeRpt = string.Format("Báo Cáo Hoạt Động Quầy Đăng Ký Từ Ngày {0} Đến Ngày {1}", FromDate.Value.ToString("dd/MM/yyyy"), ToDate.Value.ToString("dd/MM/yyyy"));
                //proAlloc.LocationName = SelectedLocation != null && SelectedLocation.Location != null && SelectedLocation.Location.LID > 0 ? SelectedLocation.Location.LocationName : null;
                //proAlloc.DeptLocID = SelectedLocation != null ? SelectedLocation.DeptLocationID : 0;
                //proAlloc.FromDate = FromDate;
                //proAlloc.ToDate = ToDate;
                //proAlloc.eItem = ReportName.RptHoatDongQuayDK;

                //var instance = proAlloc as Conductor<object>;
                //Globals.ShowDialog(instance, (o) => { });

                Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
                {
                    proAlloc.TieuDeRpt = string.Format("Báo Cáo Hoạt Động Quầy Đăng Ký Từ Ngày {0} Đến Ngày {1}", FromDate.Value.ToString("dd/MM/yyyy"), ToDate.Value.ToString("dd/MM/yyyy"));
                    proAlloc.LocationName = SelectedLocation != null && SelectedLocation.Location != null && SelectedLocation.Location.LID > 0 ? SelectedLocation.Location.LocationName : null;
                    proAlloc.DeptLocID = SelectedLocation != null ? SelectedLocation.DeptLocationID : 0;
                    proAlloc.FromDate = FromDate;
                    proAlloc.ToDate = ToDate;
                    proAlloc.eItem = ReportName.RptHoatDongQuayDK;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
        }

        private bool CheckDateValid()
        {
            if (FromDate != null && ToDate != null)
            {
                DateTime F = DateTime.Now;
                DateTime.TryParse(FromDate.ToString(), out F);

                DateTime T = DateTime.Now;
                DateTime.TryParse(ToDate.ToString(), out T);

                if (F > T)
                {
                    MessageBox.Show(eHCMSResources.Z0467_G1_TuNgNhoHonBangDenNg, eHCMSResources.G0981_G1_Tim, MessageBoxButton.OK);
                    return false;
                }
                return true;
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0884_G1_Msg_InfoNhapTuNgDenNg);
                return false;
            }
        }

        private DeptLocation _selectedLocation;
        public DeptLocation SelectedLocation
        {
            get
            {
                return _selectedLocation;
            }
            set
            {
                _selectedLocation = value;
                NotifyOfPropertyChange(() => SelectedLocation);
            }
        }

        private ObservableCollection<DeptLocation> _locations;
        public ObservableCollection<DeptLocation> Locations
        {
            get
            {
                return _locations;
            }
            set
            {
                _locations = value;
                NotifyOfPropertyChange(() => Locations);
            }
        }

        
        #region Export excel from database

        public void btnExportExcel()
        {
            if (CheckDateValid())
            {
                //ExportToExcellAll_DoanhThuPK();
                SaveFileDialog objSFD = new SaveFileDialog()
                {
                    DefaultExt = ".xls",
                    Filter = "Excel xls (*.xls)|*.xls",
                    FilterIndex = 1
                };
                if (objSFD.ShowDialog() != true)
                {
                    return;
                }
                IsLoading = true;
                ReportParameters RptParameters = new ReportParameters 
                { FromDate = FromDate, ToDate = ToDate, DeptLocID = SelectedLocation != null ? SelectedLocation.DeptLocationID : 0};
                RptParameters.ReportType = ReportType.BAOCAO_TONGHOP;
                RptParameters.Show = "HoatDongQuayDangKy";
                RptParameters.reportName = ReportName.RptHoatDongQuayDK;

                ExportToExcelGeneric.Action(RptParameters, objSFD, this);
                //Coroutine.BeginExecute(DoSaveExcel(RptParameters, objSFD));
            }
        }

        //private IEnumerator<IResult> DoSaveExcel(ReportParameters rptParameters, SaveFileDialog objSFD) 
        //{
        //    var res = new ExportToExcellAllGenericTask(rptParameters, objSFD);
        //    yield return res;
        //    IsLoading = false;
        //    yield break;
        //}
        public void ExportToExcellAll_DoanhThuPK()
        {
            ReportParameters RptParameters = new ReportParameters { FromDate=FromDate,ToDate=ToDate, DeptLocID = SelectedLocation != null ? SelectedLocation.DeptLocationID : 0};
            //isLoadingSearch = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginExportToExcellAll_DoanhThuTongHop(RptParameters, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndExportToExcellAll_DoanhThuTongHop(asyncResult);
                            ExportToExcelFileAllData.Export(results, "DoanhThuTongHop");
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingSearch = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        
        #endregion


    }

}
