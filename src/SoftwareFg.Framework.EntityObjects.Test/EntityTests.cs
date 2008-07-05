using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using SoftwareFg.Framework.EntityObjects;

namespace SoftwareFg.Framework.EntityObjects.Tests
{
    [TestFixture]
    public class EntityTests
    {

        private class Person : Entity<int>
        {
            public Person( int id )
            {
                Id = id;
            }
        }

        private class Animal : Entity<int>
        {
            public Animal( int id )
            {
                Id = id;
            }
        }

        [Test]
        public void TestDifferentEntityObjectsForEquality()
        {
            Person p = new Person (21);

            Animal a = new Animal (21);

            Assert.IsFalse (p.Equals (a), "p should not be equal to a, since the 2 objects are of a different type");
        }

        [Test]
        public void TestEntityObjectsEqualityUsingOverloadedOperators()
        {
            Person p = new Person (1);
            Person p2 = new Person (1);

            Assert.IsTrue (p == p2, "The 2 objects should be considered equal, since they have the same ID.");
        }

        [Test]
        public void TextEntityObjectsInequalityUsingOverloadedOperators()
        {
            Person p = new Person (1);
            Person p2 = new Person (2);

            Assert.IsTrue (p != p2, "The 2 objects should not be considered equal, since they have different ids.");
        }

    }
}
