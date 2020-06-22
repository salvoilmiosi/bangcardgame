using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SWNetwork;

[Serializable]
public class MessageData {
    public LobbyPlayer playerData;
    public string message;
}

public class LobbyChat : MonoBehaviour
{
    public InputField chatInputField;
    public GameObject chatContent;

    public Text textPrefab;

    void Start() {
        NetworkClient.Lobby.OnRoomMessageEvent += OnRoomMessage;
    }

    void onDestroy() {
        NetworkClient.Lobby.OnRoomMessageEvent -= OnRoomMessage;
    }

    public void OnChatSend() {
        if (chatInputField.text != null && chatInputField.text.Length > 0) {
            MessageData messageData = new MessageData();
            messageData.playerData = Lobby.getPlayerData();
            messageData.message = chatInputField.text;
            NetworkClient.Lobby.MessageRoom(messageData, (success, error) => {
                if (success) {
                    displayMessage(messageData);
                } else {
                    Debug.Log("Error sending lobby message: " + error);
                }
            });
        }
    }

    void OnRoomMessage(SWMessageRoomEventData eventData) {
        MessageData messageData = eventData.GetMessageData<MessageData>();
        displayMessage(messageData);
    }

    private void displayMessage(MessageData messageData) {
        Text obj = Instantiate(textPrefab);
        obj.transform.parent = chatContent.transform;
        obj.text = messageData.playerData.playerName + " : " + messageData.message;
    }
}