using System.Net.Sockets;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Xml;

namespace _3TU_C_
{
    internal class Program
    {
        static bool?[,] gameBoard = InitGameBoard();

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


        static bool?[,] InitGameBoard()
        {
            return new bool?[9, 9];
        }

        //returns what field to play next in
        static byte PlacePlayer(bool player, byte x, byte y)
        {
            gameBoard[x, y] = player;

            return Convert.ToByte((x % 3 + 1) + (y % 3 * 3));
        }

        static bool HasWon(bool?[,] board, out bool? winner, out bool?[,] capturedFields)
        {
            capturedFields = new bool?[3, 3];

            for (int row = 0; row < board.Length; row++)
            {
                for (int col = 0; col < board.Length; col++)
                {
                    capturedFields[row, col] = FieldStatus(board, row * 3, col * 3);
                }
            }

            winner = FieldStatus(capturedFields);

            if (winner == null) return false;

            return true;
        }

        static bool? FieldStatus(bool?[,] board, int firstX = 0, int firstY = 0)
        {
            /* 1|2|3 *
             * 4|5|6 *
             * 7|8|9 */
            bool? field1 = board[firstX, firstY];
            bool? field2 = board[firstX + 1, firstY];
            bool? field3 = board[firstX + 2, firstY];
            bool? field4 = board[firstX, firstY + 1];
            bool? field5 = board[firstX + 1, firstY + 1];
            bool? field6 = board[firstX + 2, firstY + 1];
            bool? field7 = board[firstX, firstY + 2];
            bool? field8 = board[firstX + 1, firstY + 2];
            bool? field9 = board[firstX + 2, firstY + 2];

            bool? capturedPlayer = null;

            if (AreSameAndNotNull(field1, field2, field3, ref capturedPlayer) || // Option: 1
                AreSameAndNotNull(field4, field5, field6, ref capturedPlayer) || // Option: 2
                AreSameAndNotNull(field7, field8, field9, ref capturedPlayer) || // Option: 3
                AreSameAndNotNull(field1, field4, field7, ref capturedPlayer) || // Option: 4
                AreSameAndNotNull(field2, field5, field8, ref capturedPlayer) || // Option: 5
                AreSameAndNotNull(field3, field6, field9, ref capturedPlayer) || // Option: 6
                AreSameAndNotNull(field1, field5, field9, ref capturedPlayer) || // Option: 7
                AreSameAndNotNull(field3, field5, field7, ref capturedPlayer))   // Option: 8
            { return capturedPlayer; }

            return null;

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

        static bool IsLegalPlacement(bool?[,] board, int row, int col)
        {
            return (!HasWon(board, out _, out bool?[,] capturedFields) && capturedFields[row / 3, col / 3] == null && board[row, col] == null);
        }

        static bool AreSameAndNotNull(bool? a, bool? b, bool? c, ref bool? boolValue)
        {
            if (a != null && a == b && b == c)
            {
                boolValue = a;
                return true;
            }

            boolValue = null;
            return false;
        }

        static async Task HandleWebSocketConnection(WebSocket webSocket)
        {
            byte[] buffer = new byte[1024];

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string receivedString = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    //Console.WriteLine($"Received string: {receivedString}");

                    //GetAnswer()

                    bool isValid = !string.IsNullOrEmpty(receivedString);

                    byte[] responseBuffer = Encoding.UTF8.GetBytes(isValid.ToString().ToLower());
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

            if (request.StartsWith("VALID"))
            {
                string[] arr = request.Split(';');

                if (IsLegalPlacement(gameBoard, Convert.ToInt32(arr[1]), Convert.ToInt32(arr[2])))
                {
                    answer = "TRUE";
                }
                else
                {
                    answer = "FALSE";
                }
            }
            else if (request.StartsWith("WIN"))
            {
                bool hasWon = HasWon(gameBoard, out bool? winner, out bool?[,] capturedFields);

                if (hasWon)
                {
                    answer = "TRUE";

                    if (winner == true) { answer += ";X"; }
                    else { answer += ";O"; }
                }
                else
                {
                    answer = "TIE";

                    foreach (bool? b in capturedFields)
                    {
                        if (b == null) { answer = "FALSE"; }
                    }
                }

                answer += "," + Convert2DBoolArrayToString(gameBoard);
            }
            else if (request.StartsWith("PLACE"))
            {
                string[] arr = request.Split(';');

                string algebraicNotation = arr[1];

                ConvertNotationToCoordinates(algebraicNotation, out int x, out int y);

                bool isLegal = IsLegalPlacement(gameBoard, x, y);
                answer = isLegal.ToString().ToUpper();

                if (isLegal)
                {
                    answer += PlacePlayer(arr[0] == "X", Convert.ToByte(arr[1]), Convert.ToByte(arr[2])).ToString();
                }
            }

            return answer;
        }

        static string Convert2DBoolArrayToString(bool?[,] arr)
        {
            string result = "";

            foreach (bool? b in arr)
            {
                if (b == true)
                {
                    result += "X";
                }
                else if (b == false)
                {
                    result += "O";
                }
                else
                {
                    result += "_";
                }
            }

            return result;
        }

        static bool ConvertNotationToCoordinates(string s, out int posX, out int posY)
        {
            int grid = s[1] - 1;
            int cell = s[2] - 1;

            posX = (grid % 3) * 3 + (cell % 3);
            posY = grid - (grid % 3) + ((cell - (cell % 3)) / 3);

            return s[0] == 'X';
        }
    }
}