using DataEntities;
using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Media;

namespace aEMR.Common.Converters
{
    public class HightlightByStatusConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(values == null || values.Length == 0)
            {
                return Brushes.Black;
            }

            if(values.Length == 1)
            {
                long ExamRegStatus = (long)values[0];
                if (ExamRegStatus == (long)AllLookupValues.ExamRegStatus.HOAN_TAT)
                {
                    //return Brushes.Blue;
                    return new SolidColorBrush(Colors.Blue);
                }

                return new SolidColorBrush(Colors.Green);
            }
            else if(values.Length == 3)
            {
                bool IsProcedureEdit = (bool)values[0];
                int InPtAdmissionStatus = (int)values[1];

                if(!IsProcedureEdit)
                {
                    switch (InPtAdmissionStatus)
                    {
                        case 0:
                            // return Brushes.Red;
                            return new SolidColorBrush(Colors.Red);
                        case 1:
                        case 6:
                        case 7:
                            return new SolidColorBrush(Colors.Black);
                        case 2:
                            return new SolidColorBrush(Colors.Black);
                        case 3:
                            return new SolidColorBrush(Colors.Green);
                        case 4:
                            return new SolidColorBrush(Colors.Red);
                        case 5:
                            return new SolidColorBrush(Colors.Purple);
                        default:
                            return new SolidColorBrush(Colors.Red);
                    }
                }
                else
                {
                    ObservableCollection<PatientRegistrationDetail> patientRegistrationDetail = (ObservableCollection<PatientRegistrationDetail>)values[2];
                    foreach (var RegistrationDetail in patientRegistrationDetail)
                    {
                        switch (RegistrationDetail.V_ExamRegStatus)
                        {
                            case 704:
                                return new SolidColorBrush(Colors.Red);
                            default:
                                return new SolidColorBrush(Colors.Black);
                        }
                    }
                    return Brushes.Black;
                }
            }
            else return Brushes.Black;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}