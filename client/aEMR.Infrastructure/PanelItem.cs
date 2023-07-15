using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
//using EPos.Domains;

namespace aEMR.Infrastructure
{
    public class PanelItem : PropertyChangedBase
    {
        public string Text { get; set; }
        public string ToolTip { get; set; }
        private string _imgSrc;
        public string ImgSrc
        {
            get { return _imgSrc; }
            set
            {
                _imgSrc = value;
                NotifyOfPropertyChange(() => ImgSrc);
            }
        }

        //public IList<Position> Positions { get; set; }
        public Type ViewModelType { get; set; }
        public string Parameter { get; set; }

    }
}
