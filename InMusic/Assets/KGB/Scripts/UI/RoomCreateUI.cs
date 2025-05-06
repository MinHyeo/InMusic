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
        // ����� ����� �� ������ �޼��� ���
        passwordToggle.onValueChanged.AddListener(OnPasswordToggleChanged);
        // ���� �� ��� ���� �ݿ�
        OnPasswordToggleChanged(passwordToggle.isOn);

    }

    void OnPasswordToggleChanged(bool isOn) 
    {
        // ����� ����� �� ����Ǵ� �޼���
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
