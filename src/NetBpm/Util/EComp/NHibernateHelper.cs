using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBpm.Util.EComp
{
    public static class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        internal static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    var configuration = ConfigurationFactory.CreateSQLServer2005("NetBPM", new string[] { "NetBpm"} );

                    _sessionFactory = configuration.BuildSessionFactory();
                }
                return _sessionFactory;
            }
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
