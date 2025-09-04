using UnityEngine;
using UnityEngine.UI;

public class RoomNameUI : MonoBehaviour
{
    [SerializeField] private Text roomNameText;
    
    void Start()
    {
        // 컴포넌트가 지정되지 않았으면 자동으로 찾기
        if (roomNameText == null)
        {
            roomNameText = GetComponent<Text>();
        }
        
        // MultiRoomManager에서 방 이름 가져와서 설정
        if (roomNameText != null && MultiRoomManager.Instance != null)
        {
            roomNameText.text = MultiRoomManager.Instance.RoomName;
            Debug.Log($"[RoomNameUI] Set room name to: {MultiRoomManager.Instance.RoomName}");
        }
    }
    
}