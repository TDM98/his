using System.Windows;
using System.Windows.Controls;
namespace aEMR.Common.Views
{
    public partial class PCLItems_SearchAutoCompleteView : UserControl
    {
        public PCLItems_SearchAutoCompleteView()
        {
            InitializeComponent();
        }

        //public string TextToChanged
        //{
        //    get { return (string)GetValue(TextToChangedProperty); }
        //    set { SetValue(TextToChangedProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for TextToChanged.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty TextToChangedProperty =
        //    DependencyProperty.Register("TextToChanged", typeof(string), typeof(PCLItems_SearchAutoCompleteView), new PropertyMetadata(new PropertyChangedCallback(PCLItems_SearchAutoCompleteView.SetValuePropertyChanged)));

        //#region OnNullValueContentPropertyChanged

        //private static void SetValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    ((PCLItems_SearchAutoCompleteView)d).SetValuePropertyChanged();
        //}
        //#endregion

        //private void SetValuePropertyChanged()
        //{ }
    }
}
