using System;
using System.Net;
using System.Collections.Generic;

/*********************************
*
* Created by: Trinh Thai Tuyen
* Date Created: Friday, August 20, 2010
* Last Modified Date: 
*
*********************************/
using eHCMS.Services.Core.Base;
using System.Runtime.Serialization;
namespace eHCMS.Services.Core
{
    public class CRUDOperationResponse:ServiceOperationResponse
    {
        public CRUDOperationResponse()
        {
            this.Error = null;
        }
    }
}
