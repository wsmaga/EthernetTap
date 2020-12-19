using NetCon.enums;
using System;
using System.Data;
using System.Data.SqlClient;

namespace NetCon.export.services
{
    class DatabaseService
    {

        private static DatabaseService instance;
        public static DatabaseService GetInstance()
        {
            if (instance == null)
                instance = new DatabaseService();
            return instance;
        }

        private DatabaseService() { }

        public DBConnectionStatus CheckConnection(string serverAddress, string databaseName)
        {
            if (!CheckServer(serverAddress))
                return DBConnectionStatus.BadServer;

            if (!CheckDatabase(serverAddress, databaseName))
                return DBConnectionStatus.BadDB;

            return DBConnectionStatus.OK;
        }

        public bool InitializeDatabase(string serverAddress, string databaseName)
        {
            bool result = true;
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder()
            {
                DataSource = serverAddress,
                InitialCatalog = databaseName,
                IntegratedSecurity = true
            };
            using (DBContext dBContext = new DBContext(connectionStringBuilder.ConnectionString))
            {
                try
                {
                    dBContext.Database.Initialize(true);
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    result = false;
                }
            }

            return result;
        }

        private bool CheckServer(string serverAddress)
        {
            bool result = true;
            SqlConnection connection = new SqlConnection(String.Format(
                "Data source={0};Integrated Security=true;Connection Timeout=5",
                serverAddress
            ));
            try
            {
                connection.Open();
            }
            catch (SqlException e)
            {
                //Console.WriteLine(e.Message);
                //Console.WriteLine(e.StackTrace);
                result = false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
        }

        private bool CheckDatabase(string serverAddress, string databaseName)
        {
            bool result = true;
            SqlConnection connection = new SqlConnection(String.Format(
                "Data source={0};Database={1};Integrated Security=true;Connection Timeout=5",
                serverAddress,
                databaseName
            ));
            try
            {
                connection.Open();
            }
            catch (SqlException e)
            {
                //Console.WriteLine(e.Message);
                //Console.WriteLine(e.StackTrace);
                result = false;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
        }

    }
}
