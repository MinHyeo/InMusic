using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobby_UI : MonoBehaviour
{
    [Header("UI Mode")]
    [SerializeField]private bool isSolo = true;
    [Header("UI Button")]
    [Tooltip("Start���� �ڵ����� �Ҵ� ��")]
    [SerializeField] private GameObject buttonRoot;
    [SerializeField] private Dictionary<string, GameObject> buttons = new Dictionary<string, GameObject>();

    [Tooltip("�̰� GameManaer���� �޾ƿ� ����")]
    [SerializeField]public InputManager temp = new InputManager();

    void Start()
    {
        buttonRoot = GameObject.Find("ButtonRoot");
        int numOfChild = buttonRoot.transform.childCount;
        for (int i = 0; i < numOfChild; i++) {
            GameObject temp = buttonRoot.transform.GetChild(i).gameObject;
            buttons.Add(temp.name, temp);
        }
        buttons["Solo"].gameObject.SetActive(isSolo);
        buttons["Multi"].gameObject.SetActive(!isSolo);

        //�ߺ� �Է� ����
        temp.keyPress -= KeyEvent;
        temp.keyPress += KeyEvent;
    }

    // Update is called once per frame
    void Update()
    {
        temp.OnUpdate();
    }

    //��ư ���
    public void ButtonEvent(string Type) {
        switch (Type) {
            //����
            case "Gear":
                //Todo
                Debug.Log("Gear function is not implemented");
                break;
            //������
            case "Exit":
                Debug.Log("Exit function is not implemented");
                break;
            //����
            case "Left":
                isSolo = !isSolo;
                buttons["Solo"].gameObject.SetActive(!isSolo);
                buttons["Multi"].gameObject.SetActive(isSolo);
                //Animation Control

                break;
            //������
            case "Right":
                isSolo = !isSolo;
                buttons["Solo"].gameObject.SetActive(!isSolo);
                buttons["Multi"].gameObject.SetActive(isSolo);
                //Animation Control

                break;
            //�ַ�
            case "Solo":
                //Todo
                Debug.Log("Solo function is not implemented");
                break;
            //��Ƽ
            case "Multi":
                //Todo
                Debug.Log("Multi function is not implemented");
                break;
            case "KeyGuide":
                //Todo
                Debug.Log("KeyGuide function is not implemented");
                break;
            default:
                Debug.Log("���� ����� ���ų� �߸� �Է�");
                break;
        }
    }

    //ButtonEvent�� �� �ѱ�
    public void KeyEvent(Define.UIControl keyEven) {
        switch (keyEven)
        {
            case Define.UIControl.Right:
                ButtonEvent("Right");
                break;
            case Define .UIControl.Left:
                ButtonEvent("Left");
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
}
