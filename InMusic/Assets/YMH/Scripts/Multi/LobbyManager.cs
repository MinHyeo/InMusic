using Fusion;
using UnityEngine;
using System.Threading.Tasks;

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

    public void BackToMainMenu()
    {
        // 메인 메뉴로 돌아가기
        //SceneManager.LoadScene("MainMenu");
    }

    // Create Room
    public void CreateRoom()
    {
        createRoomUI.SetActive(true);
    }

    public async void TryJoinRoom(string roomName)
    {
        await JoinRoomAsync(roomName);
    }

    //Join Room
    public async Task JoinRoomAsync(string roomName)
    {
        try
        {
            var result = await NetworkManager.runnerInstance.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Shared,  // Photon Cloud Server 방식으로 변경
                SessionName = roomName,
            });

            if (result.Ok)
            {
                Debug.Log($"방 '{roomName}'에 성공적으로 접속했습니다.");
            }
            else
            {
                Debug.LogError($"방 접속 실패: {result.ShutdownReason}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"방 접속 중 오류 발생: {e.Message}");
        }
    }

    public void JoinLockedRoom(SessionInfo sessionInfo)
    {
        passwordPopupUI.SetActive(true);
        passwordPopup.SetSessionInfo(sessionInfo);
    }
}
