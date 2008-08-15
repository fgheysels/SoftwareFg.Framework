using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace SoftwareFg.Framework.EntityObjects.Tests
{
    [TestFixture]
    public class LockableEntityTests
    {
        private class LockablePerson : LockableEntity<int>
        {
            [Lockable]
            public string Name
            {
                get;
                set;
            }

            public override object GetEntityId()
            {
                return this.Id;
            }
        }

        [Test]
        public void CanLockProperties()
        {
            LockablePerson p = new LockablePerson ();

            p.Name = "Somename";

            p.Lock ("Name");

            Assert.IsTrue (p.IsLocked ("Name"));
        }

        [Test]
        public void CanUnlockProperties()
        {
            LockablePerson p = new LockablePerson ();

            p.Name = "Somename";

            p.Lock ("Name");

            Assert.IsTrue (p.IsLocked ("Name"));

            p.Unlock ("name");

            Assert.IsFalse (p.IsLocked ("Name"));
        }

        [Test]
        public void CannotModifyLockedProperty()
        {
            LockablePerson p = new LockablePerson ();
            
            p.Name = "Somename";

            p.Lock ("Name");

            p.Name = "AnotherName";

            Assert.AreEqual ("Somename", p.Name, "The name of the person has been locked and should not have been changed.");
        }

        [Test]
        public void CanModifyUnlockedProperty()
        {
            LockablePerson p = new LockablePerson ();
            
            p.Name = "Somename";

            p.Lock ("name");

            p.Unlock ("name");

            p.Name = "anothername";

            Assert.AreEqual ("anothername", p.Name, "The name of the person should have been modified, since it is no longer locked.");
        }

        private bool _lockedPropertyChangeAttemptEventHandlerEntered;

        [SetUp]
        public void ResetLockedPropertyChangedAttemptFlag()
        {
            _lockedPropertyChangeAttemptEventHandlerEntered = false;
        }

        [Test]
        public void AttemptToModifyLockedPropertyIsReported()
        {
            LockablePerson p = new LockablePerson ();

            p.LockedPropertyChangeAttempt += new EventHandler<LockedPropertyChangedAttemptEventArgs> (OnLockedPropertyChangeAttempt);

            p.Name = "Frederik";

            p.Lock ("Name");

            p.Name = "test";

            Assert.IsTrue (_lockedPropertyChangeAttemptEventHandlerEntered, "The attempt to modify the locked property is not reported.");
        }

        public void OnLockedPropertyChangeAttempt( object sender, LockedPropertyChangedAttemptEventArgs e )
        {
            _lockedPropertyChangeAttemptEventHandlerEntered = true;

            Assert.AreEqual ("Frederik", e.ExistingPropertyValue);
            Assert.AreEqual ("test", e.ProposedPropertyValue);

        }
    }
}
