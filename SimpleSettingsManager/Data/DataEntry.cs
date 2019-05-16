using System;

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

        /// <summary>
        /// Get variable type
        /// </summary>
        /// <returns>A variables type</returns>
        public Type GetVariableType()
        {
            return _varType;
        }

        /// <summary>
        /// Get variable name
        /// </summary>
        /// <returns>A variables unique name</returns>
        public string GetVariableName()
        {
            return _varName;
        }

        /// <summary>
        /// Get variable group
        /// </summary>
        /// <returns>A variables group</returns>
        public string GetVariableGroup()
        {
            return _varGroup;
        }

        /// <summary>
        /// Get variable value
        /// </summary>
        /// <returns>A variables value</returns>
        public byte[] GetVariableValue()
        {
            return _varValue;
        }

        /// <summary>
        /// Get variable default value
        /// </summary>
        /// <returns>A variables default value</returns>
        public byte[] GetVariableDefault()
        {
            return _varDefault;
        }

        /// <summary>
        /// Get variable description
        /// </summary>
        /// <returns>A variables description</returns>
        public string GetVariableDescription()
        {
            return _varDesc;
        }
    }
}
