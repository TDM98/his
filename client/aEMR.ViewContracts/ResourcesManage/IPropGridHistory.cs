using System;
using System.Collections.ObjectModel;
using DataEntities;
using eHCMS.Common.Collections;

namespace Ax.ViewContracts.SL
{
    public interface IPropGridHistory
    {
        object ActiveContent { get; set; }
        PagedSortableCollectionView<ResourcePropLocations> allRsrcPropLocBreak { get; set; }
    }
}
