using DistribuisciEsamiCommon;
using System;
using System.Collections.Generic;
using System.IO;

namespace DistribuisciEsami
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string file = null;
            if (args.Length > 0) {
                file = args[0];
            }
                
            else {
                Console.WriteLine("No filepath was passed as argument. Please input the path to the file.");
                file = Console.ReadLine().Replace("\"","");
            }


                string filecontent = null;
                try
                {
                    filecontent = File.ReadAllText(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (string.IsNullOrEmpty(filecontent))
                {
                    Console.WriteLine("There was an error reading the file.");
                    Console.ReadLine();
                    return;
                }
                else
                {
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
                                Console.WriteLine("How many CFUs is " + subjectname + " worth?");
                                tmp = Console.ReadLine();
                                while (tmp != "" && !int.TryParse(tmp, out cfunum)) {
                                    Console.WriteLine("The only accepted values are numbers or nothing at all. Please try again.");
                                    Console.WriteLine("How many CFUs is " + Environment.NewLine + subjectname + " worth?");
                                    tmp = Console.ReadLine();
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
                        filecontent = String.Join(Environment.NewLine, JSON);

                        try {
                            esami = new Esami(filecontent);
                        }
                        catch {
                            ;
                        }

                        if (esami == null || esami.GetEsami() == null || esami.GetEsami().Count == 0) {
                            Console.WriteLine("No exams found in that file. Is it formatted correctly?");
                            return;
                        }
                    }

                    Console.WriteLine();
                    Console.WriteLine("Possible solutions:");
                    Tuple<RispostaCompleta, string> punteggi = DistribuisciEsamiCommon.RispostaCompleta.CalcolaRisposta(esami);
                    if (punteggi.Item1 != null)
                    {
                        MostraEsito(punteggi.Item1);
                    }
                    else
                    {
                        Console.WriteLine(punteggi.Item2);
                    }

                Console.ReadLine();
                return;
                }
            }


        private static void MostraEsito(RispostaCompleta punteggi)
        {
            foreach (List<int> p in punteggi.punteggi.rank)
            {
                foreach (var p2 in p)
                {
                    string s2 = EsitoCLI_Esami(punteggi.soluzioni[p2].ToConsoleOutput(esami));
                    Console.WriteLine(s2);
                    Console.WriteLine(punteggi.soluzioni[p2].value);
                    Console.WriteLine("\n");
                }

                Console.WriteLine(".");
            }
        }

        private static string EsitoCLI_Esami(List<string> lists)
        {
            if (lists == null || lists.Count == 0)
                return "";

            string r = "";

            foreach (string x in lists)
            {
                r += x;
                r += "\n";
            }

            r = r.Remove(r.Length - 1);
            return r;
        }

        /*
        private static void Main3(string[] args)
        {
            string file = null;
            var OPFD = new CommonOpenFileDialog();
            if (args.Length > 0) {
                file = args[0];
            }
            else {
                OPFD.Title = "Select the file.";
                CommonFileDialogResult result = CommonFileDialogResult.None;
                while (result != CommonFileDialogResult.Ok) {
                    result = OPFD.ShowDialog();
                }
                    file = OPFD.FileName;
            }

            try {
                file = File.ReadAllText(file);
            }
            catch {
                ;
            }

            if (string.IsNullOrEmpty(file)) {
                Console.WriteLine("There was an error reading the file.");
                return;
            }
            else if (args.Length == 0) {
                DialogResult DR = MessageBox.Show("If you formatted the text file according to the README, then click yes, if you instead copied it from the exam page, click no.", "Import?", MessageBoxButtons.YesNo);
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
                                tmp = Interaction.InputBox("How many CFUs is " + subjectname + " worth?", "CFU", "");
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
            }
            Main2(file,(args.Length == 0));
            return;
        }
        */

        public static Esami esami = null;
    }
}