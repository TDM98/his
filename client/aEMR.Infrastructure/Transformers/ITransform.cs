using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aEMR.Infrastructure.Transformers
{
    public interface ITransform<out T, in TObject>
    {
        /// <summary>
        /// Transforms the specified object transform.
        /// </summary>
        /// <param name="objectTransform">The object transform.</param>
        /// <returns>The specific object has been transformed.</returns>
        T Transform(TObject objectTransform);
    }
}
