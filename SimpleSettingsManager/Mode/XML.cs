using SimpleSettingsManager.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SimpleSettingsManager.Mode
{
    internal class XML : IMode
    {
        private XmlDocument _xmlDoc = new XmlDocument();
        private XmlElement _xmlDocBody;
        private string _xmlDocContent;
        private XmlDeclaration _xmlDocDec;

        private bool _autoSave = true;

        private string _settingsPath;

        #region Init
        public void Open(SSM_File ssmFile)
        {
            _settingsPath = ssmFile.GetPath(true);

            bool isNewDatabase = CreateXmlFile();

            if (isNewDatabase) AddNewSsmMetaData();
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
            AddMetaData("SSM_CreationMode", "HistoricalInfo", "XML", "The SSM mode used to create the SSM file.");
            AddMetaData("SSM_LastAccessMode", "LastAccessInfo", "XML", "The SSM mode used to last access the SSM file.");
        }

        private void ExistingSsmMetaData()
        {
            SetMetaData("SSM_LastAccessFormatVersion", SSM.GetSsmFormatVersion());
            SetMetaData("SSM_LastLoadedTimestamp", Convert.ToString(IntUtilities.GetUnixTimestamp()));
            SetMetaData("SSM_LastAccessMode", "XML");
        }

        public void Close()
        {
            SaveXML();
        }

        public void UpdateCrossModeMigrationStatus()
        {
            LoadXML();

            if (!AddMetaData("SSM_LastMigration", "MigrationInfo", Convert.ToString(IntUtilities.GetUnixTimestamp()), "The timestamp of when the SSM file was last migrated."))
                SetMetaData("SSM_LastMigration", Convert.ToString(IntUtilities.GetUnixTimestamp()));

            if(!AddMetaData("SSM_MigrationCount", "MigrationInfo", "1", "The total number of migrations the SSM file has gone through."))
            {
                ulong totalMigrations = Convert.ToUInt64(GetRawXmlValue("SSM_MigrationCount", "_MetaData"));
                totalMigrations++;
                SetMetaData("SSM_MigrationCount", Convert.ToString(totalMigrations));
            }
            ExistingSsmMetaData();

            SaveXML();
        }

        public void UpdateXmlSettingsMigrationStatus()
        {
            LoadXML();

            if (!AddMetaData("SSM_XmlSettingsMigration", "MigrationInfo", Convert.ToString(IntUtilities.GetUnixTimestamp()), "The timestamp of when the XmlSettings file was migrated to SSM."))
                SetMetaData("SSM_XmlSettingsMigration", Convert.ToString(IntUtilities.GetUnixTimestamp()));

            if (!AddMetaData("SSM_XmlSettingsMigrationMode", "MigrationInfo", "XML", "The initial SSM mode the XmlSettings file was migrated to."))
                SetMetaData("SSM_XmlSettingsMigrationMode", "XML");

            ExistingSsmMetaData();

            SaveXML();
        }

        public void SetAutoSave(bool autoSave)
        {
            _autoSave = autoSave;
        }

        public bool GetAutoSave()
        {
            return _autoSave;
        }

        public void SaveXML()
        {
            _xmlDoc.Save(_settingsPath);
        }

        public void LoadXML()
        {
            try
            {
                _xmlDoc.Load(_settingsPath);
                _xmlDocContent = _xmlDoc.InnerXml;
                _xmlDocBody = _xmlDoc.DocumentElement;
            }
            catch (Exception e)
            {
                Logging.Log(String.Format("XML file could not be loaded!"), Severity.ERROR);
                throw new FileLoadException(String.Format("Loading XML Failed: {0}", e.ToString()));
            }
        }

        public void CleanXMLFile()
        {
            DeleteEmptyParents();
        }

        #endregion

        #region Add Variables

        public bool AddBoolean(string uniqueName, bool value, string description, string group = "default")
        {
            return AddBoolean(uniqueName, value, value, description, group);
        }

        public bool AddBoolean(string uniqueName, bool value, bool defaultValue, string description, string group = "default")
        {
            if (!DoesVariableExist(uniqueName, "Boolean"))
            {
                XmlNode headingNode = AddToHeadingNode("Boolean");
                _xmlDocBody.AppendChild(headingNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/Boolean/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    headingNode.AppendChild(groupNode);
                }
                else
                {
                    groupNode = _xmlDoc.SelectNodes(String.Format("//SSM/Boolean/{0}", group))[0];
                }

                XmlNode varNode = _xmlDoc.CreateElement(uniqueName);
                groupNode.AppendChild(varNode);

                XmlNode varValueNode = _xmlDoc.CreateElement("value");
                varValueNode.InnerText = value.ToString();

                XmlNode defaultValueNode = _xmlDoc.CreateElement("default");
                defaultValueNode.InnerText = defaultValue.ToString();

                XmlNode varDescNode = _xmlDoc.CreateElement("description");
                varDescNode.InnerText = description.ToString();

                varNode.AppendChild(varValueNode);
                varNode.AppendChild(defaultValueNode);
                varNode.AppendChild(varDescNode);

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool AddByteArray(string uniqueName, byte[] value, string description, string group = "default")
        {
            return AddByteArray(uniqueName, value, value, description, group);
        }

        public bool AddByteArray(string uniqueName, byte[] value, byte[] defaultValue, string description, string group = "default")
        {
            if (!DoesVariableExist(uniqueName, "ByteArray"))
            {
                XmlNode headingNode = AddToHeadingNode("ByteArray");
                _xmlDocBody.AppendChild(headingNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/ByteArray/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    headingNode.AppendChild(groupNode);
                }
                else
                {
                    groupNode = _xmlDoc.SelectNodes(String.Format("//SSM/ByteArray/{0}", group))[0];
                }

                XmlNode varNode = _xmlDoc.CreateElement(uniqueName);
                groupNode.AppendChild(varNode);

                XmlNode varValueNode = _xmlDoc.CreateElement("value");
                varValueNode.InnerText = Encoding.UTF8.GetString(value);

                XmlNode defaultValueNode = _xmlDoc.CreateElement("default");
                defaultValueNode.InnerText = Encoding.UTF8.GetString(defaultValue);

                XmlNode varDescNode = _xmlDoc.CreateElement("description");
                varDescNode.InnerText = description.ToString();

                varNode.AppendChild(varValueNode);
                varNode.AppendChild(defaultValueNode);
                varNode.AppendChild(varDescNode);

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool AddDouble(string uniqueName, double value, string description, string group = "default")
        {
            return AddDouble(uniqueName, value, value, description, group);
        }

        public bool AddDouble(string uniqueName, double value, double defaultValue, string description, string group = "default")
        {
            if (!DoesVariableExist(uniqueName, "Double"))
            {
                XmlNode headingNode = AddToHeadingNode("Double");
                _xmlDocBody.AppendChild(headingNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/Double/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    headingNode.AppendChild(groupNode);
                }
                else
                {
                    groupNode = _xmlDoc.SelectNodes(String.Format("//SSM/Double/{0}", group))[0];
                }

                XmlNode varNode = _xmlDoc.CreateElement(uniqueName);
                groupNode.AppendChild(varNode);

                XmlNode varValueNode = _xmlDoc.CreateElement("value");
                varValueNode.InnerText = value.ToString();

                XmlNode defaultValueNode = _xmlDoc.CreateElement("default");
                defaultValueNode.InnerText = defaultValue.ToString();

                XmlNode varDescNode = _xmlDoc.CreateElement("description");
                varDescNode.InnerText = description.ToString();

                varNode.AppendChild(varValueNode);
                varNode.AppendChild(defaultValueNode);
                varNode.AppendChild(varDescNode);

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool AddFloat(string uniqueName, float value, string description, string group = "default")
        {
            return AddFloat(uniqueName, value, value, description, group);
        }

        public bool AddFloat(string uniqueName, float value, float defaultValue, string description, string group = "default")
        {
            if (!DoesVariableExist(uniqueName, "Float"))
            {
                XmlNode headingNode = AddToHeadingNode("Float");
                _xmlDocBody.AppendChild(headingNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/Float/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    headingNode.AppendChild(groupNode);
                }
                else
                {
                    groupNode = _xmlDoc.SelectNodes(String.Format("//SSM/Float/{0}", group))[0];
                }

                XmlNode varNode = _xmlDoc.CreateElement(uniqueName);
                groupNode.AppendChild(varNode);

                XmlNode varValueNode = _xmlDoc.CreateElement("value");
                varValueNode.InnerText = value.ToString();

                XmlNode defaultValueNode = _xmlDoc.CreateElement("default");
                defaultValueNode.InnerText = defaultValue.ToString();

                XmlNode varDescNode = _xmlDoc.CreateElement("description");
                varDescNode.InnerText = description.ToString();

                varNode.AppendChild(varValueNode);
                varNode.AppendChild(defaultValueNode);
                varNode.AppendChild(varDescNode);

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool AddInt16(string uniqueName, short value, string description, string group = "default")
        {
            return AddInt16(uniqueName, value, value, description, group);
        }

        public bool AddInt16(string uniqueName, short value, short defaultValue, string description, string group = "default")
        {
            if (!DoesVariableExist(uniqueName, "Int16"))
            {
                XmlNode headingNode = AddToHeadingNode("Int16");
                _xmlDocBody.AppendChild(headingNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/Int16/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    headingNode.AppendChild(groupNode);
                }
                else
                {
                    groupNode = _xmlDoc.SelectNodes(String.Format("//SSM/Int16/{0}", group))[0];
                }

                XmlNode varNode = _xmlDoc.CreateElement(uniqueName);
                groupNode.AppendChild(varNode);

                XmlNode varValueNode = _xmlDoc.CreateElement("value");
                varValueNode.InnerText = value.ToString();

                XmlNode defaultValueNode = _xmlDoc.CreateElement("default");
                defaultValueNode.InnerText = defaultValue.ToString();

                XmlNode varDescNode = _xmlDoc.CreateElement("description");
                varDescNode.InnerText = description.ToString();

                varNode.AppendChild(varValueNode);
                varNode.AppendChild(defaultValueNode);
                varNode.AppendChild(varDescNode);

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool AddInt32(string uniqueName, int value, string description, string group = "default")
        {
            return AddInt32(uniqueName, value, value, description, group);
        }

        public bool AddInt32(string uniqueName, int value, int defaultValue, string description, string group = "default")
        {
            if (!DoesVariableExist(uniqueName, "Int32"))
            {
                XmlNode headingNode = AddToHeadingNode("Int32");
                _xmlDocBody.AppendChild(headingNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/Int32/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    headingNode.AppendChild(groupNode);
                }
                else
                {
                    groupNode = _xmlDoc.SelectNodes(String.Format("//SSM/Int32/{0}", group))[0];
                }

                XmlNode varNode = _xmlDoc.CreateElement(uniqueName);
                groupNode.AppendChild(varNode);

                XmlNode varValueNode = _xmlDoc.CreateElement("value");
                varValueNode.InnerText = value.ToString();

                XmlNode defaultValueNode = _xmlDoc.CreateElement("default");
                defaultValueNode.InnerText = defaultValue.ToString();

                XmlNode varDescNode = _xmlDoc.CreateElement("description");
                varDescNode.InnerText = description.ToString();

                varNode.AppendChild(varValueNode);
                varNode.AppendChild(defaultValueNode);
                varNode.AppendChild(varDescNode);

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool AddInt64(string uniqueName, long value, string description, string group = "default")
        {
            return AddInt64(uniqueName, value, value, description, group);
        }

        public bool AddInt64(string uniqueName, long value, long defaultValue, string description, string group = "default")
        {
            if (!DoesVariableExist(uniqueName, "Int64"))
            {
                XmlNode headingNode = AddToHeadingNode("Int64");
                _xmlDocBody.AppendChild(headingNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/Int64/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    headingNode.AppendChild(groupNode);
                }
                else
                {
                    groupNode = _xmlDoc.SelectNodes(String.Format("//SSM/Int64/{0}", group))[0];
                }

                XmlNode varNode = _xmlDoc.CreateElement(uniqueName);
                groupNode.AppendChild(varNode);

                XmlNode varValueNode = _xmlDoc.CreateElement("value");
                varValueNode.InnerText = value.ToString();

                XmlNode defaultValueNode = _xmlDoc.CreateElement("default");
                defaultValueNode.InnerText = defaultValue.ToString();

                XmlNode varDescNode = _xmlDoc.CreateElement("description");
                varDescNode.InnerText = description.ToString();

                varNode.AppendChild(varValueNode);
                varNode.AppendChild(defaultValueNode);
                varNode.AppendChild(varDescNode);

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool AddUInt16(string uniqueName, UInt16 value, string description, string group = "default")
        {
            return AddUInt16(uniqueName, value, value, description, group);
        }

        public bool AddUInt16(string uniqueName, UInt16 value, UInt16 defaultValue, string description, string group = "default")
        {
            if (!DoesVariableExist(uniqueName, "UInt16"))
            {
                XmlNode headingNode = AddToHeadingNode("UInt16");
                _xmlDocBody.AppendChild(headingNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/UInt16/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    headingNode.AppendChild(groupNode);
                }
                else
                {
                    groupNode = _xmlDoc.SelectNodes(String.Format("//SSM/UInt16/{0}", group))[0];
                }

                XmlNode varNode = _xmlDoc.CreateElement(uniqueName);
                groupNode.AppendChild(varNode);

                XmlNode varValueNode = _xmlDoc.CreateElement("value");
                varValueNode.InnerText = value.ToString();

                XmlNode defaultValueNode = _xmlDoc.CreateElement("default");
                defaultValueNode.InnerText = defaultValue.ToString();

                XmlNode varDescNode = _xmlDoc.CreateElement("description");
                varDescNode.InnerText = description.ToString();

                varNode.AppendChild(varValueNode);
                varNode.AppendChild(defaultValueNode);
                varNode.AppendChild(varDescNode);

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool AddUInt32(string uniqueName, UInt32 value, string description, string group = "default")
        {
            return AddUInt32(uniqueName, value, value, description, group);
        }

        public bool AddUInt32(string uniqueName, UInt32 value, UInt32 defaultValue, string description, string group = "default")
        {
            if (!DoesVariableExist(uniqueName, "UInt32"))
            {
                XmlNode headingNode = AddToHeadingNode("UInt32");
                _xmlDocBody.AppendChild(headingNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/UInt32/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    headingNode.AppendChild(groupNode);
                }
                else
                {
                    groupNode = _xmlDoc.SelectNodes(String.Format("//SSM/UInt32/{0}", group))[0];
                }

                XmlNode varNode = _xmlDoc.CreateElement(uniqueName);
                groupNode.AppendChild(varNode);

                XmlNode varValueNode = _xmlDoc.CreateElement("value");
                varValueNode.InnerText = value.ToString();

                XmlNode defaultValueNode = _xmlDoc.CreateElement("default");
                defaultValueNode.InnerText = defaultValue.ToString();

                XmlNode varDescNode = _xmlDoc.CreateElement("description");
                varDescNode.InnerText = description.ToString();

                varNode.AppendChild(varValueNode);
                varNode.AppendChild(defaultValueNode);
                varNode.AppendChild(varDescNode);

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool AddUInt64(string uniqueName, UInt64 value, string description, string group = "default")
        {
            return AddUInt64(uniqueName, value, value, description, group);
        }

        public bool AddUInt64(string uniqueName, UInt64 value, UInt64 defaultValue, string description, string group = "default")
        {
            if (!DoesVariableExist(uniqueName, "UInt64"))
            {
                XmlNode headingNode = AddToHeadingNode("UInt64");
                _xmlDocBody.AppendChild(headingNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/UInt64/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    headingNode.AppendChild(groupNode);
                }
                else
                {
                    groupNode = _xmlDoc.SelectNodes(String.Format("//SSM/UInt64/{0}", group))[0];
                }

                XmlNode varNode = _xmlDoc.CreateElement(uniqueName);
                groupNode.AppendChild(varNode);

                XmlNode varValueNode = _xmlDoc.CreateElement("value");
                varValueNode.InnerText = value.ToString();

                XmlNode defaultValueNode = _xmlDoc.CreateElement("default");
                defaultValueNode.InnerText = defaultValue.ToString();

                XmlNode varDescNode = _xmlDoc.CreateElement("description");
                varDescNode.InnerText = description.ToString();

                varNode.AppendChild(varValueNode);
                varNode.AppendChild(defaultValueNode);
                varNode.AppendChild(varDescNode);

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool AddString(string uniqueName, string value, string description, string group = "default")
        {
            return AddString(uniqueName, value, value, description, group);
        }

        public bool AddString(string uniqueName, string value, string defaultValue, string description, string group = "default")
        {
            if (!DoesVariableExist(uniqueName, "String"))
            {
                XmlNode headingNode = AddToHeadingNode("String");
                _xmlDocBody.AppendChild(headingNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/String/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    headingNode.AppendChild(groupNode);
                }
                else
                {
                    groupNode = _xmlDoc.SelectNodes(String.Format("//SSM/String/{0}", group))[0];
                }

                XmlNode varNode = _xmlDoc.CreateElement(uniqueName);
                groupNode.AppendChild(varNode);

                XmlNode varValueNode = _xmlDoc.CreateElement("value");
                varValueNode.InnerText = value.ToString();

                XmlNode defaultValueNode = _xmlDoc.CreateElement("default");
                defaultValueNode.InnerText = defaultValue.ToString();

                XmlNode varDescNode = _xmlDoc.CreateElement("description");
                varDescNode.InnerText = description.ToString();

                varNode.AppendChild(varValueNode);
                varNode.AppendChild(defaultValueNode);
                varNode.AppendChild(varDescNode);

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }
        #endregion
        #region Set Variables
        public bool SetBoolean(string uniqueName, bool value)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Boolean", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Boolean", uniqueName))[0];

                xmlNode.SelectSingleNode("value").InnerText = value.ToString();

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool SetByteArray(string uniqueName, byte[] value)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "ByteArray", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "ByteArray", uniqueName))[0];

                xmlNode.SelectSingleNode("value").InnerText = Encoding.UTF8.GetString(value);

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool SetDouble(string uniqueName, double value)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Double", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Double", uniqueName))[0];

                xmlNode.SelectSingleNode("value").InnerText = value.ToString();

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool SetFloat(string uniqueName, float value)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Float", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Float", uniqueName))[0];

                xmlNode.SelectSingleNode("value").InnerText = value.ToString();

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool SetInt16(string uniqueName, short value)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Int16", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Int16", uniqueName))[0];

                xmlNode.SelectSingleNode("value").InnerText = value.ToString();

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool SetInt32(string uniqueName, int value)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Int32", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Int32", uniqueName))[0];

                xmlNode.SelectSingleNode("value").InnerText = value.ToString();

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool SetInt64(string uniqueName, long value)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Int64", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Int64", uniqueName))[0];

                xmlNode.SelectSingleNode("value").InnerText = value.ToString();

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool SetUInt16(string uniqueName, UInt16 value)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "UInt16", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "UInt16", uniqueName))[0];

                xmlNode.SelectSingleNode("value").InnerText = value.ToString();

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool SetUInt32(string uniqueName, UInt32 value)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "UInt32", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "UInt32", uniqueName))[0];

                xmlNode.SelectSingleNode("value").InnerText = value.ToString();

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool SetUInt64(string uniqueName, UInt64 value)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "UInt64", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "UInt64", uniqueName))[0];

                xmlNode.SelectSingleNode("value").InnerText = value.ToString();

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool SetString(string uniqueName, string value)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "String", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "String", uniqueName))[0];

                xmlNode.SelectSingleNode("value").InnerText = value.ToString();

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }
        #endregion
        #region Edit Variables
        public bool EditBoolean(string uniqueName, string description, string group)
        {
            if(DoesVariableExist(uniqueName, "Boolean"))
            {
                EditVarDesc(uniqueName, description, "Boolean");

                if (GetVarGroup(uniqueName, "Boolean") != group)
                    MoveVarGroup(uniqueName, group, "Boolean");

                return true;
            }
            return false;
        }

        public bool EditByteArray(string uniqueName, string description, string group)
        {
            if (DoesVariableExist(uniqueName, "ByteArray"))
            {
                EditVarDesc(uniqueName, description, "ByteArray");

                if (GetVarGroup(uniqueName, "ByteArray") != group)
                    MoveVarGroup(uniqueName, group, "ByteArray");

                return true;
            }
            return false;
        }

        public bool EditDouble(string uniqueName, string description, string group)
        {
            if (DoesVariableExist(uniqueName, "Double"))
            {
                EditVarDesc(uniqueName, description, "Double");

                if (GetVarGroup(uniqueName, "Double") != group)
                    MoveVarGroup(uniqueName, group, "Double");

                return true;
            }
            return false;
        }

        public bool EditFloat(string uniqueName, string description, string group)
        {
            if (DoesVariableExist(uniqueName, "Float"))
            {
                EditVarDesc(uniqueName, description, "Float");

                if (GetVarGroup(uniqueName, "Float") != group)
                    MoveVarGroup(uniqueName, group, "Float");

                return true;
            }
            return false;
        }

        public bool EditInt16(string uniqueName, string description, string group)
        {
            if (DoesVariableExist(uniqueName, "Int16"))
            {
                EditVarDesc(uniqueName, description, "Int16");

                if (GetVarGroup(uniqueName, "Int16") != group)
                    MoveVarGroup(uniqueName, group, "Int16");

                return true;
            }
            return false;
        }

        public bool EditInt32(string uniqueName, string description, string group)
        {
            if (DoesVariableExist(uniqueName, "Int32"))
            {
                EditVarDesc(uniqueName, description, "Int32");

                if (GetVarGroup(uniqueName, "Int32") != group)
                    MoveVarGroup(uniqueName, group, "Int32");

                return true;
            }
            return false;
        }

        public bool EditInt64(string uniqueName, string description, string group)
        {
            if (DoesVariableExist(uniqueName, "Int64"))
            {
                EditVarDesc(uniqueName, description, "Int64");

                if (GetVarGroup(uniqueName, "Int64") != group)
                    MoveVarGroup(uniqueName, group, "Int64");

                return true;
            }
            return false;
        }

        public bool EditUInt16(string uniqueName, string description, string group)
        {
            if (DoesVariableExist(uniqueName, "UInt16"))
            {
                EditVarDesc(uniqueName, description, "UInt16");

                if (GetVarGroup(uniqueName, "UInt16") != group)
                    MoveVarGroup(uniqueName, group, "UInt16");

                return true;
            }
            return false;
        }

        public bool EditUInt32(string uniqueName, string description, string group)
        {
            if (DoesVariableExist(uniqueName, "UInt32"))
            {
                EditVarDesc(uniqueName, description, "UInt32");

                if (GetVarGroup(uniqueName, "UInt32") != group)
                    MoveVarGroup(uniqueName, group, "UInt32");

                return true;
            }
            return false;
        }

        public bool EditUInt64(string uniqueName, string description, string group)
        {
            if (DoesVariableExist(uniqueName, "UInt64"))
            {
                EditVarDesc(uniqueName, description, "UInt64");

                if (GetVarGroup(uniqueName, "UInt64") != group)
                    MoveVarGroup(uniqueName, group, "UInt64");

                return true;
            }
            return false;
        }

        public bool EditString(string uniqueName, string description, string group)
        {
            if (DoesVariableExist(uniqueName, "String"))
            {
                EditVarDesc(uniqueName, description, "String");

                if (GetVarGroup(uniqueName, "String") != group)
                    MoveVarGroup(uniqueName, group, "String");

                return true;
            }
            return false;
        }
        #endregion
        #region Get Variables
        public bool GetBoolean(string uniqueName)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Boolean", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Boolean", uniqueName))[0];

                return Convert.ToBoolean(xmlNode.SelectSingleNode("value").InnerText);
            }
            return false;
        }

        public byte[] GetByteArray(string uniqueName)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "ByteArray", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "ByteArray", uniqueName))[0];

                return Encoding.UTF8.GetBytes(xmlNode.SelectSingleNode("value").InnerText);
            }
            return null;
        }

        public double GetDouble(string uniqueName)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Double", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Double", uniqueName))[0];

                return Convert.ToDouble(xmlNode.SelectSingleNode("value").InnerText);
            }
            return 0;
        }

        public float GetFloat(string uniqueName)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Float", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Float", uniqueName))[0];

                return Convert.ToSingle(xmlNode.SelectSingleNode("value").InnerText);
            }
            return 0F;
        }

        public short GetInt16(string uniqueName)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Int16", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Int16", uniqueName))[0];

                return Convert.ToInt16(xmlNode.SelectSingleNode("value").InnerText);
            }
            return 0;
        }

        public int GetInt32(string uniqueName)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Int32", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Int32", uniqueName))[0];

                return Convert.ToInt32(xmlNode.SelectSingleNode("value").InnerText);
            }
            return 0;
        }

        public long GetInt64(string uniqueName)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Int64", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "Int64", uniqueName))[0];

                return Convert.ToInt64(xmlNode.SelectSingleNode("value").InnerText);
            }
            return 0;
        }

        public UInt16 GetUInt16(string uniqueName)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "UInt16", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "UInt16", uniqueName))[0];

                return Convert.ToUInt16(xmlNode.SelectSingleNode("value").InnerText);
            }
            return 0;
        }

        public UInt32 GetUInt32(string uniqueName)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "UInt32", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "UInt32", uniqueName))[0];

                return Convert.ToUInt32(xmlNode.SelectSingleNode("value").InnerText);
            }
            return 0;
        }

        public UInt64 GetUInt64(string uniqueName)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "UInt64", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "UInt64", uniqueName))[0];

                return Convert.ToUInt64(xmlNode.SelectSingleNode("value").InnerText);
            }
            return 0;
        }

        public string GetString(string uniqueName)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "String", uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", "String", uniqueName))[0];

                return xmlNode.SelectSingleNode("value").InnerText;
            }
            return null;
        }
        #endregion
        #region Delete Variables
        public bool DeleteBoolean(string uniqueName)
        {
            return RemoveXmlElement(uniqueName, "Boolean");
        }

        public bool DeleteByteArray(string uniqueName)
        {
            return RemoveXmlElement(uniqueName, "ByteArray");
        }

        public bool DeleteDouble(string uniqueName)
        {
            return RemoveXmlElement(uniqueName, "Double");
        }

        public bool DeleteFloat(string uniqueName)
        {
            return RemoveXmlElement(uniqueName, "Float");
        }

        public bool DeleteInt16(string uniqueName)
        {
            return RemoveXmlElement(uniqueName, "Int16");
        }

        public bool DeleteInt32(string uniqueName)
        {
            return RemoveXmlElement(uniqueName, "Int32");
        }

        public bool DeleteInt64(string uniqueName)
        {
            return RemoveXmlElement(uniqueName, "Int64");
        }

        public bool DeleteUInt16(string uniqueName)
        {
            return RemoveXmlElement(uniqueName, "UInt16");
        }

        public bool DeleteUInt32(string uniqueName)
        {
            return RemoveXmlElement(uniqueName, "UInt32");
        }

        public bool DeleteUInt64(string uniqueName)
        {
            return RemoveXmlElement(uniqueName, "UInt64");
        }

        public bool DeleteString(string uniqueName)
        {
            return RemoveXmlElement(uniqueName, "String");
        }
        #endregion
        #region MetaData
        public bool AddMetaData(string varName, string group, string varValue, string description)
        {
            if (!DoesMetaDataVariableExist(varName))
            {
                XmlNode headingNode = AddToHeadingNode("_MetaData");
                _xmlDocBody.AppendChild(headingNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/_MetaData/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    headingNode.AppendChild(groupNode);
                }
                else
                {
                    groupNode = _xmlDoc.SelectNodes(String.Format("//SSM/_MetaData/{0}", group))[0];
                }

                XmlNode varNode = _xmlDoc.CreateElement(varName);
                groupNode.AppendChild(varNode);

                XmlNode varValueNode = _xmlDoc.CreateElement("value");
                varValueNode.InnerText = varValue.ToString();

                XmlNode varDescNode = _xmlDoc.CreateElement("description");
                varDescNode.InnerText = description.ToString();

                varNode.AppendChild(varValueNode);
                varNode.AppendChild(varDescNode);

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool SetMetaData(string varName, string varValue)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/_MetaData/*/{0}", varName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/_MetaData/*/{0}", varName))[0];

                xmlNode.SelectSingleNode("value").InnerText = varValue.ToString();

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        public bool EditMetaData(string varName, string description, string group)
        {
            if (DoesVariableExist(varName, "_MetaData"))
            {
                EditVarDesc(varName, description, "_MetaData");

                if (GetVarGroup(varName, "_MetaData") != group)
                    MoveVarGroup(varName, group, "_MetaData");

                return true;
            }
            return false;
        }

        public bool DeleteMetaData(string varName)
        {
            return RemoveXmlElement(varName, "_MetaData");
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
                    AddInt16(dataEntry.GetVariableName(), BitConverter.ToInt16(dataEntry.GetVariableValue(), 0), BitConverter.ToInt16(dataEntry.GetVariableDefault(), 0), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetInt16(dataEntry.GetVariableName(), BitConverter.ToInt16(dataEntry.GetVariableValue(), 0));
                    EditInt16(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefault(dataEntry.GetVariableName(), Convert.ToString(BitConverter.ToInt16(dataEntry.GetVariableDefault(), 0)), "Int16");
            }
            else if (dataEntry.GetVariableType() == typeof(Int32))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "Int32"))
                {
                    AddInt32(dataEntry.GetVariableName(), BitConverter.ToInt32(dataEntry.GetVariableValue(), 0), BitConverter.ToInt32(dataEntry.GetVariableDefault(), 0), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetInt32(dataEntry.GetVariableName(), BitConverter.ToInt32(dataEntry.GetVariableValue(), 0));
                    EditInt32(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefault(dataEntry.GetVariableName(), Convert.ToString(BitConverter.ToInt32(dataEntry.GetVariableDefault(), 0)), "Int32");
            }
            else if (dataEntry.GetVariableType() == typeof(Int64))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "Int64"))
                {
                    AddInt64(dataEntry.GetVariableName(), BitConverter.ToInt64(dataEntry.GetVariableValue(), 0), BitConverter.ToInt64(dataEntry.GetVariableDefault(), 0), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetInt64(dataEntry.GetVariableName(), BitConverter.ToInt64(dataEntry.GetVariableValue(), 0));
                    EditInt64(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefault(dataEntry.GetVariableName(), Convert.ToString(BitConverter.ToInt64(dataEntry.GetVariableDefault(), 0)), "Int64");
            }
            else if (dataEntry.GetVariableType() == typeof(UInt16))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "UInt16"))
                {
                    AddUInt16(dataEntry.GetVariableName(), BitConverter.ToUInt16(dataEntry.GetVariableValue(), 0), BitConverter.ToUInt16(dataEntry.GetVariableDefault(), 0), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetUInt16(dataEntry.GetVariableName(), BitConverter.ToUInt16(dataEntry.GetVariableValue(), 0));
                    EditUInt16(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefault(dataEntry.GetVariableName(), Convert.ToString(BitConverter.ToUInt16(dataEntry.GetVariableDefault(), 0)), "UInt16");
            }
            else if (dataEntry.GetVariableType() == typeof(UInt32))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "UInt32"))
                {
                    AddUInt32(dataEntry.GetVariableName(), BitConverter.ToUInt32(dataEntry.GetVariableValue(), 0), BitConverter.ToUInt32(dataEntry.GetVariableDefault(), 0), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetUInt32(dataEntry.GetVariableName(), BitConverter.ToUInt32(dataEntry.GetVariableValue(), 0));
                    EditUInt32(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefault(dataEntry.GetVariableName(), Convert.ToString(BitConverter.ToUInt32(dataEntry.GetVariableDefault(), 0)), "UInt32");
            }
            else if (dataEntry.GetVariableType() == typeof(UInt64))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "UInt64"))
                {
                    AddUInt64(dataEntry.GetVariableName(), BitConverter.ToUInt64(dataEntry.GetVariableValue(), 0), BitConverter.ToUInt64(dataEntry.GetVariableDefault(), 0), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetUInt64(dataEntry.GetVariableName(), BitConverter.ToUInt64(dataEntry.GetVariableValue(), 0));
                    EditUInt64(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefault(dataEntry.GetVariableName(), Convert.ToString(BitConverter.ToUInt64(dataEntry.GetVariableDefault(), 0)), "UInt64");
            }
            else if (dataEntry.GetVariableType() == typeof(float))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "Float"))
                {
                    AddFloat(dataEntry.GetVariableName(), BitConverter.ToSingle(dataEntry.GetVariableValue(), 0), BitConverter.ToSingle(dataEntry.GetVariableDefault(), 0), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetFloat(dataEntry.GetVariableName(), BitConverter.ToSingle(dataEntry.GetVariableValue(), 0));
                    EditFloat(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefault(dataEntry.GetVariableName(), Convert.ToString(BitConverter.ToSingle(dataEntry.GetVariableDefault(), 0)), "Float");
            }
            else if (dataEntry.GetVariableType() == typeof(Double))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "Double"))
                {
                    AddDouble(dataEntry.GetVariableName(), BitConverter.ToDouble(dataEntry.GetVariableValue(), 0), BitConverter.ToDouble(dataEntry.GetVariableDefault(), 0), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetDouble(dataEntry.GetVariableName(), BitConverter.ToDouble(dataEntry.GetVariableValue(), 0));
                    EditDouble(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefault(dataEntry.GetVariableName(), Convert.ToString(BitConverter.ToDouble(dataEntry.GetVariableDefault(), 0)), "Double");
            }
            else if (dataEntry.GetVariableType() == typeof(String))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "String"))
                {
                    AddString(dataEntry.GetVariableName(), Encoding.UTF8.GetString(dataEntry.GetVariableValue()), Encoding.UTF8.GetString(dataEntry.GetVariableDefault()), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetString(dataEntry.GetVariableName(), Encoding.UTF8.GetString(dataEntry.GetVariableValue()));
                    EditString(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefault(dataEntry.GetVariableName(), Encoding.UTF8.GetString(dataEntry.GetVariableDefault()), "String");
            }
            else if (dataEntry.GetVariableType() == typeof(byte[]))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "ByteArray"))
                {
                    AddByteArray(dataEntry.GetVariableName(), dataEntry.GetVariableValue(), dataEntry.GetVariableDefault(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetByteArray(dataEntry.GetVariableName(), dataEntry.GetVariableValue());
                    EditByteArray(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefault(dataEntry.GetVariableName(), Encoding.UTF8.GetString(dataEntry.GetVariableDefault()), "ByteArray");
            }
            else if (dataEntry.GetVariableType() == typeof(Boolean))
            {
                if (!DoesVariableExist(dataEntry.GetVariableName(), "Boolean"))
                {
                    AddBoolean(dataEntry.GetVariableName(), BitConverter.ToBoolean(dataEntry.GetVariableValue(), 0), BitConverter.ToBoolean(dataEntry.GetVariableDefault(), 0), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                else
                {
                    SetBoolean(dataEntry.GetVariableName(), BitConverter.ToBoolean(dataEntry.GetVariableValue(), 0));
                    EditBoolean(dataEntry.GetVariableName(), dataEntry.GetVariableDescription(), dataEntry.GetVariableGroup());
                }
                EditVarDefault(dataEntry.GetVariableName(), Convert.ToString(BitConverter.ToBoolean(dataEntry.GetVariableDefault(), 0)), "Boolean");
            }
        }

        public DataEntry[] GetAllMetaData()
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "_MetaData")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "_MetaData"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(MetaDataObject), xmlNodeGroup.ChildNodes[i].Name, xmlNodeGroup.Name, Encoding.UTF8.GetBytes(xmlNodeGroup.ChildNodes[i].SelectSingleNode("value").InnerText), Encoding.UTF8.GetBytes(xmlNodeGroup.ChildNodes[i].SelectSingleNode("value").InnerText), xmlNodeGroup.ChildNodes[i].SelectSingleNode("description").InnerText));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllInt16()
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "Int16")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "Int16"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(Int16), xmlNodeGroup.ChildNodes[i].Name, xmlNodeGroup.Name, BitConverter.GetBytes(Convert.ToInt16(xmlNodeGroup.ChildNodes[i].SelectSingleNode("value").InnerText)), BitConverter.GetBytes(Convert.ToInt16(xmlNodeGroup.ChildNodes[i].SelectSingleNode("default").InnerText)), xmlNodeGroup.ChildNodes[i].SelectSingleNode("description").InnerText));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllInt32()
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "Int32")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "Int32"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(Int32), xmlNodeGroup.ChildNodes[i].Name, xmlNodeGroup.Name, BitConverter.GetBytes(Convert.ToInt32(xmlNodeGroup.ChildNodes[i].SelectSingleNode("value").InnerText)), BitConverter.GetBytes(Convert.ToInt32(xmlNodeGroup.ChildNodes[i].SelectSingleNode("default").InnerText)), xmlNodeGroup.ChildNodes[i].SelectSingleNode("description").InnerText));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllInt64()
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "Int64")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "Int64"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(Int64), xmlNodeGroup.ChildNodes[i].Name, xmlNodeGroup.Name, BitConverter.GetBytes(Convert.ToInt64(xmlNodeGroup.ChildNodes[i].SelectSingleNode("value").InnerText)), BitConverter.GetBytes(Convert.ToInt64(xmlNodeGroup.ChildNodes[i].SelectSingleNode("default").InnerText)), xmlNodeGroup.ChildNodes[i].SelectSingleNode("description").InnerText));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllUInt16()
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "UInt16")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "UInt16"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(UInt16), xmlNodeGroup.ChildNodes[i].Name, xmlNodeGroup.Name, BitConverter.GetBytes(Convert.ToUInt16(xmlNodeGroup.ChildNodes[i].SelectSingleNode("value").InnerText)), BitConverter.GetBytes(Convert.ToUInt16(xmlNodeGroup.ChildNodes[i].SelectSingleNode("default").InnerText)), xmlNodeGroup.ChildNodes[i].SelectSingleNode("description").InnerText));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllUInt32()
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "UInt32")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "UInt32"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(UInt32), xmlNodeGroup.ChildNodes[i].Name, xmlNodeGroup.Name, BitConverter.GetBytes(Convert.ToUInt32(xmlNodeGroup.ChildNodes[i].SelectSingleNode("value").InnerText)), BitConverter.GetBytes(Convert.ToUInt32(xmlNodeGroup.ChildNodes[i].SelectSingleNode("default").InnerText)), xmlNodeGroup.ChildNodes[i].SelectSingleNode("description").InnerText));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllUInt64()
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "UInt64")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "UInt64"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(UInt64), xmlNodeGroup.ChildNodes[i].Name, xmlNodeGroup.Name, BitConverter.GetBytes(Convert.ToUInt64(xmlNodeGroup.ChildNodes[i].SelectSingleNode("value").InnerText)), BitConverter.GetBytes(Convert.ToUInt64(xmlNodeGroup.ChildNodes[i].SelectSingleNode("default").InnerText)), xmlNodeGroup.ChildNodes[i].SelectSingleNode("description").InnerText));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllFloat()
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "Float")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "Float"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(float), xmlNodeGroup.ChildNodes[i].Name, xmlNodeGroup.Name, BitConverter.GetBytes(Convert.ToSingle(xmlNodeGroup.ChildNodes[i].SelectSingleNode("value").InnerText)), BitConverter.GetBytes(Convert.ToSingle(xmlNodeGroup.ChildNodes[i].SelectSingleNode("default").InnerText)), xmlNodeGroup.ChildNodes[i].SelectSingleNode("description").InnerText));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllDouble()
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "Double")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "Double"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(Double), xmlNodeGroup.ChildNodes[i].Name, xmlNodeGroup.Name, BitConverter.GetBytes(Convert.ToDouble(xmlNodeGroup.ChildNodes[i].SelectSingleNode("value").InnerText)), BitConverter.GetBytes(Convert.ToDouble(xmlNodeGroup.ChildNodes[i].SelectSingleNode("default").InnerText)), xmlNodeGroup.ChildNodes[i].SelectSingleNode("description").InnerText));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllString()
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "String")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "String"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(String), xmlNodeGroup.ChildNodes[i].Name, xmlNodeGroup.Name, Encoding.UTF8.GetBytes(xmlNodeGroup.ChildNodes[i].SelectSingleNode("value").InnerText), Encoding.UTF8.GetBytes(xmlNodeGroup.ChildNodes[i].SelectSingleNode("default").InnerText), xmlNodeGroup.ChildNodes[i].SelectSingleNode("description").InnerText));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllByteArrays()
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "ByteArray")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "ByteArray"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(byte[]), xmlNodeGroup.ChildNodes[i].Name, xmlNodeGroup.Name, Encoding.UTF8.GetBytes(xmlNodeGroup.ChildNodes[i].SelectSingleNode("value").InnerText), Encoding.UTF8.GetBytes(xmlNodeGroup.ChildNodes[i].SelectSingleNode("default").InnerText), xmlNodeGroup.ChildNodes[i].SelectSingleNode("description").InnerText));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllBooleans()
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "Boolean")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*", "Boolean"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(Boolean), xmlNodeGroup.ChildNodes[i].Name, xmlNodeGroup.Name, BitConverter.GetBytes(Convert.ToBoolean(xmlNodeGroup.ChildNodes[i].SelectSingleNode("value").InnerText)), BitConverter.GetBytes(Convert.ToBoolean(xmlNodeGroup.ChildNodes[i].SelectSingleNode("default").InnerText)), xmlNodeGroup.ChildNodes[i].SelectSingleNode("description").InnerText));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        public DataEntry[] GetAllTypes()
        {
            List<DataEntry> dataList = new List<DataEntry>();

            List<Task> allTypesTasks = new List<Task>
            {
                Task.Factory.StartNew(() => { try { if (this.GetAllMetaData() != null) dataList.AddRange(this.GetAllMetaData()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllInt16() != null) dataList.AddRange(this.GetAllInt16()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllInt32() != null) dataList.AddRange(this.GetAllInt32()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllInt64() != null) dataList.AddRange(this.GetAllInt64()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllUInt16() != null) dataList.AddRange(this.GetAllUInt16()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllUInt32() != null) dataList.AddRange(this.GetAllUInt32()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllUInt64() != null) dataList.AddRange(this.GetAllUInt64()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllFloat() != null) dataList.AddRange(this.GetAllFloat()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllDouble() != null) dataList.AddRange(this.GetAllDouble()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllString() != null) dataList.AddRange(this.GetAllString()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllByteArrays() != null) dataList.AddRange(this.GetAllByteArrays()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllBooleans() != null) dataList.AddRange(this.GetAllBooleans()); } catch { } })
            };
            Task.WaitAll(allTypesTasks.ToArray());

            dataList.RemoveAll(item => item == null);
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

        private bool CreateXmlFile()
        {
            bool r = false;
            if (!File.Exists(_settingsPath))
            {
                XmlDeclaration xmlDeclaration = _xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = _xmlDoc.DocumentElement;

                _xmlDoc.InsertBefore(xmlDeclaration, root);

                _xmlDocBody = _xmlDoc.CreateElement(string.Empty, "SSM", string.Empty);
                _xmlDoc.AppendChild(_xmlDocBody);

                SaveXML();

                r = true;
            }

            LoadXML();
            LoadXmlDeclaration();
            CleanXMLFile();

            if (_autoSave) SaveXML();           

            return r;
        }

        private bool DoesVariableExist(string varName, string type)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", type, varName)).Count > 0)
                return true;

            return false;
        }

        private bool EditVarDefault(string uniqueName, string defaultValue, string type)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", type, uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", type, uniqueName))[0];

                xmlNode.SelectSingleNode("default").InnerText = defaultValue.ToString();

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        private bool EditVarDesc(string uniqueName, string description, string type)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", type, uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", type, uniqueName))[0];

                xmlNode.SelectSingleNode("description").InnerText = description.ToString();

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        private string GetVarGroup(string uniqueName, string type)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", type, uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", type, uniqueName))[0];

                return xmlNode.ParentNode.InnerText;
            }
            return null;
        }

        private bool MoveVarGroup(string uniqueName, string group, string type)
        {
            if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", type, uniqueName)).Count == 1)
            {
                XmlNode xmlNode = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", type, uniqueName))[0];
                DataEntry de;

                string value = xmlNode.SelectSingleNode("value").InnerText;
                string desc = xmlNode.SelectSingleNode("description").InnerText;

                if (type != "_MetaData")
                {
                    string defVal = xmlNode.SelectSingleNode("default").InnerText;

                    if (type == "Int16")
                        de = new DataEntry(typeof(Int16), uniqueName, group, Encoding.UTF8.GetBytes(value), Encoding.UTF8.GetBytes(defVal), desc);
                    else if (type == "Int32")
                        de = new DataEntry(typeof(Int32), uniqueName, group, Encoding.UTF8.GetBytes(value), Encoding.UTF8.GetBytes(defVal), desc);
                    else if (type == "Int64")
                        de = new DataEntry(typeof(Int64), uniqueName, group, Encoding.UTF8.GetBytes(value), Encoding.UTF8.GetBytes(defVal), desc);
                    else if (type == "Float")
                        de = new DataEntry(typeof(float), uniqueName, group, Encoding.UTF8.GetBytes(value), Encoding.UTF8.GetBytes(defVal), desc);
                    else if (type == "Double")
                        de = new DataEntry(typeof(Double), uniqueName, group, Encoding.UTF8.GetBytes(value), Encoding.UTF8.GetBytes(defVal), desc);
                    else if (type == "String")
                        de = new DataEntry(typeof(String), uniqueName, group, Encoding.UTF8.GetBytes(value), Encoding.UTF8.GetBytes(defVal), desc);
                    else if (type == "ByteArray")
                        de = new DataEntry(typeof(byte[]), uniqueName, group, Encoding.UTF8.GetBytes(value), Encoding.UTF8.GetBytes(defVal), desc);
                    else if (type == "Boolean")
                        de = new DataEntry(typeof(Boolean), uniqueName, group, Encoding.UTF8.GetBytes(value), Encoding.UTF8.GetBytes(defVal), desc);
                    else
                    {
                        Logging.Log(String.Format("Type '{0}' is not supported!", type), Severity.ERROR);
                        throw new NotSupportedException(String.Format("Unknown type '{0}'", type));
                    }   
                }
                else
                    de = new DataEntry(typeof(MetaDataObject), uniqueName, group, Encoding.UTF8.GetBytes(value), Encoding.UTF8.GetBytes(value), desc);

                RemoveXmlElement(uniqueName, type);

                ImportDataEntry(de);

                if (_autoSave) SaveXML();

                return true;
            }
            return false;
        }

        private bool DoesMetaDataVariableExist(string varName)
        {
            return DoesVariableExist(varName, "_MetaData");
        }

        private XmlNode AddToHeadingNode(string headingName)
        {
            XmlNode node;

            if (_xmlDoc.DocumentElement[headingName] != null)
                node = _xmlDoc.DocumentElement[headingName];
            else
            {
                node = _xmlDoc.CreateElement(headingName);
                _xmlDocBody.AppendChild(node);
            }
            return node;
        }

        private bool RemoveXmlElement(string varName, string type)
        {
            try
            {
                if (_autoSave) LoadXML();

                if (_xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", type, varName)).Count > 0)
                {
                    XmlNode node = _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", type, varName))[0];

                    node.ParentNode.RemoveAll();
                    if (_autoSave) SaveXML();
                    DeleteEmptyParents();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private string GetRawXmlValue(string varName, string type)
        {
            if (DoesVariableExist(varName, type))
            {
                return _xmlDoc.SelectNodes(String.Format("//SSM/{0}/*/{1}", type, varName))[0].SelectNodes("value")[0].InnerText;
            }
            return null;
        }

        private void DeleteEmptyParents()
        {
            if (_autoSave) LoadXML();

            for (int i = 0; i < _xmlDoc.ChildNodes.Count; i++)
            {
                for (int n = 0; n < _xmlDoc.ChildNodes[i].ChildNodes.Count; n++)
                {
                    XmlNode node = _xmlDoc.ChildNodes[i].ChildNodes[n];
                    if (node.ChildNodes.Count == 0)
                    {
                        node.ParentNode.RemoveChild(node);
                    }
                }
            }
            if (_autoSave) SaveXML();
        }

        private void LoadXmlDeclaration()
        {
            try
            {
                if (_xmlDoc.ChildNodes[0].NodeType == XmlNodeType.XmlDeclaration)
                {
                    _xmlDocDec = _xmlDoc.ChildNodes[0] as XmlDeclaration;
                }
            }
            catch (Exception e)
            {
                Logging.Log(String.Format("XML Declaration is invalid!"), Severity.ERROR);
                throw new FileLoadException(String.Format("XML Declaration Error: {0}", e.ToString()));
            }
        }

        #endregion
    }
}
