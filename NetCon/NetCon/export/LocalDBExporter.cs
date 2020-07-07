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
                // Is DBContext initialized?
                if (Context == null)
                    return false;

                // Is it possible to add byte data to the database?
                Context.ByteMeasurementSet.Add(
                    new ByteMeasurement
                    {
                        MeasurementID = -1,
                        Value = 0,
                        Timestamp = DateTime.Now
                    }
                    );
                if (!(Context.SaveChanges() == 1))
                    return false;

                // Is it possible to find and remove said data?
                try
                {
                    ByteMeasurement temp = Context.ByteMeasurementSet.Where(element => element.MeasurementID == -1).First<ByteMeasurement>();

                    Context.ByteMeasurementSet.Remove(temp);
                    if (!(Context.SaveChanges() == 1))
                        return false;
                }
                catch(ArgumentNullException e)
                {
                    return false;
                }

                // ...if so - return true.
                return true;
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

        private List<ByteMeasurement> ByteDataToStore { get; } = new List<ByteMeasurement>();
        private int CurrentMeasurementID { get; set; } = -1;
        private bool StorageErrorOccured { get; set; } = false;

        public bool StoreByte(byte data)
        {
            if (Context == null)
                throw new NullReferenceException("You should not try to store byte when DBContext is not initialized!");

            if (CurrentMeasurementID < 0)
                throw new NullReferenceException("You should not try to store byte before invoking StartNewStoring!");

            ByteDataToStore.Add(
                new ByteMeasurement
                {
                    MeasurementID = CurrentMeasurementID,
                    Timestamp = DateTime.Now,
                    Value = data
                }
                );

            if(ByteDataToStore.Count >= IDataExporterSettings.DATA_TO_BUFFER_NUM)
            {
                Context.ByteMeasurementSet.AddRange(ByteDataToStore);
                Task.Run(() =>
                {
                    if (Context.SaveChanges() < IDataExporterSettings.DATA_TO_BUFFER_NUM)
                        StorageErrorOccured = true;
                });
                ByteDataToStore.Clear();
            }

            if (StorageErrorOccured)
            {
                StorageErrorOccured = false;
                return false;
            }
            else
                return true;
        }

        public void StartNewStoring()
        {
            int maxMeasurementID = 0;

            try
            {
                ByteMeasurement temp = Context.ByteMeasurementSet
                    .OrderByDescending(element => element.MeasurementID)
                    .First<ByteMeasurement>();
                if (temp.MeasurementID > maxMeasurementID)
                    maxMeasurementID = temp.MeasurementID;

            }
            catch (ArgumentNullException e) { }

            CurrentMeasurementID = maxMeasurementID + 1;
        }

        public bool FinishStoring()
        {
            if (ByteDataToStore.Count > 0)
            {
                Context.ByteMeasurementSet.AddRange(ByteDataToStore);
                StorageErrorOccured = (Context.SaveChanges() != ByteDataToStore.Count);
                ByteDataToStore.Clear();

                if (StorageErrorOccured)
                {
                    StorageErrorOccured = false;
                    return false;
                }
                else
                    return true;
            }
            else
                return true;
        }
    }
}
