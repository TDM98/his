using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Collections.ObjectModel;
using eHCMS.ControlsLibrary;
using System.ComponentModel;
using System.Reflection;
using eHCMS.Common;
using System.Collections.Generic;
using eHCMS.Common.BindingSource;

namespace eHCMS.CommonUserControls
{
    public partial class UCLeftMenu : UserControl
    {
        //public event EventHandler<SelectionChangedEventArgs> MenuItemChanged;

        //public event EventHandler<SelectionChangingEventArgs> CustomMenuItemChanging;

        //private DependencyPropertyChangedListener _propertyChangedListener;
        //private Dictionary<MenuBindingSource, ListBox> _listBoxes;
        //private MenuBindingSource _selectedItem;
        //private MenuBindingSource _previousItem;

        //MenuViewModel MenuVM
        //{
        //    get { return (MenuViewModel)this.LayoutRoot.DataContext; }
        //}

        public UCLeftMenu()
        {
            //// Required to initialize variables
            //InitializeComponent();
            //_propertyChangedListener = DependencyPropertyChangedListener.Create(mnuLeft, "ItemsSource");
            //_propertyChangedListener.ValueChanged += new EventHandler<DependencyPropertyValueChangedEventArgs>(_propertyChangedListener_ValueChanged);

        }

    //    void _propertyChangedListener_ValueChanged(object sender, DependencyPropertyValueChangedEventArgs e)
    //    {
    //        //Recreate _listBoxes when ItemsSource property changed
    //        _listBoxes = new Dictionary<MenuBindingSource, ListBox>();
    //        _selectedItem = MenuVM.LeftMenuSelectedItem;
    //        _previousItem = null;

    //        RefreshAccordion();
    //    }

    //    private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
    //    {

    //    }

    //    int previousSelectedIndex = -1;

    //    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    //    {
    //        if (e.AddedItems.Count == 0)
    //            return;

    //        ListBox curListBox = sender as ListBox;

    //        SelectionChangingEventArgs args = new SelectionChangingEventArgs();
    //        args.NewSelectedIndex = curListBox.SelectedIndex;

    //        if (CustomMenuItemChanging != null)
    //        {
    //            CustomMenuItemChanging(curListBox, args);
    //        }
    //        if (args.Cancel)
    //        {
    //            return;
    //        }

    //        bool allowed = false;
    //        MenuBindingSource currentItem = (MenuBindingSource)e.AddedItems[0];
    //        if (null == _previousItem && e.RemovedItems.Count > 0)
    //        {
    //            _previousItem = (MenuBindingSource)e.RemovedItems[0];
    //        }

    //        //Kiem tra dc phep chon item nay khong.
    //        if (MenuVM.CanSelectNextItemDelegate != null)
    //        {
    //            allowed = MenuVM.CanSelectNextItemDelegate();
    //        }

    //        if (!allowed)
    //        {
    //            MessageBox.Show("Cannot select item: " + e.AddedItems[0].ToString());
    //            RefreshLeftMenu();
    //            return;
    //        }

    //        MenuVM.SelectItem(currentItem);

    //        RefreshLeftMenu();
    //        if (null != MenuItemChanged)
    //        {
    //            MenuItemChanged(sender, e);
    //        }
    //        _previousItem = currentItem;
    //        previousSelectedIndex = curListBox.SelectedIndex;
    //    }
    //    private void RefreshLeftMenu()
    //    {
    //        _selectedItem = MenuVM.LeftMenuSelectedItem;
          
    //        ListBox curListBox = null;
    //        bool bCurListBoxFound = false;
    //        bool bPrevListBoxFound = false;

    //        ListBox prevListBox = null;
            
    //        if (_listBoxes != null)
    //        {
    //            foreach (ListBox b in _listBoxes.Values)
    //            {
    //                foreach (MenuBindingSource item in b.Items)
    //                {
    //                    if (item == _selectedItem)
    //                    {
    //                        curListBox = b;
    //                        bCurListBoxFound = true;
    //                    }
    //                    else if (item == MenuVM.PrevMenuSelectedItem)
    //                    {
    //                        prevListBox = b;
    //                        bPrevListBoxFound = true;
    //                    }

