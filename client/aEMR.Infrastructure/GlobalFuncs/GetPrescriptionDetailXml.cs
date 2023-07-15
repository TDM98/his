using eHCMSLanguage;
using System;
using System.Windows;
using System.Windows.Controls;
using DataEntities;
using System.Xml.Linq;
using System.Linq;

namespace aEMR.Infrastructure.GlobalFuncs
{
    public class ScheduleSimple
    {
        public string T { get; set; }
        public string T2 { get; set; }
        public string T3 { get; set; }
        public string T4 { get; set; }
        public string T5 { get; set; }
        public string T6 { get; set; }
        public string T7 { get; set; }
        public string CN { get; set; }
    }
    public class GetPrescriptionDetailXml
    {
        private string GetPeriodOfDay(object value)
        {
            if (Convert.ToInt64(value) == Convert.ToInt64(AllLookupValues.V_PeriodOfDay.SANG))
            {
                return eHCMSResources.S0540_G1_Sang;
            }
            else if (Convert.ToInt64(value) == Convert.ToInt64(AllLookupValues.V_PeriodOfDay.TRUA))
            {
                return eHCMSResources.G1808_G1_Trua;
            }
            else if (Convert.ToInt64(value) == Convert.ToInt64(AllLookupValues.V_PeriodOfDay.CHIEU))
            {
                return eHCMSResources.K1860_G1_Chieu;
            }
            else
            {
                return eHCMSResources.G1446_G1_Toi;
            }
        }
        public void getPendingClientGrid(StackPanel PendingClientsGrid, Prescription curPrescription)
        {
            if (PendingClientsGrid == null || curPrescription ==null)
            {
                return;
            }

            if(string.IsNullOrEmpty(curPrescription.PrescriptDetailsStr))
            {
                if (PendingClientsGrid.Children != null) 
                {
                    PendingClientsGrid.Children.Clear();
                }                
                return;
            }
            PendingClientsGrid.Children.Clear();
            XDocument oDoc = XDocument.Parse(curPrescription.PrescriptDetailsStr);
            var myData = from info in oDoc.Element("Prescription").Elements("PrescriptionDetails")//.Elements("PrescriptionDetailSchedules").Elements("PrescriptionDetailSchedules")
                         select new
                       {
                           BrandName = Convert.ToString(info.Element("BrandName").Value),
                           UnitName = Convert.ToString(info.Element("UnitName").Value),
                           Administration = Convert.ToString(info.Element("Administration").Value),
                           DrugInstructionNotes =info.Element("DrugInstructionNotes").Value,
                           DayRpts = Convert.ToDecimal(info.Element("DayRpts").Value),
                           ADoseStr = Convert.ToString(info.Element("ADoseStr").Value),
                           EDoseStr = Convert.ToString(info.Element("EDoseStr").Value),
                           MDoseStr = Convert.ToString(info.Element("MDoseStr").Value),
                           NDoseStr = Convert.ToString(info.Element("NDoseStr").Value),
                           Qty = Convert.ToDecimal(info.Element("Qty").Value),
                           V_DrugType= Convert.ToInt64(info.Element("V_DrugType").Value),
                           UnitNameUse = Convert.ToString(info.Element("UnitNameUse").Value),
                           PrescriptionDetailSchedules = info.Elements("PrescriptionDetailSchedules").Elements("PrescriptionDetailSchedules")
                       };

            int count = 0;
            foreach (var item in myData.ToList())
            {
                count++;
                StackPanel StackPanel1 = new StackPanel();
                StackPanel1.Orientation = Orientation.Horizontal;
                TextBlock l1 = new TextBlock();
                l1.Text = count.ToString() + ". ";
                l1.Style = (Style)Application.Current.Resources["TBlock.Content.00"];
                StackPanel1.Children.Add(l1);

                TextBlock l2 = new TextBlock();
                l2.Text = item.BrandName;
                l2.Style = (Style)Application.Current.Resources["TBlock.Content.00"];
                StackPanel1.Children.Add(l2);

                TextBlock l3 = new TextBlock();
                l3.Text = "( " + item.Qty.ToString("#,#.##") + " " + item.UnitName + " )";
                l3.Margin = new Thickness(10, 0, 0, 0);
                l3.Style = (Style)Application.Current.Resources["TBlock.Content.00"];
                StackPanel1.Children.Add(l3);

                PendingClientsGrid.Children.Add(StackPanel1);

                if (item.PrescriptionDetailSchedules != null && item.PrescriptionDetailSchedules.Count() > 0)
                {
                    var itemsource = from info in item.PrescriptionDetailSchedules
                                     select new ScheduleSimple
                                     {
                                         T = GetPeriodOfDay(info.Element("V_PeriodOfDay").Value),
                                         T2 = Convert.ToString(info.Element("MondayStr").Value),
                                         T3 = Convert.ToString(info.Element("TuesdayStr").Value),
                                         T4 = Convert.ToString(info.Element("WednesdayStr").Value),
                                         T5 = Convert.ToString(info.Element("ThursdayStr").Value),
                                         T6 = Convert.ToString(info.Element("FridayStr").Value),
                                         T7 = Convert.ToString(info.Element("SaturdayStr").Value),
                                         CN = Convert.ToString(info.Element("SundayStr").Value)

                                     };

                    if (itemsource != null && itemsource.Count() > 0)
                    {
                        Grid panelGrid = new Grid();

                        RowDefinition rowDef1 = new RowDefinition();
                        panelGrid.RowDefinitions.Add(rowDef1);


                        DataGrid dataGridFirst = new DataGrid();
                        dataGridFirst.AutoGenerateColumns = true;
                        dataGridFirst.HeadersVisibility = DataGridHeadersVisibility.Column;
                        dataGridFirst.HorizontalAlignment = HorizontalAlignment.Stretch;
                        dataGridFirst.VerticalAlignment = VerticalAlignment.Center;
                        dataGridFirst.IsReadOnly = true;
                        dataGridFirst.Style = (Style)Application.Current.Resources["MainFrame.DataGridStyle.OneColor"];
                        dataGridFirst.ColumnHeaderStyle = (Style)Application.Current.Resources["MainFrame.DataGridColumnHeaderStyle"];

                        dataGridFirst.ItemsSource = itemsource;

                        Grid.SetRow(dataGridFirst, 0);
                        panelGrid.Children.Add(dataGridFirst);
                        PendingClientsGrid.Children.Add(panelGrid);
                    }

                }
                else
                {
                    Grid StackPanel2 = new Grid();
                    ColumnDefinition Col1 = new ColumnDefinition();
                    Col1.Width = new GridLength(120);
                    ColumnDefinition Col2 = new ColumnDefinition();
                    Col2.Width = new GridLength(120);
                    ColumnDefinition Col3 = new ColumnDefinition();
                    Col3.Width = new GridLength(120);
                    ColumnDefinition Col4 = new ColumnDefinition();
                    Col4.Width = new GridLength(120);

                    StackPanel2.ColumnDefinitions.Add(Col1);
                    StackPanel2.ColumnDefinitions.Add(Col2);
                    StackPanel2.ColumnDefinitions.Add(Col3);
                    StackPanel2.ColumnDefinitions.Add(Col4);

                    if (!string.IsNullOrEmpty(item.MDoseStr) && item.MDoseStr != "0")
                    {
                        TextBlock l22 = new TextBlock();
                        l22.Text = string.Format("{0} : ", eHCMSResources.S0540_G1_Sang) + item.MDoseStr + " " + item.UnitNameUse + ".";
                        l22.Margin = new Thickness(10, 0, 0, 0);
                        l22.Style = (Style)Application.Current.Resources["TBlock.Content.00"];
                        Grid.SetColumn(l22, 0);
                        StackPanel2.Children.Add(l22);
                    }
                    if (!string.IsNullOrEmpty(item.NDoseStr) && item.NDoseStr != "0")
                    {
                        TextBlock l23 = new TextBlock();
                        l23.Text = string.Format("{0} : ", eHCMSResources.G1808_G1_Trua) + item.NDoseStr + " " + item.UnitNameUse + ".";
                        l23.Margin = new Thickness(20, 0, 0, 0);
                        l23.Style = (Style)Application.Current.Resources["TBlock.Content.00"];
                        Grid.SetColumn(l23, 1);
                        StackPanel2.Children.Add(l23);
                    }
                    if (!string.IsNullOrEmpty(item.ADoseStr) && item.ADoseStr != "0")
                    {
                        TextBlock l24 = new TextBlock();
                        l24.Text = string.Format("{0} : ", eHCMSResources.K1860_G1_Chieu) + item.ADoseStr + " " + item.UnitNameUse + ".";
                        l24.Margin = new Thickness(20, 0, 0, 0);
                        l24.Style = (Style)Application.Current.Resources["TBlock.Content.00"];
                        Grid.SetColumn(l24, 2);
                        StackPanel2.Children.Add(l24);
                    }
                    if (!string.IsNullOrEmpty(item.EDoseStr) && item.EDoseStr != "0")
                    {
                        TextBlock l25 = new TextBlock();
                        l25.Text = string.Format("{0} : ", eHCMSResources.G1446_G1_Toi) + item.EDoseStr + " " + item.UnitNameUse + ".";
                        l25.Margin = new Thickness(20, 0, 0, 0);
                        l25.Style = (Style)Application.Current.Resources["TBlock.Content.00"];
                        Grid.SetColumn(l25, 3);
                        StackPanel2.Children.Add(l25);
                    }
                    
                    PendingClientsGrid.Children.Add(StackPanel2);
                }

                StackPanel StackPanel3 = new StackPanel();
                StackPanel3.Orientation = Orientation.Horizontal;
                TextBlock l21 = new TextBlock();
                string str = "";
                if (!string.IsNullOrEmpty(item.DrugInstructionNotes))
                {
                    str = "( " + item.DrugInstructionNotes + " )";
                }
                l21.Text = string.Format("{0} trong {1} ngày.", item.Administration, item.DayRpts.ToString("#,#.##")) + str;
                l21.Margin = new Thickness(10, 0, 0, 0);
                l21.Style = (Style)Application.Current.Resources["TBlock.Content.00"];
                StackPanel3.Children.Add(l21);
                PendingClientsGrid.Children.Add(StackPanel3);
            }
        }
    }
}