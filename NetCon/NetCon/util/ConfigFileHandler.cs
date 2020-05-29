using Microsoft.SqlServer.Server;
using NetCon.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NetCon.util
{
    class ConfigFileHandler<T>
    {
        const string CONFIG_FILE_NAME = "config.xml";
        public static void WriteSettings(T conf)
        {
            XmlSerializer serializer = new XmlSerializer(conf.GetType());
            TextWriter writer = new StreamWriter(CONFIG_FILE_NAME);
            serializer.Serialize(writer, conf);
            writer.Close();
        }

        public static T ReadSettings()
        {
            T conf;

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            try
            {
                FileStream reader = new FileStream(CONFIG_FILE_NAME, FileMode.Open, FileAccess.Read, FileShare.Read);
                conf = (T)serializer.Deserialize(reader);
                reader.Close();
                return conf;
            }
            catch(FileNotFoundException e) { return default(T); }
        }
    }
}
