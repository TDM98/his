using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public enum eoDanhMucVatTu : int
    {
        mNhomVatTu = 1,
        mLoaiVatTu = 2,
        mVatTu = 3        
    }

    public enum eKBTongQuat : int
    {
        mCommonRecord = 1,
        
        mCount=4
    }

}
