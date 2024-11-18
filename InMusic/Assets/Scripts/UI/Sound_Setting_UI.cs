using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.UI;

public class Sound_Setting_UI : MonoBehaviour
{
    [Header("Setting List")]
    [Tooltip("Start���� �ڵ����� �Ҵ� ��")]
    [SerializeField] private GameObject[] menuList;
    [SerializeField] private int numOfMenuList;
    [SerializeField] private Slider[] menuSliders;
    [SerializeField] private Text[] menuValues;
    [Header("Currentyl selected menue and UI")]
    [SerializeField] private GameObject curMenu;
    [SerializeField] int curMenuIndex = 0;
    [SerializeField] GameObject keySet = null;

    void Start()
    {
        //���� �׸� ��������
        numOfMenuList = transform.GetChild(0).childCount - 2;
        menuList = new GameObject[numOfMenuList];
        menuSliders = new Slider[numOfMenuList - 2];
        menuValues = new Text[numOfMenuList];
        for (int i = 0; i < numOfMenuList; i++) {
            menuList[i] = transform.GetChild(0).GetChild(i + 2).gameObject;
            Slider tmp = menuList[i].transform.GetChild(1).GetChild(0).GetComponent<Slider>();
            if (tmp != null) { 
                menuSliders[i] = tmp;
            }
            menuValues[i] = menuList[i].transform.GetChild(1).GetChild(2).GetComponent<Text>();
        }

        curMenu = menuList[curMenuIndex];
        ChangeMenu();

        GameManager.Input.SetUIKeyEvent(SoundSetKeyEvent);
    }

    void Update()
    {
        UpdateSliderValues();
    }

    public void ControlMenu(string arrow)
    {
        switch (arrow)
        {
            case "Left":
                if (curMenuIndex > 3) return;
                menuSliders[curMenuIndex].value -= 1.0f;
                break;
            case "Right":
                if (curMenuIndex > 3) return;                
                menuSliders[curMenuIndex].value += 1.0f;
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

    void ChangeMenu()
    {
        if (curMenuIndex < 0) curMenuIndex = numOfMenuList - 1;

        if (curMenuIndex >= numOfMenuList) curMenuIndex %= numOfMenuList;

        curMenu.GetComponent<Animator>().Play("Idle");
        curMenu = menuList[curMenuIndex];
        curMenu.GetComponent<Animator>().Play("Select");
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
                keySet = GameManager.Resource.Instantiate("KeySetting_UI");
                Debug.Log(keySet.gameObject.name);
                break;
            case "Exit":
                GameManager.Input.RemoveUIKeyEvent(SoundSetKeyEvent);
                Destroy(gameObject);
                break;
            default:
                Debug.Log("���� ����� ���ų� �߸� �Է�");
                break;
        }
    }

    public void SoundSetKeyEvent(Define.UIControl keyEvent)
    {
        //Ű ���� UI�� Ȱ��ȭ�Ǿ� �ִ� ���� Ű �Է� ����
        if (keySet != null) return;

        switch (keyEvent)
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

    void UpdateSliderValues() 
    {
        for (int i = 0; i < menuSliders.Length; i++) {
            menuSliders[i].value = Mathf.Round(menuSliders[i].value);
            menuValues[i].text = $"{menuSliders[i].value}%";
        }
    }

}
