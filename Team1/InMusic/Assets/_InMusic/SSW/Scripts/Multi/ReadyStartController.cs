using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SongList;

public class ReadyStartController : MonoBehaviour
{
    [Header("Ready Buttons - 일반 플레이어 전용")]
    [SerializeField] private Button _readyActivateButton;    // READY 버튼 (활성화용)
    [SerializeField] private Button _readyDeactivateButton;  // CANCEL 버튼 (비활성화용)
    
    [Header("Start Buttons - 방장 전용")]
    [SerializeField] private Button _startDisabledButton;    // START 버튼 (비활성화 상태)
    [SerializeField] private Button _startEnabledButton;     // START 버튼 (활성화 상태)
    
    private PlayerStateController _boundPlayer;

    public void Initialize(PlayerStateController player)
    {
        _boundPlayer = player;
        SetupButtons();
        UpdateButtonStates();
    }
    
    public void Cleanup()
    {
        // 모든 버튼 이벤트 해제
        if (_readyActivateButton != null)
        {
            _readyActivateButton.onClick.RemoveAllListeners();
        }
        if (_readyDeactivateButton != null)
        {
            _readyDeactivateButton.onClick.RemoveAllListeners();
        }
        if (_startDisabledButton != null)
        {
            _startDisabledButton.onClick.RemoveAllListeners();
        }
        if (_startEnabledButton != null)
        {
            _startEnabledButton.onClick.RemoveAllListeners();
        }
        
        _boundPlayer = null;
    }
    
    /// <summary>
    /// 로컬 플레이어(HasInputAuthority가 true인 플레이어) 가져오기
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
    
    public void UpdateButtonStates()
    {
        if (_boundPlayer == null) return;

        if (IsSharedModeMasterClient())
        {
            // 방장: 스타트 버튼만 표시
            ShowStartButton();
            HideReadyButtons();
        }
        else
        {
            // 일반 플레이어: 레디 버튼만 표시
            ShowReadyButtons();
            HideStartButton();
        }
    }
    
    private void SetupButtons()
    {
        // 레디 활성화 버튼 설정
        if (_readyActivateButton != null)
        {
            _readyActivateButton.onClick.RemoveAllListeners();
            _readyActivateButton.onClick.AddListener(OnReadyActivateClicked);
        }
        
        // 레디 비활성화 버튼 설정
        if (_readyDeactivateButton != null)
        {
            _readyDeactivateButton.onClick.RemoveAllListeners();
            _readyDeactivateButton.onClick.AddListener(OnReadyDeactivateClicked);
        }
        
        // 스타트 버튼들 설정 (활성화 버튼만 클릭 가능)
        if (_startEnabledButton != null)
        {
            _startEnabledButton.onClick.RemoveAllListeners();
            _startEnabledButton.onClick.AddListener(OnStartButtonClicked);
        }
        
        // 비활성화 스타트 버튼은 클릭 불가능하므로 이벤트 연결 안함
        if (_startDisabledButton != null)
        {
            _startDisabledButton.onClick.RemoveAllListeners();
            // 클릭 이벤트 연결 안함 (비활성화 상태)
        }
    }
    
    private bool IsSharedModeMasterClient()
    {
        // SharedModeMasterClient 확인으로 변경
        return SharedModeMasterClientTracker.IsLocalPlayerSharedModeMasterClient();
    }
    
    private void ShowStartButton()
    {
        bool allPlayersReady = CheckAllPlayersReady();
        
        if (allPlayersReady)
        {
            // 모든 플레이어가 준비되었을 때: 활성화 버튼 표시, 비활성화 버튼 숨김
            if (_startEnabledButton != null)
            {
                _startEnabledButton.gameObject.SetActive(true);
            }
            if (_startDisabledButton != null)
            {
                _startDisabledButton.gameObject.SetActive(false);
            }
            
            Debug.Log("All players ready - showing enabled start button");
        }
        else
        {
            // 일부 플레이어가 준비되지 않았을 때: 비활성화 버튼 표시, 활성화 버튼 숨김
            if (_startDisabledButton != null)
            {
                _startDisabledButton.gameObject.SetActive(true);
            }
            if (_startEnabledButton != null)
            {
                _startEnabledButton.gameObject.SetActive(false);
            }
            
            Debug.Log("Not all players ready - showing disabled start button");
        }
    }
    
    private void HideStartButton()
    {
        // 두 스타트 버튼 모두 숨김
        if (_startEnabledButton != null)
        {
            _startEnabledButton.gameObject.SetActive(false);
        }
        if (_startDisabledButton != null)
        {
            _startDisabledButton.gameObject.SetActive(false);
        }
    }
    
    private void ShowReadyButtons()
    {
        // 로컬 플레이어 기준으로 버튼 상태 결정
        var localPlayer = GetLocalPlayer();
        if (localPlayer == null) return;
        
        if (localPlayer.IsReady)
        {
            // 레디 상태: CANCEL 버튼만 활성화, READY 버튼은 숨김
            if (_readyActivateButton != null) 
            {
                _readyActivateButton.gameObject.SetActive(false);
            }
            if (_readyDeactivateButton != null) 
            {
                _readyDeactivateButton.gameObject.SetActive(true);
                _readyDeactivateButton.interactable = true; // 클릭 가능 + 애니메이션 가능
            }
        }
        else
        {
            // 레디 안한 상태: READY 버튼만 활성화, CANCEL 버튼은 숨김
            if (_readyActivateButton != null) 
            {
                _readyActivateButton.gameObject.SetActive(true);
                _readyActivateButton.interactable = true; // 클릭 가능 + 애니메이션 가능
            }
            if (_readyDeactivateButton != null) 
            {
                _readyDeactivateButton.gameObject.SetActive(false);
            }
        }
    }
    
