using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.export
{
    /// <summary>
    /// Data exporter exporting to a local MSSQL database.
    /// </summary>
    class LocalDBExporter : IDataExporter
    {
        private EntityContainer Context { get; set; }
        
        public bool CanConnect
        {
            get
            {
                throw new NotImplementedException();
                return false;
            }
        }

        public void SetMdfFile(String path)
        {
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder(
                ConfigurationManager.ConnectionStrings["EntityContainer"].ConnectionString
                );

            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder(
                entityBuilder.ProviderConnectionString
                );

            sqlBuilder.AttachDBFilename = path;
            entityBuilder.ProviderConnectionString = sqlBuilder.ToString();

            Context = new EntityContainer(entityBuilder.ToString());
        }

        public void StoreByte(byte data)
        {
            Context.ByteMeasurementSet.Add(new ByteMeasurement { MeasurementID = 420, Timestamp = DateTime.Now, Value = data });
            Context.SaveChanges();
        }
    }
}
