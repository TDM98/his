using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using System.Windows.Controls;
using System.Windows.Media;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(ITransferFormList)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class TransferFormListViewModel: Conductor<object>, ITransferFormList
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public TransferFormListViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);

            var hospitalAutoCompleteVm1 = Globals.GetViewModel<IHospitalAutoCompleteListing>();
            hospitalAutoCompleteVm1.IsPaperReferal = true;
            FromHospitalAutoCnt = hospitalAutoCompleteVm1;
            FromHospitalAutoCnt.InitBlankBindingValue();
            Globals.EventAggregator.Subscribe(this);

            LoadCriterialTypes();
            FromDate = DateTime.Now.Date;
            ToDate = DateTime.Now.Date;
        }

        private IPaperReferalFull _Parent;
        public IPaperReferalFull ParentReferralPaper
        {
            get
            {
                return _Parent;
            }
            set
            {
                _Parent = value;
            }
        }

        private ObservableCollection<TransferForm> _TransferFormList;
        public ObservableCollection<TransferForm> TransferFormList
        {
            get
            {
                return _TransferFormList;
            }
            set
            {
                _TransferFormList = value;
               
                NotifyOfPropertyChange(() => TransferFormList);
            }
        }

        private TransferForm _SelectedTransferForm;
        public TransferForm SelectedTransferForm
        {
            get
            {
                return _SelectedTransferForm;
            }
            set
            {
                _SelectedTransferForm = value;
                NotifyOfPropertyChange(() => SelectedTransferForm);
            }

        }
        
        private LeftModuleActive _leftModule;
        public LeftModuleActive LeftModule
        {
            get { return _leftModule; }
            set
            {
                _leftModule = value;
                NotifyOfPropertyChange(() => LeftModule);
            }
        }

        public void PrintCmd()
        {
            //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
            //proAlloc.eItem = ReportName.REGISTRATIOBLIST;

            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
            {
                proAlloc.eItem = ReportName.REGISTRATIOBLIST;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }
     
        public void gridRegistrations_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            PatientRegistration objRows = e.Row.DataContext as PatientRegistration;
            if (objRows != null)
            {
                switch (objRows.InPtAdmissionStatus)
                {
                    case 0:                        
                        e.Row.Foreground = new SolidColorBrush(Colors.Red);
                        break;                        
                    case 1:
                    case 6:
                    case 7:
                        e.Row.Foreground = new SolidColorBrush(Colors.Black);
                        break;                        
                    case 2:                        
                        e.Row.Foreground = new SolidColorBrush(Colors.Black);
                        break;
                    case 3:
                        e.Row.Foreground = new SolidColorBrush(Colors.Green);
                        break;
                    case 4:
                        e.Row.Foreground = new SolidColorBrush(Colors.Red);
                        break;
                    case 5:
                        e.Row.Foreground = new SolidColorBrush(Colors.Purple);
                        break;
                    default:
                        e.Row.Foreground = new SolidColorBrush(Colors.Red);
                        break;                                                
                }
            }
        }

        public void btnSearch_Click()
        {
            if (IsUsedAdvFilterTool)
            {
                if (FindBy == null || FindBy.LookupID <= 0)
                {
                    MessageBox.Show("Chọn điều kiện tìm kiếm!");
                    return;
                }
                if (FindBy.LookupID == (long)AllLookupValues.CriterialTypes.BV_CHUYEN)
                {
                    if (IsNotFromHos)
                    {
                        TextCriterial = "0";
                    }
                    else if (IsFromHos)
                    {
                        if (FromHospitalAutoCnt.SelectedHospital == null || FromHospitalAutoCnt.SelectedHospital.HosID <= 0)
                        {
                            MessageBox.Show("Vui lòng chọn bệnh viện!");
                            return;
                        }
                        TextCriterial = FromHospitalAutoCnt.SelectedHospital.HosID.ToString();
                    }
                }
                if (string.IsNullOrEmpty(TextCriterial) || string.IsNullOrWhiteSpace(TextCriterial))
                {
                    MessageBox.Show("Vui lòng nhập nội dung tìm kiếm!");
                    return;
                }
            }
            int PatientFindBy = IsAllPatientType ? (int)AllLookupValues.PatientFindBy.CAHAI : (IsOutPatient ? (int)AllLookupValues.PatientFindBy.NGOAITRU : (int)AllLookupValues.PatientFindBy.NOITRU);
            long FilterBy = IsUsedAdvFilterTool && FindBy != null ? FindBy.LookupID : 0;
            SearchTransferForm(FilterBy, PatientFindBy);
        }

        private int _V_TransferFormType;
        public int V_TransferFormType
        {
            get
            {
                return _V_TransferFormType;
            }
            set
            {
                _V_TransferFormType = value;
                NotifyOfPropertyChange(() => V_TransferFormType);
            }
        }

        private void SearchTransferForm(object FindBy, object PatientFindBy)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetTransferForm_Date(string.IsNullOrEmpty(TextCriterial) ? "" : TextCriterial.ToString(),
                            (long)FindBy, (int)V_TransferFormType, (int)PatientFindBy, null,Convert.ToDateTime(FromDate),Convert.ToDateTime(ToDate),  /*TMA*/
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                TransferFormList = new ObservableCollection<TransferForm>();
                                IList<TransferForm> Result = contract.EndGetTransferForm_Date(asyncResult);
                                if (Result == null || Result.Count <= 0)
                                {
                                    MessageBox.Show("Không tìm thấy giấy chuyển tuyến theo điều kiện đã cho!");
                                }
                                else
                                {
                                    TransferFormList = Result.ToObservableCollection();
                                }                               
                                TextCriterial = "";
                                this.HideBusyIndicator();
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.Message);
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private ObservableCollection<Lookup> _FindByList;
        public ObservableCollection<Lookup> FindByList
        {
            get
            {
                return _FindByList;
            }
            set
            {
                _FindByList = value;
                NotifyOfPropertyChange(() => FindByList);
            }
        }

        public bool IsShowHospitalContent
        {
            get
            {
                return (FindBy != null && FindBy.LookupID == (long)AllLookupValues.CriterialTypes.BV_CHUYEN);
            }
        }

        private Lookup _FindBy;
        public Lookup FindBy
        {
            get
            {
                return _FindBy;
            }
            set
            {
                _FindBy = value;
                TextCriterial = "";
                FromHospitalAutoCnt.SelectedHospital = null;
                NotifyOfPropertyChange(() => FindBy);
                NotifyOfPropertyChange(() => IsShowHospitalContent);
            }
        }

        private bool _IsNotFromHos;
        public bool IsNotFromHos
        {
            get
            {
                return _IsNotFromHos;
            }
            set
            {
                _IsNotFromHos = value;
                NotifyOfPropertyChange(() => IsNotFromHos);
            }
        }

        private bool _IsFromHos;
        public bool IsFromHos
        {
            get
            {
                return _IsFromHos;
            }
            set
            {
                _IsFromHos = value;
                NotifyOfPropertyChange(() => IsFromHos);
            }
        }

        private string _TextCriterial;
        public string TextCriterial
        {
            get
            {
                return _TextCriterial;
            }
            set
            {
                _TextCriterial = value;
                NotifyOfPropertyChange(() => TextCriterial);
            }
        }

        private bool _IsAllPatientType = true;
        public bool IsAllPatientType
        {
            get
            {
                return _IsAllPatientType;
            }
            set
            {
                _IsAllPatientType = value;
                NotifyOfPropertyChange(() => IsAllPatientType);
            }
        }

        private bool _IsInPatient;
        public bool IsInPatient
        {
            get
            {
                return _IsInPatient;
            }
            set
            {
                _IsInPatient = value;
                NotifyOfPropertyChange(() => IsInPatient);
            }
        }

        private bool _IsOutPatient;
        public bool IsOutPatient
        {
            get
            {
                return _IsOutPatient;
            }
            set
            {
                _IsOutPatient = value;
                NotifyOfPropertyChange(() => IsOutPatient);
            }
        }

        private bool _IsAllType = true;
        public bool IsAllType
        {
            get
            {
                return _IsAllType;
            }
            set
            {
                _IsAllType = value;
                NotifyOfPropertyChange(() => IsAllType);
            }
        }

        private bool _IsTransferTo;
        public bool IsTransferTo
        {
            get
            {
                return _IsTransferTo;
            }
            set
            {
                _IsTransferTo = value;
                NotifyOfPropertyChange(() => IsTransferTo);
            }
        }

        private bool _IsTransferToPCL;
        public bool IsTransferToPCL
        {
            get
            {
                return _IsTransferToPCL;
            }
            set
            {
                _IsTransferToPCL = value;
                NotifyOfPropertyChange(() => IsTransferToPCL);
            }
        }

        private bool _IsTransferFrom;
        public bool IsTransferFrom
        {
            get
            {
                return _IsTransferFrom;
            }
            set
            {
                _IsTransferFrom = value;
                NotifyOfPropertyChange(() => IsTransferFrom);
            }
        }

        private bool _IsUsedAdvFilterTool;
        public bool IsUsedAdvFilterTool
        {
            get
            {
                return _IsUsedAdvFilterTool;
            }
            set
            {
                _IsUsedAdvFilterTool = value;
                FindBy = new Lookup();                
                NotifyOfPropertyChange(() => IsUsedAdvFilterTool);
                NotifyOfPropertyChange(() => FindBy);
            }
        }

        private IHospitalAutoCompleteListing _FromHospitalAutoCnt;
        public IHospitalAutoCompleteListing FromHospitalAutoCnt
        {
            get { return _FromHospitalAutoCnt; }
            set
            {
                _FromHospitalAutoCnt = value;
                NotifyOfPropertyChange(() => FromHospitalAutoCnt);
            }
        }

        public void LoadCriterialTypes()
        {
            FindByList = new ObservableCollection<Lookup> 
            {
                new Lookup
                {
                    LookupID = (int)AllLookupValues.CriterialTypes.MA_BN,
                    ObjectValue = GetDescription(AllLookupValues.CriterialTypes.MA_BN)
                } ,
                new Lookup
                {
                    LookupID = (int)AllLookupValues.CriterialTypes.TEN_BN,
                    ObjectValue = GetDescription(AllLookupValues.CriterialTypes.TEN_BN)
                } ,
                new Lookup
                {
                    LookupID = (int)AllLookupValues.CriterialTypes.MA_CHUYEN_TUYEN,
                    ObjectValue = GetDescription(AllLookupValues.CriterialTypes.MA_CHUYEN_TUYEN)
                },
                 new Lookup
                {
                    LookupID = (int)AllLookupValues.CriterialTypes.BV_CHUYEN,
                    ObjectValue = GetDescription(AllLookupValues.CriterialTypes.BV_CHUYEN)
                }
            };
        }
        public string GetDescription(Enum value)
        {
            System.Reflection.FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute
                    = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                        as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public void grdDblClick(object sender, EventArgs<object> eventArgs)
        {
            if (ParentReferralPaper != null && ParentReferralPaper is IPaperReferalFull)
            {
                TransferForm item = eventArgs.Value as TransferForm;
                ParentReferralPaper.SetCurrentTransferForm(item);
                this.TryClose();
            }
        }

        /*TMA 20/11/2017*/
        private DateTime _FromDate;
        public DateTime FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }

        private DateTime _ToDate;
        public DateTime ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }
    }
}
