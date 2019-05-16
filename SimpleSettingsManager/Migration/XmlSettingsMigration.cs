using SimpleSettingsManager.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SimpleSettingsManager.Migration
{
    public class XmlSettingsMigration
    {
        private string _xmlFilePath;
        private SSM_File _exportSsmFile;

        private XmlDocument _xmlDoc;
        private XmlElement _xmlDocBody;
        private string _xmlDocContent;
        private XmlDeclaration _xmlDocDec;

        /// <summary>
        /// Migrates an XmlSettings file to a new SSM File
        /// </summary>
        /// <param name="xmlSettingsXmlPath">Path to an XmlSettings file</param>
        /// <param name="exportSsmFile">>New SSM File to write to</param>
        public XmlSettingsMigration(string xmlSettingsXmlPath, SSM_File exportSsmFile)
        {
            if (File.Exists(xmlSettingsXmlPath) && Utilities.IsFileXML(xmlSettingsXmlPath))
            {
                _xmlFilePath = xmlSettingsXmlPath;
                _exportSsmFile = exportSsmFile;
                _xmlDoc = new XmlDocument();
            }
            else
            {
                Logging.Log(String.Format("You did not specify a valid XmlSettings file for XmlSettingsMigration!"), Severity.ERROR);
                throw new Exception("Error: You did not specify a valid XmlSettings file for XmlSettingsMigration!");
            }
        }

        /// <summary>
        /// Begins the migration process
        /// </summary>
        public void Migrate()
        {
            InitXmlSettingsXML();
            SSM exportSsm = new SSM(_exportSsmFile);

            exportSsm.Open();

            try
            {
                DataEntry[] dataEntries = GetAllTypes();
                Console.WriteLine("Total Entries: {0}", dataEntries.Length);

                foreach (DataEntry data in dataEntries)
                {
                    if (data != null)
                    {
                        Console.WriteLine("Importing '{0}'", data.GetVariableName());
                        exportSsm.ImportDataEntry(data);
                    }
                }
                exportSsm.UpdateXmlSettingsMigrationStatus();
            }
            catch (Exception e)
            {
                Logging.Log(String.Format("An unexpected error occured within XmlSettingsMigration! Exception: {0}", e.ToString()), Severity.ERROR);
                throw new Exception("Error: An unexpected error occured within XmlSettingsMigration! Are you sure you selected a valid XmlSettings file? Exception: " + e.ToString());
            }
            finally
            {
                exportSsm.Close();
            }
        }

        #region Load XmlSettings XML File

        private void InitXmlSettingsXML()
        {
            LoadXML();
            LoadXmlDeclaration();
        }

        private void LoadXML()
        {
            try
            {
                _xmlDoc.Load(_xmlFilePath);
                _xmlDocContent = _xmlDoc.InnerXml;
                _xmlDocBody = _xmlDoc.DocumentElement;
            }
            catch (Exception e)
            {
                Logging.Log(String.Format("Loading of the XmlSettings file failed!"), Severity.ERROR);
                throw new Exception(String.Format("Error: Loading XmlSettings Failed: {0}", e.ToString()));
            }
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
                Logging.Log(String.Format("XmlSettings XML Declaration is invalid!"), Severity.ERROR);
                throw new FileLoadException(String.Format("XmlSettings XML Declaration Error: {0}", e.ToString()));
            }
        }

        #endregion
        #region Convert XmlSettings Entries to SSM-compatible DataEntries

        private DataEntry[] GetAllInt16()
        {
            if (_xmlDoc.SelectNodes(String.Format("//body/{0}/*", "Int16")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//body/{0}/*", "Int16"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(Int16), xmlNodeGroup.ChildNodes[i].ParentNode.Name, "default", BitConverter.GetBytes(Convert.ToInt16(xmlNodeGroup.ChildNodes[i].InnerText)), BitConverter.GetBytes(Convert.ToInt16(xmlNodeGroup.ChildNodes[i].ParentNode.Attributes["default"].InnerText)), xmlNodeGroup.ChildNodes[i].ParentNode.Name));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        private DataEntry[] GetAllInt32()
        {
            if (_xmlDoc.SelectNodes(String.Format("//body/{0}/*", "int")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//body/{0}/*", "int"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(Int32), xmlNodeGroup.ChildNodes[i].ParentNode.Name, "default", BitConverter.GetBytes(Convert.ToInt32(xmlNodeGroup.ChildNodes[i].InnerText)), BitConverter.GetBytes(Convert.ToInt32(xmlNodeGroup.ChildNodes[i].ParentNode.Attributes["default"].InnerText)), xmlNodeGroup.ChildNodes[i].ParentNode.Name));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        private DataEntry[] GetAllInt64()
        {
            if (_xmlDoc.SelectNodes(String.Format("//body/{0}/*", "long")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//body/{0}/*", "long"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(Int64), xmlNodeGroup.ChildNodes[i].ParentNode.Name, "default", BitConverter.GetBytes(Convert.ToInt64(xmlNodeGroup.ChildNodes[i].InnerText)), BitConverter.GetBytes(Convert.ToInt64(xmlNodeGroup.ChildNodes[i].ParentNode.Attributes["default"].InnerText)), xmlNodeGroup.ChildNodes[i].ParentNode.Name));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        private DataEntry[] GetAllUInt16()
        {
            if (_xmlDoc.SelectNodes(String.Format("//body/{0}/*", "UInt16")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//body/{0}/*", "UInt16"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(UInt16), xmlNodeGroup.ChildNodes[i].ParentNode.Name, "default", BitConverter.GetBytes(Convert.ToUInt16(xmlNodeGroup.ChildNodes[i].InnerText)), BitConverter.GetBytes(Convert.ToUInt16(xmlNodeGroup.ChildNodes[i].ParentNode.Attributes["default"].InnerText)), xmlNodeGroup.ChildNodes[i].ParentNode.Name));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        private DataEntry[] GetAllUInt32()
        {
            if (_xmlDoc.SelectNodes(String.Format("//body/{0}/*", "UInt32")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//body/{0}/*", "UInt32"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(UInt32), xmlNodeGroup.ChildNodes[i].ParentNode.Name, "default", BitConverter.GetBytes(Convert.ToUInt32(xmlNodeGroup.ChildNodes[i].InnerText)), BitConverter.GetBytes(Convert.ToUInt32(xmlNodeGroup.ChildNodes[i].ParentNode.Attributes["default"].InnerText)), xmlNodeGroup.ChildNodes[i].ParentNode.Name));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        private DataEntry[] GetAllUInt64()
        {
            if (_xmlDoc.SelectNodes(String.Format("//body/{0}/*", "UInt64")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//body/{0}/*", "UInt64"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(UInt64), xmlNodeGroup.ChildNodes[i].ParentNode.Name, "default", BitConverter.GetBytes(Convert.ToUInt64(xmlNodeGroup.ChildNodes[i].InnerText)), BitConverter.GetBytes(Convert.ToUInt64(xmlNodeGroup.ChildNodes[i].ParentNode.Attributes["default"].InnerText)), xmlNodeGroup.ChildNodes[i].ParentNode.Name));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        private DataEntry[] GetAllFloat()
        {
            if (_xmlDoc.SelectNodes(String.Format("//body/{0}/*", "float")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//body/{0}/*", "float"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(float), xmlNodeGroup.ChildNodes[i].ParentNode.Name, "default", BitConverter.GetBytes(Convert.ToSingle(xmlNodeGroup.ChildNodes[i].InnerText)), BitConverter.GetBytes(Convert.ToSingle(xmlNodeGroup.ChildNodes[i].ParentNode.Attributes["default"].InnerText)), xmlNodeGroup.ChildNodes[i].ParentNode.Name));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        private DataEntry[] GetAllDouble()
        {
            if (_xmlDoc.SelectNodes(String.Format("//body/{0}/*", "double")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//body/{0}/*", "double"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(double), xmlNodeGroup.ChildNodes[i].ParentNode.Name, "default", BitConverter.GetBytes(Convert.ToDouble(xmlNodeGroup.ChildNodes[i].InnerText)), BitConverter.GetBytes(Convert.ToDouble(xmlNodeGroup.ChildNodes[i].ParentNode.Attributes["default"].InnerText)), xmlNodeGroup.ChildNodes[i].ParentNode.Name));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        private DataEntry[] GetAllString()
        {
            if (_xmlDoc.SelectNodes(String.Format("//body/{0}/*", "string")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//body/{0}/*", "string"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(String), xmlNodeGroup.ChildNodes[i].ParentNode.Name, "default", Encoding.UTF8.GetBytes(xmlNodeGroup.ChildNodes[i].InnerText), Encoding.UTF8.GetBytes(xmlNodeGroup.ChildNodes[i].ParentNode.Attributes["default"].InnerText), xmlNodeGroup.ChildNodes[i].ParentNode.Name));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        private DataEntry[] GetAllBytes()
        {
            if (_xmlDoc.SelectNodes(String.Format("//body/{0}/*", "byte")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//body/{0}/*", "byte"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(byte[]), xmlNodeGroup.ChildNodes[i].ParentNode.Name, "default", Encoding.UTF8.GetBytes(xmlNodeGroup.ChildNodes[i].InnerText), Encoding.UTF8.GetBytes(xmlNodeGroup.ChildNodes[i].ParentNode.Attributes["default"].InnerText), xmlNodeGroup.ChildNodes[i].ParentNode.Name));
                    }
                }
                return dataList.ToArray();
            }
            return null;
        }

        private DataEntry[] GetAllBooleans()
        {
            if (_xmlDoc.SelectNodes(String.Format("//body/{0}/*", "boolean")).Count > 0)
            {
                List<DataEntry> dataList = new List<DataEntry>();

                XmlNodeList xmlNodeList = _xmlDoc.SelectNodes(String.Format("//body/{0}/*", "boolean"));

                foreach (XmlNode xmlNodeGroup in xmlNodeList)
                {
                    for (int i = 0; i < xmlNodeGroup.ChildNodes.Count; i++)
                    {
                        dataList.Add(new DataEntry(typeof(bool), xmlNodeGroup.ChildNodes[i].ParentNode.Name, "default", BitConverter.GetBytes(Convert.ToBoolean(xmlNodeGroup.ChildNodes[i].InnerText)), BitConverter.GetBytes(Convert.ToBoolean(xmlNodeGroup.ChildNodes[i].ParentNode.Attributes["default"].InnerText)), xmlNodeGroup.ChildNodes[i].ParentNode.Name));
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
                Task.Factory.StartNew(() => { try { if (this.GetAllInt16() != null) dataList.AddRange(this.GetAllInt16()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllInt32() != null) dataList.AddRange(this.GetAllInt32()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllInt64() != null) dataList.AddRange(this.GetAllInt64()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllUInt16() != null) dataList.AddRange(this.GetAllUInt16()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllUInt32() != null) dataList.AddRange(this.GetAllUInt32()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllUInt64() != null) dataList.AddRange(this.GetAllUInt64()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllFloat() != null) dataList.AddRange(this.GetAllFloat()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllDouble() != null) dataList.AddRange(this.GetAllDouble()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllString() != null) dataList.AddRange(this.GetAllString()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllBytes() != null) dataList.AddRange(this.GetAllBytes()); } catch { } }),
                Task.Factory.StartNew(() => { try { if (this.GetAllBooleans() != null) dataList.AddRange(this.GetAllBooleans()); } catch { } })
            };
            Task.WaitAll(allTypesTasks.ToArray());

            return dataList.ToArray();
        }

        #endregion
    }
}
