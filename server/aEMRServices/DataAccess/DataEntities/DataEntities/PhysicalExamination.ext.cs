using System;
using System.Net;
using System.Collections.ObjectModel;

using System.Globalization;
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-10-07
 * Contents: physicalexamination extension
/*******************************************************************/
#endregion

namespace DataEntities
{
    public partial class PhysicalExamination
    {
        public string BMI
        {
            //ref: http://vi.wikipedia.org/wiki/Ch%E1%BB%89_s%E1%BB%91_kh%E1%BB%91i_c%C6%A1_th%E1%BB%83
            get
            {
                double bmi=0.0;
                double? H;
                double? W;                
                H = this.Height;//m
                W = this.Weight;//kg
                if (H!=null && H > 0 && W!=null && W > 0)
                {
                    H = H / 100;
                    bmi = (double)(W / (H * H));
                }
                if (bmi >0)
                    //return "BMI:" + bmi.ToString("F", new CultureInfo("en-US"));
                    return bmi.ToString("F", new CultureInfo("en-US"));
                else
                    //return "BMI:";
                    return "";
            }
            set 
            {
            }
        }

        public string BMIValue
        {
            //ref: http://vi.wikipedia.org/wiki/Ch%E1%BB%89_s%E1%BB%91_kh%E1%BB%91i_c%C6%A1_th%E1%BB%83
            get
            {
                double bmi = 0.0;
                double? H;
                double? W;
                H = this.Height;//m
                W = this.Weight;//kg
                if (H != null && H > 0 && W != null && W > 0)
                {
                    H = H / 100;
                    bmi = (double)(W / (H * H));
                }
                if (bmi > 0)
                    return bmi.ToString("F", new CultureInfo("en-US"));
                else
                    return "";
            }
            set
            {
            }
        }

        public string PressureSystolicDiastolic
        {
            get
            {
                if ((this.SystolicPressure != null) && (this.DiastolicPressure != null))
                    return (this.SystolicPressure.ToString() + "/" + this.DiastolicPressure.ToString());
                else
                    return "_/_";
            }
            set
            {
            }
        }

        public string AlcoholString
        {
            get
            {
                string result= string.Empty;
                if (this.Alcohol_Never != null && this.Alcohol_Never == true)
                    result += "Never";
                else
                    if (this.Alcohol_HeavyInThePast != null && this.Alcohol_HeavyInThePast == true)
                        result += "Heavy In The Past";
                    else
                        if (this.Alcohol_CurrentLight != null && this.Alcohol_CurrentLight == true)
                            result += "Current Light";
                        else
                            if(this.Alcohol_CurrentHeavy!= null && this.Alcohol_CurrentHeavy== true)
                                result+="Current Heavy";
                            else
                                result += "";
                return result;
            }
            set
            {
            }
        }

        public string SmokeString
        {
            get
            {
                string result = string.Empty;
                if (this.Smoke_Never != null && this.Smoke_Never == true)
                    result += "Never";
                else
                    if (this.Smoke_EveryDay != null && this.Smoke_EveryDay == true)
                        result += "EveryDay";
                    else
                        if (this.Smoke_OnOccasion != null && this.Smoke_OnOccasion == true)
                            result += "On Occasion";
                        else
                            result += "";
                return result;
            }
            set
            {
            }
        }        


    }
}
