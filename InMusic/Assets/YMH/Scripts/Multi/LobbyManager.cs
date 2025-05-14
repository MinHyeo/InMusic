using Unity.VisualScripting;
using UnityEngine;

public class LobbyManager : SingleTon<LobbyManager>
{
    // crate room
    [SerializeField]
    private GameObject createRoomUI;
    private CreateMultiRoom createMultiRoom;



    private void Start()
    {
        Initialized();
    }

    public void Initialized()
    {
        // 초기화
        createMultiRoom = createRoomUI.GetComponent<CreateMultiRoom>();
        createMultiRoom.Initialized();
        createRoomUI.SetActive(false);
    }

    // Create Room
    public void CreateRoom(){
        createRoomUI.SetActive(true);
    }
}
