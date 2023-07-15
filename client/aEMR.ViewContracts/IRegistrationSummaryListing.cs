using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Dùng để thống kê đăng ký theo 1 tiêu chuẩn tìm kiếm nào đó.
    /// </summary>
    public interface IRegistrationSummaryListing
    {
        void StartSearching();
        void DoubleClick(object args);

        bool IsLoading { get; set; }
        RegistrationSummaryInfo SelectedItem { get; set; }
        SeachPtRegistrationCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<RegistrationSummaryInfo> RegistrationSummaryList { get; }
        RegistrationsTotalSummary TotalSummary { get; set; }
    }
}
