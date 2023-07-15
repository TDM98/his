using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using DataEntities;
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure;
using aEMR.ViewContracts.Configuration;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(ICommonPCLExamTarget)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTargetViewModel : Conductor<object>, ICommonPCLExamTarget
        , IHandle<DbClickSelectedObjectEvent<PCLExamType>>
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public PCLExamTargetViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _eventArg = eventArg;
        }

        //==== 20161115 CMN Begin: Fix Choose poppup handle
        protected override void OnActivate()
        {
            base.OnActivate();
            _eventArg.Subscribe(this);
        }
        //==== 20161115 End.

        private Nullable<DateTime> _FromDate = Globals.ServerDate.Value;
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

        private Nullable<DateTime> _ToDate = Globals.ServerDate.Value;
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

        private PCLExamType _selectedPCLExamType;
        public PCLExamType SelectedPCLExamType
        {
            get { return _selectedPCLExamType; }
            set
            {
                _selectedPCLExamType = value;
                NotifyOfPropertyChange(() => SelectedPCLExamType);
            }
        }

        private bool? _IsAppointment = true;
        public bool? IsAppointment
        {
            get { return _IsAppointment; }
            set
            {
                _IsAppointment = value;
                NotifyOfPropertyChange(() => IsAppointment);
            }
        }

        public void RdtRegistration_Checked(RoutedEventArgs e)
        {
            IsAppointment = false;
        }

        public void RdtAppointment_Checked(RoutedEventArgs e)
        {
            IsAppointment = true;
        }

        public void btView()
        {
            if (CheckDateValid())
            {
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.TieuDeRpt = string.Format("{0} ", eHCMSResources.Z1134_G1_DSCLS.ToUpper()) + SelectedPCLExamType.PCLExamTypeName;
                    proAlloc.ID = SelectedPCLExamType.PCLExamTypeID;
                    proAlloc.FromDate = FromDate;
                    proAlloc.ToDate = ToDate;
                    proAlloc.IsAppointment = IsAppointment;

                    proAlloc.eItem = ReportName.PCLExamTypeTarget;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
        }

        private bool CheckDateValid()
        {
            if (FromDate != null && ToDate != null)
            {
                DateTime F = Globals.ServerDate.Value;
                DateTime.TryParse(FromDate.ToString(), out F);

                DateTime T = Globals.ServerDate.Value;
                DateTime.TryParse(ToDate.ToString(), out T);

                if (F > T)
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0467_G1_TuNgNhoHonBangDenNg), eHCMSResources.G0981_G1_Tim, MessageBoxButton.OK);
                    return false;
                }
            }
            else
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0884_G1_Msg_InfoNhapTuNgDenNg));
                return false;
            }

            if (SelectedPCLExamType == null || SelectedPCLExamType.PCLExamTypeID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0298_G1_ChonCLS);
                return false;
            }
            return true;
        }

        public void ChooseExamTypeCmd()
        {
            Action<IPCLExamTypes_List_Paging> onInitDlg = delegate (IPCLExamTypes_List_Paging UCPCLExamTypes)
            {
                UCPCLExamTypes.IsChildWindow = true;
                UCPCLExamTypes.SearchCriteria = new PCLExamTypeSearchCriteria();
                UCPCLExamTypes.SearchCriteria.IsNotInPCLExamTypeLocations = true;
                UCPCLExamTypes.SearchCriteria.IsNotInPCLItems = true;
                UCPCLExamTypes.IsNotInPCLItemsVisibility = Visibility.Collapsed;
                UCPCLExamTypes.FormLoad();
            };
            GlobalsNAV.ShowDialog<IPCLExamTypes_List_Paging>(onInitDlg);
        }

        public void Handle(DbClickSelectedObjectEvent<PCLExamType> message)
        {
            if (this.IsActive && message != null)
            {
                SelectedPCLExamType = message.Result;
            }
        }
    }
}