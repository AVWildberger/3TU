namespace _3TU_Server
{
    internal class States : IWinnable
    {
        /// <summary>
        /// Represents the State of a Field. Either Won, Tie or None
        /// </summary>
        public enum State { None, Won, Tie }

        /// <summary>
        /// Manages the current Status of the States Object.
        /// </summary>
        public State Status { get; set; }

        /// <summary>
        /// If the Status is Won, then the Winner will be saved here.
        /// </summary>
        public Player.PlayerStates Winner { get; set; }

        /// <summary>
        /// Gets the winner (Generic Reasons)
        /// </summary>
        /// <returns>returns the winner if someone has won, else Null.</returns>
        public Player.PlayerStates GetWinner()
        {
            return Winner;
        }

        /// <summary>
        /// Gets the state of the whole board.
        /// </summary>
        /// <param name="board">Gameboard</param>
        /// <param name="boardState">States of the single fields</param>
        /// <returns>returns the state of full board.</returns>
        public static States GetBoardState(Player[,] board, out States[,] boardState)
        {
            boardState = new States[3, 3];

            for (int row = 0; row < board.GetLength(0) / 3; row++)
            {
                for (int col = 0; col < board.GetLength(1) / 3; col++)
                {
                    boardState[col, row] = Checker.CheckFieldState(board, row * 3, col * 3);
                }
            }

            return Checker.CheckFieldState(boardState);
        }
    }
}
