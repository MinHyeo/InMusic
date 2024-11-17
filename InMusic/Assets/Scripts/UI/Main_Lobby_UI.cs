using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI_BASE_PSH;
public class Main_Lobby_UI : UI_Base
{
    [Header("UI Mode")]
    [SerializeField]private bool isSolo = true;
    [SerializeField]private bool isChange = false;
    [Header("UI Button")]
    [Tooltip("Start에서 자동으로 할당 됨")]
    [SerializeField] private GameObject buttonRoot;
    [SerializeField] private Dictionary<string, GameObject> buttons = new Dictionary<string, GameObject>();
    [Tooltip("이거 GameManaer에서 받아올 예정")]
    [SerializeField]public InputManager itemp = new InputManager();

    void Start()
    {
        buttonRoot = GameObject.Find("ButtonRoot");
        int numOfChild = buttonRoot.transform.childCount;
        for (int i = 0; i < numOfChild; i++) {
            GameObject temp = buttonRoot.transform.GetChild(i).gameObject;
            buttons.Add(temp.name, temp);
        }
        //중복 입력 방지
        itemp.uIKeyPress -= KeyEvent;
        itemp.uIKeyPress += KeyEvent;
    }

    // Update is called once per frame
    void Update()
    {
        itemp.UIUpdate();
    }

    //버튼 기능
    public void ButtonEvent(string type) {
        switch (type) {
            //설정
            case "Gear":
                //Todo
                Gear();
                break;
            //나가기
            case "Exit":
                    rtemp.Instantiate("Message_UI");
                break;
            //왼쪽
            case "Left":
                if (isSolo){ //Solo To Munti
                    buttons["Solo"].gameObject.GetComponent<Animator>().Play("Exit_To_Right");
                    buttons["Multi"].gameObject.GetComponent<Animator>().Play("Enter_From_Left");
                }
                else{ //Multi To Solo
                    buttons["Solo"].gameObject.GetComponent<Animator>().Play("Enter_From_Left");
                    buttons["Multi"].gameObject.GetComponent<Animator>().Play("Exit_To_Right");
                }
                isSolo = !isSolo;
                break;
            //오른쪽
            case "Right":
                if (isSolo) { //Solo To Multi
                    buttons["Solo"].gameObject.GetComponent<Animator>().Play("Exit_To_Left");
                    buttons["Multi"].gameObject.GetComponent<Animator>().Play("Enter_From_Right");
                }
                else{ //Multi To Solo
                    buttons["Solo"].gameObject.GetComponent<Animator>().Play("Enter_From_Right");
                    buttons["Multi"].gameObject.GetComponent<Animator>().Play("Exit_To_Left");
                }
                isSolo = !isSolo;
                break;
            //솔로
            case "Solo":
                //Todo
                Debug.Log("Solo function is not implemented");
                break;
            //멀티
            case "Multi":
                //Todo
                Debug.Log("Multi function is not implemented");
                break;
            case "KeyGuide":
                //Todo
                Guide();
                break;
            default:
                Debug.Log("아직 기능이 없거나 잘못 입력");
                break;
        }
    }

    //ButtonEvent로 다 넘김
    public void KeyEvent(Define.UIControl keyEven) {
        switch (keyEven)
        {
            case Define.UIControl.Right:
                if (!isChange)
                {
                    ButtonEvent("Right");
                    isChange = true;
                }
                break;
            case Define .UIControl.Left:
                if (!isChange) 
                {
                    ButtonEvent("Left");
                    isChange = true;
                }
                break;
            case Define.UIControl.Up:
                
                break;
            case Define.UIControl.Down:
                
                break;
            case Define.UIControl.Enter:
                string Mode = isSolo ? "Solo" : "Multi";
                ButtonEvent(Mode);
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
    public void GetChangeSignal() {
        isChange = false;
    }
}
