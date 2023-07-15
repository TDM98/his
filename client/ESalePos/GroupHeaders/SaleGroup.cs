using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using aEMR.Infrastructure;
using EPos.ViewContracts.PosViewContracts;

namespace ESalePos.GroupHeaders
{
    public static class SaleGroup
    {

        public static PanelItem[] Groups = new[]
        {
            new PanelItem
            {
                Text = "POS",
                ToolTip = "Point of Sale",
                ImgSrc =
                    "/EPos.Resources;component/Assets/Images/Pos.jpg"
                , ViewModelType = typeof(IPosViewModel)
            },
                
        };
    }
}
