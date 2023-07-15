﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace aEMR.ViewContracts
{
    public interface IPCLExamTypeContactDrugList
    {
        Int64 V_MedProductType { get; set; }
        string TitleForm { get; set; }
        string TextGroupTimKiem { get; set; }
        string TextButtonThemMoi { get; set; }
        string TextDanhSach { get; set; }
        Visibility dgColumnExtOfThuoc_Visible { get; set; }
        //ny them 
        bool IsPopUp { get; set; }
        bool IsStore { get; set; }

        bool mTim { get; set; }
        bool mThemMoi { get; set; }
        bool mChinhSua { get; set; }
        long PCLExamTypeID { get; set; }
    }
}
