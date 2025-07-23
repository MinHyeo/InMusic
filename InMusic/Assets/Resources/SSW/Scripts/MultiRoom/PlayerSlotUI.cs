using UnityEngine;
using UnityEngine.UI;

public class PlayerSlotUI : MonoBehaviour
{
    [SerializeField] private Text _nicknameText;
    [SerializeField] private GameObject _readyIcon;
    private PlayerStateController _boundPlayer;
    
    private string _lastNickname;
    private bool _lastReady;

    public bool IsAvailable => _boundPlayer == null;

    public PlayerStateController BoundPlayer => _boundPlayer;

    public void Bind(PlayerStateController player)
    {
        _boundPlayer = player;
        gameObject.SetActive(true);
        UpdateDisplay();
    }

    public void Unbind()
    {
        _boundPlayer = null;
        gameObject.SetActive(false);
    }

    public void UpdateDisplay()
    {
        if (_boundPlayer == null) return;
        if (_lastNickname == _boundPlayer.Nickname && _lastReady == _boundPlayer.IsReady) return;

        _nicknameText.text = _boundPlayer.Nickname;
        _readyIcon.SetActive(_boundPlayer.IsReady);

        _lastNickname = _boundPlayer.Nickname;
        _lastReady = _boundPlayer.IsReady;
        Debug.Log($"[PlayerSlotUI] Updated: {_boundPlayer.Nickname} (Ready: {_boundPlayer.IsReady})");
    }
}