using System;
using NHibernate;
using SoftwareFg.Framework.EntityObjects;

namespace SoftwareFg.Framework.NHibernateUtils
{
    public class AuditInterceptor : IInterceptor
    {       

        #region IInterceptor Members

        public void AfterTransactionBegin( ITransaction tx )
        {
            // do nothing
        }

        public void AfterTransactionCompletion( ITransaction tx )
        {
            // do nothing
        }

        public void BeforeTransactionCompletion( ITransaction tx )
        {
            // do nothing
        }

        public int[] FindDirty( object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types )
        {
            return null;
        }

        public object Instantiate( Type type, object id )
        {
            return null;
        }

        public object IsUnsaved( object entity )
        {
            return null;
        }

        public void OnDelete( object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types )
        {
            // do nothing
        }

        public bool OnFlushDirty( object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types )
        {
            IAuditable auditableObject = entity as IAuditable;

            if( auditableObject != null )
            {
                for( int i = 0; i < propertyNames.Length; i++ )
                {
                    if( propertyNames[i] == auditableObject.UpdatedPropertyName )
                    {
                        currentState[i] = DateTime.Now;
                    }
                }
                return true;
            }

            return false;
        }

        public bool OnLoad( object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types )
        {
            return false;
        }

        public bool OnSave( object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types )
        {
            IAuditable auditableObject = entity as IAuditable;

            if( auditableObject != null )
            {
                DateTime currentDate = DateTime.Now;               

                //auditableObject.SetCreationDate (currentDate);
                //auditableObject.SetUpdateDate (currentDate);

                for( int i = 0; i < propertyNames.Length; i++ )
                {
                    if( propertyNames[i] == auditableObject.CreatedPropertyName )
                    {
                        state[i] = currentDate;
                    }
                    if( propertyNames[i] == auditableObject.UpdatedPropertyName )
                    {
                        state[i] = currentDate;
                    }
                }

                System.Diagnostics.Debug.WriteLine ("interceptor: created: " + auditableObject.Created);
                System.Diagnostics.Debug.WriteLine ("interceptor: updated: " + auditableObject.Updated);

                return true;
            }

            return false;
        }

        public void PostFlush( System.Collections.ICollection entities )
        {
            // do nothing
        }

        public void PreFlush( System.Collections.ICollection entities )
        {
            // do nothing
        }

        public void SetSession( ISession session )
        {
            // do nothing.
        }

        #endregion
    }
}
