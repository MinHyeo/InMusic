using Fusion;
using System.Collections;
using System.Collections.Generic;
using UI_BASE_PSH;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Waiting_Room_UI : UI_Base_PSH
{
    [SerializeField] private GameObject popupUI;

    [Header("현재 선택한 음악 항목 확인")]
    [SerializeField] private GameObject curMusicItem;

    [Header("아이템 리스트 관련 확인")]
    [SerializeField] private GameObject musicList;
    [SerializeField] MusicList mList;

    [Header("방 정보")]
    [SerializeField] Text roomName;
   

    [Header("플레이어 상태 정보")]
    [SerializeField] PlayerStatusController playerStatusController;
    [Tooltip("플레이어 Prefab 관리용")]
    [SerializeField] private Dictionary<PlayerRef, PlayerInfo> _playerInfos = new Dictionary<PlayerRef, PlayerInfo>();

    void OnEnable()
    {
        PlayerInfo.OnPlayerObjectInitialized += PlayerEnter;
        NetworkManager.OnPlayerLeftEvt += PlayerLeft;
    }

    void OnDisable()
    {
        PlayerInfo.OnPlayerObjectInitialized -= PlayerEnter;
        NetworkManager.OnPlayerLeftEvt -= PlayerLeft;
    }


    private void Awake()
    {
        GameManager_PSH.Web.GetMusicLogs();
        if (musicList == null || mList == null)
            musicList = transform.Find("MusicList").gameObject;
        mList = musicList.GetComponent<MusicList>();
    }

    void Start()
    {
        StartCoroutine(SetLogData());
        LoadingScreen.Instance.SceneReady();
        roomName.text = NetworkManager.runnerInstance.SessionInfo.Name; //값 이상함
    }

    void PlayerEnter(PlayerRef playerRef, NetworkObject networkObject) {
        PlayerInfo playerInfo = networkObject.GetComponent<PlayerInfo>();
        if (!_playerInfos.ContainsKey(playerRef))
        {
            _playerInfos.Add(playerRef, playerInfo);
        }
        else
        {
            _playerInfos[playerRef] = playerInfo; // 이미 있다면 업데이트
        }
        bool isLocalPlayerObject = networkObject.HasInputAuthority;

        UpdatePlayerStatusUI(playerInfo, isLocalPlayerObject); // UI 업데이트
    }

    void PlayerLeft(PlayerRef playerRef) {
        Debug.Log($"플레이어 나감: {playerRef.PlayerId}");
        if (_playerInfos.ContainsKey(playerRef))
        {
            _playerInfos.Remove(playerRef); // 딕셔너리에서 제거
        }
        playerStatusController.InitP2Status();
    }


    /// <summary>
    /// 데이터를 입력하는 입장에서 봐야함
    /// </summary>
    /// <param name="info"></param>
    /// <param name="isLocal"></param>
    void UpdatePlayerStatusUI(PlayerInfo info, bool isLocal) {

        bool isCurrentHost = GameManager_PSH.PlayerRole;

        //방장 기준
        if (isCurrentHost)
        {
            //P1 칸에 자신 정보 입력
            if (isLocal)
            {
                playerStatusController.SetPlayerName(0, info.PlayerName.ToString());
                playerStatusController.SetRoomOwner(true);
                playerStatusController.SetPlayerStatus(0, false, true); // 준비 상태 설정
                playerStatusController.SetPlayerMark(true);
            }
            //P2 칸에 상대방 정보 입력
            else
            {
                playerStatusController.SetPlayerName(1, info.PlayerName.ToString());
                playerStatusController.SetPlayerStatus(1,false, false);
            }
        }
        //상대방 기준
        else
        {
            //P2칸에 자신 정보 입력
            if (isLocal)
            {
                playerStatusController.SetPlayerName(1, info.PlayerName.ToString());
                playerStatusController.SetPlayerStatus(1, false, false);
                playerStatusController.SetPlayerMark(false);
            }
            //P1칸에 상대방(방장) 정보 입력
            else
            {
                playerStatusController.SetPlayerName(0, info.PlayerName.ToString());
                playerStatusController.SetRoomOwner(true);
                playerStatusController.SetPlayerStatus(0, false, true);
            }
        }
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

    //방 생성시 최소 한번만 실행
    public void InitPlayerStatus() {

    }

    public void ChangeRoomOwner(bool isP1) {
        playerStatusController.SetRoomOwner(isP1);
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
                //키 입력 이벤트 제거
                GameManager_PSH.Input.RemoveUIKeyEvent(SingleLobbyKeyEvent);
                NetworkManager.runnerInstance.Shutdown();
                SceneManager.LoadScene(3); //추후 로비씬으로 바꾸기
                break;
            case "Enter":
                if (!CheckReady())
                    return;
                if (curMusicItem.GetComponent<MusicItem>().HasBMS) {
                    //키 입력 이벤트 제거
                    GameManager_PSH.Input.RemoveUIKeyEvent(SingleLobbyKeyEvent);

                    //다음 씬에 넘겨줄 MusicData 값 설정
                    GameManager_PSH.Data.SetData(curMusicItem.GetComponent<MusicItem>());

                    //LoadingScreen.Instance.LoadScene("KGB_SinglePlay", GameManager_PSH.Data.GetData());
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
    IEnumerator SetLogData()
    {
        if (!GameManager_PSH.Data.isLogReady)
            yield return null;

        //서버와 유니티 음악 리소스랑 동기화
        GameManager_PSH.Resource.CheckMusic();
        //음악 목록 가져와서 아이템 목록에 넘겨주기
        mList.SetData(GameManager_PSH.Resource.GetMusicList());
    }

    //플레이어 준비 상태 확인
    bool CheckReady() {
        if (playerStatusController.GetP1Status() || playerStatusController.GetP2Status()) {
            return true;
        }
        return false;
    }
}
