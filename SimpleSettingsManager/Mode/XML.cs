using SimpleSettingsManager.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            if (isNewDatabase)
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
            else
            {
                SetMetaData("SSM_LastAccessFormatVersion", SSM.GetSsmFormatVersion());
                SetMetaData("SSM_LastLoadedTimestamp", Convert.ToString(IntUtilities.GetUnixTimestamp()));
                SetMetaData("SSM_LastAccessMode", "XML");
            }
        }

        public void Close()
        {
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
                throw new Exception(String.Format("Loading XML Failed: {0}", e.ToString()));
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
            if (!DoesVariableExist(uniqueName, "Boolean"))
            {
                XmlNode metaDataNode = AddToHeadingNode("Boolean");
                _xmlDocBody.AppendChild(metaDataNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/Boolean/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    //groupNode.InnerText = group.ToString();
                    metaDataNode.AppendChild(groupNode);
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
                defaultValueNode.InnerText = value.ToString();

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
            if (!DoesVariableExist(uniqueName, "ByteArray"))
            {
                XmlNode metaDataNode = AddToHeadingNode("ByteArray");
                _xmlDocBody.AppendChild(metaDataNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/ByteArray/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    //groupNode.InnerText = group.ToString();
                    metaDataNode.AppendChild(groupNode);
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
                defaultValueNode.InnerText = Encoding.UTF8.GetString(value);

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
            if (!DoesVariableExist(uniqueName, "Double"))
            {
                XmlNode metaDataNode = AddToHeadingNode("Double");
                _xmlDocBody.AppendChild(metaDataNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/Double/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    //groupNode.InnerText = group.ToString();
                    metaDataNode.AppendChild(groupNode);
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
                defaultValueNode.InnerText = value.ToString();

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
            if (!DoesVariableExist(uniqueName, "Float"))
            {
                XmlNode metaDataNode = AddToHeadingNode("Float");
                _xmlDocBody.AppendChild(metaDataNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/Float/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    //groupNode.InnerText = group.ToString();
                    metaDataNode.AppendChild(groupNode);
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
                defaultValueNode.InnerText = value.ToString();

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

        public bool AddInt(string uniqueName, int value, string description, string group = "default")
        {
            return AddInt32(uniqueName, value, description, group);
        }

        public bool AddInt16(string uniqueName, short value, string description, string group = "default")
        {
            if (!DoesVariableExist(uniqueName, "Int16"))
            {
                XmlNode metaDataNode = AddToHeadingNode("Int16");
                _xmlDocBody.AppendChild(metaDataNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/Int16/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    //groupNode.InnerText = group.ToString();
                    metaDataNode.AppendChild(groupNode);
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
                defaultValueNode.InnerText = value.ToString();

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
            if (!DoesVariableExist(uniqueName, "Int32"))
            {
                XmlNode metaDataNode = AddToHeadingNode("Int32");
                _xmlDocBody.AppendChild(metaDataNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/Int32/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    //groupNode.InnerText = group.ToString();
                    metaDataNode.AppendChild(groupNode);
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
                defaultValueNode.InnerText = value.ToString();

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
            if (!DoesVariableExist(uniqueName, "Int64"))
            {
                XmlNode metaDataNode = AddToHeadingNode("Int64");
                _xmlDocBody.AppendChild(metaDataNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/Int64/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    //groupNode.InnerText = group.ToString();
                    metaDataNode.AppendChild(groupNode);
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
                defaultValueNode.InnerText = value.ToString();

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

        public bool AddLong(string uniqueName, long value, string description, string group = "default")
        {
            return AddInt64(uniqueName, value, description, group);
        }

        public bool AddString(string uniqueName, string value, string description, string group = "default")
        {
            if (!DoesVariableExist(uniqueName, "String"))
            {
                XmlNode metaDataNode = AddToHeadingNode("String");
                _xmlDocBody.AppendChild(metaDataNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/String/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    //groupNode.InnerText = group.ToString();
                    metaDataNode.AppendChild(groupNode);
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
                defaultValueNode.InnerText = value.ToString();

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

        public bool SetInt(string uniqueName, int value)
        {
            return SetInt32(uniqueName, value);
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

        public bool SetLong(string uniqueName, long value)
        {
            return SetInt64(uniqueName, value);
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

        public int GetInt(string uniqueName)
        {
            return GetInt32(uniqueName);
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

        public long GetLong(string uniqueName)
        {
            return GetInt64(uniqueName);
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

        public bool DeleteInt(string uniqueName)
        {
            return DeleteInt32(uniqueName);
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

        public bool DeleteLong(string uniqueName)
        {
            return DeleteInt64(uniqueName);
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
                XmlNode metaDataNode = AddToHeadingNode("_MetaData");
                _xmlDocBody.AppendChild(metaDataNode);

                XmlNode groupNode;

                if (_xmlDoc.SelectNodes(String.Format("//SSM/_MetaData/{0}", group)).Count == 0)
                {
                    groupNode = _xmlDoc.CreateElement(group);
                    //groupNode.InnerText = group.ToString();
                    metaDataNode.AppendChild(groupNode);
                }
                else
                {
                    groupNode = _xmlDoc.SelectNodes(String.Format("//SSM/_MetaData/{0}", group))[0];
                }

                XmlNode varNode = _xmlDoc.CreateElement(varName);
                groupNode.AppendChild(varNode);

                XmlNode varValueNode = _xmlDoc.CreateElement("value");
                varValueNode.InnerText = varValue.ToString();

                XmlNode defaultValueNode = _xmlDoc.CreateElement("default");
                defaultValueNode.InnerText = varValue.ToString();

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
            throw new NotImplementedException();
        }

        public bool DeleteMetaData(string varName)
        {
            return RemoveXmlElement(varName, "_MetaData");
        }
        #endregion

        #region DataEntry

        public void ImportDataEntry(DataEntry dataEntry)
        {
            throw new NotImplementedException();
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
                        dataList.Add(new DataEntry(typeof(MetaDataObject), xmlNodeGroup.ChildNodes[i].Name, xmlNodeGroup.Name, Encoding.UTF8.GetBytes(xmlNodeGroup.ChildNodes[i].SelectSingleNode("value").InnerText), Encoding.UTF8.GetBytes(xmlNodeGroup.ChildNodes[i].SelectSingleNode("default").InnerText), xmlNodeGroup.ChildNodes[i].SelectSingleNode("description").InnerText));
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

        public DataEntry[] GetAllInt()
        {
            return GetAllInt32();
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

        public DataEntry[] GetAllLong()
        {
            return GetAllInt64();
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

            if (this.GetAllMetaData() != null) dataList.AddRange(this.GetAllMetaData());
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

                if (_xmlDoc.SelectNodes(String.Format("//SSM/_MetaData/{0}/{1}", type, varName)).Count > 0)
                {
                    XmlNode node = _xmlDoc.SelectNodes(String.Format("//SSM/_MetaData/{0}/{1}", type, varName))[0];

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
                throw new Exception(String.Format("XML Declaration Error: {0}", e.ToString()));
            }
        }

        #endregion
    }
}
