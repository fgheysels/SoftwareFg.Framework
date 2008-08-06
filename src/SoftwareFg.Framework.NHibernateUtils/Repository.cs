using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Type;
using NHibernate;
using NHibernate.Criterion;

namespace SoftwareFg.Framework.NHibernateUtils
{
    /// <summary>
    /// Repository that contains basic functionality for saving, deleting, retrieving entities via NHibernate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepository<T>
    {
        /// <summary>
        /// Gets the UnitOfWork that is used by this Repository instance
        /// </summary>
        protected UnitOfWork Uow
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Creates a Repository instance which uses the given UnitOfWork
        /// </summary>
        /// <param name="uow"></param>
        public Repository( UnitOfWork uow )
        {
            this.Uow = uow;
        }

        #region IRepository<T> Members

        /// <summary>
        /// Gets the entity that matches the given ID from the datastore.
        /// </summary>
        /// <remarks>If there exists no entity that matches the given ID, NULL is returned.</remarks>
        /// <param name="id"></param>
        /// <returns>The entity or NULL if there exists no entity for the given id.</returns>
        public T Get( object id )
        {
            return Uow.Get<T> (id);
        }

        /// <summary>
        /// Loads the entity that matches the given ID from the datastore.
        /// An exception is thrown when there exists no entity for the given ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The entity that matches the given id.</returns>
        public T Load( object id )
        {
            return Uow.Session.Load<T> (id);
        }

        /// <summary>
        /// Deletes the given entity from the datastore.
        /// </summary>
        /// <param name="entity">The entity that must be deleted</param>
        public void Delete( T entity )
        {
            Uow.Delete (entity);
        }

        public int DeleteAll( string query )
        {
            return Uow.Session.Delete (query);
        }
       
        /// <summary>
        /// Saves the given entity to the datastore (INSERT)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T Save( T entity )
        {
            Uow.Save (entity);

            return entity;
        }

        /// <summary>
        /// Saves or Updates the given entity to the datastore.
        /// </summary>
        /// <remarks>Depending on the ID of the given entity, an INSERT or an UPDATe is performed.</remarks>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T SaveOrUpdate( T entity )
        {
            Uow.SaveOrUpdate (entity);

            return entity;
        }

        public T SaveOrUpdateCopy( T entity )
        {
            Uow.SaveOrUpdateCopy (entity);

            return entity;
        }

        /// <summary>
        /// Updates the given entity
        /// </summary>
        /// <param name="entity"></param>
        public void Update( T entity )
        {
            Uow.Update (entity);
        }

        /// <summary>
        /// Retrieve a list of all the entities
        /// </summary>
        /// <returns></returns>
        public ICollection<T> FindAll()
        {
            return Uow.Session.CreateCriteria (typeof (T)).List<T> ();
        }

        /// <summary>
        /// Retrieve a list of entities, with paging support.
        /// </summary>
        /// <param name="firstResult"></param>
        /// <param name="numberOfResults"></param>
        /// <returns></returns>
        public ICollection<T> FindAll( int firstResult, int numberOfResults )
        {
            ICriteria crit = Uow.Session.CreateCriteria (typeof (T));

            crit.SetFirstResult (firstResult);
            crit.SetMaxResults (numberOfResults);

            return crit.List<T> ();
        }
        
        public ICollection<T> FindAll( NHibernate.ICriteria criteria )
        {
            return criteria.List<T> ();
        }

        public ICollection<T> FindAll( NHibernate.ICriteria criteria, int firstResult, int numberOfResults )
        {
            return criteria.SetFirstResult (firstResult).SetMaxResults (numberOfResults).List<T> ();            
        }

        public ICollection<T> FindAll( DetachedCriteria criteria )
        {
            return criteria.GetExecutableCriteria (Uow.Session).List<T> ();
        }

        public ICollection<T> FindAll( DetachedCriteria criteria, int firstResult, int numberOfResults )
        {
            ICriteria crit = criteria.GetExecutableCriteria (Uow.Session);
            
            crit.SetFirstResult (firstResult);
            crit.SetMaxResults (numberOfResults);

            return crit.List<T> ();
        }

        #endregion
    }
}
