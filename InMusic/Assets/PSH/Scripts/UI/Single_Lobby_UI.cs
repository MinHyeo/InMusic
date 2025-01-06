using UnityEngine;
using UI_BASE_PSH;

public class Single_Lobby_UI : UI_Base
{
    void Start()
    {
        //음악 목록 Load하기

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
                //로비 화면 Load하기
                break;
            case "KeyGuide":
                Guide();
                break;
            default:
                Debug.Log("아직 기능이 없거나 잘못 입력");
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
