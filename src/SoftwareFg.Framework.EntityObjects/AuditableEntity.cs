using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace SoftwareFg.Framework.EntityObjects
{
    [Serializable]
    public abstract class AuditableEntity<TId> : Entity<TId>, IAuditable
    {
        public DateTime Created
        {
            get;
            // Private setter is required so that NHibernate can access it.
            private set;
        }

        public DateTime Updated
        {
            get;
            // Private setter is required so that NHibernate can access it.
            private set;
        }

        public int Version
        {
            get;
            // Private setter is required so that NHibernate can access it.
            private set;
        }

        // The Setxxx methods are explicitly implemented, so that they're a bit hidden for the user.

        void IAuditable.SetCreationDate( DateTime created )
        {
            this.Created = created;            
        }

        void IAuditable.SetUpdateDate( DateTime updated )
        {
            this.Updated = updated;            
        }

        void IAuditable.SetVersion( int version )
        {
            this.Version = version;
        }

        string IAuditable.CreatedPropertyName
        {
            get
            {
                string createdPropName = "Created";

                #if DEBUG

                CheckIfPropertyExists (createdPropName);

                #endif

                return createdPropName;
            }
        }

        string IAuditable.UpdatedPropertyName
        {
            get
            {
                string updatedPropName = "Updated";

                #if DEBUG

                CheckIfPropertyExists (updatedPropName);

                #endif

                return updatedPropName;
            }
        }

        private void CheckIfPropertyExists( string propertyName )
        {
            PropertyInfo pi = this.GetType ().GetProperty (propertyName);

            Debug.Assert (pi != null, String.Format ("There exists no property {0}", propertyName));
        }
    }
}
