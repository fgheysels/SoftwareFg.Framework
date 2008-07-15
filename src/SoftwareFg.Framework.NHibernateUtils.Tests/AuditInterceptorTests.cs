using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using SoftwareFg.Framework.NHibernateUtils.Tests.Utils;
using SoftwareFg.Framework.NHibernateUtils.Tests.TestEntities;

namespace SoftwareFg.Framework.NHibernateUtils.Tests
{
    [TestFixture]
    public class AuditInterceptorTests
    {
        [TestFixtureSetUp]
        public void CreateTables()
        {
            log4net.Config.XmlConfigurator.Configure ();

            Database.CreateTable (this.GetCreateAuditablePersonTableStatement ());

            Database.Close ();
        }

        [Test]
        public void TestAuditProperties()
        {
            AuditPerson p = new AuditPerson ();
            p.Name = "Frederik";
            p.DateOfBirth = new DateTime (1978, 12, 14);

            SessionManager.Instance.DefaultInterceptor = new AuditInterceptor ();            

            SessionManager.Instance.GetSession ().Save (p);

            Console.WriteLine (p.Name + " " + p.DateOfBirth + " " + p.Created + " " + p.Updated);

            SessionManager.Instance.GetSession ().Flush ();

            SessionManager.Instance.CloseSession ();
        }

        [Test]
        public void TestCastleIoCInterceptor()
        {
        }

        [TestFixtureTearDown]
        public void DropTables()
        {
        }

        private string GetCreateAuditablePersonTableStatement()
        {
            return "IF NOT EXISTS ( SELECT * FROM sysobjects WHERE [name] = \'AuditPerson\' ) \n" +
                "BEGIN \n" +
                "       CREATE TABLE AuditPerson ( \n" +
                "       PersonId INT IDENTITY (1,1), \n" +
                "       Name VARCHAR(50), \n" +
                "       Dob DATETIME, \n" +
                "       CreationDate DATETIME, \n" +
                "       LastUpdatedDate DATETIME, \n" +
                "       Version INT) \n" +
                "END";
        }
    }
}
