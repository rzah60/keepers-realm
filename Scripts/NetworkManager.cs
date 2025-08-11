// NetworkManager.cs - Singleton Pattern Implementation
using Godot;
using System;
using Newtonsoft.Json; 
using System.Collections.Generic;
using System.Linq;
using WebSocketSharp;
using WebSocketSharp.Server;

// An instance of this class is created for every player that connects.
public class GameSessionBehavior : WebSocketBehavior
{
    // It no longer needs a direct reference field.

    protected override void OnOpen()
    {
        // We can now access the NetworkManager through its static Instance.
        NetworkManager.Instance.RegisterPlayer(this.ID);
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        NetworkManager.Instance.ProcessPacket(this.ID, e.Data);
    }

    protected override void OnClose(CloseEventArgs e)
    {
        NetworkManager.Instance.UnregisterPlayer(this.ID);
    }
}


public partial class NetworkManager : Node
{
    // --- FIX #1: Add a static Instance property ---
    public static NetworkManager Instance { get; private set; }

    [Signal] public delegate void ServerCreatedEventHandler();
    
    private const int DEFAULT_PORT = 7777;
    private const int WEB_PORT = 8080;
    private const int SESSION_CODE_LENGTH = 4;

    private WebServer _webServer;
    private Dictionary<string, PlayerSession> _playerSessions = new Dictionary<string, PlayerSession>();
    private WebSocketServer _wsServer;
    
    public string SessionCode { get; private set; }

    public override void _Ready()
    {
        // --- FIX #2: Assign the instance ---
        Instance = this;

        _webServer = new WebServer { NetworkManager = this };
        AddChild(_webServer);
    }
    
    public override void _Process(double delta) { }
    
    public override void _ExitTree()
    {
        _wsServer?.Stop();
    }

    public void CreateServer()
    {
        _wsServer = new WebSocketServer($"ws://127.0.0.1:{DEFAULT_PORT}");

        // --- FIX #3: Use the simpler method overload that avoids the compiler error ---
        _wsServer.AddWebSocketService<GameSessionBehavior>("/");
        
        _wsServer.Start();
        
        SessionCode = GenerateSessionCode();
        GD.Print($"WebSocketSharp server started on port {DEFAULT_PORT}. Session code: {SessionCode}");
        
        _webServer.StartServer("127.0.0.1", WEB_PORT);
        EmitSignal(SignalName.ServerCreated);
    }
    
    public void RegisterPlayer(string sessionId)
    {
        GD.Print($"Player connected with Session ID: {sessionId}");
    }
    
    public void UnregisterPlayer(string sessionId)
    {
        GD.Print($"Player disconnected: {sessionId}");
        if (_playerSessions.ContainsKey(sessionId))
        {
            _playerSessions.Remove(sessionId);
        }
    }

    public void ProcessPacket(string sessionId, string jsonString)
    {
        try
        {
            dynamic json = JsonConvert.DeserializeObject(jsonString);
            string messageType = json.type;
            
            GD.Print($"Received '{messageType}' from {sessionId}");

            if (messageType == "authenticate")
            {
                string code = json.code;
                AuthenticatePlayer(sessionId, code);
            }
            else if (messageType == "select_investigator")
            {
                string investigatorName = json.investigatorName;
                SelectInvestigatorForPlayer(sessionId, investigatorName);
            }
            // --- ADD THIS NEW BLOCK ---
            else if (messageType == "update_stats")
            {
                // Newtonsoft.Json can deserialize the 'stats' object into a C# Dictionary.
                var stats = json.stats.ToObject<Dictionary<string, long>>();
                UpdatePlayerStats(sessionId, stats);
            }
            // --- END OF NEW BLOCK ---
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error processing packet from {sessionId}: {e.Message}");
        }
    }

    public void AuthenticatePlayer(string sessionId, string code)
    {
        if (code != this.SessionCode) return;

        if (!_playerSessions.ContainsKey(sessionId))
        {
            _playerSessions.Add(sessionId, new PlayerSession());
            SendInvestigatorList(sessionId);
        }
    }
    
    private void SelectInvestigatorForPlayer(string sessionId, string investigatorName)
    {
        var campaign = CampaignManager.Instance.CurrentCampaign;
        if (campaign == null) return;
        
        Investigator selectedInvestigator = campaign.Investigators
            .FirstOrDefault(inv => inv.Name == investigatorName);

        if (selectedInvestigator != null && _playerSessions.TryGetValue(sessionId, out PlayerSession session))
        {
            session.ChosenInvestigator = selectedInvestigator;
            GD.Print($"Player {sessionId} selected investigator: {investigatorName}");
            SendFullCharacterData(sessionId, selectedInvestigator);
        }
    }

