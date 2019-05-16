using System;

namespace SimpleSettingsManager.Migration
{
    public class OldVersionMigration
    {
        /// <summary>
        /// Migrates an Old-Format SSM File to a new Updated-Format SSM File
        /// </summary>
        /// <param name="importSsmFile">SSM File to convert/migrate</param>
        /// <param name="exportSsmFile">New SSM File to write to</param>
        public OldVersionMigration(SSM_File importSsmFile, SSM_File exportSsmFile)
        {
            Logging.Log(String.Format("OldVersionMigration is not implemented in this version."), Severity.ERROR);
            throw new NotImplementedException("OldVersionMigration is not implemented in this version.");
        }
    }
}
