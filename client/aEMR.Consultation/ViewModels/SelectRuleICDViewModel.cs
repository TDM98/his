using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using Castle.Windsor;
using aEMR.Common.BaseModel;
using System.Linq;
using aEMR.Controls;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.Infrastructure.Events;
using aEMR.ReportModel.ReportModels;
using DevExpress.ReportServer.Printing;
using System.Data;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Data;
using aEMR.CommonTasks;
using aEMR.Common.HotKeyManagement;
using System.Windows.Input;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(ISelectRuleICD)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SelectRuleICDViewModel : ViewModelBase, ISelectRuleICD
    {
        [ImportingConstructor]
        public SelectRuleICDViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            //HasInputBindingCmd = true;
            CurrentItem = new RuleDiseasesReferences();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }

        public override void DeactivateItem(object item, bool close)
        {
            base.DeactivateItem(item, close);
            Globals.EventAggregator.Unsubscribe(this);
            var homevm = Globals.GetViewModel<IHome>();
            homevm.OutstandingTaskContent = null;
            homevm.IsExpandOST = false;
        }

        #region Properties
        private ObservableCollection<RuleDiseasesReferences> _ListRuleDiseasesReferences;
        public ObservableCollection<RuleDiseasesReferences> ListRuleDiseasesReferences
        {
            get
            {
                return _ListRuleDiseasesReferences;
            }
            set
            {
                _ListRuleDiseasesReferences = value;
                NotifyOfPropertyChange(() => ListRuleDiseasesReferences);
            }
        }
        public string TitleForm { get; set; }
        public RuleDiseasesReferences CurrentItem { get; set; }
        public int MainICDIndex { get; set; }
        #endregion

        public void dtgListSelectionChanged(object args)
        {
            if (((object[])(((SelectionChangedEventArgs)(args)).AddedItems)).Length > 0)
            {
                if (((object[])(((SelectionChangedEventArgs)(args)).AddedItems))[0] != null)
                {
                    RuleDiseasesReferences temp = (RuleDiseasesReferences)((object[])(((SelectionChangedEventArgs)(args)).AddedItems))[0];
                    temp.IsChoosed = true;
                    CurrentItem = temp.DeepCopy();
                    foreach (var item in ListRuleDiseasesReferences)
                    {
                        if (item.SubICD10 != temp.SubICD10)
                        {
                            item.IsChoosed = false;
                        }
                        else
                        {
                            item.IsChoosed = true;
                        }
                    }
                    NotifyOfPropertyChange(() => ListRuleDiseasesReferences);
                }
            }
        }

        public void DoubleClick(object source)
        {
            if (source == null)
            {
                return;
            }
            EventArgs<object> eventArgs = source as EventArgs<object>;
            CurrentItem = eventArgs.Value as RuleDiseasesReferences;
            if (CurrentItem != null)
            {
                Globals.EventAggregator.Publish(new SelectedRuleICDForDiagnosisTreatment()
                {
                    SubICDInfo = CurrentItem.SubICDInfo,
                    MainICDIndex = MainICDIndex,
                    IsException = CurrentItem.IsException
                });
                TryClose();
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z1688_G1_TTinKgHopLe);
            }
        }

        public void ItemClick(object source)
        {
            if (source == null)
            {
                return;
            }
            EventArgs<object> eventArgs = source as EventArgs<object>;
            CurrentItem = eventArgs.Value as RuleDiseasesReferences;
            if (CurrentItem != null)
            {

            }
            else
            {
                MessageBox.Show(eHCMSResources.Z1688_G1_TTinKgHopLe);
            }
        }

        public void btnOK()
        {
            if (CurrentItem != null && CurrentItem.SubICDInfo != null)
            {
                Globals.EventAggregator.Publish(new SelectedRequireSubICDForDiagnosisTreatment()
                {
                    SubICDInfo = CurrentItem.SubICDInfo,
                    MainICDIndex = MainICDIndex,
                    IsException = CurrentItem.IsException
                });
                TryClose();
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z1688_G1_TTinKgHopLe);
            }
        }
    }
}
