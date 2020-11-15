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
    public partial class EditPersonalInfo : Form
    {
        public EditPersonalInfo()
        {
            InitializeComponent();
            Config cfg = new Config();
            String school = cfg.Get(Config.SCHOOL, "");
            String room = cfg.Get(Config.ROOM, "");
            tbSchool.Text = school;
            tbRoom.Text = room;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Config cfg = new Config();
            cfg.Set(Config.SCHOOL, tbSchool.Text.ToString());
            cfg.Set(Config.ROOM, tbRoom.Text.ToString());
            cfg.Save();
            this.Close();
        }
    }
}
