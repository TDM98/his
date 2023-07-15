using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Core.Common
{
    /// <summary>
    /// Mô tả trạng thái của các object
    /// </summary>
    /// 
    public enum EntityState : byte
    {
        //Object mới được tạo ra, chưa set thuộc tính
        //Object này chưa được add vào database
        NEW = 0,

        //Object mới được tạo ra và đã được set thuộc tính rồi.
        //Object này chưa được add vào database
        DETACHED = 1,

        //Object được lấy từ database ra, và chưa thay đổi thuộc tính
        PERSITED = 2,

        //Object được lấy từ database ra, và đã thay đổi thuộc tính
        MODIFIED = 3,

        //Object bị đánh dấu xóa. (nam trong DB roi)
        DELETED_PERSITED = 4,

        DELETED_MODIFIED = 5
    }

   

}