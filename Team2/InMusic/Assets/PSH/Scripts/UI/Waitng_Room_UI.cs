using Fusion;
using System.Collections;
using UI_BASE_PSH;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Waiting_Room_UI : UI_Base_PSH
{
    [SerializeField] private GameObject popupUI;

    [Header("���� ������ ���� �׸� Ȯ��")]
    [SerializeField] private GameObject curMusicItem;
    [SerializeField] private bool HasBMS;

    [Header("������ ����Ʈ ���� Ȯ��")]
    [SerializeField] private GameObject musicList;
    [SerializeField] MusicList mList;

    [Header("�� ����")]
    [SerializeField] Text roomName;

    [Header("�÷��̾� ���� ���� (���� ������Ʈ)")]
    [SerializeField] PlayerStatusUIController playerStatusController;
    [SerializeField] bool isOwner; //���� ����
    [SerializeField] bool canStart;
    [SerializeField] NetworkObject localPlayerObject;
    [SerializeField] NetworkObject otherPlayerObject;

    [Header("���۹�ư")]
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
        roomName.text = NetworkManager.runnerInstance.SessionInfo.Name; //�� �̻���
    }

    //p2 �ʱ�ȭ
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
        Debug.Log($"�÷��̾� ����: {playerRef.PlayerId}");

        if (playerRef.PlayerId != 2)
        {
            //���� ���� ����
            NetworkManager.runnerInstance.Shutdown();
            SceneManager.LoadScene(3);
            return;
        }
        otherPlayerObject = null;
        playerStatusController.InitP2Status();

        GameManager_PSH.Data.OtherName = "Who??";

        //���� ���� ��������
        localPlayerObject.GetComponent<PlayerInfo>().Rpc_SetOwner(true);
        playerStatusController.SetRoomOwner(true);
        canStart = false;
    }

    void Update()
    {
        if (mList.IsScrolling ||  localPlayerObject  == null || !localPlayerObject.GetComponent<PlayerInfo>().IsOwner || 
            (isOwner && localPlayerObject.GetComponent<PlayerInfo>().IsReady)) //������ �غ�� ���¸� Ű �Է� ����
        {
            return;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        //��� ���� ����
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
        //P1ĭ �ʱ�ȭ
        ChangeRoomOwner(true);
        playerStatusController.SetPlayerMark(true);
        playerStatusController.SetPlayerName(0, localPlayerObject.GetComponent<PlayerInfo>().PlayerName.ToString());
        playerStatusController.SetPlayerStatus(0, false, true);
        playerStatusController.SetP1Object(localPlayerObject.gameObject);
        //P2ĭ �ʱ�ȭ
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
                //P2ĭ �ʱ�ȭ
                ChangeRoomOwner(true);
                playerStatusController.SetPlayerMark(false);
                playerStatusController.SetPlayerStatus(1, false, false);
                playerStatusController.SetPlayerName(1, GameManager_PSH.Data.MyName);
                playerStatusController.SetP2Object(localPlayerObject.gameObject);
            }
            else
            {
                otherPlayerObject = pObject;
                //P1ĭ �ʱ�ȭ
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
                //Ű �Է� �̺�Ʈ ����
                GameManager_PSH.Input.RemoveUIKeyEvent(SingleLobbyKeyEvent);
                NetworkManager.runnerInstance.Shutdown();
                SceneManager.LoadScene(3);
                break;
            case "Enter":
                OnReadyButton();
                if (!HasBMS)
                {
                    Debug.Log("BMS ���� ����");
                    return;
                }
                if (!canStart || !isOwner)
                    return;
                Debug.Log("���� ����");
                localPlayerObject.GetComponent<PlayerUIController>().BroadGameStart();
                break;
            case "KeyGuide":
                Guide();
                break;
            default:
                Debug.Log("���� ����� ���ų� �߸� �Է�");
                break;
        }
    }
    void OnReadyButton()
    {
        //�� �̵� �� �غ� ���� ���� ����
        if (canStart && isOwner) {
            return;
        }

        NetworkObject localPlayerObject = NetworkManager.runnerInstance.GetPlayerObject(NetworkManager.runnerInstance.LocalPlayer);
        if (localPlayerObject != null)
        {
            //localPlayer ã�Ƽ� ���� ����
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
            //���� ����
            Debug.Log("BMS �Ҵ� �Ϸ�");
            HasBMS = true;
        }
        else
        {
            //popupUI = GameManager_PSH.Resource.Instantiate("Notice_UI");
            Debug.LogWarning("BMS ������ ���� ��");
            HasBMS = false;
        }
    }

    public void UpdateAllPlayerStatus()
    {
        PlayerInfo me = localPlayerObject.GetComponent<PlayerInfo>();
        PlayerInfo you = otherPlayerObject.GetComponent<PlayerInfo>();
        Debug.Log($"�� ����: �غ� {me.IsReady} ���� {me.IsOwner}");
        Debug.Log($"�� ����: �غ� {you.IsReady} ����{you.IsOwner}");

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
            Debug.Log("���� ����");
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

        //������ ����Ƽ ���� ���ҽ��� ����ȭ
        GameManager_PSH.Resource.CheckMusic();
        //���� ��� �����ͼ� ������ ��Ͽ� �Ѱ��ֱ�
        mList.SetData(GameManager_PSH.Resource.GetMusicList());
    }

    //��Ʈ��ũ ������Ʈ �Ҵ�
    IEnumerator GetLocalPlayerObject()
    {
        while (NetworkManager.runnerInstance == null || !NetworkManager.runnerInstance.IsRunning)
        {
            yield return null;
        }

        //���� �÷��̾� NetworkObject�� ������ ������ ��ٸ�
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