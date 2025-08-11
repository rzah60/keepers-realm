// WebServer.cs - Final Corrected Version with Logging
using Godot;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

public partial class WebServer : Node
{
    private TcpListener _tcpListener;
    public NetworkManager NetworkManager { get; set; }

    public void StartServer(string address, int port)
    {
        try
        {
            IPAddress localAddr = IPAddress.Parse(address);
            _tcpListener = new TcpListener(localAddr, port);
            _tcpListener.Start();
            GD.Print($"Simple API server started with TCP. Listening on {address}:{port}");
            Task.Run(HandleConnections);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to start Simple API server: {ex.Message}");
            _tcpListener = null;
        }
    }

    private async Task HandleConnections()
    {
        while (_tcpListener != null)
        {
            try
            {
                using TcpClient client = await _tcpListener.AcceptTcpClientAsync();
                await ProcessClient(client);
            }
            catch (Exception ex)
            {
                if (_tcpListener != null)
                {
                    GD.PrintErr($"Error accepting client: {ex.Message}");
                }
            }
        }
    }

    private async Task ProcessClient(TcpClient client)
    {
        await using NetworkStream stream = client.GetStream();
        
        using var memoryStream = new MemoryStream();
        byte[] buffer = new byte[1024];
        int bytesRead;

        GD.Print("[WebServer] Client connected. Reading request...");

        // A simple read loop. For a production server, add timeouts.
        do
        {
            bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            await memoryStream.WriteAsync(buffer, 0, bytesRead);
        } while (stream.DataAvailable);
    
        string requestData = Encoding.UTF8.GetString(memoryStream.ToArray());

        // --- DIAGNOSTIC LOGGING ADDED ---
        GD.Print($"""
        [WebServer] Received raw request data:
        ---
        {requestData}
        ---
        """);
        
        if (string.IsNullOrWhiteSpace(requestData))
        {
            GD.Print("[WebServer] Received empty request. Closing connection.");
            return;
        }

        // --- FIX: Regex now case-insensitive
        Match requestLineMatch = Regex.Match(requestData, @"^POST /api/session/validate", RegexOptions.IgnoreCase);
        if (!requestLineMatch.Success)
        {
            await SendResponse(stream, "{\"error\":\"Not Found\"}", "404 Not Found");
            return;
        }

        // --- FIX: Regex now case-insensitive
        Match contentLengthMatch = Regex.Match(requestData, @"Content-Length: (\d+)", RegexOptions.IgnoreCase);
        if (!contentLengthMatch.Success)
        {
            GD.PrintErr("[WebServer] 'Content-Length' header not found in request.");
            await SendResponse(stream, "{\"error\":\"Content-Length header missing\"}", "400 Bad Request");
            return;
        }
        
        int headerEndIndex = requestData.IndexOf("\r\n\r\n");
        if (headerEndIndex == -1)
        {
            GD.PrintErr("[WebServer] Malformed request: No header/body separator found.");
            await SendResponse(stream, "{\"error\":\"Malformed request\"}", "400 Bad Request");
            return;
        }
        string body = requestData.Substring(headerEndIndex).Trim();
        
        try
        {
            dynamic jsonBody = JsonConvert.DeserializeObject(body);
            string code = jsonBody.code;

            if (code == NetworkManager.SessionCode)
            {
                await SendResponse(stream, "{\"status\": \"valid\"}", "200 OK");
            }
            else
            {
                await SendResponse(stream, "{\"status\": \"invalid\"}", "401 Unauthorized");
            }
        }
        catch (JsonException ex)
        {
            GD.PrintErr($"[WebServer] JSON Deserialization Error: {ex.Message} on body: {body}");
            await SendResponse(stream, "{\"error\": \"Invalid JSON format\"}", "400 Bad Request");
        }
    }
        
    private async Task SendResponse(NetworkStream stream, string json, string httpStatus)
    {
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        string headers =
            $"HTTP/1.1 {httpStatus}\r\n" +
            $"Content-Type: application/json\r\n" +
            $"Content-Length: {jsonBytes.Length}\r\n" +
            $"Connection: close\r\n" +
            $"Access-Control-Allow-Origin: *\r\n" +
            $"\r\n";

        byte[] headerBytes = Encoding.UTF8.GetBytes(headers);
        await stream.WriteAsync(headerBytes);
        await stream.WriteAsync(jsonBytes);
    }

    public void StopServer()
    {
        _tcpListener?.Stop();
        _tcpListener = null;
        GD.Print("Simple API server stopped.");
    }

    public override void _ExitTree()
    {
        StopServer();
    }
}