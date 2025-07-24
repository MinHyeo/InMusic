using UnityEngine;
using UnityEngine.UI;

public class PlayerSlotUI : MonoBehaviour
{
    [SerializeField] private Text _nicknameText;
    [SerializeField] private GameObject _readyIcon;
    [SerializeField] private ReadyStartController _readyStartController;  // 레디/스타트 관리자

    private PlayerStateController _boundPlayer;
    
    private string _lastNickname;
    private bool _lastReady;

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
        
        UpdateDisplay();
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

    public void UpdateDisplay()
    {
        if (_boundPlayer == null) return;
        if (_lastNickname == _boundPlayer.Nickname && _lastReady == _boundPlayer.IsReady) return;

        // 플레이어 정보 표시
        _nicknameText.text = _boundPlayer.Nickname;
        _readyIcon.SetActive(_boundPlayer.IsReady);

        // 버튼 상태 업데이트
        if (_readyStartController != null)
        {
            _readyStartController.UpdateButtonStates();
        }

        _lastNickname = _boundPlayer.Nickname;
        _lastReady = _boundPlayer.IsReady;
        Debug.Log($"[PlayerSlotUI] Updated: {_boundPlayer.Nickname} (Ready: {_boundPlayer.IsReady})");
    }
}