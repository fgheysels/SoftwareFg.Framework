using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using NHibernate.Cfg;
using System.Data;

namespace SoftwareFg.Framework.NHibernateUtils.Tests.Utils
{
    public static class Database
    {
        private static SqlConnection _conn;

        static Database()
        {
            // Create an NHibernate Configuration object just to get the connection-string that we're using.
            Configuration cfg = new Configuration ();
            cfg.Configure ();

            string connectionString = cfg.Properties["hibernate.connection.connection_string"].ToString ();

            _conn = new SqlConnection (connectionString);
        }

        private static void OpenConnection()
        {
            if( _conn.State != ConnectionState.Open )
            {
                _conn.Open ();
            }
        }

        public static void CreateTable( string createTableStatement )
        {
            OpenConnection ();

            SqlCommand cmd = new SqlCommand (createTableStatement, _conn);
            cmd.ExecuteNonQuery ();

        }

        public static void DropTable( string tableName )
        {
            OpenConnection ();

            SqlCommand cmd = new SqlCommand (String.Format ("DROP TABLE {0}", tableName, _conn));
            cmd.ExecuteNonQuery ();
        }

        public static void Close()
        {
            _conn.Close ();
        }
    }
}
