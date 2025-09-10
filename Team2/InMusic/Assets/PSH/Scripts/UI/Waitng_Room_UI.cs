using Fusion;
using System.Collections;
using UI_BASE_PSH;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Waiting_Room_UI : UI_Base_PSH
{
    [SerializeField] private GameObject popupUI;

    [Header("현재 선택한 음악 항목 확인")]
    [SerializeField] private GameObject curMusicItem;
    [SerializeField] private bool HasBMS;

    [Header("아이템 리스트 관련 확인")]
    [SerializeField] private GameObject musicList;
    [SerializeField] MusicList mList;

    [Header("방 정보")]
    [SerializeField] Text roomName;

    [Header("플레이어 상태 정보 (로컬 오브젝트)")]
    [SerializeField] PlayerStatusUIController playerStatusController;
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

    //p2 초기화
    void PlayerEnter(PlayerRef playerRef, NetworkObject networkObject)
    {
        otherPlayerObject = networkObject;
        PlayerInfo playerInfo = networkObject.GetComponent<PlayerInfo>();
        playerStatusController.SetP2Object(networkObject.gameObject);
        playerStatusController.SetPlayerName(1, playerInfo.PlayerName.ToString());
        playerStatusController.SetPlayerStatus(1, false, false);

        GameManager_PSH.Data.OtherName = otherPlayerObject.GetComponent<PlayerInfo>().PlayerName.ToString();
    }

    void PlayerLeft(PlayerRef playerRef)
    {
        Debug.Log($"플레이어 나감: {playerRef.PlayerId}");

        if (playerRef.PlayerId != 2)
        {
            //세션 연결 끊기
            NetworkManager.runnerInstance.Shutdown();
            SceneManager.LoadScene(3);
            return;
        }
        otherPlayerObject = null;
        playerStatusController.InitP2Status();

        GameManager_PSH.Data.OtherName = "Who??";

        //방장 권한 가져오기
        localPlayerObject.GetComponent<PlayerInfo>().Rpc_SetOwner(true);
        playerStatusController.SetRoomOwner(true);
        canStart = false;
    }

    void Update()
    {
        if (mList.IsScrolling ||  localPlayerObject  == null || !localPlayerObject.GetComponent<PlayerInfo>().IsOwner || 
            (isOwner && localPlayerObject.GetComponent<PlayerInfo>().IsReady)) //방장이 준비된 상태면 키 입력 막음
        {
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
            ButtonEvent("Enter");
        }
    }

    void InitHost()
    {
        //P1칸 초기화
        ChangeRoomOwner(true);
        playerStatusController.SetPlayerMark(true);
        playerStatusController.SetPlayerName(0, localPlayerObject.GetComponent<PlayerInfo>().PlayerName.ToString());
        playerStatusController.SetPlayerStatus(0, false, true);
        playerStatusController.SetP1Object(localPlayerObject.gameObject);
        //P2칸 초기화
        playerStatusController.InitP2Status();
    }

    void InitClient()
    {
        foreach (var playerRef in NetworkManager.runnerInstance.ActivePlayers)
        {
            NetworkObject pObject = NetworkManager.runnerInstance.GetPlayerObject(playerRef);
            if (pObject.GetComponent<PlayerInfo>().PlayerRole == PlayerInfo.PlayerType.Client)
            {
                localPlayerObject = pObject;
                //P2칸 초기화
                ChangeRoomOwner(true);
                playerStatusController.SetPlayerMark(false);
                playerStatusController.SetPlayerStatus(1, false, false);
                playerStatusController.SetPlayerName(1, GameManager_PSH.Data.MyName);
                playerStatusController.SetP2Object(localPlayerObject.gameObject);
            }
            else
            {
                otherPlayerObject = pObject;
                //P1칸 초기화
                playerStatusController.SetPlayerStatus(0, false, true);
                playerStatusController.SetPlayerName(0, pObject.GetComponent<PlayerInfo>().PlayerName.ToString());
                playerStatusController.SetP1Object(otherPlayerObject.gameObject);
                GameManager_PSH.Data.OtherName = otherPlayerObject.GetComponent<PlayerInfo>().PlayerName.ToString();
            }
        }
    }

    public void ChangeRoomOwner(bool isP1)
    {
        playerStatusController.SetRoomOwner(isP1);
        if (localPlayerObject.GetComponent<PlayerInfo>().IsOwner)
        {
            InitOwnerReadyButton();
        }
        else
        {
            InitClintReadyButton();
        }
    }
    #region Detect
    void OnTriggerEnter2D(Collider2D listItem)
    {
        curMusicItem = listItem.gameObject;
        curMusicItem.GetComponent<MusicItem>().ItemSelect();
        SetBMS();
    }

    private void OnTriggerExit2D(Collider2D listItem)
    {
        listItem.gameObject.GetComponent<MusicItem>().ItemUnselect();
    }
    #endregion

    public void ButtonEvent(string type)
    {
        switch (type)
        {
            case "Up":
                mList.ScrollUp();
                break;
            case "Down":
                mList.ScrollDown();
                break;
            case "Exit":
                GameManager_PSH.PlayerRole = false;
                //키 입력 이벤트 제거
                GameManager_PSH.Input.RemoveUIKeyEvent(SingleLobbyKeyEvent);
                NetworkManager.runnerInstance.Shutdown();
                SceneManager.LoadScene(3);
                break;
            case "Enter":
                OnReadyButton();
                if (!HasBMS)
                {
                    Debug.Log("BMS 파일 없음");
                    return;
                }
                if (!canStart || !isOwner)
                    return;
                Debug.Log("게임 시작");
                localPlayerObject.GetComponent<PlayerUIController>().BroadGameStart();
                break;
            case "KeyGuide":
                Guide();
                break;
            default:
                Debug.Log("아직 기능이 없거나 잘못 입력");
                break;
        }
    }
    void OnReadyButton()
    {
        //씬 이동 전 준비 상태 해제 방지
        if (canStart && isOwner) {
            return;
        }

        NetworkObject localPlayerObject = NetworkManager.runnerInstance.GetPlayerObject(NetworkManager.runnerInstance.LocalPlayer);
        if (localPlayerObject != null)
        {
            //localPlayer 찾아서 상태 변경
            PlayerInfo localPlayerInfo = localPlayerObject.GetComponent<PlayerInfo>();
            localPlayerInfo.IsReady = !localPlayerInfo.IsReady;
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

    public void SetBMS() {
        if (curMusicItem.GetComponent<MusicItem>().HasBMS)
        {
            GameManager_PSH.Input.RemoveUIKeyEvent(SingleLobbyKeyEvent);
            GameManager_PSH.Data.SetData(curMusicItem.GetComponent<MusicItem>());
            //게임 시작
            Debug.Log("BMS 할당 완료");
            HasBMS = true;
        }
        else
        {
            //popupUI = GameManager_PSH.Resource.Instantiate("Notice_UI");
            Debug.LogWarning("BMS 파일이 없는 곡");
            HasBMS = false;
        }
    }

    public void UpdateAllPlayerStatus()
    {
        PlayerInfo me = localPlayerObject.GetComponent<PlayerInfo>();
        PlayerInfo you = otherPlayerObject.GetComponent<PlayerInfo>();
        Debug.Log($"내 상태: 준비 {me.IsReady} 방장 {me.IsOwner}");
        Debug.Log($"너 상태: 준비 {you.IsReady} 방장{you.IsOwner}");

        isOwner = me.IsOwner;

        if (me.PlayerRole == PlayerInfo.PlayerType.Host)
        {
            playerStatusController.SetPlayerStatus(0, me.IsReady, isOwner);
            playerStatusController.SetPlayerStatus(1, you.IsReady, you.IsOwner);
            playerStatusController.SetRoomOwner(isOwner);
        }
        else
        {
            playerStatusController.SetPlayerStatus(1, me.IsReady, isOwner);
            playerStatusController.SetPlayerStatus(0, you.IsReady, you.IsOwner);
            playerStatusController.SetRoomOwner(you.IsOwner);
        }

        if (you.IsReady && me.IsReady && curMusicItem.GetComponent<MusicItem>().HasBMS)
        {
            canStart = true;
            Debug.Log("시작 가능");
        }
        else
        {
            canStart = false;
        }

        if (isOwner) {
            InitOwnerReadyButton();
        }
        else
        {
            InitClintReadyButton();
        }
    }

    #region Player Controll

    public void SetOwner(bool isP1) {
        if (localPlayerObject.GetComponent<PlayerInfo>().PlayerRole == PlayerInfo.PlayerType.Host) {
            localPlayerObject.GetComponent<PlayerInfo>().Rpc_SetOwner(isP1);
            otherPlayerObject.GetComponent<PlayerInfo>().Rpc_SetOwner(!isP1);
        }
        else
        {

            localPlayerObject.GetComponent<PlayerInfo>().Rpc_SetOwner(!isP1);
            otherPlayerObject.GetComponent<PlayerInfo>().Rpc_SetOwner(isP1);
        }
    }

    public void KickOtherPlayer()
    {
        localPlayerObject.GetComponent<PlayerInfo>().Rpc_KickUser();
    }
        #endregion

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

        //로컬 플레이어 NetworkObject가 스폰될 때까지 기다림
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

    void InitOwnerReadyButton() {
        startButtonName.text = "start";
        if (canStart) {
            startButtonColor.sprite = startButtonTrue;
        }
        else
        {
            startButtonColor.sprite = startButtonFalse;
        }

    }

    void InitClintReadyButton() {
        startButtonName.text = "ready";
        startButtonColor.sprite = startButtonTrue;
    }
    #endregion
}