using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareFg.Framework.EntityObjects
{
    public interface ILockable
    {
        object GetEntityId();

        void Lock( string propertyName );

        void Unlock( string propertyName );

        bool IsLocked( string propertyName );
    }
}
