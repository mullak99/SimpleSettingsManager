using SimpleSettingsManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSettingsManager.Migration
{
    public class CrossModeMigration
    {
        private SSM_File _importSsmFile, _exportSsmFile;

        public CrossModeMigration(SSM_File importSsmFile, SSM_File exportSsmFile)
        {
            _importSsmFile = importSsmFile;
            _exportSsmFile = exportSsmFile;
        }

        public void Migrate()
        {
            SSM importSsm = new SSM(_importSsmFile);
            SSM exportSsm = new SSM(_exportSsmFile);

            importSsm.Open();
            exportSsm.Open();

            DataEntry[] dataEntries = importSsm.GetAllTypes();
            Console.WriteLine("Total Entries: {0}", dataEntries.Length);

            importSsm.Close();

            exportSsm.UpdateMigrationStatus();

            foreach (DataEntry data in dataEntries)
            {
                Console.WriteLine("Importing '{0}'", data.GetVariableName());
                exportSsm.ImportDataEntry(data);
            }

            exportSsm.Close();
        }
    }
}
