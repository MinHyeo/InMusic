using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.UIElements;

public class Setting_UI : MonoBehaviour
{
    [Header("Setting List")]
    [Tooltip("Start에서 자동으로 할당 됨")]
    [SerializeField] private GameObject[] menuList;
    [SerializeField] private int numOfMenuList;
    [Header("Currentyl Selected Menue")]
    [SerializeField] private GameObject curMenu;
    [SerializeField] int curMenuIndex = 0;
    [Tooltip("이거 GameManaer에서 받아올 예정")]
    [SerializeField] public InputManager itemp = new InputManager();

    void Start()
    {
        //설정 항목 가져오기
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
                Debug.Log("박자 조절 미구현");
                break;
            case "KeySet":
                //TODO
                Debug.Log("키 설정 미구현");
                break;
            case "Exit":
                Destroy(gameObject);
                break;
            default:
                Debug.Log("아직 기능이 없거나 잘못 입력");
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
