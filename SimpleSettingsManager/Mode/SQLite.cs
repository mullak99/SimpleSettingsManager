using SimpleSettingsManager.Data;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace SimpleSettingsManager.Mode
{
    internal class SQLite : IMode
    {
        private SQLiteConnection _dbConnection;
        private string _settingsPath;

        #region Init

        public void Open(SSM_File ssmFile)
        {
            _settingsPath = ssmFile.GetPath(true);

            bool isNewDatabase = createSQLiFile();

            _dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", _settingsPath));
            _dbConnection.Open();

            if (isNewDatabase)
            {
                CreateMetaTable();
                AddNewSsmMetaData();
            }
            else ExistingSsmMetaData();
        }

        private void AddNewSsmMetaData()
        {
            AddMetaData("SSM_CreationAppVersion", "HistoricalInfo", SSM.GetVersion(), "The version of SSM used to create the SSM file.");
            AddMetaData("SSM_LastAccessAppVersion", "LastAccessInfo", SSM.GetVersion(), "The version of SSM used to last edit the SSM file.");
            AddMetaData("SSM_CreationFormatVersion", "HistoricalInfo", SSM.GetSsmFormatVersion(), "The original SSM Format version of the SSM file.");
            AddMetaData("SSM_LastAccessFormatVersion", "LastAccessInfo", SSM.GetSsmFormatVersion(), "The current SSM Format version of the SSM file.");
            AddMetaData("SSM_CreationTimestamp", "HistoricalInfo", Convert.ToString(IntUtilities.GetUnixTimestamp()), "The timestamp of when the SSM file was created.");
            AddMetaData("SSM_LastLoadedTimestamp", "LastAccessInfo", Convert.ToString(IntUtilities.GetUnixTimestamp()), "The timestamp of when the SSM file was last loaded.");
            AddMetaData("SSM_CreationMode", "HistoricalInfo", "SQLite", "The SSM mode used to create the SSM file.");
            AddMetaData("SSM_LastAccessMode", "LastAccessInfo", "SQLite", "The SSM mode used to last access the SSM file.");
        }

        private void ExistingSsmMetaData()
        {
            SetMetaData("SSM_LastAccessFormatVersion", SSM.GetSsmFormatVersion());
            SetMetaData("SSM_LastLoadedTimestamp", Convert.ToString(IntUtilities.GetUnixTimestamp()));
            SetMetaData("SSM_LastAccessMode", "SQLite");
        }

        public void Close()
        {
            _dbConnection.Close();
        }

        public void UpdateMigrationStatus()
        {
            if (!AddMetaData("SSM_LastMigration", "MigrationInfo", Convert.ToString(IntUtilities.GetUnixTimestamp()), "The timestamp of when the SSM file was last migrated."))
                SetMetaData("SSM_LastMigration", Convert.ToString(IntUtilities.GetUnixTimestamp()));

            if (!AddMetaData("SSM_MigrationCount", "MigrationInfo", "1", "The total number of migrations the SSM file has gone through."))
            {
                ulong totalMigrations = Convert.ToUInt64(GetVarInTable("SSM_MigrationCount", "_MetaData"));
                totalMigrations++;
                SetMetaData("SSM_MigrationCount", Convert.ToString(totalMigrations));
            }
            ExistingSsmMetaData();
        }

        private bool createSQLiFile()
        {
            if (!File.Exists(_settingsPath))
            {
                SQLiteConnection.CreateFile(_settingsPath);
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

        public bool AddInt64(string uniqueName, Int64 value, string description, string group = "default")
        {
            CreateInt64Table();
            return AddVarToTable(uniqueName, value, description, group, "Int64");
        }

        public bool AddUInt16(string uniqueName, UInt16 value, string description, string group = "default")
        {
            CreateUInt16Table();
            return AddVarToTable(uniqueName, value, description, group, "UInt16");
        }

        public bool AddUInt32(string uniqueName, UInt32 value, string description, string group = "default")
        {
            CreateUInt32Table();
            return AddVarToTable(uniqueName, value, description, group, "UInt32");
        }

        public bool AddUInt64(string uniqueName, UInt64 value, string description, string group = "default")
        {
            CreateUInt64Table();
            return AddVarToTable(uniqueName, value, description, group, "UInt64");
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

        public bool SetInt64(string uniqueName, Int64 value)
        {
            return SetVarInTable(uniqueName, value, "Int64");
        }

        public bool SetUInt16(string uniqueName, UInt16 value)
        {
            return SetVarInTable(uniqueName, value, "UInt16");
        }

        public bool SetUInt32(string uniqueName, UInt32 value)
        {
            return SetVarInTable(uniqueName, value, "UInt32");
        }

        public bool SetUInt64(string uniqueName, UInt64 value)
        {
            return SetVarInTable(uniqueName, value, "UInt64");
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

        public bool EditInt64(string uniqueName, string description, string group)
        {
            return EditVarInTable(uniqueName, description, group, "Int64");
        }

        public bool EditUInt16(string uniqueName, string description, string group)
        {
            return EditVarInTable(uniqueName, description, group, "UInt16");
        }

        public bool EditUInt32(string uniqueName, string description, string group)
        {
            return EditVarInTable(uniqueName, description, group, "UInt32");
        }

        public bool EditUInt64(string uniqueName, string description, string group)
        {
            return EditVarInTable(uniqueName, description, group, "UInt64");
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

        public Int64 GetInt64(string uniqueName)
        {
            return Convert.ToInt64(GetVarInTable(uniqueName, "Int64"));
        }

        public UInt16 GetUInt16(string uniqueName)
        {
            return Convert.ToUInt16(GetVarInTable(uniqueName, "UInt16"));
        }

        public UInt32 GetUInt32(string uniqueName)
        {
            return Convert.ToUInt32(GetVarInTable(uniqueName, "UInt32"));
        }

        public UInt64 GetUInt64(string uniqueName)
        {
            return Convert.ToUInt64(GetVarInTable(uniqueName, "UInt64"));
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

        public bool DeleteInt64(string uniqueName)
        {
            return DeleteVarInTable(uniqueName, "Int64");
        }

        public bool DeleteUInt16(string uniqueName)
        {
            return DeleteVarInTable(uniqueName, "UInt16");
        }

        public bool DeleteUInt32(string uniqueName)
        {
            return DeleteVarInTable(uniqueName, "UInt32");
        }

        public bool DeleteUInt64(string uniqueName)
        {
            return DeleteVarInTable(uniqueName, "UInt64");
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

        #region DataEntry

        public void ImportDataEntry(DataEntry dataEntry)
        {
            if (dataEntry.GetVariableType() == typeof(MetaDataObject))
            {
                if (!DoesMetaDataVariableExist(dataEntry.GetVariableName()))
                {
                    AddMetaData(dataEntry.GetVariableName(), dataEntry.GetVariableGroup(), Encoding.UTF8.GetString(dataEntry.GetVariableValue()), dataEntry.GetVariableDescription());
                }
                else
                {
                    SetMetaData(dataEntry.GetVariableName(), Encoding.UTF8.GetString(dataEntry.GetVariableValue()));
                    EditMetaData(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
            }
            else if (dataEntry.GetVariableType() == typeof(Int16))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "Int16"))
                {
                    AddInt16(dataEntry.GetVariableName(), BitConverter.ToInt16(dataEntry.GetVariableValue(), 0), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetInt16(dataEntry.GetVariableName(), BitConverter.ToInt16(dataEntry.GetVariableValue(), 0));
                    EditInt16(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefaultInTable(dataEntry.GetVariableName(), BitConverter.ToInt16(dataEntry.GetVariableDefault(), 0), "Int16");
            }
            else if (dataEntry.GetVariableType() == typeof(Int32))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "Int32"))
                {
                    AddInt32(dataEntry.GetVariableName(), BitConverter.ToInt32(dataEntry.GetVariableValue(), 0), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetInt32(dataEntry.GetVariableName(), BitConverter.ToInt32(dataEntry.GetVariableValue(), 0));
                    EditInt32(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefaultInTable(dataEntry.GetVariableName(), BitConverter.ToInt32(dataEntry.GetVariableDefault(), 0), "Int32");
            }
            else if (dataEntry.GetVariableType() == typeof(Int64))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "Int64"))
                {
                    AddInt64(dataEntry.GetVariableName(), BitConverter.ToInt64(dataEntry.GetVariableValue(), 0), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetInt64(dataEntry.GetVariableName(), BitConverter.ToInt64(dataEntry.GetVariableValue(), 0));
                    EditInt64(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefaultInTable(dataEntry.GetVariableName(), BitConverter.ToInt64(dataEntry.GetVariableDefault(), 0), "Int64");
            }
            else if (dataEntry.GetVariableType() == typeof(UInt16))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "UInt16"))
                {
                    AddUInt16(dataEntry.GetVariableName(), BitConverter.ToUInt16(dataEntry.GetVariableValue(), 0), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetUInt16(dataEntry.GetVariableName(), BitConverter.ToUInt16(dataEntry.GetVariableValue(), 0));
                    EditUInt16(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefaultInTable(dataEntry.GetVariableName(), BitConverter.ToUInt16(dataEntry.GetVariableDefault(), 0), "UInt16");
            }
            else if (dataEntry.GetVariableType() == typeof(UInt32))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "UInt32"))
                {
                    AddUInt32(dataEntry.GetVariableName(), BitConverter.ToUInt32(dataEntry.GetVariableValue(), 0), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetUInt32(dataEntry.GetVariableName(), BitConverter.ToUInt32(dataEntry.GetVariableValue(), 0));
                    EditUInt32(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefaultInTable(dataEntry.GetVariableName(), BitConverter.ToUInt32(dataEntry.GetVariableDefault(), 0), "UInt32");
            }
            else if (dataEntry.GetVariableType() == typeof(UInt64))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "UInt64"))
                {
                    AddUInt64(dataEntry.GetVariableName(), BitConverter.ToUInt64(dataEntry.GetVariableValue(), 0), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetUInt64(dataEntry.GetVariableName(), BitConverter.ToUInt64(dataEntry.GetVariableValue(), 0));
                    EditUInt64(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefaultInTable(dataEntry.GetVariableName(), BitConverter.ToUInt64(dataEntry.GetVariableDefault(), 0), "UInt64");
            }
            else if (dataEntry.GetVariableType() == typeof(float))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "Float"))
                {
                    AddFloat(dataEntry.GetVariableName(), BitConverter.ToSingle(dataEntry.GetVariableValue(), 0), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetFloat(dataEntry.GetVariableName(), BitConverter.ToSingle(dataEntry.GetVariableValue(), 0));
                    EditFloat(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefaultInTable(dataEntry.GetVariableName(), BitConverter.ToSingle(dataEntry.GetVariableDefault(), 0), "Float");
            }
            else if (dataEntry.GetVariableType() == typeof(Double))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "Double"))
                {
                    AddDouble(dataEntry.GetVariableName(), BitConverter.ToDouble(dataEntry.GetVariableValue(), 0), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetDouble(dataEntry.GetVariableName(), BitConverter.ToDouble(dataEntry.GetVariableValue(), 0));
                    EditDouble(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefaultInTable(dataEntry.GetVariableName(), BitConverter.ToDouble(dataEntry.GetVariableDefault(), 0), "Double");
            }
            else if (dataEntry.GetVariableType() == typeof(String))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "String"))
                {
                    AddString(dataEntry.GetVariableName(), Encoding.UTF8.GetString(dataEntry.GetVariableValue()), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetString(dataEntry.GetVariableName(), Encoding.UTF8.GetString(dataEntry.GetVariableValue()));
                    EditString(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefaultInTable(dataEntry.GetVariableName(), Encoding.UTF8.GetString(dataEntry.GetVariableDefault()), "String");
            }
            else if (dataEntry.GetVariableType() == typeof(byte[]))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "ByteArray"))
                {
                    AddByteArray(dataEntry.GetVariableName(), dataEntry.GetVariableValue(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetByteArray(dataEntry.GetVariableName(), dataEntry.GetVariableValue());
                    EditByteArray(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefaultInTable(dataEntry.GetVariableName(), dataEntry.GetVariableDefault(), "ByteArray");
            }
            else if (dataEntry.GetVariableType() == typeof(Boolean))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "Boolean"))
                {
                    AddBoolean(dataEntry.GetVariableName(), BitConverter.ToBoolean(dataEntry.GetVariableValue(), 0), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetBoolean(dataEntry.GetVariableName(), BitConverter.ToBoolean(dataEntry.GetVariableValue(), 0));
                    EditBoolean(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefaultInTable(dataEntry.GetVariableName(), BitConverter.ToBoolean(dataEntry.GetVariableDefault(), 0), "Boolean");
            }
        }

        public DataEntry[] GetAllMetaData()
        {
            if (DoesTableExist("_MetaData"))
            {
                List<DataEntry> dataList = new List<DataEntry>();

                SQLiteCommand command = new SQLiteCommand("SELECT VariableName, VariableGroup, VariableValue, VariableDesc FROM _MetaData", _dbConnection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    dataList.Add(new DataEntry(typeof(MetaDataObject), Convert.ToString(reader["VariableName"]), Convert.ToString(reader["VariableGroup"]), Encoding.UTF8.GetBytes(Convert.ToString(reader["VariableValue"])), Encoding.UTF8.GetBytes(Convert.ToString(reader["VariableValue"])), Convert.ToString(reader["VariableDesc"])));
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllInt16()
        {
            if(DoesTableExist("Int16"))
            {
                List<DataEntry> dataList = new List<DataEntry>();

                SQLiteCommand command = new SQLiteCommand("SELECT VariableName, VariableGroup, VariableValue, VariableDefault, VariableDesc FROM Int16", _dbConnection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    dataList.Add(new DataEntry(typeof(Int16), Convert.ToString(reader["VariableName"]), Convert.ToString(reader["VariableGroup"]), BitConverter.GetBytes(Convert.ToInt16(reader["VariableValue"])), BitConverter.GetBytes(Convert.ToInt16(reader["VariableDefault"])), Convert.ToString(reader["VariableDesc"])));
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllInt32()
        {
            if (DoesTableExist("Int32"))
            {
                List<DataEntry> dataList = new List<DataEntry>();

                SQLiteCommand command = new SQLiteCommand("SELECT VariableName, VariableGroup, VariableValue, VariableDefault, VariableDesc FROM Int32", _dbConnection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    dataList.Add(new DataEntry(typeof(Int16), Convert.ToString(reader["VariableName"]), Convert.ToString(reader["VariableGroup"]), BitConverter.GetBytes(Convert.ToInt32(reader["VariableValue"])), BitConverter.GetBytes(Convert.ToInt32(reader["VariableDefault"])), Convert.ToString(reader["VariableDesc"])));
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllInt64()
        {
            if (DoesTableExist("Int64"))
            {
                List<DataEntry> dataList = new List<DataEntry>();

                SQLiteCommand command = new SQLiteCommand("SELECT VariableName, VariableGroup, VariableValue, VariableDefault, VariableDesc FROM Int64", _dbConnection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    dataList.Add(new DataEntry(typeof(Int64), Convert.ToString(reader["VariableName"]), Convert.ToString(reader["VariableGroup"]), BitConverter.GetBytes(Convert.ToInt64(reader["VariableValue"])), BitConverter.GetBytes(Convert.ToInt64(reader["VariableDefault"])), Convert.ToString(reader["VariableDesc"])));
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllUInt16()
        {
            if (DoesTableExist("UInt16"))
            {
                List<DataEntry> dataList = new List<DataEntry>();

                SQLiteCommand command = new SQLiteCommand("SELECT VariableName, VariableGroup, VariableValue, VariableDefault, VariableDesc FROM UInt16", _dbConnection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    dataList.Add(new DataEntry(typeof(UInt16), Convert.ToString(reader["VariableName"]), Convert.ToString(reader["VariableGroup"]), (byte[])reader["VariableValue"], (byte[])reader["VariableDefault"], Convert.ToString(reader["VariableDesc"])));
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllUInt32()
        {
            if (DoesTableExist("UInt32"))
            {
                List<DataEntry> dataList = new List<DataEntry>();

                SQLiteCommand command = new SQLiteCommand("SELECT VariableName, VariableGroup, VariableValue, VariableDefault, VariableDesc FROM UInt32", _dbConnection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    dataList.Add(new DataEntry(typeof(UInt16), Convert.ToString(reader["VariableName"]), Convert.ToString(reader["VariableGroup"]), (byte[])reader["VariableValue"], (byte[])reader["VariableDefault"], Convert.ToString(reader["VariableDesc"])));
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllUInt64()
        {
            if (DoesTableExist("UInt64"))
            {
                List<DataEntry> dataList = new List<DataEntry>();

                SQLiteCommand command = new SQLiteCommand("SELECT VariableName, VariableGroup, VariableValue, VariableDefault, VariableDesc FROM UInt64", _dbConnection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    dataList.Add(new DataEntry(typeof(UInt64), Convert.ToString(reader["VariableName"]), Convert.ToString(reader["VariableGroup"]), (byte[])reader["VariableValue"], (byte[])reader["VariableDefault"], Convert.ToString(reader["VariableDesc"])));
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllFloat()
        {
            if (DoesTableExist("Float"))
            {
                List<DataEntry> dataList = new List<DataEntry>();

                SQLiteCommand command = new SQLiteCommand("SELECT VariableName, VariableGroup, VariableValue, VariableDefault, VariableDesc FROM Float", _dbConnection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    dataList.Add(new DataEntry(typeof(float), Convert.ToString(reader["VariableName"]), Convert.ToString(reader["VariableGroup"]), BitConverter.GetBytes(Convert.ToSingle(reader["VariableValue"])), BitConverter.GetBytes(Convert.ToSingle(reader["VariableDefault"])), Convert.ToString(reader["VariableDesc"])));
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllDouble()
        {
            if (DoesTableExist("Double"))
            {
                List<DataEntry> dataList = new List<DataEntry>();

                SQLiteCommand command = new SQLiteCommand("SELECT VariableName, VariableGroup, VariableValue, VariableDefault, VariableDesc FROM Double", _dbConnection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    dataList.Add(new DataEntry(typeof(Double), Convert.ToString(reader["VariableName"]), Convert.ToString(reader["VariableGroup"]), BitConverter.GetBytes(Convert.ToDouble(reader["VariableValue"])), BitConverter.GetBytes(Convert.ToDouble(reader["VariableDefault"])), Convert.ToString(reader["VariableDesc"])));
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllString()
        {
            if (DoesTableExist("String"))
            {
                List<DataEntry> dataList = new List<DataEntry>();

                SQLiteCommand command = new SQLiteCommand("SELECT VariableName, VariableGroup, VariableValue, VariableDefault, VariableDesc FROM String", _dbConnection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    dataList.Add(new DataEntry(typeof(String), Convert.ToString(reader["VariableName"]), Convert.ToString(reader["VariableGroup"]), Encoding.UTF8.GetBytes(Convert.ToString(reader["VariableValue"])), Encoding.UTF8.GetBytes(Convert.ToString(reader["VariableDefault"])), Convert.ToString(reader["VariableDesc"])));
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllByteArrays()
        {
            if (DoesTableExist("ByteArray"))
            {
                List<DataEntry> dataList = new List<DataEntry>();

                SQLiteCommand command = new SQLiteCommand("SELECT VariableName, VariableGroup, VariableValue, VariableDefault, VariableDesc FROM ByteArray", _dbConnection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    dataList.Add(new DataEntry(typeof(byte[]), Convert.ToString(reader["VariableName"]), Convert.ToString(reader["VariableGroup"]), IntUtilities.ObjectToByteArray(reader["VariableValue"]), IntUtilities.ObjectToByteArray(reader["VariableDefault"]), Convert.ToString(reader["VariableDesc"])));
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllBooleans()
        {
            if (DoesTableExist("Boolean"))
            {
                List<DataEntry> dataList = new List<DataEntry>();

                SQLiteCommand command = new SQLiteCommand("SELECT VariableName, VariableGroup, VariableValue, VariableDefault, VariableDesc FROM Boolean", _dbConnection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    dataList.Add(new DataEntry(typeof(Boolean), Convert.ToString(reader["VariableName"]), Convert.ToString(reader["VariableGroup"]), BitConverter.GetBytes(Convert.ToBoolean(reader["VariableValue"])), BitConverter.GetBytes(Convert.ToBoolean(reader["VariableDefault"])), Convert.ToString(reader["VariableDesc"])));
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllTypes()
        {
            List<DataEntry> dataList = new List<DataEntry>();

            if (this.GetAllMetaData() != null) dataList.AddRange(this.GetAllMetaData());
            if (this.GetAllInt16() != null) dataList.AddRange(this.GetAllInt16());
            if (this.GetAllInt32() != null) dataList.AddRange(this.GetAllInt32());
            if (this.GetAllInt64() != null) dataList.AddRange(this.GetAllInt64());
            if (this.GetAllUInt16() != null) dataList.AddRange(this.GetAllUInt16());
            if (this.GetAllUInt32() != null) dataList.AddRange(this.GetAllUInt32());
            if (this.GetAllUInt64() != null) dataList.AddRange(this.GetAllUInt64());
            if (this.GetAllFloat() != null) dataList.AddRange(this.GetAllFloat());
            if (this.GetAllDouble() != null) dataList.AddRange(this.GetAllDouble());
            if (this.GetAllString() != null) dataList.AddRange(this.GetAllString());
            if (this.GetAllByteArrays() != null) dataList.AddRange(this.GetAllByteArrays());
            if (this.GetAllBooleans() != null) dataList.AddRange(this.GetAllBooleans());

            return dataList.ToArray();
        }

        #endregion

        #region Mode

        public string GetMode()
        {
            return "SQLite";
        }

        #endregion

        #region Raw Database Handling

        private bool AddVarToTable(string uniqueName, object value, string description, string group, string table)
        {
            if (!DoesVariableExist(uniqueName, table))
            {
                SQLiteCommand command = new SQLiteCommand("INSERT INTO " + IntUtilities.SqlEscape(table) + " (VariableName, VariableGroup, VariableValue, VariableDefault, VariableDesc) VALUES (@varName, @varGroup, @varValue, @varValue, @varDesc)", _dbConnection);

                command.Parameters.AddWithValue("@varName", uniqueName);
                command.Parameters.AddWithValue("@varGroup", group);

                if (value.GetType() == typeof(ushort))
                    command.Parameters.AddWithValue("@varValue", BitConverter.GetBytes((ushort)value));
                else if (value.GetType() == typeof(uint))
                    command.Parameters.AddWithValue("@varValue", BitConverter.GetBytes((uint)value));
                else if (value.GetType() == typeof(ulong))
                    command.Parameters.AddWithValue("@varValue", BitConverter.GetBytes((ulong)value));
                else
                    command.Parameters.AddWithValue("@varValue", value);

                command.Parameters.AddWithValue("@varDesc", description);
                command.ExecuteNonQuery();

                return true;
            }
            return false;
        }

        private bool SetVarInTable(string uniqueName, object value, string table)
        {
            if (DoesVariableExist(uniqueName, table))
            {
                SQLiteCommand command = new SQLiteCommand("UPDATE " + IntUtilities.SqlEscape(table) + " SET VariableValue = @varValue WHERE VariableName = @varName", _dbConnection);

                command.Parameters.AddWithValue("@varName", uniqueName);

                if (value.GetType() == typeof(ushort))
                    command.Parameters.AddWithValue("@varValue", BitConverter.GetBytes((ushort)value));
                else if (value.GetType() == typeof(uint))
                    command.Parameters.AddWithValue("@varValue", BitConverter.GetBytes((uint)value));
                else if (value.GetType() == typeof(ulong))
                    command.Parameters.AddWithValue("@varValue", BitConverter.GetBytes((ulong)value));
                else
                    command.Parameters.AddWithValue("@varValue", value);

                command.ExecuteNonQuery();

                return true;
            }
            return false;
        }

        private bool EditVarInTable(string uniqueName, string description, string group, string table)
        {
            if (DoesVariableExist(uniqueName, table))
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
            if (DoesVariableExist(uniqueName, table))
            {
                SQLiteCommand command = new SQLiteCommand("SELECT VariableValue FROM " + IntUtilities.SqlEscape(table) + " WHERE VariableName = @varName", _dbConnection);

                command.Parameters.AddWithValue("@varName", uniqueName);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (table == "UInt16")
                            return BitConverter.ToUInt16((byte[])reader[0], 0);
                        else if (table == "UInt32")
                            return BitConverter.ToUInt32((byte[])reader[0], 0);
                        else if (table == "UInt64")
                            return BitConverter.ToUInt64((byte[])reader[0], 0);
                        else
                            return reader[0];
                    }
                }
            }
            return null;
        }

        private bool DeleteVarInTable(string uniqueName, string table)
        {
            if (DoesVariableExist(uniqueName, table))
            {
                SQLiteCommand command = new SQLiteCommand("DELETE FROM " + IntUtilities.SqlEscape(table) + " WHERE VariableName = @varName", _dbConnection);

                command.Parameters.AddWithValue("@varName", uniqueName);
                command.ExecuteNonQuery();

                return true;
            }
            return false;
        }

        private bool EditVarDefaultInTable(string uniqueName, object defaultValue, string table)
        {
            if (DoesVariableExist(uniqueName, table))
            {
                SQLiteCommand command = new SQLiteCommand("UPDATE " + IntUtilities.SqlEscape(table) + " SET VariableDefault = @varValue WHERE VariableName = @varName", _dbConnection);

                command.Parameters.AddWithValue("@varName", uniqueName);
                command.Parameters.AddWithValue("@varValue", defaultValue);
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

        private bool DoesVariableExist(string uniqueName, string table)
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

        private void CreateUInt16Table()
        {
            SQLiteCommand command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS UInt16 (VariableName VARCHAR(255), VariableGroup VARCHAR(255), VariableValue VARBINARY(2), VariableDefault VARBINARY(2), VariableDesc VARCHAR(255))", _dbConnection);
            command.ExecuteNonQuery();
        }

        private void CreateUInt32Table()
        {
            SQLiteCommand command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS UInt32 (VariableName VARCHAR(255), VariableGroup VARCHAR(255), VariableValue VARBINARY(4), VariableDefault VARBINARY(4), VariableDesc VARCHAR(255))", _dbConnection);
            command.ExecuteNonQuery();
        }

        private void CreateUInt64Table()
        {
            SQLiteCommand command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS UInt64 (VariableName VARCHAR(255), VariableGroup VARCHAR(255), VariableValue VARBINARY(8), VariableDefault VARBINARY(8), VariableDesc VARCHAR(255))", _dbConnection);
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
