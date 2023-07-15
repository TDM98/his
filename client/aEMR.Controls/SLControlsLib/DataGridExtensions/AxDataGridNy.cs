using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Controls
{
    public interface IAXUserControl
    {
        bool IsLastControlEditing { get; }
    }
    public class AxDataGridNy : DataGrid
    {
        public bool isEscape = false;
        public AxDataGridNy()
        {
            DefaultStyleKey = typeof(DataGrid);
            this.CanUserAddRows = false;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Tab) || e.Key.Equals(Key.Enter) || e.Key.Equals(Key.Down) || e.Key.Equals(Key.Up))
            {
                e.Handled = true;
                try
                {
                    var CurrentDataGridCell = CurrentColumn.GetCellContent(CurrentItem).Parent as DataGridCell;
                    if (CurrentDataGridCell.Content != null && CurrentDataGridCell.Content is ContentPresenter && (CurrentDataGridCell.Content as ContentPresenter).ContentTemplate != null)
                    {
                        var CurrentCellContentTemplate = (CurrentDataGridCell.Content as ContentPresenter).ContentTemplate as DataTemplate;
                        if (CurrentCellContentTemplate.LoadContent() is IAXUserControl)
                        {
                            var UCDateTimeHourPicker = (CurrentDataGridCell.Content as ContentPresenter).GetChildrenByType<UserControl>().FirstOrDefault();
                            if (UCDateTimeHourPicker is IAXUserControl && !(UCDateTimeHourPicker as IAXUserControl).IsLastControlEditing)
                            {
                                if (Keyboard.FocusedElement != null && Keyboard.FocusedElement is Control)
                                {
                                    (Keyboard.FocusedElement as Control).MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.Next));
                                }
                                return;
                            }
                        }
                    }
                }
                catch { }
                if (CurrentColumn != null)
                {

                    if (this.IsLastRow() & this.IsLastCell()) // Last Row And Last Cell (End of the grid)
                    {
                        CommitEdit();
                        SelectedIndex = SelectedIndex + 1;
                        
                        this.NextCellToEdit();

                    }
                    else if (e.Key.Equals(Key.Down))
                    {
                        this.BeginEdit();
                        
                        // TxD 11/07/2018: In the middle of fixing, to be continued ....
                        //int curRowIdx = this.Items.IndexOf(this.CurrentItem);
                        //this.CurrentCell = new DataGridCellInfo(SelectedItem, this.CurrentCell.Column);

                        //this.NextCellToEdit();
                        //var u = e.OriginalSource as UIElement;
                        //u.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                        //this.BeginEdit();
                    }
                    else if (e.Key.Equals(Key.Up))
                    {
                        this.BeginEdit();

                        // TxD 11/07/2018: In the middle of fixing, to be continued ....
                        //int curRowIdx = this.Items.IndexOf(this.CurrentItem);
                        //this.CurrentCell = new DataGridCellInfo(SelectedItem, this.CurrentCell.Column);

                        //this.NextCellToEdit();
                        //var u = e.OriginalSource as UIElement;
                        //u.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
                        //this.BeginEdit();
                    }
                    else if (this.IsLastCell()) // Last Cell
                    {
                        this.NextRowSelectColumnToEdit(0);
                    }
                    else // Normal navigation
                    {
                        this.NextCellToEdit();                        
                    }
                    if (CurrentColumn.GetCellContent(CurrentItem) != null &&
                        CurrentColumn.GetCellContent(CurrentItem).Parent != null &&
                        (CurrentColumn.GetCellContent(CurrentItem).Parent is DataGridCell) &&
                        !(CurrentColumn.GetCellContent(CurrentItem).Parent as DataGridCell).IsTabStop)
                    {
                        if (this.IsLastCell())
                        {
                            this.NextRowSelectColumnToEdit(0);
                        }
                        else
                        {
                            this.NextCellToEdit();
                        }
                    }
                }
            }            
            else
            {
                base.OnKeyUp(e);
            }

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
            if (e.Key.Equals(Key.Tab) || e.Key.Equals(Key.Enter))
            {
                e.Handled = true;
            }
        }

        public bool IsValid
        { get { return true; } }
    }

    public class EmrPrescriptionGrid : DataGrid
    {
        public int nFirstEditIdx = 0;       // Fisrt Column index to Begin Edit after changing to Next Row
        public int nLastEditIdx = 1;        // Last Editable Column of a Row
        public bool isEscape = false;
        public EmrPrescriptionGrid()
        {
            DefaultStyleKey = typeof(DataGrid);
            this.CanUserAddRows = false;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key.Equals(Key.End))
            {
                e.Handled = true;
                if (CurrentColumn != null)
                {
                    if (CurrentColumn.DisplayIndex >= nFirstEditIdx && CurrentColumn.DisplayIndex < nLastEditIdx)
                    {
                        this.LastCellToEdit(nLastEditIdx);
                    }
                }
            }
            else if (e.Key.Equals(Key.Tab) || e.Key.Equals(Key.Enter))
            {
                e.Handled = true;
                if (CurrentColumn != null)
                {
                    if (CurrentColumn.DisplayIndex == nLastEditIdx)
                    {
                        this.NextOrPrevRowSelectColumnToEdit(nFirstEditIdx, true);
                    }
                    else // Normal navigation
                    {
                        this.NextOrPrevCellToEdit(true);
                    }
                }
            }
            else if (e.Key.Equals(Key.Down))
            {
                if (CurrentColumn.DisplayIndex != 5 && CurrentColumn.DisplayIndex != 8 && CurrentColumn.DisplayIndex != 15 && CurrentColumn.DisplayIndex != 16)    // column with AutoCompleteBox or Combobox
                {
                    e.Handled = true;
                    if (CurrentColumn != null)
                    {
                        this.NextOrPrevRowSelectColumnToEdit(CurrentColumn.DisplayIndex, true);
                    }
                }
            }
            else if (e.Key.Equals(Key.Up))
            {
                if (CurrentColumn.DisplayIndex != 5 && CurrentColumn.DisplayIndex != 8 && CurrentColumn.DisplayIndex != 15 && CurrentColumn.DisplayIndex != 16)    // column with AutoCompleteBox or Combobox
                {
                    e.Handled = true;
                    if (CurrentColumn != null)
                    {
                        this.NextOrPrevRowSelectColumnToEdit(CurrentColumn.DisplayIndex, false);
                    }
                }
            }

            base.OnKeyUp(e);

        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // TxD 17/07/2018: We have to Handle Tab and Enter key this way for OnKeyUp to work properly
            //                 Because without the following block of code the: After a blank row was added in ViewModel and the Enter Key was pressed
            //                 then for some unknown reason Focus shifted to the newly added row then OnKeyUp is called so it's too late 
            //                 Also noted that without any handling of the Enter Key by default Enter key will shift focus downward to the row below
            if (e.Key.Equals(Key.Tab) || e.Key.Equals(Key.Enter))
            {
                e.Handled = true;
            }
        }

        public bool IsValid
        { get { return true; } }
    }

    public class AxDataGridNyICD10 : DataGrid
    {
        private enum Icd10GridColumnIdx { ColIcdCode = 0, ColIcdName = 1, ColMainIcd = 2, ColCongenital = 3, ColCurStatus = 4, ColDelete = 5 };
        public bool bIcd10CodeAcbPopulated = false;
        public bool isEscape = false;
        public AxDataGridNyICD10()
        {
            DefaultStyleKey = typeof(DataGrid);
            this.CanUserAddRows = false;
        }

        private bool isEndCell()
        {
            return this.CurrentColumn.DisplayIndex == this.Columns.Count-2;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Tab) || e.Key.Equals(Key.Enter))
            {
                e.Handled = true;
                if (CurrentColumn != null)
                {
                    int curColIdx = CurrentColumn.DisplayIndex;
                    if (curColIdx == (int)Icd10GridColumnIdx.ColCurStatus)
                    {
                        this.NextRowSelectColumnToEdit(0);
                    }
                    else if (curColIdx == (int)Icd10GridColumnIdx.ColIcdCode)
                    {
                        if (bIcd10CodeAcbPopulated)
                        {
                            bIcd10CodeAcbPopulated = false;
                            this.CellAfterNextToEdit();
                        }
                        else
                        {
                            this.NextCellToEdit();
                        }
                    }
                    else if (curColIdx == (int)Icd10GridColumnIdx.ColIcdName || curColIdx == (int)Icd10GridColumnIdx.ColMainIcd || curColIdx == (int)Icd10GridColumnIdx.ColCongenital)
                    {
                        this.NextCellToEdit();
                    }
                    if (CurrentColumn.GetCellContent(CurrentItem) != null &&
                        CurrentColumn.GetCellContent(CurrentItem).Parent != null &&
                        (CurrentColumn.GetCellContent(CurrentItem).Parent is DataGridCell) &&
                        !(CurrentColumn.GetCellContent(CurrentItem).Parent as DataGridCell).IsTabStop)
                    {
                        if (this.IsLastCell())
                        {
                            this.NextRowSelectColumnToEdit(0);
                        }
                        else
                        {
                            this.NextCellToEdit();
                        }
                    }
                }
            }
            else
            {
                base.OnKeyUp(e);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // TxD 17/07/2018: We have to Handle Tab and Enter key this way for OnKeyUp to work properly
            //                 Because without the following block of code the: After a blank row was added in ViewModel and the Enter Key was pressed
            //                 then for some unknown reason Focus shifted to the newly added row then OnKeyUp is called so it's too late 
            //                 Also noted that without any handling of the Enter Key by default Enter key will shift focus downward to the row below
            if (e.Key.Equals(Key.Tab) || e.Key.Equals(Key.Enter))
            {
                e.Handled = true;
            }
        }


    }
    public class DataGridPCL : AxDataGrid
    {
        public new bool isEscape = false;
        public DataGridPCL()
        {
            DefaultStyleKey = typeof(DataGrid);
            this.CanUserAddRows = false;
        }

        private bool isEndCell()
        {
            return this.CurrentColumn.DisplayIndex == this.Columns.Count ;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Tab) || e.Key.Equals(Key.Enter))
            {
                e.Handled = true;
                if (CheckIfRowIndexIsInValidRange(SelectedIndex+1))
                {
                    CommitEdit();
                    this.SelectedIndex = this.SelectedIndex + 1;
                    this.ScrollIntoView(this.SelectedIndex, this.CurrentColumn);
                    this.BeginEdit();
                    //this.NextCellToEdit();
                }
            }
            else
            {
                base.OnKeyUp(e);
            }

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
            if (e.Key.Equals(Key.Tab) || e.Key.Equals(Key.Enter))
            {
                e.Handled = true;
            }
        }    

    }
}
