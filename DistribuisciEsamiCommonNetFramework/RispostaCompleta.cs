using System;
using System.Collections.Generic;

namespace DistribuisciEsamiCommon
{
    public class RispostaCompleta
    {
        public List<Soluzione> soluzioni;
        public Punteggi punteggi;

        public RispostaCompleta(List<Soluzione> soluzioni, Punteggi punteggi)
        {
            this.soluzioni = soluzioni;
            this.punteggi = punteggi;
        }
        private static List<Soluzione> GetSoluzioni(Esami esami)
        {
            List<string> keys = esami.GetKeys();

            List<Soluzione> r = new List<Soluzione>();
            r.AddRange(GetSoluzioni2(0, keys, new Soluzione(),esami));
            return r;
        }

        private static Punteggi CalcolaPunteggi(List<Soluzione> soluzioni)
        {
            Punteggi punteggi = new Punteggi();
            for (int i = 0; i < soluzioni.Count; i++)
            {
                decimal value = soluzioni[i].value;
                if (!punteggi.punteggi.ContainsKey(value))
                {
                    punteggi.punteggi[value] = new List<int>() { i };
                }
                else if (punteggi.punteggi[value] == null)
                {
                    punteggi.punteggi[value] = new List<int>() { i };
                }
                else
                {
                    punteggi.punteggi[value].Add(i);
                }
            }

            punteggi.CalcolaRank();

            return punteggi;
        }


        private static List<Soluzione> GetSoluzioni2(int v, List<string> keys, Soluzione soluzione, Esami esami)
        {
            if (v >= keys.Count)
            {
                return null;
            }
            List<DateTime> dateTimes = esami.GetDateTimes(keys[v]);
            List<Soluzione> r = new List<Soluzione>();
            foreach (var d in dateTimes)
            {
                Soluzione s1 = soluzione.Clone();
                s1.dictionary[keys[v]] = d;
                var r2 = GetSoluzioni2(v + 1, keys, s1, esami);
                if (r2 == null || r2.Count == 0)
                {
                    r.Add(s1);
                }
                else
                {
                    r.AddRange(r2);
                }
            }
            return r;
        }


        public static Tuple<DistribuisciEsamiCommon.RispostaCompleta, string> CalcolaRisposta(Esami esami)
        {
            if (esami == null || esami.IsEmpty())
            {
                string s1 = "There are no exams";
                return new Tuple<RispostaCompleta, string>(null, s1);
            }

            List<Soluzione> soluzioni = GetSoluzioni(esami);
            if (soluzioni == null || soluzioni.Count == 0)
            {
                string s2 = "No solutions!";
                return new Tuple<RispostaCompleta, string>(null, s2);
            }

            for (int i = 0; i < soluzioni.Count; i++)
            {
                soluzioni[i].CalcolaPunteggio(esami);
            }

            Punteggi punteggi = CalcolaPunteggi(soluzioni);
            return new Tuple<RispostaCompleta, string>(new RispostaCompleta(soluzioni, punteggi), null);
        }
    }
}