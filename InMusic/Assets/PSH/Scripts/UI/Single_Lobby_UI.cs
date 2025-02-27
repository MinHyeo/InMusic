using UnityEngine;
using UnityEngine.UI;
using UI_BASE_PSH;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class Single_Lobby_UI : UI_Base
{
    [SerializeField] private GameObject popupUI;
    [Header("���� ������ ���� �׸�")]
    [SerializeField] private GameObject curMusicItem;

    [Header("������ ������ ����")]
    [SerializeField] private GameObject musicInfo;
    [SerializeField] MusicInfo mInfo;

    [Header("������ ����Ʈ ����")]
    [SerializeField] private GameObject musicList;
    [SerializeField] MusicList mList;


    private void Awake()
    {
        if (musicInfo == null || mInfo == null)
            musicInfo = transform.Find("MusicInfo").gameObject;
        mInfo = musicInfo.GetComponent<MusicInfo>();


        if (musicList == null || mList == null)
            musicList = transform.Find("MusicList").gameObject;
        mList = musicList.GetComponent<MusicList>();

    }

    void Start()
    {

        //���� ��� Load�ؼ� �Ѱ��ֱ�
        mList.SetData(GameManager_PSH.Resource.GetMusicList());
        
        //���� ����
        GameManager_PSH.Input.SetUIKeyEvent(SingleLobbyKeyEvent);
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
                //SceneManager.LoadScene("�κ� ��");
                break;
            case "Enter":
                if (curMusicItem.GetComponent<MusicItem>().HasBMS) {
                    //Ű �Է� �̺�Ʈ ����
                    GameManager_PSH.Input.RemoveUIKeyEvent(SingleLobbyKeyEvent);

                    //���� ���� �Ѱ��� MusicData �� ����
                    GameManager_PSH.Data.SetData(curMusicItem.GetComponent<MusicItem>());

                    SceneManager.LoadScene(1);
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

    void SingleLobbyKeyEvent(Define.UIControl keyEvent)
    {
        if (popupUI != null || SettingUI != null || guideUI != null || mList.IsScrolling) return;

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
}
