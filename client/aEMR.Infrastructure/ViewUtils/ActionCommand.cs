using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aEMR.Infrastructure.ViewUtils
{
    public class ActionCommand
    {
        public long ActionPk { get; set; }
        public string ActionName { get; set; }
        public Type TypeViewModel { get; set; }

        public override string ToString()
        {
            return ActionName;
        }

    }
}
