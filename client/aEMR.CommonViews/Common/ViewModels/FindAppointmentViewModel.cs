using System.Collections.Generic;
using System.Windows.Input;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System;
using System.Windows;
using eHCMSLanguage;
using aEMR.Controls;
using System.Collections.ObjectModel;
using System.Threading;
using aEMR.ServiceClient;
/*
* 20180904 #001 TTM: Ngăn không cho tìm kiếm rỗng
* 20190816 #002 TBL: Thêm 1 nút tìm những cuộc hẹn thuộc đối tượng khám sức khỏe, lọc đối tượng KSK theo tên công ty
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IFindAppointment)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class FindAppointmentViewModel : ViewModelBase, IFindAppointment
        , IHandle<DoubleClick>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        public const string HICARD_REG_EXP = @"^(DN|HX|CH|NN|TK|HC|XK|CA|HT|TB|MS|XB|XN|TN|CC|CK|CB|KC|HD|BT|HN|TC|TQ|TA|TY|TE|HG|LS|CN|HS|GD|TL|XV|NO)([1-7])(\d{2})(\d{2})(\d{3})(\d{5})$";
        public const string PATIENT_CODE_REG_EXP = "^[0-9]{8}$";

        protected override void OnActivate()
        {
            base.OnActivate();

            _criteria = _searchCriteria;
            Appointments.PageIndex = 0;
            //SearchAppointments(0, Appointments.PageSize, true);
            //==== 20161115 CMN Begin: Fix Choose poppup handle
            Globals.EventAggregator.Subscribe(this);
            //==== 20161115 CMN End.
        }
        [ImportingConstructor]
        public FindAppointmentViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventArg.Subscribe(this);

            SearchCriteria = new AppointmentSearchCriteria();
            //KMx: Mặc định không có DateFrom và DateTo (12/04/2014 14:26).
            //SearchCriteria.DateFrom = Globals.ServerDate.Value.Date;
            //SearchCriteria.DateTo = Globals.ServerDate.Value.Date;

            Appointments = new PagedSortableCollectionView<PatientAppointment>();
            //Appointments.OnRefresh += new WeakEventHandler<RefreshEventArgs>(Patients_OnRefresh).Handler;
            Globals.EventAggregator.Subscribe(this);

            var vm = Globals.GetViewModel<IEnumListing>();
            vm.EnumType = typeof(AllLookupValues.ApptStatus);
            vm.AddSelectOneItem = true;
            vm.LoadData();
            AppointmentStatusContent = vm;
            ((INotifyPropertyChangedEx) AppointmentStatusContent).PropertyChanged += AppointmentStatusContent_PropertyChanged;

            //KMx: Mặc định chọn Status "Đã xác nhận" (12/04/2014 14:06).
            if(vm.EnumItemList != null && vm.EnumItemList.Count > 0)
            {
                foreach (var item in vm.EnumItemList)
                {
                    if (item.EnumItem != null)
                    {
                        if ((long)(AllLookupValues.ApptStatus)item.EnumItem == (long)AllLookupValues.ApptStatus.BOOKED)
                        {
                            vm.SelectedItem = item;
                            break;
                        }
                    }
                }
            }

            
            var appointmentListingVm = Globals.GetViewModel<IAppointmentListingV2>();
            AppointmentListingContent = appointmentListingVm;
            ActivateItem(appointmentListingVm);
        }

        void AppointmentStatusContent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                if (AppointmentStatusContent.SelectedItem != null
                    && AppointmentStatusContent.SelectedItem.EnumItem != null
                    && (long)(AllLookupValues.ApptStatus)AppointmentStatusContent.SelectedItem.EnumItem > 0)
                {
                    SearchCriteria.V_ApptStatus = (long)(AllLookupValues.ApptStatus)AppointmentStatusContent.SelectedItem.EnumItem;   
                }
                else
                {
                    SearchCriteria.V_ApptStatus = null;
                }
            }
        }

        //public void Patients_OnRefresh(object sender, RefreshEventArgs e)
        //{
        //    SearchAppointments(Appointments.PageIndex, Appointments.PageSize, false);
        //}

        public void CancelCmd()
        {
            SelectedAppointment = null;
            TryClose();
        }

        public void OkCmd()
        {
            Globals.EventAggregator.Publish(new ItemSelected<PatientAppointment> { Item = SelectedAppointment});
            TryClose();
        }

        private IEnumListing _appointmentStatusContent;
        public IEnumListing AppointmentStatusContent
        {
            get { return _appointmentStatusContent; }
            set
            {
                _appointmentStatusContent = value;
                NotifyOfPropertyChange(() => AppointmentStatusContent);
            }
        }

        private IAppointmentListingV2 _appointmentListingContent;
        public IAppointmentListingV2 AppointmentListingContent
        {
            get { return _appointmentListingContent; }
            set
            {
                _appointmentListingContent = value;
                NotifyOfPropertyChange(() => AppointmentListingContent);
            }
        }

        private PatientAppointment _selectedAppointment;

        public PatientAppointment SelectedAppointment
        {
            get { return _selectedAppointment; }
            set
            {
                _selectedAppointment = value;
                NotifyOfPropertyChange(()=>SelectedAppointment);
            }
        }

        /// <summary>
        /// Biến này lưu lại thông tin người dùng tìm kiếm trên form, 
        /// </summary>
        private AppointmentSearchCriteria _criteria;

        private AppointmentSearchCriteria _searchCriteria;
        public AppointmentSearchCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set 
            {
                _searchCriteria = value; 
                NotifyOfPropertyChange(()=>SearchCriteria);
            }
        }

        private PagedSortableCollectionView<PatientAppointment> _appointments;
        public PagedSortableCollectionView<PatientAppointment> Appointments
        {
            get { return _appointments; }
            private set
            {
                _appointments = value;
                NotifyOfPropertyChange(()=>Appointments);
            }
        }
        //▼====== #001
        AxSearchPatientTextBox txtNameTextBox;
        public void txtName_Loaded(object sender, RoutedEventArgs e)
        {
            txtNameTextBox = (AxSearchPatientTextBox)sender;
        }
        public void SearchCmd()
        {
            if ((txtNameTextBox == null || !String.IsNullOrEmpty(txtNameTextBox.Text)))
            {
                //_criteria = _searchCriteria.DeepCopy();
                //Appointments.PageIndex = 0;
                //SearchAppointments(0, Appointments.PageSize, true);

                if (_searchCriteria.DateFrom.HasValue && _searchCriteria.DateTo.HasValue &&
                    _searchCriteria.DateFrom.Value > _searchCriteria.DateTo.Value)
                {
                    var temp = _searchCriteria.DateFrom;
                    _searchCriteria.DateFrom = _searchCriteria.DateTo;
                    _searchCriteria.DateTo = temp;
                }
                AppointmentListingContent.SearchCriteria = _searchCriteria;
                AppointmentListingContent.StartSearching();
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z2291_G1_CanhBaoNhapDuChiTiet);
                return;
            }
            ////▲====== #001
        }

        
        //private void SearchAppointments(int pageIndex,int pageSize, bool bCountTotal)
        //{
        //    if(_searchCriteria.DateFrom.HasValue && _searchCriteria.DateTo.HasValue &&
        //        _searchCriteria.DateFrom.Value > _searchCriteria.DateTo.Value)
        //    {
        //        var temp = _searchCriteria.DateFrom;
        //        _searchCriteria.DateFrom = _searchCriteria.DateTo;
        //        _searchCriteria.DateTo = temp;
        //    }
        //    var t = new Thread(() =>
        //    {
        //        if (_criteria == null)
        //        {
        //            _criteria = _searchCriteria;
        //        }
        //        IsLoading = true;
        //        AxErrorEventArgs error = null;
        //        try
        //        {
        //            using (var serviceFactory = new AppointmentServiceClient())
        //            {
        //                var client = serviceFactory.ServiceInstance;
        //                client.BeginGetAppointmentsDay(_criteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback(asyncResult =>
        //                {
        //                    var totalCount = 0;
        //                    IList<PatientAppointment> allItems = null;
        //                    var bOK = false;
        //                    try
        //                    {
        //                        allItems = client.EndGetAppointmentsDay(out totalCount, asyncResult);
        //                        bOK = true;
        //                    }
        //                    catch (FaultException<AxException> fault)
        //                    {
        //                        error = new AxErrorEventArgs(fault);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        error = new AxErrorEventArgs(ex);
        //                    }

        //                    Appointments.Clear();
        //                    if (!bOK) return;
        //                    if (bCountTotal)
        //                    {
        //                        Appointments.TotalItemCount = totalCount;
        //                    }
        //                    if (allItems != null)
        //                    {
        //                        foreach (var item in allItems)
        //                        {
        //                            Appointments.Add(item);
        //                        }
        //                    }
        //                }), null)
        //                    ;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            error = new AxErrorEventArgs(ex);
        //        }
        //        finally
        //        {
        //            IsLoading = false;
        //        }
        //        if (error != null)
        //        {
        //            Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
        //        }
        //    });
        //    t.Start();
        //}

        public void ResetFilterCmd()
        {
            SearchCriteria = new AppointmentSearchCriteria();
        }
        public void DoubleClick(object args)
        {
            var eventArgs = args as EventArgs<object>;
            if (eventArgs != null)
            {
                SelectedAppointment = eventArgs.Value as PatientAppointment;

                Globals.EventAggregator.Publish(new ItemSelected<PatientAppointment> { Item = eventArgs.Value as PatientAppointment });
            }
            TryClose();
        }


        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(()=>IsLoading);
            }
        }

        public void SelectPatientAndClose(object context)
        {
            SelectedAppointment = context as PatientAppointment;

            if (SelectedAppointment != null)
            {
                Globals.EventAggregator.Publish(new ItemSelected<PatientAppointment> { Item = SelectedAppointment });
            }

            TryClose();
        }
        public void KeyUpCmd(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (!IsLoading)
                {
                    if (e.Key == Key.Right)
                    {
                        //Go To Next Page.
                        Appointments.MoveToNextPage();
                        e.Handled = true;
                    }
                    else if (e.Key == Key.Left)
                    {
                        //Back to Prev Page.
                        Appointments.MoveToPreviousPage();
                        e.Handled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Copy danh sách bệnh nhân vào Patient list. Khỏi mắc công search lại.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="criteria"></param>
        /// <param name="total"></param>
        public void CopyExistingAppointmentList(IList<PatientAppointment> items, AppointmentSearchCriteria criteria, int total)
        {
            _criteria = criteria.DeepCopy();
            _appointments.Clear();
            if (items != null && items.Count > 0)
            {
                foreach (var p in items)
                {
                    _appointments.Add(p);
                }
                _appointments.TotalItemCount = total;
            }
            NotifyOfPropertyChange(()=>Appointments);
        }

        //public void Handle(AddCompleted<Patient> message)
        //{
        //    if(message != null && message.Item != null)
        //    {
        //        if(IsActive)
        //        {
        //            SearchAppointments(Appointments.PageIndex, Appointments.PageSize, true);
        //        }
        //    }
        //}
        public void Handle(DoubleClick message)
        {
            if(message.Source != AppointmentListingContent)
                return;
            var eventArgs = message.EventArgs;
            SelectedAppointment = eventArgs.Value as PatientAppointment;
            Globals.EventAggregator.Publish(new ItemSelecting<object, PatientAppointment> { Sender = this, Item = SelectedAppointment });
        }
    }
}
