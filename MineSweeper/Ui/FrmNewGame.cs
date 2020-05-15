using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MineSweeper.Ui
{
    public partial class FrmNewGame : Form
    {
        private FrmMain mParentForm = null;

        public FrmNewGame(FrmMain frmMain)
        {
            InitializeComponent();

            mParentForm = frmMain;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            int width = 0;
            int height = 0;
            int bomb = 0;

            if (rbrnEasy.Checked) {
                width = 10;
                height = 10;
                bomb = 10;
            } else if (rbrnNormal.Checked) {
                width = 30;
                height = 20;
                bomb = 90;
            } else {
                width = 70;
                height = 35;
                bomb = 500;
            }

            mParentForm.SetGameOption(width, height, bomb);
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
