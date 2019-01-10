using SimpleSettingsManager.Data;
using System;

namespace SimpleSettingsManager.Mode
{
    internal interface IMode
    {
        void Open(SSM_File ssmFile);
        void Close();

        string GetMode();

        bool AddInt16(string uniqueName, Int16 value, string description, string group = "default");
        bool AddInt32(string uniqueName, Int32 value, string description, string group = "default");
        bool AddInt64(string uniqueName, Int64 value, string description, string group = "default");
        bool AddUInt16(string uniqueName, UInt16 value, string description, string group = "default");
        bool AddUInt32(string uniqueName, UInt32 value, string description, string group = "default");
        bool AddUInt64(string uniqueName, UInt64 value, string description, string group = "default");
        bool AddFloat(string uniqueName, float value, string description, string group = "default");
        bool AddDouble(string uniqueName, double value, string description, string group = "default");
        bool AddString(string uniqueName, string value, string description, string group = "default");
        bool AddByteArray(string uniqueName, byte[] value, string description, string group = "default");
        bool AddBoolean(string uniqueName, bool value, string description, string group = "default");

        bool SetInt16(string uniqueName, Int16 value);
        bool SetInt32(string uniqueName, Int32 value);
        bool SetInt64(string uniqueName, Int64 value);
        bool SetUInt16(string uniqueName, UInt16 value);
        bool SetUInt32(string uniqueName, UInt32 value);
        bool SetUInt64(string uniqueName, UInt64 value);
        bool SetFloat(string uniqueName, float value);
        bool SetDouble(string uniqueName, double value);
        bool SetString(string uniqueName, string value);
        bool SetByteArray(string uniqueName, byte[] value);
        bool SetBoolean(string uniqueName, bool value);

        bool EditInt16(string uniqueName, string description, string group);
        bool EditInt32(string uniqueName, string description, string group);
        bool EditInt64(string uniqueName, string description, string group);
        bool EditUInt16(string uniqueName, string description, string group);
        bool EditUInt32(string uniqueName, string description, string group);
        bool EditUInt64(string uniqueName, string description, string group);
        bool EditFloat(string uniqueName, string description, string group);
        bool EditDouble(string uniqueName, string description, string group);
        bool EditString(string uniqueName, string description, string group);
        bool EditByteArray(string uniqueName, string description, string group);
        bool EditBoolean(string uniqueName, string description, string group);

        Int16 GetInt16(string uniqueName);
        Int32 GetInt32(string uniqueName);
        Int64 GetInt64(string uniqueName);
        UInt16 GetUInt16(string uniqueName);
        UInt32 GetUInt32(string uniqueName);
        UInt64 GetUInt64(string uniqueName);
        float GetFloat(string uniqueName);
        double GetDouble(string uniqueName);
        string GetString(string uniqueName);
        byte[] GetByteArray(string uniqueName);
        bool GetBoolean(string uniqueName);

        bool DeleteInt16(string uniqueName);
        bool DeleteInt32(string uniqueName);
        bool DeleteInt64(string uniqueName);
        bool DeleteUInt16(string uniqueName);
        bool DeleteUInt32(string uniqueName);
        bool DeleteUInt64(string uniqueName);
        bool DeleteFloat(string uniqueName);
        bool DeleteDouble(string uniqueName);
        bool DeleteString(string uniqueName);
        bool DeleteByteArray(string uniqueName);
        bool DeleteBoolean(string uniqueName);

        DataEntry[] GetAllMetaData();
        DataEntry[] GetAllInt16();
        DataEntry[] GetAllInt32();
        DataEntry[] GetAllInt64();
        DataEntry[] GetAllUInt16();
        DataEntry[] GetAllUInt32();
        DataEntry[] GetAllUInt64();
        DataEntry[] GetAllFloat();
        DataEntry[] GetAllDouble();
        DataEntry[] GetAllString();
        DataEntry[] GetAllByteArrays();
        DataEntry[] GetAllBooleans();

        DataEntry[] GetAllTypes();

        void ImportDataEntry(DataEntry dataEntry);

        void UpdateMigrationStatus();
    }
}
