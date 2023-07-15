using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DataEntities;


namespace eHCMS.Services.Core
{
    public class ServiceCollectioneHCMS
    {
        static private readonly ServiceCollectioneHCMS _instance = null;

        static public ServiceCollectioneHCMS Instance
        {
            get
            {
                return _instance;
            }
        }
        
        public static void Init()
        {
            //Khong lam gi het. De ben ngoai goi vay thoi
        }
       
    }

    
}
