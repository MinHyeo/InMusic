using Fusion;
using UnityEngine;

public class LobbyManager : SingleTon<LobbyManager>
{
    // crate room
    [SerializeField]
    private GameObject createRoomUI;
    private CreateMultiRoom createMultiRoom;

    [SerializeField]
    private GameObject passwordPopupUI;
    private PasswordPopup passwordPopup;

    private void Start()
    {
        Initialized();
    }

    public void Initialized()
    {
        // 초기화
        createMultiRoom = createRoomUI.GetComponent<CreateMultiRoom>();
        passwordPopup = passwordPopupUI.GetComponent<PasswordPopup>();

        createMultiRoom.Initialized();
        createRoomUI.SetActive(false);
    }

    // Create Room
    public void CreateRoom()
    {
        createRoomUI.SetActive(true);
    }

    //Join Room
    public void JoinRoom(string roomName)
    {
        NetworkManager.runnerInstance.StartGame(new StartGameArgs()
        {
            SessionName = roomName,
            GameMode = GameMode.Shared,
        });
    }

    public void JoinLockedRoom(SessionInfo sessionInfo)
    {
        passwordPopupUI.SetActive(true);
        passwordPopup.SetSessionInfo(sessionInfo);
    }
}
