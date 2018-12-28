using System;
using System.Data.SQLite;
using System.IO;

namespace SimpleSettingsManager.Mode
{
    internal class SQLite : IMode
    {
        private const string _ssmFormatVer = "1.0";
        private const string _minSsmFormatVer = "1.0";

        private SQLiteConnection _dbConnection;
        private string _settingsPath;

        #region Init

        public void Open(string path)
        {
            _settingsPath = path;

            bool isNewDatabase = createSQLiFile(_settingsPath);

            _dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", _settingsPath));
            _dbConnection.Open();

            if (isNewDatabase)
            {
                CreateMetaTable();

                AddMetaData("SSM_CreationAppVersion", "HistoricalInfo", Utilities.GetVersion(), "The version of SSM used to create the database.");
                AddMetaData("SSM_LastAccessAppVersion", "LastAccessInfo", Utilities.GetVersion(), "The version of SSM used to last edit the database.");
                AddMetaData("SSM_CreationFormatVersion", "HistoricalInfo", _ssmFormatVer, "The original SSM Format version of the database.");
                AddMetaData("SSM_LastAccessFormatVersion", "LastAccessInfo", _ssmFormatVer, "The current SSM Format version of the database.");
                AddMetaData("SSM_CreationTimestamp", "HistoricalInfo", Convert.ToString(IntUtilities.GetUnixTimestamp()), "The timestamp of when the database was created.");
                AddMetaData("SSM_LastLoadedTimestamp", "LastAccessInfo", Convert.ToString(IntUtilities.GetUnixTimestamp()), "The timestamp of when the database was last loaded.");
            }
            else
            {
                SetMetaData("SSM_LastLoadedTimestamp", Convert.ToString(IntUtilities.GetUnixTimestamp()));
            }
        }

        public void Close()
        {
            _dbConnection.Close();
        }

        private bool createSQLiFile(string path)
        {
            if (!File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);
                return true;
            }
            return false;
        }

        #endregion

        #region Add Variables

        public bool AddInt16(string uniqueName, Int16 value, string description, string group = "default")
        {
            CreateInt16Table();
            return AddVarToTable(uniqueName, value, description, group, "Int16");
        }

        public bool AddInt32(string uniqueName, Int32 value, string description, string group = "default")
        {
            CreateInt32Table();
            return AddVarToTable(uniqueName, value, description, group, "Int32");
        }

        public bool AddInt(string uniqueName, int value, string description, string group = "default")
        {
            return AddInt32(uniqueName, value, description, group);
        }

        public bool AddInt64(string uniqueName, Int64 value, string description, string group = "default")
        {
            CreateInt64Table();
            return AddVarToTable(uniqueName, value, description, group, "Int64");
        }

        public bool AddLong(string uniqueName, long value, string description, string group = "default")
        {
            return AddInt64(uniqueName, value, description, group);
        }

        public bool AddFloat(string uniqueName, float value, string description, string group = "default")
        {
            CreateFloatTable();
            return AddVarToTable(uniqueName, value, description, group, "Float");
        }

        public bool AddDouble(string uniqueName, double value, string description, string group = "default")
        {
            CreateDoubleTable();
            return AddVarToTable(uniqueName, value, description, group, "Double");
        }

        public bool AddString(string uniqueName, string value, string description, string group = "default")
        {
            CreateStringTable();
            return AddVarToTable(uniqueName, value, description, group, "String");
        }

        public bool AddByteArray(string uniqueName, byte[] value, string description, string group = "default")
        {
            CreateByteArrayTable();
            return AddVarToTable(uniqueName, value, description, group, "ByteArray");
        }

        public bool AddBoolean(string uniqueName, bool value, string description, string group = "default")
        {
            CreateBooleanTable();
            return AddVarToTable(uniqueName, value, description, group, "Boolean");
        }

        #endregion
        #region Set Variables

        public bool SetInt16(string uniqueName, Int16 value)
        {
            return SetVarInTable(uniqueName, value, "Int16");
        }

        public bool SetInt32(string uniqueName, Int32 value)
        {
            return SetVarInTable(uniqueName, value, "Int32");
        }

        public bool SetInt(string uniqueName, int value)
        {
            return SetInt32(uniqueName, value);
        }

        public bool SetInt64(string uniqueName, Int64 value)
        {
            return SetVarInTable(uniqueName, value, "Int64");
        }

        public bool SetLong(string uniqueName, long value)
        {
            return SetInt64(uniqueName, value);
        }

        public bool SetFloat(string uniqueName, float value)
        {
            return SetVarInTable(uniqueName, value, "Float");
        }

        public bool SetDouble(string uniqueName, double value)
        {
            return SetVarInTable(uniqueName, value, "Double");
        }

