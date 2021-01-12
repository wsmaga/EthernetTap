using NetCon.model;
using System.IO;
using System.Xml.Serialization;

namespace NetCon.hardwareFilters
{
    class HardwareFiltersConfigFileHandler
    {
        private string filePath;
        public HardwareFiltersConfigFileHandler(string filePath)
        {
            this.filePath = filePath;
        }

        public void WriteSettings(HardwareFiltersDefinition conf)
        {
            XmlSerializer serializer = new XmlSerializer(conf.GetType());
            TextWriter writer = new StreamWriter(filePath);
            serializer.Serialize(writer, conf);
            writer.Close();
        }

        public HardwareFiltersDefinition ReadSettings()
        {
            HardwareFiltersDefinition conf;

            XmlSerializer serializer = new XmlSerializer(typeof(HardwareFiltersDefinition));
            try
            {
                FileStream reader = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                conf = (HardwareFiltersDefinition)serializer.Deserialize(reader);
                reader.Close();
                return conf;
            }
            catch (FileNotFoundException) { return new HardwareFiltersDefinition(); }
        }

    }
}
