using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
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
using eHCMS.Services.Core;
using System.Collections.Generic;
using aEMR.Common;
using System.Linq;
using aEMR.Controls;
using System.Windows.Controls;

namespace aEMR.Configuration.ICDList.ViewModels
{
    [Export(typeof(IICD_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ICD_AddEditViewModel : Conductor<object>, IICD_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ICD_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            DiseaseChapters = new ObservableCollection<DiseaseChapters>();
            DiseaseChapters_GetAll();
            LoadGenders();
            AfterRefIDC10Code = new ObservableCollection<DiseasesReference>();
            SelectedAfterICD10 = new DiseasesReference();
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

        private DiseaseChapters _ObjDiseaseChapters_Current;
        public DiseaseChapters ObjDiseaseChapters_Current
        {
            get { return _ObjDiseaseChapters_Current; }
            set
            {
                _ObjDiseaseChapters_Current = value;
                NotifyOfPropertyChange(() => ObjDiseaseChapters_Current);
            }
        }
        private Diseases _ObjDiseases_Current;
        public Diseases ObjDiseases_Current
        {
            get { return _ObjDiseases_Current; }
            set
            {
                _ObjDiseases_Current = value;
                NotifyOfPropertyChange(() => ObjDiseases_Current);
            }
        }
        private ICD _ObjICD_Current;
        public ICD ObjICD_Current
        {
            get { return _ObjICD_Current; }
            set
            {
                _ObjICD_Current = value;
                if(_ObjICD_Current != null && _ObjICD_Current.Gender == null)
                {
                    _ObjICD_Current.Gender = new Gender("U", "Không xác định");
                }
                if (_ObjICD_Current.IsICD10CodeYHCT)
                {
                    LoadRefDiseases(_ObjICD_Current.ICD10CodeFromYHCT.Trim(), 0, 0, 100, false);
                    //SelectedAfterICD10 = new DiseasesReference { ICD10Code = _ObjICD_Current.ICD10CodeFromYHCT };
                }
                NotifyOfPropertyChange(() => ObjICD_Current);
            }
        }
        private ObservableCollection<Gender> _genders;
        public ObservableCollection<Gender> Genders
        {
            get { return _genders; }
            set
            {
                _genders = value;
                NotifyOfPropertyChange(() => Genders);
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

        private long _TypeForm;
        public long TypeForm
        {
            get { return _TypeForm; }
            set
            {
                _TypeForm = value;
                NotifyOfPropertyChange(() => TypeForm);
                CheckTypeForm();
            }
        }
        private Visibility _ChapterVisible = Visibility.Collapsed;
        public Visibility ChapterVisible
        {
            get { return _ChapterVisible; }
            set
            {
                _ChapterVisible = value;
                NotifyOfPropertyChange(() => ChapterVisible);
            }
        }
        private Visibility _DiseasesVisible = Visibility.Collapsed;
        public Visibility DiseasesVisible
        {
            get { return _DiseasesVisible; }
            set
            {
                _DiseasesVisible = value;
                NotifyOfPropertyChange(() => DiseasesVisible);
            }
        }
        private Visibility _ICDVisible = Visibility.Collapsed;
        public Visibility ICDVisible
        {
            get { return _ICDVisible; }
            set
            {
                _ICDVisible = value;
                NotifyOfPropertyChange(() => ICDVisible);
            }
        }
        public AxTextBoxFilter.TextBoxFilterType IntNumberFilter
        {
            get
            {
                return AxTextBoxFilter.TextBoxFilterType.Integer;
            }
        }
        private void CheckTypeForm()
        {
            switch (TypeForm)
            {
                case 1:
                    ChapterVisible = Visibility.Visible;
                    break;
                case 2:
                    DiseasesVisible = Visibility.Visible;
                    break;
                case 3:
                    ICDVisible = Visibility.Visible;
                    break;
            }
        }
        public void InitializeNewItem()
        {
            ObjICD_Current = new ICD();
            ObjDiseases_Current = new Diseases();
            ObjDiseaseChapters_Current = new DiseaseChapters();
        }

        public void btSave()
        {
            switch (TypeForm)
            {
                case 1:
                    ChapterSave();
                    break;
                case 2:
                    DiseasesSave();
                    break;
                case 3:
                    ICDSave();
                    break;
            }

        }
        private void DiseasesSave()
        {
            if (ObjDiseases_Current.DiseaseNameVN != "" && ObjDiseases_Current.ICDXCode != "")
            {
                if (CheckValidDiseases(ObjDiseases_Current))
                {
                    Diseases_InsertUpdate(ObjDiseases_Current, true);
                }
            }
            else
            {
                MessageBox.Show("Nhập đầy đủ thông tin tên nhóm và ICD!", eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
            }
        }
        private void ChapterSave()
        {
            if (ObjDiseaseChapters_Current.DiseaseChapterNameVN != "" && ObjDiseaseChapters_Current.ICDXCode != "")
            {
                if (CheckValidChapter(ObjDiseaseChapters_Current))
                {
                    Chapter_InsertUpdate(ObjDiseaseChapters_Current, true);
                }
            }
            else
            {
                MessageBox.Show("Nhập đầy đủ thông tin tên chương và ICD!", eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
            }
        }
        private void ICDSave()
        {
            if (ObjICD_Current.ICD10Code != "" && ObjICD_Current.DiseaseNameVN != "")
            {
                if (CheckValidICD(ObjICD_Current))
                {
                    ICD_InsertUpdate(ObjICD_Current, true);
                }
            }
            else
            {
                MessageBox.Show("Nhập đầy đủ thông tin mã ICD và chẩn đoán!", eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
            }
        }
        public bool CheckValidDiseases(object temp)
        {
            Diseases p = temp as Diseases;
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }
        public bool CheckValidChapter(object temp)
        {
            DiseaseChapters p = temp as DiseaseChapters;
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }
        public bool CheckValidICD(object temp)
        {
            ICD p = temp as ICD;
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
        public void LoadGenders()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllGenders(
                            Globals.DispatchCallback(asyncResult =>
                            {
                                IList<Gender> allItems = null;
                                try
                                {
                                    allItems = contract.EndGetAllGenders(asyncResult);
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    MessageBox.Show(eHCMSResources.A0692_G1_Msg_InfoKhTheLayDSGioiTinh);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                                if (allItems != null)
                                {
                                    Genders = new ObservableCollection<Gender>();
                                    Genders.Add(new Gender ("U","Không xác định"));
                                    foreach(var item in allItems)
                                    {
                                        Genders.Add(item);
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
        private void Chapter_InsertUpdate(DiseaseChapters Obj, bool SaveToDB)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    Obj.DateModified = Convert.ToDateTime(Globals.ServerDate);
                    Obj.ModifiedLog = Globals.LoggedUserAccount.StaffID.ToString();

                    contract.BeginChapter_Save(Obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndChapter_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Duplex-Name":
                                    {
                                        MessageBox.Show(string.Format("{0} {1}!", "Tên chương đã tồn tại", eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new Chapter_Event_Save() { Result = true });
                                        TitleForm = string.Format("{0} ({1})", eHCMSResources.T1484_G1_HChinh, ObjDiseaseChapters_Current.DiseaseChapterNameVN.Trim());
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        TryClose();
                                        break;
                                    }
                                case "Insert-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-1":
                                    {
                                        Globals.EventAggregator.Publish(new Chapter_Event_Save() { Result = true });
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        TryClose();
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
        private void Diseases_InsertUpdate(Diseases Obj, bool SaveToDB)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    Obj.DateModified = Convert.ToDateTime(Globals.ServerDate);
                    Obj.ModifiedLog = Globals.LoggedUserAccount.StaffID.ToString();

                    contract.BeginDiseases_Save(Obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndDiseases_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Duplex-Name":
                                    {
                                        MessageBox.Show(string.Format("{0} {1}!", "Tên nhóm đã tồn tại", eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new Diseases_Event_Save() { Result = true });
                                        TitleForm = string.Format("{0} ({1})", eHCMSResources.T1484_G1_HChinh, ObjDiseases_Current.DiseaseNameVN.Trim());
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        TryClose();
                                        break;
                                    }
                                case "Insert-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-1":
                                    {
                                        Globals.EventAggregator.Publish(new Diseases_Event_Save() { Result = true });
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        TryClose();
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
        private void ICD_InsertUpdate(ICD Obj, bool SaveToDB)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    Obj.DateModified = Convert.ToDateTime(Globals.ServerDate);
                    Obj.ModifiedLog = Globals.LoggedUserAccount.StaffID.ToString();

                    contract.BeginICD_Save(Obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndICD_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Duplex-Code":
                                    {
                                        MessageBox.Show(string.Format("{0} {1}!", "Mã ICD đã tồn tại.", "Vui lòng dùng mã khác!!"), eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Duplex-Name":
                                    {
                                        MessageBox.Show(string.Format("{0} {1}!", "ICD đã tồn tại.", eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new ICD_Event_Save() { Result = true });
                                        TitleForm = string.Format("{0} ({1})", eHCMSResources.T1484_G1_HChinh, ObjICD_Current.DiseaseNameVN.Trim());
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        TryClose();
                                        break;
                                    }
                                case "Insert-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-1":
                                    {
                                        Globals.EventAggregator.Publish(new ICD_Event_Save() { Result = true });
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        TryClose();
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
        public void DiseaseChapters_GetAll()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Loại Phòng..." });
            this.ShowBusyIndicator(eHCMSResources.K2993_G1_DSLgoaiPg);
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
        AutoCompleteBox AfterAcb_ICD10_Code = null;
        private DiseasesReference _SelectedAfterICD10;
        public DiseasesReference SelectedAfterICD10
        {
            get
            {
                return _SelectedAfterICD10;
            }
            set
            {
                if (_SelectedAfterICD10 != value)
                {
                    _SelectedAfterICD10 = value;
                }
                NotifyOfPropertyChange(() => SelectedAfterICD10);
            }
        }
        private ObservableCollection<DiseasesReference> _AfterRefIDC10Code;
        public ObservableCollection<DiseasesReference> AfterRefIDC10Code
        {
            get
            {
                return _AfterRefIDC10Code;
            }
            set
            {
                if (_AfterRefIDC10Code != value)
                {
                    _AfterRefIDC10Code = value;
                }
                NotifyOfPropertyChange(() => AfterRefIDC10Code);
            }
        }
        public void AfterAcbICD10Code_Loaded(object sender, RoutedEventArgs e)
        {
            AfterAcb_ICD10_Code = (AutoCompleteBox)sender;
        }
        public void AfterAucICD10_Populating(object sender, PopulatingEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Parameter))
            {
                LoadRefAfterDiseases(e.Parameter, 0, 0, 100);
            }
        }
        private bool AfterIsDropDown = false;
        public void AfterAxAutoComplete_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AfterIsDropDown = true;
        }
        public void AfterAxAutoComplete_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!AfterIsDropDown)
            {
                return;
            }
            AfterIsDropDown = false;
            if (AfterAcb_ICD10_Code != null)
            {
                if (AfterAcb_ICD10_Code.SelectedItem is DiseasesReference BDiagTreatment)
                {
                    //AfterDiagTreatment = BDiagTreatment.DiseaseNameVN;
                    SelectedAfterICD10 = BDiagTreatment;
                    //SmallProcedureObj.AfterICD10 = BDiagTreatment;
                    ObjICD_Current.ICD10CodeFromYHCT = SelectedAfterICD10.ICD10Code;
                }
            }
        }
        public void LoadRefAfterDiseases(string name, byte type, int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchRefDiseases(name, PageIndex, PageSize, type, 0, Globals.GetCurServerDateTime()
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndSearchRefDiseases(out int Total, asyncResult);
                                AfterRefIDC10Code.Clear();
                                if (results != null)
                                {
                                    foreach (DiseasesReference p in results)
                                    {
                                        AfterRefIDC10Code.Add(p);
                                    }
                                }
                                AfterAcb_ICD10_Code.ItemsSource = AfterRefIDC10Code;
                                AfterAcb_ICD10_Code.PopulateComplete();
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                            }
                        }), null);
                }
            });

            t.Start();
        }
        public void LoadRefDiseases(string name, byte type, int PageIndex, int PageSize, bool isBefore)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchRefDiseases(name, PageIndex, PageSize, type
                        , 0
                        , Globals.GetCurServerDateTime()
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndSearchRefDiseases(out int Total, asyncResult);
                                AfterRefIDC10Code.Clear();
                                if (results != null && results.Count() > 0)
                                {
                                    if (isBefore)
                                    {
                                        //SelectedBeforeICD10 = results[0];
                                        //BeforeDiagTreatment = SelectedBeforeICD10.DiseaseNameVN;
                                    }
                                    else
                                    {
                                        SelectedAfterICD10 = results[0];
                                        //AfterDiagTreatment = SelectedAfterICD10.DiseaseNameVN;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                            }
                        }), null);
                }
            });

            t.Start();
        }
    }
}
