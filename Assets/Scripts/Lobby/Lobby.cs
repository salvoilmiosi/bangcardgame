using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SWNetwork;

public class Lobby : MonoBehaviour
{
    public InputField playerNameInputField;
    public InputField lobbyNameInputField;
    public GameObject playerRegisterScreen;
    public GameObject lobbyScreen;
    public GameObject playerListScreen;

    public GameObject lobbyList;
    public GameObject playerList;

    public GameObject lobbyEntryPrefab;
    public GameObject playerEntryPrefab;

    private int selectedPage = 0;
    private int totalPages = 0;
    public int lobbyCount = 10;

    private LobbyPlayer playerData;
    private LobbyData lobbyData;

    private const int MAX_PLAYERS = 10;

    void Start() {
        playerRegisterScreen.SetActive(true);
        lobbyScreen.SetActive(false);
        playerListScreen.SetActive(false);

        NetworkClient.Lobby.OnNewPlayerJoinRoomEvent += OnNewPlayerJoinRoom;
        NetworkClient.Lobby.OnPlayerLeaveRoomEvent += OnPlayerLeaveRoom;
        NetworkClient.Lobby.OnRoomCustomDataChangeEvent += OnRoomCustomDataChange;

        NetworkClient.Lobby.OnLobbyConnectedEvent += OnLobbyConnected;

        NetworkClient.Lobby.OnRoomStartingEvent += OnRoomStarting;
    }

    void onDestroy() {
        NetworkClient.Lobby.OnNewPlayerJoinRoomEvent -= OnNewPlayerJoinRoom;
        NetworkClient.Lobby.OnPlayerLeaveRoomEvent -= OnPlayerLeaveRoom;
        NetworkClient.Lobby.OnRoomCustomDataChangeEvent -= OnRoomCustomDataChange;

        NetworkClient.Lobby.OnLobbyConnectedEvent -= OnLobbyConnected;

        NetworkClient.Lobby.OnRoomStartingEvent -= OnRoomStarting;
    }

    public void OnConnect() {
        if (playerNameInputField.text != null && playerNameInputField.text.Length > 0) {
            NetworkClient.Instance.CheckIn(playerNameInputField.text, (success, error) => {
                if (!success) {
                    Debug.Log("Error on checking in: " + error);
                }
            });
        }
    }

    private void OnLobbyConnected() {
        playerData = new LobbyPlayer();
        playerData.playerName = playerNameInputField.text;
        playerData.playerId = NetworkClient.Lobby.PlayerId;
        NetworkClient.Lobby.Register(playerData, (success, reply, error) => {
            if (success) {
                Debug.Log("Player registered: " + reply);
                string lobbyId = reply.roomId;
                if (lobbyId == null) {
                    playerRegisterScreen.SetActive(false);
                    lobbyScreen.SetActive(true);
                    playerListScreen.SetActive(false);
                    OnLobbyRefresh();
                } else if (reply.started) {
                    ConnectToRoom();
                } else {
                    GotoRoom();
                }
            } else {
                Debug.Log("Error registering player: " + error);
            }
        });
    }

    public void OnLobbyPrev() {
        if (selectedPage > 0) {
            --selectedPage;
            OnLobbyRefresh();
        }
    }

    public void OnLobbyNext() {
        if (selectedPage < totalPages - 1) {
            ++selectedPage;
            OnLobbyRefresh();
        }
    }

    public void OnLobbyRefresh() {
        NetworkClient.Lobby.GetRooms(selectedPage, lobbyCount, (success, reply, error) => {
            if (success) {
                Debug.Log("Got rooms: " + reply);
                totalPages = reply.totalPages;
                foreach (Transform child in lobbyList.transform) {
                    GameObject.Destroy(child.gameObject);
                }
                foreach (SWRoom room in reply.rooms) {
                    AddLobbyEntry(room.id, room.GetCustomData<LobbyData>());
                }
            } else {
                Debug.Log("Error getting lobbies: " + error);
            }
        });
    }

    private void AddLobbyEntry(string lobbyId, LobbyData lobbyData) {
        GameObject obj = Instantiate(lobbyEntryPrefab);
        obj.transform.parent = lobbyList.transform;

        LobbyEntry lobbyEntry = obj.GetComponent<LobbyEntry>();
        lobbyEntry.lobbyName.text = lobbyData.lobbyName;
        lobbyEntry.lobbyStatus.text = lobbyData.lobbyStatus;
        lobbyEntry.playerCount.text = lobbyData.players.Count + "/" + lobbyData.maxPlayers;

        lobbyEntry.OnJoin = () => {
            Debug.Log("Cliccato bottone join: " + lobbyId);
            OnJoinLobby(lobbyId);
        };
    }

