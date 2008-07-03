using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareFg.Framework.Infrastructure.DesignByContract
{
    /// <summary>
    /// Exception that is thrown when a postcondition fails.
    /// </summary>
    [Serializable]
    public class PostconditionException : DesignByContractException
    {
        /// <summary>
        /// Postcondition Exception.
        /// </summary>
        public PostconditionException()
        {
        }

        /// <summary>
        /// Postcondition Exception.
        /// </summary>
        public PostconditionException( string message )
            : base (message)
        {
        }

        /// <summary>
        /// Postcondition Exception.
        /// </summary>
        public PostconditionException( string message, Exception inner )
            : base (message, inner)
        {
        }
    }
}
