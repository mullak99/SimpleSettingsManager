using SimpleSettingsManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSettingsManager.Mode
{
    internal class XML : IMode
    {
        #region Init
        public void Open(string path)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Add Variables
        public bool AddBoolean(string uniqueName, bool value, string description, string group = "default")
        {
            throw new NotImplementedException();
        }

        public bool AddByteArray(string uniqueName, byte[] value, string description, string group = "default")
        {
            throw new NotImplementedException();
        }

        public bool AddDouble(string uniqueName, double value, string description, string group = "default")
        {
            throw new NotImplementedException();
        }

        public bool AddFloat(string uniqueName, float value, string description, string group = "default")
        {
            throw new NotImplementedException();
        }

        public bool AddInt(string uniqueName, int value, string description, string group = "default")
        {
            throw new NotImplementedException();
        }

        public bool AddInt16(string uniqueName, short value, string description, string group = "default")
        {
            throw new NotImplementedException();
        }

        public bool AddInt32(string uniqueName, int value, string description, string group = "default")
        {
            throw new NotImplementedException();
        }

        public bool AddInt64(string uniqueName, long value, string description, string group = "default")
        {
            throw new NotImplementedException();
        }

        public bool AddLong(string uniqueName, long value, string description, string group = "default")
        {
            throw new NotImplementedException();
        }

        public bool AddString(string uniqueName, string value, string description, string group = "default")
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Set Variables
        public bool SetBoolean(string uniqueName, bool value)
        {
            throw new NotImplementedException();
        }

        public bool SetByteArray(string uniqueName, byte[] value)
        {
            throw new NotImplementedException();
        }

        public bool SetDouble(string uniqueName, double value)
        {
            throw new NotImplementedException();
        }

        public bool SetFloat(string uniqueName, float value)
        {
            throw new NotImplementedException();
        }

        public bool SetInt(string uniqueName, int value)
        {
            throw new NotImplementedException();
        }

        public bool SetInt16(string uniqueName, short value)
        {
            throw new NotImplementedException();
        }

        public bool SetInt32(string uniqueName, int value)
        {
            throw new NotImplementedException();
        }

        public bool SetInt64(string uniqueName, long value)
        {
            throw new NotImplementedException();
        }

        public bool SetLong(string uniqueName, long value)
        {
            throw new NotImplementedException();
        }

        public bool SetString(string uniqueName, string value)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Edit Variables
        public bool EditBoolean(string uniqueName, string description, string group)
        {
            throw new NotImplementedException();
        }

        public bool EditByteArray(string uniqueName, string description, string group)
        {
            throw new NotImplementedException();
        }

        public bool EditDouble(string uniqueName, string description, string group)
        {
            throw new NotImplementedException();
        }

        public bool EditFloat(string uniqueName, string description, string group)
        {
            throw new NotImplementedException();
        }

        public bool EditInt(string uniqueName, string description, string group)
        {
            throw new NotImplementedException();
        }

        public bool EditInt16(string uniqueName, string description, string group)
        {
            throw new NotImplementedException();
        }

        public bool EditInt32(string uniqueName, string description, string group)
        {
            throw new NotImplementedException();
        }

        public bool EditInt64(string uniqueName, string description, string group)
        {
            throw new NotImplementedException();
        }

        public bool EditLong(string uniqueName, string description, string group)
        {
            throw new NotImplementedException();
        }

        public bool EditString(string uniqueName, string description, string group)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Get Variables
        public bool GetBoolean(string uniqueName)
        {
            throw new NotImplementedException();
        }

        public byte[] GetByteArray(string uniqueName)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(string uniqueName)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(string uniqueName)
        {
            throw new NotImplementedException();
        }

        public int GetInt(string uniqueName)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(string uniqueName)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(string uniqueName)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(string uniqueName)
        {
            throw new NotImplementedException();
        }

        public long GetLong(string uniqueName)
        {
            throw new NotImplementedException();
        }

        public string GetString(string uniqueName)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Delete Variables
        public bool DeleteBoolean(string uniqueName)
        {
            throw new NotImplementedException();
        }

        public bool DeleteByteArray(string uniqueName)
        {
            throw new NotImplementedException();
        }

        public bool DeleteDouble(string uniqueName)
        {
            throw new NotImplementedException();
        }

        public bool DeleteFloat(string uniqueName)
        {
            throw new NotImplementedException();
        }

        public bool DeleteInt(string uniqueName)
        {
            throw new NotImplementedException();
        }

        public bool DeleteInt16(string uniqueName)
        {
            throw new NotImplementedException();
        }

        public bool DeleteInt32(string uniqueName)
        {
            throw new NotImplementedException();
        }

        public bool DeleteInt64(string uniqueName)
        {
            throw new NotImplementedException();
        }

        public bool DeleteLong(string uniqueName)
        {
            throw new NotImplementedException();
        }

        public bool DeleteString(string uniqueName)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region MetaData
        public bool AddMetaData(string varName, string group, string varValue, string description)
        {
            throw new NotImplementedException();
        }

        public bool SetMetaData(string varName, string varValue)
        {
            throw new NotImplementedException();
        }

        public bool EditMetaData(string varName, string description, string group)
        {
            throw new NotImplementedException();
        }

        public bool DeleteMetaData(string varName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region DataEntry

        public void ImportDataEntry(DataEntry dataEntry)
        {
            throw new NotImplementedException();
        }

        public DataEntry[] GetAllInt16()
        {
            throw new NotImplementedException();
        }

        public DataEntry[] GetAllInt32()
        {
            throw new NotImplementedException();
        }

        public DataEntry[] GetAllInt()
        {
            return GetAllInt32();
        }

        public DataEntry[] GetAllInt64()
        {
            throw new NotImplementedException();
        }

        public DataEntry[] GetAllLong()
        {
            return GetAllInt64();
        }

        public DataEntry[] GetAllFloat()
        {
            throw new NotImplementedException();
        }

        public DataEntry[] GetAllDouble()
        {
            throw new NotImplementedException();
        }

        public DataEntry[] GetAllString()
        {
            throw new NotImplementedException();
        }

        public DataEntry[] GetAllByteArrays()
        {
            throw new NotImplementedException();
        }

        public DataEntry[] GetAllBooleans()
        {
            throw new NotImplementedException();
        }

        public DataEntry[] GetAllTypes()
        {
            List<DataEntry> dataList = new List<DataEntry>();

            if (this.GetAllInt16() != null) dataList.AddRange(this.GetAllInt16());
            if (this.GetAllInt32() != null) dataList.AddRange(this.GetAllInt32());
            if (this.GetAllInt64() != null) dataList.AddRange(this.GetAllInt64());
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
            return "XML";
        }

        #endregion

        #region Raw XML Handling

        #endregion
    }
}
