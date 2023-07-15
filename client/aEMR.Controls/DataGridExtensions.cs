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
using System.Windows.Data;
using System.Collections;
using System.Linq;
using aEMR.Common;
using aEMR.Common.PagedCollectionView;

using Service.Core.Common;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;

namespace aEMR.Controls
{
    public class DataGridColNamingUtil
    {
        public static string GetColName(DependencyObject depObj)
        {
            return (string)depObj.GetValue(NameProperty);
        }

        public static void SetColName(DependencyObject obj, string valName)
        {
            obj.SetValue(NameProperty, valName);
        }

        public static readonly DependencyProperty NameProperty =
                               DependencyProperty.RegisterAttached("Name", typeof(string), typeof(DataGridColNamingUtil), new UIPropertyMetadata(""));
    }

    public static class DataGridExtensions
    {
        public static DataGridColumn GetColumnByName(this DataGrid grid, string ColumnName)
        {
            if (grid.Columns == null || grid.Columns.Count == 0)
            {
                return null;
            }
            foreach (DataGridColumn col1 in grid.Columns)
            {
                //string tmp = col1.GetValue(FrameworkElement.NameProperty).ToString();
                string tmp = DataGridColNamingUtil.GetColName(col1);
                if (String.Compare(tmp, ColumnName, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    return col1;
                }                
            }

            return null;
            //return grid.Columns.SingleOrDefault(p => String.Compare((string)p.GetValue(FrameworkElement.NameProperty) , ColumnName,StringComparison.CurrentCultureIgnoreCase) == 0);
        }
        public static void NextCellToEdit(this DataGrid grid)
        {
            if (grid.SelectedItem != null)
            {
                int nextColumnIndex = GetNextAvailableColumnIndex(grid);
                grid.CurrentColumn = grid.Columns[nextColumnIndex];
                grid.ScrollIntoView(grid.SelectedItem, grid.CurrentColumn);
                grid.BeginEdit();
                if (grid.CurrentCell == null
                    || grid.CurrentCell.Item == null
                    || grid.CurrentCell.Column.GetCellContent(grid.CurrentCell.Item) == null
                    || grid.CurrentCell.Column.GetCellContent(grid.CurrentCell.Item).Parent == null) return;
                if (grid.CurrentCell.Column.GetCellContent(grid.CurrentCell.Item).Parent is DataGridCell)
                {
                    DataGridCell mDataGridCell = grid.CurrentCell.Column.GetCellContent(grid.CurrentCell.Item).Parent as DataGridCell;
                    mDataGridCell.IsEditing = true;
                    if (mDataGridCell.GetChildrenByType<Control>() == null || mDataGridCell.GetChildrenByType<Control>().Count == 0) return;
                    Control mControl = mDataGridCell.GetChildrenByType<Control>().FirstOrDefault();
                    if (mControl != null) mControl.Focus();
                }
            }
        }

        public static void LastCellToEdit(this DataGrid grid, int nLastCellIdx)
        {
            if (grid.SelectedItem == null)
                return;
            grid.CurrentColumn = grid.Columns[nLastCellIdx];
            grid.ScrollIntoView(grid.SelectedItem, grid.CurrentColumn);
            grid.BeginEdit();
            if (grid.CurrentCell == null
                || grid.CurrentCell.Item == null
                || grid.CurrentCell.Column.GetCellContent(grid.CurrentCell.Item) == null
                || grid.CurrentCell.Column.GetCellContent(grid.CurrentCell.Item).Parent == null) return;
            if (grid.CurrentCell.Column.GetCellContent(grid.CurrentCell.Item).Parent is DataGridCell)
            {
                DataGridCell mDataGridCell = grid.CurrentCell.Column.GetCellContent(grid.CurrentCell.Item).Parent as DataGridCell;
                mDataGridCell.IsEditing = true;
                if (mDataGridCell.GetChildrenByType<Control>() == null || mDataGridCell.GetChildrenByType<Control>().Count == 0) return;
                Control mControl = mDataGridCell.GetChildrenByType<Control>().FirstOrDefault();
                if (mControl != null) mControl.Focus();
            }
        }

        public static void NextOrPrevCellToEdit(this DataGrid grid, bool bNext = true)
        {
            if (grid.SelectedItem == null)
                return;

            int nextOrPrevColumnIndex = 0;
            if (bNext)
            {
                nextOrPrevColumnIndex = GetNextAvailableColumnIndex(grid);
            }
            else
            {
                nextOrPrevColumnIndex = GetPrevAvailableColumnIndex(grid);
            }
            grid.CurrentColumn = grid.Columns[nextOrPrevColumnIndex];
            grid.ScrollIntoView(grid.SelectedItem, grid.CurrentColumn);
            grid.BeginEdit();
            if (grid.CurrentCell == null
                || grid.CurrentCell.Item == null
                || grid.CurrentCell.Column.GetCellContent(grid.CurrentCell.Item) == null
                || grid.CurrentCell.Column.GetCellContent(grid.CurrentCell.Item).Parent == null) return;
            if (grid.CurrentCell.Column.GetCellContent(grid.CurrentCell.Item).Parent is DataGridCell)
            {
                DataGridCell mDataGridCell = grid.CurrentCell.Column.GetCellContent(grid.CurrentCell.Item).Parent as DataGridCell;
                mDataGridCell.IsEditing = true;
                if (mDataGridCell.GetChildrenByType<Control>() == null || mDataGridCell.GetChildrenByType<Control>().Count == 0) return;
                Control mControl = mDataGridCell.GetChildrenByType<Control>().FirstOrDefault();
                if (mControl != null) mControl.Focus();
            }

        }
        public static void NextOrPrevRowSelectColumnToEdit(this DataGrid theGrid, int nSelColIdx, bool bNextRow)
        {
            if (bNextRow && theGrid.IsLastRow())    // Already at Last Row
                return;
            if (bNextRow == false && theGrid.SelectedIndex == 0)    // Already at Top Row
                return;
            if (theGrid.CurrentColumn == null)
                return;
            int nNextOrPrevRow = (bNextRow ? 1 : -1);
            int nextRowIdx = theGrid.SelectedIndex + nNextOrPrevRow;
            theGrid.SelectedItem = theGrid.Items[nextRowIdx];
            theGrid.ScrollIntoView(theGrid.Items[nextRowIdx]);
            DataGridRow theRow = (DataGridRow)theGrid.ItemContainerGenerator.ContainerFromItem(theGrid.Items[nextRowIdx]);
            theRow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            if (theGrid.CurrentColumn.DisplayIndex != nSelColIdx)
            {
                theGrid.CurrentColumn = theGrid.Columns[nSelColIdx];
                theGrid.ScrollIntoView(theGrid.SelectedItem, theGrid.CurrentColumn);
            }
            theGrid.BeginEdit();

            if (theGrid.CurrentCell == null || theGrid.CurrentCell.Item == null
                                || theGrid.CurrentCell.Column.GetCellContent(theGrid.CurrentCell.Item) == null
                                || theGrid.CurrentCell.Column.GetCellContent(theGrid.CurrentCell.Item).Parent == null)
            {
                return;
            }
            if (theGrid.CurrentCell.Column.GetCellContent(theGrid.CurrentCell.Item).Parent is DataGridCell)
            {
                DataGridCell mDataGridCell = theGrid.CurrentCell.Column.GetCellContent(theGrid.CurrentCell.Item).Parent as DataGridCell;
                mDataGridCell.IsEditing = true;
                if (mDataGridCell.GetChildrenByType<Control>() == null || mDataGridCell.GetChildrenByType<Control>().Count == 0)
                    return;
                Control mControl = mDataGridCell.GetChildrenByType<Control>().FirstOrDefault();
                if (mControl != null)
                    mControl.Focus();
            }
        }

        public static void NextRowSelectColumnToEdit(this DataGrid theGrid, int nSelColIdx)
        {
            if (theGrid.IsLastRow())
                return;
            int nextRowIdx = theGrid.SelectedIndex + 1;
            theGrid.SelectedItem = theGrid.Items[nextRowIdx];
            theGrid.ScrollIntoView(theGrid.Items[nextRowIdx]);
            DataGridRow theRow = (DataGridRow)theGrid.ItemContainerGenerator.ContainerFromItem(theGrid.Items[nextRowIdx]);
            theRow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                                
            if (theGrid.CurrentColumn.DisplayIndex != nSelColIdx)
            {
                theGrid.CurrentColumn = theGrid.Columns[nSelColIdx];
                theGrid.ScrollIntoView(theGrid.SelectedItem, theGrid.CurrentColumn);
            }
            theGrid.BeginEdit();

            if (theGrid.CurrentCell == null || theGrid.CurrentCell.Item == null
                                || theGrid.CurrentCell.Column.GetCellContent(theGrid.CurrentCell.Item) == null
                                || theGrid.CurrentCell.Column.GetCellContent(theGrid.CurrentCell.Item).Parent == null)
            {
                return;
            }
            if (theGrid.CurrentCell.Column.GetCellContent(theGrid.CurrentCell.Item).Parent is DataGridCell)
            {
                DataGridCell mDataGridCell = theGrid.CurrentCell.Column.GetCellContent(theGrid.CurrentCell.Item).Parent as DataGridCell;
                mDataGridCell.IsEditing = true;
                if (mDataGridCell.GetChildrenByType<Control>() == null || mDataGridCell.GetChildrenByType<Control>().Count == 0)
                    return;
                Control mControl = mDataGridCell.GetChildrenByType<Control>().FirstOrDefault();
                if (mControl != null)
                    mControl.Focus();
            }
        }
        public static void CellAfterNextToEdit(this DataGrid grid)
        {
            if (grid.SelectedItem != null)
            {
                int nextColumnIndex = GetNextAvailableColumnIndex(grid);
                grid.CurrentColumn = grid.Columns[nextColumnIndex + 1];
                grid.ScrollIntoView(grid.SelectedItem, grid.CurrentColumn);
                grid.BeginEdit();
                if (grid.CurrentCell == null
                    || grid.CurrentCell.Item == null
                    || grid.CurrentCell.Column.GetCellContent(grid.CurrentCell.Item) == null
                    || grid.CurrentCell.Column.GetCellContent(grid.CurrentCell.Item).Parent == null) return;
                if (grid.CurrentCell.Column.GetCellContent(grid.CurrentCell.Item).Parent is DataGridCell)
                {
                    DataGridCell mDataGridCell = grid.CurrentCell.Column.GetCellContent(grid.CurrentCell.Item).Parent as DataGridCell;
                    mDataGridCell.IsEditing = true;
                    if (mDataGridCell.GetChildrenByType<Control>() == null || mDataGridCell.GetChildrenByType<Control>().Count == 0) return;
                    Control mControl = mDataGridCell.GetChildrenByType<Control>().FirstOrDefault();
                    if (mControl != null) mControl.Focus();
                }
            }
        }
        public static void NextCellChoiceToEdit(this DataGrid grid, int nextColumnIndex)
        {
            if (grid.SelectedItem != null)
            {
                grid.BeginEdit();
                grid.CurrentColumn = grid.Columns[nextColumnIndex];
                grid.ScrollIntoView(grid.SelectedItem, grid.CurrentColumn);                
            }
        }
        static int GetNextAvailableColumnIndex(DataGrid grid)
        {
            return grid.GetNextAvailableColumn().DisplayIndex;
        }
        public static DataGridColumn GetNextAvailableColumn(this DataGrid grid)
        {
            int nextColumnIndex = grid.CurrentColumn.DisplayIndex + 1;
            if (nextColumnIndex >= grid.Columns.Count)
            {
                nextColumnIndex = 0;
            }

            while (grid.Columns[nextColumnIndex].IsReadOnly || grid.Columns[nextColumnIndex].Visibility == Visibility.Collapsed)
            {
                if (nextColumnIndex < grid.Columns.Count - 1)
                {
                    nextColumnIndex++;
                }
                else
                {
                    nextColumnIndex = 0;
                }
            }

            return grid.Columns[nextColumnIndex];
        }

        static int GetPrevAvailableColumnIndex(DataGrid grid)
        {
            return grid.GetPrevAvailableColumn().DisplayIndex;
        }
        public static DataGridColumn GetPrevAvailableColumn(this DataGrid grid)
        {
            int prevColumnIndex = grid.CurrentColumn.DisplayIndex - 1;
            if (prevColumnIndex < 0)
            {
                prevColumnIndex = 0;
            }

            while (grid.Columns[prevColumnIndex].IsReadOnly || grid.Columns[prevColumnIndex].Visibility == Visibility.Collapsed)
            {
                if (prevColumnIndex > 1)
                {
                    prevColumnIndex--;
                }
                else
                {
                    prevColumnIndex = 0;
                }
            }

            return grid.Columns[prevColumnIndex];
        }

        public static bool IsLastCell(this DataGrid grid)
        {

            int currentColumnIndex = grid.CurrentColumn.DisplayIndex;

            if (currentColumnIndex == grid.Columns.Count - 1)
            {
                return true;
            }
            else
            {
                //Check if exists an not readonly cell available
                for (int index = currentColumnIndex + 1; index <= grid.Columns.Count - 1; index++)
                {
                    if (!grid.Columns[index].IsReadOnly)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public static bool IsCurrentRow(this DataGrid grid,int oldPos)
        {
            return (grid.SelectedIndex == oldPos ? true : false);           
        }
        public static bool IsLastRow(this DataGrid grid)
        {
            if (grid.ItemsSource!=null)
            {
                PagedCollectionView source = new PagedCollectionView(grid.ItemsSource);

                if (source != null)
                {
                    return (grid.SelectedIndex == source.TotalItemCount - 1 ? true : false);
                }
                else
                {
                    return true;
                }    
            }
            else
            {
                return true;
            }
        }
        public static void SelectedItemAndBeginEdit(this AxDataGrid grid, object item, int ColumnIndexForEdit)
        {
            grid.UpdateLayout();
            grid.ScrollIntoView(item, (DataGridColumn)grid.Columns[ColumnIndexForEdit]);
            grid.SelectedItem = true;
            grid.Focus();
            grid.CurrentColumn = (DataGridColumn)grid.Columns[ColumnIndexForEdit];
            grid.BeginEdit();

        }
        public static bool IsValid(this DataGrid aDataGrid)
        {
            if (aDataGrid.ItemsSource != null)
            {
                var mType = aDataGrid.ItemsSource.Cast<object>().FirstOrDefault();
                if (mType == null) return true;
                var mErrorProperty = mType.GetType().GetProperty("HasErrors");
                if (mErrorProperty == null) return true;
                if (aDataGrid.ItemsSource.Cast<object>().Any(x => mErrorProperty.GetValue(x).Equals(true)))
                {
                    return false;
                }
            }
            return true;
        }
    }

    public delegate void RemoveEmptyRow(int index);

    public class TestConverter:IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SolidColorBrush(Color.FromArgb(125,255,0,0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    public class AxDataGrid : DataGrid
    {
        private int selectPos = -1;
        public int EditingIndex
        {
            get
            {
                return selectPos;
            }
        }

        private DataGridDoubleClickBehavior dblClickBehavior;

        private bool isEdit = false;
        public bool isButtonChoice = true;
        public bool isEscape= false;
        //public bool IsReadOnlyEx = false;

        public static readonly DependencyProperty IsReadOnlyExProperty = DependencyProperty.Register("IsReadOnlyEx",
            typeof(bool),
            typeof(AxDataGrid),
            new PropertyMetadata(false,null));

        public bool IsReadOnlyEx
        {
            get
            {
                return (bool)GetValue(IsReadOnlyExProperty);
            }
            set
            {
                SetValue(IsReadOnlyExProperty, value);
            }
        }


        public bool IsEditing
        {
            get
	        {
		        return isEdit && !IsReadOnlyEx; 
	        }
        }
        

        public AxDataGrid()
        {
            DefaultStyleKey = typeof(DataGrid);
            isEdit = false;
            selectPos = 0;
            
            _backgroundConverter = new AxDataGridRowBackgroundConverter(this);

            this.Unloaded += new RoutedEventHandler(AxDataGrid_Unloaded);
            this.Loaded += new RoutedEventHandler(AxDataGrid_Loaded);

            dblClickBehavior = new DataGridDoubleClickBehavior();
            System.Windows.Interactivity.Interaction.GetBehaviors(this).Add(dblClickBehavior);
            this.MouseDoubleClick += AxDataGrid_MouseDoubleClick;
        }
        private void AxDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DblClick != null && this.SelectedItem != null)
            {
                DblClick(sender, new EventArgs<object>(this.SelectedItem));
            }
        }

        void AxDataGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            //dblClickBehavior.DoubleClick -= new EventHandler<MouseButtonEventArgs>(dblClickBehavior_DoubleClick);
        }

        void AxDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //dblClickBehavior.DoubleClick += new EventHandler<MouseButtonEventArgs>(dblClickBehavior_DoubleClick);
        }
        public event EventHandler<EventArgs<object>> DblClick;
        void dblClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DblClick != null)
            {
                //DblClick(sender, new EventArgs<object>(((DataGridRow)sender).DataContext));
            }
        }

