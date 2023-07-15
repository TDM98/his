using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.ViewContracts;
using DataEntities;

namespace aEMR.Common.Views
{
    public partial class DeptTreeView : UserControl, IDeptTreeView
    {
        public DeptTreeView()
        {
            InitializeComponent();
        }
        public void SelectTreeItemByDataItem(RefDepartmentsTree node)
        {
            var viewItem = treeView.ItemContainerGenerator.ContainerFromItem(node) as TreeViewItem;
            if (viewItem != null)
            {
                viewItem.IsSelected = true;
            }
        }
    }
}
