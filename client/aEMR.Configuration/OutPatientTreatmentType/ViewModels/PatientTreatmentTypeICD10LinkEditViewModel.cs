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
    [Export(typeof(IPatientTreatmentTypeICD10LinkEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientTreatmentTypeICD10LinkEditViewModel : Conductor<object>, IPatientTreatmentTypeICD10LinkEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PatientTreatmentTypeICD10LinkEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
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

        private OutpatientTreatmentTypeICD10Link _ObjICD10Link_Current;
        public OutpatientTreatmentTypeICD10Link ObjICD10Link_Current
        {
            get { return _ObjICD10Link_Current; }
            set
            {
                _ObjICD10Link_Current = value;
                NotifyOfPropertyChange(() => ObjICD10Link_Current);
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
        
        public void InitializeNewItem()
        {
            ObjICD10Link_Current = new OutpatientTreatmentTypeICD10Link();
        }

        public void btSave()
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

                        contract.BeginOutpatientTreatmentTypeICD10Link_Edit(ObjICD10Link_Current, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Result = contract.EndOutpatientTreatmentTypeICD10Link_Edit(asyncResult);
                                if (Result)
                                {
                                    Globals.EventAggregator.Publish(new SaveEvent<OutpatientTreatmentTypeICD10Link> { Result = ObjICD10Link_Current });
                                    TryClose();
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
    }
}