        protected override void OnCurrentCellChanged(EventArgs e)
        {
            this.OnCurrentCellChanged();
            base.OnCurrentCellChanged(e);
        }

        public RemoveEmptyRow RemoveEmptyRow;

        protected override void OnRowEditEnding(DataGridRowEditEndingEventArgs e)
        {
            if (!isButtonChoice )
            { 
                isEdit = false; 
            }
            isButtonChoice = true;
            IEntityState curObject = e.Row.DataContext as IEntityState;
            if(e.EditAction == DataGridEditAction.Cancel)
            {
                //Kiểm tra nếu item này mới được add vào, và liền kề bên dưới còn 1 item nào thì remove item này đi.
                //IEntityState curObject = e.Row.DataContext as IEntityState;
                if(curObject != null)
                {
                    if(curObject.EntityState == EntityState.NEW)
                    {
                        if (RemoveEmptyRow != null)
                        {
                            RemoveEmptyRow(e.Row.GetIndex());
                        }
                    }
                }
            }
            else //Commit
            {
                if(curObject != null)
                {
                    switch (curObject.EntityState)
                    {
                        case EntityState.NEW:
                            curObject.EntityState = EntityState.DETACHED;
                            break;
                        case EntityState.DETACHED:
                            break;
                        case EntityState.PERSITED:
                            curObject.EntityState = EntityState.MODIFIED;
                            break;
                        case EntityState.MODIFIED:
                            break;
                        case EntityState.DELETED_MODIFIED:
                            break;
                    }
                }
            }
            base.OnRowEditEnding(e);
        }

