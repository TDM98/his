using aEMR.Common.BaseModel;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using DataEntities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace aEMR.StoreDept.ViewModels
{
    [Export(typeof(IOutwardDrugClinicDeptTemplateSelection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OutwardDrugClinicDeptTemplateSelectionViewModel : ViewModelBase, IOutwardDrugClinicDeptTemplateSelection
    {
        #region Properties
        private ObservableCollection<OutwardDrugClinicDeptTemplate> _RequestTemplateCollection;
        public ObservableCollection<OutwardDrugClinicDeptTemplate> RequestTemplateCollection
        {
            get
            {
                return _RequestTemplateCollection;
            }
            set
            {
                _RequestTemplateCollection = value;
                NotifyOfPropertyChange(() => RequestTemplateCollection);
            }
        }

        public OutwardDrugClinicDeptTemplate CurrentRequestTemplate { get; set; }
        #endregion

        #region Events
        public void gvRequestTemplate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as DataGrid).SelectedItem == null)
            {
                return;
            }
            CurrentRequestTemplate = (sender as DataGrid).SelectedItem as OutwardDrugClinicDeptTemplate;
            TryClose();
        }

        public void SearchKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchKey = (sender as TextBox).Text;
                btnFilter();
            }
        }

        public void btnFilter()
        {
            CV_ObjRequestTemplate.Filter = null;
            CV_ObjRequestTemplate.Filter = new Predicate<object>(DoFilter);
        }

        private CollectionViewSource CVS_ObjRequestTemplate = null;
        public CollectionView CV_ObjRequestTemplate
        {
            get;
            set;
        }

        public void LoadDataGrid()
        {
            CVS_ObjRequestTemplate = null;
            CVS_ObjRequestTemplate = new CollectionViewSource { Source = RequestTemplateCollection };
            CV_ObjRequestTemplate = (CollectionView)CVS_ObjRequestTemplate.View;
            NotifyOfPropertyChange(() => CV_ObjRequestTemplate);
        }

        private bool DoFilter(object o)
        {
            OutwardDrugClinicDeptTemplate emp = o as OutwardDrugClinicDeptTemplate;
            if (emp != null)
            {
                if (string.IsNullOrEmpty(SearchKey))
                {
                    SearchKey = "";
                }
                if ((!string.IsNullOrEmpty(emp.OutwardDrugClinicDeptTemplateName) 
                    && Globals.RemoveVietnameseString(emp.OutwardDrugClinicDeptTemplateName.ToLower()).IndexOf(SearchKey.Trim().ToLower()) >= 0)
                    || (!string.IsNullOrEmpty(emp.OutwardDrugClinicDeptTemplateName) 
                    && emp.OutwardDrugClinicDeptTemplateName.ToLower().IndexOf(SearchKey.Trim().ToLower()) >= 0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private string _SearchKey;
        public string SearchKey
        {
            get { return _SearchKey; }
            set
            {
                _SearchKey = value;
                NotifyOfPropertyChange(() => SearchKey);
            }
        }

        public void GvRequestTemplate_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }
        #endregion
    }
}
