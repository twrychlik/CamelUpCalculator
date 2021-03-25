namespace CamelUpCalculator
{
    public class GameState
    {
        public GameState()
        {
            BoardState = new BoardState();
            Dice = new Dice();
            Bets = new Bets();
        }

        public BoardState BoardState;
        public Dice Dice;
        public Bets Bets;
    }
}
