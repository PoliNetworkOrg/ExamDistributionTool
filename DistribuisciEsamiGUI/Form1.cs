using DistribuisciEsamiCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace DistribuisciEsamiGUI
{
    public partial class Form1 : Form
    {
        public static DistribuisciEsamiCommon.Esami esami = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var r = openFileDialog.ShowDialog();
            if (r == DialogResult.OK)
            {
                string file = openFileDialog.FileName;
                string filecontent = null;
                try
                {
                    filecontent = File.ReadAllText(file);
                }
                catch
                {
                    ;
                }

                if (string.IsNullOrEmpty(filecontent))
                {
                    MessageBox.Show("Error while reading the file selected.");
                    return;
                }

                esami = null;
                try
                {
                    esami = new DistribuisciEsamiCommon.Esami(filecontent);
                }
                catch
                {
                    ;
                }

                if (esami == null || esami.GetEsami() == null || esami.GetEsami().Count == 0)
                {
                    MessageBox.Show("No exams found in that file. Is it formatted correctly?");
                    return;
                }
                var rispostaCompleta = DistribuisciEsamiCommon.RispostaCompleta.CalcolaRisposta(esami);
                if (rispostaCompleta.Item1 != null)
                {
                    MostraSoluzione(rispostaCompleta.Item1);
                }
                else
                {
                    MessageBox.Show(rispostaCompleta.Item2);
                }
            }
        }

        private void MostraSoluzione(RispostaCompleta rispostaCompleta)
        {
            listBox1.Items.Clear();
            var esami2 = esami.GetEsami();
            listBox1.Items.Add("[NAME]\t\t[CFU]");
            foreach (string x in esami2.Keys)
            {
                string s = "" + esami.GetExam(x).ToStringListBoxGUI();
                listBox1.Items.Add(s);
            }

            textBox1.Text = "";
            List<string> lines = new List<string>();
            foreach (List<int> p in rispostaCompleta.punteggi.rank)
            {
                foreach (var p2 in p)
                {
                    lines.AddRange(rispostaCompleta.soluzioni[p2].ToConsoleOutput(esami));
                    lines.Add(rispostaCompleta.soluzioni[p2].value.ToString());
                    lines.Add("\n");
                }

                lines.Add(".");
            }

            textBox1.Lines = lines.ToArray();
        }
    }
}