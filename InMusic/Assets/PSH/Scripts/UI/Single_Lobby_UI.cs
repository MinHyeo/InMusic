using UnityEngine;
using UnityEngine.UI;
using UI_BASE_PSH;

public class Single_Lobby_UI : UI_Base
{
    [SerializeField] private GameObject[] musicData = new GameObject[4];
    [SerializeField] private Text[] logData = new Text[4];
    void Start()
    {
        //���� ��� Load�ϱ�

        GameManager.Input.SetUIKeyEvent(SingleLobbyKeyEvent);
    }

    void Update()
    {

    }

    public void ButtonEvent(string type) {
        switch (type)
        {
            case "Up":
                //TODO

                break;
            case "Down":
                //TODO

                break;
            case "Exit":
                //TODO
                //�κ� ȭ�� Load�ϱ�
                break;
            case "KeyGuide":
                Guide();
                break;
            default:
                Debug.Log("���� ����� ���ų� �߸� �Է�");
                break;
        }
    }

    void SingleLobbyKeyEvent(Define.UIControl keyEvent) 
    {
        switch (keyEvent)
        {
            case Define.UIControl.Up:
                ButtonEvent("Up");
                break;
            case Define.UIControl.Down:
                ButtonEvent("Down");
                break;
            case Define.UIControl.Enter:
                ButtonEvent("Enter");
                break;
            case Define.UIControl.Esc:
                ButtonEvent("Exit");
                break;
            case Define.UIControl.Guide:
                Gear();
                break;
            case Define.UIControl.Setting:
                ButtonEvent("Gear");
                break;
        }
    }
}
