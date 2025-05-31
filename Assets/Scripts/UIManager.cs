using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviourPunCallbacks
{
    [Header("Lobby")]
    public TMP_InputField createRoom_Input;
    public TMP_InputField joinRoom_Input;
    public GameObject noRoomError;
    public Button createRoomBtn;
    public Button JoinRoomBtn;

    [Header("Scripts Ref")]
    [SerializeField] private LobbyManager lobbyManager;

    #region MonoBehaviou
    public override void OnEnable()
    {
        base.OnEnable();
        createRoomBtn.onClick.AddListener(OnCreateRoom);
        JoinRoomBtn.onClick.AddListener(OnJoinRoom);
    }
    public override void OnDisable()
    {
        base.OnDisable();
        createRoomBtn.onClick.RemoveAllListeners();
        JoinRoomBtn.onClick.RemoveAllListeners();
    }
    #endregion

    #region Buttons Actions
    private void OnCreateRoom() 
    {
        lobbyManager.CreateRoom(createRoom_Input.text);
    }

    private void OnJoinRoom() 
    {
        lobbyManager.JoinRoom(joinRoom_Input.text);
    }
    #endregion

    public IEnumerator DeactivateErrorMessage()
    {
        noRoomError.SetActive(true);
        yield return new WaitForSeconds(2f);
        noRoomError.SetActive(false);
    }
}
