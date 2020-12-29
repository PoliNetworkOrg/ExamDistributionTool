using System;
using System.Collections.Generic;

namespace DistribuisciEsamiCommon
{
    public class Esame
    {
        public string nome;
        public List<DateTime> dateTimes;
        public int cfu;

        public Esame(string nome, List<DateTime> date, int cfu)
        {
            this.nome = nome;
            this.dateTimes = date;
            this.cfu = cfu;
        }

        public string ToStringListBoxGUI()
        {
            return this.nome + "\t\t" + this.cfu.ToString();
        }
    }
}