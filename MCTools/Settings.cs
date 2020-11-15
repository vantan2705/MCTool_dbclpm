using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCTools
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
            Config cfg = new Config();
            String value = cfg.Get(Config.OPEN_FOLDER_AFTER_EXPORT, "Không");
            if (value == "Không")
            {
                cbbOpenAfterExport.SelectedIndex = 1;
            }
            else
            {
                cbbOpenAfterExport.SelectedIndex = 0;
            }
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Config cfg = new Config();
            cfg.Set(Config.OPEN_FOLDER_AFTER_EXPORT, cbbOpenAfterExport.SelectedItem);
            cfg.Save();
            this.Close();
        }
    }
}
