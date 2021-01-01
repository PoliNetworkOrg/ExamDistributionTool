using System.Collections.Generic;

namespace DistribuisciEsamiCommon
{
    public class EsamiFromFile
    {
        private readonly List<string> jSON;
        private readonly Esami esami;

        public EsamiFromFile(List<string> jSON)
        {
            this.jSON = jSON;
        }

        public EsamiFromFile(Esami esami)
        {
            this.esami = esami;
        }

        public bool AlreadyContaisExams()
        {
            return this.esami != null;
        }

        public Esami GetExams()
        {
            return this.esami;
        }

        public List<string> GetLines()
        {
            return this.jSON;
        }
    }
}