        public bool SetString(string uniqueName, string value)
        {
            return SetVarInTable(uniqueName, value, "String");
        }

        public bool SetByteArray(string uniqueName, byte[] value)
        {
            return SetVarInTable(uniqueName, value, "ByteArray");
        }

        public bool SetBoolean(string uniqueName, bool value)
        {
            return SetVarInTable(uniqueName, value, "Boolean");
        }

        #endregion
        #region Edit Variables

        public bool EditInt16(string uniqueName, string description, string group)
        {
            return EditVarInTable(uniqueName, description, group, "Int16");
        }

        public bool EditInt32(string uniqueName, string description, string group)
        {
            return EditVarInTable(uniqueName, description, group, "Int32");
        }

        public bool EditInt(string uniqueName, string description, string group)
        {
            return EditInt32(uniqueName, description, group);
        }

        public bool EditInt64(string uniqueName, string description, string group)
        {
            return EditVarInTable(uniqueName, description, group, "Int64");
        }

        public bool EditLong(string uniqueName, string description, string group)
        {
            return EditInt64(uniqueName, description, group);
        }

        public bool EditFloat(string uniqueName, string description, string group)
        {
            return EditVarInTable(uniqueName, description, group, "Float");
        }

        public bool EditDouble(string uniqueName, string description, string group)
        {
            return EditVarInTable(uniqueName, description, group, "Double");
        }

        public bool EditString(string uniqueName, string description, string group)
        {
            return EditVarInTable(uniqueName, description, group, "String");
        }

        public bool EditByteArray(string uniqueName, string description, string group)
        {
            return EditVarInTable(uniqueName, description, group, "ByteArray");
        }

        public bool EditBoolean(string uniqueName, string description, string group)
        {
            return EditVarInTable(uniqueName, description, group, "Boolean");
        }

        #endregion
        #region Get Variables

        public Int16 GetInt16(string uniqueName)
        {
            return Convert.ToInt16(GetVarInTable(uniqueName, "Int16"));
        }

        public Int32 GetInt32(string uniqueName)
        {
            return Convert.ToInt32(GetVarInTable(uniqueName, "Int32"));
        }

        public int GetInt(string uniqueName)
        {
            return GetInt32(uniqueName);
        }

        public Int64 GetInt64(string uniqueName)
        {
            return Convert.ToInt64(GetVarInTable(uniqueName, "Int64"));
        }

        public long GetLong(string uniqueName)
        {
            return GetInt64(uniqueName);
        }

        public float GetFloat(string uniqueName)
        {
            return Convert.ToSingle(GetVarInTable(uniqueName, "Float"));
        }

        public double GetDouble(string uniqueName)
        {
            return Convert.ToDouble(GetVarInTable(uniqueName, "Double"));
        }

        public string GetString(string uniqueName)
        {
            return Convert.ToString(GetVarInTable(uniqueName, "String"));
        }

        public byte[] GetByteArray(string uniqueName)
        {
            return IntUtilities.ObjectToByteArray(GetVarInTable(uniqueName, "ByteArray"));
        }

        public bool GetBoolean(string uniqueName)
        {
            return Convert.ToBoolean(GetVarInTable(uniqueName, "Boolean"));
        }

        #endregion
        #region Delete Variables

        public bool DeleteInt16(string uniqueName)
        {
            return DeleteVarInTable(uniqueName, "Int16");
        }

        public bool DeleteInt32(string uniqueName)
        {
            return DeleteVarInTable(uniqueName, "Int32");
        }

        public bool DeleteInt(string uniqueName)
        {
            return DeleteInt32(uniqueName);
        }

        public bool DeleteInt64(string uniqueName)
        {
            return DeleteVarInTable(uniqueName, "Int64");
        }

        public bool DeleteLong(string uniqueName)
        {
            return DeleteInt64(uniqueName);
        }

        public bool DeleteFloat(string uniqueName)
        {
            return DeleteVarInTable(uniqueName, "Float");
        }

        public bool DeleteDouble(string uniqueName)
        {
            return DeleteVarInTable(uniqueName, "Double");
        }

        public bool DeleteString(string uniqueName)
        {
            return DeleteVarInTable(uniqueName, "String");
        }

        public bool DeleteByteArray(string uniqueName)
        {
            return DeleteVarInTable(uniqueName, "ByteArray");
        }

        public bool DeleteBoolean(string uniqueName)
        {
            return DeleteVarInTable(uniqueName, "Boolean");
        }

        #endregion
        #region MetaData

