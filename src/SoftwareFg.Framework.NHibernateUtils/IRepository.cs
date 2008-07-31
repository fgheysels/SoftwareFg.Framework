using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Expression;
using NHibernate;

namespace SoftwareFg.Framework.NHibernateUtils
{
    public interface IRepository<T>
    {
        /// <summary>
        /// Get the entity from the persistence store, or return NULL if it doesn't exist.
        /// </summary>
        /// <param name="id">The ID of the entity</param>
        /// <returns>The entity that matches the given Id.  If none was found, NULL is returned</returns>
        T Get( object id );

        /// <summary>
        /// Load the entity from the persistence store, or throw an exception if there doesn't exists an entity
        /// for the given id
        /// </summary>
        /// <param name="id">The ID of the entity that must be retrieved</param>
        /// <returns>The entity that matches the given Id</returns>
        T Load( object id );

        void Delete( T entity );

        int DeleteAll( string query );

        T Save( T entity );

        T SaveOrUpdate( T entity );

        T SaveOrUpdateCopy( T entity );

        void Update( T entity );

        ICollection<T> FindAll();

        ICollection<T> FindAll( int firstResult, int numberOfResults );

        ICollection<T> FindAll( ICriteria criteria );

        ICollection<T> FindAll( ICriteria criteria, int firstResult, int numberOfResults );

        ICollection<T> FindAll( DetachedCriteria criteria );

        ICollection<T> FindAll( DetachedCriteria criteria, int firstResult, int numberOfResults );


    }
}
