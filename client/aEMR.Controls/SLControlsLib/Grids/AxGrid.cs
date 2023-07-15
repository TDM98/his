using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Linq;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace aEMR.Controls
{
    public class AxGrid : Grid
    {
        public AxGrid()
            : base()
        {
            //this.KeyDown += new KeyEventHandler(AxUserControl_KeyDown);
            this.KeyUp += new KeyEventHandler(AxGrid_KeyUp);
            this.Loaded += new RoutedEventHandler(AxGrid_Loaded);
        }
        void AxGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_LazyLoad)
            {
                IEnumerable<DependencyObject> children = GetAllChildControls();
                _AllControls = children.OrderBy(ctrl => ((Control)ctrl).TabIndex).ToList();
            }
        }
        void AxGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DisableFirstNextFocus)
                {
                    DisableFirstNextFocus = false;
                    return;
                }
                if (AllControls.Count > 1)
                {
                    SetFocus(e);
                }
            }
        }
        void AxUserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (AllControls.Count > 1)
                {
                    SetFocus(e);
                }
            }
        }
        private bool _LazyLoad = true;
        private List<DependencyObject> _AllControls;
        public List<DependencyObject> AllControls
        {
            get
            {
                if (_LazyLoad)
                {
                    IEnumerable<DependencyObject> children = GetAllChildControls();
                    _AllControls = children.OrderBy(ctrl => ((Control)ctrl).TabIndex).ToList();
                }
                return _AllControls;
            }
        }
        private void SetFocus(KeyEventArgs e)
        {
            if (e.OriginalSource.GetType().Equals(typeof(TextBox)) || e.OriginalSource.GetType().Equals(typeof(TextBox)))
            {
                TextBox tb = (TextBox)e.OriginalSource;
                if (tb.AcceptsReturn)
                {
                    // allow multiline Textbox to handle Enter to insert new line
                    return;
                }
            }

            Control lastChild = (Control)_AllControls.LastOrDefault();
            if (lastChild != null)
            {
                object original = e.OriginalSource;

                Control focusTo = null;
                int matchIndex = -2;
                if (lastChild.Equals(original))
                {
                    matchIndex = -1;
                    //focusTo = (Control)_AllControls[matchIndex];
                }
                else
                {
                    int count = _AllControls.Count;
                    //int index = (original as Control).TabIndex;
                    // matchIndex = index;
                    for (int i = 0; i < count; i++)
                    {
                        if (original.Equals(_AllControls[i]))
                        {
                            matchIndex = i;
                            break;
                        }
                    }
                }
                //Nếu không chọn được control thì chọn control kế tiếp
                if (matchIndex >= -1)
                {
                    bool bFocusOK = false;
                    while (!bFocusOK && matchIndex < _AllControls.Count - 1)
                    {
                        focusTo = (Control)_AllControls[++matchIndex];
                        bFocusOK = focusTo.Focus();
                    }
                }
            }
        }

        private List<Type> _KnownTypes = new List<Type>(){typeof(TextBox),typeof(PasswordBox),
            typeof(CheckBox), typeof(RadioButton),
            typeof(ListBox), typeof(ComboBox),
            typeof(Button), typeof(KeyEnabledComboBox),
            typeof(AxTextBox), typeof(AxComboBox),
            typeof(DatePicker),typeof(AutoCompleteBox),
            typeof(AxAutoComplete),typeof(DatePickerTextBox) };
        private IEnumerable<DependencyObject> GetAllChildControls(DependencyObject root)
        {
            List<DependencyObject> all = new List<DependencyObject>();
            //if(root is Control)
            //{
            //    all.Add(root); 
            //}
            if (_KnownTypes.Contains(root.GetType()))
            {
                all.Add(root);
            }
            int count = VisualTreeHelper.GetChildrenCount(root);
            DependencyObject temp;
            for (int i = 0; i < count; i++)
            {
                temp = VisualTreeHelper.GetChild(root, i);
                all.AddRange(GetAllChildControls(temp));
            }
            return all;
        }
        public IEnumerable<DependencyObject> GetAllChildControls()
        {
            return GetAllChildControls(this);
        }
        public bool DisableFirstNextFocus = false;
    }
}