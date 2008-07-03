using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareFg.Framework.Infrastructure.DesignByContract
{
    /// <summary>
    /// Exception that is raised when an invariant fails.
    /// </summary>
    [Serializable]
    public class InvariantException : DesignByContractException
    {
        /// <summary>
        /// Invariant Exception.
        /// </summary>
        public InvariantException()
        {
        }

        /// <summary>
        /// Invariant Exception.
        /// </summary>
        public InvariantException( string message )
            : base (message)
        {
        }

        /// <summary>
        /// Invariant Exception.
        /// </summary>
        public InvariantException( string message, Exception inner )
            : base (message, inner)
        {
        }
    }
}
