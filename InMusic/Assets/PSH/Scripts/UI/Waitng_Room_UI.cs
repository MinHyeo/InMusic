using Fusion;
using System.Collections;
using System.Collections.Generic;
using UI_BASE_PSH;
using Unity.Services.Authentication;
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
    [SerializeField] string curPlayer;
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

        curPlayer = GameManager_PSH.Data.GetPlayerID();
    }

    void Start()
    {
        StartCoroutine(SetLogData());
        LoadingScreen.Instance.SceneReady();
        roomName.text = NetworkManager.runnerInstance.SessionInfo.Name; //�� �̻���

        InitPlayerStatus();
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
        UpdatePlayerStatusUI(playerRef, playerInfo); // UI ������Ʈ
    }

    void PlayerLeft(PlayerRef playerRef) {
        Debug.Log($"�÷��̾� ����: {playerRef.PlayerId}");
        if (_playerInfos.ContainsKey(playerRef))
        {
            _playerInfos.Remove(playerRef); // ��ųʸ����� ����
        }
    }

    void UpdatePlayerStatusUI(PlayerRef target, PlayerInfo info) {
        // ���� �÷��̾� (�ڽ�)
        if (target == NetworkManager.runnerInstance.LocalPlayer)
        {
            playerStatusController.SetP1Name(info.PlayerName.ToString());
            playerStatusController.SetRoomOwner(NetworkManager.runnerInstance.IsServer);
            playerStatusController.Setp1Status(false); // �غ� ���� ����
            Debug.Log($"���� �÷��̾� UI ������Ʈ: {info.PlayerName.ToString()}");
        }
        else // ���� �÷��̾�
        {
            playerStatusController.SetP2Name(info.PlayerName.ToString());
            playerStatusController.Setp2Status(false);
            Debug.Log($"���� �÷��̾� UI ������Ʈ: {info.PlayerName.ToString()}");
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
    }

    public void InitPlayerStatus() {
        if (GameManager_PSH.PlayerRole)
        {
            playerStatusController.SetP1Name("Player1");
            playerStatusController.InitP2Status();
            ChangeRoomOwner(true);
        }
        else
        {
            playerStatusController.SetP2Name("Player2");
        }
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
