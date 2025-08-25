using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MultiRoomManager : Managers.Singleton<MultiRoomManager>
{
    [SerializeField]
    private string _roomName;

    public string RoomName => _roomName;

    public string songName;
    public string artist;

    // PlayerStateController들을 조회해서 정보 가져오기
    public List<PlayerStateController> GetAllPlayers()
    {
        return FindObjectsByType<PlayerStateController>(FindObjectsSortMode.None).ToList();
    }

    public PlayerStateController GetRoomHost()
    {
        var allPlayers = GetAllPlayers();

        // SharedModeMasterClientTracker 사용 (존재하는 경우)
        var masterClientPlayer = SharedModeMasterClientTracker.GetSharedModeMasterClientPlayerRef();
        if (masterClientPlayer.HasValue)
        {
            return allPlayers.FirstOrDefault(p => p.Object.InputAuthority == masterClientPlayer.Value);
        }

        // Tracker가 없으면 로컬 플레이어 기준으로만 확인
        return allPlayers.FirstOrDefault(p => IsSharedModeMasterClient(p));
    }

    public bool IsMasterClient(PlayerStateController player)
    {
        if (player == null) return false;

        // SharedModeMasterClientTracker 사용 (존재하는 경우)
        return SharedModeMasterClientTracker.IsPlayerSharedModeMasterClient(player.Object.InputAuthority);
    }

    /// <summary>
    /// 해당 플레이어가 SharedModeMasterClient인지 확인
    /// </summary>
    private bool IsSharedModeMasterClient(PlayerStateController player)
    {
        var runner = NetworkManager.runnerInstance;
        if (runner == null) return false;

        // 로컬 플레이어가 Master Client이고, 해당 player가 로컬 플레이어인 경우
        if (runner.IsSharedModeMasterClient && player.Object.InputAuthority == runner.LocalPlayer)
        {
            return true;
        }

        // 다른 플레이어가 Master Client인지 확인하려면 별도 추적 시스템 필요
        // 현재는 로컬 Master Client만 확인 가능
        return false;
    }

    public int GetPlayerCount()
    {
        return GetAllPlayers().Count;
    }

    public void SetRoomName(string roomName)
    {
        _roomName = roomName;
        Debug.Log($"[MultiRoom Manager] Room name set to: {roomName}");
    }

    /// <summary>
    /// NetworkManager에서 플레이어가 나갔을 때 호출됨
    /// SharedModeMasterClient 시스템이 자동으로 권한을 관리하므로 UI 업데이트만 처리
    /// </summary>
    public void NotifyPlayerLeft(Fusion.PlayerRef leftPlayer)
    {
        Debug.Log($"[MultiRoomManager] Player left notification: {leftPlayer}");

        PlayerUIController uiController = FindFirstObjectByType<PlayerUIController>();
        if (uiController != null)
        {
            Debug.Log("[MultiRoomManager] PlayerUIController found! Forcing UI refresh due to player left");
            uiController.ForceRefreshAllSlots();
        }
        else
        {
            Debug.LogError("[MultiRoomManager] PlayerUIController NOT FOUND!");
        }
    }

    public void DestroyRoomManager()
    {
        Debug.Log("[MultiRoom Manager] Destroying Room Manager instance.");
        Destroy(gameObject);
    }

    /// <summary>
    /// 게임 시작 (오직 MasterClient만 가능)
    /// </summary>
    public void StartGame(string sceneName = "GameScene")
    {
        // MasterClient 권한 확인
        var runner = NetworkManager.runnerInstance;
        if (runner == null || !runner.IsSharedModeMasterClient)
        {
            Debug.LogWarning("[MultiRoomManager] Only MasterClient can start the game!");
            return;
        }

        Debug.Log($"[MultiRoomManager] Starting game - Loading scene: {sceneName}");

        // 모든 플레이어가 준비되었는지 확인
        var allPlayers = GetAllPlayers();
        var notReadyPlayers = allPlayers.Where(p => !p.IsReady).ToList();

        if (notReadyPlayers.Count > 0)
        {
            Debug.LogWarning($"[MultiRoomManager] Cannot start - {notReadyPlayers.Count} players not ready");
            return;
        }

        // 씬 로딩 (MasterClient만 가능)
        Debug.Log($"[MultiRoomManager] Loading scene: {sceneName}");
        runner.LoadScene(sceneName);
    }


    /// <summary>
    /// 방장 권한 양도 처리 (SharedModeMasterClient 활용)
    /// </summary>
    public void TransferHostTo(PlayerStateController targetPlayer)
    {
        if (targetPlayer != null)
        {
            Debug.Log($"[MultiRoomManager] Transferring master client to: {targetPlayer.Nickname}");

            // Fusion의 내장 Master Client 시스템 사용
            var runner = NetworkManager.runnerInstance;
            if (runner != null && runner.IsSharedModeMasterClient)
            {
                runner.SetMasterClient(targetPlayer.Object.InputAuthority);
                Debug.Log($"[MultiRoomManager] Master client transferred to {targetPlayer.Nickname}");
            }
            else
            {
                Debug.LogWarning("[MultiRoomManager] Cannot transfer - not current master client or runner not found");
            }
        }
    }

    /// <summary>
    /// 플레이어 강제 퇴장 (SharedModeMasterClient 활용)
    /// </summary>
    public void KickPlayer(PlayerStateController targetPlayer)
    {
        // MasterClient 권한 확인
        var runner = NetworkManager.runnerInstance;
        if (runner == null || !runner.IsSharedModeMasterClient)
        {
            Debug.LogWarning("[MultiRoomManager] Only MasterClient can kick players!");
            return;
        }

        if (targetPlayer == null)
        {
            Debug.LogError("[MultiRoomManager] Target player is null");
            return;
        }

        // 자기 자신은 강퇴할 수 없음
        if (targetPlayer.Object.InputAuthority == runner.LocalPlayer)
        {
            Debug.LogWarning("[MultiRoomManager] Cannot kick yourself!");
            return;
        }

        Debug.Log($"[MultiRoomManager] Kicking player: {targetPlayer.Nickname}");

        // Fusion에서 플레이어 연결 해제
        try
        {
            runner.Disconnect(targetPlayer.Object.InputAuthority);
            Debug.Log($"[MultiRoomManager] Successfully kicked player: {targetPlayer.Nickname}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[MultiRoomManager] Failed to kick player {targetPlayer.Nickname}: {e.Message}");
        }
    }

    public void SetSongInfo(string songTitle, string artist)
    {
        this.songName = songTitle;
        this.artist = artist;
    }
}
