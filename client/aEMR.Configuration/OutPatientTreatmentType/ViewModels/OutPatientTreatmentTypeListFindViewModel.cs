using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ViewContracts;
using System.Windows.Controls;
using aEMR.Common.BaseModel;
using System.Linq;
using aEMR.Common.Utilities;
using ObjectCopier = aEMR.Common.ObjectCopier;
using aEMR.Common.ExportExcel;
/*
* #001 20180921 TNHX: Apply BusyIndicator, refactor code
* 20230622 #002 DatTB:
* + Thêm filter theo mã ICD đã lưu của nhóm bệnh
* + Thêm function chỉnh sửa ICD của nhóm bệnh
* + Thêm function xuất excel ICD của nhóm bệnh
*/
namespace aEMR.Configuration.OutPatientTreatmentType.ViewModels
{
    [Export(typeof(IOutPatientTreatmentTypeListFind)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OutPatientTreatmentTypeListFindViewModel : ViewModelBase, IOutPatientTreatmentTypeListFind
        , IHandle<SaveEvent<OutpatientTreatmentType>>
        , IHandle<SaveEvent<OutpatientTreatmentTypeICD10Link>>
    {
        protected override void OnActivate()
        {
            authorization();
            base.OnActivate();
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
        }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public OutPatientTreatmentTypeListFindViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();
            Globals.EventAggregator.Subscribe(this);

            DiseaseChapters = new ObservableCollection<DiseaseChapters>();
            DiseaseChapters_GetAll();
            OutpatientTreatmentType = new ObservableCollection<OutpatientTreatmentType>();
            OutpatientTreatmentType_GetAll();

            ObjSearchICD_Paging = new PagedSortableCollectionView<ICD>();
            ObjSearchICD_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjSearchICD_Paging_OnRefresh);
            ObjOutpatientTreatmentType_Paging = new PagedSortableCollectionView<OutpatientTreatmentType>();
            ObjOutpatientTreatmentType_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjOutpatientTreatmentType_Paging_OnRefresh);
            ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging = new PagedSortableCollectionView<OutpatientTreatmentTypeICD10Link>();
            ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging_OnRefresh);

