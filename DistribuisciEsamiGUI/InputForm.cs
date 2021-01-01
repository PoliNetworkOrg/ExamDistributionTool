using System;
using System.Windows.Forms;

namespace DistribuisciEsamiGUI
{
    public partial class InputForm : Form
    {
        public InputForm()
        {
            InitializeComponent();
        }

        private void InputForm_Load(object sender, EventArgs e)
        {
        }

        private void OK_Click(object sender, EventArgs e)
        {
            var isNumeric = int.TryParse(InputText.Text, out _);
            if (!isNumeric && InputText.Text != "")
                MessageBox.Show("Please input a number.");
            else
                InputForm.ActiveForm.Close();
        }

        private void InputForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var isNumeric = int.TryParse(InputText.Text, out _);
            if (!isNumeric)
                InputText.Text = "";
        }
    }
}