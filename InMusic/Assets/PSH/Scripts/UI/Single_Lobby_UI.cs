using UnityEngine;
using UnityEngine.UI;
using UI_BASE_PSH;

public class Single_Lobby_UI : UI_Base
{
    [SerializeField] private GameObject[] musicItems = new GameObject[17];
    [SerializeField] private GameObject curMusicItem;
    [SerializeField] private GameObject[] curMusicData = new GameObject[4];
    [SerializeField] private GameObject[] curMusic = new GameObject[3];
    [SerializeField] private Text[] logData = new Text[4];
    //스크롤 관련
    [SerializeField]private RectTransform contentPos;
    private float itemGap = 40.0f;


    void Start()
    {
        //음악 목록 Load하기

        curMusicItem = musicItems[3];
        GameManager.Input.SetUIKeyEvent(SingleLobbyKeyEvent);
    }

    void Update()
    {
        //무한 스크롤 (위/아래로 2칸 이상 이동 시 작동)
        if (contentPos.localPosition.y <= 1.45f) {
            contentPos.localPosition = new Vector2(0, 200.0f);
        }
        else if (contentPos.localPosition.y >= 395.0f) {
            contentPos.localPosition = new Vector2(0, 200.0f);
        }
    }

    public void ButtonEvent(string type) {
        switch (type)
        {
            case "Up":
                //TODO
                //contentPos.position -= new Vector2(0, itemGap);
                break;
            case "Down":
                //TODO
                //contentPos.position += new Vector2(0, itemGap);
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
                Gear();
                break;
            case Define.UIControl.Setting:
                ButtonEvent("Gear");
                break;
        }
    }
}
