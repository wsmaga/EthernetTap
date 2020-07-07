using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.export
{
    public static class IDataExporterSettings
    {
        readonly public static int DATA_TO_BUFFER_NUM = 10;
    }

    /// <summary>
    /// Interface of a data exporter that can send data to an external storage like a database.
    /// </summary>
    interface IDataExporter
    {
        /// <summary>
        /// True if the storage system is reachable.
        /// </summary>
        bool CanConnect { get; }
        
        /// <summary>
        /// Stores a single byte in the storage system.
        /// </summary>
        /// <param name="data">Data to be stored.</param>
        bool StoreByte(byte data);
    }
}
