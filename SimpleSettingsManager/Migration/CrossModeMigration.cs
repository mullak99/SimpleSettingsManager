using SimpleSettingsManager.Data;
using System;

namespace SimpleSettingsManager.Migration
{
    public class CrossModeMigration
    {
        private SSM_File _importSsmFile, _exportSsmFile;

        public CrossModeMigration(SSM_File importSsmFile, SSM_File exportSsmFile)
        {
            if (importSsmFile.GetMode() != exportSsmFile.GetMode())
            {
                _importSsmFile = importSsmFile;
                _exportSsmFile = exportSsmFile;
            }
            else throw new Exception("Error: You can not migrate between the same SSM modes with CrossModeMigration!");
        }

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
                    Console.WriteLine("Importing '{0}'", data.GetVariableName());
                    exportSsm.ImportDataEntry(data);
                }
                exportSsm.UpdateCrossModeMigrationStatus();

                exportSsm.Close();
            }
            catch
            {
                throw new Exception("Error: An unexpected error occured within CrossModeMigration!");
            }
        }
    }
}
