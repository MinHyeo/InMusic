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

    [Header("���� ������ ���� �׸� Ȯ��")]
    [SerializeField] private GameObject curMusicItem;

    [Header("������ ����Ʈ ���� Ȯ��")]
    [SerializeField] private GameObject musicList;
    [SerializeField] MusicList mList;

    [Header("�� ����")]
    [SerializeField] Text roomName;

    [Header("�÷��̾� ���� ����")]
    [SerializeField] PlayerStatusController playerStatusController;
    [SerializeField] bool isCurrentHost; //�� ���� ���� (UI ���п�)
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
        isCurrentHost = GameManager_PSH.PlayerRole; 
        isOwner = GameManager_PSH.PlayerRole;
        ChangeRoomOwner(isOwner);
    }

    void PlayerEnter(PlayerRef playerRef, NetworkObject networkObject) {
        otherPlayerObject = networkObject;
        PlayerInfo playerInfo = networkObject.GetComponent<PlayerInfo>();
        Debug.Log($"�÷��̾� ����: {playerInfo.PlayerName}");
        bool isLocalPlayerObject = networkObject.HasInputAuthority;

        UpdatePlayerStatusUI(playerInfo, isLocalPlayerObject); // UI ������Ʈ
    }

    void PlayerLeft(PlayerRef playerRef) {
        Debug.Log($"�÷��̾� ����: {playerRef.PlayerId}");
        otherPlayerObject = null;

        playerStatusController.InitP2Status();
    }


    /// <summary>
    /// �����͸� �Է��ϴ� ���忡�� ������
    /// </summary>
    /// <param name="info"></param>
    /// <param name="isLocal"></param>
    void UpdatePlayerStatusUI(PlayerInfo info, bool isLocal) {

        //���� ����
        if (isOwner)
        {
            //P1 ĭ�� �ڽ� ���� �Է�
            if (isLocal)
            {
                playerStatusController.SetPlayerName(0, info.PlayerName.ToString());
                playerStatusController.SetRoomOwner(true);
                playerStatusController.SetPlayerStatus(0, false, true); // �غ� ���� ����
                playerStatusController.SetPlayerMark(true);
            }
            //P2 ĭ�� ���� ���� �Է�
            else
            {
                playerStatusController.SetPlayerName(1, info.PlayerName.ToString());
                playerStatusController.SetPlayerStatus(1,false, false);
            }
        }
        //���� ����
        else
        {
            //P2ĭ�� �ڽ� ���� �Է�
            if (isLocal)
            {
                playerStatusController.SetPlayerName(1, info.PlayerName.ToString());
                playerStatusController.SetPlayerStatus(1, false, false);
                playerStatusController.SetPlayerMark(false);
            }
            //P1ĭ�� ����(����) ���� �Է�
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
            localPlayerObject.GetComponent<PlayerUIController>().BroadGameStart();
        }
    }

    public void ChangeRoomOwner(bool isP1) {
        playerStatusController.SetRoomOwner(isP1);
        if (isOwner)
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
                //Ű �Է� �̺�Ʈ ����
                GameManager_PSH.Input.RemoveUIKeyEvent(SingleLobbyKeyEvent);
                NetworkManager.runnerInstance.Shutdown();
                SceneManager.LoadScene(3); //���� �κ������ �ٲٱ�
                break;
            case "Enter":
                OnReadyButton();
                break;
            case "Start":
                if (!canStart || !isOwner)
                    return;
                Debug.Log("���� ����");
                if (curMusicItem.GetComponent<MusicItem>().HasBMS)
                {
                    //Ű �Է� �̺�Ʈ ����
                    GameManager_PSH.Input.RemoveUIKeyEvent(SingleLobbyKeyEvent);

                    //���� ���� �Ѱ��� MusicData �� ����
                    GameManager_PSH.Data.SetData(curMusicItem.GetComponent<MusicItem>());

                    //LoadingScreen.Instance.LoadScene("KGB_SinglePlay", GameManager_PSH.Data.GetData());
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

    void OnReadyButton() {
        if (canStart) {
            return;
        }
        NetworkObject localPlayerObject = NetworkManager.runnerInstance.GetPlayerObject(NetworkManager.runnerInstance.LocalPlayer);
        if (localPlayerObject != null)
        {
            //localPlayer ã�Ƽ� ���� ����
            PlayerInfo localPlayerInfo = localPlayerObject.GetComponent<PlayerInfo>();
            bool newReadyState = !localPlayerInfo.IsReady;
            localPlayerInfo.Rpc_SetReady(newReadyState);
        }
    }

    public void UpdateAllPlayerReady() {
        //���� �濡 �ִ� ��� �÷��̾�(��Ʈ��ũ ������Ʈ) Ȯ��

        foreach (var playerRef in NetworkManager.runnerInstance.ActivePlayers) {
            NetworkObject pObject = NetworkManager.runnerInstance.GetPlayerObject(playerRef);
            PlayerInfo pInfo = pObject.GetComponent<PlayerInfo>();
            bool isLocalPlayer = pObject.HasInputAuthority;
            //P1���� ���� UI ����
            if (isCurrentHost) {
                if (isLocalPlayer) {
                    playerStatusController.SetPlayerStatus(0, pInfo.IsReady, isOwner);
                }
                else
                {
                    playerStatusController.SetPlayerStatus(1, pInfo.IsReady, !isOwner);
                }
            }
            //P2���� ���� UI ����
            else
            {
                if (isLocalPlayer)
                {
                    playerStatusController.SetPlayerStatus(1, pInfo.IsReady, isOwner);
                }
                else
                {
                    playerStatusController.SetPlayerStatus(0, pInfo.IsReady, !isOwner);
                }
            }
        }
        if (isOwner) {
            if (playerStatusController.GetP1Status() && playerStatusController.GetP2Status()) {
                canStart = true;
                startButtonColor.sprite = startButtonTrue;
            }
            else
            {
                canStart = false;
                startButtonColor.sprite = startButtonFalse;
            }
        }
    }

    IEnumerator SetLogData()
    {
        if (!GameManager_PSH.Data.isLogReady)
            yield return null;

        //������ ����Ƽ ���� ���ҽ��� ����ȭ
        GameManager_PSH.Resource.CheckMusic();
        //���� ��� �����ͼ� ������ ��Ͽ� �Ѱ��ֱ�
        mList.SetData(GameManager_PSH.Resource.GetMusicList());
    }

    IEnumerator GetLocalPlayerObject()
    {
        while (NetworkManager.runnerInstance == null || !NetworkManager.runnerInstance.IsRunning)
        {
            yield return null;
        }

        // ���� �÷��̾� NetworkObject�� ������ ������ ��ٸ��ϴ�.
        while (localPlayerObject == null)
        {
            localPlayerObject = NetworkManager.runnerInstance.GetPlayerObject(NetworkManager.runnerInstance.LocalPlayer);
            if (localPlayerObject == null)
            {
                yield return null;
            }
        }
    }   
}
