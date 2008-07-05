﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;

using NHibernate.Cfg;
using NUnit.Framework;
using SoftwareFg.Framework.NHibernateUtils.Tests.TestEntities;
using NHibernate.Expression;
using System.IO;

namespace SoftwareFg.Framework.NHibernateUtils.Tests
{
    [TestFixture]
    public class SessionManagerTests
    {

        private string _connectionString;

        [TestFixtureSetUp]
        public void CreateTestDatabase()
        {            
            log4net.Config.XmlConfigurator.Configure ();

            // Create an NHibernate Configuration object just to get the connection-string that we're using.
            Configuration cfg = new Configuration ();
            cfg.Configure ();

            _connectionString = cfg.Properties["hibernate.connection.connection_string"].ToString ();

            CreateTables ();
            
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

            SessionManager.Instance.GetSession ().Save (p);
            SessionManager.Instance.GetSession ().Save (p2);

            SessionManager.Instance.CommitTransaction ();

            Assert.AreNotEqual (p, p2, "The 2 persons are not the same");


            IList<Person> ps = SessionManager.Instance.GetSession ().
                            CreateCriteria (typeof (Person)).Add (Expression.Eq ("Name", "Frederik")).List<Person>();

            Assert.AreEqual (1, ps.Count, "There should be only one item in the collection.");

            Assert.AreEqual (p, ps[0], "The 2 object should be equal.");

            Assert.AreNotEqual (ps[0], p2, "the 2 objects should not be equal.");

            ps[0].Name = "melp";

            SessionManager.Instance.GetSession ().SaveOrUpdate (ps[0]);
            SessionManager.Instance.GetSession ().Flush ();

            SessionManager.Instance.CloseSession ();


        }
        
        [TestFixtureTearDown]
        public void DropTestDatabase()
        {
            SqlConnection conn = new SqlConnection (_connectionString);

            conn.Open ();
            try
            {
                DropTable (conn, "Persons");
            }
            finally
            {
                conn.Close ();
            }
        }

        #region Utility Methods to create Database tables

        private void CreateTables()
        {
            SqlConnection conn = new SqlConnection (_connectionString);

            conn.Open ();

            try
            {
                CreateTable (conn, this.GetCreatePersonsTableSqlString ());
            }
            finally
            {
                conn.Close ();
            }
        }

        private void CreateTable( SqlConnection conn, string createTableStatement )
        {
            SqlCommand cmd = new SqlCommand (createTableStatement, conn);

            cmd.ExecuteNonQuery ();
        }

        private string GetCreatePersonsTableSqlString()
        {
            return "CREATE TABLE Persons ( \n" +
                "[PersonId] INT IDENTITY (1,1), \n" +
                "[Name] VARCHAR(50), \n" +
                "[SEX] INT, \n" +
                "[DateOfBirth] DATETIME )";
        }

        private void DropTable( SqlConnection conn, string tableName )
        {
            SqlCommand cmd = new SqlCommand (String.Format ("DROP TABLE {0}", tableName), conn);

            cmd.ExecuteNonQuery ();
        }

        #endregion

    }
}
