using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Castle.Windsor;
using Castle.Core.Logging;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using aEMR.Common.Utilities;
using aEMR.Common.BaseModel;
/*
* 20181210 #001 TTM:   BM 0005406: Cho phép tìm kiếm CLS hình ảnh bằng AutoComplete
* 20181229 #002 TTM:   Clear dữ liệu khi đã chọn CLS từ autocomplete (bằng chuột hoặc tay)
* 20190813 #003 TTM:   BM 0013003: Bổ sung thêm chức năng gõ mã tìm dịch vụ ở chỉ định CLS
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPCLItems_SearchAutoComplete)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLItems_SearchAutoCompleteViewModel : ViewModelBase, IPCLItems_SearchAutoComplete
    {
        private ICS_DataStorage _CS_DS = null;
        public ICS_DataStorage CS_DS
        {
            get
            {
                return _CS_DS;
            }
            set
            {
                _CS_DS = value;
            }
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

        public string KeyAction { get; set; }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PCLItems_SearchAutoCompleteViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            SearchCriteria = new PCLExamTypeSearchCriteria();
            ObjPCLItems_SearchAutoComplete = new PagedSortableCollectionView<PCLExamType>();
            ObjPCLItems_SearchAutoComplete.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPCLItems_SearchAutoComplete_OnRefresh);

            dicPCLExamType = new Dictionary<long, PCLExamType>();
            dicPCLExamTypeImage = new Dictionary<long, PCLExamType>();
        }

        void ObjPCLItems_SearchAutoComplete_OnRefresh(object sender, RefreshEventArgs e)
        {
            PCLItems_SearchAutoComplete(ObjPCLItems_SearchAutoComplete.PageIndex, ObjPCLItems_SearchAutoComplete.PageSize, true);
        }

        private PCLExamType _selectedPCLExamType;
        public PCLExamType SelectedPCLExamType
        {
            get { return _selectedPCLExamType; }
            set
            {
                if (_selectedPCLExamType != value)
                {
                    _selectedPCLExamType = value;
                    NotifyOfPropertyChange(() => SelectedPCLExamType);
                }
            }
        }

        public Dictionary<long, PCLExamType> _dicPCLExamType;
        public Dictionary<long, PCLExamType> dicPCLExamType
        {
            get
            {
                return _dicPCLExamType;
            }
            set
            {
                _dicPCLExamType = value;
                NotifyOfPropertyChange(() => dicPCLExamType);

            }
        }

        private PCLExamTypeSearchCriteria _SearchCriteria;
        public PCLExamTypeSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }


        private PagedSortableCollectionView<PCLExamType> _ObjPCLItems_SearchAutoComplete;
        public PagedSortableCollectionView<PCLExamType> ObjPCLItems_SearchAutoComplete
        {
            get
            {
                return _ObjPCLItems_SearchAutoComplete;
            }
            set
            {
                if (_ObjPCLItems_SearchAutoComplete != value)
                {
                    _ObjPCLItems_SearchAutoComplete = value;
                    NotifyOfPropertyChange(() => ObjPCLItems_SearchAutoComplete);
                }
            }
        }

        AutoCompleteBox CtrcboAutoComplete;
        public void cboAutoComplete_Populating(object sender, PopulatingEventArgs e)
        {
            CtrcboAutoComplete = sender as AutoCompleteBox;

            //SearchCriteria.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Laboratory;
            SearchCriteria.PCLExamTypeSubCategoryID = 0;
            SearchCriteria.PCLExamTypeName = e.Parameter.Trim();
            SearchCriteria.PCLFormID = 0;

            ObjPCLItems_SearchAutoComplete.PageIndex = 0;
            //PCLItems_SearchAutoComplete(ObjPCLItems_SearchAutoComplete.PageIndex, ObjPCLItems_SearchAutoComplete.PageSize,true);
            if (V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
            {
                PCLItems_SearchAutoComplete();
            }
            else
            {
                PCLItems_SearchAutoComplete_ForImage();
            }
        }
        public void PCLItems_SearchAutoComplete()
        {
            if (!string.IsNullOrEmpty(SearchCriteria.PCLExamTypeName))
            {
                var resComBo = from c in dicPCLExamType
                               where (VNConvertString.ConvertString(c.Value.PCLExamTypeName).ToLower().Contains(VNConvertString.ConvertString(SearchCriteria.PCLExamTypeName).ToLower())
/*#003: Điều kiện bổ sung ở dòng này*/         || VNConvertString.ConvertString(c.Value.PCLExamTypeCode).ToLower().Contains(VNConvertString.ConvertString(SearchCriteria.PCLExamTypeName).ToLower()))
                                   && (!c.Value.IsRegimenChecking || !IsRegimenChecked || CS_DS == null || CS_DS.TreatmentRegimenCollection == null
                                       || CS_DS.TreatmentRegimenCollection.Count == 0
                                       || !CS_DS.TreatmentRegimenCollection.Any(x => x.RefTreatmentRegimenPCLDetails != null && x.RefTreatmentRegimenPCLDetails.Count > 0)
                                       || !CS_DS.TreatmentRegimenCollection.Where(x => x.RefTreatmentRegimenPCLDetails != null && x.RefTreatmentRegimenPCLDetails.Count > 0).Any(x => x.RefTreatmentRegimenPCLDetails.Count > 0)
                                       || CS_DS.TreatmentRegimenCollection.Where(x => x.RefTreatmentRegimenPCLDetails != null && x.RefTreatmentRegimenPCLDetails.Count > 0).SelectMany(x => x.RefTreatmentRegimenPCLDetails).Any(x => c.Value == null || x.PCLExamTypeID == c.Value.PCLExamTypeID))
                               select c;

                List<PCLExamType> allPclType = new List<PCLExamType>();
                ObjPCLItems_SearchAutoComplete.Clear();
                foreach (var item in resComBo)
                {
                    ObjPCLItems_SearchAutoComplete.Add(item.Value);
                }
            }
            else
            {
                ObjPCLItems_SearchAutoComplete.Clear();
            }
            NotifyOfPropertyChange(() => ObjPCLItems_SearchAutoComplete);
            //CtrcboAutoComplete.PopulateComplete();
        }

        //KMX: Sau khi kiểm tra, thấy hàm này không còn được sử dụng nữa (26/04/2014 16:56)
        private void PCLItems_SearchAutoComplete(int PageIndex, int PageSize, bool CountTotal)
        {
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var client = serviceFactory.ServiceInstance;
                    client.BeginPCLItems_SearchAutoComplete(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                     {
                         int Total = 0;
                         List<PCLExamType> allItems = null;
                         bool bOK = false;

                         try
                         {
                             allItems = client.EndPCLItems_SearchAutoComplete(out Total, asyncResult);
                             bOK = true;
                         }
                         catch (Exception innerEx)
                         {
                             ClientLoggerHelper.LogInfo(innerEx.ToString());
                         }
                         finally
                         {
                             IsLoading = false;
                         }

                         if (bOK)
                         {
                             TinhGiaChoBenhNhanBaoHiem(allItems);

                             ObjPCLItems_SearchAutoComplete.TotalItemCount = Total;

                             if (allItems != null)
                             {
                                 ObjPCLItems_SearchAutoComplete.Clear();

                                 foreach (var item in allItems)
                                 {
                                     ObjPCLItems_SearchAutoComplete.Add(item);
                                 }
                                 CtrcboAutoComplete.PopulateComplete();
                             }
                         }
                     }), null);

                }

            });

            t.Start();
        }

        int i = 0;
        public void cboAutoComplete_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (SelectedPCLExamType != null && SelectedPCLExamType.PCLExamTypeID > 0)
            {
                i++;
                if (i == 2)
                {
                    i = 0;
                    if (V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                    {
                        //▼===== 20190927 TTM: Tách sự riêng để phân biệt đâu là của hẹn bệnh đâu là của chỉ định
                        if (!IsAppointment)
                        {
                            Globals.EventAggregator.Publish(new SelectedObjectEventWithKey<PCLExamType, String> { Result = SelectedPCLExamType, Key = KeyAction });
                        }
                        else
                        {
                            Globals.EventAggregator.Publish(new SelectedObjectEventWithKeyAppt<PCLExamType, String> { Result = SelectedPCLExamType, Key = KeyAction });
                        }
                    }
                    else
                    {
                        if (!IsAppointment)
                        {
                            Globals.EventAggregator.Publish(new SelectedObjectEventWithKey_ForImage<PCLExamType, String> { Result = SelectedPCLExamType, Key = KeyAction });
                        }
                        else
                        {
                            Globals.EventAggregator.Publish(new SelectedObjectEventWithKey_ForImageAppt<PCLExamType, String> { Result = SelectedPCLExamType, Key = KeyAction });
                        }
                        //♥===== 
                    }
                    //▼====== #002
                    if (cboAutoComplete != null)
                    {
                        cboAutoComplete.Text = "";
                    }
                    //▲====== #002
                }
            }

        }
        AutoCompleteBox cboAutoComplete = null;
        public void cboAutoComplete_Load(object sender, RoutedEventArgs e)
        {
            cboAutoComplete = sender as AutoCompleteBox;
        }
        #region "CalcPatientHI"
        private void TinhGiaChoBenhNhanBaoHiem(List<PCLExamType> ObjList)
        {
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null
                && Registration_DataStorage.CurrentPatientRegistration.HisID.GetValueOrDefault(0) > 0)
            {
                foreach (var pclExamType in ObjList)
                {
                    pclExamType.NormalPrice = pclExamType.HIPatientPrice;
                }
            }
            else
            {
                foreach (var pclExamType in ObjList)
                {
                    pclExamType.HIAllowedPrice = 0;
                }
            }
        }
        #endregion
        //▼====== #001
        private long _V_PCLMainCategory;
        public long V_PCLMainCategory
        {
            get { return _V_PCLMainCategory; }
            set
            {
                if (_V_PCLMainCategory != value)
                {
                    _V_PCLMainCategory = value;
                    NotifyOfPropertyChange(() => V_PCLMainCategory);
                }
            }
        }

        public Dictionary<long, PCLExamType> _dicPCLExamTypeImage;
        public Dictionary<long, PCLExamType> dicPCLExamTypeImage
        {
            get
            {
                return _dicPCLExamTypeImage;
            }
            set
            {
                _dicPCLExamTypeImage = value;
                NotifyOfPropertyChange(() => dicPCLExamTypeImage);

            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            SetDataForAutoComplete(Globals.ListPclExamTypesAllPCLForms, Globals.ListPclExamTypesAllCombos
                , Globals.ListPclExamTypesAllPCLFormImages);
        }

        public void SetDataForAutoComplete(IList<PCLExamType> ListPclExamTypesAllPCLForms
            , IList<PCLExamTypeComboItem> ListPclExamTypesAllCombos
            , IList<PCLExamType> ListPclExamTypesAllPCLFormImages)
        {
            if (ListPclExamTypesAllPCLForms == null)
            {
                ListPclExamTypesAllPCLForms = new List<PCLExamType>();
            }
            if (ListPclExamTypesAllCombos == null)
            {
                ListPclExamTypesAllCombos = new List<PCLExamTypeComboItem>();
            }
            if (ListPclExamTypesAllPCLFormImages == null)
            {
                ListPclExamTypesAllPCLFormImages = new List<PCLExamType>();
            }
            dicPCLExamType = new Dictionary<long, PCLExamType>();
            dicPCLExamTypeImage = new Dictionary<long, PCLExamType>();
            if (V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
            {
                if (ListPclExamTypesAllPCLForms.Any(x => x.V_PCLMainCategory == V_PCLMainCategory))
                {
                    foreach (var item in ListPclExamTypesAllPCLForms.Where(x => x.V_PCLMainCategory == V_PCLMainCategory))
                    {
                        if (!dicPCLExamType.ContainsKey(item.PCLExamTypeID))
                        {
                            dicPCLExamType.Add(item.PCLExamTypeID, item);
                        }
                    }
                }
                foreach (var item in ListPclExamTypesAllCombos)
                {
                    if (!dicPCLExamType.ContainsKey(item.PCLExamTypeID))
                    {
                        dicPCLExamType.Add(item.PCLExamTypeID, item.PCLExamType);
                    }
                }
            }
            else
            {
                if (ListPclExamTypesAllPCLFormImages.Any(x => x.V_PCLMainCategory == V_PCLMainCategory))
                {
                    foreach (var item in ListPclExamTypesAllPCLFormImages.Where(x => x.V_PCLMainCategory == V_PCLMainCategory))
                    {
                        if (!dicPCLExamTypeImage.ContainsKey(item.PCLExamTypeID))
                        {
                            dicPCLExamTypeImage.Add(item.PCLExamTypeID, item);
                        }
                    }
                }
            }
        }
        public void PCLItems_SearchAutoComplete_ForImage()
        {
            if (!string.IsNullOrEmpty(SearchCriteria.PCLExamTypeName))
            {
                var resComBo = from c in dicPCLExamTypeImage
                               where (VNConvertString.ConvertString(c.Value.PCLExamTypeName).ToLower().Contains(VNConvertString.ConvertString(SearchCriteria.PCLExamTypeName).ToLower())
/*#003: Điều kiện bổ sung ở dòng này*/  || VNConvertString.ConvertString(c.Value.PCLExamTypeCode).ToLower().Contains(VNConvertString.ConvertString(SearchCriteria.PCLExamTypeName).ToLower()))
                                    && (!IsPCLBookingView || c.Value.IsCasePermitted)
                                    && (!c.Value.IsRegimenChecking || !IsRegimenChecked || CS_DS == null || CS_DS.TreatmentRegimenCollection == null
                                       || CS_DS.TreatmentRegimenCollection.Count == 0
                                       || !CS_DS.TreatmentRegimenCollection.Any(x => x.RefTreatmentRegimenPCLDetails != null && x.RefTreatmentRegimenPCLDetails.Count > 0)
                                       || !CS_DS.TreatmentRegimenCollection.Where(x => x.RefTreatmentRegimenPCLDetails != null && x.RefTreatmentRegimenPCLDetails.Count > 0).Any(x => x.RefTreatmentRegimenPCLDetails.Count > 0)
                                       || CS_DS.TreatmentRegimenCollection.Where(x => x.RefTreatmentRegimenPCLDetails != null && x.RefTreatmentRegimenPCLDetails.Count > 0).SelectMany(x => x.RefTreatmentRegimenPCLDetails).Any(x => c.Value == null || x.PCLExamTypeID == c.Value.PCLExamTypeID))
                               select c;
                List<PCLExamType> allPclType = new List<PCLExamType>();
                ObjPCLItems_SearchAutoComplete.Clear();
                foreach (var item in resComBo)
                {
                    ObjPCLItems_SearchAutoComplete.Add(item.Value);
                }
            }
            else
            {
                ObjPCLItems_SearchAutoComplete.Clear();
            }
            NotifyOfPropertyChange(() => ObjPCLItems_SearchAutoComplete);
        }

        //▲====== #001
        private bool _IsRegimenChecked = Globals.ServerConfigSection.ConsultationElements.CheckedTreatmentRegimen; //20190628 TBL: Tu dong tick theo cau hinh
        public bool IsRegimenChecked
        {
            get
            {
                return _IsRegimenChecked;
            }
            set
            {
                _IsRegimenChecked = value;
                NotifyOfPropertyChange(() => IsRegimenChecked);
            }
        }
        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
            }
        }
        private bool _IsAppointment;
        public bool IsAppointment
        {
            get { return _IsAppointment; }
            set
            {
                if (_IsAppointment != value)
                {
                    _IsAppointment = value;
                    NotifyOfPropertyChange(() => IsAppointment);
                }
            }
        }
        private bool _IsPCLBookingView = false;
        public bool IsPCLBookingView
        {
            get
            {
                return _IsPCLBookingView;
            }
            set
            {
                if (_IsPCLBookingView == value)
                {
                    return;
                }
                _IsPCLBookingView = value;
                NotifyOfPropertyChange(() => IsPCLBookingView);
            }
        }
    }
}