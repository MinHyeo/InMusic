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

    [Tooltip("�÷��̾� Prefab ������")]
    [SerializeField] private Dictionary<PlayerRef, PlayerInfo> _playerInfos = new Dictionary<PlayerRef, PlayerInfo>();

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
        StartCoroutine(SetLogData());
        LoadingScreen.Instance.SceneReady();
        roomName.text = NetworkManager.runnerInstance.SessionInfo.Name; //�� �̻���
        isCurrentHost = GameManager_PSH.PlayerRole; 
        isOwner = GameManager_PSH.PlayerRole;
        ChangeRoomOwner(isOwner);
    }

    void PlayerEnter(PlayerRef playerRef, NetworkObject networkObject) {
        PlayerInfo playerInfo = networkObject.GetComponent<PlayerInfo>();
        if (!_playerInfos.ContainsKey(playerRef))
        {
            _playerInfos.Add(playerRef, playerInfo);
        }
        else
        {
            _playerInfos[playerRef] = playerInfo; // �̹� �ִٸ� ������Ʈ
        }
        bool isLocalPlayerObject = networkObject.HasInputAuthority;

        UpdatePlayerStatusUI(playerInfo, isLocalPlayerObject); // UI ������Ʈ
    }

    void PlayerLeft(PlayerRef playerRef) {
        Debug.Log($"�÷��̾� ����: {playerRef.PlayerId}");
        if (_playerInfos.ContainsKey(playerRef))
        {
            _playerInfos.Remove(playerRef); // ��ųʸ����� ����
        }
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

        if (isOwner) {
            //���濡�� ��ȣ �ֱ�
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
                readyStatus();
                if (!CheckReady() || !isOwner)
                    return;
                if (curMusicItem.GetComponent<MusicItem>().HasBMS) {
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
    IEnumerator SetLogData()
    {
        if (!GameManager_PSH.Data.isLogReady)
            yield return null;

        //������ ����Ƽ ���� ���ҽ��� ����ȭ
        GameManager_PSH.Resource.CheckMusic();
        //���� ��� �����ͼ� ������ ��Ͽ� �Ѱ��ֱ�
        mList.SetData(GameManager_PSH.Resource.GetMusicList());
    }

    //�÷��̾� �غ� ���� Ȯ��
    bool CheckReady() {
        if (playerStatusController.GetP1Status() && playerStatusController.GetP2Status()) {
            return true;
        }
        Debug.Log("���� �غ����� �÷��̾� ����");
        return false;
    }

    void readyStatus() {
        NetworkObject localPlayerObject = NetworkManager.runnerInstance.GetPlayerObject(NetworkManager.runnerInstance.LocalPlayer);
        if (localPlayerObject != null)
        {
            //localPlayer ã�Ƽ� ���� ����
            PlayerInfo localPlayerInfo = localPlayerObject.GetComponent<PlayerInfo>();
            bool newReadyState = !localPlayerInfo.IsReady;
            localPlayerInfo.Rpc_SetReady(newReadyState);

            //UI ������Ʈ
            if (isCurrentHost) {
                //P1 ���� ������Ʈ
                playerStatusController.SetPlayerStatus(0, !playerStatusController.GetP1Status(), false);
            }
            //P2 ���� ������Ʈ
            playerStatusController.SetPlayerStatus(1, !playerStatusController.GetP2Status(), false);
        }
    }
}
