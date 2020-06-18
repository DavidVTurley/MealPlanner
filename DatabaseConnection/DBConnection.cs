using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using DatabaseConnection.Databases;
using MySql.Data.MySqlClient;

namespace DatabaseConnection
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once InconsistentNaming
    public class DBConnection
    {
        private MySqlConnection _connection;
        private Int32 _connectionTimeout;

        private MySqlBaseConnectionStringBuilder _connectionString;
        //private String _server;
        //private String _database;
        //private String _userName;
        //private String _password;

        private MySqlCommand _cmd;
        private MySqlDataReader _reader;

        private readonly SemaphoreSlim _dbLock;

        public DBConnection()
        {
            _dbLock = new SemaphoreSlim(1, 1);
            Initialize();
        }

        private Boolean Initialize(String server = "", String database = "", String username = "", String password = "")
        {
            _connectionString = new MySqlConnectionStringBuilder
            {
                Server = "localhost", 
                Database = "meal_planner", 
                UserID = "root", 
                Password = ""
            };
//#if DEBUG


            //_connectionString.Server = "185.56.145.33/";
            //_connectionString.Database = "dav52005_meal_planner";
            //_connectionString.UserID = "dav52005";
            //_connectionString.Password = "Dag1962020";


//#endif
//#if !DEBUG
//            _server = server;
//            _database = database;
//            _userName = username;
//            _password = password;
//#endif

            _connection = new MySqlConnection(_connectionString.ConnectionString);
            _connectionTimeout = _connection.ConnectionTimeout;
            //var ping = _connection.Ping();
            //var s = _connection.GetSchema();
            return true;
        }

        public Boolean OpenConnection()
        {
            _dbLock.Wait();
            try
            {
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }

                _cmd = new MySqlCommand("", _connection);
                return true;
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        public Boolean CloseConnection()
        {

            try
            {
                _connection.Close();
                _cmd.Dispose();
                _dbLock.Release();
                return true;
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        private Boolean IsConnectionOpen()
        {
            return _connection.State == ConnectionState.Open;
        }

        // ExecuteNonQuery: Used to execute a command that will not return any data, for example Insert, update or delete.
        // ExecuteReader: Used to execute a command that will return 0 or more records, for example Select.
        // ExecuteScalar: Used to execute a command that will return only 1 value, for example Select Count(*).
        public Int32 ExecuteNonQuery(String sql)
        {
            if (!IsConnectionOpen()) return -2;
            try
            {
                _cmd.CommandText = sql;
                return _cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }
        
        // Straight up
        public void Select(String sql)
        {
            if (!IsConnectionOpen()) return;
            if (!sql.StartsWith("SELECT", true, CultureInfo.CurrentCulture)) return;

            _cmd.CommandText = sql;
            _reader = _cmd.ExecuteReader();
        }
        public Boolean Read()
        {
            return _reader.Read();
        }

        public Boolean HasRos()
        {
            return _reader.HasRows;
        }

        public Int32 Update(String sql)
        {
            if (!IsConnectionOpen()) return -2;
            try
            {
                if (!sql.StartsWith("UPDATE", true, CultureInfo.InvariantCulture)) return -1;

                _cmd.CommandText = sql;
                return _cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

                return -1;
            }

        }
        public Int32 Update(String tableName, String[] fieldNames, String[] values, String FieldID, Int32 FieldIdValueToUpdate)
        {
            if (!IsConnectionOpen()) return -2;
            try
            {
                String fieldValueCombination = "";
                for (Int32 i = 0; i < values.Length; i++)
                {
                    fieldValueCombination += " `" + fieldNames[i] + "` = '" + values[i] + "', ";
                }

                String cmdText = $"UPDATE `" + tableName + "SET " + fieldValueCombination + " WHERE `" + FieldID + " = '" + FieldIdValueToUpdate +"';";

                _cmd = new MySqlCommand(cmdText, _connection);

                return _cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return -1;
            }
        }

        public Int32 Insert(String tableName, String[] fieldNames, List<String[]> values)
        {
            if (!IsConnectionOpen()) return -2;
            try
            {
                String valuesToAdd = "";
                for (Int32 i = 0; i < values.Count; i++)
                {
                    String[] value = values[i];

                    String tempValueHolder = "(";
                    foreach (String s in value)
                    {
                        tempValueHolder += "'" + SanatizeStringForSQL(s) + "',";
                    }
                    // This is not removing the final comma for some reason...
                    tempValueHolder = tempValueHolder.Remove(tempValueHolder.Length-1, 1) + "),";
                    valuesToAdd += tempValueHolder;
                }

                valuesToAdd = valuesToAdd.Remove(valuesToAdd.Length - 1, 1);

                String cmdText = $"INSERT INTO `" + tableName + "` (`" + String.Join("`, `", fieldNames) + "`) VALUES " + valuesToAdd + ";";
                
                _cmd = new MySqlCommand(cmdText, _connection);

                return _cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return -1;
            }
        }
        public Int32 Insert(String sql)
        {
            if (!IsConnectionOpen()) return -2;
            try
            {

                _cmd = new MySqlCommand(sql, _connection);

                _cmd.ExecuteNonQuery();
                
                return (Int32)_cmd.LastInsertedId;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return -1;
            }
        }
        public Int32 Delete(String sql)
        {
            if (!IsConnectionOpen()) return -2;
            try
            {
                _cmd.CommandText = sql;
                return _cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return -1;
            }
        }

        // ReSharper disable once InconsistentNaming
        private static String SanatizeStringForSQL(String s)
        {
            s = s.Replace("'", "\\'");

            return s;
        }

        // Get Last Inserted Column
        public Int32 GetLastInsertedId()
        {
            Select("SELECT LAST_INSERT_ID();");
            _reader.Read();
            return _reader.GetInt32("LAST_INSERT_ID()");
        }
        // Get values from reader
        public Int32 GetInt32(String fieldName)
        {
            return _reader.GetInt32(fieldName);
        }
        public String GetString(String fieldName)
        {
            return _reader.GetString(fieldName);
        }

        public T GetEnum<T>(String fieldName)
        {
            return (T)_reader[fieldName];
        }
        public Boolean GetBoolean(String fieldName)
        {
            return _reader.GetBoolean(fieldName);
        }
        public DateTime GetDateTime(String fieldName)
        {
            try
            {
                return _reader.GetDateTime(fieldName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return DateTime.Now;
            }
        }
    }
}
