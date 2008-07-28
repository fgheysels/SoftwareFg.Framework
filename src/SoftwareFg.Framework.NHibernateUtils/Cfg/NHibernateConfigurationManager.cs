using System.IO;
using System.Xml;

namespace SoftwareFg.Framework.NHibernateUtils.Cfg
{
    public static class NHibernateConfigurationManager
    {

        public static NHibernateConfigurationSettings GetNHibernateConfigSettings( string fileName )
        {
            if( File.Exists (fileName) == false )
            {
                throw new IOException ("The file " + fileName + " does not exists.");
            }

            XmlDocument doc = new XmlDocument ();
            doc.Load (fileName);

            return new NHibernateConfigurationSettings (doc);
        }

        public static void SaveNHibernateConfigSettings( NHibernateConfigurationSettings settings, string fileName )
        {
            settings.XmlDoc.Save (fileName);
        }

    }
}
