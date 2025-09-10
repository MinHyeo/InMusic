using System.Collections.Generic;
using Fusion;
using UnityEngine;
using SongList;

/// <summary>
/// SharedModeMasterClient 기반 게임 시작 관리자
/// </summary>
public class GameStartManager : NetworkBehaviour
{
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StartGame(string songTitle, string songArtist, string songDuration)
    {
        Debug.Log($"[GameStartManager] RPC_StartGame - Song: {songTitle}, IsSharedModeMasterClient: {NetworkManager.runnerInstance.IsSharedModeMasterClient}");
        
        // 스프라이트는 로컬에서 로드 (네트워크 전송 비용 절약)
        Sprite songSprite = Resources.Load<Sprite>($"Song/{songTitle}/{songTitle}");
        MultiRoomManager.Instance.SetSongInfo(songTitle, songArtist);
        // 모든 클라이언트가 로딩 UI 표시 - 네트워크로 전달받은 정확한 데이터 사용
        MultiLoadingSong loadingSong = MultiLoadingSong.Instance;
        if (loadingSong != null)
        {
            loadingSong.LoadPlay("MultiPlay_InMusic", songTitle, songArtist, songDuration, songSprite);
        }
        else
        {
            Debug.LogError("[GameStartManager] MultiLoadingSong.Instance is null!");
        }

        // SharedModeMasterClient만 실제 네트워크 씬 로딩 실행
        if (NetworkManager.runnerInstance.IsSharedModeMasterClient)
        {
            Debug.Log("[GameStartManager] SharedModeMasterClient starting network scene load...");
            // 세션 프로퍼티는 이미 RequestGameStart()에서 업데이트됨
            Debug.Log($"[GameStartManager] Network scene loading for song: {songTitle}");
        }
        else
        {
            Debug.Log("[GameStartManager] Non-master client - waiting for network scene load from master");
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
            
            // 현재 선택된 곡 정보 가져오기
            MultiHighlightSong highlightSong = FindFirstObjectByType<MultiHighlightSong>();
            if (highlightSong != null)
            {
                var (title, artist, duration, sprite) = highlightSong.GetSelectedSongInfo();

                // 그 다음에 RPC 호출하여 씬 로딩 시작
                RPC_StartGame(title, artist, duration);
            }
            else
            {
                Debug.LogError("[GameStartManager] MultiHighlightSong not found!");
                // 폴백으로 MultiSongListController에서 정보 가져오기
                string selectedSongName = GetSelectedSongName();
                
                RPC_StartGame(selectedSongName, "Unknown Artist", "00:00");
            }
        }
        else
        {
            Debug.LogWarning("[GameStartManager] Game start denied - not SharedModeMasterClient");
        }
    }

    /// <summary>
    /// 세션 프로퍼티 업데이트 (씬 로딩 전에 호출)
    /// </summary>
    // private void UpdateSessionProperties(string selectedSongName, string selectedSongArtist)
    // {
    //     Debug.Log($"[GameStartManager] Updating session properties BEFORE scene loading - Song: {selectedSongName}");
        
    //     // MultiRoomManager.Instance.SetSongInfo(selectedSongName, selectedSongArtist);
    //     // try
    //     // {
    //     //     Dictionary<string, SessionProperty> newProps = new()
    //     //     {
    //     //         { "songName", selectedSongName },
    //     //         { "gameStarted", true }
    //     //     };

    //     //     NetworkManager.runnerInstance.SessionInfo.UpdateCustomProperties(newProps);

    //     //     Debug.Log($"[GameStartManager] Session properties updated successfully - gameStarted: true, songName: {selectedSongName}");
    //     // }
    //     // catch (System.Exception ex)
    //     // {
    //     //     Debug.LogError($"[GameStartManager] Failed to update session properties: {ex.Message}");
    //     // }
    // }
}
