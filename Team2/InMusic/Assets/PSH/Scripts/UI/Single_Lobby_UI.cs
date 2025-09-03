using UnityEngine;
using UI_BASE_PSH;
using UnityEngine.SceneManagement;
using System.Collections;

public class Single_Lobby_UI : UI_Base_PSH
{
    [SerializeField] private GameObject popupUI;

    [Header("���� ������ ���� �׸� Ȯ��")]
    [SerializeField] private GameObject curMusicItem;

    [Header("������ ������ ���� Ȯ��")]
    [SerializeField] private GameObject musicInfo;
    [SerializeField] MusicInfo mInfo;

    [Header("������ ����Ʈ ���� Ȯ��")]
    [SerializeField] private GameObject musicList;
    [SerializeField] MusicList mList;


    private void Awake()
    {
        GameManager_PSH.Web.GetMusicLogs();

        if (musicInfo == null || mInfo == null)
            musicInfo = transform.Find("MusicInfo").gameObject;
        mInfo = musicInfo.GetComponent<MusicInfo>();


        if (musicList == null || mList == null)
            musicList = transform.Find("MusicList").gameObject;
        mList = musicList.GetComponent<MusicList>();
    }


    void Start()
    {
        StartCoroutine(SetLogData());
        LoadingScreen.Instance.SceneReady();
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
        mInfo.UpdateInfo(curMusicItem.GetComponent<MusicItem>());
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
                SceneManager.LoadScene(0);
                break;
            case "Enter":
                if (curMusicItem.GetComponent<MusicItem>().HasBMS) {
                    //Ű �Է� �̺�Ʈ ����
                    GameManager_PSH.Input.RemoveUIKeyEvent(SingleLobbyKeyEvent);

                    //���� ���� �Ѱ��� MusicData �� ����
                    GameManager_PSH.Data.SetData(curMusicItem.GetComponent<MusicItem>());

                    //SceneManager.LoadScene(1);
                    LoadingScreen.Instance.LoadScene("KGB_SinglePlay", GameManager_PSH.Data.GetData());
                    //LoadingScreen.Instance.LoadScene("KGB_SinglePlay");
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

    //TestCode
    /*
    public void LogSaveTestButton()
    {
        //MusicData ����(GameManager�� ���� �ִ� ��)
        Music_Item tmp = curMusicItem.GetComponent<Music_Item>();
        GameManager_PSH.Instance.GetComponent<MusicData>().DirPath = tmp.DirPath;
        GameManager_PSH.Instance.GetComponent<MusicData>().BMS = tmp.Data.BMS;
        GameManager_PSH.Instance.GetComponent<MusicData>().Album = tmp.Album.sprite;
        GameManager_PSH.Instance.GetComponent<MusicData>().Audio = tmp.Audio;
        GameManager_PSH.Instance.GetComponent<MusicData>().MuVi = tmp.MuVi;
        GameManager_PSH.Instance.GetComponent<MusicData>().Score = tmp.Score;
        GameManager_PSH.Instance.GetComponent<MusicData>().Accuracy = tmp.Accuracy;
        GameManager_PSH.Instance.GetComponent<MusicData>().Combo = tmp.Combo;
        GameManager_PSH.Instance.GetComponent<MusicData>().Rank = tmp.Rank.text;

        //���� ���� �� �Է¹޴� ��
        MusicLog test = new MusicLog();
        test.Combo = "100";
        test.Accuracy = "10%";
        test.Score = "1000";
        test.Rank = "A";
        //����
        GameManager_PSH.Data.SaveData(test);
    }*/

    /// <summary>
    /// �α� �����ͼ� DataManager���� �Ҵ��ϴ°Ŷ�, �Ҵ��� ���� �̿��� musicData �����ϴ� �������� �񵿱� ���� �߻��ؼ�, musicData ���� ��û�� �񵿱�� ����
    /// </summary>
    /// <returns></returns>
    IEnumerator SetLogData()
    {
        if (!GameManager_PSH.Data.isLogReady)
            yield return null;
        //������ ����Ƽ ���� ���ҽ��� ����ȭ
        GameManager_PSH.Resource.CheckMusic();
        //���� ��� �����ͼ� ������ ��Ͽ� �Ѱ��ֱ�
        mList.SetData(GameManager_PSH.Resource.GetMusicList());
    }
}
