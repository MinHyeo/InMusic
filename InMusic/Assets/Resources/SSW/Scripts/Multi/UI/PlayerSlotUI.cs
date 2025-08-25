using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerSlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Text _nicknameText;
    [SerializeField] private GameObject _readyIcon;
    [SerializeField] private GameObject _hostIcon;  // 호스트 아이콘
    [SerializeField] private GameObject _meIcon;    // 본인 표시 아이콘 (ME)
    [SerializeField] private ReadyStartController _readyStartController;  // 레디/스타트 관리자
    [SerializeField] private GameObject _contextMenuPanel;  // 우클릭 시 뜰 팝업 패널
    [SerializeField] private Image _slotImage;  // 실제 클릭을 감지할 슬롯 이미지
    private PlayerStateController _boundPlayer;
    
    private string _lastNickname;
    private bool _lastReady;
    private bool _lastHost;
    private bool _lastIsMe;

    public bool IsAvailable => _boundPlayer == null;
    public PlayerStateController BoundPlayer => _boundPlayer;

    private void Start()
    {
        // 슬롯 이미지가 설정되어 있으면 Raycast Target 확인
        if (_slotImage != null)
        {
            _slotImage.raycastTarget = true;
        }
    }

    public void Bind(PlayerStateController player)
    {
        _boundPlayer = player;
        gameObject.SetActive(true);
        
        // ReadyStartController 초기화
        if (_readyStartController != null)
        {
            _readyStartController.Initialize(player);
        }
        
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
    /// 우클릭 감지 - Unity EventSystem 사용
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // 우클릭 감지
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 실제로 슬롯 이미지가 클릭되었는지 확인
            if (_slotImage != null && eventData.pointerCurrentRaycast.gameObject == _slotImage.gameObject)
            {
                OnRightClick();
            }
        }
        // 좌클릭으로 팝업 패널 닫기
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (_contextMenuPanel != null && _contextMenuPanel.activeSelf)
            {
                // 패널 밖을 클릭했으면 패널 닫기
                if (!IsMouseOverPanel())
                {
                    _contextMenuPanel.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// 우클릭 처리 로직
    /// </summary>
    private void OnRightClick()
    {
        Debug.Log($"[PlayerSlotUI] Right click detected on slot: {_boundPlayer?.Nickname ?? "NULL"}");
        
        // 바인딩된 플레이어가 없으면 무시
        if (_boundPlayer == null) 
        {
            Debug.Log("[PlayerSlotUI] No bound player - ignoring right click");
            return;
        }

        // 마스터 클라이언트만 가능
        if (!SharedModeMasterClientTracker.IsLocalPlayerSharedModeMasterClient()) return;
        
        // 자기 자신 제외
        if (_boundPlayer.Object.HasInputAuthority) return;

        // 팝업 패널 활성화
        if (_contextMenuPanel != null)
        {
            _contextMenuPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 마우스가 팝업 패널 위에 있는지 확인
    /// </summary>
    private bool IsMouseOverPanel()
    {
        if (_contextMenuPanel == null || !_contextMenuPanel.activeSelf)
            return false;

        // 팝업 패널의 RectTransform 가져오기
        RectTransform panelRect = _contextMenuPanel.GetComponent<RectTransform>();
        if (panelRect == null) return false;

        // 마우스 위치가 패널 영역 안에 있는지 확인
        Vector2 mousePosition = Input.mousePosition;
        return RectTransformUtility.RectangleContainsScreenPoint(panelRect, mousePosition, Camera.main);
    }
}