    private void SendInvestigatorList(string sessionId)
    {
        var campaign = CampaignManager.Instance.CurrentCampaign;
        if (campaign == null) return;

        var investigatorNames = new Godot.Collections.Array();
        foreach (var inv in campaign.Investigators)
        {
            investigatorNames.Add(inv.Name);
        }

        var payload = new Godot.Collections.Dictionary
        {
            { "type", "authentication_success" },
            { "investigators", investigatorNames }
        };
        SendData(sessionId, payload);
    }
    
    private void SendFullCharacterData(string sessionId, Investigator investigator)
    {
        // Build the data dictionary manually with all required fields.
        var investigatorData = new Godot.Collections.Dictionary
        {
            // Personal Info
            { "Name", investigator.Name },
            { "Occupation", investigator.Occupation },
            { "Age", investigator.Age },
            { "Sex", investigator.Sex },

            // Characteristics
            { "Strength", investigator.Strength },
            { "Dexterity", investigator.Dexterity },
            { "Intelligence", investigator.Intelligence },
            { "Constitution", investigator.Constitution },
            { "Appearance", investigator.Appearance },
            { "Power", investigator.Power },
            { "Size", investigator.Size },
            { "Education", investigator.Education },

            // Core Stats
            { "HitPointsCurrent", investigator.HitPointsCurrent },
            { "HitPointsMax", investigator.HitPointsMax },
            { "SanityCurrent", investigator.SanityCurrent },
            { "SanityMax", investigator.SanityMax },
            { "MagicPointsCurrent", investigator.MagicPointsCurrent },
            { "MagicPointsMax", investigator.MagicPointsMax },
            { "LuckCurrent", investigator.LuckCurrent },
            { "LuckMax", investigator.LuckMax },

            // Skills
            { "Skills", investigator.Skills }
        };

        var payload = new Godot.Collections.Dictionary
        {
            { "type", "character_data" },
            { "data", investigatorData } 
        };
        SendData(sessionId, payload);
    }

    private void UpdatePlayerStats(string sessionId, Dictionary<string, long> stats)
    {
        if (_playerSessions.TryGetValue(sessionId, out PlayerSession session) && session.ChosenInvestigator != null)
        {
            var investigator = session.ChosenInvestigator;
            GD.Print($"Updating stats for {investigator.Name} from session {sessionId}");
            
            // Safely update stats that are present in the dictionary
            if (stats.TryGetValue("HitPointsCurrent", out long hp)) investigator.HitPointsCurrent = (int)hp;
            if (stats.TryGetValue("SanityCurrent", out long san)) investigator.SanityCurrent = (int)san;
            if (stats.TryGetValue("MagicPointsCurrent", out long mp)) investigator.MagicPointsCurrent = (int)mp;
            if (stats.TryGetValue("LuckCurrent", out long luck)) investigator.LuckCurrent = (int)luck;

            // Send the full, updated sheet back to the client for confirmation and to sync state.
            SendFullCharacterData(sessionId, investigator);
        }
    }

    public void UpdatePlayerCharacter(Investigator updatedInvestigator)
    {
        if (_wsServer == null || !_wsServer.IsListening) return;

        // Find the session ID for the player controlling this investigator
        string sessionId = _playerSessions
            .FirstOrDefault(pair => pair.Value.ChosenInvestigator == updatedInvestigator)
            .Key;

        // If we found a connected player for this investigator, send them the new data
        if (sessionId != null)
        {
            GD.Print($"Keeper is pushing update for {updatedInvestigator.Name} to session {sessionId}.");
            SendFullCharacterData(sessionId, updatedInvestigator);
        }
    }

    private void SendData(string sessionId, Godot.Collections.Dictionary payload)
    {
        string jsonResponse = Json.Stringify(payload);
        _wsServer.WebSocketServices["/"].Sessions.SendTo(jsonResponse, sessionId);
    }
    
    private string GenerateSessionCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        var code = new char[SESSION_CODE_LENGTH];
        for (int i = 0; i < code.Length; i++)
        {
            code[i] = chars[random.Next(chars.Length)];
        }
        return new string(code);
    }
}