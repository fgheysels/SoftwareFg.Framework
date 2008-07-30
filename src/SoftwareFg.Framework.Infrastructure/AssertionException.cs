using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareFg.Framework
{
    /// <summary>
    /// Exception that is raised when an Assertion fails.
    /// </summary>
    [Serializable]
    public class AssertionException : DesignByContractException
    {
        /// <summary>
        /// Assertion Exception.
        /// </summary>
        public AssertionException()
        {
        }

        /// <summary>
        /// Assertion Exception.
        /// </summary>
        public AssertionException( string message )
            : base (message)
        {
        }

        /// <summary>
        /// Assertion Exception.
        /// </summary>
        public AssertionException( string message, Exception inner )
            : base (message, inner)
        {
        }
    }
}
