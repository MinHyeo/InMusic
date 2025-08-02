using UnityEngine;
using Fusion;

public class SongSelectionNetwork : NetworkBehaviour
{
    [Networked] public int SelectedSongIndex { get; set; }

    public static SongSelectionNetwork Instance { get; private set; }
    private MultiSongListController _controller;
    
    private int _previousSelectedSongIndex = -1;

    public override void Spawned()
    {
        if (Instance == null)
        {
            Instance = this;
            _controller = FindFirstObjectByType<MultiSongListController>();

            if (_controller == null)
            {
                Debug.LogError("[SongSelectionNetwork] MultiSongListController not found");
            }
            
            _previousSelectedSongIndex = SelectedSongIndex;
        }
        else
        {
            Debug.LogWarning("[SongSelectionNetwork] Multiple instances detected");
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (_previousSelectedSongIndex != SelectedSongIndex)
        {
            OnSongIndexChanged();
            _previousSelectedSongIndex = SelectedSongIndex;
        }
    }

    /// <summary>
    /// 마스터 클라이언트가 스크롤할 때 호출하는 메서드
    /// </summary>
    public void UpdateSongIndex(int newIndex)
    {
        if (!Runner.IsSharedModeMasterClient)
        {
            Debug.LogWarning("[SongSelectionNetwork] Only SharedModeMasterClient can update song index");
            return;
        }

        if (SelectedSongIndex != newIndex)
        {
            SelectedSongIndex = newIndex;
            Debug.Log($"[SongSelectionNetwork] Updated song index to: {newIndex}");
            
            // 곡 선택 변경 시 일반 클라이언트들의 레디 상태 해제
            ClearNonMasterPlayersReady();
        }
        else
        {
            Debug.Log($"[SongSelectionNetwork] No change needed, already at index: {newIndex}");
        }
    }

    /// <summary>
    /// SelectedSongIndex가 변경될 때 자동 호출되는 콜백
    /// </summary>
    private void OnSongIndexChanged()
    {
        Debug.Log($"[SongSelectionNetwork] OnSongIndexChanged called - IsMaster: {Runner.IsSharedModeMasterClient}");
        
        // 마스터 클라이언트는 자기가 변경한 것이므로 UI 업데이트 스킵
        if (Runner.IsSharedModeMasterClient) 
        {
            Debug.Log("[SongSelectionNetwork] Skipping UI sync - This is master client");
            return;
        }

        // 일반 클라이언트만 UI 동기화
        if (_controller != null)
        {
            Debug.Log($"[SongSelectionNetwork] Syncing UI to song index: {SelectedSongIndex}");
            _controller.ForceCenterAtIndex(SelectedSongIndex);
        }
        else
        {
            Debug.LogError("[SongSelectionNetwork] Controller is null! Cannot sync UI");
        }
    }

    /// <summary>
    /// 현재 선택된 곡 인덱스 반환
    /// </summary>
    public int GetCurrentSongIndex()
    {
        return SelectedSongIndex;
    }

    /// <summary>
    /// 일반 클라이언트들의 레디 상태 해제 (곡 변경 시 호출)
    /// </summary>
    private void ClearNonMasterPlayersReady()
    {
        // ReadyStartController에 레디 해제 요청
        var readyController = FindFirstObjectByType<ReadyStartController>();
        if (readyController != null)
        {
            readyController.ClearNonMasterPlayersReady();
        }
    }
}