        public void AddRecord() 
        {
            isEdit = true;
            PagedCollectionView source = new PagedCollectionView(this.ItemsSource);
            selectPos = source.TotalItemCount-1 ;
            
        }
        public void EditRecord()
        {
            isEdit = true;
        }
        
        public void Cancel()
        {
            isEdit = false;
        }
        public void SaveRecord()
        {
            isEdit = false;
        }
        public void DeleteRecord()
        {
            
        }
        
        /// <summary>
        /// Cho phep edit ngay tai dong co chi so rowIndex
        /// </summary>
        /// <param name="rowIndex"></param>
        public void EnterEdit(int rowIndex)
        {
            isEdit = true;
            if(CheckIfRowIndexIsInValidRange(rowIndex))
            {
                selectPos = rowIndex;
            }
            else
            {
                throw new Exception("grid row index out of range.");
            }
        }

        /// <summary>
        /// Cho phep edit ngay tai dong co chi so rowIndex
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="startEdit">Chuyen sang che do edit lien tai cho. Khoi can click vao 1 cell</param>
        public void EnterEdit(int rowIndex, bool startEdit = false)
        {
            EnterEdit(rowIndex);
            if (startEdit)
            {
                this.NextCellToEdit();
            } 
        }

        public bool CheckIfRowIndexIsInValidRange(int rowIndex)
        {
            return rowIndex >= 0 && rowIndex < GetTotalItems();
        }
        private int GetTotalItems()
        {
            if(this.ItemsSource == null)
            {
                return 0;
            }
            int total = 0;
            IEnumerator enumerator = this.ItemsSource.GetEnumerator();
            while (enumerator.MoveNext())
            {
                total++;
            }
            return total;
        }
        public void OnCurrentCellChanged() 
        {
            try
            {
                PagedCollectionView source = new PagedCollectionView(this.ItemsSource);

                if (this.SelectedIndex == source.TotalItemCount - 1)
                {
                    selectPos = source.TotalItemCount - 1;
                    this.isEdit = true;
                }
                //kiem tra lien luon
                //if (this.isEdit)
                if (this.IsEditing)
                {
                    if (this.IsCurrentRow(selectPos))
                    {
                        this.IsReadOnly = false;
                    }
                    else this.IsReadOnly = true;
                }
                else
                {
                    this.IsReadOnly = true;
                    selectPos = this.SelectedIndex;
                }
                
                                
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogError(ex.ToString());
            }
        }
        
