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

namespace aEMR.Infrastructure.Events
{
    public class ICD_Event
    {
    }

    public class dgListDblClickSelectICD_Event
    {
        public object ICD_Current { get; set; }
    }

    public class dgICDListClickSelectionChanged_Event
    {
        public object ICD_Current { get; set; }
    }
    public class Chapter_Event_Save
    {
        public object Result { get; set; }
    }
    public class Diseases_Event_Save
    {
        public object Result { get; set; }
    }
    public class ICD_Event_Save
    {
        public object Result { get; set; }
    }
}