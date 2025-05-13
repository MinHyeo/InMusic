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
        if (roomNameInput.text == string.Empty) //방 이름을 입력 안했을 때
        {
            return;
        }

        if (passwordToggle.isOn) //비번 체크 on
        {
            if (roomPasswordInput == null) // 비번 체크하고 비번을 입력 안했을 때
            {
                return;
            }
            else // 비번 체크하고 비번 입력 완료
            {
                networkManager.CreateSession(roomNameInput.text, roomPasswordInput.text);
            }
        }
        else //비번 체크 off
        {
            networkManager.CreateSession(roomPasswordInput.text);
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
