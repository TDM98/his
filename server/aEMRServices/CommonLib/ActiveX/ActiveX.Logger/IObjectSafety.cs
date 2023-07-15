using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace eHCMS.ActiveX.Logging
{
    [ComImport()]
    [Guid("CB5BDC81-93C1-11CF-8F20-00805F2CD064")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IObjectSafety
    {
        [PreserveSig()]
        ObjectSafetyRetVal GetInterfaceSafetyOptions(ref Guid riid, out ObjectSafetyOptions pdwSupportedOptions, out ObjectSafetyOptions pdwEnabledOptions);

        [PreserveSig()]
        ObjectSafetyRetVal SetInterfaceSafetyOptions(ref Guid riid, ObjectSafetyOptions dwOptionSetMask, ObjectSafetyOptions dwEnabledOptions);
    }
}
