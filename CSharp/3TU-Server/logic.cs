using _3TU_Server;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace _3TU_C_
{
    internal class Logic
    {
        private static Player[,] gameBoard = Player.InitGameBoard();
        private static Player.PlayerStates nextPlayer = Player.GetRandomBeginner();
        private static byte nextField = 0;

        /// <summary>
        /// Handles Main Process of Server. Default Start-Method.
        /// </summary>
        static async Task Main()
        {
            #region WebSocket
            HttpListener httpListener = new();
            httpListener.Prefixes.Add("http://localhost:8080/");
            httpListener.Start();
            Console.WriteLine("WebSocket server started at ws://localhost:8080/\n");

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
        }

        #region Websocket
        /// <summary>
        /// Handles the Websocket Connection with the client.
        /// </summary>
        /// <param name="webSocket">Websocket Object</param>
        static async Task HandleWebSocketConnection(WebSocket webSocket)
        {
            bool debug = false;

            byte[] buffer = new byte[1024];

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"> | {receivedMessage}");

                    string response = GetAnswer(receivedMessage);

                    byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
                    await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                    Console.WriteLine($"< | {response}\n");

                    if (debug) { Utils.PrintDebugInfos(gameBoard, response); }
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
            }
        }

        /// <summary>
        /// Generates the answer from the request from the client.
        /// </summary>
        /// <param name="request">request from the client</param>
        /// <returns>returns the response to send to the client.</returns>
        static string GetAnswer(string originalRequest)
        {
            string[] splitRequest = originalRequest.Split('?');

            string answer = splitRequest[0] + "?";
            string request = splitRequest[1];

            if (answer == "WIN?")
            {
                States gameState = Checker.HasWon(gameBoard, out States[,] capturedFields);

                if (gameState.Status == States.State.Won)
                {
                    answer += "TRUE;";

                    answer += gameState.Winner.ToString();
                }
                else
                {
                    answer += "FALSE";
                }

                answer += ";" + Utils.ConvertFieldToString(capturedFields);
            }
            else if (answer == "PLACE?")
            {
                string[] arr = request.Split(';');

                string algebraicNotation = arr[0];

                nextPlayer = Player.SwitchPlayer(Utils.ConvertNotationToCoordinates(algebraicNotation, out byte x, out byte y));

                bool isLegal = Checker.IsLegalPlacement(gameBoard, x, y);
                answer += isLegal.ToString().ToUpper();

                if (isLegal)
                {
                    answer += $";{algebraicNotation}";
                    nextField = Utils.ConvertCharToPlayerState(arr[0][0]).Place(ref gameBoard, x, y);
                    answer += ";" + nextField.ToString();
                }
            }
            else if (answer == "FETCH?")
            {
                answer += Utils.ConvertFieldToString(gameBoard) + ";";

                States.GetBoardState(gameBoard, out States[,] boardStates);
                answer += Utils.ConvertFieldToString(boardStates) + ";";

                answer += nextPlayer.ToString() + ";";

                answer += nextField.ToString();
            }

            return answer;
        }
        #endregion
    }
}