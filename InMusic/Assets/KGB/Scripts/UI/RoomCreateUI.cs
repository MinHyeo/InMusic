using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomCreateUI : MonoBehaviour
{
    public TMP_InputField roomNameInput, roomPasswordInput;
    public Toggle passwordToggle;
    public NetworkManager networkManager;

    void Awake()
    {
        Init();
    }

    void Start()
    {
        // 토글이 변경될 때 실행할 메서드 등록
        passwordToggle.onValueChanged.AddListener(OnPasswordToggleChanged);
        // 시작 시 토글 상태 반영
        OnPasswordToggleChanged(passwordToggle.isOn);

    }

    void OnPasswordToggleChanged(bool isOn) 
    {
        // 토글이 변경될 때 실행되는 메서드
        roomPasswordInput.interactable = isOn;
        if (!passwordToggle.isOn)
        {
            roomPasswordInput.text = string.Empty;
        }
    }

    public void CreateRoom()
    {
        if (roomNameInput.text != string.Empty)
        {
            networkManager.CreateSession(roomNameInput.text);
        }
    }

    public void CancelButton()
    {
        Init();
        gameObject.SetActive(false);
    }

    private void Init()
    {
        roomNameInput.text = string.Empty;
        roomPasswordInput.text = string.Empty;
        passwordToggle.isOn = false;
        gameObject.SetActive(false);
    }
}
