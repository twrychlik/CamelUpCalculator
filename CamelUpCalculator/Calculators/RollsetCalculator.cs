using System.Collections.Generic;
using System.Linq;

namespace CamelUpCalculator
{
    public class RollsetCalculator
    {
        public RollsetCalculator()
        {
            var allCamels = new List<int>();
            for (int i = 0; i < Constants.NumberOfCamels; i++)
            {
                allCamels.Add(i);
            }
            MaxRollSet = CreateAllRollSets(allCamels);
        }

        public readonly List<List<Roll>> MaxRollSet;

        public List<List<Roll>> CreateAllRollSets(List<int> camels)
        {
            if (camels.Count == Constants.NumberOfCamels && MaxRollSet != null)
            {
                return MaxRollSet;
            }

            var allRollSets = new List<List<Roll>>();

            for (int i = 0; i < camels.Count; i++)
            {
                var camelsLeft = camels.Where(x => x != camels[i]).ToList();
                var possiblePreviousRolls = new List<List<Roll>>(){ };
                if (camelsLeft.Count != 0)
                {
                     possiblePreviousRolls = CreateAllRollSets(camelsLeft);
                }

                for (int j = 1; j < 4; j++)
                {
                    var lastRoll = new List<Roll>() { new Roll(camels[i], j) };

                    if (possiblePreviousRolls.Count == 0)
                    {
                        allRollSets.Add(lastRoll.ToList());
                    }

                    for (int k = 0; k < possiblePreviousRolls.Count; k++)
                    {
                        var previousRolls = possiblePreviousRolls[k];
                        allRollSets.Add(previousRolls.Concat(lastRoll).ToList());
                    }
                }
            }

            return allRollSets;
        }
    }
}
