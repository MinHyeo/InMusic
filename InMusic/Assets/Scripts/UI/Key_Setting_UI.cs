using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class Key_Setting_UI : MonoBehaviour
{
    [Header("Setting List")]
    [Tooltip("Start에서 자동으로 할당 됨")]
    [SerializeField] private GameObject[] menuList;
    [SerializeField] private Text[] menuValues;
    [SerializeField] private Text message;
    [SerializeField] private int numOfMenuList;
    [Header("Currentyl selected menue")]
    [SerializeField] private GameObject curMenu;
    [SerializeField] int curMenuIndex = 0;

    [Tooltip("이거 GameManaer에서 받아올 예정")]
    [SerializeField] public InputManager itemp = new InputManager();

    void Start()
    {
        numOfMenuList = transform.GetChild(0).childCount - 3;
        menuList = new GameObject[numOfMenuList];
        menuValues = new Text[numOfMenuList];
        for (int i = 0; i < numOfMenuList; i++) {
            menuList[i] = transform.GetChild(0).GetChild(i + 3).gameObject;
            menuValues[i] = menuList[i].transform.GetChild(1).transform.GetChild(1).GetComponent<Text>();
        }

        curMenu = menuList[curMenuIndex];
        ChangeMenu();

        itemp.uIKeyPress -= KeyEvent;
        itemp.uIKeyPress += KeyEvent;
    }

    void Update()
    {
        itemp.UIUpdate();
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
            case "Exit":
                Destroy(gameObject);
                break;
            default:
                Debug.Log("아직 기능이 없거나 잘못 입력");
                break;
        }
    }

    void KeyEvent(Define.UIControl keyEvent) {
        switch (keyEvent)
        {
            case Define.UIControl.Up:
                ControlMenu("Up");
                break;
            case Define.UIControl.Down:
                ControlMenu("Down");
                break;
            case Define.UIControl.Enter:
                break;
            case Define.UIControl.Esc:
                ButtonEvent("Exit");
                break;
        }

    }

    void ChangeMenu()
    {
        if (curMenuIndex < 0) curMenuIndex = numOfMenuList - 1;

        if (curMenuIndex >= numOfMenuList) curMenuIndex %= numOfMenuList;

        curMenu.GetComponent<Animator>().Play("Idle");
        curMenu = menuList[curMenuIndex];
        curMenu.GetComponent<Animator>().Play("Select");
    }

}
