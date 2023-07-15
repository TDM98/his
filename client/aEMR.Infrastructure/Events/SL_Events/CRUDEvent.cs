using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
/*
 * 20190822 #001 TTM:   BM 0013217: Cho phép sử dụng Grid phiếu chỉ định trong đợt đăng ký để thực hiện hiệu chỉnh.
 */
namespace aEMR.Infrastructure.Events
{
    public class CRUDEvent
    {

    }

    public class SaveEvent<T>
    {
        public T Result { get; set; }
    }

    public class SaveEventWithKey<TKey,TResult>
    {
        public TKey Key { get; set; }
        public TResult Result { get; set; }
    }

    public class DeleteEvent<T>
    {
        public T Result { get; set; }
    }

    public class SelectedObjectEvent<T>
    {
        public T Result { get; set; }
    }

    public class SelectedObjectEventWithKey<T,TKey>
    {
        public T Result { get; set; }
        public TKey Key { get; set; }
    }
    public class SelectedObjectEventWithKey_ForImage<T, TKey>
    {
        public T Result { get; set; }
        public TKey Key { get; set; }
    }
    public class SelectedObjectEventWithKeyAppt<T, TKey>
    {
        public T Result { get; set; }
        public TKey Key { get; set; }
    }
    public class SelectedObjectEventWithKey_ForImageAppt<T, TKey>
    {
        public T Result { get; set; }
        public TKey Key { get; set; }
    }

    public class ClosePriceEvent
    {

    }

    public class ReLoadDataAfterCUD
    {

    }

    public class ReLoadDataAfterU
    {

    }

    public class ReLoadDataAfterD
    {

    }

    public class ReLoadDataAfterC
    {

    }
    

    public class DbClickSelectedObjectEvent<T>
    {
        public T Result { get; set; }
        public bool IsReadOnly { get; set; }
    }

  
    public class DbClickSelectedObjectEventWithKey<TA,TB>
    {
        public TA ObjA { get; set; }
        public TB ObjB { get; set; }
        public bool IsReadOnly { get; set; }
    }
    //▼===== #001:  DbClickSelectedObjectEventWithKey Tách 1 sự kiện chính (sử dụng chung xét nghiệm và hình ảnh) thành nhiều sự kiện con, vì đang gộp chung chỉ định XN và HA thành tab. 
    //              Không tách riêng dẫn đến xem thông tin của HA mà bên tab XN vẫn thấy dữ liệu.
    public class DbClickSelectedObjectEventWithKeyForImage<TA, TB>
    {
        public TA ObjA { get; set; }
        public TB ObjB { get; set; }
        public bool IsReadOnly { get; set; }
    }
    public class DbClickSelectedObjectEventWithKeyToShowDetails<TA, TB>
    {
        public TA ObjA { get; set; }
        public TB ObjB { get; set; }
        public bool IsReadOnly { get; set; }
    }
    public class DbClickSelectedObjectEventWithKeyToShowDetailsForImage<TA, TB>
    {
        public TA ObjA { get; set; }
        public TB ObjB { get; set; }
        public bool IsReadOnly { get; set; }
    }
    //▲===== #001
    public class DbClickPatientPCLRequest<TA, TB>
    {
        public TA ObjA { get; set; }
        public TB ObjB { get; set; }
        public bool IsReadOnly { get; set; }
    }

    public class DbClickPtPCLReqExtEdit<TA, TB>
    {
        public TA ObjA { get; set; }
        public TB ObjB { get; set; }
    }

    public class DbClickPtPCLReqExtDo<TA, TB,TC>
    {
        public TA ObjA { get; set; }
        public TB ObjB { get; set; }
        public TC ObjC { get; set; }
    }
    /// <summary>
    /// TA:FromDate
	/// TB: ToDate
	/// Quy
	/// Thang
	/// Nam
    /// </summary>
    /// <typeparam name="TA"></typeparam>
    /// <typeparam name="TB"></typeparam>
    /// <typeparam name="TC"></typeparam>
    /// <typeparam name="TD"></typeparam>
    /// <typeparam name="TE"></typeparam>
    public class SelectedObjectWithKey<TA, TB,TC,TD,TE,TKey>
    {
        public TA ObjA { get; set; }
        public TB ObjB { get; set; }
        public TC ObjC { get; set; }
        public TD ObjD { get; set; }
        public TE ObjE { get; set; }
        public TKey ObjKey{ get; set;}
    }

    public class SelectedObjectWithKey<TA, TB, TC, TD, TE,TF, TKey>
    {
        public TA ObjA { get; set; }
        public TB ObjB { get; set; }
        public TC ObjC { get; set; }
        public TD ObjD { get; set; }
        public TE ObjE { get; set; }
        public TF ObjF { get; set; }
        public TKey ObjKey { get; set; }
    }
    public class SelectedObjectWithKey<TA, TB, TC, TD, TE, TF, TG, TKey>
    {
        public TA ObjA { get; set; }
        public TB ObjB { get; set; }
        public TC ObjC { get; set; }
        public TD ObjD { get; set; }
        public TE ObjE { get; set; }
        public TF ObjF { get; set; }
        public TG ObjG { get; set; }
        public TKey ObjKey { get; set; }
    }
    public class SelectedObjectWithKey<TA, TB, TC, TD, TE, TF, TG, TH, TK, TKey>
    {
        public TA ObjA { get; set; }
        public TB ObjB { get; set; }
        public TC ObjC { get; set; }
        public TD ObjD { get; set; }
        public TE ObjE { get; set; }
        public TF ObjF { get; set; }
        public TG ObjG { get; set; }
        public TH ObjH { get; set; }
        public TK ObjK { get; set; }
        public TKey ObjKey { get; set; }
    }

    /*TMA 24/10/2017*/
    public class SelectedObjectWithKeyExcel<TA, TB, TC, TD, TE, TF, TKey>
    {
        public TA ObjA { get; set; }
        public TB ObjB { get; set; }
        public TC ObjC { get; set; }
        public TD ObjD { get; set; }
        public TE ObjE { get; set; }
        public TF ObjF { get; set; }
        public TKey ObjKey { get; set; }
    }
    /*TMA 24/10/2017*/
    public class SelectedObjectWithKeyExcel<TA, TB, TC, TD, TE, TF, TG, TKey>
    {
        public TA ObjA { get; set; }
        public TB ObjB { get; set; }
        public TC ObjC { get; set; }
        public TD ObjD { get; set; }
        public TE ObjE { get; set; }
        public TF ObjF { get; set; }
        public TG ObjG { get; set; }
        public TKey ObjKey { get; set; }
    }
}
