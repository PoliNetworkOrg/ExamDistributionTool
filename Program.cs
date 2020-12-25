using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace DistribuisciEsami
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            var OPFD = new CommonOpenFileDialog();
            OPFD.Title = "Seleziona il file.";
            CommonFileDialogResult result = CommonFileDialogResult.None;
            while (result != CommonFileDialogResult.Ok) {
                result = OPFD.ShowDialog();
            }

            string file = null;
            try {
                file = File.ReadAllText(OPFD.FileName);
            }
            catch {
                ;
            }

            if (string.IsNullOrEmpty(file)) {
                Console.WriteLine("There was an error reading the file.");
                return;
            }
            else {
                DialogResult DR = MessageBox.Show("Se hai formattato il testo come specificato nel README, allora premi si, se invece l'hai copiato dalla pagina degli esami, premi no.", "Some Title", MessageBoxButtons.YesNo);
                if (DR == DialogResult.No) {
                    List<string> JSON = new List<string>();
                    JSON.Add("[");
                    string[] lines = File.ReadAllLines(OPFD.FileName);
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
                                dateline += "\"" + nextline.Substring(0, nextline.IndexOf(":")).Replace("/", "-") + "\",";
                                nextline = lines[Array.IndexOf(lines, nextline) + 1];
                            }
                            JSON.Add((dateline + "\"],").Replace("\",\"],", "\"],"));        //2lazy to fix it properly
                            var isNumeric = false;
                            int cfunum = 0;
                            string tmp = "";
                            while (isNumeric == false) {
                                tmp = Interaction.InputBox("Quanti CFU vale " + subjectname + "?", "CFU", "");
                                isNumeric = int.TryParse(tmp, out cfunum);
                                if (tmp == "")
                                    isNumeric = true;
                            }
                            if (tmp != "")
                                JSON.Add("\"cfu\":\"" + cfunum + "\"");
                            else
                                JSON.Add("\"cfu\":\"" + "\"");

                            JSON.Add("},");
                        }
                    }
                    JSON.Add("]");
                    JSON[JSON.Count - 2] = "}";    //Remove the comma from the last closed curly bracket
                    file = String.Join(Environment.NewLine, JSON);
                }
                Main2(file);
                return;
            }
        }

        public static Esami esami = null;

        private static void Main2(string file)
        {
            esami = new Esami(file);
            if (esami == null || esami.IsEmpty()) {
                Console.WriteLine("There are no exams");
                return;
            }

            List<Soluzione> soluzioni = GetSoluzioni();
            if (soluzioni == null || soluzioni.Count == 0) {
                Console.WriteLine("No solutions!");
                return;
            }

            for (int i = 0; i < soluzioni.Count; i++) {
                soluzioni[i].CalcolaPunteggio();
            }

            Punteggi punteggi = CalcolaPunteggi(soluzioni);

            ;
            int latest = 0;
            foreach (List<int> p in punteggi.rank) {
                foreach (var p2 in p) {
                    Console.WriteLine(soluzioni[p2].ToConsoleOutput());
                    Console.WriteLine(soluzioni[p2].value);
                    Console.WriteLine("\n");
                    latest = p2;
                }

                Console.WriteLine(".");
            }
            MessageBox.Show("La soluzione consigliata è:" + Environment.NewLine + soluzioni[latest].ToConsoleOutput() + Environment.NewLine + "Per vedere le altre soluzioni, consulta la console.");
        }

        private static Punteggi CalcolaPunteggi(List<Soluzione> soluzioni)
        {
            Punteggi punteggi = new Punteggi();
            for (int i = 0; i < soluzioni.Count; i++) {
                decimal value = soluzioni[i].value;
                if (!punteggi.punteggi.ContainsKey(value)) {
                    punteggi.punteggi[value] = new List<int>() { i };
                }
                else if (punteggi.punteggi[value] == null) {
                    punteggi.punteggi[value] = new List<int>() { i };
                }
                else {
                    punteggi.punteggi[value].Add(i);
                }
            }

            punteggi.CalcolaRank();

            return punteggi;
        }

        private static List<Soluzione> GetSoluzioni()
        {
            List<string> keys = esami.GetKeys();

            List<Soluzione> r = new List<Soluzione>();
            r.AddRange(GetSoluzioni2(0, keys, new Soluzione()));
            return r;
        }

        private static List<Soluzione> GetSoluzioni2(int v, List<string> keys, Soluzione soluzione)
        {
            if (v >= keys.Count) {
                return null;
            }
            List<DateTime> dateTimes = esami.GetDateTimes(keys[v]);
            List<Soluzione> r = new List<Soluzione>();
            foreach (var d in dateTimes) {
                Soluzione s1 = soluzione.Clone();
                s1.dictionary[keys[v]] = d;
                var r2 = GetSoluzioni2(v + 1, keys, s1);
                if (r2 == null || r2.Count == 0) {
                    r.Add(s1);
                }
                else {
                    r.AddRange(r2);
                }
            }
            return r;
        }
    }
}