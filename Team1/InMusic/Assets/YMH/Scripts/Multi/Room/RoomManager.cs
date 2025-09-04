using UnityEngine;

public class RoomManager : Singleton<RoomManager>
{
    private string songName = "Heya";
    private string _roomName;
    
    public string RoomName => _roomName;
    
    public void SetRoomName(string roomName)
    {
        _roomName = roomName;
        Debug.Log($"[RoomManager] Room name set to: {roomName}");
    }
}
