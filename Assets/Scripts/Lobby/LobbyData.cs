using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class LobbyPlayer {
    public string playerName;
    public string playerId;
}

[Serializable]
public class LobbyData {
    public string lobbyName;
    public string lobbyStatus;

    public int maxPlayers;
    public List<LobbyPlayer> players;
}