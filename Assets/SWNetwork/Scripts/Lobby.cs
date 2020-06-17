using UnityEngine;
using SWNetwork; 
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Basic lobby matchmaking implementation.
/// </summary>
public class Lobby : MonoBehaviour
{
    /// <summary>
    /// Button for checking into SocketWeaver services
    /// </summary>
    public Button registerButton;

    /// <summary>
    /// Button for joining or creating room
    /// </summary>
    public Button playButton;

    /// <summary>
    /// Button for entering custom playerId
    /// </summary>
    public InputField customPlayerIdField;

    void Start()
    {
        // Add an event handler for the OnRoomReadyEvent
        NetworkClient.Lobby.OnRoomReadyEvent += Lobby_OnRoomReadyEvent;

        // Add an event handler for the OnFailedToStartRoomEvent
        NetworkClient.Lobby.OnFailedToStartRoomEvent += Lobby_OnFailedToStartRoomEvent;

        // Add an event handler for the OnLobbyConnectedEvent
        NetworkClient.Lobby.OnLobbyConnectedEvent += Lobby_OnLobbyConnectedEvent;

        // allow player to register
        registerButton.gameObject.SetActive(true);
        playButton.gameObject.SetActive(false);
    }

    void onDestroy()
    {
        // remove the handlers
        NetworkClient.Lobby.OnRoomReadyEvent -= Lobby_OnRoomReadyEvent;
        NetworkClient.Lobby.OnFailedToStartRoomEvent -= Lobby_OnFailedToStartRoomEvent;
        NetworkClient.Lobby.OnLobbyConnectedEvent -= Lobby_OnLobbyConnectedEvent;
    }

    /* Lobby events handlers */
    void Lobby_OnRoomReadyEvent(SWRoomReadyEventData eventData)
    {
        Debug.Log("Room is ready: roomId= " + eventData.roomId);
        // Room is ready to join and its game servers have been assigned.
        ConnectToRoom();
    }

    void Lobby_OnFailedToStartRoomEvent(SWFailedToStartRoomEventData eventData)
    {
        Debug.Log("Failed to start room: " + eventData);
    }

    void Lobby_OnLobbyConnectedEvent()
    {
        Debug.Log("Lobby connected");
        RegisterPlayer();
    }

    /* UI event handlers */
    /// <summary>
    /// Register button was clicked
    /// </summary>
    public void Register()
    {
        string customPlayerId = customPlayerIdField.text;

        if(customPlayerId != null && customPlayerId.Length > 0)
        {
            // use the user entered playerId to check into SocketWeaver. Make sure the PlayerId is unique.
            NetworkClient.Instance.CheckIn(customPlayerId,(bool ok, string error) =>
            {
                if (!ok)
                {
                    Debug.LogError("Check-in failed: " + error);
                }
            });
        }
        else
        {
            // use a randomly generated playerId to check into SocketWeaver.
            NetworkClient.Instance.CheckIn((bool ok, string error) =>
            {
                if (!ok)
                {
                    Debug.LogError("Check-in failed: " + error);
                }
            });
        }
    }

    public LobbyList lobbyList;

    public void GetRooms() {
	// Get the rooms for the current page.
        NetworkClient.Lobby.GetRooms(0, 6, (successful, reply, error) => {
            if (successful) {
                Debug.Log("Got rooms " + reply);
                
                // Remove rooms in the rooms list
                //GUI.ClearRoomList();

                foreach (SWRoom room in reply.rooms) {
                    Debug.Log(room);
                    // Deserialize the room custom data.
                    LobbyData rData = room.GetCustomData<LobbyData>();
                    if (rData != null) {
                        // Add rooms to the rooms list.
                        
                        lobbyList.AddRowForRoom(rData.name, room.id);
                    }
                }
            } else {
                Debug.Log("Failed to get rooms " + error);
            }
        });
    }
    public void OnRoomSelected(string roomId)
    {
        Debug.Log("OnRoomSelected: " + roomId);
        // Join the selected room
        NetworkClient.Lobby.JoinRoom(roomId, (successful, reply, error) =>
        {
            if (successful)
            {
                Debug.Log("Joined room " + reply);
                // refresh the player list
                //GetPlayersInCurrentRoom();
            }
            else
            {
                Debug.Log("Failed to Join room " + error);
            }
        });
    }

    /// <summary>
    /// Play button was clicked
    /// </summary>
    public void Play()
    {
        // Here we use the JoinOrCreateRoom method to get player into rooms quickly.
        NetworkClient.Lobby.JoinOrCreateRoom(true, 2, 60, HandleJoinOrCreatedRoom);
    }

    /* Lobby helper methods*/
    /// <summary>
    /// Register the player to lobby
    /// </summary>
    void RegisterPlayer()
    {
        NetworkClient.Lobby.Register((successful, reply, error) =>
        {
            if (successful)
            {
                Debug.Log("Registered " + reply);

                if (reply.started)
                {
                    // player is already in a room and the room has started.
                    // We can connect to the room's game servers now.
                    ConnectToRoom();
                }
                else
                {
                    // allow player to join or create room
                    playButton.gameObject.SetActive(true);
                    registerButton.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.Log("Failed to register " + error);
            }
        });
    }

    /// <summary>
    /// Callback method for NetworkClient.Lobby.JoinOrCreateRoom().
    /// </summary>
    /// <param name="successful">If set to <c>true</c> <paramref name="successful"/>, the player has joined or created a room.</param>
    /// <param name="reply">Reply.</param>
    /// <param name="error">Error.</param>
    void HandleJoinOrCreatedRoom(bool successful, SWJoinRoomReply reply, SWLobbyError error)
    {
        if (successful)
        {
            Debug.Log("Joined or created room " + reply);

            // the player has joined a room which has already started.
            if (reply.started)
            {
                ConnectToRoom();
            }
            else if (NetworkClient.Lobby.IsOwner)
            {
                // the player did not find a room to join
                // the player created a new room and became the room owner.
                StartRoom();
            }
        }
        else
        {
            Debug.Log("Failed to join or create room " + error);
        }
    }

    /// <summary>
    /// Start local player's current room. Lobby server will ask SocketWeaver to assign suitable game servers for the room.
    /// </summary>
    void StartRoom()
    {
        NetworkClient.Lobby.StartRoom((okay, error) =>
        {
            if (okay)
            {
                // Lobby server has sent request to SocketWeaver. The request is being processed.
                // If socketweaver finds suitable server, Lobby server will invoke the OnRoomReadyEvent.
                // If socketweaver cannot find suitable server, Lobby server will invoke the OnFailedToStartRoomEvent.
                Debug.Log("Started room");
            }
            else
            {
                Debug.Log("Failed to start room " + error);
            }
        });
    }

    /// <summary>
    /// Connect to the game servers of the room.
    /// </summary>
    void ConnectToRoom()
    {
        NetworkClient.Instance.ConnectToRoom(HandleConnectedToRoom);
    }

    /// <summary>
    /// Callback method NetworkClient.Instance.ConnectToRoom();
    /// </summary>
    /// <param name="connected">If set to <c>true</c>, the client has connected to the game servers successfully.</param>
    void HandleConnectedToRoom(bool connected)
    {
        if (connected)
        {
            Debug.Log("Connected to room");
            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.Log("Failed to connect to room");
        }
    }
}