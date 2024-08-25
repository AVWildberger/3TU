namespace _3TU_Server
{
    public class Player : IWinnable
    {
        /// <summary>
        /// Represents the States a Player can be. [X, O, Null]
        /// </summary>
        public enum PlayerStates { Null, X, O }

        /// <summary>
        /// Manages the current Status of the Player Object.
        /// </summary>
        public PlayerStates Status { get; set; }

        /// <summary>
        /// default constructor.
        /// </summary>
        public Player() { }

        /// <summary>
        /// constructor to directly set the Player State
        /// </summary>
        /// <param name="state"></param>
        public Player(char state)
        {
            Status = Utils.ConvertCharToPlayerState(state).Status;
        }

        /// <summary>
        /// Gets random PlayerState Object to determine the Beginner of the game.
        /// </summary>
        /// <returns>returns the randomized beginner.</returns>
        static public PlayerStates GetRandomBeginner()
        {
            Random rand = new();

            if (rand.Next(0, 2) == 0)
            {
                return PlayerStates.O;
            }
            else
            {
                return PlayerStates.X;
            }
        }

        /// <summary>
        /// Gets the winner (Generic Reasons)
        /// </summary>
        /// <returns>returns the winner if someone has won, else Null.</returns>
        public Player.PlayerStates GetWinner()
        {
            return this.Status;
        }

        /// <summary>
        /// Initializes the gameboard.
        /// </summary>
        /// <returns>returns the initialized gameboard.</returns>
        public static Player[,] InitGameBoard()
        {
            Player[,] board = new Player[9, 9];

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    board[i, j] = new Player();
                }
            }

            return board;
        }

        /// <summary>
        /// Places the given x- and y-coordinate on the provided gameboard.
        /// </summary>
        /// <param name="board">Gameboard</param>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate</param>
        /// <returns>returns what field to play next in. if every field is allowed, 0 is returned.</returns>
        public byte Place(ref Player[,] board, byte x, byte y)
        {
            board[x, y] = this;

            byte nextField = Convert.ToByte((x % 3 + 1) + (y % 3 * 3));

            States.GetBoardState(board, out States[,] boardStates);

            byte a = Convert.ToByte(nextField - 1);

            byte row = Convert.ToByte((a - a % 3) / 3);
            byte col = Convert.ToByte(a % 3);

            if (boardStates[col, row].Status != States.State.None) return 0;

            return nextField;
        }

        /// <summary>
        /// Gets player who has played last time and gets player whos turn is next.
        /// </summary>
        /// <param name="player">old player</param>
        /// <returns>returns next player to play.</returns>
        public static PlayerStates SwitchPlayer(Player player)
        {
            if (player.Status == PlayerStates.X) { return PlayerStates.O; }
            else { return PlayerStates.X; }
        }
    }
}
