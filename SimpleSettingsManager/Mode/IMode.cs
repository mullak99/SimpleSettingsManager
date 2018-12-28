using System;

namespace SimpleSettingsManager.Mode
{
    internal interface IMode
    {
        void Open(string path);
        void Close();

        bool AddInt16(string uniqueName, Int16 value, string description, string group = "default");
        bool AddInt32(string uniqueName, Int32 value, string description, string group = "default");
        bool AddInt(string uniqueName, int value, string description, string group = "default");
        bool AddInt64(string uniqueName, Int64 value, string description, string group = "default");
        bool AddLong(string uniqueName, long value, string description, string group = "default");
        bool AddFloat(string uniqueName, float value, string description, string group = "default");
        bool AddDouble(string uniqueName, double value, string description, string group = "default");
        bool AddString(string uniqueName, string value, string description, string group = "default");
        bool AddByteArray(string uniqueName, byte[] value, string description, string group = "default");
        bool AddBoolean(string uniqueName, bool value, string description, string group = "default");

        bool SetInt16(string uniqueName, Int16 value);
        bool SetInt32(string uniqueName, Int32 value);
        bool SetInt(string uniqueName, int value);
        bool SetInt64(string uniqueName, Int64 value);
        bool SetLong(string uniqueName, long value);
        bool SetFloat(string uniqueName, float value);
        bool SetDouble(string uniqueName, double value);
        bool SetString(string uniqueName, string value);
        bool SetByteArray(string uniqueName, byte[] value);
        bool SetBoolean(string uniqueName, bool value);

        bool EditInt16(string uniqueName, string description, string group);
        bool EditInt32(string uniqueName, string description, string group);
        bool EditInt(string uniqueName, string description, string group);
        bool EditInt64(string uniqueName, string description, string group);
        bool EditLong(string uniqueName, string description, string group);
        bool EditFloat(string uniqueName, string description, string group);
        bool EditDouble(string uniqueName, string description, string group);
        bool EditString(string uniqueName, string description, string group);
        bool EditByteArray(string uniqueName, string description, string group);
        bool EditBoolean(string uniqueName, string description, string group);

        Int16 GetInt16(string uniqueName);
        Int32 GetInt32(string uniqueName);
        int GetInt(string uniqueName);
        Int64 GetInt64(string uniqueName);
        long GetLong(string uniqueName);
        float GetFloat(string uniqueName);
        double GetDouble(string uniqueName);
        string GetString(string uniqueName);
        byte[] GetByteArray(string uniqueName);
        bool GetBoolean(string uniqueName);

        bool DeleteInt16(string uniqueName);
        bool DeleteInt32(string uniqueName);
        bool DeleteInt(string uniqueName);
        bool DeleteInt64(string uniqueName);
        bool DeleteLong(string uniqueName);
        bool DeleteFloat(string uniqueName);
        bool DeleteDouble(string uniqueName);
        bool DeleteString(string uniqueName);
        bool DeleteByteArray(string uniqueName);
        bool DeleteBoolean(string uniqueName);
    }
}
