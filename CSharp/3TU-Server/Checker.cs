namespace _3TU_Server
{
    internal static class Checker
    {
        /// <summary>
        /// Checks if the player has won the game.
        /// </summary>
        /// <param name="board">Gameboard</param>
        /// <param name="boardState">State of each field of the gameboard (Win[X], Win[O], Tie, None)</param>
        /// <returns>return current status of the game.</returns>
        public static States HasWon(Player[,] board, out States[,] boardState)
        {
            return States.GetBoardState(board, out boardState);
        }

        /// <summary>
        /// Checks if the given placement is legal.
        /// </summary>
        /// <param name="board">Gameboard</param>
        /// <param name="row">row of gameboard</param>
        /// <param name="col">column of gameboard</param>
        /// <returns>returns bool object which checks if the given placement is legal.</returns>
        public static bool IsLegalPlacement(Player[,] board, int row, int col)
        {
            return (HasWon(board, out States[,] boardState).Status == States.State.None && boardState[row / 3, col / 3].Status == States.State.None && board[row, col].Status == Player.PlayerStates.Null);
        }

        /// <summary>
        /// Checks if 3 types are the same and not the default value.
        /// </summary>
        /// <typeparam name="T">Must be from the Type IWinnable, which is Player and States</typeparam>
        /// <param name="first">first value</param>
        /// <param name="second">second value</param>
        /// <param name="third">third value</param>
        /// <param name="winner">if all three types are the same, winner is the type, else default</param>
        /// <returns>returns bool object which checks if 3 given types are the same and not default.</returns>
        public static bool AreSameAndNotDefault<T>(T first, T second, T third, out Player winner) where T : IWinnable
        {
            winner = new Player();
            bool value = false;

            if (!first.Equals(default(T)) && !first.GetWinner().Equals(default(Player.PlayerStates)) && first.GetWinner().Equals(second.GetWinner()) && second.GetWinner().Equals(third.GetWinner()))
            {
                winner.Status = first.GetWinner();
                value = true;
            }

            return value;
        }

        /// <summary>
        /// Checks if field is won.
        /// </summary>
        /// <typeparam name="T">Must be from the Type IWinnable, which is Player and States</typeparam>
        /// <param name="field">3x3 Field or 9x9 Gameboard</param>
        /// <param name="winner">if field is won, in winner the winner gets written into, else default</param>
        /// <returns>returns bool object which checks if 3 given types are the same not default.</returns>
        public static bool IsFieldWon<T>(T[,] field, out Player winner) where T : IWinnable
        {
            return AreSameAndNotDefault(field[0, 0], field[1, 0], field[2, 0], out winner) || // Option: 1
                    AreSameAndNotDefault(field[0, 1], field[1, 1], field[2, 1], out winner) || // Option: 2
                    AreSameAndNotDefault(field[0, 2], field[1, 2], field[2, 2], out winner) || // Option: 3
                    AreSameAndNotDefault(field[0, 0], field[0, 1], field[0, 2], out winner) || // Option: 4
                    AreSameAndNotDefault(field[1, 0], field[1, 1], field[1, 2], out winner) || // Option: 5
                    AreSameAndNotDefault(field[2, 0], field[2, 1], field[2, 2], out winner) || // Option: 6
                    AreSameAndNotDefault(field[0, 0], field[1, 1], field[2, 2], out winner) || // Option: 7
                    AreSameAndNotDefault(field[2, 0], field[1, 1], field[0, 2], out winner);   // Option: 8

            #region Options
            /* * * * * * * * * * * * * * * * * * *
             * Option: 1 * Option: 2 * Option: 3 *
             *           *           *           *
             *   x|x|x   *   _|_|_   *   _|_|_   *
             *   _|_|_   *   x|x|x   *   _|_|_   *
             *   _|_|_   *   _|_|_   *   x|x|x   *
             * * * * * * * * * * * * * * * * * * *
             * Option: 4 * Option: 5 * Option: 6 *
             *           *           *           *
             *   x|_|_   *   _|x|_   *   _|_|x   *
             *   x|_|_   *   _|x|_   *   _|_|x   *
             *   x|_|_   *   _|x|_   *   _|_|x   *
             * * * * * * * * * * * * * * * * * * *
             * Option: 7 * Option: 8 *           *
             *                                   *
             *  x|_|_    *   _|_|x   *           *
             *  _|x|_    *   _|x|_   *           *
             *  _|_|x    *   x|_|_   *           *
             * * * * * * * * * * * * * * * * * * */
            #endregion
        }

        /// <summary>
        /// Checks if field is fully filled.
        /// </summary>
        /// <typeparam name="T">Must be from the Type IWinnable, which is Player and States</typeparam>
        /// <param name="field">3x3 Field or 9x9 Gameboard</param>
        /// <returns>returns bool object which checks if the field is filled.</returns>
        public static bool IsFieldFull<T>(T[,] field) where T : IWinnable
        {
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j].GetWinner() == Player.PlayerStates.Null) { return false; }
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the field is either won from X, won from O, tie or none.
        /// </summary>
        /// <typeparam name="T">Can be from the Type IWinnable, which is Player and States</typeparam>
        /// <param name="field">3x3 Field or 9x9 Gameboard</param>
        /// <param name="firstX">the first x-coordinate the field starts on (0, 3, 6)</param>
        /// <param name="firstY">the first y-coordinate the field starts on (0, 3, 6)</param>
        /// <returns>returns States object which contains the current state of the field.</returns>
        public static States CheckFieldState<T>(T[,] field, int firstX = 0, int firstY = 0) where T : IWinnable
        {
            T[,] newField = Utils.Sub2DArray(field, firstX, firstY, 3, 3);
            States state = new();

            if (IsFieldWon(newField, out Player winner))
            {
                state.Status = States.State.Won;
                state.Winner = winner.Status;
            }
            else if (IsFieldFull(newField))
            {
                state.Status = States.State.Tie;
            }
            else
            {
                state.Status = States.State.None;
            }

            return state;
        }
    }
}
