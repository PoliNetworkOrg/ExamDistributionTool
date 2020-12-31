using DistribuisciEsamiCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
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
            listView1.Columns[1].Width = listView1.Width - listView1.Columns[0].Width -4;

            SolutionsView.Columns[0].Width = SolutionsView.Width - SolutionsView.Width / 6 - SolutionsView.Width / 4 -4 - SystemInformation.VerticalScrollBarWidth;
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

                esami = null;
                try
                {
                    esami = new Esami(filecontent);
                }
                catch
                {
                    ;
                }

                if (esami == null || esami.GetEsami() == null || esami.GetEsami().Count == 0)
                {
                    //The file isn't formatted like the readme says it should be. Let's see if it's copied from the exams page.
                    List<string> JSON = new List<string>();
                    JSON.Add("[");
                    string[] lines = File.ReadAllLines(file);
                    foreach (string line in lines) {
                        if (line.Contains("-")) {
                            if (Array.IndexOf(lines, line) + 1 == lines.Length)
                                break; //This line is a new subject, but there's no lines after this so there can't be any dates.
                            else {
                                if (lines[Array.IndexOf(lines, line) + 1].IndexOf("/") != 2)
                                    continue; //Is the next line NOT a date? Then keep going. This subject has no listed dates.
                            }
                            JSON.Add("{");
                            string subjectname = line.Substring(0, line.IndexOf("-")).Trim();
                            JSON.Add("\"name\":\"" + subjectname + "\",");
                            string dateline = "\"date\":[";
                            string nextline = lines[Array.IndexOf(lines, line) + 1];
                            while (nextline.IndexOf("/") == 2) {
                                //So long as the next line is a date
                                dateline += "\"" + Convert.ToDateTime(nextline.Substring(0, nextline.IndexOf(":")).Replace("/", "-")).ToString("yyyy-MM-dd") + "\",";
                                nextline = lines[Array.IndexOf(lines, nextline) + 1];
                                
                            }
                            dateline = (dateline + "\"],").Replace("\",\"],", "\"],");  //2lazy to fix it properly
                            JSON.Add(dateline);        
                            //MessageBox.Show(dateline);
                            int cfunum = 0;
                            string tmp = "";
                                InputForm inpFrm = new InputForm();
                                inpFrm.Label1.Text = "How many CFUs is " + Environment.NewLine + subjectname + " worth?";
                                inpFrm.ShowDialog();
                                tmp = inpFrm.InputText.Text;
                                int.TryParse(tmp, out cfunum);
                            if (tmp != "")
                                JSON.Add("\"cfu\":\"" + cfunum + "\"");
                            else
                                JSON.Add("\"cfu\":\"" + "\"");

                            JSON.Add("},");
                        }
                    }
                    JSON.Add("]");
                    JSON[JSON.Count - 2] = "}";    //Remove the comma from the last closed curly bracket
                    filecontent = String.Join(Environment.NewLine, JSON);

                    try {
                        esami = new Esami(filecontent);
                    }
                    catch {
                        ;
                    }

                    if (esami == null || esami.GetEsami() == null || esami.GetEsami().Count == 0) {
                        MessageBox.Show("No exams found in that file. Is it formatted correctly?");
                        return;
                    }   
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
            listView1.Items.Clear();
            var esami2 = esami.GetEsami();
            foreach (string x in esami2.Keys)
            {
                string s = "" + esami.GetExam(x).ToStringListBoxGUI();

                ListViewItem newItem = new ListViewItem(s.Substring(0,s.IndexOf("\t")));
                newItem.SubItems.Add(s.Substring(s.IndexOf("\t")+1, s.Length - s.IndexOf("\t")-1));
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
                    foreach (string x in rispostaCompleta.soluzioni[p2].ToConsoleOutput(esami)) {
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

            //textBox1.Lines = lines.ToArray();
        }
    }
    class WeightSort : IComparer
    {
        public int Compare(object x, object y)
        {
            float z = float.Parse(((ListViewItem)x).SubItems[3].Text);
            float w = float.Parse(((ListViewItem)y).SubItems[3].Text);

            /*MessageBox.Show(((ListViewItem)x).Text + " - " + ((ListViewItem)x).SubItems.Count.ToString());
            MessageBox.Show(((ListViewItem)y).Text + " - " + ((ListViewItem)y).SubItems.Count.ToString());

            if (((ListViewItem)x).SubItems.Count < 4)
                z = 9999999999;
            else if (!float.TryParse(((ListViewItem)x).SubItems[2].Text, out z))
                z = 9999999999;


            if (!float.TryParse(((ListViewItem)y).SubItems[2].Text, out w))
                w = 9999999999;*/

            return w.CompareTo(z);
        }
    }
}

