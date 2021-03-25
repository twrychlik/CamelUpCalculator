using System.Collections.Generic;

namespace CamelUpCalculator
{
    public class Dice
    {
        private readonly List<int> _allCamels;

        public Dice()
        {
            _allCamels = new List<int>();
            for (int i = 0; i < Constants.NumberOfCamels; i++)
            {
                _allCamels.Add(i);
            }

            CamelsLeft = _allCamels.CloneList();
        }
        public List<int> CamelsLeft { get; private set; }

        public void Reset()
        {
            CamelsLeft = _allCamels.CloneList();
        }

        public Roll[] GetRollSet()
        {
            var rollSet = new Roll[CamelsLeft.Count];
            var colours = CamelsLeft.Shuffle();
            for (int i = 0; i < CamelsLeft.Count; i++)
            {
                var number = ThreadSafeRandom.ThisThreadsRandom.Next(1, 4);
                rollSet[i] = new Roll(colours[i], number);
            }

            return rollSet;
        }

        public Roll GetRoll()
        {
            var colours = CamelsLeft.Shuffle();
            var number = ThreadSafeRandom.ThisThreadsRandom.Next(1, 4);
            return new Roll(colours[0], number);
        }

        public bool IsCamelLeft(int camel)
        {
            return CamelsLeft.Contains(camel);
        }

        public bool RemoveCamel(int camel)
        {
            CamelsLeft.Remove(camel);
            if (CamelsLeft.Count == 0)
            {
                CamelsLeft = _allCamels.CloneList();
                return true;
            }
            return false;
        }
    }
}