    //                    if(bCurListBoxFound & bPrevListBoxFound)
    //                    {
    //                        break;
    //                    }
    //                }
    //                if (bCurListBoxFound & bPrevListBoxFound)
    //                {
    //                    break;
    //                }
    //            }
                
    //        }
    //        if (curListBox != null)
    //        {
    //            curListBox.SelectedItem = _selectedItem;
    //        }

    //        if(prevListBox != null && prevListBox != curListBox)
    //        {
    //            prevListBox.SelectedIndex = -1;
    //        }
    //        RefreshAccordion();
    //    }

    //    private void ListBox_Loaded(object sender, RoutedEventArgs e)
    //    {
    //        //Set the Listbox's width. Fix the error caused by Accordion
    //        ListBox curListBox = sender as ListBox;
    //        if (curListBox != null)
    //        {
    //            curListBox.Width = mnuLeft.ActualWidth - 5;
    //            _listBoxes.Add((MenuBindingSource)curListBox.DataContext, curListBox);
    //            curListBox.SelectedItem = MenuVM.LeftMenuSelectedItem;
    //        }
    //    }

    //    private void mnuLeft_MouseLeave(object sender, MouseEventArgs e)
    //    {
    //        RefreshAccordion();
    //    }

    //    private void RefreshAccordion()
    //    {
    //        if (_selectedItem != null)
    //        {
    //            if (_selectedItem.Parent != mnuLeft.SelectedItem)
    //            {
    //                mnuLeft.SelectedItem = _selectedItem.Parent;
    //            }
    //        }
    //    }


    //    /// <summary>
    //    /// Tìm item chứa navigateURL
    //    /// </summary>
    //    /// <param name="navigateURL"></param>
    //    /// <param name="b">Listbox chứa item này</param>
    //    /// <returns>Item đang được chọn</returns>
    //    public MenuBindingSource FindLeftMenuItemByNavigationURL(string navigateURL, out ListBox b)
    //    {
    //        b = null;
    //        MenuBindingSource match = null;
    //        if (_listBoxes == null)
    //            return null;
    //        foreach (ListBox listbox in _listBoxes.Values)
    //        {
    //            if (listbox.Items == null || listbox.Items.Count == 0)
    //            {
    //                return null;
    //            }
    //            foreach (MenuBindingSource child in listbox.Items)
    //            {
    //                match = child.FindItemContainsNavigationURL(navigateURL);
    //                if (match != null)
    //                {
    //                    b = listbox;
    //                    return match;
    //                }
    //            }
    //        }

    //        return match;
    //    }
    //    public void SelectItem(ListBox b, MenuBindingSource item)
    //    {
    //        b.SelectedItem = item;
    //        RefreshAccordion();
    //    }

    //    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    //    {
    //        //MenuVM.CanSelectNextItemDelegate = () => { return false; };
    //        MenuVM.CanSelectNextItemDelegate = () => { return true; };
    //    }

    //    private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
    //    {
    //        mnuLeft.Height = LayoutRoot.ActualHeight - mnuLeft.Margin.Top - mnuLeft.Margin.Bottom;
    //    }
    //    public void SetHeight(double newHeight)
    //    {
    //        this.LayoutRoot.Height = newHeight;
    //    }
    //}

    //public class SelectionChangingEventArgs : CancelEventArgs
    //{
    //    public SelectionChangingEventArgs() : base() { }

    //    public SelectionChangingEventArgs(bool cancel) : base(cancel) { }

    //    private int _OldSelectedIndex = -1;
    //    private int _NewSelectedIndex = -1;

    //    public int OldSelectedIndex
    //    {
    //        get
    //        {
    //            return _OldSelectedIndex;
    //        }
    //        set
    //        {
    //            _OldSelectedIndex = value;
    //        }
    //    }
    //    public int NewSelectedIndex
    //    {
    //        get
    //        {
    //            return _NewSelectedIndex;
    //        }
    //        set
    //        {
    //            _NewSelectedIndex = value;
    //        }
    //    }
    }
}