using NHibernate;
using SoftwareFg.Framework;

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
            DefaultFlushMode = FlushMode.Auto;

            NHibernate.Cfg.Configuration cfg = new NHibernate.Cfg.Configuration ();
            cfg.Configure ();
            //NHibernate.Mapping.PersistentClass pc = cfg.GetClassMapping ("");
            //pc.Table.Name;
            //foreach( NHibernate.Mapping.Column col in pc.GetProperty("").ColumnIterator )
            //    col.Name
            _sessionFactory = cfg.BuildSessionFactory ();
        }

        #endregion

        private ISessionFactory         _sessionFactory;

        public IInterceptor DefaultInterceptor
        {
            get;
            set;
        }

        public FlushMode DefaultFlushMode
        {
            get;
            set;
        }

        public UnitOfWork CreateUnitOfWork()
        {
            return CreateUnitOfWork (DefaultInterceptor, DefaultFlushMode);
        }

        public UnitOfWork CreateUnitOfWork( IInterceptor interceptor )
        {
            return CreateUnitOfWork (interceptor, DefaultFlushMode);
        }

        public UnitOfWork CreateUnitOfWork( FlushMode flushMode )
        {
            return CreateUnitOfWork (DefaultInterceptor, flushMode);
        }

        public UnitOfWork CreateUnitOfWork( IInterceptor interceptor, FlushMode flushMode )
        {
            UnitOfWork uow;

            if( interceptor == null )
            {
                uow = new UnitOfWork (_sessionFactory.OpenSession ());
            }
            else
            {
                uow = new UnitOfWork (_sessionFactory.OpenSession (interceptor));
            }
            
            Check.Ensure (uow != null, "The UnitOfWork cannot be NULL");

            uow.Session.FlushMode = flushMode;

            return uow;

        }
        
    }
}
