using eHCMSLanguage;
//   Date  : 2010-10-21
//	 Author: info@loekvandenouweland.com
//

using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Collections.Generic;

namespace aEMR.Controls
{
    public class DataGrid2ExcelAction : TriggerAction<DataGrid>
    {
        private CultureInfo _cultureInfo = Thread.CurrentThread.CurrentUICulture;
        public DataGrid2ExcelAction()
        {
        }
        private object GetValue(object o, string path)
        {
            var index = path.LastIndexOf('.');

            if (index > 0)
            {
                var propPath = path.Substring(0, index);
                path = path.Substring(index + 1);
                o = GetValue(o, propPath);
            }

            if (o == null)
                return null;

            var property = o.GetType().GetProperty(path);

            var value = property.GetValue(o, null);

            return value;
        }


        protected override void Invoke(object o)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog objSFD = new Microsoft.Win32.SaveFileDialog()
                    {
                        DefaultExt = ".xls",
                        Filter = "Excel (*.xls)|*.xls",
                        FilterIndex = 1
                    };

                if (objSFD.ShowDialog() != true)
                {
                    return;
                }

                using (Stream s = objSFD.OpenFile())
                {
                    var dataGrid = AssociatedObject as DataGrid;
                    var headers = "";
                    foreach (var column in dataGrid.Columns)
                    {
                        headers += "<Cell ss:StyleID=\"OrangeStyle\"><Data ss:Type=\"String\">" + column.Header + "</Data></Cell>";
                    }

                    var data = "";
                    foreach (var row in dataGrid.ItemsSource)
                    {
                        data += "<Row>\n";
                        foreach (var column in dataGrid.Columns)
                        {
                            var binding = (Binding)((column as DataGridBoundColumn).Binding);
                            var path = binding.Path.Path;
                            var value = GetValue(row, path);
                            data += "<Cell><Data ss:Type=\"String\">" + value + "</Data></Cell>\n";
                        }
                        data += "</Row>\n";
                    }

                    string excelXML = xmlTemplate.Replace("[headers]", headers).Replace("[data]", data);

                    byte[] contents = Encoding.UTF8.GetBytes(excelXML);

                    s.Write(contents, 0, contents.Length);
                }

                MessageBox.Show("Xuất Excel thành công!");
            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi xuất excel!");
            }
        }


        //protected override void Invoke(object o)
        //{
        //    try
        //    {
        //        var dataGrid = AssociatedObject as DataGrid;
        //        var headers = "";
        //        foreach (var column in dataGrid.Columns)
        //        {
        //            headers += "<Cell ss:StyleID=\"OrangeStyle\"><Data ss:Type=\"String\">" + column.Header + "</Data></Cell>";
        //        }

        //        var data = "";
        //        foreach (var row in dataGrid.ItemsSource)
        //        {
        //            data += "<Row>\n";
        //            foreach (var column in dataGrid.Columns)
        //            {
        //                //if ((column as DataGridBoundColumn) != null)
        //                //{
        //                    var binding = (column as DataGridBoundColumn).Binding;
        //                    var path = binding.Path.Path;
        //                    var value = GetValue(row, path);
        //                    data += "<Cell><Data ss:Type=\"String\">" + value + "</Data></Cell>\n";
        //                //}
        //                //else if ((column as DataGridTemplateColumn) != null)
        //                //{
        //                //    var value = column.GetCellContent(row);
        //                //    if ((value as TextBlock) != null)
        //                //    {
        //                //        data += "<Cell><Data ss:Type=\"String\">" + (value as TextBlock).Text + "</Data></Cell>\n";
        //                //    }
        //                //    else if ((value as TextBox) != null)
        //                //    {
        //                //        data += "<Cell><Data ss:Type=\"String\">" + (value as TextBox).Text + "</Data></Cell>\n";
        //                //    }
        //                //}

        //            }
        //            data += "</Row>\n";
        //        }

        //        string excelXML = xmlTemplate.Replace("[headers]", headers).Replace("[data]", data);

        //        byte[] contents = Encoding.UTF8.GetBytes(excelXML);

        //        var dialog = new SaveFileDialog();
        //        dialog.DefaultExt = ".xls";
        //        dialog.Filter = "Excel (*.xls)|*.xls";

        //        if (dialog.ShowDialog() == true)
        //        {
        //            using (Stream s = dialog.OpenFile())
        //            {
        //                s.Write(contents, 0, contents.Length);
        //            }
        //            MessageBox.Show(eHCMSResources.A0467_G1_Msg_InfoDaKetXuat);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("Lỗi kết xuất excel!");
        //    }
        //}

        private string xmlTemplate = @"
<?xml version=""1.0""?>
<?mso-application progid=""Excel.Sheet""?>
<Workbook xmlns=""urn:schemas-microsoft-com:office:spreadsheet""
 xmlns:o=""urn:schemas-microsoft-com:office:office""
 xmlns:x=""urn:schemas-microsoft-com:office:excel""
 xmlns:ss=""urn:schemas-microsoft-com:office:spreadsheet""
 xmlns:html=""http://www.w3.org/TR/REC-html40"">
 <DocumentProperties xmlns=""urn:schemas-microsoft-com:office:office"">
  <Version>12.00</Version>
 </DocumentProperties>
 <ExcelWorkbook xmlns=""urn:schemas-microsoft-com:office:excel"">
  <ProtectStructure>False</ProtectStructure>
  <ProtectWindows>False</ProtectWindows>
 </ExcelWorkbook>
 <Styles>
  <Style ss:ID=""OrangeStyle"">
   <Interior ss:Color=""#FFC000"" ss:Pattern=""Solid""/>
  </Style>
 </Styles>
 <Worksheet ss:Name=""Export"">
  <Table x:FullColumns=""1""
   x:FullRows=""1"" ss:DefaultRowHeight=""15"">
   <Row>[headers]</Row>
   [data]
</Table>
  <WorksheetOptions xmlns=""urn:schemas-microsoft-com:office:excel"">
   <PageSetup>
    <Header x:Margin=""0.3""/>
    <Footer x:Margin=""0.3""/>
    <PageMargins x:Bottom=""0.75"" x:Left=""0.7"" x:Right=""0.7"" x:Top=""0.75""/>
   </PageSetup>
   <Print>
    <ValidPrinterInfo/>
    <PaperSizeIndex>9</PaperSizeIndex>
    <HorizontalResolution>600</HorizontalResolution>
    <VerticalResolution>0</VerticalResolution>
   </Print>
   <Selected/>
   <Panes>
    <Pane>
     <Number>3</Number>
     <ActiveRow>4</ActiveRow>
     <ActiveCol>3</ActiveCol>
    </Pane>
   </Panes>
   <ProtectObjects>False</ProtectObjects>
   <ProtectScenarios>False</ProtectScenarios>
  </WorksheetOptions>
 </Worksheet>
</Workbook>
";
    }
}