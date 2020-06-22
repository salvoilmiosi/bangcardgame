using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyEntry : MonoBehaviour
{
    public Text lobbyName;

    public Text lobbyStatus;

    public Text playerCount;

    public delegate void JoinFunctionDelegate();

    public JoinFunctionDelegate OnJoin;

    public void OnJoinClicked() {
        OnJoin();
    }
}
