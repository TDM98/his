using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPatientRegistrationDetailsListing)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientRegistrationDetailsListingViewModel : Conductor<object>, IPatientRegistrationDetailsListing
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PatientRegistrationDetailsListingViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

        }
        protected override void OnActivate()
        {
            base.OnActivate();

            RegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();

            GetAllRegistrationDetails_ForGoToKhamBenh(PtRegistrationID);
        }

        private PatientRegistration _selectedRegistration;
        public PatientRegistration SelectedRegistration
        {
            get { return _selectedRegistration; }
            set
            {
                _selectedRegistration = value;
                NotifyOfPropertyChange(() => SelectedRegistration);
            }
        }

        private Int64 _PtRegistrationID;
        public Int64 PtRegistrationID
        {
            get { return _PtRegistrationID; }
            set
            {
                _PtRegistrationID = value;
                NotifyOfPropertyChange(() => PtRegistrationID);
            }
        }

        public void CancelCmd()
        {
            SelectedRegistrationDetails = null;
            TryClose();
        }

        private PatientRegistrationDetail _SelectedRegistrationDetails;
        public PatientRegistrationDetail SelectedRegistrationDetails
        {
            get { return _SelectedRegistrationDetails; }
            set
            {
                _SelectedRegistrationDetails = value;
                NotifyOfPropertyChange(() => SelectedRegistrationDetails);
            }
        }

        private ObservableCollection<PatientRegistrationDetail> _registrationDetails;
        public ObservableCollection<PatientRegistrationDetail> RegistrationDetails
        {
            get { return _registrationDetails; }
            private set
            {
                _registrationDetails = value;
                NotifyOfPropertyChange(() => RegistrationDetails);
            }
        }
        private void GetAllRegistrationDetails_ForGoToKhamBenh(Int64 PtRegistrationID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K2945_G1_DSDV) });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllRegistrationDetails_ForGoToKhamBenh(PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetAllRegistrationDetails_ForGoToKhamBenh(asyncResult);

                            if (items != null)
                            {
                                RegistrationDetails = new ObservableCollection<DataEntities.PatientRegistrationDetail>(items);
                            }
                            else
                            {
                                RegistrationDetails = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }


        public void hplChoose_Click(object selectItem)
        {
            if (selectItem != null)
            {
                PatientRegistrationDetail p = (selectItem as PatientRegistrationDetail);

                CheckForDiKhamBenh(p);
            }
        }

        private void CheckForDiKhamBenh(PatientRegistrationDetail p)
        {
            if (SelectedRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                if (p.PaidTime == null && (SelectedRegistration.PatientClassID.GetValueOrDefault(0) != (long)ePatientClassification.PayAfter && SelectedRegistration.PatientClassID.GetValueOrDefault(0) != (long)ePatientClassification.CompanyHealthRecord))
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z1470_G1_DV0ChuaTraTienKgTheKB, p.RefMedicalServiceItem.MedServiceName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
            }
            
            //Đã trả tiền và chưa khám thì mới khám, có khi vừa khám xong
            if (p.V_ExamRegStatus == (long)AllLookupValues.ExamRegStatus.DANG_KY_KHAM ||
                p.V_ExamRegStatus == (long)AllLookupValues.ExamRegStatus.HOAN_TAT ||
                p.V_ExamRegStatus == (long)AllLookupValues.ExamRegStatus.BAT_DAU_THUC_HIEN
                )
            {
                Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration, PatientRegistrationDetail>() { Sender = SelectedRegistration, Item = p });
                TryClose();
            }
            else
            {
                switch (p.V_ExamRegStatus)
                {
                    //case (Int64)AllLookupValues.ExamRegStatus.HOAN_TAT:
                    //    {
                    //        MessageBox.Show(string.Format("{0}: ", eHCMSResources.K3421_G1_DV) + p.RefMedicalServiceItem.MedServiceName.Trim() + ", Đã Hoàn Tất!" + Environment.NewLine + "Không Thể Tiến Hành Khám Bệnh!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    //        break;
                    //    }
                    case (Int64)AllLookupValues.ExamRegStatus.KHONG_XAC_DINH:
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z1394_G1_KgTheTienHanhKB, p.RefMedicalServiceItem.MedServiceName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            break;
                        }
                    case (Int64)AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI:
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z1395_G1_DVDaNgungVaTraLaiTien, p.RefMedicalServiceItem.MedServiceName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            break;
                        }
                }
            }
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            SelectedRegistrationDetails = eventArgs.Value as PatientRegistrationDetail;

            PatientRegistrationDetail p = SelectedRegistrationDetails;

            CheckForDiKhamBenh(p);
        }

       

    }
}
