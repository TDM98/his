using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Input;
using aEMR.Common.BaseModel;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IInPatientInstructionOutstandingTask)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientInstructionOutstandingTaskViewModel : Conductor<object>, IInPatientInstructionOutstandingTask
        , IHandle<LoadMedicalInstructionEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public InPatientInstructionOutstandingTaskViewModel (IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            Globals.EventAggregator.Subscribe(this);
        }
        public void Handle(LoadMedicalInstructionEvent message)
        {
            if (message != null && message.gRegistration != null)
            {
                LoadInPatientInstruction(message.gRegistration);
            }
        }
        private void LoadInPatientInstruction(PatientRegistration aRegistration)
        {
            CurrentRegistration = aRegistration;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInPatientInstructionCollection(CurrentRegistration, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                gPatientInstructionCollection = contract.EndGetInPatientInstructionCollection(asyncResult).ToObservableCollection();
                                PatientInstructionTreeCollection = new ObservableCollection<InPatientInstructionTree>();
                                foreach (var item in gPatientInstructionCollection.Select(x => x.Department).ToList())
                                {
                                    if (!PatientInstructionTreeCollection.Select(x => x.Department.DeptID).Contains(item.DeptID))
                                    {
                                        PatientInstructionTreeCollection.Add(new InPatientInstructionTree(item));
                                    }
                                }
                                foreach(var item in PatientInstructionTreeCollection)
                                {
                                    item.Children = gPatientInstructionCollection.Where(x => x.Department.DeptID == item.Department.DeptID).Select(x => new InPatientInstructionTree(x)).ToList();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
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
                    MessageBox.Show(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private ObservableCollection<InPatientInstruction> _gPatientInstructionCollection;
        public ObservableCollection<InPatientInstruction> gPatientInstructionCollection
        {
            get
            {
                return _gPatientInstructionCollection;
            }
            set
            {
                _gPatientInstructionCollection = value;
                NotifyOfPropertyChange(() => gPatientInstructionCollection);
            }
        }
        private ObservableCollection<InPatientInstructionTree> _PatientInstructionTreeCollection;
        public ObservableCollection<InPatientInstructionTree> PatientInstructionTreeCollection
        {
            get
            {
                return _PatientInstructionTreeCollection;
            }
            set
            {
                _PatientInstructionTreeCollection = value;
                NotifyOfPropertyChange(() => PatientInstructionTreeCollection);
            }
        }
        private PatientRegistration _CurrentRegistration;
        public PatientRegistration CurrentRegistration
        {
            get
            {
                return _CurrentRegistration;
            }
            set
            {
                if (_CurrentRegistration == value)
                {
                    return;
                }
                _CurrentRegistration = value;
                NotifyOfPropertyChange(() => CurrentRegistration);
            }
        }
        public void InstructionDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TreeView).SelectedItem == null)
            {
                return;
            }
            InPatientInstructionTree SelectedItem = (sender as TreeView).SelectedItem as InPatientInstructionTree;
            if (SelectedItem.Instruction != null)
            {
                //Globals.EventAggregator.Publish(new ReloadLoadMedicalInstructionEvent() { gInPatientInstruction = SelectedItem.Instruction });
                GlobalsNAV.ShowDialog<IInPatientInstruction>((aView) =>
                {
                    if (aView is ViewModelBase)
                    {
                        (aView as ViewModelBase).IsDialogView = true;
                    }
                    aView.LoadRegistrationInfo(CurrentRegistration, false);
                    aView.ReloadLoadInPatientInstruction(SelectedItem.Instruction.IntPtDiagDrInstructionID);
                }, null, false, true, Globals.GetDefaultDialogViewSize());
            }
        }
    }
}