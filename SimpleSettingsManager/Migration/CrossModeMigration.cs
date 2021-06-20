using SimpleSettingsManager.Data;
using System;

namespace SimpleSettingsManager.Migration
{
    public class CrossModeMigration
    {
        private SSM_File _importSsmFile, _exportSsmFile;

        /// <summary>
        /// Migrates an SSM File of one mode to a new SSM File of another mode
        /// </summary>
        /// <param name="importSsmFile">SSM File to convert/migrate</param>
        /// <param name="exportSsmFile">New SSM File to write to</param>
        public CrossModeMigration(SSM_File importSsmFile, SSM_File exportSsmFile)
        {
            if (importSsmFile.GetMode() != exportSsmFile.GetMode())
            {
                _importSsmFile = importSsmFile;
                _exportSsmFile = exportSsmFile;
            }
            else
            {
                Logging.Log("You cannot migrate between the same SSM modes with CrossModeMigration!", Severity.ERROR);
                throw new Exception("Error: You cannot migrate between the same SSM modes with CrossModeMigration!");
            }
        }

        /// <summary>
        /// Begins the migration process
        /// </summary>
        public void Migrate()
        {
            try
            {
                SSM importSsm = new SSM(_importSsmFile);
                SSM exportSsm = new SSM(_exportSsmFile);

                importSsm.Open();
                exportSsm.Open();

                DataEntry[] dataEntries = importSsm.GetAllTypes();
                Console.WriteLine("Total Entries: {0}", dataEntries.Length);

                importSsm.Close();

                foreach (DataEntry data in dataEntries)
                {
                    if (data != null)
                    {
                        Console.WriteLine("Importing '{0}'", data.GetVariableName());
                        exportSsm.ImportDataEntry(data);
                    }
                }
                exportSsm.UpdateCrossModeMigrationStatus();

                exportSsm.Close();
            }
            catch (Exception e)
            {
                Logging.Log("An unexpected error occured within CrossModeMigration!", Severity.ERROR);
                throw new Exception("Error: An unexpected error occured within CrossModeMigration! Error: " + e);
            }
        }
    }
}
