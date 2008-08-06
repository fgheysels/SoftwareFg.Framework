using System;
using NHibernate;
using SoftwareFg.Framework.EntityObjects;

namespace SoftwareFg.Framework.NHibernateUtils
{
    public class AuditInterceptor : EmptyInterceptor
    {

        public override bool OnSave( object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types )
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

        public override bool OnFlushDirty( object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types )
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
        
    }
}
