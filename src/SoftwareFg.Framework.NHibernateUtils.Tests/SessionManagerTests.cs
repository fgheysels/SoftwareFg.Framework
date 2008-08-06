using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;

using NHibernate.Cfg;
using NUnit.Framework;
using SoftwareFg.Framework.NHibernateUtils.Tests.TestEntities;
using System.IO;
using SoftwareFg.Framework.NHibernateUtils.Tests.Utils;
using NHibernate.Criterion;

namespace SoftwareFg.Framework.NHibernateUtils.Tests
{
    [TestFixture]
    public class SessionManagerTests
    {

        [TestFixtureSetUp]
        public void CreateTestDatabase()
        {
            log4net.Config.XmlConfigurator.Configure ();

            Database.CreateTable (this.GetCreatePersonsTableSqlString ());

            Database.Close ();
        }

        [Test]
        public void CreateAndReadPerson()
        {
            Person p = new Person ();

            p.Name = "Frederik";
            p.Sex = Sex.Male;
            p.DateOfBirth = new DateTime (1978, 12, 14);

            Person p2 = new Person ();

            p2.Name = "Barbara";
            p2.Sex = Sex.Female;
            p2.DateOfBirth = new DateTime (1978, 2, 6);



            SessionManager.Instance.BeginTransaction ();

            SessionManager.Instance.UnitOfWork.Save (p);
            SessionManager.Instance.UnitOfWork.Save (p2);

            SessionManager.Instance.CommitTransaction ();

            Assert.AreNotEqual (p, p2, "The 2 persons are not the same");


            IList<Person> ps = SessionManager.Instance.UnitOfWork.
                            CreateCriteria (typeof (Person)).Add (Expression.Eq ("Name", "Frederik")).List<Person> ();

            Assert.AreEqual (1, ps.Count, "There should be only one item in the collection.");

            Assert.AreEqual (p, ps[0], "The 2 object should be equal.");

            Assert.AreNotEqual (ps[0], p2, "the 2 objects should not be equal.");

            ps[0].Name = "melp";

            SessionManager.Instance.UnitOfWork.SaveOrUpdate (ps[0]);
            SessionManager.Instance.UnitOfWork.Flush ();

            SessionManager.Instance.CloseUnitOfWork ();


        }

        [TestFixtureTearDown]
        public void DropTestDatabase()
        {

            Database.DropTable ("Persons");

            Database.Close ();
        }

        #region Utility Methods to create Database tables

        private string GetCreatePersonsTableSqlString()
        {
            return
                "IF NOT EXISTS ( SELECT * FROM sysobjects WHERE [name] = \'Persons\' ) \n" +
                "BEGIN \n" +
                "   CREATE TABLE Persons ( \n" +
                "   [PersonId] INT IDENTITY (1,1), \n" +
                "   [Name] VARCHAR(50), \n" +
                "   [SEX] INT, \n" +
                "   [DateOfBirth] DATETIME ) \n" +
                "END";
        }

        #endregion

    }
}
