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
    public partial class FrmEndResult : Form
    {
        private string mMessage = string.Empty;

        public FrmEndResult(string message)
        {
            InitializeComponent();

            mMessage = message;
        }

        private void FrmEndResult_Load(object sender, EventArgs e)
        {
            lbMessage.Text = mMessage;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
