using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace aEMR.Infrastructure.Collections
{
    /// <summary>
    /// Event argument used for page index change notifications. The requested page move
    /// can be canceled by setting e.Cancel to True.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public sealed class PageChangingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Constructor that takes the target page index
        /// </summary>
        /// <param name="newPageIndex">Index of the requested page</param>
        public PageChangingEventArgs(int newPageIndex)
        {
            this.NewPageIndex = newPageIndex;
        }

        /// <summary>
        /// Gets the index of the requested page
        /// </summary>
        public int NewPageIndex
        {
            get;
            private set;
        }
    }
}
