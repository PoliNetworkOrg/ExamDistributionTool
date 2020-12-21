using System;
using System.Collections.Generic;

namespace DistribuisciEsami
{
    public class Esame
    {
        public string nome;
        public List<DateTime> dateTimes;

        public Esame(string nome, List<DateTime> date)
        {
            this.nome = nome;
            this.dateTimes = date;
        }
    }
}