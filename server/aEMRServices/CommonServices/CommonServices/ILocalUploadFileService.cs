using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using eHCMS.Services.Core;
using System.IO;

namespace CommonServices
{
    [ServiceContract]
    public interface ILocalUploadFileService
    {
        [OperationContract]
        bool CopyAll(DirectoryInfo source, DirectoryInfo target);
    }
}
