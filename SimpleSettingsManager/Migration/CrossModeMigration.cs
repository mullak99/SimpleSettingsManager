using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSettingsManager.Migration
{
    class CrossModeMigration
    {
        private SSM_File _ssmFile;
        private SSM_File.Mode _ImportAsMode, _ExportAsMode;

        public CrossModeMigration(SSM_File ssmFile, SSM_File.Mode exportAsMode)
        {
            _ssmFile = ssmFile;
            _ImportAsMode = _ssmFile.GetMode();
            _ExportAsMode = exportAsMode;
        }

        public void Start()
        {

        }
    }
}
