using System;
using System.Collections.Generic;

namespace CamelUpCalculator
{
    [Serializable]
    public class Square
    {
        public int Index { get; }
        public List<Colour> Camels => _camels;
        public int CamelCount => _camels.Count;
        public Modifier Modifier { get; private set; }

        public Square(int index)
        {
            Index = index;
            Modifier = Modifier.None;

            _camels = new List<Colour>();
        }

        private List<Colour> _camels;

        public List<Colour> RemoveCamels(int height)
        {
            var numberofCamels = _camels.Count;
            var camelsToMove = _camels.GetRange(height, numberofCamels - height);
            _camels.RemoveRange(height, numberofCamels - height);
            return camelsToMove;
        }

        public void AddCamels(List<Colour> camels, bool addBelow)
        {
            if (addBelow)
            {
                _camels.InsertRange(0, camels);
            }
            else
            {
                _camels.AddRange(camels);
            }
        }

        public bool PlaceModifier(Modifier modifier)
        {
            if (CamelCount == 0)
            {
                Modifier = modifier;
                return true;
            }
            return false;
        }

        public void RemoveModifier()
        {
            Modifier = Modifier.None;
        }
    }
}