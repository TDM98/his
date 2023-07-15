using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aEMR.Infrastructure.Events
{
    public class RefRowShelfEdit_Event
    {
        public object Result { get; set; }
    }

    public class MedicalFileManagerEdit_Event
    {
        public object Result { get; set; }
    }

    public class RefRowMedicalFileManagerEdit_Event
    {
        public object Result { get; set; }
    }

    public class RefShelfMedicalFileManagerEdit_Event
    {
        public object Result { get; set; }
    }

    public class RefShelfDetailsMedicalFileManagerEdit_Event
    {
        public object Result { get; set; }
    }

    public class MedicalFileManagerCheckOutEdit_Event
    {
        public object Result { get; set; }
    }
    public class MedicalFileManagerCheckInEdit_Event
    {
        public object Result { get; set; }
    }
}
