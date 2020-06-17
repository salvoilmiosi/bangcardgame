using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LobbyPlayer {
    public string name;
    public bool connected;
    public bool alive;
}

[Serializable]
public class LobbyData {
    public string name;
    public List<LobbyPlayer> players = new List<LobbyPlayer>();
}