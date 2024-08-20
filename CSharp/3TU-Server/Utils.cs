using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3TU_Server
{
    internal static class Utils
    {
        public static string Convert2DBoolArrayToString(States[,] arr)
        {
            string result = "";

            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    States status = arr[j, i];

                    if (status.Status == States.State.Won)
                    {
                        result += status.Winner.ToString();
                    }
                    else if (status.Status == States.State.Tie)
                    {
                        result += "T";
                    }
                    else
                    {
                        result += "_";
                    }
                }
            }

            return result;
        }

        public static bool ConvertNotationToCoordinates(string s, out byte posX, out byte posY)
        {
            int grid = s[1] - 1 - '0';
            int cell = s[2] - 1 - '0';

            posX = Convert.ToByte(grid % 3 * 3 + cell % 3);
            posY = Convert.ToByte(grid - (grid % 3) + ((cell - (cell % 3)) / 3));

            return s[0] == 'X';
        }

        public static T[,] Sub2DArray<T>(T[,] arr, int firstX, int firstY, int newRows, int newCols)
        {
            T[,] newArr = new T[newRows, newCols];

            for (int i = firstY; i < firstY + newRows; i++)
            {
                for (int j = firstX; j < firstX + newCols; j++)
                {
                    newArr[i - firstY, j - firstX] = arr[i, j];
                }
            }

            return newArr;
        }

        public static Player ConvertCharToPlayerState(char c)
        {
            Player player = new Player();

            if (c == 'X') player.Status = Player.PlayerStates.X;
            else if (c == 'O') player.Status = Player.PlayerStates.O;
            else player.Status = Player.PlayerStates.Null;

            return player;
        }
    }
}
