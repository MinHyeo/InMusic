using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class Key_Setting_UI : MonoBehaviour
{
    [Header("Setting List")]
    [Tooltip("Start���� �ڵ����� �Ҵ� ��")]
    [SerializeField] private GameObject[] menuList;
    [SerializeField] private Text[] menuValues;
    [SerializeField] private GameObject message;
    [SerializeField] private int numOfMenuList;
    [Header("Currentyl selected menue")]
    [SerializeField] private GameObject curMenu;
    [SerializeField] int curMenuIndex = 0;
    [SerializeField] Define.NoteControl curNote = Define.NoteControl.Key1;
    [SerializeField] bool isSetMode = false;
    public bool _isSetting = false;

    void Start()
    {
        numOfMenuList = transform.GetChild(0).childCount - 3;
        menuList = new GameObject[numOfMenuList];
        menuValues = new Text[numOfMenuList];
        for (int i = 0; i < numOfMenuList; i++) {
            menuList[i] = transform.GetChild(0).GetChild(i + 3).gameObject;
            menuValues[i] = menuList[i].transform.GetChild(1).transform.GetChild(1).GetComponent<Text>();
        }

        message = transform.GetChild(0).GetChild(2).gameObject;
        message.SetActive(isSetMode);

        curMenu = menuList[curMenuIndex];
        ChangeMenu();

        GameManager.Input.SetUIKeyEvent(KeySetKeyEvent);
    }

    public void ControlMenu(string arrow)
    {
        switch (arrow)
        {
            case "Up":
                curMenuIndex--;
                ChangeMenu();
                break;
            case "Down":
                curMenuIndex++;
                ChangeMenu();
                break;
        }
    }

    public void ButtonEvent(string type)
    {
        switch (type)
        {
            case "Key1":
                curNote = Define.NoteControl.Key1;
                StartKeySet();
                break;
            case "Key2":
                curNote = Define.NoteControl.Key2;
                StartKeySet();
                break;
            case "Key3":
                curNote = Define.NoteControl.Key3;
                StartKeySet();
                break;
            case "Key4":
                curNote = Define.NoteControl.Key4;
                StartKeySet();
                break;
            case "Exit":
                GameManager.Input.RemoveUIKeyEvent(KeySetKeyEvent);
                Destroy(gameObject);
                break;
            default:
                Debug.Log("���� ����� ���ų� �߸� �Է�");
                break;
        }
    }

    void KeySetKeyEvent(Define.UIControl keyEvent) {
        switch (keyEvent)
        {
            case Define.UIControl.Up:
                ControlMenu("Up");
                break;
            case Define.UIControl.Down:
                ControlMenu("Down");
                break;
            case Define.UIControl.Enter:
                if (curMenuIndex == 0) ButtonEvent("Key1");
                if (curMenuIndex == 1) ButtonEvent("Key2");
                if (curMenuIndex == 2) ButtonEvent("Key3");
                if (curMenuIndex == 3) ButtonEvent("Key4");
                break;
            //Ű ���� �� �Է� ��
            case Define.UIControl.Any:
                if (isSetMode) EndKeySet();
                break;
            case Define.UIControl.Esc:
                if (isSetMode) EndKeySet();
                else ButtonEvent("Exit");
                break;
        }
    }

    public void ChangeMenu(int index = -1)
    {
        if (index != -1) {
            curMenuIndex = index;
        }
        if (curMenuIndex < 0) curMenuIndex = numOfMenuList - 1;

        if (curMenuIndex >= numOfMenuList) curMenuIndex %= numOfMenuList;

        curMenu.GetComponent<Animator>().Play("Idle");
        curMenu = menuList[curMenuIndex];
        curMenu.GetComponent<Animator>().Play("Select");
    }

    void StartKeySet() {
        isSetMode = true;
        string curKey = menuValues[curMenuIndex].text.Replace("black", "red");
        menuValues[curMenuIndex].text = curKey;
        message.SetActive(isSetMode);
        GameManager.Input.ChangeKey(curNote);
    }

    void EndKeySet() {
        isSetMode = false;
        string newKey = GameManager.Input.GetKey(curNote);
        message.SetActive(isSetMode);

        if (newKey.Equals("Escape")) {
            string curKey = menuValues[curMenuIndex].text.Replace("red", "black");
            menuValues[curMenuIndex].text = curKey;
            return;
        }

        menuValues[curMenuIndex].text = $"<color=black>{newKey}</color>";
        Debug.Log($"Set New key: {newKey}");
    }
}