        private bool AddMetaData(string varName, string group, string varValue, string description)
        {
            if (!DoesMetaDataVariableExist(varName))
            {
                SQLiteCommand command = new SQLiteCommand("INSERT INTO _MetaData (VariableName, VariableGroup, VariableValue, VariableDesc) VALUES (@varName, @varGroup, @varValue, @varDesc)", _dbConnection);

                command.Parameters.AddWithValue("@varName", varName);
                command.Parameters.AddWithValue("@varGroup", group);
                command.Parameters.AddWithValue("@varValue", varValue);
                command.Parameters.AddWithValue("@varDesc", description);
                command.ExecuteNonQuery();

                return true;
            }
            return false;
        }

        private bool SetMetaData(string varName, string varValue)
        {
            if (DoesMetaDataVariableExist(varName))
            {
                SQLiteCommand command = new SQLiteCommand("UPDATE _MetaData SET VariableValue = @varValue WHERE VariableName = @varName", _dbConnection);

                command.Parameters.AddWithValue("@varName", varName);
                command.Parameters.AddWithValue("@varValue", varValue);
                command.ExecuteNonQuery();

                return true;
            }
            return false;
        }

        private bool EditMetaData(string varName, string description, string group)
        {
            if (DoesMetaDataVariableExist(varName))
            {
                SQLiteCommand command = new SQLiteCommand("UPDATE _MetaData SET VariableGroup = @varGroup, VariableDesc = @varDesc WHERE VariableName = @varName", _dbConnection);

                command.Parameters.AddWithValue("@varName", varName);
                command.Parameters.AddWithValue("@varGroup", group);
                command.Parameters.AddWithValue("@varDesc", description);
                command.ExecuteNonQuery();

                return true;
            }
            return false;
        }

        private bool DeleteMetaData(string varName)
        {
            if (DoesMetaDataVariableExist(varName))
            {
                SQLiteCommand command = new SQLiteCommand("DELETE FROM _MetaData WHERE VariableName = @varName", _dbConnection);

                command.Parameters.AddWithValue("@varName", varName);
                command.ExecuteNonQuery();

                return true;
            }
            return false;
        }

        #endregion

        #region Raw Database Handling

        private bool AddVarToTable(string uniqueName, object value, string description, string group, string table)
        {
            if (!DoesVariableExist(uniqueName))
            {
                SQLiteCommand command = new SQLiteCommand("INSERT INTO " + IntUtilities.SqlEscape(table) + " (VariableName, VariableGroup, VariableValue, VariableDefault, VariableDesc) VALUES (@varName, @varGroup, @varValue, @varValue, @varDesc)", _dbConnection);

                command.Parameters.AddWithValue("@varName", uniqueName);
                command.Parameters.AddWithValue("@varGroup", group);
                command.Parameters.AddWithValue("@varValue", value);
                command.Parameters.AddWithValue("@varDesc", description);
                command.ExecuteNonQuery();

                return true;
            }
            return false;
        }

        private bool SetVarInTable(string uniqueName, object value, string table)
        {
            if (DoesVariableExist(uniqueName))
            {
                SQLiteCommand command = new SQLiteCommand("UPDATE " + IntUtilities.SqlEscape(table) + " SET VariableValue = @varValue WHERE VariableName = @varName", _dbConnection);

                command.Parameters.AddWithValue("@varName", uniqueName);
                command.Parameters.AddWithValue("@varValue", value);
                command.ExecuteNonQuery();

                return true;
            }
            return false;
        }

        private bool EditVarInTable(string uniqueName, string description, string group, string table)
        {
            if (DoesVariableExist(uniqueName))
            {
                SQLiteCommand command = new SQLiteCommand("UPDATE " + IntUtilities.SqlEscape(table) + " VariableGroup = @varGroup, VariableDesc = @varDesc WHERE VariableName = @varName", _dbConnection);

                command.Parameters.AddWithValue("@varName", uniqueName);
                command.Parameters.AddWithValue("@varGroup", group);
                command.Parameters.AddWithValue("@varDesc", description);
                command.ExecuteNonQuery();

                return true;
            }
            return false;
        }

