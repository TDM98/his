using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aEMR.Infrastructure.CachingUtils
{
    public interface ICachable
    {
        int NumberObject { get; set; }
        int BytesUsed { get; set; }
        long TimeExpired { get; set; }
    }
}
