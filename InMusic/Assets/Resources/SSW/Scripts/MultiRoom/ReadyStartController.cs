using UnityEngine;
using UnityEngine.UI;

public class ReadyStartController : MonoBehaviour
{
    [Header("Ready Buttons")]
    [SerializeField] private Button _readyActivateButton;    // 레디 활성화 버튼
    [SerializeField] private Button _readyDeactivateButton;  // 레디 비활성화 버튼
    
    [Header("Start Button")]
    [SerializeField] private Button _startButton;            // 방장용 스타트 버튼
    
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
        if (_startButton != null)
        {
            _startButton.onClick.RemoveAllListeners();
        }
        
        _boundPlayer = null;
    }
    
    public void UpdateButtonStates()
    {
        if (_boundPlayer == null) return;
        
        if (_boundPlayer.Object.HasInputAuthority)
        {
            if (IsHost())
            {
                ShowStartButton();
                HideReadyButtons();
            }
            else
            {
                ShowReadyButtons();
                HideStartButton();
            }
        }
        else
        {
            // 다른 플레이어의 슬롯: 모든 버튼 비활성화 상태로 표시
            ShowReadyButtonsAsDisabled();
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
        
        // 스타트 버튼 설정
        if (_startButton != null)
        {
            _startButton.onClick.RemoveAllListeners();
            _startButton.onClick.AddListener(OnStartButtonClicked);
        }
    }
    
    private bool IsHost()
    {
        return _boundPlayer != null && _boundPlayer.Object.HasStateAuthority;
    }
    
    private void ShowStartButton()
    {
        if (_startButton != null)
        {
            _startButton.gameObject.SetActive(true);
            
            bool allPlayersReady = CheckAllPlayersReady();
            _startButton.interactable = allPlayersReady;
            
            // 스타트 버튼 텍스트 업데이트
            Text buttonText = _startButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = allPlayersReady ? "START" : "WAIT";
            }
            
            // 버튼 색상 변경
            ColorBlock colors = _startButton.colors;
            colors.normalColor = allPlayersReady ? Color.green : Color.gray;
            _startButton.colors = colors;
        }
    }
    
    private void HideStartButton()
    {
        if (_startButton != null)
        {
            _startButton.gameObject.SetActive(false);
        }
    }
    
    private void ShowReadyButtons()
    {
        if (_boundPlayer.IsReady)
        {
            // 레디 상태: 비활성화 버튼만 표시
            if (_readyActivateButton != null) _readyActivateButton.gameObject.SetActive(false);
            if (_readyDeactivateButton != null) 
            {
                _readyDeactivateButton.gameObject.SetActive(true);
                _readyDeactivateButton.interactable = true;
            }
        }
        else
        {
            // 레디 안한 상태: 활성화 버튼만 표시
            if (_readyActivateButton != null) 
            {
                _readyActivateButton.gameObject.SetActive(true);
                _readyActivateButton.interactable = true;
            }
            if (_readyDeactivateButton != null) _readyDeactivateButton.gameObject.SetActive(false);
        }
    }
    
    private void HideReadyButtons()
    {
        if (_readyActivateButton != null) _readyActivateButton.gameObject.SetActive(false);
        if (_readyDeactivateButton != null) _readyDeactivateButton.gameObject.SetActive(false);
    }
    
    private void ShowReadyButtonsAsDisabled()
    {
        if (_boundPlayer.IsReady)
        {
            if (_readyActivateButton != null) _readyActivateButton.gameObject.SetActive(false);
            if (_readyDeactivateButton != null) 
            {
                _readyDeactivateButton.gameObject.SetActive(true);
                _readyDeactivateButton.interactable = false;
            }
        }
        else
        {
            if (_readyActivateButton != null) 
            {
                _readyActivateButton.gameObject.SetActive(true);
                _readyActivateButton.interactable = false;
            }
            if (_readyDeactivateButton != null) _readyDeactivateButton.gameObject.SetActive(false);
        }
    }
    
    private bool CheckAllPlayersReady()
    {
        int nonHostCount = 0;
        int readyCount = 0;
        
        foreach (var player in PlayerStateController.AllPlayers)
        {
            if (!player.Object.HasStateAuthority) // 방장이 아닌 플레이어
            {
                nonHostCount++;
                if (player.IsReady)
                {
                    readyCount++;
                }
            }
        }
        
        return nonHostCount > 0 && readyCount == nonHostCount;
    }
    
    // 버튼 이벤트 핸들러들
    private void OnReadyActivateClicked()
    {
        if (_boundPlayer != null && _boundPlayer.Object.HasInputAuthority && !_boundPlayer.IsReady)
        {
            Debug.Log($"[ReadyStartManager] Ready activate clicked by {_boundPlayer.Nickname}");
            _boundPlayer.RPC_ToggleReady();
        }
    }
    
    private void OnReadyDeactivateClicked()
    {
        if (_boundPlayer != null && _boundPlayer.Object.HasInputAuthority && _boundPlayer.IsReady)
        {
            Debug.Log($"[ReadyStartManager] Ready deactivate clicked by {_boundPlayer.Nickname}");
            _boundPlayer.RPC_ToggleReady();
        }
    }
    
    private void OnStartButtonClicked()
    {
        if (_boundPlayer != null && _boundPlayer.Object.HasStateAuthority)
        {
            Debug.Log($"[ReadyStartManager] Start button clicked by host {_boundPlayer.Nickname}");
            StartGame();
        }
    }
    
    private void StartGame()
    {
        Debug.Log("게임을 시작합니다!");
        // 나중에 게임 시작 로직 구현
    }
}
