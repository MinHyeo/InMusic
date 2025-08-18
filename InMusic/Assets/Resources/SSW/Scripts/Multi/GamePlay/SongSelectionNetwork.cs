using UnityEngine;
using Fusion;

public class SongSelectionNetwork : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnSongIndexNetworkChanged))] 
    public int SelectedSongIndex { get; set; }

    public static SongSelectionNetwork Instance { get; private set; }
    private MultiSongListController _controller;

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
        // StateAuthority 확인 로그 (디버깅용)
        Debug.Log($"[SongSelectionNetwork] FixedUpdateNetwork - HasStateAuthority: {Object.HasStateAuthority}, IsMaster: {Runner.IsSharedModeMasterClient}");
        
        // 기존에 해당 메소드에서 변경감지를 시도했는데, 일반 클라이언트에서 정상적으로 동기화되지 않는 문제가 발생함
        // OnChangedRender 콜백 사용
    }

    /// <summary>
    /// SelectedSongIndex가 네트워크를 통해 변경될 때 모든 클라이언트에서 호출되는 콜백
    /// </summary>
    private void OnSongIndexNetworkChanged()
    {
        Debug.Log($"[SongSelectionNetwork] OnSongIndexNetworkChanged called - IsMaster: {Runner.IsSharedModeMasterClient}, Index: {SelectedSongIndex}");
        
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