﻿using System.Net.Sockets;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Xml;
using System;
using _3TU_Server;

namespace _3TU_C_
{
    internal class Logic
    {
        static Game game = new Game();

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

        static async Task HandleWebSocketConnection(WebSocket webSocket)
        {
            byte[] buffer = new byte[1024];

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"<{receivedMessage}");

                    string response = GetAnswer(receivedMessage);

                    byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
                    await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    Console.WriteLine($">{response}");
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
            }
        }

        // Get the board and all relevant information as a string
        static string FetchBoard()
        {
            string board = game.GetBoardAsString();
            string fieldStatuses = game.GetFieldStatuses();
            string nextPlayer = game.NextPlayer.ToString();
            string nextField = game.NextField.ToString();

            return board + ";" + fieldStatuses + ";" + nextPlayer + ";" + nextField;
        }

        static string GetAnswer(string request)
        {
            string answer = "";

            if (request.StartsWith("WIN"))
            {
                answer = "WIN?";

                bool hasWon = game.State != Game.GameStatus.None;

                if (hasWon)
                {
                    answer += "TRUE";

                    switch (game.State)
                    {
                        case Game.GameStatus.X:
                            answer += ";X";
                            break;
                        case Game.GameStatus.O:
                            answer += ";O";
                            break;
                        case Game.GameStatus.Tie:
                            answer += ";TIE";
                            break;
                    }
                }
                else
                {
                    answer += "FALSE";
                }

                answer += ";" + game.GetFieldStatuses();
            }
            else if (request.StartsWith("PLACE"))
            {
                answer = "PLACE?";

                string[] arr = request.Split('?')[1].Split(';');
                string algebraicNotation = arr[0];

                int next = game.PlacePlayer(algebraicNotation);

                if (next < 0)
                {
                    if (next == -1)
                    {
                        answer += "FALSE";
                    }

                    // Client should resync
                    if (next == -2)
                    {
                        // Send the board and field statuses to the client to resync
                        answer = "FETCH?" + FetchBoard();
                    }
                }
                else
                {
                    answer += $"TRUE;{algebraicNotation};{next}";
                }
            }
            else if (request.StartsWith("FETCH"))
            {
                answer = "FETCH?" + FetchBoard();
            }
            else if (request.StartsWith("LOG"))
            {
                answer = "LOG?" + game.GetLogs();
            }

            return answer;
        }
    }
}