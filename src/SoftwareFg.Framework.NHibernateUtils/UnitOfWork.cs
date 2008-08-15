using System;
using System.Data;
using NHibernate;

namespace SoftwareFg.Framework.NHibernateUtils
{
    public sealed class UnitOfWork : IDisposable
    {

        #region Members

        /// <summary>
        /// Contains the ITransaction if a Transaction is active.
        /// </summary>
        private ITransaction _currentTransaction;

        /// <summary>
        /// Gets the NHibernate ISession that is used by this UnitOfWork.
        /// </summary>
        public ISession Session
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a UnitOfWork instance.
        /// </summary>
        /// <param name="session"></param>
        internal UnitOfWork( ISession session )
        {
            this.Session = session;
        }

        #endregion

        /// <summary>
        /// Gets an indication whether the UnitOfWork is still open or if it has already been closed.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return Session.IsOpen;
            }
        }

        /// <summary>
        /// Disconnects the UnitOfWork from the DB.          
        /// </summary>
        /// <remarks>The underlying database connection is closed, but the UnitOfWork remains useable.</remarks>
        public void Disconnect()
        {
            if( IsInTransaction )
            {
                throw new InvalidOperationException ("The UnitOfWork cannot be disconnected when we are in a transaction.");
            }

            if( Session.IsConnected )
            {
                Session.Disconnect ();
            }
        }

        /// <summary>
        /// Reconnects the UnitOfWork to the DB.
        /// </summary>
        public void Reconnect()
        {
            if( Session.IsConnected == false )
            {
                Session.Reconnect ();
            }
        }

        /// <summary>
        /// Gets an indication whether there is a Transaction active.
        /// </summary>
        public bool IsInTransaction
        {
            get
            {
                return _currentTransaction != null;
            }
        }

        /// <summary>
        /// Gets an indication whether the UnitOfWork contains dirty entities.  
        /// </summary>
        /// <returns>True if the UnitOfWork contains entities that have changes.</returns>
        public bool IsDirty()
        {
            return this.Session.IsDirty ();
        }

        /// <summary>
        /// Flushes the changes that have been made to the UnitOfWork to the DB.
        /// </summary>
        /// <remarks>You need to call this method when you want to commit the changes to the Database and when you're not
        /// using a Transaction.</remarks>
        public void Flush()
        {
            bool isConnected = Session.IsConnected;

            if( isConnected == false )
            {
                Session.Reconnect ();
            }

            Session.Flush ();

            if( isConnected == false )
            {
                Session.Disconnect ();
            }
        }

        /// <summary>
        /// Starts a Transaction with the ReadCommitted isolation level.
        /// </summary>
        public void BeginTransaction()
        {
            BeginTransaction (IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// Starts a Transaction with the given isolation level.
        /// </summary>
        /// <param name="iso"></param>
        public void BeginTransaction( IsolationLevel iso )
        {
            if( _currentTransaction != null )
            {
                throw new InvalidOperationException ("There is already a Transaction active.  Nested transactions are not supported.");
            }

            _currentTransaction = Session.BeginTransaction (iso);

        }

        /// <summary>
        /// Commits the Transaction
        /// </summary>
        public void CommitTransaction()
        {
            if( _currentTransaction == null )
            {
                throw new InvalidOperationException ("There is no Transaction active.  Cannot Commit when no transaction has started.");
            }

            _currentTransaction.Commit ();
            _currentTransaction = null;

        }

        /// <summary>
        /// Rollback the transaction
        /// </summary>
        public void RollbackTransaction()
        {
            if( _currentTransaction == null )
            {
                throw new InvalidOperationException ("There is no Transaction active.  Cannot Rollback when no transaction has started.");
            }

            _currentTransaction.Rollback ();
            _currentTransaction = null;
        }

        /// <summary>
        /// Remove an object from the UnitOfWork.
        /// </summary>
        /// <param name="entity"></param>
        public void RemoveFromUnitOfWork( object entity )
        {
            Session.Evict (entity);
        }

        /// <summary>
        /// Returns the persistent entity of the given type that has the given identifier.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Get<T>( object id )
        {
            return Session.Get<T> (id);
        }

        /// <summary>
        /// Checks whether the UnitOfWork contains the given object.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Contains( object entity )
        {
            return Session.Contains (entity);
        }
        
        /// <summary>
        /// Deletes the given object from the DataStore
        /// </summary>
        /// <param name="entity"></param>
        public void Delete( object entity )
        {
            Session.Delete (entity);
        }

        public void Update( object entity, object id )
        {
            Session.Update (entity, id);
        }

        public void Update( object entity )
        {
            Session.Update (entity);
        }

        public void Save( object entity, object id )
        {
            Session.Save (entity, id);
        }

        public void Save( object entity )
        {
            Session.Save (entity);
        }

        public void SaveOrUpdate( object entity )
        {
            Session.SaveOrUpdate (entity);
        }

        public object SaveOrUpdateCopy( object entity )
        {
            return Session.SaveOrUpdateCopy (entity);
        }

        public T SaveOrUpdateCopy<T>( T entity )
        {
            return (T)Session.SaveOrUpdateCopy (entity);
        }

        public object SaveOrUpdateCopy( object entity, object id )
        {
            return Session.SaveOrUpdateCopy (entity, id);
        }

        public T SaveOrUpdateCopy<T>( T entity, object id )
        {
            return (T)Session.SaveOrUpdateCopy (entity, id);
        }

        public IQuery CreateQuery( string queryString )
        {            
            return Session.CreateQuery (queryString);
        }

        public ICriteria CreateCriteria( Type persistentClass )
        {
            return Session.CreateCriteria (persistentClass);
        }

        public ICriteria CreateCriteria(Type persistentClass, string alias)
        {            
            return Session.CreateCriteria (persistentClass, alias);
        }

        public void Clear()
        {
            Session.Clear ();
        }

        public void Close()
        {
            Session.Close ();
        }

        #region Dispose pattern.

        private bool _isDisposed = false;

        ~UnitOfWork()
        {            
            Dispose (false);
        }

        public void Dispose()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }


        private void Dispose( bool disposing )
        {
            if( !_isDisposed )
            {
                if( disposing )
                {
                    Session.Dispose ();

                    if( _currentTransaction != null )
                    {
                        _currentTransaction.Dispose ();
                    }
                }
            }

            _isDisposed = true;
        }

        #endregion
    }
}
