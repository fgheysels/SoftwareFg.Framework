using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareFg.Framework.EntityObjects
{
    [Serializable]
    public class LockInfo : IEquatable<LockInfo>
    {
        public object EntityId
        {
            get;
            private set;
        }

        public string LockedPropertyName
        {
            get;
            private set;
        }

        public string EntityName
        {
            get;
            private set;
        }

        public string LockedBy
        {
            get;
            private set;
        }

        public DateTime LockCreationDate
        {
            get;
            private set;
        }

        
        public LockInfo( object id, string propName, string entityName, string lockedBy, DateTime lockDate ) 
        {
            EntityId = id;
            LockedPropertyName = propName;
            EntityName = entityName;
            LockedBy = lockedBy;
            LockCreationDate = lockDate;
        }

        #region IEquatable<LockInfo> Members

        public override int GetHashCode()
        {
            return ( GetType ().FullName + 
                     "|entityid" + this.EntityId + 
                     "|entityname" + EntityName.ToLower () + 
                     "|propname" + LockedPropertyName.ToLower () ).GetHashCode ();
        }

        public override sealed bool Equals( object obj )
        {
            LockInfo other = obj as LockInfo;

            if( other == null )
            {
                return false;
            }
            else
            {
                return this.Equals (other);
            }
        }

        public bool Equals( LockInfo other )
        {
            return this.GetHashCode () == other.GetHashCode ();            
        }

        #endregion
        
    }
}