        protected override void OnKeyUp(KeyEventArgs e)
        {
            //if (e.Key.Equals(Key.Tab) || e.Key.Equals(Key.Enter))
            //{
            //    e.Handled = true;

            //    if (CurrentColumn != null)
            //    {
            //        if (this.IsLastRow() & this.IsLastCell()) // Last Row And Last Cell (End of the grid)
            //        {
            //            CommitEdit();
            //            SelectedIndex = SelectedIndex + 1;
            //            this.NextCellToEdit();
            //        }
            //        else if (this.IsLastCell()) // Last Cell
            //        {
            //            SelectedIndex = SelectedIndex + 1;
            //            this.NextCellToEdit();

            //        }
            //        else // Normal navigation
            //        {
            //            this.NextCellToEdit();
            //        }

            //    }
            //}
            //else
            {
                base.OnKeyUp(e);
            }

        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Tab) || e.Key.Equals(Key.Enter))
            {
                e.Handled = true;
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        protected override void OnLoadingRow(DataGridRowEventArgs e)
        {
            Binding backgroundBinding = new Binding();
            backgroundBinding.Mode = BindingMode.OneWay;
            backgroundBinding.Path = new PropertyPath("EntityState");

            backgroundBinding.Converter = _backgroundConverter;
            e.Row.SetBinding(DataGridRow.BackgroundProperty, backgroundBinding);

            base.OnLoadingRow(e);
        }

        /// <summary>
        /// Load lai luoi, vi khi set binding cho background no khong ep phe. (Chua biet vi sao)
        /// </summary>
        public void RefreshGrid()
        {
            this.OnApplyTemplate();
        }

        public Brush MarkAsDeletedRowBackground
        {
            get
            {
                return (Brush)GetValue(MarkAsDeletedRowBackgroundProperty);
            }
            set
            {
                SetValue(MarkAsDeletedRowBackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty MarkAsDeletedRowBackgroundProperty = DependencyProperty.Register("MarkAsDeletedRowBackground",
            typeof(Brush),
            typeof(AxDataGrid),
            new PropertyMetadata(null, null));


        public Brush ReadOnlyRowBackground
        {
            get
            {
                return (Brush)GetValue(ReadOnlyRowBackgroundProperty);
            }
            set
            {
                SetValue(ReadOnlyRowBackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty ReadOnlyRowBackgroundProperty = DependencyProperty.Register("ReadOnlyRowBackground",
            typeof(Brush),
            typeof(AxDataGrid),
            new PropertyMetadata(null, null));


        public Brush NewlyAddedRowBackground
        {
            get
            {
                return (Brush)GetValue(NewlyAddedRowBackgroundProperty);
            }
            set
            {
                SetValue(NewlyAddedRowBackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty NewlyAddedRowBackgroundProperty = DependencyProperty.Register("NewlyAddedRowBackground",
            typeof(Brush),
            typeof(AxDataGrid),
            new PropertyMetadata(null, null));

        public Brush UpdatedRowBackground
        {
            get
            {
                return (Brush)GetValue(UpdatedRowBackgroundProperty);
            }
            set
            {
                SetValue(UpdatedRowBackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty UpdatedRowBackgroundProperty = DependencyProperty.Register("UpdatedRowBackground",
            typeof(Brush),
            typeof(AxDataGrid),
            new PropertyMetadata(null, null));


        public Brush NormalRowBackground
        {
            get
            {
                return (Brush)GetValue(NormalRowBackgroundProperty);
            }
            set
            {
                SetValue(NormalRowBackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty NormalRowBackgroundProperty = DependencyProperty.Register("NormalRowBackground",
            typeof(Brush),
            typeof(AxDataGrid),
            new PropertyMetadata(null, null));

        private AxDataGridRowBackgroundConverter _backgroundConverter;

        private class AxDataGridRowBackgroundConverter : IValueConverter
        {
            public AxDataGridRowBackgroundConverter(AxDataGrid parent)
            {
                this._parent = parent;
            }
            private AxDataGrid _parent;
            #region IValueConverter Members

            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                //IEntityState state = value as IEntityState;
                EntityState state = (EntityState)value;
                switch (state)
                {
                    case EntityState.DELETED_PERSITED:
                    case EntityState.DELETED_MODIFIED:
                        return _parent.MarkAsDeletedRowBackground;

                    case EntityState.NEW:
                        return _parent.NewlyAddedRowBackground;

                    case EntityState.PERSITED:
                    case EntityState.DETACHED:
                        return _parent.NormalRowBackground;

                    case EntityState.MODIFIED:
                        return _parent.UpdatedRowBackground;

                    default:
                        return null;
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return null;
            }

            #endregion
        }

    }
    
}
