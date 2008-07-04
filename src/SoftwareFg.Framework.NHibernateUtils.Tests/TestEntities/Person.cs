﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftwareFg.Framework.EntityObjects;

namespace SoftwareFg.Framework.NHibernateUtils.Tests.TestEntities
{
    public class Person : Entity<int>
    {
        public string Name
        {
            get;
            set;
        }

        public DateTime DateOfBirth
        {
            get;
            set;
        }

        public Sex Sex
        {
            get;
            set;
        }
    }
}
