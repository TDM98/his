using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Data.SqlClient;
using DataEntities;
using eHCMS.Services.Core;
using Service.Core.Common;
using System.Text;
using eHCMS.Configurations;
using System.Collections;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;
using AxLogging;
using eHCMS.Caching;


namespace eHCMS.DAL
{
    public abstract class FunctionBase : DataAccess
    {
        public int GetStartSequenceNumber(DateTime curDate, ConsultationRoomTarget crt)
        {
            switch (curDate.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return crt.MondayStartSequenceNumber;

                case DayOfWeek.Tuesday:
                    return crt.TuesdayStartSequenceNumber;

                case DayOfWeek.Wednesday:
                    return crt.WednesdayStartSequenceNumber ;

                case DayOfWeek.Thursday:
                    return crt.ThursdayStartSequenceNumber;

                case DayOfWeek.Friday:
                    return crt.FridayStartSequenceNumber;

                case DayOfWeek.Saturday:
                    return crt.SaturdayStartSequenceNumber;

                case DayOfWeek.Sunday:
                    return crt.SundayStartSequenceNumber;
                default: return 0;
            }
        }
    }
}



