﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DistribuisciEsami
{
    internal class Punteggi
    {
        public Dictionary<decimal, List<int>> punteggi;
        public List<List<int>> rank;

        public Punteggi()
        {
            this.punteggi = new Dictionary<decimal, List<int>>();
        }

        internal void CalcolaRank()
        {
            rank = new List<List<int>>();
            List<decimal> rankValue = GetRankValue();
            foreach (var i in rankValue)
            {
                rank.Add(punteggi[i]);
            }
        }

        private List<decimal> GetRankValue()
        {
            List<decimal> r = new List<decimal>();
            foreach (var i in punteggi.Keys)
            {
                r.Add(i);
            }

            var r2 = r.ToArray();
            Array.Sort<decimal>(r2, new Comparison<decimal>(
                     (i1, i2) => i2.CompareTo(i1)));

            return r2.ToList();
        }
    }
}