using System.Collections.Generic;
using System.Linq;

namespace CamelUpCalculator
{
    public class Bets
    {
        private List<LegBet> _legBets;
        private List<EndBet> _endBets;

        public Bets()
        {
            _legBets = new List<LegBet>();
            _endBets = new List<EndBet>();
        }

        public void Reset()
        {
            _legBets.Clear();
            _endBets.Clear();
        }

        public bool AddLegBet(LegBet legBet)
        {
            if (_legBets.Any(x => x.Camel == legBet.Camel && x.Bet == legBet.Bet))
            {
                return false;
            }

            _legBets.Add(legBet);
            _legBets.Sort((x,y) => (-x.Bet).CompareTo(-y.Bet));
            return true;
        }

        public void ClearLegBets()
        {
            _legBets.Clear();
        }

        public List<LegBet> GetLegBets()
        {
            return _legBets;
        }

        public bool AddEndBet(EndBet endBet)
        {
            if (_endBets.Any(x => x.Camel == endBet.Camel))
            {
                return false;
            }

            _endBets.Add(endBet);
            _endBets.Sort((x, y) => x.Place.CompareTo(y.Place));
            return true;
        }

        public List<EndBet> GetEndBets()
        {
            return _endBets;
        }
    }
}
