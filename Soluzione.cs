using System;
using System.Collections.Generic;

namespace DistribuisciEsami
{
    internal class Soluzione
    {
        public Dictionary<string, DateTime> dictionary;
        public decimal value;

        public Soluzione()
        {
            this.dictionary = new Dictionary<string, DateTime>();
            this.value = -1;
        }

        public Soluzione(Dictionary<string, DateTime> dictionarycopy)
        {
            this.dictionary = dictionarycopy;
        }

        internal string ToConsoleOutput()
        {
            string r = "";

            List<string> ordine = GetOrdineDegliEsami();

            foreach (string x in ordine)
            {
                r += x.ToString() + "\t" + this.dictionary[x].ToString();
                r += "\n";
            }

            r = r[0..^1];
            return r;
        }

        private List<string> GetOrdineDegliEsami()
        {
            List<Tuple<string, DateTime>> tuples = new List<Tuple<string, DateTime>>();
            foreach (var x in this.dictionary.Keys)
            {
                tuples.Add(new Tuple<string, DateTime>(x, this.dictionary[x]));
            }

            for (int i = 0; i < tuples.Count; i++)
            {
                for (int j = 0; j < tuples.Count - 1; j++)
                {
                    if (tuples[j].Item2 > tuples[j + 1].Item2)
                    {
                        var t = tuples[j];
                        tuples[j] = tuples[j + 1];
                        tuples[j + 1] = t;
                    }
                }
            }

            List<string> r = new List<string>();
            foreach (var s in tuples)
            {
                r.Add(s.Item1);
            }

            return r;
        }

        internal Soluzione Clone()
        {
            Dictionary<string, DateTime> dictionarycopy = new Dictionary<string, DateTime>();
            foreach (var k in this.dictionary.Keys)
            {
                dictionarycopy[k] = dictionary[k];
            }
            return new Soluzione(dictionarycopy);
        }

        internal void CalcolaPunteggio()
        {
            List<DateTime> datetimeInOrdine = GetDateTimeInOrdine();
            List<int> r1 = new List<int>();
            for (int i = 0; i < datetimeInOrdine.Count - 1; i++)
            {
                var days = (datetimeInOrdine[i + 1] - datetimeInOrdine[i]).TotalDays;
                r1.Add((int)days);
            }

            var variance = (decimal)Variance(r1.ToArray());

            decimal tot_days = (decimal)(datetimeInOrdine[datetimeInOrdine.Count - 1] - datetimeInOrdine[0]).TotalDays;

            value = variance / tot_days;
        }

        private double Variance(int[] nums)
        {
            if (nums.Length > 1)
            {
                // Get the average of the values

                double avg = GetAverage(nums);

                // Now figure out how far each point is from the mean

                // So we subtract from the number the average

                // Then raise it to the power of 2

                double sumOfSquares = 0.0;

                foreach (int num in nums)
                {
                    sumOfSquares += Math.Pow((num - avg), 2.0);
                }

                // Finally divide it by n - 1 (for standard deviation variance)
                // Or use length without subtracting one ( for population standard deviation variance)

                return sumOfSquares / (double)(nums.Length - 1);
            }
            else { return 0.0; }
        }

        private double GetAverage(int[] nums)
        {
            int sum = 0;

            if (nums.Length > 1)
            {
                // Sum up the values

                foreach (int num in nums)
                {
                    sum += num;
                }

                // Divide by the number of values

                return sum / (double)nums.Length;
            }
            else { return (double)nums[0]; }
        }

        private List<DateTime> GetDateTimeInOrdine()
        {
            List<DateTime> r = new List<DateTime>();
            foreach (var x in this.dictionary.Keys)
            {
                r.Add(this.dictionary[x]);
            }

            r.Sort();

            return r;
        }
    }
}