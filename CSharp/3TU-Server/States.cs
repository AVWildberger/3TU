using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3TU_Server
{
    internal class States : IWinnable
    {
        public enum State { None, Won, Tie }

        public State Status { get; set; }
        public Player.PlayerStates Winner { get; set; }

        public Player.PlayerStates GetWinner()
        {
            return Winner;
        }

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
