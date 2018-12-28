using System;
using SimpleSettingsManager.Mode;

namespace SimpleSettingsManager
{
    public class SSM
    {
        private string _settingsPath;
        private Mode _mode;
        private IMode _handler;

        public SSM(string settingsPath, Mode mode = Mode.Auto)
        {
            _settingsPath = settingsPath;
            _mode = mode;

            switch (_mode)
            {
                case Mode.SQLite:
                    {
                        _handler = new SQLite();
                        break;
                    }
                case Mode.XML:
                    {
                        _handler = new XML();
                        break;
                    }
                case Mode.Auto:
                    {
                        if (IntUtilities.isSQLiteDB(_settingsPath))
                            _handler = new SQLite();
                        else
                            _handler = new XML();
                        break;
                    }
                default:
                    break;
            }
        }

        public void Open()
        {
            _handler.Open(_settingsPath);
        }

        public void Close()
        {
            _handler.Close();
        }

        #region Add Variables

        public bool AddInt16(string uniqueName, Int16 value, string description, string group = "default")
        {
            return _handler.AddInt16(uniqueName, value, description, group);
        }

        public bool AddInt32(string uniqueName, Int32 value, string description, string group = "default")
        {
            return _handler.AddInt32(uniqueName, value, description, group);
        }

        public bool AddInt(string uniqueName, int value, string description, string group = "default")
        {
            return AddInt32(uniqueName, value, description, group);
        }

        public bool AddInt64(string uniqueName, Int64 value, string description, string group = "default")
        {
            return _handler.AddInt64(uniqueName, value, description, group);
        }

        public bool AddLong(string uniqueName, long value, string description, string group = "default")
        {
            return AddInt64(uniqueName, value, description, group);
        }

        public bool AddFloat(string uniqueName, float value, string description, string group = "default")
        {
            return _handler.AddFloat(uniqueName, value, description, group);
        }

        public bool AddDouble(string uniqueName, double value, string description, string group = "default")
        {
            return _handler.AddDouble(uniqueName, value, description, group);
        }

        public bool AddString(string uniqueName, string value, string description, string group = "default")
        {
            return _handler.AddString(uniqueName, value, description, group);
        }

        public bool AddByteArray(string uniqueName, byte[] value, string description, string group = "default")
        {
            return _handler.AddByteArray(uniqueName, value, description, group);
        }

        public bool AddBoolean(string uniqueName, bool value, string description, string group = "default")
        {
            return _handler.AddBoolean(uniqueName, value, description, group);
        }

        #endregion
        #region Set Variables

        public bool SetInt16(string uniqueName, Int16 value)
        {
            return _handler.SetInt16(uniqueName, value);
        }

        public bool SetInt32(string uniqueName, Int32 value)
        {
            return _handler.SetInt32(uniqueName, value);
        }

        public bool SetInt(string uniqueName, int value)
        {
            return SetInt32(uniqueName, value);
        }

        public bool SetInt64(string uniqueName, Int64 value)
        {
            return _handler.SetInt64(uniqueName, value);
        }

        public bool SetLong(string uniqueName, long value)
        {
            return SetInt64(uniqueName, value);
        }

        public bool SetFloat(string uniqueName, float value)
        {
            return _handler.SetFloat(uniqueName, value);
        }

        public bool SetDouble(string uniqueName, double value)
        {
            return _handler.SetDouble(uniqueName, value);
        }

        public bool SetString(string uniqueName, string value)
        {
            return _handler.SetString(uniqueName, value);
        }

        public bool SetByteArray(string uniqueName, byte[] value)
        {
            return _handler.SetByteArray(uniqueName, value);
        }

        public bool SetBoolean(string uniqueName, bool value)
        {
            return _handler.SetBoolean(uniqueName, value);
        }

        #endregion
        #region Edit Variables

        public bool EditInt16(string uniqueName, string description, string group)
        {
            return _handler.EditInt16(uniqueName, description, group);
        }

        public bool EditInt32(string uniqueName, string description, string group)
        {
            return _handler.EditInt32(uniqueName, description, group);
        }

        public bool EditInt(string uniqueName, string description, string group)
        {
            return EditInt32(uniqueName, description, group);
        }

        public bool EditInt64(string uniqueName, string description, string group)
        {
            return _handler.EditInt64(uniqueName, description, group);
        }

        public bool EditLong(string uniqueName, string description, string group)
        {
            return EditInt64(uniqueName, description, group);
        }

        public bool EditFloat(string uniqueName, string description, string group)
        {
            return _handler.EditFloat(uniqueName, description, group);
        }

        public bool EditDouble(string uniqueName, string description, string group)
        {
            return _handler.EditDouble(uniqueName, description, group);
        }

        public bool EditString(string uniqueName, string description, string group)
        {
            return _handler.EditString(uniqueName, description, group);
        }

        public bool EditByteArray(string uniqueName, string description, string group)
        {
            return _handler.EditByteArray(uniqueName, description, group);
        }

        public bool EditBoolean(string uniqueName, string description, string group)
        {
            return _handler.EditBoolean(uniqueName, description, group);
        }

        #endregion
        #region Get Variables

        public Int16 GetInt16(string uniqueName)
        {
            return _handler.GetInt16(uniqueName);
        }

        public Int32 GetInt32(string uniqueName)
        {
            return _handler.GetInt32(uniqueName);
        }

        public int GetInt(string uniqueName)
        {
            return GetInt32(uniqueName);
        }

        public Int64 GetInt64(string uniqueName)
        {
            return _handler.GetInt64(uniqueName);
        }

        public long GetLong(string uniqueName)
        {
            return GetInt64(uniqueName);
        }

        public float GetFloat(string uniqueName)
        {
            return _handler.GetFloat(uniqueName);
        }

        public double GetDouble(string uniqueName)
        {
            return _handler.GetDouble(uniqueName);
        }

        public string GetString(string uniqueName)
        {
            return _handler.GetString(uniqueName);
        }

        public byte[] GetByteArray(string uniqueName)
        {
            return _handler.GetByteArray(uniqueName);
        }

        public bool GetBoolean(string uniqueName)
        {
            return _handler.GetBoolean(uniqueName);
        }

        #endregion
        #region Delete Variables

        public bool DeleteInt16(string uniqueName)
        {
            return _handler.DeleteInt16(uniqueName);
        }

        public bool DeleteInt32(string uniqueName)
        {
            return _handler.DeleteInt32(uniqueName);
        }

        public bool DeleteInt(string uniqueName)
        {
            return DeleteInt32(uniqueName);
        }

        public bool DeleteInt64(string uniqueName)
        {
            return _handler.DeleteInt64(uniqueName);
        }

        public bool DeleteLong(string uniqueName)
        {
            return DeleteInt64(uniqueName);
        }

        public bool DeleteFloat(string uniqueName)
        {
            return _handler.DeleteFloat(uniqueName);
        }

        public bool DeleteDouble(string uniqueName)
        {
            return _handler.DeleteDouble(uniqueName);
        }

        public bool DeleteString(string uniqueName)
        {
            return _handler.DeleteString(uniqueName);
        }

        public bool DeleteByteArray(string uniqueName)
        {
            return _handler.DeleteByteArray(uniqueName);
        }

        public bool DeleteBoolean(string uniqueName)
        {
            return _handler.DeleteBoolean(uniqueName);
        }

        #endregion

        #region Enums

        public enum Mode {
            Auto,
            SQLite,
            XML
        };

        #endregion
    }
}
