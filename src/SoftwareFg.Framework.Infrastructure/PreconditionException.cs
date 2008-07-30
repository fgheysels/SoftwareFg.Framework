using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareFg.Framework
{
    /// <summary>
    /// Exception that is raised when a precondition fails.
    /// </summary>
    [Serializable]
    public class PreconditionException : DesignByContractException
    {
        /// <summary>
        /// Precondition Exception.
        /// </summary>
        public PreconditionException()
        {
        }

        /// <summary>
        /// Precondition Exception.
        /// </summary>
        public PreconditionException( string message )
            : base (message)
        {
        }

        /// <summary>
        /// Precondition Exception.
        /// </summary>
        public PreconditionException( string message, Exception inner )
            : base (message, inner)
        {
        }
    }
}
