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
    [Export(typeof(ISelectRequireSubICD)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SelectRequireSubICDViewModel : ViewModelBase, ISelectRequireSubICD
    {
        [ImportingConstructor]
        public SelectRequireSubICDViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            //HasInputBindingCmd = true;
            CurrentItem = new RequiredSubDiseasesReferences();
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
        private ObservableCollection<RequiredSubDiseasesReferences> _ListRequiredSubDiseasesReferences;
        public ObservableCollection<RequiredSubDiseasesReferences> ListRequiredSubDiseasesReferences
        {
            get
            {
                return _ListRequiredSubDiseasesReferences;
            }
            set
            {
                _ListRequiredSubDiseasesReferences = value;
                NotifyOfPropertyChange(() => ListRequiredSubDiseasesReferences);
            }
        }
        public string TitleForm { get; set; }
        public RequiredSubDiseasesReferences CurrentItem { get; set; }
        public int MainICDIndex { get; set; }
        #endregion

        public void dtgListSelectionChanged(object args)
        {
            if (((object[])(((SelectionChangedEventArgs)(args)).AddedItems)).Length > 0)
            {
                if (((object[])(((SelectionChangedEventArgs)(args)).AddedItems))[0] != null)
                {
                    RequiredSubDiseasesReferences temp = (RequiredSubDiseasesReferences)((object[])(((SelectionChangedEventArgs)(args)).AddedItems))[0];
                    temp.IsChoosed = true;
                    CurrentItem = temp.DeepCopy();
                    foreach (var item in ListRequiredSubDiseasesReferences)
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
                    NotifyOfPropertyChange(() => ListRequiredSubDiseasesReferences);
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
            CurrentItem = eventArgs.Value as RequiredSubDiseasesReferences;
            if (CurrentItem != null)
            {
                Globals.EventAggregator.Publish(new SelectedRequireSubICDForDiagnosisTreatment()
                {
                    SubICDInfo = CurrentItem.SubICDInfo,
                    MainICDIndex = MainICDIndex
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
            CurrentItem = eventArgs.Value as RequiredSubDiseasesReferences;
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
                    MainICDIndex = MainICDIndex
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
