using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3TU_Server
{
    internal static class Utils
    {
        public static string ConvertFieldToString(States[,] arr)
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

        public static string ConvertFieldToString(Player[,] arr)
        {
            string result = "";

            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    Player player = arr[j, i];

                    if (player.Status == Player.PlayerStates.Null)
                    {
                        result += "_";
                    }
                    else
                    {
                        result += player.Status.ToString();
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

        //Unfinished
        public static void PrintDebugInfos(Player[,] board, string response)
        {
            Console.OutputEncoding = Encoding.UTF8;

            string[] responseArr = response.Split('?');
            string[] details = responseArr[1].Split(';');
            
            if (responseArr[0] == "WIN")
            {
                Console.WriteLine($"Gamestatus: {details[0]}");
                if (details[0] == "TRUE") { Console.WriteLine($"Winner: {details[1]}"); }
            }
            else
            {
                Console.WriteLine($"Valid Move: {details[0]}");
                if (details[0] == "TRUE") { Console.WriteLine($"Next Field: {details[2]}"); };
            }

            PrintBoard(board);
        }

        private static void PrintBoard(Player[,] board)
        {
            string[] arr = new string[]
            {
                "┏━━━┯━━━┯━━━┳━━━┯━━━┯━━━┳━━━┯━━━┯━━━┓",
                "┃ _ │ _ │ _ ┃ _ │ _ │ _ ┃ _ │ _ │ _ ┃",
                "┠───┼───┼───╂───┼───┼───╂───┼───┼───┨",
                "┃ _ │ _ │ _ ┃ _ │ _ │ _ ┃ _ │ _ │ _ ┃",
                "┠───┼───┼───╂───┼───┼───╂───┼───┼───┨",
                "┃ _ │ _ │ _ ┃ _ │ _ │ _ ┃ _ │ _ │ _ ┃",
                "┣━━━┿━━━┿━━━╋━━━┿━━━┿━━━╋━━━┿━━━┿━━━┫",
                "┃ _ │ _ │ _ ┃ _ │ _ │ _ ┃ _ │ _ │ _ ┃",
                "┠───┼───┼───╂───┼───┼───╂───┼───┼───┨",
                "┃ _ │ _ │ _ ┃ _ │ _ │ _ ┃ _ │ _ │ _ ┃",
                "┠───┼───┼───╂───┼───┼───╂───┼───┼───┨",
                "┃ _ │ _ │ _ ┃ _ │ _ │ _ ┃ _ │ _ │ _ ┃",
                "┣━━━┿━━━┿━━━╋━━━┿━━━┿━━━╋━━━┿━━━┿━━━┫",
                "┃ _ │ _ │ _ ┃ _ │ _ │ _ ┃ _ │ _ │ _ ┃",
                "┠───┼───┼───╂───┼───┼───╂───┼───┼───┨",
                "┃ _ │ _ │ _ ┃ _ │ _ │ _ ┃ _ │ _ │ _ ┃",
                "┠───┼───┼───╂───┼───┼───╂───┼───┼───┨",
                "┃ _ │ _ │ _ ┃ _ │ _ │ _ ┃ _ │ _ │ _ ┃",
                "┗━━━┷━━━┷━━━┻━━━┷━━━┷━━━┻━━━┷━━━┷━━━┛"
            };

            for (int row = 0; row < arr.Length; row++)
            {
                for (int col = 0; col < arr[0].Length; col++)
                {
                    if ((row + 1) % 2 == 0 && (col + 1) % 4 == 0 && board[(col + 1) / 4 - 1, (row + 1) / 2 - 1].Status != Player.PlayerStates.Null)
                    {
                        //string asf = board[(col + 1) / 4 - 1, (row + 1) / 2 - 1].Status.ToString();
                        Console.Write(Convert.ToChar(board[(col + 1) / 4 - 2, (row + 1) / 2 - 1].Status.ToString()));
                    }
                    else
                    {
                        Console.Write(arr[row][col]);
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
