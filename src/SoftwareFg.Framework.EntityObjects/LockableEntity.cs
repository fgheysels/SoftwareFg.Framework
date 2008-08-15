using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace SoftwareFg.Framework.EntityObjects
{
    [Serializable]
    public abstract class LockableEntity<TId> : AuditableEntity<TId>, ILockable
    {
        
        public event EventHandler<LockedPropertyChangedAttemptEventArgs> LockedPropertyChangeAttempt;

        private IDictionary<string, LockInfo> _locks = new Dictionary<string, LockInfo> (StringComparer.InvariantCultureIgnoreCase);

        public abstract object GetEntityId();

        public void Lock( string propertyName )
        {

            CheckIfPropertyExistsAndIfPropertyIsLockable (propertyName);

            if( IsLocked (propertyName) )
            {
                throw new InvalidOperationException (String.Format ("There already exists a lock on {0}", propertyName));
            }
            
            LockInfo lockInfoObj = new LockInfo (GetEntityId(), 
                                                 propertyName, 
                                                 this.GetType().Name, 
                                                 Thread.CurrentPrincipal.Identity.Name, 
                                                 DateTime.Now);

            _locks.Add (propertyName, lockInfoObj);
        }

        private IDictionary<string, PropertyInfo> _properties;

        private void CheckIfPropertyExistsAndIfPropertyIsLockable( string propertyName )
        {
            if( _properties == null )
            {
                _properties = new Dictionary<string, PropertyInfo> (StringComparer.InvariantCultureIgnoreCase);

                PropertyInfo[] props = this.GetType ().GetProperties (BindingFlags.Instance | BindingFlags.Instance | ~BindingFlags.DeclaredOnly);

                foreach( PropertyInfo p in props )
                {
                    _properties.Add (p.Name, p);
                }
            }

            if( _properties.ContainsKey (propertyName) == false )
            {
                throw new ArgumentException (String.Format ("{0} is not a valid property for {1}", propertyName, this.GetType ().Name));                
            }            
        }

        public void Unlock( string propertyName )
        {
            if( IsLocked (propertyName) )
            {
                _locks.Remove (propertyName);
            }
        }

        public bool IsLocked( string propertyName )
        {
            return _locks.ContainsKey (propertyName);
        }

        protected internal void OnAttemptToModifyLockedProperty( string propertyName, object existingValue, object proposedValue )
        {
            if( IsLocked (propertyName) == false )
            {
                throw new InvalidOperationException (String.Format ("property {0} is not locked."));
            }

            if( LockedPropertyChangeAttempt != null )
            {
                LockInfo lockInfoObj = _locks[propertyName];

                LockedPropertyChangeAttempt (this, new LockedPropertyChangedAttemptEventArgs (lockInfoObj, existingValue, proposedValue));

            }
        }

    }
}
