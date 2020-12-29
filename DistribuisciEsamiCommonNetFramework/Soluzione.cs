using System;
using System.Collections.Generic;

namespace DistribuisciEsamiCommon
{
    public class Soluzione
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

        public List<string> ToConsoleOutput(Esami esami)
        {
            List<string> r = new List<string>();

            List<string> ordine = GetOrdineDegliEsami();

            foreach (string x in ordine)
            {
                r.Add( x.ToString() + "\t" + esami.GetExam(x).cfu + "\t" + StampaData(this.dictionary[x]));
            }

            return r;
        }

        private static string StampaData(DateTime dateTime)
        {
            return dateTime.Year.ToString().PadLeft(4, '0') + "-" + dateTime.Month.ToString().PadLeft(2, '0') + "-" + dateTime.Day.ToString().PadLeft(2, '0');
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

        public Soluzione Clone()
        {
            Dictionary<string, DateTime> dictionarycopy = new Dictionary<string, DateTime>();
            foreach (var k in this.dictionary.Keys)
            {
                dictionarycopy[k] = dictionary[k];
            }
            return new Soluzione(dictionarycopy);
        }

        public void CalcolaPunteggio(Esami esami)
        {
            List<Tuple<DateTime, int>> datetimeInOrdine = GetDateTimeInOrdine(esami);
            List<double> r1 = new List<double>();
            for (int i = 0; i < datetimeInOrdine.Count - 1; i++)
            {
                double days = (datetimeInOrdine[i + 1].Item1 - datetimeInOrdine[i].Item1).TotalDays;
                double points = days * datetimeInOrdine[i + 1].Item2;
                r1.Add(points);
            }

            var variance = (decimal)Variance(r1.ToArray());

            decimal tot_days = GetSum(r1);

            value = variance / tot_days;
        }

        private static decimal GetSum(List<double> r1)
        {
            decimal r = 0;
            foreach (var d in r1)
            {
                r += (decimal)d;
            }

            return r;
        }

        private static double Variance(double[] nums)
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

        private static double GetAverage(double[] nums)
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

        private List<Tuple<DateTime, int>> GetDateTimeInOrdine(Esami esami)
        {
            List<Tuple<DateTime, int>> r = new List<Tuple<DateTime, int>>();
            foreach (string x in this.dictionary.Keys)
            {
                r.Add(new Tuple<DateTime, int>(this.dictionary[x], esami.GetExam(x).cfu));
            }

            r.Sort();

            return r;
        }
    }
}