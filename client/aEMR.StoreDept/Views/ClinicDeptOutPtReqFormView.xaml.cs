using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace aEMR.StoreDept.Views
{
    public partial class ClinicDeptOutPtReqFormView : UserControl
    {
        //List<TestData> list;
        //Dictionary<int, bool> selectedValues = new Dictionary<int, bool>();

        public ClinicDeptOutPtReqFormView()
        {
            InitializeComponent();
            //list = new List<TestData>();
            //for (int i = 0; i < 100; i++)
            //{
            //    list.Add(new TestData() { Text = "group " + i % 10, Number = i });
            //    if (i % 3 == 0)
            //        list[i].Number = null;
            //}
            //grid.ItemsSource = list;
        }

      
        //int emptyCellsTotalCount = 0;
        //int emptyCellsGroupCount = 0;

        //void grid_CustomSummary(object sender, CustomSummaryEventArgs e)
        //{
        //    if (((GridSummaryItem)e.Item).FieldName != "Number")
        //        return;
        //    if (e.IsTotalSummary)
        //    {
        //        if (e.SummaryProcess == CustomSummaryProcess.Start)
        //        {
        //            emptyCellsTotalCount = 0;
        //        }
        //        if (e.SummaryProcess == CustomSummaryProcess.Calculate)
        //        {
        //            int? val = (int?)e.FieldValue;
        //            if (!val.HasValue)
        //                emptyCellsTotalCount++;
        //            e.TotalValue = emptyCellsTotalCount;
        //        }
        //    }
        //    if (e.IsGroupSummary)
        //    {
        //        if (e.SummaryProcess == CustomSummaryProcess.Start)
        //        {
        //            emptyCellsGroupCount = 0;
        //        }
        //        if (e.SummaryProcess == CustomSummaryProcess.Calculate)
        //        {
        //            int? val = (int?)e.FieldValue;
        //            if (val.HasValue)
        //                emptyCellsGroupCount+=val.GetValueOrDefault();
        //            e.TotalValue = emptyCellsGroupCount;
        //        }
        //    }

        //}
        //public class TestData
        //{
        //    public string Text { get; set; }
        //    public int? Number { get; set; }
        //}
    }
}
