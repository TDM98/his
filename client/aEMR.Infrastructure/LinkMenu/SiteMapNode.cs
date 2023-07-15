using System;
using Caliburn.Micro;

namespace aEMR.Infrastructure.LinkMenu
{
    public class SiteMapNode : PropertyChangedBase
    {
        public SiteMapNode()
        {
            
        }

        public int MenuId { get; set; }

        public string MenuTitle { get; set; }

        public string ImageSrc { get; set; }

        public Type ScreenType { get; set; }

        public int Parent { get; set; }

    }
}
