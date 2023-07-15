using aEMR.Infrastructure.Events;
using DataEntities;

/*
 * 20180913 TTM #001: Thêm mới hàm để nhận dữ liệu từ ViewModel cha và thêm cờ để xác định xem nó có phải là Dialog hay không
 */

namespace aEMR.ViewContracts
{
    public interface IDiseaseProgression
    {
        bool IsOpenFromInstruction { get; set; }
        bool IsOpenFromTicketCare { get; set; }
    }
}