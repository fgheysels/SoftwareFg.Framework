using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SoftwareFg.Framework.Infrastructure.DesignByContract
{
    /// <summary>
    /// Design By Contract Checks.
    /// 
    /// Each method generates an exception or a trace assertion statement if the contract is broken.
    /// </summary>
    /// <remarks>
    /// This example shows how to call the Require method.
    /// <code>
    /// public void Test(int x)
    /// {
    /// 	try
    /// 	{
    ///			Check.Require(x > 1, "x must be > 1");
    ///		}
    ///		catch (System.Exception ex)
    ///		{
    ///			Console.WriteLine(ex.ToString());
    ///		}
    ///	}
    /// </code>
    ///
    /// You can direct output to a Trace listener. For example, you could insert
    /// <code>
    /// Trace.Listeners.Clear();
    /// Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
    /// </code>
    /// 
    /// or direct output to a file or the Event Log.
    /// 
    /// (Note: For ASP.NET clients use the Listeners collection
    /// of the Debug, not the Trace, object and, for a Release build, only exception-handling
    /// is possible.)
    /// </remarks>
    public sealed class Check
    {
        #region Interface

        /// <summary>
        /// Precondition check - should run regardless of preprocessor directives.
        /// </summary>
        public static void Require( bool assertion, string message )
        {
            if( UseExceptions )
            {
                if( !assertion )
                {
                    throw new PreconditionException (message);
                }
            }
            else
            {
                Trace.Assert (assertion, "Precondition: " + message);
            }
        }

        /// <summary>
        /// Precondition check - should run regardless of preprocessor directives.
        /// </summary>
        public static void Require( bool assertion, string message, Exception inner )
        {
            if( UseExceptions )
            {
                if( assertion == false )
                {
                    throw new PreconditionException (message, inner);
                }
            }
            else
            {
                Trace.Assert (assertion, "Precondition: " + message);
            }
        }

        /// <summary>
        /// Precondition check - should run regardless of preprocessor directives.
        /// </summary>
        public static void Require( bool assertion )
        {
            if( UseExceptions )
            {
                if( assertion == false )
                {
                    throw new PreconditionException ("Precondition failed.");
                }
            }
            else
            {
                Trace.Assert (assertion, "Precondition failed.");
            }
        }

        /// <summary>
        /// Postcondition check.
        /// </summary>
        public static void Ensure( bool assertion, string message )
        {
            if( UseExceptions )
            {
                if( assertion == false )
                {
                    throw new PostconditionException (message);
                }
            }
            else
            {
                Trace.Assert (assertion, "Postcondition: " + message);
            }
        }

        /// <summary>
        /// Postcondition check.
        /// </summary>
        public static void Ensure( bool assertion, string message, Exception inner )
        {
            if( UseExceptions )
            {
                if( assertion == false )
                {
                    throw new PostconditionException (message, inner);
                }
            }
            else
            {
                Trace.Assert (assertion, "Postcondition: " + message);
            }
        }

        /// <summary>
        /// Postcondition check.
        /// </summary>
        public static void Ensure( bool assertion )
        {
            if( UseExceptions )
            {
                if( assertion == false )
                {
                    throw new PostconditionException ("Postcondition failed.");
                }
            }
            else
            {
                Trace.Assert (assertion, "Postcondition failed.");
            }
        }

        /// <summary>
        /// Invariant check.
        /// </summary>
        public static void Invariant( bool assertion, string message )
        {
            if( UseExceptions )
            {
                if( assertion == false )
                {
                    throw new InvariantException (message);
                }
            }
            else
            {
                Trace.Assert (assertion, "Invariant: " + message);
            }
        }

        /// <summary>
        /// Invariant check.
        /// </summary>
        public static void Invariant( bool assertion, string message, Exception inner )
        {
            if( UseExceptions )
            {
                if( assertion == false )
                {
                    throw new InvariantException (message, inner);
                }
            }
            else
            {
                Trace.Assert (assertion, "Invariant: " + message);
            }
        }

        /// <summary>
        /// Invariant check.
        /// </summary>
        public static void Invariant( bool assertion )
        {
            if( UseExceptions )
            {
                if( assertion == false )
                {
                    throw new InvariantException ("Invariant failed.");
                }
            }
            else
            {
                Trace.Assert (assertion, "Invariant failed.");
            }
        }

        /// <summary>
        /// Assertion check.
        /// </summary>
        public static void Assert( bool assertion, string message )
        {
            if( UseExceptions )
            {
                if( assertion == false )
                {
                    throw new AssertionException (message);
                }
            }
            else
            {
                Trace.Assert (assertion, "Assertion: " + message);
            }
        }

        /// <summary>
        /// Assertion check.
        /// </summary>
        public static void Assert( bool assertion, string message, Exception inner )
        {
            if( UseExceptions )
            {
                if( assertion == false )
                {
                    throw new AssertionException (message, inner);
                }
            }
            else
            {
                Trace.Assert (assertion, "Assertion: " + message);
            }
        }

        /// <summary>
        /// Assertion check.
        /// </summary>
        public static void Assert( bool assertion )
        {
            if( UseExceptions )
            {
                if( assertion == false )
                {
                    throw new AssertionException ("Assertion failed.");
                }
            }
            else
            {
                Trace.Assert (assertion, "Assertion failed.");
            }
        }

        /// <summary>
        /// Set this if you wish to use Trace Assert statements 
        /// instead of exception handling. 
        /// (The Check class uses exception handling by default.)
        /// </summary>
        public static bool UseAssertions
        {
            get;
            set;
        }


        #endregion // Interface

        #region Implementation

        // No creation
        private Check()
        {
            // The default behaviour is that Exceptions are being thrown.
            UseAssertions = false;
        }

        /// <summary>
        /// Is exception handling being used?
        /// </summary>
        private static bool UseExceptions
        {
            get
            {
                return UseAssertions == false;
            }
        }
        
        #endregion // Implementation
    }
}
