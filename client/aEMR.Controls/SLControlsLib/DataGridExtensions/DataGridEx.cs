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
//using System.Linq;
using Service.Core.Common;
//using System.Collections.Generic;
//using eHCMS.Common.Collections;
//using eHCMS.Services.Core.Base;
using aEMR.Common.PagedCollectionView;
using aEMR.Common;

namespace aEMR.Controls
{
    
    public delegate void RemoveEmptyRowEx(int index);

    public class TestConverterEx : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new SolidColorBrush(Color.FromArgb(125, 255, 0, 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }

    public class AxDataGridEx : DataGrid
    {
        private int selectPos = -1;
        private bool isEdit = false;
        public bool isButtonChoice = true;
        public bool isEscape = false;
        public AxDataGridEx()
        {
            DefaultStyleKey = typeof(DataGrid);
            isEdit = false;
            selectPos = 0;
            _backgroundConverter = new AxDataGridExRowBackgroundConverter(this);
            this.CanUserAddRows = false;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (//e.Key.Equals(Key.Tab) || 
                e.Key.Equals(Key.Enter))
            {
                e.Handled = true;
                if (CurrentColumn != null)
                {

                    if (this.IsLastRow() & this.IsLastCell()) // Last Row And Last Cell (End of the grid)
                    {
                        CommitEdit();
                        SelectedIndex = SelectedIndex + 1;
                        this.NextCellToEdit();

                    }
                    else if (this.IsLastCell()) // Last Cell
                    {
                        SelectedIndex = SelectedIndex + 1;
                        this.NextCellToEdit();
                    }
                    else // Normal navigation
                    {
                        this.NextCellToEdit();
                    }

                }
            }
            else
            {
                base.OnKeyUp(e);
            }

        }

        protected override void OnCurrentCellChanged(EventArgs e)
        {
            this.OnCurrentCellChanged();
            base.OnCurrentCellChanged(e);
        }
        
        public RemoveEmptyRowEx RemoveEmptyRow;

        protected override void OnRowEditEnding(DataGridRowEditEndingEventArgs e)
        {
            if (!isButtonChoice)
            {
                isEdit = false;
            }
            isButtonChoice = true;
            IEntityState curObject = e.Row.DataContext as IEntityState;
            if (e.EditAction == DataGridEditAction.Cancel)
            {
                //Kiểm tra nếu item này mới được add vào, và liền kề bên dưới còn 1 item nào thì remove item này đi.
                //IEntityState curObject = e.Row.DataContext as IEntityState;
                if (curObject != null)
                {
                    if (curObject.EntityState == EntityState.NEW)
                    {
                        if (RemoveEmptyRow != null)
                        {
                            RemoveEmptyRow(e.Row.GetIndex());
                        }
                        //BlankRowCollection<EntityBase> allItems = (BlankRowCollection<EntityBase>)this.ItemsSource;
                        //int total = allItems.Count;
                        //if(e.Row.GetIndex() + 1 < total)
                        //{

                        //}
                    }
                }
            }
            else //Commit
            {
                if (curObject != null)
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

            //this.OnApplyTemplate();
        }

        public void AddRecord()
        {
            isEdit = true;
            PagedCollectionView source = new PagedCollectionView(this.ItemsSource);
            selectPos = source.TotalItemCount - 1;

        }
        public void EditRecord()
        {
            isEdit = true;
            selectPos = this.SelectedIndex;
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
            if (CheckIfRowIndexIsInValidRange(rowIndex))
            {
                selectPos = rowIndex;
            }
            else
            {
                throw new Exception("grid row index out of range.");
            }
        }
        private bool CheckIfRowIndexIsInValidRange(int rowIndex)
        {
            return rowIndex >= 0 && rowIndex < GetTotalItems();
        }
        private int GetTotalItems()
        {
            if (this.ItemsSource == null)
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
                if (this.isEdit)
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
        public bool isLastRow()
        {
            return this.IsLastRow();
        }
        public int TotalItem()
        {
            return this.GetTotalItems();
        }
        protected override void OnKeyDown(KeyEventArgs e)
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
            //{
            //    base.OnKeyDown(e);
            //}

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
            typeof(AxDataGridEx),
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
            typeof(AxDataGridEx),
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
            typeof(AxDataGridEx),
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
            typeof(AxDataGridEx),
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
            typeof(AxDataGridEx),
            new PropertyMetadata(null, null));

        private AxDataGridExRowBackgroundConverter _backgroundConverter;

        private class AxDataGridExRowBackgroundConverter : IValueConverter
        {
            public AxDataGridExRowBackgroundConverter(AxDataGridEx parent)
            {
                this._parent = parent;
            }
            private AxDataGridEx _parent;
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
