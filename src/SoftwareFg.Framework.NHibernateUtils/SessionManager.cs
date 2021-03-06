﻿using System;
using System.Runtime.Remoting.Messaging;
using System.Web;
using NHibernate;
using SoftwareFg.Framework;


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
            static Nested()
            {
                // static constructor is necessary so that the compiler will not
                // mark the type as beforefieldinit.
            }

            internal static readonly SessionManager SessionManager = new SessionManager ();
        }

        #endregion

        private void InitSessionFactory()
        {            
        }

        /// <summary>
        /// Gets or sets the NHibernate IInterceptor that must be used in every ISession that is opened/created via the
        /// SessionManager.
        /// </summary>
        public IInterceptor DefaultInterceptor
        {
            get
            {
                return UnitOfWorkFactory.Instance.DefaultInterceptor;
            }
            set
            {
                UnitOfWorkFactory.Instance.DefaultInterceptor = value;
            }
        }

        /// <summary>
        /// Starts a new Session
        /// </summary>
        /// <remarks>This method needs to be called before you can use the Session.  Nested Sessions are not supported</remarks>
        /// <exception cref="InvalidOperationException">An InvalidOperationException is thrown when StartUnitOfWork is called while
        /// there is already another Session active.</exception>
        public void StartUnitOfWork()
        {
            UnitOfWork uow = ContextUnitOfWork;

            if( uow == null )
            {
                uow = UnitOfWorkFactory.Instance.CreateUnitOfWork ();
                
                ContextUnitOfWork = uow;
            }
            else
            {
                throw new InvalidOperationException ("There is already a UnitOfWork running, nested UnitOfWorks are not supported.");
            }

            Check.Ensure (uow != null, "The UnitOfWork is null");
        }

        /// <summary>
        /// Gets the ISession object that is currently active.
        /// The ISession that is returned, is in a connected state.
        /// </summary>
        /// <exception cref="InvalidOperationException">An InvalidOperationException is thrown when there is no Session active.</exception>        
        public UnitOfWork UnitOfWork
        {
            get
            {
                UnitOfWork uow = ContextUnitOfWork;

                if( uow == null )
                {
                    throw new InvalidOperationException (@"Currently, there's no UnitOfWork active.  Did you forget to call StartUnitOfWork?");
                }

                uow.Reconnect ();
                
                return uow;
            }
        }

        /// <summary>
        /// Flushes the ISession so that all changes are committed to the DB.
        /// </summary>
        public void Flush()
        {            
            UnitOfWork uow = ContextUnitOfWork;

            if( uow != null && uow.IsOpen )
            {
                uow.Flush ();
            }
        }

        /// <summary>
        /// Disconnects the Session from the Database.
        /// </summary>
        /// <remarks>Disconnecting a Session means that only the underlying DB connection will be closed.  The Session itself
        /// will remain open.</remarks>
        public void Disconnect()
        {            
            UnitOfWork uow = ContextUnitOfWork;

            if( uow != null && uow.IsOpen )
            {
                uow.Disconnect ();
            }
        }

        /// <summary>
        /// Closes the session, but doesn't flush it.  When you do not use transactions (Committing a transaction flushes the session),
        /// you should use call Flush yourselves.
        /// </summary>
        public void CloseUnitOfWork()
        {
            UnitOfWork uow = ContextUnitOfWork;

            if( uow != null && uow.IsOpen )
            {
                uow.Close ();
            }

            ContextUnitOfWork = null;
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
                UnitOfWork uow = ContextUnitOfWork;

                if( uow == null )
                {
                    this.StartUnitOfWork ();
                }

                transaction = this.UnitOfWork.Session.BeginTransaction ();
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
                    Flush ();

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
                CloseUnitOfWork ();
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
        private UnitOfWork ContextUnitOfWork
        {
            get
            {
                if( IsInWebContext () )
                {
                    return (UnitOfWork)HttpContext.Current.Items[SESSION_KEY];
                }
                else
                {
                    return (UnitOfWork)CallContext.GetData (SESSION_KEY);
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


    }
}
