using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using System.ComponentModel;
using aEMR.Common;


namespace aEMR.Controls
{
    /// <summary>
    /// Fix bug: Khi itemsource được update lại, combobox không thể chọn được selecteditem.
    /// Bug của Silverlight.
    /// </summary>
    public class AxListBox : ListBox
    {
        public AxListBox()
            : base()
        {
            SelectionChanged += new SelectionChangedEventHandler(AxListBox_SelectionChanged);
        }

        void AxListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TODO:
            //Neu da bind roi thoi.
            if (_selectedItemRebind)
                return;

            //Neu khong co item nao duoc chon thi thoi. Neu khong no gan gia tri null thi khung luon.
            //Co chon item moi gan gia tri.
            if(e.AddedItems != null && e.AddedItems.Count > 0)
            {
                SelectedItemEx = this.SelectedItem;
                SelectedValueEx = this.SelectedValue;
            }
            
        }
        private bool _selectedItemRebind = false;
             
        public void ResetSelectedItem(object newItem)
        {
            try
            {
                _selectedItemRebind = true;
                SelectedItem = newItem;
            }
            catch (System.Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
            }
            finally
            {
                _selectedItemRebind = false;
            }
        }

        private bool _isSelected;
        public bool isSelected 
        {
            get { return _isSelected; }
            set 
            {
                _isSelected = value;
            }        
        }
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            //ResetSelectedItem(SelectedItemEx);
            if (this.GetBindingExpression(AxListBox.SelectedItemExProperty) != null)
            {
                ResetSelectedItem(SelectedItemEx);
            }
            else if (this.GetBindingExpression(AxListBox.SelectedValueExProperty) != null)
            {
                ResetSelectedValue(SelectedValueEx);
            }
        }
        public static readonly DependencyProperty SelectedItemExProperty =
            DependencyProperty.Register(
                "SelectedItemEx",
                typeof(object),
                typeof(AxListBox),
                new PropertyMetadata((o, dp) =>
                {
                    var comboBoxEx = o as AxListBox;
                    if (comboBoxEx == null)
                        return;

                    comboBoxEx.ResetSelectedItem(dp.NewValue);
                }));

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public object SelectedItemEx
        {
            get { return (object)GetValue(SelectedItemExProperty); }
            set { SetValue(SelectedItemExProperty, value); }
        }

        public static readonly DependencyProperty SelectedValueExProperty =
            DependencyProperty.Register(
                "SelectedValueEx",
                typeof(object),
                typeof(AxListBox),
                new PropertyMetadata((o, dp) =>
                {
                    var comboBoxEx = o as AxListBox;
                    if (comboBoxEx == null)
                        return;

                    comboBoxEx.ResetSelectedValue(dp.NewValue);
                }));

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public object SelectedValueEx
        {
            get { return (object)GetValue(SelectedValueExProperty); }
            set { SetValue(SelectedValueExProperty, value); }
        }
        public void ResetSelectedValue(object newItem)
        {
            try
            {
                SelectedValue = newItem;
            }
            catch (System.Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
            }
        }
    }
}
