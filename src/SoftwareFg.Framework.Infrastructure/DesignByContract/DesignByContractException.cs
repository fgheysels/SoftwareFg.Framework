using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareFg.Framework.Infrastructure.DesignByContract
{
    /// <summary>
    /// Exception raised when a contract is broken.
    /// Catch this exception type if you wish to differentiate between 
    /// any DesignByContract exception and other runtime exceptions.    
    /// </summary>
    [Serializable]
    public class DesignByContractException : ApplicationException
    {
        protected DesignByContractException()
        {
        }

        protected DesignByContractException( string message )
            : base (message)
        {
        }

        protected DesignByContractException( string message, Exception inner )
            : base (message, inner)
        {
        }
    }
}
