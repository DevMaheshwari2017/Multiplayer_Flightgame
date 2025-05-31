using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private byte gameScene = 1;
    [SerializeField] private byte maxPlayersPerRoom = 3;

    [Header("Scripts ref")]
    [SerializeField] private UIManager uiManager;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master successfully");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
    }

    #endregion

    #region CreatAndJoinRoom
    public void CreateRoom(string roomName) 
    {
        RoomOptions roomOptions = new RoomOptions {MaxPlayers = maxPlayersPerRoom };
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.CreateRoom(roomName);
            Debug.Log("Room: " + roomName + ", created successfully");
        }
        else
        {
            Debug.LogWarning("Not connected to Master yet.");
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
        Debug.Log("Room: " + roomName + ", Joined successfully");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(gameScene);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
       StartCoroutine(uiManager.DeactivateErrorMessage());
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room created successfully: " + PhotonNetwork.CurrentRoom.Name);
    }
    #endregion
}
