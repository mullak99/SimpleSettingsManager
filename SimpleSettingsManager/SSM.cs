using System;
using System.IO;
using System.Reflection;
using SimpleSettingsManager.Data;
using SimpleSettingsManager.Mode;

namespace SimpleSettingsManager
{
    public class SSM
    {
        private const string _ssmFormatVer = "1.0";
        private const string _minSsmFormatVer = "1.0";

        private const bool IsPreReleaseBuild = true;
        private const string PreReleaseTag = "BETA_2";

        private static bool _verboseMode = false;
        private static bool _logToFile = false; 

        private SSM_File _ssmFile;
        private IMode _handler;

        /// <summary>
        /// Creates a new SSM instance with the chosen SSM File
        /// </summary>
        /// <param name="ssmFile">SSM File to read-from/write-to</param>
        public SSM(SSM_File ssmFile)
        {
            _ssmFile = ssmFile;
            _handler = _ssmFile.GetHandler();

            Logging.Log(String.Format("New SSM instance started! ({0})", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
            Logging.Log(String.Format("SSM_File Name: {0}, SSM_File Mode: {1}, SSM_File Path: {2}", _ssmFile.GetFileName(), _ssmFile.GetMode(), _ssmFile.GetPath(true)), Severity.DEBUG);
        }

        /// <summary>
        /// Creates a new SSM instance with the chosen SSM File
        /// </summary>
        /// <param name="ssmFile">SSM File to read-from/write-to</param>
        /// <param name="verboseLogging">Verbose logging state</param>
        /// <param name="logToFile">File logging state</param>
        public SSM(SSM_File ssmFile, bool verboseLogging = false, bool logToFile = false) : this(ssmFile)
        {
            _verboseMode = verboseLogging;
            _logToFile = logToFile;
        }

        #region SSM Getters/Setters

        /// <summary>
        /// Get the SSM File currently used in SSM
        /// </summary>
        /// <returns>SSM File in-use</returns>
        public SSM_File GetSSMFile()
        {
            return _ssmFile;
        }

        /// <summary>
        /// Used to get the current version of SSM
        /// </summary>
        /// <returns>The version number of SSM</returns>
        public static string GetVersion()
        {
            #pragma warning disable CS0162 //Unreachable code detected
            string[] ver = (typeof(SSM).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version).Split('.');
            if (!IsPreReleaseBuild)
                return "v" + ver[0] + "." + ver[1] + "." + ver[2];
            else
                return "v" + ver[0] + "." + ver[1] + "." + ver[2] + "-" + PreReleaseTag;
            #pragma warning restore CS0162 //Unreachable code detected
        }

        /// <summary>
        /// Get SSM BuildDate as a formatted string (dd/MM/yyyy hh:mm:ss tt)
        /// </summary>
        /// <returns>SSM BuildDate as a string</returns>
        public static string GetBuildDateFormatted()
        {
            return GetBuildDate().ToString("dd/MM/yyyy hh:mm:ss tt");
        }

        /// <summary>
        /// Get SSM BuildDate as DateTime
        /// </summary>
        /// <returns>SSM BuildDate as DateTime</returns>
        public static DateTime GetBuildDate()
        {
            return new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;
        }

        /// <summary>
        /// Sets if the programs operation should be verbosely logged to console.
        /// </summary>
        /// <param name="state">Verbose logging state</param>
        public static void SetVerboseLogging(bool state)
        {
            _verboseMode = state;
        }

        /// <summary>
        /// Gets if the programs operation are being verbosely logged to console.
        /// </summary>
        /// <returns>Verbose logging state</returns>
        public static bool GetVerboseLogging()
        {
            return _verboseMode;
        }

        /// <summary>
        /// Sets if the program should also output its log to a file (SSM.log)
        /// </summary>
        /// <param name="state">File logging state</param>
        public static void SetFileLogging(bool state)
        {
            _logToFile = state;
        }

        /// <summary>
        /// Gets if the program is also outputting its log to a file (SSM.log)
        /// </summary>
        /// <returns>File logging state</returns>
        public static bool GetFileLogging()
        {
            return _logToFile;
        }

        /// <summary>
        /// Gets if the current version of SSM is a Pre-Release version
        /// </summary>
        /// <returns>If the current SSM version is a Pre-Release build</returns>
        public static bool IsPreReleaseVersion()
        {
            return IsPreReleaseBuild;
        }

        /// <summary>
        /// Gets the current formatting version of SSM
        /// </summary>
        /// <returns>SSM formatting version</returns>
        public static string GetSsmFormatVersion()
        {
            return _ssmFormatVer;
        }

        /// <summary>
        /// Gets the minimum supported formatting version of SSM
        /// </summary>
        /// <returns>SSM formatting version</returns>
        public static string GetMinimumSupportedSsmFormat()
        {
            return _minSsmFormatVer;
        }

        #endregion

        #region Init

        /// <summary>
        /// Opens the SSM File
        /// </summary>
        public void Open()
        {
            if (_ssmFile != null)
                _handler.Open(_ssmFile);
            else
            {
                Logging.Log("No SSM File has been selected!", Severity.ERROR);
                throw new Exception("No SSM File has been selected!");
            }
        }

        /// <summary>
        /// Closes the SSM File
        /// </summary>
        public void Close()
        {
            if (_ssmFile != null)
                _handler.Close();
            else
            {
                Logging.Log("No SSM File has been selected!", Severity.ERROR);
                throw new Exception("No SSM File has been selected!");
            }
        }

        /// <summary>
        /// Updates the CrossModeMigrationStatus of an SSM File
        /// </summary>
        internal void UpdateCrossModeMigrationStatus()
        {
            if (_ssmFile != null)
                _handler.UpdateCrossModeMigrationStatus();
            else
            {
                Logging.Log("No SSM File has been selected!", Severity.ERROR);
                throw new Exception("No SSM File has been selected!");
            }
        }

        /// <summary>
        /// Updates the XmlSettingsMigrationStatus of an SSM File
        /// </summary>
        internal void UpdateXmlSettingsMigrationStatus()
        {
            if (_ssmFile != null)
                _handler.UpdateXmlSettingsMigrationStatus();
            else
            {
                Logging.Log("No SSM File has been selected!", Severity.ERROR);
                throw new Exception("No SSM File has been selected!");
            }
        }

        #endregion

        #region Add Variables

        /// <summary>
        /// Creates a new short (Int16) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If a new variable was created successfully</returns>
        public bool AddInt16(string uniqueName, Int16 value, string description, string group = "default")
        {
            return _handler.AddInt16(uniqueName, value, description, group);
        }

        /// <summary>
        /// Creates a new short (Int16) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If a new variable was created successfully</returns>
        public bool AddShort(string uniqueName, short value, string description, string group = "default")
        {
            return AddInt16(uniqueName, value, description, group);
        }

        /// <summary>
        /// Creates a new int (Int32) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If a new variable was created successfully</returns>
        public bool AddInt32(string uniqueName, Int32 value, string description, string group = "default")
        {
            return _handler.AddInt32(uniqueName, value, description, group);
        }

        /// <summary>
        /// Creates a new int (Int32) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If a new variable was created successfully</returns>
        public bool AddInt(string uniqueName, int value, string description, string group = "default")
        {
            return AddInt32(uniqueName, value, description, group);
        }

        /// <summary>
        /// Creates a new long (Int64) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If a new variable was created successfully</returns>
        public bool AddInt64(string uniqueName, Int64 value, string description, string group = "default")
        {
            return _handler.AddInt64(uniqueName, value, description, group);
        }

        /// <summary>
        /// Creates a new long (Int64) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If a new variable was created successfully</returns>
        public bool AddLong(string uniqueName, long value, string description, string group = "default")
        {
            return AddInt64(uniqueName, value, description, group);
        }

        /// <summary>
        /// Creates a new ushort (UInt16) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If a new variable was created successfully</returns>
        public bool AddUInt16(string uniqueName, UInt16 value, string description, string group = "default")
        {
            return _handler.AddUInt16(uniqueName, value, description, group);
        }

        /// <summary>
        /// Creates a new ushort (UInt16) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If a new variable was created successfully</returns>
        public bool AddUShort(string uniqueName, ushort value, string description, string group = "default")
        {
            return AddUInt16(uniqueName, value, description, group);
        }

        /// <summary>
        /// Creates a new uint (UInt32) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If a new variable was created successfully</returns>
        public bool AddUInt32(string uniqueName, UInt32 value, string description, string group = "default")
        {
            return _handler.AddUInt32(uniqueName, value, description, group);
        }

        /// <summary>
        /// Creates a new uint (UInt32) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If a new variable was created successfully</returns>
        public bool AddUInt(string uniqueName, uint value, string description, string group = "default")
        {
            return AddUInt32(uniqueName, value, description, group);
        }

        /// <summary>
        /// Creates a new ulong (UInt64) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If a new variable was created successfully</returns>
        public bool AddUInt64(string uniqueName, UInt64 value, string description, string group = "default")
        {
            return _handler.AddUInt64(uniqueName, value, description, group);
        }

        /// <summary>
        /// Creates a new ulong (UInt64) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If a new variable was created successfully</returns>
        public bool AddULong(string uniqueName, ulong value, string description, string group = "default")
        {
            return AddUInt64(uniqueName, value, description, group);
        }

        /// <summary>
        /// Creates a new float value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If a new variable was created successfully</returns>
        public bool AddFloat(string uniqueName, float value, string description, string group = "default")
        {
            return _handler.AddFloat(uniqueName, value, description, group);
        }

        /// <summary>
        /// Creates a new double value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If a new variable was created successfully</returns>
        public bool AddDouble(string uniqueName, double value, string description, string group = "default")
        {
            return _handler.AddDouble(uniqueName, value, description, group);
        }

        /// <summary>
        /// Creates a new string value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If a new variable was created successfully</returns>
        public bool AddString(string uniqueName, string value, string description, string group = "default")
        {
            return _handler.AddString(uniqueName, value, description, group);
        }

        /// <summary>
        /// Creates a new byte[] value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If a new variable was created successfully</returns>
        public bool AddByteArray(string uniqueName, byte[] value, string description, string group = "default")
        {
            return _handler.AddByteArray(uniqueName, value, description, group);
        }

        /// <summary>
        /// Creates a new boolean value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If a new variable was created successfully</returns>
        public bool AddBoolean(string uniqueName, bool value, string description, string group = "default")
        {
            return _handler.AddBoolean(uniqueName, value, description, group);
        }

        #endregion
        #region Set Variables

        /// <summary>
        /// Sets the value of an existing short (Int16) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool SetInt16(string uniqueName, Int16 value)
        {
            return _handler.SetInt16(uniqueName, value);
        }

        /// <summary>
        /// Sets the value of an existing short (Int16) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool SetShort(string uniqueName, short value)
        {
            return SetInt16(uniqueName, value);
        }

        /// <summary>
        /// Sets the value of an existing int (Int32) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool SetInt32(string uniqueName, Int32 value)
        {
            return _handler.SetInt32(uniqueName, value);
        }

        /// <summary>
        /// Sets the value of an existing int (Int32) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool SetInt(string uniqueName, int value)
        {
            return SetInt32(uniqueName, value);
        }

        /// <summary>
        /// Sets the value of an existing long (Int64) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool SetInt64(string uniqueName, Int64 value)
        {
            return _handler.SetInt64(uniqueName, value);
        }

        /// <summary>
        /// Sets the value of an existing long (Int64) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool SetLong(string uniqueName, long value)
        {
            return SetInt64(uniqueName, value);
        }

        /// <summary>
        /// Sets the value of an existing ushort (UInt16) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool SetUInt16(string uniqueName, UInt16 value)
        {
            return _handler.SetUInt16(uniqueName, value);
        }

        /// <summary>
        /// Sets the value of an existing ushort (UInt16) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool SetUShort(string uniqueName, ushort value)
        {
            return SetUInt16(uniqueName, value);
        }

        /// <summary>
        /// Sets the value of an existing uint (UInt32) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool SetUInt32(string uniqueName, UInt32 value)
        {
            return _handler.SetUInt32(uniqueName, value);
        }

        /// <summary>
        /// Sets the value of an existing uint (UInt32) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool SetUInt(string uniqueName, uint value)
        {
            return SetUInt32(uniqueName, value);
        }

        /// <summary>
        /// Sets the value of an existing ulong (UInt64) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool SetUInt64(string uniqueName, UInt64 value)
        {
            return _handler.SetUInt64(uniqueName, value);
        }

        /// <summary>
        /// Sets the value of an existing ulong (UInt64) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool SetULong(string uniqueName, ulong value)
        {
            return SetUInt64(uniqueName, value);
        }

        /// <summary>
        /// Sets the value of an existing float in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool SetFloat(string uniqueName, float value)
        {
            return _handler.SetFloat(uniqueName, value);
        }

        /// <summary>
        /// Sets the value of an existing double in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool SetDouble(string uniqueName, double value)
        {
            return _handler.SetDouble(uniqueName, value);
        }

        /// <summary>
        /// Sets the value of an existing string in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool SetString(string uniqueName, string value)
        {
            return _handler.SetString(uniqueName, value);
        }

        /// <summary>
        /// Sets the value of an existing byte[] in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool SetByteArray(string uniqueName, byte[] value)
        {
            return _handler.SetByteArray(uniqueName, value);
        }

        /// <summary>
        /// Sets the value of an existing boolean in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="value">Value of the variable</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool SetBoolean(string uniqueName, bool value)
        {
            return _handler.SetBoolean(uniqueName, value);
        }

        #endregion
        #region Edit Variables

        /// <summary>
        /// Edits the misc data of an existing short (Int16) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool EditInt16(string uniqueName, string description, string group)
        {
            return _handler.EditInt16(uniqueName, description, group);
        }

        /// <summary>
        /// Edits the misc data of an existing short (Int16) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool EditShort(string uniqueName, string description, string group)
        {
            return EditInt16(uniqueName, description, group);
        }

        /// <summary>
        /// Edits the misc data of an existing int (Int32) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool EditInt32(string uniqueName, string description, string group)
        {
            return _handler.EditInt32(uniqueName, description, group);
        }

        /// <summary>
        /// Edits the misc data of an existing int (Int32) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool EditInt(string uniqueName, string description, string group)
        {
            return EditInt32(uniqueName, description, group);
        }

        /// <summary>
        /// Edits the misc data of an existing long (Int64) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool EditInt64(string uniqueName, string description, string group)
        {
            return _handler.EditInt64(uniqueName, description, group);
        }

        /// <summary>
        /// Edits the misc data of an existing long (Int64) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool EditLong(string uniqueName, string description, string group)
        {
            return EditInt64(uniqueName, description, group);
        }

        /// <summary>
        /// Edits the misc data of an existing ushort (UInt16) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool EditUInt16(string uniqueName, string description, string group)
        {
            return _handler.EditUInt16(uniqueName, description, group);
        }

        /// <summary>
        /// Edits the misc data of an existing ushort (UInt16) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool EditUShort(string uniqueName, string description, string group)
        {
            return EditUInt16(uniqueName, description, group);
        }

        /// <summary>
        /// Edits the misc data of an existing uint (UInt32) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool EditUInt32(string uniqueName, string description, string group)
        {
            return _handler.EditUInt32(uniqueName, description, group);
        }

        /// <summary>
        /// Edits the misc data of an existing uint (UInt32) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool EditUInt(string uniqueName, string description, string group)
        {
            return EditUInt32(uniqueName, description, group);
        }

        /// <summary>
        /// Edits the misc data of an existing ulong (UInt64) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool EditUInt64(string uniqueName, string description, string group)
        {
            return _handler.EditUInt64(uniqueName, description, group);
        }

        /// <summary>
        /// Edits the misc data of an existing ulong (UInt64) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool EditULong(string uniqueName, string description, string group)
        {
            return EditUInt64(uniqueName, description, group);
        }

        /// <summary>
        /// Edits the misc data of an existing float in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool EditFloat(string uniqueName, string description, string group)
        {
            return _handler.EditFloat(uniqueName, description, group);
        }

        /// <summary>
        /// Edits the misc data of an existing double in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool EditDouble(string uniqueName, string description, string group)
        {
            return _handler.EditDouble(uniqueName, description, group);
        }

        /// <summary>
        /// Edits the misc data of an existing string in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool EditString(string uniqueName, string description, string group)
        {
            return _handler.EditString(uniqueName, description, group);
        }

        /// <summary>
        /// Edits the misc data of an existing byte[] in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool EditByteArray(string uniqueName, string description, string group)
        {
            return _handler.EditByteArray(uniqueName, description, group);
        }

        /// <summary>
        /// Edits the misc data of an existing boolean in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <param name="description">Description of the variable</param>
        /// <param name="group">Group the variable belongs to</param>
        /// <returns>If the variable was edited successfully</returns>
        public bool EditBoolean(string uniqueName, string description, string group)
        {
            return _handler.EditBoolean(uniqueName, description, group);
        }

        #endregion
        #region Get Variables

        /// <summary>
        /// Gets the value of an existing short (Int16) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>Value of the variable</returns>
        public Int16 GetInt16(string uniqueName)
        {
            return _handler.GetInt16(uniqueName);
        }

        /// <summary>
        /// Gets the value of an existing short (Int16) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>Value of the variable</returns>
        public short GetShort(string uniqueName)
        {
            return GetInt16(uniqueName);
        }

        /// <summary>
        /// Gets the value of an existing int (Int32) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>Value of the variable</returns>
        public Int32 GetInt32(string uniqueName)
        {
            return _handler.GetInt32(uniqueName);
        }

        /// <summary>
        /// Gets the value of an existing int (Int32) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>Value of the variable</returns>
        public int GetInt(string uniqueName)
        {
            return GetInt32(uniqueName);
        }

        /// <summary>
        /// Gets the value of an existing long (Int64) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>Value of the variable</returns>
        public Int64 GetInt64(string uniqueName)
        {
            return _handler.GetInt64(uniqueName);
        }

        /// <summary>
        /// Gets the value of an existing long (Int64) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>Value of the variable</returns>
        public long GetLong(string uniqueName)
        {
            return GetInt64(uniqueName);
        }

        /// <summary>
        /// Gets the value of an existing ushort (UInt16) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>Value of the variable</returns>
        public UInt16 GetUInt16(string uniqueName)
        {
            return _handler.GetUInt16(uniqueName);
        }

        /// <summary>
        /// Gets the value of an existing ushort (UInt16) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>Value of the variable</returns>
        public ushort GetUShort(string uniqueName)
        {
            return GetUInt16(uniqueName);
        }

        /// <summary>
        /// Gets the value of an existing uint (UInt32) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>Value of the variable</returns>
        public UInt32 GetUInt32(string uniqueName)
        {
            return _handler.GetUInt32(uniqueName);
        }

        /// <summary>
        /// Gets the value of an existing uint (UInt32) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>Value of the variable</returns>
        public uint GetUInt(string uniqueName)
        {
            return GetUInt32(uniqueName);
        }

        /// <summary>
        /// Gets the value of an existing ulong (UInt64) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>Value of the variable</returns>
        public UInt64 GetUInt64(string uniqueName)
        {
            return _handler.GetUInt64(uniqueName);
        }

        /// <summary>
        /// Gets the value of an existing ulong (UInt64) in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>Value of the variable</returns>
        public ulong GetULong(string uniqueName)
        {
            return _handler.GetUInt64(uniqueName);
        }

        /// <summary>
        /// Gets the value of an existing float in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>Value of the variable</returns>
        public float GetFloat(string uniqueName)
        {
            return _handler.GetFloat(uniqueName);
        }

        /// <summary>
        /// Gets the value of an existing double in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>Value of the variable</returns>
        public double GetDouble(string uniqueName)
        {
            return _handler.GetDouble(uniqueName);
        }

        /// <summary>
        /// Gets the value of an existing string in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>Value of the variable</returns>
        public string GetString(string uniqueName)
        {
            return _handler.GetString(uniqueName);
        }

        /// <summary>
        /// Gets the value of an existing byte[] in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>Value of the variable</returns>
        public byte[] GetByteArray(string uniqueName)
        {
            return _handler.GetByteArray(uniqueName);
        }

        /// <summary>
        /// Gets the value of an existing boolean in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>Value of the variable</returns>
        public bool GetBoolean(string uniqueName)
        {
            return _handler.GetBoolean(uniqueName);
        }

        #endregion
        #region Delete Variables

        /// <summary>
        /// Deletes an existing short (Int16) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>If the deletion was successful</returns>
        public bool DeleteInt16(string uniqueName)
        {
            return _handler.DeleteInt16(uniqueName);
        }

        /// <summary>
        /// Deletes an existing short (Int16) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>If the deletion was successful</returns>
        public bool DeleteShort(string uniqueName)
        {
            return DeleteInt16(uniqueName);
        }

        /// <summary>
        /// Deletes an existing int (Int32) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>If the deletion was successful</returns>
        public bool DeleteInt32(string uniqueName)
        {
            return _handler.DeleteInt32(uniqueName);
        }

        /// <summary>
        /// Deletes an existing int (Int32) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>If the deletion was successful</returns>
        public bool DeleteInt(string uniqueName)
        {
            return DeleteInt32(uniqueName);
        }

        /// <summary>
        /// Deletes an existing long (Int64) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>If the deletion was successful</returns>
        public bool DeleteInt64(string uniqueName)
        {
            return _handler.DeleteInt64(uniqueName);
        }

        /// <summary>
        /// Deletes an existing long (Int64) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>If the deletion was successful</returns>
        public bool DeleteLong(string uniqueName)
        {
            return DeleteInt64(uniqueName);
        }

        /// <summary>
        /// Deletes an existing ushort (UInt16) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>If the deletion was successful</returns>
        public bool DeleteUInt16(string uniqueName)
        {
            return _handler.DeleteUInt16(uniqueName);
        }

        /// <summary>
        /// Deletes an existing ushort (UInt16) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>If the deletion was successful</returns>
        public bool DeleteUShort(string uniqueName)
        {
            return DeleteUInt16(uniqueName);
        }

        /// <summary>
        /// Deletes an existing uint (UInt32) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>If the deletion was successful</returns>
        public bool DeleteUInt32(string uniqueName)
        {
            return _handler.DeleteUInt32(uniqueName);
        }

        /// <summary>
        /// Deletes an existing uint (UInt32) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>If the deletion was successful</returns>
        public bool DeleteUInt(string uniqueName)
        {
            return DeleteUInt32(uniqueName);
        }

        /// <summary>
        /// Deletes an existing ulong (UInt64) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>If the deletion was successful</returns>
        public bool DeleteUInt64(string uniqueName)
        {
            return _handler.DeleteUInt64(uniqueName);
        }

        /// <summary>
        /// Deletes an existing ulong (UInt64) value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>If the deletion was successful</returns>
        public bool DeleteULong(string uniqueName)
        {
            return DeleteUInt64(uniqueName);
        }

        /// <summary>
        /// Deletes an existing float value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>If the deletion was successful</returns>
        public bool DeleteFloat(string uniqueName)
        {
            return _handler.DeleteFloat(uniqueName);
        }

        /// <summary>
        /// Deletes an existing double value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>If the deletion was successful</returns>
        public bool DeleteDouble(string uniqueName)
        {
            return _handler.DeleteDouble(uniqueName);
        }

        /// <summary>
        /// Deletes an existing string value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>If the deletion was successful</returns>
        public bool DeleteString(string uniqueName)
        {
            return _handler.DeleteString(uniqueName);
        }

        /// <summary>
        /// Deletes an existing byte[] value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>If the deletion was successful</returns>
        public bool DeleteByteArray(string uniqueName)
        {
            return _handler.DeleteByteArray(uniqueName);
        }

        /// <summary>
        /// Deletes an existing boolean value in the SSM File
        /// </summary>
        /// <param name="uniqueName">A unique name for the variable</param>
        /// <returns>If the deletion was successful</returns>
        public bool DeleteBoolean(string uniqueName)
        {
            return _handler.DeleteBoolean(uniqueName);
        }

        #endregion

        #region DataEntry

        /// <summary>
        /// Import a data entry into the SSM File
        /// </summary>
        /// <param name="dataEntry">Data entry</param>
        public void ImportDataEntry(DataEntry dataEntry)
        {
            _handler.ImportDataEntry(dataEntry);
        }

        /// <summary>
        /// Get an array of all MetaData values as DataEntries
        /// </summary>
        /// <returns>An array of DataEntries</returns>
        public DataEntry[] GetAllMetaData()
        {
            return _handler.GetAllMetaData();
        }

        /// <summary>
        /// Get an array of all short (Int16) values as DataEntries
        /// </summary>
        /// <returns>An array of DataEntries</returns>
        public DataEntry[] GetAllInt16()
        {
            return _handler.GetAllInt16();
        }

        /// <summary>
        /// Get an array of all short (Int16) values as DataEntries
        /// </summary>
        /// <returns>An array of DataEntries</returns>
        public DataEntry[] GetAllShort()
        {
            return GetAllShort();
        }

        /// <summary>
        /// Get an array of all int (Int32) values as DataEntries
        /// </summary>
        /// <returns>An array of DataEntries</returns>
        public DataEntry[] GetAllInt32()
        {
            return _handler.GetAllInt32();
        }

        /// <summary>
        /// Get an array of all int (Int32) values as DataEntries
        /// </summary>
        /// <returns>An array of DataEntries</returns>
        public DataEntry[] GetAllInt()
        {
            return GetAllInt32();
        }

        /// <summary>
        /// Get an array of all long (Int64) values as DataEntries
        /// </summary>
        /// <returns>An array of DataEntries</returns>
        public DataEntry[] GetAllInt64()
        {
            return _handler.GetAllInt64();
        }

        /// <summary>
        /// Get an array of all long (Int64) values as DataEntries
        /// </summary>
        /// <returns>An array of DataEntries</returns>
        public DataEntry[] GetAllLong()
        {
            return GetAllInt64();
        }

        /// <summary>
        /// Get an array of all ushort (UInt16) values as DataEntries
        /// </summary>
        /// <returns>An array of DataEntries</returns>
        public DataEntry[] GetAllUInt16()
        {
            return _handler.GetAllUInt16();
        }

        /// <summary>
        /// Get an array of all ushort (UInt16) values as DataEntries
        /// </summary>
        /// <returns>An array of DataEntries</returns>
        public DataEntry[] GetAllUShort()
        {
            return GetAllUInt16();
        }

        /// <summary>
        /// Get an array of all uint (UInt32) values as DataEntries
        /// </summary>
        /// <returns>An array of DataEntries</returns>
        public DataEntry[] GetAllUInt32()
        {
            return _handler.GetAllUInt32();
        }

        /// <summary>
        /// Get an array of all uint (UInt32) values as DataEntries
        /// </summary>
        /// <returns>An array of DataEntries</returns>
        public DataEntry[] GetAllUInt()
        {
            return GetAllUInt32();
        }

        /// <summary>
        /// Get an array of all ulong (UInt64) values as DataEntries
        /// </summary>
        /// <returns>An array of DataEntries</returns>
        public DataEntry[] GetAllUInt64()
        {
            return _handler.GetAllUInt64();
        }

        /// <summary>
        /// Get an array of all ulong (UInt64) values as DataEntries
        /// </summary>
        /// <returns>An array of DataEntries</returns>
        public DataEntry[] GetAllULong()
        {
            return GetAllUInt64();
        }

        /// <summary>
        /// Get an array of all float values as DataEntries
        /// </summary>
        /// <returns>An array of DataEntries</returns>
        public DataEntry[] GetAllFloat()
        {
            return _handler.GetAllFloat();
        }

        /// <summary>
        /// Get an array of all double values as DataEntries
        /// </summary>
        /// <returns>An array of DataEntries</returns>
        public DataEntry[] GetAllDouble()
        {
            return _handler.GetAllDouble();
        }

        /// <summary>
        /// Get an array of all string values as DataEntries
        /// </summary>
        /// <returns>An array of DataEntries</returns>
        public DataEntry[] GetAllString()
        {
            return _handler.GetAllString();
        }

        /// <summary>
        /// Get an array of all byte[] values as DataEntries
        /// </summary>
        /// <returns>An array of DataEntries</returns>
        public DataEntry[] GetAllByteArrays()
        {
            return _handler.GetAllByteArrays();
        }

        /// <summary>
        /// Get an array of all boolean values as DataEntries
        /// </summary>
        /// <returns>An array of DataEntries</returns>
        public DataEntry[] GetAllBooleans()
        {
            return _handler.GetAllBooleans();
        }

        /// <summary>
        /// Get an array of all values as DataEntries
        /// </summary>
        /// <returns>An array of DataEntries</returns>
        public DataEntry[] GetAllTypes()
        {
            return _handler.GetAllTypes();
        }

        #endregion
    }

    public class SSM_File
    {
        private string _settingsPath;
        private Mode _mode;
        private IMode _handler;

        /// <summary>
        /// Creates a new, or loads an existing SSM File
        /// </summary>
        /// <param name="settingsPath">Path of the SSM file</param>
        /// <param name="mode">Mode to use when creating an SSM file</param>
        public SSM_File(string settingsPath, Mode mode = Mode.Auto)
        {
            if (File.Exists(settingsPath))
            {
                _settingsPath = settingsPath;
                _mode = DetectMode();
            }
            else
            {
                _settingsPath = settingsPath;

                if (mode == Mode.Auto) _mode = Mode.SQLite;
                else _mode = mode;
            }
            SetHandler();
        }

        #region SSM File Internal Mode Handler

        /// <summary>
        /// Automatically detects the mode of the SSM File
        /// </summary>
        /// <returns>Mode of the SSM File</returns>
        internal Mode DetectMode()
        {
            if (Utilities.IsFileSQLiteDB(_settingsPath))
            {
                return Mode.SQLite;
            }
            else if (Utilities.IsFileXML(_settingsPath))
            {
                return Mode.XML;
            }
            else
            {
                Logging.Log(String.Format("SSM File format not supported!"), Severity.ERROR);
                throw new FileLoadException("Unsupported file format!");
            }
        }

        /// <summary>
        /// Sets the Mode Handler of the SSM File
        /// </summary>
        internal void SetHandler()
        {
            if (_mode == Mode.SQLite)
                _handler = new SQLite();
            else if (_mode == Mode.XML)
                _handler = new XML();
        }
        /// <summary>
        /// Gets the Mode Handler of the SSM File
        /// </summary>
        /// <returns>Mode Handler</returns>
        internal IMode GetHandler()
        {
            return _handler;
        }

        #endregion
        #region SSM File Getters/Setters

        /// <summary>
        /// Gets the file path of the SSM File
        /// </summary>
        /// <param name="fullPath">Whether to display the full filepath</param>
        /// <returns>SSM File filepath</returns>
        public string GetPath(bool fullPath = false)
        {
            if (fullPath) return Path.GetFullPath(_settingsPath);
            else return _settingsPath;
        }

        /// <summary>
        /// Gets the SSM File filename
        /// </summary>
        /// <param name="includeExtension">Whether to include the extention</param>
        /// <returns>SSM File filename</returns>
        public string GetFileName(bool includeExtension = true)
        {
            if (includeExtension) return Path.GetFileName(this.GetPath(true));
            else return Path.GetFileNameWithoutExtension(this.GetPath(true));
        }

        /// <summary>
        /// Gets the DateTime of when the SSM File was last modified
        /// </summary>
        /// <returns>DateTime of last modification</returns>
        public DateTime GetLastModified()
        {
            return File.GetLastWriteTimeUtc(GetPath());
        }

        /// <summary>
        /// Gets the Mode of the SSM File
        /// </summary>
        /// <returns>SSM File Mode</returns>
        public Mode GetMode()
        {
            return _mode;
        }

        #endregion
        #region SSM File Enums

        /// <summary>
        /// Mode of the SSM File
        /// </summary>
        public enum Mode
        {
            Auto,
            SQLite,
            XML
        };

        #endregion
    }
}