    public void OnNewLobby() {
        if (lobbyNameInputField.text != null && lobbyNameInputField.text.Length > 0) {
            lobbyData = new LobbyData();
            lobbyData.lobbyName = lobbyNameInputField.text;
            lobbyData.maxPlayers = MAX_PLAYERS;
            lobbyData.lobbyStatus = "Waiting";
            lobbyData.players = new List<LobbyPlayer>();
            lobbyData.players.Add(playerData);

            NetworkClient.Lobby.CreateRoom(lobbyData, false, MAX_PLAYERS, (success, lobbyId, error) => {
                if (success) {
                    Debug.Log("Lobby created: " + lobbyId);
                    GotoRoom();
                } else {
                    Debug.Log("Error creating lobby: " + error);
                }
            });
        }
    }

    public void OnJoinLobby(string lobbyId) {
        NetworkClient.Lobby.JoinRoom(lobbyId, (success, reply, error) => {
            if (success) {
                Debug.Log("Joined room: " + reply);
                GotoRoom();
            } else {
                Debug.Log("Error joining lobby: " + error);
            }
        });
    }

    private void GotoRoom() {
        playerRegisterScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        playerListScreen.SetActive(true);
        RefreshPlayerList();
    }

    private void RefreshPlayerList() {
        NetworkClient.Lobby.GetPlayersInRoom((success, reply, error) => {
            if (success) {
                foreach (Transform child in playerList.transform) {
                    GameObject.Destroy(child.gameObject);
                }
                foreach (SWPlayer player in reply.players) {
                    LobbyPlayer playerData = player.GetCustomData<LobbyPlayer>();
                    AddPlayerEntry(playerData);
                }
            } else {
                Debug.Log("Error getting players in room: " + error);
            }
        });
    }

    private void OnNewPlayerJoinRoom(SWJoinRoomEventData eventData) {
        if (NetworkClient.Lobby.IsOwner) {
            LobbyPlayer newPlayer = eventData.GetMessageData<LobbyPlayer>();
            lobbyData.players.Add(playerData);
            NetworkClient.Lobby.ChangeRoomCustomData(lobbyData, (success, error) => {
                if (success) {
                    RefreshPlayerList();
                } else {
                    Debug.Log("Error changing lobby data: " + error);
                }
            });
        }
    }

    private void OnPlayerLeaveRoom(SWLeaveRoomEventData eventData) {
        if (NetworkClient.Lobby.IsOwner) {
            foreach (string id in eventData.leavePlayerIds) {
                lobbyData.players.RemoveAll((player) => { return player.playerId.Equals(id); });
            }
            NetworkClient.Lobby.ChangeRoomCustomData(lobbyData, (success, error) => {
                if (success) {
                    RefreshPlayerList();
                } else {
                    Debug.Log("Error changing lobby data: " + error);
                }
            });
        }
    }

    private void OnRoomCustomDataChange(SWRoomCustomDataChangeEventData eventdata) {
        RefreshPlayerList();
    }

    private void AddPlayerEntry(LobbyPlayer playerData) {
        GameObject obj = Instantiate(playerEntryPrefab);
        obj.transform.parent = playerList.transform;

        PlayerEntry playerEntry = obj.GetComponent<PlayerEntry>();
        playerEntry.playerName.text = playerData.playerName;
    }

    private void OnRoomStarting(SWStartRoomEventData eventData) {
        Debug.Log("Room is starting");
        ConnectToRoom();
    }

    public void OnStartGame() {
        if (NetworkClient.Lobby.IsOwner) {
            NetworkClient.Lobby.StartRoom((success, error) => {
                if (success) {
                    ConnectToRoom();
                } else {
                    Debug.Log("Error starting room: " + error);
                }
            });
        }
    }

    private void ConnectToRoom() {
        NetworkClient.Instance.ConnectToRoom((success) => {
            if (success) {
                Debug.Log("Game started");
                // go to game scene
            }
        });
    }

    public void OnLeaveLobby() {
        NetworkClient.Lobby.LeaveRoom((success, error) => {
            if (success) {
                playerRegisterScreen.SetActive(false);
                lobbyScreen.SetActive(true);
                playerListScreen.SetActive(false);
                OnLobbyRefresh();
            } else {
                Debug.Log("Error leaving lobby: " + error);
            }
        });
    }
}
