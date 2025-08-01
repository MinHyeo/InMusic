using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// SharedModeMasterClient 기반 게임 시작 관리자
/// </summary>
public class GameStartManager : NetworkBehaviour
{
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StartGame()
    {
        // SharedModeMasterClient만 실제 게임 시작 로직 실행
        if (NetworkManager.runnerInstance.IsSharedModeMasterClient)
        {
            Debug.Log("[GameStartManager] RPC_StartGame called by SharedModeMasterClient");
            StartGameInternal();
        }
        else
        {
            Debug.Log("[GameStartManager] RPC_StartGame received, but not SharedModeMasterClient - ignoring");
        }
    }

    private void StartGameInternal()
    {
        Debug.Log("[GameStartManager] Starting game with session properties update...");
        
        try
        {
            // 선택된 곡 정보 가져오기
            string selectedSongName = GetSelectedSongName();
            
            Dictionary<string, SessionProperty> newProps = new()
            {
                { "songName", selectedSongName },
                { "gameStarted", true }
            };

            NetworkManager.runnerInstance.SessionInfo.UpdateCustomProperties(newProps);
            
            // 게임 씬 로드
            NetworkManager.runnerInstance.LoadScene("MultiPlay");
            
            Debug.Log($"[GameStartManager] Game started successfully with song: {selectedSongName}!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GameStartManager] Failed to start game: {ex.Message}");
        }
    }

    /// <summary>
    /// 현재 선택된 곡 이름 가져오기
    /// </summary>
    private string GetSelectedSongName()
    {
        // MultiSongListController에서 현재 하이라이트된 곡 이름 바로 가져오기
        var songListController = FindFirstObjectByType<MultiSongListController>();
        if (songListController != null)
        {
            string songName = songListController.GetCurrentHighlightedSongName();
            Debug.Log($"[GameStartManager] Selected song: {songName}");
            return songName;
        }
        
        Debug.LogWarning("[GameStartManager] Could not get selected song name, using fallback");
        return "DefaultSong"; // 기본값
    }

    /// <summary>
    /// 외부에서 게임 시작 요청 (SharedModeMasterClient만 가능)
    /// </summary>
    public void RequestGameStart()
    {
        if (NetworkManager.runnerInstance.IsSharedModeMasterClient)
        {
            Debug.Log("[GameStartManager] Game start requested by SharedModeMasterClient");
            RPC_StartGame();
        }
        else
        {
            Debug.LogWarning("[GameStartManager] Game start denied - not SharedModeMasterClient");
        }
    }
}
