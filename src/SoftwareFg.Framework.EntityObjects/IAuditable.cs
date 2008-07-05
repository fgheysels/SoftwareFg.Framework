using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareFg.Framework.EntityObjects
{
    public interface IAuditable
    {
        DateTime Created
        {
            get;        
        }

        DateTime Updated
        {
            get;            
        }

        int Version
        {
            get;            
        }

        void SetCreationDate( DateTime created );

        void SetUpdateDate( DateTime updated );

        void SetVersion( int version );

        string CreatedPropertyName
        {
            get;
        }

        string UpdatedPropertyName
        {
            get;
        }
    }
}
