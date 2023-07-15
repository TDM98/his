using System.ComponentModel.Composition;
using System.Windows;
using aEMR.Controls;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(ResourcesListGridView))]
    public partial class ResourcesListGridView : AxUserControl
    {
        
        public ResourcesListGridView()
        {
            InitializeComponent();
            
            this.Loaded += ResourcesListGridView_Loaded;
            this.Unloaded += new RoutedEventHandler(ResourcesListGridView_Unloaded);
        }

        void ResourcesListGridView_Unloaded(object sender, RoutedEventArgs e)
        {
            //grdResources.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        void ResourcesListGridView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        
        public int ResourceType
        {
            get { return (int)GetValue(ResourceTypeProperty); }
            set { SetValue(ResourceTypeProperty, value); }
        }

        public static readonly DependencyProperty ResourceTypeProperty = DependencyProperty.Register(
            "ResourceType",
            typeof(int),
            typeof(ResourcesListGridView),
            new PropertyMetadata(0, new PropertyChangedCallback(OnResourceTypeChanged)));

        private static void OnResourceTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ResourcesListGridView)d).OnResourceTypeChanged(e);
        }

        protected virtual void OnResourceTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            //ViewModel.ResourceCategoryEnum = ResourceType;
        }

    }
}