    private void HideReadyButtons()
    {
        if (_readyActivateButton != null) _readyActivateButton.gameObject.SetActive(false);
        if (_readyDeactivateButton != null) _readyDeactivateButton.gameObject.SetActive(false);
    }
    
    private bool CheckAllPlayersReady()
    {
        int nonHostCount = 0;
        int readyCount = 0;
        
        // MultiRoomManager를 통해 모든 플레이어 확인
        var allPlayers = MultiRoomManager.Instance?.GetAllPlayers();
        if (allPlayers != null)
        {
            foreach (var player in allPlayers)
            {
                if (player == null || player.Object == null || !player.Object.IsValid)
                {
                    continue; 
                }
                // SharedModeMasterClient가 아닌 플레이어 확인
                bool isSharedModeMasterClient = SharedModeMasterClientTracker.IsPlayerSharedModeMasterClient(player.Object.InputAuthority);
                
                if (!isSharedModeMasterClient) // SharedModeMasterClient가 아닌 플레이어
                {
                    nonHostCount++;
                    if (player.IsReady)
                    {
                        readyCount++;
                    }
                }
            }
        }
        
        // 2명 고정: SharedModeMasterClient 1명 + 일반 플레이어 1명 = 일반 플레이어가 레디되면 시작 가능
        return nonHostCount == 1 && readyCount == 1;
    }
    
    // 버튼 이벤트 핸들러들
    private void OnReadyActivateClicked()
    {
        var localPlayer = GetLocalPlayer();
        if (localPlayer != null && !localPlayer.IsReady)
        {
            Debug.Log($"[ReadyStartManager] Ready activate clicked by {localPlayer.Nickname}");
            Play.SoundManager.Instance.PlayUISound("Click");
            
            // 애니메이션 후 버튼 전환을 위한 딜레이
            StartCoroutine(DelayedButtonTransition(() => {
                localPlayer.RPC_ToggleReady();
            }));
        }
    }
    
    private void OnReadyDeactivateClicked()
    {
        var localPlayer = GetLocalPlayer();
        if (localPlayer != null && localPlayer.IsReady)
        {
            Debug.Log($"[ReadyStartManager] Ready deactivate clicked by {localPlayer.Nickname}");
            
            // 애니메이션 후 버튼 전환을 위한 딜레이
            StartCoroutine(DelayedButtonTransition(() => {
                localPlayer.RPC_ToggleReady();
            }));
        }
    }
    
    private void OnStartButtonClicked()
    {
        // SharedModeMasterClient 확인
        bool isSharedModeMasterClient = SharedModeMasterClientTracker.IsLocalPlayerSharedModeMasterClient();
        
        if (isSharedModeMasterClient)
        {
            // 로컬 플레이어 직접 가져오기
            var localPlayer = GetLocalPlayer();
            if (localPlayer != null)
            {
                Debug.Log($"[ReadyStartController] Start button clicked by SharedModeMasterClient {localPlayer.Nickname}");
            }

            // GameStartManager를 통해 게임 시작
            Play.SoundManager.Instance.PlayUISound("Click");
            StartGame();
        }
    }
    
    // 버튼 애니메이션을 위한 딜레이 코루틴
    private IEnumerator DelayedButtonTransition(System.Action callback)
    {
        // 간단하게 고정된 시간 사용 (Unity 기본 버튼 애니메이션 시간)
        yield return new WaitForSeconds(0.1f);
        
        // 상태 변경 실행
        callback?.Invoke();
    }
    
    private void StartGame()
    {
        Debug.Log("게임을 시작합니다!");
        
        // SharedModeMasterClient만 게임 시작 가능
        bool isSharedModeMasterClient = SharedModeMasterClientTracker.IsLocalPlayerSharedModeMasterClient();

        if (isSharedModeMasterClient)
        {
            // GameStartManager를 통해 게임 시작
            GameStartManager gameStartManager = FindFirstObjectByType<GameStartManager>();
            if (gameStartManager != null)
            {
                gameStartManager.RequestGameStart();
            }
            else
            {
                Debug.LogError("[ReadyStartController] GameStartManager not found!");
            }
        }
    }
    
    /// <summary>
    /// 곡 선택 변경 시 일반 클라이언트들의 레디 상태 해제
    /// </summary>
    public void ClearNonMasterPlayersReady()
    {
        // 로컬 플레이어가 마스터가 아니면서 레디 상태라면 해제
        if (!SharedModeMasterClientTracker.IsLocalPlayerSharedModeMasterClient())
        {
            var localPlayer = GetLocalPlayer();
            if (localPlayer != null && localPlayer.IsReady)
            {
                Debug.Log($"[ReadyStartController] Clearing ready state for local non-master client: {localPlayer.Nickname}");
                // RPC를 통해 레디 상태 해제 (UI 업데이트는 PlayerStateController에서 자동 처리)
                localPlayer.RPC_SetReadyState(false);
                
                // UI 업데이트는 제거 - PlayerStateController의 네트워크 동기화 후 자동 업데이트됨
            }
        }
    }
}
