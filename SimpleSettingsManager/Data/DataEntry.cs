using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSettingsManager.Data
{
    public class DataEntry
    {
        private Type _varType;
        private string _varName, _varGroup, _varDesc;
        private byte[] _varValue, _varDefault;

        /// <summary>
        /// [PotentiallyDestructive] Manual creation of DataEntries are not recommended since it could potentially break the database/file they are stored in.
        /// </summary>
        /// <param name="VariableType">The variables type</param>
        /// <param name="VariableName">The variables name</param>
        /// <param name="VariableGroup">The variables group</param>
        /// <param name="VariableValue">The variables value</param>
        /// <param name="VariableDefault">The variables default/original value</param>
        /// <param name="VariableDescription">The variables description</param>
        public DataEntry(Type VariableType, string VariableName, string VariableGroup, byte[] VariableValue, byte[] VariableDefault, string VariableDescription)
        {
            _varType = VariableType;
            _varName = VariableName;
            _varGroup = VariableGroup;
            _varValue = VariableValue;
            _varDefault = VariableDefault;
            _varDesc = VariableDescription;
        }

        public Type GetVariableType()
        {
            return _varType;
        }

        public string GetVariableName()
        {
            return _varName;
        }

        public string GetVariableGroup()
        {
            return _varGroup;
        }

        public byte[] GetVariableValue()
        {
            return _varValue;
        }

        public byte[] GetVariableDefault()
        {
            return _varDefault;
        }

        public string GetVariableDescription()
        {
            return _varDesc;
        }
    }
}
