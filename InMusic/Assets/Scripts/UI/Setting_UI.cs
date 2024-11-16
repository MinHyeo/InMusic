using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.UIElements;

public class Setting_UI : MonoBehaviour
{
    [Header("Setting List")]
    [Tooltip("Start���� �ڵ����� �Ҵ� ��")]
    [SerializeField] private GameObject[] menuList;
    [SerializeField] private int numOfMenuList;
    [Header("Currentyl Selected Menue")]
    [SerializeField] private GameObject curMenu;
    [SerializeField] int curMenuIndex = 0;
    [Tooltip("�̰� GameManaer���� �޾ƿ� ����")]
    [SerializeField] public InputManager itemp = new InputManager();

    void Start()
    {
        //���� �׸� ��������
        numOfMenuList = transform.childCount - 1;
        menuList = new GameObject[numOfMenuList];
        for (int i = 0; i < numOfMenuList; i++) {
            menuList[i] = transform.GetChild(i + 1).gameObject;
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
            case "Left":
                break;
            case "Right":
                break;
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
            case "BeatSet":
                //TODO
                Debug.Log("���� ���� �̱���");
                break;
            case "KeySet":
                //TODO
                Debug.Log("Ű ���� �̱���");
                break;
            case "Exit":
                Destroy(gameObject);
                break;
            default:
                Debug.Log("���� ����� ���ų� �߸� �Է�");
                break;
        }
    }

    public void KeyEvent(Define.UIControl keyEven)
    {
        switch (keyEven)
        {
            case Define.UIControl.Right:
                ControlMenu("Right");
                break;
            case Define.UIControl.Left:
                ControlMenu("Left");
                break;
            case Define.UIControl.Up:
                ControlMenu("Up");
                break;
            case Define.UIControl.Down:
                ControlMenu("Down");
                break;
            case Define.UIControl.Enter:
                if (curMenuIndex == 3) {
                    ButtonEvent("BeatSet");
                }
                else if (curMenuIndex == 4) { 
                    ButtonEvent("KeySet");
                }
                break;
            case Define.UIControl.Esc:
                ButtonEvent("Exit");
                break;
        }
    }

    void ChangeMenu() {
        if (curMenuIndex < 0) { 
            curMenuIndex = numOfMenuList - 1;
        }

        if (curMenuIndex >= numOfMenuList) {
            curMenuIndex %= numOfMenuList;
        }
        curMenu.GetComponent<Animator>().Play("Idle");
        curMenu = menuList[curMenuIndex];
        curMenu.GetComponent<Animator>().Play("Select");
    }
}
