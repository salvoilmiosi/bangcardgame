using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SWNetwork;

[Serializable]
public class MessageData {
    public string message;
}

public class LobbyChat : MonoBehaviour
{
    public InputField chatInputField;
    public GameObject chatContent;

    void Start() {
        NetworkClient.Lobby.OnRoomMessageEvent += OnRoomMessage;
    }

    void onDestroy() {
        NetworkClient.Lobby.OnRoomMessageEvent -= OnRoomMessage;
    }

    public void OnChatSend() {
        if (chatInputField.text != null && chatInputField.text.Length > 0) {
            MessageData messageData = new MessageData();
            messageData.message = chatInputField.text;
            NetworkClient.Lobby.MessageRoom(messageData, (success, error) => {
                if (!success) {
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
        Debug.Log(messageData.message);
    }
}