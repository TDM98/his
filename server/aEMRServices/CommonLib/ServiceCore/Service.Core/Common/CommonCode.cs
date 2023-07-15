using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace Service.Core.Common
{
    /// <summary>
    /// Lay ma so phieu 
    /// </summary>
    /// 
    //[DataContract]
    //public class CodeNumbers
    //{
    //    public static string GetCode_NhapHang_NhaThuoc()
    //    {
    //        return "";
    //    }
    //    public static string GetCode_Xuat_NhaThuoc()
    //    {
    //        return "";
    //    }
    //    public static int Get_ColectDrugSeqNum_NhaThuoc(int ColectDrugSeqNumType)
    //    {
    //        //1 : benh nhan thong thuong,2 : benh nhan BH
    //        return 1;
    //    }
       
    //}
    public static class CommonFunction
    { 
        //b2d: commonfunction
        public static Stream GetVideoAndImage(string path)
        {
            try
            {
                FileInfo fi = new FileInfo(path);
                Stream strm = fi.OpenRead();

                return strm;
            }
            catch
            {
                return new MemoryStream();
            }
        }

        

    }
}