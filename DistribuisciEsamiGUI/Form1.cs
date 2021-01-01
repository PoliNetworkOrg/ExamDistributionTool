using DistribuisciEsamiCommon;
using System;
using System.Collections;
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
            listView1.Columns[0].Width = listView1.Width / 3 * 2;
            listView1.Columns[1].Width = listView1.Width - listView1.Columns[0].Width - 4;

            SolutionsView.Columns[0].Width = SolutionsView.Width - SolutionsView.Width / 6 - SolutionsView.Width / 4 - 4 - SystemInformation.VerticalScrollBarWidth;
            SolutionsView.Columns[1].Width = SolutionsView.Width / 6;
            SolutionsView.Columns[2].Width = SolutionsView.Width / 4;
            SolutionsView.ListViewItemSorter = new WeightSort();
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

                esami = GetEsamiFromFile(filecontent, file);
                if (esami == null || esami.GetEsami() == null || esami.GetEsami().Count == 0)
                {
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

        private Esami GetEsamiFromFile(string filecontent, string file)
        {
            Esami esami = new Esami();

            EsamiFromFile obj = esami.CheckText(filecontent, File.ReadAllLines(file));
            if (obj.AlreadyContaisExams())
                return obj.GetExams();

            List<string> Lines = obj.GetLines();
            string plchlind = "[CFUNUM-PLACEHOLDER-";

            InputForm inpFrm = new InputForm();
            for (int i = 0; i < Lines.Count; i++)
            {
                string x = Lines[i];
                if (x.Contains(plchlind))
                {
                    string subjectname = x.Substring(x.IndexOf(plchlind) + plchlind.Length, x.IndexOf("]\"") - x.IndexOf(plchlind) - plchlind.Length);
                    inpFrm.Label1.Text = "How many CFUs is " + Environment.NewLine + subjectname + " worth?";
                    inpFrm.InputText.Text = "";
                    inpFrm.ShowDialog();
                    Lines[i] = x.Replace(plchlind + subjectname + "]", inpFrm.InputText.Text);
                }
            }

            try
            {
                esami = new Esami(String.Join(Environment.NewLine, Lines));
            }
            catch
            {
                ;
            }

            if (esami == null || esami.GetEsami() == null || esami.GetEsami().Count == 0)
            {
                Console.WriteLine("No exams found in that file. Is it formatted correctly?");
                return null;
            }

            return esami;
        }

        private void MostraSoluzione(RispostaCompleta rispostaCompleta)
        {
            listView1.Items.Clear();
            var esami2 = esami.GetEsami();
            foreach (string x in esami2.Keys)
            {
                string s = "" + esami.GetExam(x).ToStringListBoxGUI();

                ListViewItem newItem = new ListViewItem(s.Substring(0, s.IndexOf("\t")));
                newItem.SubItems.Add(s.Substring(s.IndexOf("\t") + 1, s.Length - s.IndexOf("\t") - 1));
                listView1.Items.Add(newItem);
            }

            SolutionsView.Items.Clear();

            ListViewItem emptyItem = new ListViewItem("");
            for (int i = 0; i < 2; i++)
                emptyItem.SubItems.Add("");

            foreach (List<int> p in rispostaCompleta.punteggi.rank)
            {
                foreach (var p2 in p)
                {
                    foreach (string x in rispostaCompleta.soluzioni[p2].ToConsoleOutput(esami))
                    {
                        string[] exams = x.Split('\t');
                        ListViewItem newItem = new ListViewItem(exams[0]);

                        foreach (string y in exams)
                            if (y != exams[0])
                                newItem.SubItems.Add(y);

                        newItem.SubItems.Add(rispostaCompleta.soluzioni[p2].value.ToString());
                        SolutionsView.Items.Add(newItem);
                    }
                    ListViewItem NewEmpty = (ListViewItem)emptyItem.Clone();
                    NewEmpty.SubItems.Add(rispostaCompleta.soluzioni[p2].value.ToString()); //Ensure that it has the same weight as the other items
                    SolutionsView.Items.Add(NewEmpty);    //Add an empty separator item.
                }
            }
        }
    }

    internal class WeightSort : IComparer
    {
        public int Compare(object x, object y)
        {
            float z = float.Parse(((ListViewItem)x).SubItems[3].Text);
            float w = float.Parse(((ListViewItem)y).SubItems[3].Text);

            return w.CompareTo(z);
        }
    }
}