using System.Net.Sockets;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using _3TU_Server;
using System.Reflection;

namespace _3TU_C_
{
    internal class Logic
    {
        static Player[,] gameBoard = InitGameBoard();

        static async Task Main()
        {
            #region WebSocket
            HttpListener httpListener = new();
            httpListener.Prefixes.Add("http://localhost:8080/");
            httpListener.Start();
            Console.WriteLine("WebSocket server started at ws://localhost:8080/");

            while (true)
            {
                HttpListenerContext context = await httpListener.GetContextAsync();

                if (context.Request.IsWebSocketRequest)
                {
                    HttpListenerWebSocketContext wsContext = await context.AcceptWebSocketAsync(null);
                    WebSocket webSocket = wsContext.WebSocket;

                    await HandleWebSocketConnection(webSocket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
            #endregion

            // True = X
            // False = O
        }


        static Player[,] InitGameBoard()
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
        static byte PlacePlayer(Player player, byte x, byte y)
        {
            gameBoard[x, y] = player;

            byte nextField = Convert.ToByte((x % 3 + 1) + (y % 3 * 3));

            States[,] boardStates = GetBoardStates(gameBoard);

            byte a = Convert.ToByte(nextField - 1);

            byte row = Convert.ToByte((a - a % 3) / 3);
            byte col = Convert.ToByte(a % 3);

            if (boardStates[col, row].Status != States.State.None) return 0;

            return nextField;
        }

        static bool HasWon(Player[,] board, out Player winner, out States[,] boardState)
        {
            boardState = GetBoardStates(board);

            return IsFieldWon<States>(boardState, out winner);
        }

        static States[,] GetBoardStates(Player[,] board)
        {
            States[,] boardStates = new States[3, 3];

            for (int row = 0; row < board.GetLength(0) / 3; row++)
            {
                for (int col = 0; col < board.GetLength(1) / 3; col++)
                {
                    boardStates[row, col] = CheckFieldState(board, row * 3, col * 3);
                }
            }

            return boardStates;
        }

        static States CheckFieldState(Player[,] board, int firstX = 0, int firstY = 0)
        {
            Player[,] field = Sub2DArray(board, firstX, firstY, 3, 3);
            States state = new States();

            if (IsFieldWon(field, out Player winner))
            {
                state.Status = States.State.Won;
                state.Winner = winner.State;
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

        static T[,] Sub2DArray<T>(T[,] arr, int firstX, int firstY, int newRows, int newCols)
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

        static bool IsLegalPlacement(Player[,] board, int row, int col)
        {
            return (!HasWon(board, out _, out States[,] capturedFields) && capturedFields[row / 3, col / 3].Status == States.State.None && board[row, col].State == Player.PlayerStates.Null);
        }

        static bool AreSameAndNotDefault<T>(T a, T b, T c, ref Player winner) where T : IWinnable
        {
            bool value = false;

            if (a != null && !a.Equals(default(T)) && a.Equals(b) && b.Equals(c))
            {
                winner.State = a.GetWinner();
                value = true;
            }

            return value;
        }

        static bool IsFieldWon<T>(T[,] field, out Player winner) where T : IWinnable
        {
            winner = new Player();

            return  AreSameAndNotDefault(field[0, 0], field[1, 0], field[2, 0], ref winner) || // Option: 1
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

        static bool IsFieldFull(Player[,] field)
        {
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j].State == Player.PlayerStates.Null) { return false; }
                }
            }

            return true;
        }

        static async Task HandleWebSocketConnection(WebSocket webSocket)
        {
            byte[] buffer = new byte[1024];

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received message: {receivedMessage}");

                    string response = GetAnswer(receivedMessage);

                    byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
                    await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
            }
        }

        static string GetAnswer(string request)
        {
            string answer = "";

            if (request.StartsWith("WIN"))
            {
                answer = "WIN?";

                bool hasWon = HasWon(gameBoard, out Player winner, out States[,] capturedFields);

                if (hasWon)
                {
                    answer += "TRUE;";

                    answer += winner.ToString();
                }
                else
                {
                    answer += "FALSE";
                }

                answer += ";" + Convert2DBoolArrayToString(capturedFields);
            }
            else if (request.StartsWith("PLACE"))
            {
                answer = "PLACE?";

                string[] arr = request.Split(';');

                string algebraicNotation = arr[1];

                ConvertNotationToCoordinates(algebraicNotation, out byte x, out byte y);

                bool isLegal = IsLegalPlacement(gameBoard, x, y);
                answer += isLegal.ToString().ToUpper();

                if (isLegal)
                {
                    answer += $";{algebraicNotation}";
                    answer += ";" + PlacePlayer(ConvertCharToPlayerState(arr[1][0]), x, y).ToString();
                }
            }

            return answer;
        }

        static Player ConvertCharToPlayerState(char c)
        {
            Player player = new Player();

            if (c == 'X') player.State = Player.PlayerStates.X;
            else if (c == 'O') player.State = Player.PlayerStates.O;
            else player.State = Player.PlayerStates.Null;

            return player;
        }

        static string Convert2DBoolArrayToString(States[,] arr)
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

        static bool ConvertNotationToCoordinates(string s, out byte posX, out byte posY)
        {
            int grid = s[1] - 1 - '0';
            int cell = s[2] - 1 - '0';

            posX = Convert.ToByte(grid % 3 * 3 + cell % 3);
            posY = Convert.ToByte(grid - (grid % 3) + ((cell - (cell % 3)) / 3));

            return s[0] == 'X';
        }
    }
}