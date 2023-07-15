using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace eHCMS.Services.Core.Base
{
    /// <summary>
    /// Abstract class that represents the response of a single query or create
    /// update, or delete operation.
    /// </summary>
    /// 
    [DataContract]
    public abstract class ServiceOperationResponse
    {
        public ServiceOperationResponse(){}
        /// <summary>
        /// An System.Exception object that contains the error.
        /// </summary>
        /// 
        [DataMember]
        public Exception Error { get; set; }

        /// <summary>
        /// Integer value that contains response code.
        /// </summary>
        /// 
        [DataMember]
        public int StatusCode { get; set; }
    }
}
