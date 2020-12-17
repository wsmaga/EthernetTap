using NetCon.enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

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
            return CreateDatabase(serverAddress, databaseName) &&
                PopulateDatabaseWithTables(serverAddress, databaseName);
        }

        private bool PopulateDatabaseWithTables(string serverAddress, string databaseName)
        {
            bool result = true;

            SqlConnection databaseConnection = new SqlConnection(
                string.Format(
                    "Data source={0};Integrated Security=true;Database={1};Timeout=5;",
                    serverAddress,
                    databaseName
                )
            );

            string DDLScript = File.ReadAllText("DBInitDDL.sql");
            SqlCommand DDLCommand = new SqlCommand(DDLScript, databaseConnection);
            DDLCommand.CommandTimeout = 5;

            try
            {
                databaseConnection.Open();
                DDLCommand.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                result = false;
            }
            finally
            {
                if (databaseConnection.State == ConnectionState.Open)
                {
                    databaseConnection.Close();
                }
            }

            return result;
        }

        private bool CreateDatabase(string serverAddress, string databaseName)
        {
            bool result = true;

            SqlConnection serverConnection = new SqlConnection(
                string.Format(
                    "Data source={0};Integrated Security=true;Timeout=5;",
                    serverAddress
                )
            );

            string query = String.Format("CREATE DATABASE {0};", databaseName);
            SqlCommand createDatabaseCommand = new SqlCommand(query, serverConnection);
            createDatabaseCommand.CommandTimeout = 5;

            try
            {
                serverConnection.Open();
                createDatabaseCommand.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                result = false;
            }
            finally
            {
                if (serverConnection.State == ConnectionState.Open)
                {
                    serverConnection.Close();
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
