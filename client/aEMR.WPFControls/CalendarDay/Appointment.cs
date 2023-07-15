using System;

namespace aEMR.WPFControls
{
    public class Appointment : BindableObject
    {
        private long _AppointmentID;
        public long AppointmentID
        {
            get { return _AppointmentID; }
            set
            {
                if (_AppointmentID != value)
                {
                    _AppointmentID = value;
                    RaisePropertyChanged("AppointmentID");
                }
            }
        }

        private string subject;
        public string Subject
        {
            get { return subject; }
            set
            {
                if (subject != value)
                {
                    subject = value;
                    RaisePropertyChanged("Subject");
                }
            }
        }

        private string location;
        public string Location
        {
            get { return location; }
            set
            {
                if (location != value)
                {
                    location = value;
                    RaisePropertyChanged("Location");
                }
            }
        }

        private DateTime startTime;
        public DateTime StartTime
        {
            get { return startTime; }
            set
            {
                if (startTime != value)
                {
                    startTime = value;
                    RaisePropertyChanged("StartTime");
                }
            }
        }

        private DateTime endTime;
        public DateTime EndTime
        {
            get { return endTime; }
            set
            {
                if (endTime != value)
                {
                    endTime = value;
                    RaisePropertyChanged("EndTime");
                }
            }
        }

        private string body;
        public string Body
        {
            get { return body; }
            set
            {
                if (body != value)
                {
                    body = value;
                    RaisePropertyChanged("Body");
                }
            }
        }

        public override string ToString()
        {
            return Subject;
        }
    }
}