using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Dictionary<int, Player> Players = new Dictionary<int, Player>();
    public Dictionary<int, Team> Teams = new Dictionary<int, Team>();
    public int TeamCount = 2;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        this.InitGame();
    }

    private void InitGame()
    {
        for (int i = 1; i <= this.TeamCount; i++ )
        {
            Team team = new Team(i, $"Team {i}");
            this.Teams.Add(team.id, team);
        }
    }

    private Team GetAutoBalanceTeam()
    {
        int smallestTeamSize = 0;
        Team smallestTeam = null;

        foreach (Team team in this.Teams.Values)
        {
            if (smallestTeam == null)
            {
                smallestTeam = team;
                smallestTeamSize = team.Players.Count;
            }
            else if (team.Players.Count < smallestTeamSize)
            {
                smallestTeam = team;
            }
        }

        return smallestTeam;
    }

    public static void NewPlayer(int fromClient, Packet packet)
    {
        GameManager gameManager = GameManager.instance;
        int clientId = packet.ReadInt();

        if (fromClient != clientId)
        {
            Debug.Log($"Player (ID: {fromClient}) has assumed the wrong client ID ({clientId})!");
            return;
        }

        string name = packet.ReadString();

        if (name.Trim() == "")
        {
            name = $"Player {clientId}";
        }

        Player player = new Player(clientId, name);

        Team team = gameManager.GetAutoBalanceTeam();
        player.team = team;
        team.Players.Add(player.id, player);

        gameManager.Players.Add(player.id, player);
        Debug.Log($"Adding new player: {player.name}");
        gameManager.UpdateGameData();
    }

    private void UpdateGameData()
    {
        using (Packet packet = new Packet((int)ServerPackets.udpateGameData))
        {
            packet.Write(this.Teams.Count);

            foreach(Team team in this.Teams.Values)
            {
                packet.Write(team.id);
                packet.Write(team.name);
            }
            
            packet.Write(this.Players.Count);

            foreach(Player player in this.Players.Values)
            {
                packet.Write(player.id);
                packet.Write(player.name);
                packet.Write(player.team.id);
            }

            ServerSend.SendTCPDataToAll(packet);
        }
    }

    public static void HandleRequestChangeTeam(int fromClient, Packet packet)
    {
        int clientId = packet.ReadInt();

        if (fromClient != clientId)
        {
            Debug.Log($"Player (ID: {fromClient}) has assumed the wrong client ID ({clientId})!");
            return;
        }

        int teamId = packet.ReadInt();
        Player player = instance.Players[fromClient];
        Team oldTeam = player.team;
        Team newTeam = instance.Teams[teamId];

        oldTeam.Players.Remove(player.id);
        newTeam.Players.Add(player.id, player);

        player.team = newTeam;

        instance.UpdateGameData();
    }

    public static void HandleRequestStartGame(int fromClient, Packet packet)
    {
        int clientId = packet.ReadInt();

        if (fromClient != clientId)
        {
            Debug.Log($"Player (ID: {fromClient}) has assumed the wrong client ID ({clientId})!");
            return;
        }

        instance.StartGame();
    }

    public void StartGame()
    {
        Vector3 position = Vector3.zero;
        foreach (Team team in instance.Teams.Values)
        {
            Boat boat = BoatManager.instance.SpawnBoat(team, position, Quaternion.identity);
            position.y += 3;

            foreach (Player player in team.Players.Values)
            {
                PirateManager.instance.SpawnPirate(player, boat);
            }
        }

        BoatManager.SendBoatsToPlayers();
        PirateManager.instance.SendPirates();

        using (Packet packet = new Packet((int)ServerPackets.gameStart))
        {
            ServerSend.SendTCPDataToAll(packet);
        }
    }
}
