using UnityEngine;
using UI_BASE_PSH;
using UnityEngine.SceneManagement;
using System.Collections;

public class Single_Lobby_UI : UI_Base_PSH
{
    [SerializeField] private GameObject popupUI;

    [Header("현재 선택한 음악 항목 확인")]
    [SerializeField] private GameObject curMusicItem;

    [Header("선택한 음악의 정보 확인")]
    [SerializeField] private GameObject musicInfo;
    [SerializeField] MusicInfo mInfo;

    [Header("아이템 리스트 관련 확인")]
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
        //목록을 휠로 조작 처리
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (!mList.IsScrolling) {
            if (scroll > 0) //휠을 위로 돌렸을 때
            {
                mList.ScrollDown();
            }

            else if (scroll < 0)  //휠을 아래로 돌렸을 때
            {
                mList.ScrollUp();
            }
        }

        //방식 변경 예정
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
                //키 입력 이벤트 제거
                GameManager_PSH.Input.RemoveUIKeyEvent(SingleLobbyKeyEvent);
                SceneManager.LoadScene(0);
                break;
            case "Enter":
                if (curMusicItem.GetComponent<MusicItem>().HasBMS) {
                    //키 입력 이벤트 제거
                    GameManager_PSH.Input.RemoveUIKeyEvent(SingleLobbyKeyEvent);

                    //다음 씬에 넘겨줄 MusicData 값 설정
                    GameManager_PSH.Data.SetData(curMusicItem.GetComponent<MusicItem>());

                    //SceneManager.LoadScene(1);
                    LoadingScreen.Instance.LoadScene("KGB_SinglePlay", GameManager_PSH.Data.GetData());
                    //LoadingScreen.Instance.LoadScene("KGB_SinglePlay");
                }
                else
                {
                    //popupUI = GameManager_PSH.Resource.Instantiate("Notice_UI");
                    Debug.Log("BMS 파일이 없는 곡");
                }
                break;
            case "KeyGuide":
                Guide();
                break;
            default:
                Debug.Log("아직 기능이 없거나 잘못 입력");
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
        //MusicData 설정(GameManager가 갖고 있는 값)
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

        //게임 종료 후 입력받는 값
        MusicLog test = new MusicLog();
        test.Combo = "100";
        test.Accuracy = "10%";
        test.Score = "1000";
        test.Rank = "A";
        //저장
        GameManager_PSH.Data.SaveData(test);
    }*/

    /// <summary>
    /// 로그 가져와서 DataManager에게 할당하는거랑, 할당한 값을 이용해 musicData 가공하는 과정에서 비동기 문제 발생해서, musicData 가공 요청을 비동기로 실행
    /// </summary>
    /// <returns></returns>
    IEnumerator SetLogData()
    {
        if (!GameManager_PSH.Data.isLogReady)
            yield return null;
        //서버와 유니티 음악 리소스랑 동기화
        GameManager_PSH.Resource.CheckMusic();
        //음악 목록 가져와서 아이템 목록에 넘겨주기
        mList.SetData(GameManager_PSH.Resource.GetMusicList());
    }
}
