using System;
using System.Xml;

namespace SoftwareFg.Framework.NHibernateUtils.Cfg
{
    public sealed class NHibernateConfigurationSettings
    {
        #region Members

        private XmlDocument _doc;

        private XmlNamespaceManager _nsmgr;

        private const string NHibernateConfigurationNamespace = "urn:nhibernate-configuration-2.2";

        #endregion

        internal XmlDocument XmlDoc
        {
            get
            {
                return _doc;
            }
        }

        /// <summary>
        /// Creates a NHibernateConfigurationSettings class.
        /// This constructor is internal, since only the NHibernateConfigurationManager can instantiate this class.
        /// </summary>
        /// <param name="doc"></param>
        internal NHibernateConfigurationSettings( XmlDocument doc )
        {
            _doc = doc;

            _nsmgr = new XmlNamespaceManager (doc.NameTable);
            _nsmgr.AddNamespace ("nhib", NHibernateConfigurationNamespace);
        }

        /// <summary>
        /// Gets the value of an NHibernate 'property' setting.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetNHibernateProperty( string key )
        {
            // Get the node in 2 steps because of issues with the namespace in the xml document.
            XmlNode nhibernateConfigSettings = this.GetNHibernateConfigurationElement ();

            XmlNode node = nhibernateConfigSettings.SelectSingleNode (@"//nhib:property[@name='" + key + @"']/*", _nsmgr);

            if( node == null )
            {
                throw new ApplicationException ("There exists no NHibernate setting: " + key);
            }

            return node.InnerText;
        }

        /// <summary>
        /// Sets the value of an NHibernate property
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetNHibernateProperty( string key, string value )
        {
            // Try to find the node that has the given key.  If there exists one, then
            // update it's value.
            // If there doesn't exists a node with this key, then add one.
            XmlNode nhibernateConfigSettings = this.GetNHibernateConfigurationElement ();

            XmlNode node = nhibernateConfigSettings.SelectSingleNode (@"//nhib:property[@name='" + key + @"']", _nsmgr);

            if( node != null )
            {
                node.InnerText = value;
            }
            else
            {
                XmlElement setting = _doc.CreateElement ("property", NHibernateConfigurationNamespace);
                setting.SetAttribute ("name", key);
                setting.InnerText = value;

                XmlNode sessionFactoryNode = this.GetNHibernateSessionFactoryElement (nhibernateConfigSettings);
                sessionFactoryNode.AppendChild (setting);
            }
        }

        /// <summary>
        /// Gets the Mapping Assembly if one is specified in the settings object.  If none is specified, an
        /// empty string is returned.
        /// </summary>
        /// <returns></returns>
        public string GetNHibernateMappingAssembly()
        {
            XmlNode node = this.GetNHibernateMappingAssemblyNode ();

            if( node == null )
            {
                return string.Empty;
            }
            else
            {
                return node.InnerText;
            }
        }

        /// <summary>
        /// Sets the assembly which contains the classes that have to be mapped.
        /// </summary>
        /// <param name="assemblyName"></param>
        public void SetNHibernateMappingAssembly( string assemblyName )
        {
            XmlNode node = this.GetNHibernateMappingAssemblyNode ();

            ( (XmlElement)node ).SetAttribute ("assembly", assemblyName);
        }

        private XmlElement GetNHibernateConfigurationElement()
        {
            XmlNode nhibernateConfigSettings = _doc.SelectSingleNode ("//nhib:hibernate-configuration", _nsmgr);

            // If we haven't found the nhibernate configsettings element, we should create it.
            if( nhibernateConfigSettings == null )
            {
                nhibernateConfigSettings = _doc.CreateElement ("hibernate-configuration", NHibernateConfigurationNamespace);

                _doc.AppendChild (nhibernateConfigSettings);
            }

            return (XmlElement)nhibernateConfigSettings;
        }

        private XmlElement GetNHibernateSessionFactoryElement( XmlNode nhibernateConfigNode )
        {
            XmlNode sessionFactoryNode = nhibernateConfigNode.SelectSingleNode ("//nhib:session-factory", _nsmgr);

            if( sessionFactoryNode == null )
            {
                sessionFactoryNode = _doc.CreateElement ("session-factory", NHibernateConfigurationNamespace);

                nhibernateConfigNode.AppendChild (sessionFactoryNode);
            }

            return (XmlElement)sessionFactoryNode;
        }


        private XmlNode GetNHibernateMappingAssemblyNode()
        {
            XmlNode sessionFactory = this.GetNHibernateSessionFactoryElement (this.GetNHibernateConfigurationElement ());

            XmlNode node = sessionFactory.SelectSingleNode (@"//nhib:mapping", _nsmgr);

            if( node == null )
            {
                node = _doc.CreateElement ("mapping", NHibernateConfigurationNamespace);

                sessionFactory.AppendChild (node);
            }

            return node;
        }
    }
}
