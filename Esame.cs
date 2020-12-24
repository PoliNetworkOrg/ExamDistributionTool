using System;
using System.Collections.Generic;

namespace DistribuisciEsami
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
    }
}