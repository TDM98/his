using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using DataEntities;
using System.Linq;
using System.ComponentModel;
using eHCMSLanguage;
using aEMR.Common.Collections;
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.CommonTasks;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events.SL_Events;
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IDiseaseProgression)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DiseaseProgressionViewModel : Conductor<object>, IDiseaseProgression
    {

        private string _TitleForm = "";
        public string TitleForm
        {
            get
            {
                return _TitleForm;
            }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }

        IEventAggregator _eventArg;
        [ImportingConstructor]
        public DiseaseProgressionViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _eventArg = eventArg;
            if (Globals.allDiseaseProgression == null)
            {
                LoadDiseaseProgression();
                LoadDiseaseProgressionDetails();
            }
            else
            {
                DiseaseProgression = ObjectCopier.DeepCopy((Globals.allDiseaseProgression.ToObservableCollection() as ObservableCollection<DiseaseProgression>));
            }
        }

        protected override void OnActivate()
        {
            _eventArg.Subscribe(this);
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _eventArg.Unsubscribe(this);
        }

        private ObservableCollection<DiseaseProgression> _DiseaseProgression;
        public ObservableCollection<DiseaseProgression> DiseaseProgression
        {
            get { return _DiseaseProgression; }
            set
            {
                _DiseaseProgression = value;
                NotifyOfPropertyChange(() => DiseaseProgression);
            }
        }
        private ObservableCollection<DiseaseProgressionDetails> _DiseaseProgressionDetails;
        public ObservableCollection<DiseaseProgressionDetails> DiseaseProgressionDetails
        {
            get { return _DiseaseProgressionDetails; }
            set
            {
                _DiseaseProgressionDetails = value;
                NotifyOfPropertyChange(() => DiseaseProgressionDetails);
            }
        }
        private bool _IsOpenFromInstruction = false;
        public bool IsOpenFromInstruction
        {
            get { return _IsOpenFromInstruction; }
            set
            {
                _IsOpenFromInstruction = value;
                NotifyOfPropertyChange(() => IsOpenFromInstruction);
            }
        }
        public void LoadDiseaseProgression()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllDiseaseProgression(false, Globals.DispatchCallback(asyncResult =>
                        {
                            IList<DiseaseProgression> allItems = null;
                            try
                            {
                                allItems = contract.EndGetAllDiseaseProgression(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(eHCMSResources.A0693_G1_Msg_InfoKhTheLayDSTinhThanh);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }

                            DiseaseProgression = allItems != null ? new ObservableCollection<DiseaseProgression>(allItems) : null;
                            if (DiseaseProgression != null && DiseaseProgressionDetails != null)
                            {
                                foreach (var item in DiseaseProgression)
                                {
                                    item.DiseaseProgressionDetails = DiseaseProgressionDetails.Where(x => x.DiseaseProgressionID == item.DiseaseProgressionID).ToList();
                                }
                                Globals.allDiseaseProgression = ObjectCopier.DeepCopy((DiseaseProgression.ToList() as List<DiseaseProgression>));
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
        public void LoadDiseaseProgressionDetails()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllDiseaseProgressionDetails(Globals.DispatchCallback(asyncResult =>
                        {
                            IList<DiseaseProgressionDetails> allItems = null;
                            try
                            {
                                allItems = contract.EndGetAllDiseaseProgressionDetails(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(eHCMSResources.A0693_G1_Msg_InfoKhTheLayDSTinhThanh);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }

                            DiseaseProgressionDetails = allItems != null ? new ObservableCollection<DiseaseProgressionDetails>(allItems) : null;
                            if (DiseaseProgression != null && DiseaseProgressionDetails != null)
                            {
                                foreach (var item in DiseaseProgression)
                                {
                                    item.DiseaseProgressionDetails = DiseaseProgressionDetails.Where(x => x.DiseaseProgressionID == item.DiseaseProgressionID).ToList();
                                }
                                Globals.allDiseaseProgression = ObjectCopier.DeepCopy((DiseaseProgression.ToList() as List<DiseaseProgression>));
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
        public void btnOK()
        {
            string SelectDiseaseProgression = "";
            foreach (var item in DiseaseProgression)
            {
                foreach (var detail in item.DiseaseProgressionDetails)
                {
                    if (detail.IsChecked)
                    {
                        SelectDiseaseProgression += ", " + detail.DiseaseProgressionDetailName;
                    }
                }
            }
            if (IsOpenFromInstruction)
            {

                Globals.EventAggregator.Publish(new DiseaseProgressionInstruction_Event() { SelectedDiseaseProgression = SelectDiseaseProgression });
            }
            else if (IsOpenFromTicketCare)
            {
                Globals.EventAggregator.Publish(new DiseaseProgressionFromTicketCare_Event() { SelectedDiseaseProgression = SelectDiseaseProgression });
            }
            else
            {
                Globals.EventAggregator.Publish(new DiseaseProgression_Event() { SelectedDiseaseProgression = SelectDiseaseProgression });
            }
            TryClose();
        }
        public void btnCancel()
        {
            TryClose();
        }
        private bool _IsOpenFromTicketCare = false;
        public bool IsOpenFromTicketCare
        {
            get { return _IsOpenFromTicketCare; }
            set
            {
                _IsOpenFromTicketCare = value;
                NotifyOfPropertyChange(() => IsOpenFromTicketCare);
            }
        }
    }
}