        private object GetVarInTable(string uniqueName, string table)
        {
            if (DoesVariableExist(uniqueName))
            {
                SQLiteCommand command = new SQLiteCommand("SELECT VariableValue FROM " + IntUtilities.SqlEscape(table) + " WHERE VariableName = @varName", _dbConnection);

                command.Parameters.AddWithValue("@varName", uniqueName);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return reader[0];
                    }
                }
            }
            return null;
        }

        private bool DeleteVarInTable(string uniqueName, string table)
        {
            if (DoesVariableExist(uniqueName))
            {
                SQLiteCommand command = new SQLiteCommand("DELETE FROM " + IntUtilities.SqlEscape(table) + " WHERE VariableName = @varName", _dbConnection);

                command.Parameters.AddWithValue("@varName", uniqueName);
                command.ExecuteNonQuery();

                return true;
            }
            return false;
        }

        private bool DoesTableExist(string tableName)
        {
            SQLiteCommand command = new SQLiteCommand("SELECT COUNT(1) FROM sqlite_master WHERE type = 'table' AND name = @tableName", _dbConnection);
            command.Parameters.AddWithValue("@tableName", tableName);

            Object o = command.ExecuteScalar();
            int count = Convert.ToInt32(o);

            if (count > 0)
                return true;

            return false;
        }

        private bool DoesVariableExist(string uniqueName)
        {
            string[] tables = new string[] { "Int16", "Int32", "Int64", "Float", "Double", "String", "ByteArray", "Boolean" };

            foreach (string table in tables)
            {
                if (DoesTableExist(table))
                {
                    SQLiteCommand command = new SQLiteCommand("SELECT COUNT(1) FROM " + IntUtilities.SqlEscape(table) + " WHERE VariableName = @varName", _dbConnection);
                    command.Parameters.AddWithValue("@varName", uniqueName);

                    Object o = command.ExecuteScalar();
                    int count = Convert.ToInt32(o);

                    if (count > 0)
                        return true;
                }
                else continue;
            }
            return false;
        }

        private bool DoesMetaDataVariableExist(string varName)
        {
            if (DoesTableExist("_MetaData"))
            {
                SQLiteCommand command = new SQLiteCommand("SELECT COUNT(1) FROM _MetaData WHERE VariableName = @varName", _dbConnection);
                command.Parameters.AddWithValue("@varName", varName);

                Object o = command.ExecuteScalar();
                int count = Convert.ToInt32(o);

                if (count > 0)
                    return true;
            }
            return false;
        }

        #region Create Tables

        private void CreateMetaTable()
        {
            SQLiteCommand command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS _MetaData (VariableName VARCHAR(255), VariableGroup VARCHAR(255), VariableValue VARCHAR(255), VariableDesc VARCHAR(255))", _dbConnection);
            command.ExecuteNonQuery();
        }

        private void CreateInt16Table()
        {
            SQLiteCommand command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Int16 (VariableName VARCHAR(255), VariableGroup VARCHAR(255), VariableValue SMALLINT, VariableDefault SMALLINT, VariableDesc VARCHAR(255))", _dbConnection);
            command.ExecuteNonQuery();
        }

        private void CreateInt32Table()
        {
            SQLiteCommand command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Int32 (VariableName VARCHAR(255), VariableGroup VARCHAR(255), VariableValue INT, VariableDefault INT, VariableDesc VARCHAR(255))", _dbConnection);
            command.ExecuteNonQuery();
        }

        private void CreateInt64Table()
        {
            SQLiteCommand command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Int64 (VariableName VARCHAR(255), VariableGroup VARCHAR(255), VariableValue BIGINT, VariableDefault BIGINT, VariableDesc VARCHAR(255))", _dbConnection);
            command.ExecuteNonQuery();
        }

        private void CreateFloatTable()
        {
            SQLiteCommand command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Float (VariableName VARCHAR(255), VariableGroup VARCHAR(255), VariableValue FLOAT, VariableDefault FLOAT, VariableDesc VARCHAR(255))", _dbConnection);
            command.ExecuteNonQuery();
        }

        private void CreateDoubleTable()
        {
            SQLiteCommand command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Double (VariableName VARCHAR(255), VariableGroup VARCHAR(255), VariableValue REAL, VariableDefault REAL, VariableDesc VARCHAR(255))", _dbConnection);
            command.ExecuteNonQuery();
        }

        private void CreateStringTable()
        {
            SQLiteCommand command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS String (VariableName VARCHAR(255), VariableGroup VARCHAR(255), VariableValue VARCHAR(255), VariableDefault VARCHAR(255), VariableDesc VARCHAR(255))", _dbConnection);
            command.ExecuteNonQuery();
        }

        private void CreateByteArrayTable()
        {
            SQLiteCommand command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS ByteArray (VariableName VARCHAR(255), VariableGroup VARCHAR(255), VariableValue VARBINARY(255), VariableDefault VARBINARY(255), VariableDesc VARCHAR(255))", _dbConnection);
            command.ExecuteNonQuery();
        }

        private void CreateBooleanTable()
        {
            SQLiteCommand command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Boolean (VariableName VARCHAR(255), VariableGroup VARCHAR(255), VariableValue BIT, VariableDefault BIT, VariableDesc VARCHAR(255))", _dbConnection);
            command.ExecuteNonQuery();
        }

        #endregion

        #endregion

    }
}