            ObjOutpatientTreatmentType_Paging.PageIndex = 0;
            OutpatientTreatmentType_Paging(0, ObjOutpatientTreatmentType_Paging.PageSize, true);
        }

        
        void ObjOutpatientTreatmentType_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            OutpatientTreatmentType_Paging(ObjOutpatientTreatmentType_Paging.PageIndex,
                            ObjOutpatientTreatmentType_Paging.PageSize, false);
        }
        void ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            OutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging(ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging.PageIndex,
                            ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging.PageSize, false, true);
        }
        void ObjSearchICD_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchICD_Paging(ObjSearchICD_Paging.PageIndex,
                            ObjSearchICD_Paging.PageSize, false);
        }

        #region Properties
        private Visibility _hplAddNewVisible = Visibility.Visible;
        public Visibility hplAddNewVisible
        {
            get { return _hplAddNewVisible; }
            set
            {
                _hplAddNewVisible = value;
                NotifyOfPropertyChange(() => hplAddNewVisible);
            }
        }
        private ObservableCollection<OutpatientTreatmentType> _OutpatientTreatmentType;
        public ObservableCollection<OutpatientTreatmentType> OutpatientTreatmentType
        {
            get { return _OutpatientTreatmentType; }
            set
            {
                _OutpatientTreatmentType = value;
                NotifyOfPropertyChange(() => OutpatientTreatmentType);
            }
        }
        private ObservableCollection<DiseaseChapters> _DiseaseChapters;
        public ObservableCollection<DiseaseChapters> DiseaseChapters
        {
            get { return _DiseaseChapters; }
            set
            {
                _DiseaseChapters = value;
                NotifyOfPropertyChange(() => DiseaseChapters);
            }
        }
        private int _DiseaseChapters_ID = 0;
        public int DiseaseChapters_ID
        {
            get { return _DiseaseChapters_ID; }
            set
            {
                _DiseaseChapters_ID = value;
                NotifyOfPropertyChange(() => DiseaseChapters_ID);
            }
        }
        private long _OutpatientTreatmentTypeID = 0;
        public long OutpatientTreatmentTypeID
        {
            get { return _OutpatientTreatmentTypeID; }
            set
            {
                _OutpatientTreatmentTypeID = value;
                NotifyOfPropertyChange(() => OutpatientTreatmentTypeID);
            }
        }
        private ObservableCollection<Diseases> _Diseases;
        public ObservableCollection<Diseases> Diseases
        {
            get { return _Diseases; }
            set
            {
                _Diseases = value;
                NotifyOfPropertyChange(() => Diseases);
            }
        }
        private ObservableCollection<ICD> _ObjICD_ADD;
        public ObservableCollection<ICD> ObjICD_ADD
        {
            get { return _ObjICD_ADD; }
            set
            {
                _ObjICD_ADD = value;
                NotifyOfPropertyChange(() => ObjICD_ADD);
            }
        }
        
        private long _Disease_ID = 0;
        public long Disease_ID
        {
            get { return _Disease_ID; }
            set
            {
                _Disease_ID = value;
                NotifyOfPropertyChange(() => Disease_ID);
            }
        }
        private string _ICD10Code = "";
        public string ICD10Code
        {
            get { return _ICD10Code; }
            set
            {
                _ICD10Code = value;
                NotifyOfPropertyChange(() => ICD10Code);
            }
        }
     
        public Dictionary<long, ICD> _dicICD;
        public Dictionary<long, ICD> dicICD
        {
            get
            {
                return _dicICD;
            }
            set
            {
                _dicICD = value;
                NotifyOfPropertyChange(() => dicICD);

            }
        }

        private object _ICD_Current;
        public object ICD_Current
        {
            get { return _ICD_Current; }
            set
            {
                _ICD_Current = value;
                NotifyOfPropertyChange(() => ICD_Current);
            }
        }

        private ICD _selectedICD;
        public ICD SelectedICD
        {
            get { return _selectedICD; }
            set
            {
                if (_selectedICD != value)
                {
                    _selectedICD = value;
                    NotifyOfPropertyChange(() => SelectedICD);
                }
            }
        }
        private ICDSearchCriteria _SearchCriteria;
        public ICDSearchCriteria SearchCriteria
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

        private string _SearchCode;
        public string SearchCode
        {
            get
            {
                return _SearchCode;
            }
            set
            {
                _SearchCode = value;
                NotifyOfPropertyChange(() => SearchCode);
            }
        }
        private string _SearchName;
        public string SearchName
        {
            get
            {
                return _SearchName;
            }
            set
            {
                _SearchName = value;
                NotifyOfPropertyChange(() => SearchName);
            }
        }
        private bool _IsDeleted;
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                _IsDeleted = value;
                NotifyOfPropertyChange(() => IsDeleted);
            }
        }

        private PagedSortableCollectionView<ICD> _ObjSearchICD_Paging;
        public PagedSortableCollectionView<ICD> ObjSearchICD_Paging
        {
            get { return _ObjSearchICD_Paging; }
            set
            {
                _ObjSearchICD_Paging = value;
                NotifyOfPropertyChange(() => ObjSearchICD_Paging);
            }
        }
        private PagedSortableCollectionView<OutpatientTreatmentTypeICD10Link> _ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging;
        public PagedSortableCollectionView<OutpatientTreatmentTypeICD10Link> ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging
        {
            get { return _ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging; }
            set
            {
                _ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging = value;
                NotifyOfPropertyChange(() => ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging);
            }
        }

        private PagedSortableCollectionView<OutpatientTreatmentType> _ObjOutpatientTreatmentType_Paging;
        public PagedSortableCollectionView<OutpatientTreatmentType> ObjOutpatientTreatmentType_Paging
        {
            get { return _ObjOutpatientTreatmentType_Paging; }
            set
            {
                _ObjOutpatientTreatmentType_Paging = value;
                NotifyOfPropertyChange(() => ObjOutpatientTreatmentType_Paging);
            }
        }
        #endregion
       
   
       
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }

       
        #region Button
        public void btSearchICD()
        {
            if(DiseaseChapters_ID == 0)
            {
                MessageBox.Show("Chưa chọn chương!");
                return;
            }
            ObjSearchICD_Paging.PageIndex = 0;
            SearchICD_Paging(0, ObjSearchICD_Paging.PageSize, true);
        }

        public void btSearchOutpatientTreatmentType()
        {
            //ObjOutpatientTreatmentType_Paging.PageIndex = 0;
            OutpatientTreatmentType_Paging(ObjOutpatientTreatmentType_Paging.PageIndex, ObjOutpatientTreatmentType_Paging.PageSize, true);
        }
        public void hplAddNewOutPatientTreatmentType_Click()
        {
            Action<IOutPatientTreatmentTypeAddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Thêm Mới Nhóm Bệnh";
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }
        public void hplEditOutpatientTreatmentType_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<IOutPatientTreatmentTypeAddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjOutPatientTreatmentType_Current = ObjectCopier.DeepCopy((selectedItem as OutpatientTreatmentType)); 
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as OutpatientTreatmentType).OutpatientTreatmentName.Trim() + ")";
                };
                GlobalsNAV.ShowDialog<IOutPatientTreatmentTypeAddEdit>(onInitDlg);
            }
        }
        public void hplDelete_Click(object selectedItem)
        {
            if(selectedItem != null)
            {
                ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging.Remove((selectedItem as OutpatientTreatmentTypeICD10Link));
            }
        }
        public void btSaveItems()
        {
            if(!ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging.Any(x=>x.OutpatientTreatmentTypeICD10LinkID == 0))
            {
                MessageBox.Show("Không có gì để lưu", eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                return;
            }
            ICD_XMLInsert(Disease_ID, ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging.Where(x=>x.OutpatientTreatmentTypeICD10LinkID == 0).ToObservableCollection());
        }
        #endregion
        public void cboOutpatientTreatmentTypeSelectedItemChanged(object selectedItem)
        {
            OutpatientTreatmentTypeID = (selectedItem as OutpatientTreatmentType).OutpatientTreatmentTypeID;
            OutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging(0,ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging.PageSize,true);
        }
        public void cboDiseaseChaptersSelectedItemChanged(object selectedItem)
        {
            DiseaseChapters_ID = (selectedItem as DiseaseChapters).DiseaseChapterID;
            Disease_ID = 0;
            Diseases_ByChapterID();
            SearchICD_Paging(0, ObjSearchICD_Paging.PageSize, true);
        }
        public void cboDiseasesSelectedItemChanged(object selectedItem)
        {
            if(selectedItem!= null)
            {
                Disease_ID = (selectedItem as Diseases).DiseaseID;
            }
            SearchICD_Paging(0, ObjSearchICD_Paging.PageSize, true);
        }
        public void Diseases_ByChapterID()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Loại Phòng..." });
            this.ShowBusyIndicator("Danh sách ICD");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDiseases_ByChapterID(DiseaseChapters_ID, Globals.DispatchCallback((asyncResult) =>
                         {
                             try
                             {
                                 var items = contract.EndDiseases_ByChapterID(asyncResult);
                                 if (items != null)
                                 {
                                     Diseases = new ObservableCollection<Diseases>(items);
                                     Diseases Default = new Diseases();
                                     Default.DiseaseID = 0;
                                     Default.DiseaseNameVN = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                     Diseases.Insert(0, Default);
                                 }
                                 else
                                 {
                                     Diseases = new ObservableCollection<Diseases>();
                                 }
                             }
                             catch (Exception ex)
                             {
                                 Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                             }
                             finally
                             {
                                 this.HideBusyIndicator();
                             }
                         }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void OutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging(int PageIndex, int PageSize, bool CountTotal, bool IsRefresh = false)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging(OutpatientTreatmentTypeID, ICD10CodeSaved, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<OutpatientTreatmentTypeICD10Link> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging(out Total, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                            IList<OutpatientTreatmentTypeICD10Link> temp = null;
                            if (IsRefresh && ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging.Any(x => x.OutpatientTreatmentTypeICD10LinkID == 0))
                            {
                                temp = ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging.Where(x => x.OutpatientTreatmentTypeICD10LinkID == 0).ToList();
                            }
                            ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        //▼==== #002
                                        item.CanEdit = true;
                                        //▲==== #002
                                        ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging.Add(item);
                                    }
                                }
                                if(temp != null && temp.Count> 0)
                                {
                                    foreach (var item in temp)
                                    {
                                        //▼==== #002
                                        item.CanEdit = true;
                                        //▲==== #002
                                        ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging.Add(item);
                                    }
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            ICD_Current = eventArgs.Value as ICD;
            Globals.EventAggregator.Publish(new dgICDListClickSelectionChanged_Event() { ICD_Current = eventArgs.Value });
        }

        public void btAddChoose()
        {
            if (!ObjSearchICD_Paging.Any(x=>x.IsChecked))
            {
                MessageBox.Show("Chưa chọn ICD nào");
            }
            
            foreach (var item in ObjSearchICD_Paging.Where(x => x.IsChecked))
            {
                if (!CheckExistsICD(item))
                {
                    ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging.Add(new OutpatientTreatmentTypeICD10Link
                    {
                        OutpatientTreatmentTypeID = OutpatientTreatmentTypeID,
                        IDCode = item.IDCode,
                        ICD10 = item.ICD10Code,
                        DiseaseNameVN = item.DiseaseNameVN,
                        IsActive = item.IsActive,
                        CanDelete = true
                    });
                }
            }
        }
        private bool CheckExistsICD(ICD iCD)
        {
            foreach (var item in ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging)
            {
                if(item.IDCode == iCD.IDCode)
                {
                    return true;
                }
            }
            return false;
        }
        private void SearchICD_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchICD_Paging((long)DiseaseChapters_ID, Disease_ID, ICD10Code, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<ICD> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSearchICD_Paging(out Total, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }

                            ObjSearchICD_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjSearchICD_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjSearchICD_Paging.Add(item);
                                    }
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void OutpatientTreatmentType_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.DlgShowBusyIndicator("Danh sách nhóm bệnh");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetOutpatientTreatmentType_Paging(SearchCode,SearchName,IsDeleted, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<OutpatientTreatmentType> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetOutpatientTreatmentType_Paging(out Total, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }

                            ObjOutpatientTreatmentType_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjOutpatientTreatmentType_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjOutpatientTreatmentType_Paging.Add(item);
                                    }
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }
        public void DiseaseChapters_GetAll()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDiseaseChapters_GetAll(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndDiseaseChapters_GetAll(asyncResult);
                                if (items != null)
                                {
                                    DiseaseChapters = new ObservableCollection<DiseaseChapters>(items);
                                    //ItemDefault
                                    DiseaseChapters Default = new DiseaseChapters();
                                    Default.DiseaseChapterID = -1;
                                    Default.DiseaseChapterNameVN = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    DiseaseChapters.Insert(0, Default);
                                    //ItemDefault
                                }
                                else
                                {
                                    DiseaseChapters = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void OutpatientTreatmentType_GetAll()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginOutpatientTreatmentType_GetAll(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndOutpatientTreatmentType_GetAll(asyncResult);
                                if (items != null)
                                {
                                    OutpatientTreatmentType = new ObservableCollection<OutpatientTreatmentType>(items);
                                }
                                else
                                {
                                    OutpatientTreatmentType = new ObservableCollection<OutpatientTreatmentType>();
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void ICD_XMLInsert(Int64 Disease_ID, ObservableCollection<OutpatientTreatmentTypeICD10Link> objCollect)
        {
            bool Result = false;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });
            this.ShowBusyIndicator(eHCMSResources.Z0172_G1_DangLuuDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginOutpatientTreatmentTypeICD10Link_XMLInsert(objCollect, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Result = contract.EndOutpatientTreatmentTypeICD10Link_XMLInsert(asyncResult);
                                if (Result)
                                {
                                    ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging.PageIndex = 0;
                                    OutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging(0,
                                        ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging.PageSize, true);
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
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        #region Handle
        public void Handle(SaveEvent<OutpatientTreatmentType> message)
        {
     
            OutpatientTreatmentType_Paging(ObjOutpatientTreatmentType_Paging.PageIndex, ObjOutpatientTreatmentType_Paging.PageSize, true);
        }
        #endregion
               
        //▼==== #002
        private string _ICD10CodeSaved = "";
        public string ICD10CodeSaved
        {
            get { return _ICD10CodeSaved; }
            set
            {
                _ICD10CodeSaved = value;
                NotifyOfPropertyChange(() => ICD10CodeSaved);
            }
        }

        public void btSearchICDInOutPatientTreatmentType()
        {
            if (OutpatientTreatmentTypeID == 0)
            {
                MessageBox.Show("Chưa chọn nhóm bệnh!");
                return;
            }
            OutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging(0, ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging.PageSize, true);
        }

        public void hplEditICD10Link_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<IPatientTreatmentTypeICD10LinkEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjICD10Link_Current = ObjectCopier.DeepCopy((selectedItem as OutpatientTreatmentTypeICD10Link));
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as OutpatientTreatmentTypeICD10Link).DiseaseNameVN.Trim() + ")";
                };
                GlobalsNAV.ShowDialog<IPatientTreatmentTypeICD10LinkEdit>(onInitDlg);
            }
        }

        public void Handle(SaveEvent<OutpatientTreatmentTypeICD10Link> message)
        {
            OutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging(0, ObjOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging.PageSize, true);
        }

        public void BtnExportExcel()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0669_G1_DangLayDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        ConfigurationReportParams Params = new ConfigurationReportParams()
                        {
                            ConfigurationName = ConfigurationName.OutpatientTreatmentTypeICD10Link,
                            OutpatientTreatmentTypeID = OutpatientTreatmentTypeID
                        };

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExportExcelConfigurationManager(Params, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var results = contract.EndExportExcelConfigurationManager(asyncResult);
                                ExportToExcelFileAllData.Export(results, "Shee1");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //▲==== #002
    }
}
