using eHCMSLanguage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Threading;
using System;
using DataEntities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using aEMR.CommonTasks;
using aEMR.Infrastructure.ServiceCore;
using UserServiceProxy;
using aEMR.Common;

/*
 * 20181019 #001 TNHX: [BM0003193] Refactor code
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IRefDepartments_BystrV_DeptType)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefDepartments_BystrV_DeptTypeViewModel : Conductor<object>, IRefDepartments_BystrV_DeptType
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public RefDepartments_BystrV_DeptTypeViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            Parent = null;
        }

        private bool _ShowDeptLocation;
        public bool ShowDeptLocation
        {
            get { return _ShowDeptLocation; }
            set
            {
                _ShowDeptLocation = value;
                NotifyOfPropertyChange(() => ShowDeptLocation);
            }
        }

        private string _strV_DeptType;
        public string strV_DeptType
        {
            get { return _strV_DeptType; }
            set
            {
                _strV_DeptType = value;
                NotifyOfPropertyChange(() => strV_DeptType);
            }
        }

        private ObservableCollection<RefDepartmentsTree> _ObjRefDepartments_Tree;
        public ObservableCollection<RefDepartmentsTree> ObjRefDepartments_Tree
        {
            get
            {
                return _ObjRefDepartments_Tree;
            }
            set
            {
                _ObjRefDepartments_Tree = value;
                NotifyOfPropertyChange(() => ObjRefDepartments_Tree);
            }
        }

        private string StrV_DeptTypeOperation
        {
            get
            {
                if (Parent != null && Parent is IPatientManagement)
                { return ((int)V_DeptTypeOperation.KhoaNoi).ToString(); }
                return null;
            }
        }

        public void RefDepartments_Tree()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0149_G1_DangLayDSCacKhoa);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRefDepartments_Tree(strV_DeptType, ShowDeptLocation, StrV_DeptTypeOperation, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndRefDepartments_Tree(asyncResult);
                                if (items != null)
                                {
                                    ObjRefDepartments_Tree = new ObservableCollection<RefDepartmentsTree>(items);
                                }
                                else
                                {
                                    ObjRefDepartments_Tree = null;
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });
            t.Start();
        }

        //public object Parent { get; set; } = null;

        public void treeView1SelectedItemChanged(object selectedItem)
        {
            if (selectedItem != null)
            {
                if (Parent != null && Parent is IPatientManagement)
                {
                    Globals.EventAggregator.Publish(new LoadPatientListBySeletedDeptID { DepartmentTree = selectedItem });
                }
                else
                {
                    Globals.EventAggregator.Publish(new RefDepartments_BystrV_DeptTypeEvent.TreeSelectionChanged_Event() { ObjRefDepartments_Current = selectedItem });
                }
            }
        }
    }
}
