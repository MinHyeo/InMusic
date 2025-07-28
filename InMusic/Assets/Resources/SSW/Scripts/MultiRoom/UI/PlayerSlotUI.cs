using UnityEngine;
using UnityEngine.UI;

public class PlayerSlotUI : MonoBehaviour
{
    [SerializeField] private Text _nicknameText;
    [SerializeField] private GameObject _readyIcon;
    [SerializeField] private GameObject _hostIcon;  // 호스트 아이콘
    [SerializeField] private GameObject _meIcon;    // 본인 표시 아이콘 (ME)
    [SerializeField] private ReadyStartController _readyStartController;  // 레디/스타트 관리자
    [SerializeField] private Button _transferHostButton;  // 방장 이전 버튼 (옵션)

    private PlayerStateController _boundPlayer;
    
    private string _lastNickname;
    private bool _lastReady;
    private bool _lastHost;
    private bool _lastIsMe;

    public bool IsAvailable => _boundPlayer == null;
    public PlayerStateController BoundPlayer => _boundPlayer;

    public void Bind(PlayerStateController player)
    {
        _boundPlayer = player;
        gameObject.SetActive(true);
        
        // ReadyStartController 초기화
        if (_readyStartController != null)
        {
            _readyStartController.Initialize(player);
        }
        
        // 방장 이전 버튼 설정
        SetupTransferHostButton();
        
        ChangeDisplay();
    }

    public void Unbind()
    {
        // ReadyStartController 정리
        if (_readyStartController != null)
        {
            _readyStartController.Cleanup();
        }
        
        _boundPlayer = null;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 플레이어 정보 변경 시 호출
    /// 플레이어의 닉네임, 레디 상태, 호스트 여부 등을 업데이트
    /// </summary>
    public void ChangeDisplay()
    {
        if (_boundPlayer == null) return;

        // SharedModeMasterClient 확인 (Fusion의 네트워크 레벨 권한)
        bool isSharedModeMasterClient = SharedModeMasterClientTracker.IsPlayerSharedModeMasterClient(_boundPlayer.Object.InputAuthority);
        
        // 본인 여부 확인
        bool isMe = _boundPlayer.Object.HasInputAuthority;

        // 상태 변경 감지를 위한 디버그 로그
        if (_lastNickname != _boundPlayer.Nickname || 
            _lastReady != _boundPlayer.IsReady || 
            _lastHost != isSharedModeMasterClient || 
            _lastIsMe != isMe)
        {
            Debug.Log($"[PlayerSlotUI] State change detected for {_boundPlayer.Nickname} - " +
                     $"Ready: {_lastReady}→{_boundPlayer.IsReady}, " +
                     $"Host: {_lastHost}→{isSharedModeMasterClient}");
        }

        // 변경사항이 있을 때만 업데이트
        if (_lastNickname == _boundPlayer.Nickname &&
            _lastReady == _boundPlayer.IsReady &&
            _lastHost == isSharedModeMasterClient &&
            _lastIsMe == isMe) return;

        // 플레이어 정보 표시
        _nicknameText.text = _boundPlayer.Nickname;
        _readyIcon.SetActive(_boundPlayer.IsReady);

        // 호스트 아이콘 표시 (SharedModeMasterClient인 경우만)
        if (_hostIcon != null)
        {
            _hostIcon.SetActive(isSharedModeMasterClient);
        }

        // ME 아이콘 표시 (본인인 경우에만)
        if (_meIcon != null)
        {
            _meIcon.SetActive(isMe);
        }

        // 버튼 상태 업데이트
        if (_readyStartController != null)
        {
            _readyStartController.UpdateButtonStates();
        }

        // 상태 저장
        _lastNickname = _boundPlayer.Nickname;
        _lastReady = _boundPlayer.IsReady;
        _lastHost = isSharedModeMasterClient;
        _lastIsMe = isMe;

        // 로그에 상세 정보 표시
        string hostStatus = isSharedModeMasterClient ? "SHARED MASTER CLIENT" : "CLIENT";
        string meStatus = isMe ? " (ME)" : "";
        Debug.Log($"[PlayerSlotUI] Updated: {_boundPlayer.Nickname}{meStatus} (Ready: {_boundPlayer.IsReady}, Role: {hostStatus})");
    }
    
    /// <summary>
    /// 방장 이전 버튼 설정 (다른 곳에서 통합 관리 예정)
    /// </summary>
    private void SetupTransferHostButton()
    {
        if (_transferHostButton == null) return;
        
        _transferHostButton.onClick.RemoveAllListeners();
        
        // 방장 이전/킥 기능은 별도 UI에서 통합 관리
        _transferHostButton.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 로컬 플레이어 찾기
    /// </summary>
    private PlayerStateController GetLocalPlayer()
    {
        var allPlayers = MultiRoomManager.Instance?.GetAllPlayers();
        if (allPlayers != null)
        {
            foreach (var player in allPlayers)
            {
                if (player.Object.HasInputAuthority)
                {
                    return player;
                }
            }
        }
        return null;
    }
}