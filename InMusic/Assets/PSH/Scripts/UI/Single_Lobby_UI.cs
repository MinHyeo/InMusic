using UnityEngine;
using UI_BASE_PSH;

public class Single_Lobby_UI : UI_Base
{
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
            case "Gear":
                Gear();
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
                ButtonEvent("KeyGuide");
                break;
            case Define.UIControl.Setting:
                ButtonEvent("Gear");
                break;
        }
    }
}
