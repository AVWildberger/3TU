using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3TU_Server
{
    public class Field:TTT
    {
        private bool?[] cells;

        public GameStatus State { get; private set; }

        public static bool?[] InitGameBoard()
        {
            return new bool?[3 * 3];
        }

        public Field()
        {
            cells = InitGameBoard();
        }

        public bool PlacePlayer(char player, int cell)
        {
            if ((player != 'X' && player != 'O') || (cell < 0 || cell > 8))
            {
                return false;
            }


            if (cells[cell] == null)
            {
                cells[cell] = player == 'X';

                State = CheckWin();

                return true;
            }

            return false;
        }

        public string GetRow(int row)
        {
            string rowString = "";

            for (int i = 0; i < 3; i++)
            {
                bool? cell = cells[row * 3 + i];

                if (cell == null)
                {
                    rowString += "_";
                }
                else
                {
                    rowString += cell == true ? "X" : "O";
                }
            }

            return rowString;
        }

        protected override GameStatus CheckWin()
        {
            /* Check rows and columns */
            for (int i = 0; i < 3; i++)
            {
                /* Check rows */
                if (cells[i * 3] == cells[i * 3 + 1] && cells[i * 3] == cells[i * 3 + 2])
                {
                    if (cells[i * 3] != null)
                    {
                        return cells[i * 3] == true ? GameStatus.X : GameStatus.O;
                    }
                }
                /* Check columns */
                if (cells[i] == cells[i + 3] && cells[i] == cells[i + 6])
                {
                    if (cells[i] != null)
                    {
                        return cells[i] == true ? GameStatus.X : GameStatus.O;
                    }
                }
            }
            /* Check diagonal
             * X| |
             *  |X|
             *  | |X 
             */
            if (cells[0] == cells[4] && cells[0] == cells[8])
            {
                if (cells[0] != null)
                {
                    return cells[0] == true ? GameStatus.X : GameStatus.O;
                }
            }
            /* Check diagonal
             *  | |X
             *  |X|
             * X| | 
             */
            if (cells[2] == cells[4] && cells[2] == cells[6])
            {
                if (cells[2] != null)
                {
                    return cells[2] == true ? GameStatus.X : GameStatus.O;
                }
            }

            // Check for tie
            if (cells.All(x => x != null))
            {
                return GameStatus.Tie;
            }

            return GameStatus.None;
        }
    }
}
