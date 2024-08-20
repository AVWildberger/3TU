using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3TU_Server
{
    internal static class Checker
    {
        public static States HasWon(Player[,] board, out States[,] boardState)
        {
            return States.GetBoardState(board, out boardState);
        }

        public static bool IsLegalPlacement(Player[,] board, int row, int col)
        {
            return (HasWon(board, out States[,] boardState).Status == States.State.None && boardState[row / 3, col / 3].Status == States.State.None && board[row, col].Status == Player.PlayerStates.Null);
        }

        public static bool AreSameAndNotDefault<T>(T a, T b, T c, ref Player winner) where T : IWinnable
        {
            bool value = false;

            if (!a.Equals(default(T)) && !a.GetWinner().Equals(default(Player.PlayerStates)) && a.GetWinner().Equals(b.GetWinner()) && b.GetWinner().Equals(c.GetWinner()))
            {
                winner.Status = a.GetWinner();
                value = true;
            }

            return value;
        }

        public static bool IsFieldWon<T>(T[,] field, out Player winner) where T : IWinnable
        {
            winner = new Player();

            return AreSameAndNotDefault(field[0, 0], field[1, 0], field[2, 0], ref winner) || // Option: 1
                    AreSameAndNotDefault(field[0, 1], field[1, 1], field[2, 1], ref winner) || // Option: 2
                    AreSameAndNotDefault(field[0, 2], field[1, 2], field[2, 2], ref winner) || // Option: 3
                    AreSameAndNotDefault(field[0, 0], field[0, 1], field[0, 2], ref winner) || // Option: 4
                    AreSameAndNotDefault(field[1, 0], field[1, 1], field[1, 2], ref winner) || // Option: 5
                    AreSameAndNotDefault(field[2, 0], field[2, 1], field[2, 2], ref winner) || // Option: 6
                    AreSameAndNotDefault(field[0, 0], field[1, 1], field[2, 2], ref winner) || // Option: 7
                    AreSameAndNotDefault(field[2, 0], field[1, 1], field[0, 2], ref winner);   // Option: 8

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

        public static States CheckFieldState<T>(T[,] board, int firstX = 0, int firstY = 0) where T : IWinnable
        {
            T[,] field = Utils.Sub2DArray(board, firstX, firstY, 3, 3);
            States state = new States();

            if (IsFieldWon(field, out Player winner))
            {
                state.Status = States.State.Won;
                state.Winner = winner.Status;
            }
            else if (IsFieldFull(field))
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
