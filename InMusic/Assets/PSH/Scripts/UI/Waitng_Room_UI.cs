using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
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
    [SerializeField] bool isOwner; //방장 유무
    [SerializeField] bool canStart;
    [SerializeField] NetworkObject localPlayerObject;
    [SerializeField] NetworkObject otherPlayerObject;

    [Header("시작버튼")]
    [SerializeField] Sprite startButtonTrue;
    [SerializeField] Sprite startButtonFalse;
    [SerializeField] Image startButtonColor;
    [SerializeField] Text startButtonName;

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
        StartCoroutine(GetLocalPlayerObject());
        StartCoroutine(SetLogData());
        LoadingScreen.Instance.SceneReady();
        roomName.text = NetworkManager.runnerInstance.SessionInfo.Name; //값 이상함
    }

    void PlayerEnter(PlayerRef playerRef, NetworkObject networkObject) {
        otherPlayerObject = networkObject;
        PlayerInfo playerInfo = networkObject.GetComponent<PlayerInfo>();
        //Debug.Log($"플레이어 입장: {playerInfo.PlayerName}");
        playerStatusController.SetPlayerName(1, playerInfo.PlayerName.ToString());
        playerStatusController.SetPlayerStatus(1, false, false);

        /*
        bool isLocalPlayerObject = networkObject.HasInputAuthority;
        UpdatePlayerStatusUI(playerInfo, isLocalPlayerObject); // UI 업데이트
        */
    }

    void PlayerLeft(PlayerRef playerRef) {
        //Debug.Log($"플레이어 나감: {playerRef.PlayerId}");

        if (playerRef.PlayerId == 1){
            //세션 연결 끊기
            NetworkManager.runnerInstance.Shutdown();
            SceneManager.LoadScene(3);
            return;
        }
        otherPlayerObject = null;
        playerStatusController.InitP2Status();
    }


    /// <summary>
    /// 데이터를 입력하는 입장에서 봐야함
    /// </summary>
    /// <param name="info"></param>
    /// <param name="isLocal"></param>
    void UpdatePlayerStatusUI(PlayerInfo info, bool isLocal) {

        //방장 기준
        if (info.IsOwner)
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
        if (mList.IsScrolling || !localPlayerObject.GetComponent<PlayerInfo>().IsOwner){
            return;
        }

         float scroll = Input.GetAxis("Mouse ScrollWheel");

        //방식 변경 예정
        //UpArrow
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKey(KeyCode.UpArrow) || scroll < 0)
        {
            localPlayerObject.GetComponent<PlayerUIController>().BroadScrollUp();
        }
        //DownArrow
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKey(KeyCode.DownArrow) || scroll > 0)
        {
            localPlayerObject.GetComponent<PlayerUIController>().BroadScrollDown();
        }
        //Enter
        if (Input.GetKeyDown(KeyCode.Return))
        {
            localPlayerObject.GetComponent<PlayerUIController>().BroadGameStart();
        }
    }

    void InitHost() {
        //P1칸 초기화
        ChangeRoomOwner(true);
        playerStatusController.SetPlayerMark(true);
        playerStatusController.SetPlayerName(0, localPlayerObject.GetComponent<PlayerInfo>().PlayerName.ToString());
        playerStatusController.SetPlayerStatus(0, false, true);
        //P2칸 초기화
        playerStatusController.InitP2Status();
    }

    void InitClient() {
        foreach (var playerRef in NetworkManager.runnerInstance.ActivePlayers)
        {
            NetworkObject pObject = NetworkManager.runnerInstance.GetPlayerObject(playerRef);
            if (pObject.GetComponent<PlayerInfo>().PlayerRole == PlayerInfo.PlayerType.Client) {
                localPlayerObject = pObject;
                //P2칸 초기화
                ChangeRoomOwner(true);
                playerStatusController.SetPlayerMark(false);
                playerStatusController.SetPlayerStatus(1, false, false);
                playerStatusController.SetPlayerName(1, pObject.GetComponent<PlayerInfo>().PlayerName.ToString());
            }
            else
            {
                otherPlayerObject = pObject;
                //P1칸 초기화
                playerStatusController.SetPlayerStatus(0, false, true);
                playerStatusController.SetPlayerName(0, pObject.GetComponent<PlayerInfo>().PlayerName.ToString());
            }
        }
    }

    public void ChangeRoomOwner(bool isP1) {
        playerStatusController.SetRoomOwner(isP1);
        if (localPlayerObject.GetComponent<PlayerInfo>().IsOwner)
        {
            startButtonColor.sprite = startButtonFalse;
        }
        else
        {
            startButtonName.text = "ready";
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
                //키 입력 이벤트 제거
                GameManager_PSH.Input.RemoveUIKeyEvent(SingleLobbyKeyEvent);
                GameManager_PSH.PlayerRole = false;
                NetworkManager.runnerInstance.Shutdown();
                SceneManager.LoadScene(3); //추후 로비씬으로 바꾸기
                break;
            case "Enter":
                OnReadyButton();
                if (!canStart || !isOwner)
                    return;

                Debug.Log("게임 시작");
                if (curMusicItem.GetComponent<MusicItem>().HasBMS)
                {
                    GameManager_PSH.Input.RemoveUIKeyEvent(SingleLobbyKeyEvent);
                    GameManager_PSH.Data.SetData(curMusicItem.GetComponent<MusicItem>());
                    //게임 시작
                    localPlayerObject.GetComponent<PlayerUIController>().BroadGameStart();
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

    void OnReadyButton() {
        NetworkObject localPlayerObject = NetworkManager.runnerInstance.GetPlayerObject(NetworkManager.runnerInstance.LocalPlayer);
        if (localPlayerObject != null)
        {
            //localPlayer 찾아서 상태 변경
            PlayerInfo localPlayerInfo = localPlayerObject.GetComponent<PlayerInfo>();
            bool newReadyState = !localPlayerInfo.IsReady;
            localPlayerInfo.Rpc_SetReady(newReadyState);
        }
    }

    public void UpdateAllPlayerReady() {
        PlayerInfo me = localPlayerObject.GetComponent<PlayerInfo>();
        PlayerInfo you = otherPlayerObject.GetComponent<PlayerInfo>();
        Debug.Log($"내 상태: {me.IsReady} {me.IsOwner}");
        Debug.Log($"너 상태: {you.IsReady} {you.IsOwner}");

        if (me.PlayerRole == PlayerInfo.PlayerType.Host) {
            playerStatusController.SetPlayerStatus(0, me.IsReady, me.IsOwner);
            playerStatusController.SetPlayerStatus(1, you.IsReady, you.IsOwner);
        }
        else
        {
            playerStatusController.SetPlayerStatus(1, me.IsReady, me.IsOwner);
            playerStatusController.SetPlayerStatus(0, you.IsReady, you.IsOwner);
        }

        if (me.IsOwner && you.IsReady) {
            canStart = true;
            startButtonColor.sprite = startButtonTrue;
        }
    }

    #region Coroutine
    IEnumerator SetLogData()
    {
        if (!GameManager_PSH.Data.isLogReady)
            yield return null;

        //서버와 유니티 음악 리소스랑 동기화
        GameManager_PSH.Resource.CheckMusic();
        //음악 목록 가져와서 아이템 목록에 넘겨주기
        mList.SetData(GameManager_PSH.Resource.GetMusicList());
    }

    //네트워크 오브젝트 할당
    IEnumerator GetLocalPlayerObject()
    {
        while (NetworkManager.runnerInstance == null || !NetworkManager.runnerInstance.IsRunning)
        {
            yield return null;
        }

        // 로컬 플레이어 NetworkObject가 스폰될 때까지 기다립니다.
        while (localPlayerObject == null)
        {
            localPlayerObject = NetworkManager.runnerInstance.GetPlayerObject(NetworkManager.runnerInstance.LocalPlayer);
            if (localPlayerObject == null)
            {
                yield return null;
            }
        }
        if (localPlayerObject.GetComponent<PlayerInfo>().PlayerRole == PlayerInfo.PlayerType.Host)
        {
            isOwner = true;
            InitHost();
        }
        else
        {
            isOwner = false;
            InitClient();
        }
    }
    #endregion
}
