using UnityEngine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerStateController : NetworkBehaviour
{
    [Networked]
    public string Nickname { get; set; }

    [Networked]
    public bool IsReady { get; set; }
    
    [Networked]
    public bool IsRed { get; set; }

    private string _previousNickname;
    private bool _previousIsReady;

    public override void FixedUpdateNetwork()
    {
        // 네트워크 속성 변경 감지
        if (Nickname != _previousNickname ||
            IsReady != _previousIsReady)
        {
            OnStateChanged();
            _previousNickname = Nickname;
            _previousIsReady = IsReady;
        }

        bool isMasterClient = SharedModeMasterClientTracker.IsPlayerSharedModeMasterClient(Object.InputAuthority);
        if (isMasterClient && IsReady && Object.HasInputAuthority)
        {
            Debug.Log($"[PlayerStateController] {Nickname} became master client, auto-canceling ready state");
            IsReady = false; // 방장이 되면 자동으로 레디 해제
        }
    }

    private void OnStateChanged()
    {
        // SharedModeMasterClient 상태 확인
        bool isSharedModeMasterClient = SharedModeMasterClientTracker.IsPlayerSharedModeMasterClient(Object.InputAuthority);
        string hostStatus = isSharedModeMasterClient ? "SHARED MASTER CLIENT" : "CLIENT";
        Debug.Log($"[PlayerState] Updated: {Nickname} (Ready: {IsReady}, Role: {hostStatus})");

        // UI에 상태 변경 알림
        NotifyUIUpdate();
    }

    private void NotifyUIUpdate()
    {
        // PlayerUIController 찾아서 해당 플레이어의 슬롯만 업데이트
        PlayerUIController uiController = FindFirstObjectByType<PlayerUIController>();
        if (uiController != null)
        {
            uiController.UpdatePlayerSlot(this);
        }
    }

    public override void Spawned()
    {
        // DontDestroyOnLoad 테스트
        DontDestroyOnLoad(gameObject);
        Debug.Log($"[PlayerState] Spawned - Object.HasInputAuthority: {Object.HasInputAuthority}");
        Debug.Log($"[PlayerState] Spawned - Object.InputAuthority: {Object.InputAuthority}");
        Debug.Log($"[PlayerState] Spawned - Runner.LocalPlayer: {Runner.LocalPlayer}");

        if (Object.HasInputAuthority)
        {
            string nickname = PlayerInfoProvider.GetUserNickname();
            Debug.Log($"[Spawned] Setting Nickname: {nickname}");
            Debug.Log($"SharedModeMasterClient: {NetworkManager.runnerInstance.IsSharedModeMasterClient}");

            // 간단한 초기화 요청
            RPC_RequestInitialization(nickname);
        }
        else
        {
            Debug.Log($"[Spawned] No InputAuthority - skipping nickname setup");
        }

        Debug.Log($"[PlayerState] Spawned Complete: {Nickname} (Ready: {IsReady})");
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        string leavingPlayerName = Nickname;
        Debug.Log($"[Despawned] {leavingPlayerName} left the room");
        Debug.Log($"[Despawned] Authority Info - HasStateAuthority: {Object.HasStateAuthority}, InputAuthority: {Object.InputAuthority}");
        Debug.Log($"[Despawned] DontDestroyOnLoad 적용되어 있지만 Despawned 호출");

        // SharedModeMasterClient는 Fusion이 자동으로 승계
    }

    #region Fusion Callbacks

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestInitialization(string nickname)
    {
        Debug.Log($"[RPC_RequestInitialization] Initializing: {nickname}");

        // 닉네임만 설정 (SharedModeMasterClient는 Fusion이 자동 관리)
        Nickname = nickname;

        // 초기화 완료 후 UI 업데이트 강제 실행
        Invoke(nameof(ForceUIUpdateAfterInit), 0.2f);

        Debug.Log($"[RPC_RequestInitialization] Player {nickname} initialized");
    }

    private void ForceUIUpdateAfterInit()
    {
        Debug.Log($"[PlayerState] Force UI update after initialization for {Nickname}");

        // 모든 클라이언트에서 UI 업데이트
        PlayerUIController uiController = FindFirstObjectByType<PlayerUIController>();
        if (uiController != null)
        {
            uiController.ForceRefreshAllSlots();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetNickname(string name)
    {
        Debug.Log($"[RPC_SetNickname] Received: {name}");
        Nickname = name;
        Debug.Log($"[RPC_SetNickname] Set Nickname to: {Nickname}");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_ToggleReady()
    {
        bool oldReady = IsReady;
        IsReady = !IsReady;
        Debug.Log($"[PlayerState] {Nickname} Ready toggled: {oldReady} → {IsReady}");

        // Ready 상태 변경 후 모든 클라이언트에 강제 UI 업데이트 알림
        RPC_NotifyReadyStateChanged(Nickname, IsReady);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_NotifyReadyStateChanged(string playerName, bool readyState)
    {
        Debug.Log($"[PlayerState] Ready state changed notification: {playerName} → {readyState}");

        // 약간의 지연 후 UI 업데이트 (네트워크 동기화 완료 대기)
        Invoke(nameof(ForceUIUpdateAfterReady), 0.1f);
    }

    private void ForceUIUpdateAfterReady()
    {
        Debug.Log("[PlayerState] Force UI update after Ready state change");

        PlayerUIController uiController = FindFirstObjectByType<PlayerUIController>();
        if (uiController != null)
        {
            uiController.ForceRefreshAllSlots();
            
            // UI 재바인딩 후 SongSelectionNetwork 상태 복원
            StartCoroutine(RestoreSongSelectionSync());
        }
    }

    private IEnumerator RestoreSongSelectionSync()
    {
        // UI 재바인딩 완료 대기
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();
        
        // SongSelectionNetwork 동기화 복원
        if (SongSelectionNetwork.Instance != null)
        {
            int currentIndex = SongSelectionNetwork.Instance.GetCurrentSongIndex();
            Debug.Log($"[PlayerState] Restoring song selection sync to index: {currentIndex}");
            
            MultiSongListController controller = FindFirstObjectByType<MultiSongListController>();
            if (controller != null)
            {
                controller.ForceCenterAtIndex(currentIndex);
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetColor(bool isRed)
    {
        Debug.Log($"[PlayerState] RPC_SetColor: {Nickname} → {(isRed ? "RED" : "BLUE")}");
        IsRed = isRed;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_RequestGameStart()
    {
        // SharedModeMasterClient만 게임 시작 로직 실행
        if (NetworkManager.runnerInstance.IsSharedModeMasterClient && Object.HasInputAuthority)
        {
            Debug.Log($"[GameStart] Game start requested by SharedModeMasterClient: {Nickname}");

            // GameStartManager를 통해 게임 시작
            GameStartManager gameStartManager = FindFirstObjectByType<GameStartManager>();
            if (gameStartManager != null)
            {
                gameStartManager.RequestGameStart();
            }
            else
            {
                Debug.LogError("[PlayerStateController] GameStartManager not found!");
            }
        }
    }
    
    
    #endregion
}
