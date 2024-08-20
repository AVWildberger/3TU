using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3TU_Server
{
    public class Player : IWinnable
    {
        public enum PlayerStates { Null, X, O }
        public PlayerStates Status { get; set; }

        public Player.PlayerStates GetWinner()
        {
            return this.Status;
        }

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

        //returns what field to play next in. 0 if every field is allowed.
        public byte Place(ref Player[,] gameBoard, byte x, byte y)
        {
            gameBoard[x, y] = this;

            byte nextField = Convert.ToByte((x % 3 + 1) + (y % 3 * 3));

            States.GetBoardState(gameBoard, out States[,] boardStates);

            byte a = Convert.ToByte(nextField - 1);

            byte row = Convert.ToByte((a - a % 3) / 3);
            byte col = Convert.ToByte(a % 3);

            if (boardStates[col, row].Status != States.State.None) return 0;

            return nextField;
        }
    }
}
