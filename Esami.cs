using System;
using System.Collections.Generic;

namespace DistribuisciEsami
{
    internal class Esami
    {
        private readonly Dictionary<string, Esame> dictionary;

        public Esami()
        {
            dictionary = new Dictionary<string, Esame>();
        }

        public Esami(string file)
        {
            dictionary = new Dictionary<string, Esame>();
            var jObject = Newtonsoft.Json.JsonConvert.DeserializeObject(file);

            if (jObject is Newtonsoft.Json.Linq.JArray ja)
            {
                var jac = ja.Children();
                foreach (var x in jac)
                {
                    Aggiungi(x);
                }
            }
        }

        private void Aggiungi(Newtonsoft.Json.Linq.JToken x)
        {
            string nome = GetNomeFromJson(x);
            List<DateTime> date = GetDateTimesFromJson(x);

            Esame esame = new Esame(nome, date);
            this.dictionary[nome] = esame;
        }

        private List<DateTime> GetDateTimesFromJson(Newtonsoft.Json.Linq.JToken x)
        {
            foreach (var x2 in x.Children())
            {
                if (x2 is Newtonsoft.Json.Linq.JProperty x3)
                {
                    if (x3.Name == "date")
                    {
                        var x4 = x3.Value;

                        if (x4 is Newtonsoft.Json.Linq.JArray x5)
                        {
                            List<DateTime> r = new List<DateTime>();
                            foreach (var x6 in x5.Children())
                            {
                                if (x6 is Newtonsoft.Json.Linq.JValue x7)
                                {
                                    var x8 = x7.Value.ToString();
                                    var x9 = x8.Split("-");
                                    int anno = Convert.ToInt32(x9[0]);
                                    int mese = Convert.ToInt32(x9[1]);
                                    int giorno = Convert.ToInt32(x9[2]);
                                    DateTime dt = new DateTime(anno, mese, giorno);
                                    r.Add(dt);
                                }
                            }

                            return r;
                        }
                    }
                }
            }
            return null;
        }

        private string GetNomeFromJson(Newtonsoft.Json.Linq.JToken x)
        {
            foreach (var x2 in x.Children())
            {
                if (x2 is Newtonsoft.Json.Linq.JProperty x3)
                {
                    if (x3.Name == "name")
                    {
                        return x3.Value.ToString();
                    }
                }
            }
            return null;
        }

        internal void Add(Esame esame)
        {
            dictionary[esame.nome] = esame;
        }

        internal List<string> GetKeys()
        {
            List<string> r = new List<string>();
            foreach (var x in dictionary.Keys)
            {
                r.Add(x);
            }
            return r;
        }

        internal List<DateTime> GetDateTimes(string v)
        {
            return this.dictionary[v].dateTimes;
        }

        internal bool IsEmpty()
        {
            return this.dictionary == null || this.dictionary.Keys.Count == 0;
        }
    }
}