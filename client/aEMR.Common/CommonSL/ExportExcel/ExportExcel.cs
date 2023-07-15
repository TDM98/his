using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.IO;

namespace aEMR.Common.ExportExcel
{
    public static class ExportToExcelFileAllData
    {
        public static void Export(List<List<string>> x, string strNameExcel)
        {
            Microsoft.Win32.SaveFileDialog filePath = GetFilePath();
            ExportAll(x,strNameExcel,filePath);            
        }

        private static Microsoft.Win32.SaveFileDialog GetFilePath()
        {
            Microsoft.Win32.SaveFileDialog objSFD = new Microsoft.Win32.SaveFileDialog()
            {
                DefaultExt = ".xls",
                Filter = "Excel xls (*.xls)|*.xls",
                //Filter = "Excel (2003)(.xls)|*.xls|Excel (2010) (.xlsx)|*.xlsx |RichText File (.rtf)|*.rtf |Pdf File (.pdf)|*.pdf |Html File (.html)|*.html",
                FilterIndex = 1
            };
            if (objSFD.ShowDialog() == true)
            {
                return objSFD;
            }
            return null;
        }

        public static void ExportAll(List<List<string>> listData, string strNameExcel, Microsoft.Win32.SaveFileDialog StreamPath)
        {
            if (listData != null && StreamPath!=null)
            {
                string filePath = StreamPath.SafeFileName;
                string strFormat = filePath.Substring(filePath.IndexOf('.') + 1).ToLower();
                StringBuilder strBuilder = new StringBuilder();
                List<string> lstFields = new List<string>();
                //BuildStringOfRow(strBuilder, columnNames);
                foreach (List<string> data in listData)
                {
                    lstFields.Clear();
                    foreach (var col in data)
                    {
                        lstFields.Add(FormatField(col,strFormat));
                    }
                    BuildStringOfRow(strBuilder, lstFields,strFormat);
                }

                StreamWriter sw = new StreamWriter(StreamPath.OpenFile());
                if (strFormat == "xls")
                {
                    //Let us write the headers for the Excel XML
                    sw.WriteLine("<?xml version=\"1.0\" " +
                                    "encoding=\"utf-8\"?>");
                    sw.WriteLine("<?mso-application progid" +
                                    "=\"Excel.Sheet\"?>");
                    sw.WriteLine("<Workbook xmlns=\"urn:" +
                                    "schemas-microsoft-com:office:spreadsheet\">");
                    sw.WriteLine("<DocumentProperties " +
                                    "xmlns=\"urn:schemas-microsoft-com:" +
                                    "office:office\">");
                    //sw.WriteLine("<Author>" + Globals.LoggedUserAccount.Staff.FullName.Trim() + "</Author>");
                    sw.WriteLine("<Created>" +
                                    DateTime.Now.ToLocalTime().ToLongDateString() +
                                    "</Created>");
                    sw.WriteLine("<LastSaved>" +
                                    DateTime.Now.ToLocalTime().ToLongDateString() +
                                    "</LastSaved>");
                    sw.WriteLine("<Company>Viện Tim</Company>");
                    sw.WriteLine("<Version>12.00</Version>");
                    sw.WriteLine("</DocumentProperties>");
                    sw.WriteLine("<Worksheet ss:Name=\"" + strNameExcel + "\" " + 
                        "xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                    sw.WriteLine("<Table>");
                }
                sw.Write(strBuilder.ToString());
                if (strFormat == "xls")
                {
                    sw.WriteLine("</Table>");
                    sw.WriteLine("</Worksheet>");
                    sw.WriteLine("</Workbook>");
                }
                sw.Close();

                MessageBox.Show(eHCMSResources.A0467_G1_Msg_InfoDaKetXuat);                
            }
        }
        
        private static void BuildStringOfRow(StringBuilder strBuilder, List<string> lstFields, string strFormat)
        {
            switch (strFormat)
            {
                case "xls":
                    strBuilder.AppendLine("<Row>");
                    strBuilder.AppendLine(String.Join("\r\n", lstFields.ToArray()));
                    strBuilder.AppendLine("</Row>");
                    break;
                case "CSV":
                    strBuilder.AppendLine(String.Join(",", lstFields.ToArray()));
                    break;
            }
        }

        private static string FormatField(string data, string format)
        {
            switch (format)
            {
                case "xls":
                    return String.Format("<Cell><Data ss:Type=\"String" +
                       "\">{0}</Data></Cell>", data);
                case "CSV":
                    return String.Format("\"{0}\"",
                      data.Replace("\"", "\"\"\"").Replace("\n",
                      "").Replace("\r", ""));
            }
            return data;
        }
    }

}
