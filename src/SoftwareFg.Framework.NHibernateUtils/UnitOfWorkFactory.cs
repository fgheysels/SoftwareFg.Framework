using NHibernate;

namespace SoftwareFg.Framework.NHibernateUtils
{
    public class UnitOfWorkFactory
    {
        #region Thread safe, lazy singleton.

        /// <summary>
        /// This is a thread-safe, lazy singleton.  See http://www.yoda.arachsys.com/csharp/singleton.html
        /// for more details about its implementation.
        /// </summary>
        public static UnitOfWorkFactory Instance
        {
            get
            {
                return Nested.UnitOfWorkFactory;
            }
        }

        private static class Nested
        {
            internal static readonly UnitOfWorkFactory UnitOfWorkFactory = new UnitOfWorkFactory ();
        }

        private UnitOfWorkFactory()
        {
            NHibernate.Cfg.Configuration cfg = new NHibernate.Cfg.Configuration ();
            cfg.Configure ();

            _sessionFactory = cfg.BuildSessionFactory ();
        }

        #endregion

        private ISessionFactory         _sessionFactory;

        public IInterceptor             DefaultInterceptor
        {
            get;
            set;
        }

        public UnitOfWork CreateUnitOfWork()
        {
            if( DefaultInterceptor == null )
            {
                return new UnitOfWork (_sessionFactory.OpenSession ());
            }
            else
            {
                return new UnitOfWork (_sessionFactory.OpenSession (DefaultInterceptor));

            }
        }

        public UnitOfWork CreateUnitOfWork( IInterceptor interceptor )
        {
            return new UnitOfWork (_sessionFactory.OpenSession (interceptor));
        }
    }
}
