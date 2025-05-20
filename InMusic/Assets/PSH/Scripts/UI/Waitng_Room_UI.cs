using UnityEngine;
using UI_BASE_PSH;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using Fusion;

public class Waiting_Room_UI : UI_Base_PSH
{
    [SerializeField] private GameObject popupUI;

    [Header("���� ������ ���� �׸� Ȯ��")]
    [SerializeField] private GameObject curMusicItem;

    [Header("������ ����Ʈ ���� Ȯ��")]
    [SerializeField] private GameObject musicList;
    [SerializeField] MusicList mList;

    [Header("�� ����")]
    [SerializeField] SessionInfo roomInfo;
    [SerializeField] Text roomName;

    [Header("�÷��̾� ���� ����")]
    [SerializeField] PlayerStatusController playerStatusController;
    [SerializeField] PlayerData curPlayer;
    [SerializeField] bool isRoomCap = false;


    private void Awake()
    {
        GameManager_PSH.Web.GetMusicLogs();
        if (musicList == null || mList == null)
            musicList = transform.Find("MusicList").gameObject;
        mList = musicList.GetComponent<MusicList>();

        //��Ʈ��ũ �Ŵ��� ���� (ȣ��Ʈ ����, �� �̸� ��)
    }


    void Start()
    {
        StartCoroutine(SetLogData());
        LoadingScreen.Instance.SceneReady();
        //roomName.text = roomInfo.Name;
    }

    void Update()
    {
        //����� �ٷ� ���� ó��
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (!mList.IsScrolling) {
            if (scroll > 0) //���� ���� ������ ��
            {
                mList.ScrollDown();
            }

            else if (scroll < 0)  //���� �Ʒ��� ������ ��
            {
                mList.ScrollUp();
            }
        }

        //��� ���� ����
        //UpArrow
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKey(KeyCode.UpArrow))
        {
            SingleLobbyKeyEvent(Define_PSH.UIControl.Up);
        }
        //DownArrow
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            SingleLobbyKeyEvent(Define_PSH.UIControl.Down);
        }
        //Enter
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SingleLobbyKeyEvent(Define_PSH.UIControl.Enter);
        }
    }

    #region Detect
    void OnTriggerEnter2D(Collider2D listItem)
    {
        curMusicItem = listItem.gameObject;
        curMusicItem.GetComponent<MusicItem>().ItemSelect();
    }

    private void OnTriggerExit2D(Collider2D listItem)
    {
        listItem.gameObject.GetComponent<MusicItem>().ItemUnselect();
    }
    #endregion

    public void ButtonEvent(string type) {
        switch (type)
        {
            case "Up":
                mList.ScrollUp();
                break;
            case "Down":
                mList.ScrollDown();
                break;
            case "Exit":
                //Ű �Է� �̺�Ʈ ����
                GameManager_PSH.Input.RemoveUIKeyEvent(SingleLobbyKeyEvent);
                SceneManager.LoadScene(0); //���� �κ������ �ٲٱ�
                break;
            case "Enter":
                if (curMusicItem.GetComponent<MusicItem>().HasBMS) {
                    //Ű �Է� �̺�Ʈ ����
                    GameManager_PSH.Input.RemoveUIKeyEvent(SingleLobbyKeyEvent);

                    //���� ���� �Ѱ��� MusicData �� ����
                    GameManager_PSH.Data.SetData(curMusicItem.GetComponent<MusicItem>());

                    LoadingScreen.Instance.LoadScene("KGB_SinglePlay", GameManager_PSH.Data.GetData());
                }
                else
                {
                    //popupUI = GameManager_PSH.Resource.Instantiate("Notice_UI");
                    Debug.Log("BMS ������ ���� ��");
                }
                break;
            case "KeyGuide":
                Guide();
                break;
            default:
                Debug.Log("���� ����� ���ų� �߸� �Է�");
                break;
        }
    }

    void SingleLobbyKeyEvent(Define_PSH.UIControl keyEvent)
    {
        if (popupUI != null || SettingUI != null || guideUI != null || mList.IsScrolling) return;

        switch (keyEvent)
        {
            case Define_PSH.UIControl.Up:
                ButtonEvent("Up");
                break;
            case Define_PSH.UIControl.Down:
                ButtonEvent("Down");
                break;
            case Define_PSH.UIControl.Enter:
                ButtonEvent("Enter");
                break;
            case Define_PSH.UIControl.Esc:
                ButtonEvent("Exit");
                break;
            case Define_PSH.UIControl.Guide:
                Gear();
                break;
            case Define_PSH.UIControl.Setting:
                ButtonEvent("Gear");
                break;
        }
    }
    IEnumerator SetLogData()
    {
        if (!GameManager_PSH.Data.isLogReady)
            yield return null;
        //���� ��� �����ͼ� ������ ��Ͽ� �Ѱ��ֱ�
        mList.SetData(GameManager_PSH.Resource.GetMusicList());
    }
}
