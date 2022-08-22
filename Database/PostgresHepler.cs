
using IDMSWebServer.Models;
using Npgsql;
using System.Data;
using System.Diagnostics;

namespace IDMSWebServer.Database
{
    public class PostgresHepler
    {

        public string Host = "127.0.0.1";
        public string Port = "5432";
        public string Database = "idms_pc";
        public string UserName = "postgres";
        public string Password = "changeme";

        public PostgresHepler(IConfiguration config)
        {
            var _Host = config["Postgres:Host"];
            var _Port = config["Postgres:Port"];
            var _Database = config["Postgres:Database"];
            var _UserName = config["Postgres:UserName"];
            var _Password = config["Postgres:Password"];

            Host = _Host == null ? Host : _Host;
            Port = _Port == null ? Port : _Port;
            Database = _Database == null ? Database : _Database;
            UserName = _UserName == null ? UserName : _UserName;
            Password = _Password == null ? Password : _Password;
        }
        #region API

        internal IEnumerable<string> GetDatabases()
        {
            List<string> databaseLs = new List<string>();
            if (TryConnect(Host, Port, UserName, Password, "postgres", out NpgsqlConnection conn))
            {
                using (NpgsqlCommand command = new NpgsqlCommand("SELECT datname FROM pg_database WHERE datistemplate = false ORDER BY datname", conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var db = reader[0];
                            databaseLs.Add(db.ToString());
                        }
                    }
                }
                conn.Close();
            }
            return databaseLs;

        }

        internal bool TryGetSchemasFromDB(string DBName, out List<string> schemaList, out string message)
        {
            message = "";
            schemaList = new List<string>();
            if (TryConnect(Host, Port, UserName, Password, DBName, out NpgsqlConnection conn))
            {
                var schemas = conn.GetSchema("Tables");
                schemaList = schemas.Rows.Cast<DataRow>().Where(row => row["table_schema"].ToString().Contains("sensor")).Select(row => row["table_schema"].ToString()).ToList().Distinct().ToList();
                conn.Close();
                return true;
            }
            else
            {
                message = "DB Connect Fail";
                return false;
            }
        }

        #endregion

        #region Private Methods

        private NpgsqlConnection Connect()
        {
            return TryConnect(Host, Port, UserName, Password, Database, out NpgsqlConnection conn) ? conn : null;
        }
        private NpgsqlConnection Connect(string host, string port, string userName, string password, string tableName)
        {
            try
            {
                var conn = new NpgsqlConnection($"Server={host};Port={port};Database={tableName};User Id={userName};Password={password};");
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        protected bool TryConnect(string host, string port, string userName, string password, string tableName, out NpgsqlConnection conn)
        {
            conn = Connect(host, port, userName, password, tableName);
            if(conn == null)
                 return false;     
            return conn.State == ConnectionState.Open;
        }


        virtual protected string SqlCommandStringBuilder()
        {
            return $"";
        }
        protected bool TryGetTableFromDB(string sqlQueryString, out int dataNum, out DataTable _datatable, out string message)
        {
            message = "";
            dataNum = 0;
            _datatable = new DataTable();

            bool opened = TryConnect(Host, Port, UserName, Password, Database, out NpgsqlConnection conn);
            if (opened)
            {
                try
                {
                    using (var Command_Execute = new NpgsqlCommand(sqlQueryString, conn))
                    {
                        NpgsqlDataAdapter DataAdapte = new NpgsqlDataAdapter(Command_Execute);
                        DataAdapte.Fill(_datatable);
                    }
                    dataNum = _datatable.Rows.Count;
                    conn.CloseAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    message = $"嘗試向資料庫讀取資料時發生錯誤:${ex.Message}";
                    return false;
                }
            }
            else
            {
                message = "資料庫無法開啟";
                return false;
            }

        }

        protected bool TryGetTableFromDB(string sqlQueryString, out int dataNum, ref Dictionary<string, List<object>> _datatable, out string message)
        {
            message = "";
            dataNum = 0;
            NpgsqlConnection? conn = null;
            try
            {
                conn = new NpgsqlConnection($"Server={Host};Port={Port};Database={Database};User Id={UserName};Password={Password};");
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    using (var Command_Execute = new NpgsqlCommand(sqlQueryString, conn))
                    {
                        NpgsqlDataReader dr = Command_Execute.ExecuteReader();

                        while (dr.Read())
                        {
                            var info = dr.GetColumnSchema();
                            for (int i = 0; i < dr.FieldCount; i++)
                            {
                                string columnName = info[i].ColumnName;
                                if (!_datatable.ContainsKey(columnName))
                                    _datatable.Add(columnName, new List<object>());
                                _datatable[columnName].Add(dr[columnName]);
                            }
                        }
                        bool re = dr.Read();
                        //NpgsqlDataAdapter DataAdapte = new NpgsqlDataAdapter(Command_Execute);
                        //DataAdapte.Fill(_datatable);
                    }
                    dataNum = _datatable[_datatable.Keys.First()].Count;
                    conn.CloseAsync();
                    return true;
                }
                else
                {
                    message = "資料庫無法開啟";
                    return false;
                }
            }
            catch (Exception ex)
            {
                conn?.Close();
                message = $"嘗試與資料庫進行連線時發生例外:${ex.Message}";
                return false;
            }
        }
        #endregion
    }
}
