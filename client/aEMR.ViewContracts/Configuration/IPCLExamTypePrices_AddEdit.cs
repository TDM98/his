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

namespace aEMR.ViewContracts.Configuration
{
    public interface IPCLExamTypePrices_AddEdit
    {
        string TitleForm { get; set; }
        DataEntities.PCLExamType ObjPCLExamType_Current { get; set; }
        void InitializeNewItem();
        DataEntities.PCLExamTypePrice ObjPCLExamTypePrice_Current { get; set; }
    }
}
