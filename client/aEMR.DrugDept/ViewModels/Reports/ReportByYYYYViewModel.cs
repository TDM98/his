using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using DataEntities;
using System.Linq;
using System.Collections.Generic;
using DevExpress.Xpf.Printing;
using aEMR.ReportModel.ReportModels;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DevExpress.ReportServer.Printing;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptReportByYYYY)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReportByYYYYViewModel : Conductor<object>, IDrugDeptReportByYYYY
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ReportByYYYYViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            RptParameters = new ReportParameters();
            FillYear();

            Coroutine.BeginExecute(DoGetStore_DrugDept());
            Coroutine.BeginExecute(DoGetRefGenericDrugCategory_1List());
        }

        #region Properties Member

        private string _strHienThi;
        public string strHienThi
        {
            get
            {
                return _strHienThi;
            }
            set
            {
                _strHienThi = value;
                NotifyOfPropertyChange(() => strHienThi);
            }
        }

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
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

        private ObservableCollection<RefStorageWarehouseLocation> _StoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> StoreCbx
        {
            get
            {
                return _StoreCbx;
            }
            set
            {
                if (_StoreCbx != value)
                {
                    _StoreCbx = value;
                    NotifyOfPropertyChange(() => StoreCbx);
                }
            }
        }

        private ReportName _eItem;
        public ReportName eItem
        {
            get
            {
                return _eItem;
            }
            set
            {
                _eItem = value;
                NotifyOfPropertyChange(() => eItem);
            }
        }

        private RemoteDocumentSource _reportModel;
        public RemoteDocumentSource ReportModel
        {
            get { return _reportModel; }
            set
            {
                _reportModel = value;
                NotifyOfPropertyChange(() => ReportModel);
            }
        }

        private ReportParameters _RptParameters;
        public ReportParameters RptParameters
        {
            get { return _RptParameters; }
            set
            {
                if (_RptParameters != value)
                {
                    _RptParameters = value;
                    NotifyOfPropertyChange(() => RptParameters);
                }
            }
        }

        private ObservableCollection<int> _ListYear;
        public ObservableCollection<int> ListYear
        {
            get { return _ListYear; }
            set
            {
                if (_ListYear != value)
                {
                    _ListYear = value;
                    NotifyOfPropertyChange(() => ListYear);
                }
            }
        }
        private ObservableCollection<RefGenericDrugCategory_1> _RefGenericDrugCategory_1s;
        public ObservableCollection<RefGenericDrugCategory_1> RefGenericDrugCategory_1s
        {
            get
            {
                return _RefGenericDrugCategory_1s;
            }
            set
            {
                if (_RefGenericDrugCategory_1s != value)
                {
                    _RefGenericDrugCategory_1s = value;
                    NotifyOfPropertyChange(() => RefGenericDrugCategory_1s);
                }
            }
        }

        #endregion
  
        #region FillData Member

        private void FillYear()
        {
            if (ListYear == null)
            {
                ListYear = new ObservableCollection<int>();
            }
            else
            {
                ListYear.Clear();
            }
            int year = DateTime.Now.Year;
            for (int i = year; i > year - 3; i--)
            {
                ListYear.Add(i);
            }
            RptParameters.Year = ListYear.FirstOrDefault();

        }

        #endregion

        private bool _mXemIn = true;
        private bool _mIn = true;

        public bool mXemIn
        {
            get
            {
                return _mXemIn;
            }
            set
            {
                if (_mXemIn == value)
                    return;
                _mXemIn = value;
                NotifyOfPropertyChange(() => mXemIn);
            }
        }
        public bool mIn
        {
            get
            {
                return _mIn;
            }
            set
            {
                if (_mIn == value)
                    return;
                _mIn = value;
                NotifyOfPropertyChange(() => mIn);
            }
        }

        private IEnumerator<IResult> DoGetRefGenericDrugCategory_1List()
        {
            var paymentTypeTask = new LoadRefGenericDrugCategory_1ListTask(V_MedProductType, false, false);
            yield return paymentTypeTask;
            RefGenericDrugCategory_1s = paymentTypeTask.RefGenericDrugCategory_1List;
            yield break;
        }

        private IEnumerator<IResult> DoGetStore_DrugDept()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false,null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            yield break;
        }

        private void GetReport()
        {
            var GiamDoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.GIAM_DOC && x.IsActive).FirstOrDefault();
            var KeToanTruong = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.KE_TOAN_TRUONG && x.IsActive).FirstOrDefault();
            var TruongKhoaDuoc = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.TRUONG_KHOA_DUOC && x.IsActive).FirstOrDefault();
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            switch (_eItem)
            {
                case ReportName.DRUGDEPT_REPORT_INOUTSTOCK_ADDICTIVE:
                    ReportModel = null;
                    ReportModel = new DrugDeptInOutStockAddictiveReportModal().PreviewModel;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["RefGenDrugCatID"].Value =(int)RptParameters.RefGenDrugCatID_1.GetValueOrDefault(0);
                    rParams["V_MedProductType"].Value = (int)V_MedProductType;
                    rParams["GiamDoc"].Value = GiamDoc != null ? GiamDoc.FullNameString : "";
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    break;
            }
            // ReportModel.AutoShowParametersPanel = false;
            ReportModel.CreateDocument(rParams);
        }

        #region Print Member

        public void btnXemIn(object sender, EventArgs e)
        {
            if (GetParameters())
            {
                GetReport();
            }
        }

        #endregion

        ComboBox cbx_NhomThuoc = null;
        public void KeyEnabledComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            cbx_NhomThuoc = sender as ComboBox;
        }
        private bool GetParameters()
        {
            bool result = true;
            if (RptParameters == null)
            {
                return false;
            }
            if (cbx_NhomThuoc != null && cbx_NhomThuoc.SelectedItem != null)
            {
                RptParameters.Show = string.Format("{0} ", eHCMSResources.Z0946_G1_BCSDThuoc.ToUpper()) + (cbx_NhomThuoc.SelectedItem as RefGenericDrugCategory_1).CategoryName.ToUpper() + " " + RptParameters.Year.ToString();
            }
            else
            {
                MessageBox.Show(eHCMSResources.K0377_G1_ChonNhomThuocBC);
                return false;
            }
            return result;
        }
    }
}
