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
using Xceed.Wpf.Toolkit;

namespace aEMR.Controls
{
    public class AxChildWindow:ChildWindow
    {
        private static int InstanceNo = 0;
        private int _MyID;
        public int MyID
        {
            get
            {
                return _MyID;
            }
        }

        public static Action<string,string> ConstructorLogging;
        public static Action<string, string> DestructorLogging;
        
        ~AxChildWindow()
        {
            if (DestructorLogging != null)
            {
                DestructorLogging(this.GetType().Name, _MyID.ToString());
            }
        }
        public AxChildWindow()
            : base()
        {
            this.KeyUp += new KeyEventHandler(AxChildWindow_KeyUp);
          //  this.KeyDown += new KeyEventHandler(AxChildWindow_KeyDown);

            this.Loaded += new RoutedEventHandler(AxChildWindow_Loaded);
            this.MouseRightButtonDown += new MouseButtonEventHandler(AxChildWindow_MouseRightButtonDown);
            _MyID = InstanceNo++;
            if(ConstructorLogging != null)
            {
                ConstructorLogging(this.GetType().Name, _MyID.ToString());
            }
            this.Style = Resources["ChildWindowStyle1"] as Style;
        }

        void AxChildWindow_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        void AxChildWindow_KeyUp(object sender, KeyEventArgs e)
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

        void AxChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_LazyLoad)
            {
                IEnumerable<DependencyObject> children = GetAllChildControls();
                _AllControls = children.OrderBy(ctrl => ((Control)ctrl).TabIndex).ToList(); 
            }
        }

        void AxChildWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (AllControls.Count > 1)
                {
                    SetFocus(e);
                }
            }
        }

        private void SetFocus(KeyEventArgs e)
        {
            Control lastChild = (Control)_AllControls.LastOrDefault();
            if(lastChild != null)
            {
                object original = e.OriginalSource;

                Control focusTo = null;
                int matchIndex = -2;
                if(lastChild.Equals(original))
                {
                    matchIndex = -1;
                    //focusTo = (Control)_AllControls[matchIndex];
                }
                else
                {
                    int count = _AllControls.Count;
                    
                    for (int i = 0; i < count; i++ )
                    {
                        if(original.Equals(_AllControls[i]))
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
                    while(!bFocusOK && matchIndex <_AllControls.Count-1)
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
                                                            typeof(AxAutoComplete),typeof(DatePickerTextBox)
                                                            ,typeof(AxTextBoxHICard)
        };

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
            for(int i=0;i<count;i++)
            {
                temp = VisualTreeHelper.GetChild(root,i);
                all.AddRange(GetAllChildControls(temp));
            }
            return all;
        }

        public IEnumerable<DependencyObject> GetAllChildControls()
        {
            return GetAllChildControls(this);
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.MainWindow.SetValue(Control.IsEnabledProperty, true);
        }
    }
}
