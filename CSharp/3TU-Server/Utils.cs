using System.Text;

namespace _3TU_Server
{
    internal static class Utils
    {
        /// <summary>
        /// Converts fields of any size to string which is represented in "X", "O" and "_".
        /// </summary>
        /// <param name="field">field from Type States of any size</param>
        /// <returns>returns the converted string.</returns>
        public static string ConvertFieldToString(States[,] field)
        {
            string result = "";

            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    States status = field[j, i];

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

        /// <summary>
        /// Converts fields of any size to string which is represented in "X", "O" and "_".
        /// </summary>
        /// <param name="field">field from Type Player of any size</param>
        /// <returns>returns the converted string.</returns>
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

        /// <summary>
        /// Converts the algebraic notation {Player [X/O]}{3x3 field}{1x1 field inside bigger field} into x- and y-coordinate.
        /// </summary>
        /// <param name="notation">algebraic notation</param>
        /// <param name="posX">x-coordinate</param>
        /// <param name="posY">y-coordinate</param>
        /// <returns>returns the Player in the algebraic notation (first char)</returns>
        public static Player ConvertNotationToCoordinates(string notation, out byte posX, out byte posY)
        {
            int grid = notation[1] - 1 - '0';
            int cell = notation[2] - 1 - '0';

            posX = Convert.ToByte(grid % 3 * 3 + cell % 3);
            posY = Convert.ToByte(grid - (grid % 3) + ((cell - (cell % 3)) / 3));

            return new Player(notation[0]);
        }

        /// <summary>
        /// Creates a sub 2D array from a smaller (or same) size.
        /// </summary>
        /// <typeparam name="T">2D array of any type</typeparam>
        /// <param name="arr">2D array</param>
        /// <param name="firstX">the first x-coordinate the field starts on (0, 3, 6)</param>
        /// <param name="firstY">the first y-coordinate the field starts on (0, 3, 6)</param>
        /// <param name="newRows">row size the new array should have</param>
        /// <param name="newCols">column size the new array should have</param>
        /// <returns>returns the new array.</returns>
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

        /// <summary>
        /// Converts a PlayerState represented as char in their PlayerState form.
        /// </summary>
        /// <param name="c">the char which should be converted</param>
        /// <returns>returns Player object which holds the PlayerState form of the char.</returns>
        public static Player ConvertCharToPlayerState(char c)
        {
            Player player = new();

            if (c == 'X') player.Status = Player.PlayerStates.X;
            else if (c == 'O') player.Status = Player.PlayerStates.O;
            else player.Status = Player.PlayerStates.Null;

            return player;
        }

        //Unfinished

        /// <summary>
        /// ! UNFINISHED !
        /// Prints more detailed debug infos to console.
        /// </summary>
        /// <param name="board">Gameboard</param>
        /// <param name="response">returns the serverside response sent to the client.</param>
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

        /// <summary>
        /// Prints the Gameboard to the console
        /// </summary>
        /// <param name="board">Gameboard</param>
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
