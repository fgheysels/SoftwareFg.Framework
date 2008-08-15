using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareFg.Framework.EntityObjects
{
    [Serializable]
    public class LockedPropertyChangedAttemptEventArgs : EventArgs
    {
        public LockInfo LockInfo
        {
            get;
            private set;
        }

        public object ExistingPropertyValue
        {
            get;
            private set;
        }

        public object ProposedPropertyValue
        {
            get;
            private set;
        }

        public LockedPropertyChangedAttemptEventArgs( LockInfo lockInfoObj, object existingPropertyValue, object proposedPropertyValue )
        {
            this.LockInfo = lockInfoObj;
            this.ExistingPropertyValue = existingPropertyValue;
            this.ProposedPropertyValue = proposedPropertyValue;
        }
    }
}
