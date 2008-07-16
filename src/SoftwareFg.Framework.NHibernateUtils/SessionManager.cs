using System.Runtime.Remoting.Messaging;
using NHibernate;
using NHibernate.Cache;
using SoftwareFg.Framework.Infrastructure.DesignByContract;
using System.Web;
using System;


namespace SoftwareFg.Framework.NHibernateUtils
{
    /// <summary>
    /// Handles creation and management of sessions and transactions.  It is a singleton because 
    /// building the initial session factory is very expensive. Inspiration for this class came 
    /// from Chapter 8 of Hibernate in Action by Bauer and King.  Although it is a sealed singleton
    /// you can use TypeMock (http://www.typemock.com) for more flexible testing.
    /// </summary>
    public sealed class SessionManager
    {

        #region Thread-safe, lazy Singleton

        /// <summary>
        /// This is a thread-safe, lazy singleton.  See http://www.yoda.arachsys.com/csharp/singleton.html
        /// for more details about its implementation.
        /// </summary>
        public static SessionManager Instance
        {
            get
            {
                return Nested.SessionManager;
            }
        }

        /// <summary>
        /// Private constructor to enforce singleton
        /// </summary>
        private SessionManager()
        {
            InitSessionFactory ();
        }

        /// <summary>
        /// Assists with ensuring thread-safe, lazy singleton
        /// </summary>
        private static class Nested
        {
            internal static readonly SessionManager SessionManager = new SessionManager ();
        }

        #endregion

        private void InitSessionFactory()
        {
            NHibernate.Cfg.Configuration cfg = new NHibernate.Cfg.Configuration ();
            cfg.Configure ();

            _sessionFactory = cfg.BuildSessionFactory ();
        }

        /// <summary>
        /// Gets or sets the NHibernate IInterceptor that must be used in every ISession that is opened/created via the
        /// SessionManager.
        /// </summary>
        public IInterceptor DefaultInterceptor
        {
            get;
            set;
        }

        /// <summary>
        /// Starts a new Session
        /// </summary>
        /// <remarks>This method needs to be called before you can use the Session.  Nested Sessions are not supported</remarks>
        /// <exception cref="InvalidOperationException">An InvalidOperationException is thrown when StartSession is called while
        /// there is already another Session active.</exception>
        public void StartSession()
        {
            ISession session = ContextSession;

            if( session == null )
            {
                if( DefaultInterceptor != null )
                {
                    session = _sessionFactory.OpenSession (DefaultInterceptor);
                }
                else
                {
                    session = _sessionFactory.OpenSession ();
                }

                ContextSession = session;
            }
            else
            {
                throw new InvalidOperationException ("There is already a Session running, nested Sessions are not supported.");
            }

            Check.Ensure (session != null, "The session is null");            
        }

        /// <summary>
        /// Gets the ISession object that is currently active.
        /// </summary>
        /// <exception cref="InvalidOperationException">An InvalidOperationException is thrown when there is no Session active.</exception>        
        public ISession Session
        {
            get
            {
                ISession session = ContextSession;

                if( session == null )
                {
                    throw new InvalidOperationException (@"Currently, there's no Session active.  Did you forget to call StartSession?");
                }

                return session;
            }
        }        

        /// <summary>
        /// Flushes anything left in the session and closes the connection.
        /// </summary>
        public void CloseSession()
        {
            ISession session = ContextSession;

            if( session != null && session.IsOpen )
            {
                session.Flush ();
                session.Close ();
            }

            ContextSession = null;
        }

        /// <summary>
        /// Starts a new Transaction.
        /// </summary>
        /// <remarks>When this call is made when no Session is currently active, the SessionManager will make sure
        /// that a new Session is started.</remarks>
        public void BeginTransaction()
        {
            ITransaction transaction = ContextTransaction;

            if( transaction == null )
            {
                ISession session = ContextSession;

                if( session == null )
                {
                    this.StartSession ();
                }

                transaction = this.Session.BeginTransaction ();
                ContextTransaction = transaction;
            }
            else
            {
                throw new InvalidOperationException ("There is already a Transaction active, nested transactions are not supported.");
            }
        }

        public void CommitTransaction()
        {
            ITransaction transaction = ContextTransaction;

            try
            {
                if( IsInTransaction () )
                {
                    transaction.Commit ();
                    ContextTransaction = null;
                }
            }
            catch( HibernateException )
            {
                RollbackTransaction ();
                throw;
            }
        }

        public bool IsInTransaction()
        {
            ITransaction transaction = ContextTransaction;

            return transaction != null && !transaction.WasCommitted && !transaction.WasRolledBack;
        }

        public void RollbackTransaction()
        {
            ITransaction transaction = ContextTransaction;

            try
            {
                if( IsInTransaction () )
                {
                    transaction.Rollback ();
                }

                ContextTransaction = null;
            }
            finally
            {
                CloseSession ();
            }
        }

        /// <summary>
        /// If within a web context, this uses <see cref="HttpContext" /> instead of the WinForms 
        /// specific <see cref="CallContext" />.  Discussion concerning this found at 
        /// http://forum.springframework.net/showthread.php?t=572.
        /// </summary>
        private ITransaction ContextTransaction
        {
            get
            {
                if( IsInWebContext () )
                {
                    return (ITransaction)HttpContext.Current.Items[TRANSACTION_KEY];
                }
                else
                {
                    return (ITransaction)CallContext.GetData (TRANSACTION_KEY);
                }
            }
            set
            {
                if( IsInWebContext () )
                {
                    HttpContext.Current.Items[TRANSACTION_KEY] = value;
                }
                else
                {
                    CallContext.SetData (TRANSACTION_KEY, value);
                }
            }
        }

        /// <summary>
        /// If within a web context, this uses <see cref="HttpContext" /> instead of the WinForms 
        /// specific <see cref="CallContext" />.  Discussion concerning this found at 
        /// http://forum.springframework.net/showthread.php?t=572.
        /// </summary>
        private ISession ContextSession
        {
            get
            {
                if( IsInWebContext () )
                {
                    return (ISession)HttpContext.Current.Items[SESSION_KEY];
                }
                else
                {
                    return (ISession)CallContext.GetData (SESSION_KEY);
                }
            }
            set
            {
                if( IsInWebContext () )
                {
                    HttpContext.Current.Items[SESSION_KEY] = value;
                }
                else
                {
                    CallContext.SetData (SESSION_KEY, value);
                }
            }
        }

        private static bool IsInWebContext()
        {
            return HttpContext.Current != null;
        }

        private const string TRANSACTION_KEY = "CONTEXT_TRANSACTION";
        private const string SESSION_KEY = "CONTEXT_SESSION";
        private ISessionFactory _sessionFactory;


    }
}
