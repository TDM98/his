using System.Windows;
using System.Windows.Controls;

namespace eHCMS.CommonUserControls
{
    public partial class UCApptDetailsErrors : UserControl
    {
        public UCApptDetailsErrors()
        {
            InitializeComponent();
        }
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(UCApptDetailsErrors),
            new PropertyMetadata(null, new PropertyChangedCallback(OnTitleChanged)));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string val = e.NewValue as string;
            ((UCApptDetailsErrors)d).lblTitle.Text = val;
        }
    }
}
