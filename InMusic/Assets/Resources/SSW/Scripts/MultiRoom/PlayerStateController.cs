using UnityEngine;
using Fusion;
using System.Collections.Generic;

public class PlayerStateController : NetworkBehaviour
{
    [Networked] 
    public string Nickname { get; set; }
    
    [Networked] 
    public bool IsReady { get; set; }
    
    [Networked]
    public bool IsRoomHost { get; set; }  // 커스텀 방장 권한
    
    [Networked]
    public int JoinOrder { get; set; }  // 입장 순서 (방장 선정용)

    public static List<PlayerStateController> AllPlayers = new();

    private string _previousNickname;
    private bool _previousIsReady;
    private bool _previousHasStateAuthority;
    private bool _previousIsRoomHost;

    public override void FixedUpdateNetwork()
    {
        // 네트워크 속성 변경 감지 (Host 권한 포함)
        bool currentHasStateAuthority = Object.HasStateAuthority;
        
        if (Nickname != _previousNickname || 
            IsReady != _previousIsReady || 
            currentHasStateAuthority != _previousHasStateAuthority ||
            IsRoomHost != _previousIsRoomHost)
        {
            OnStateChanged();
            _previousNickname = Nickname;
            _previousIsReady = IsReady;
            _previousHasStateAuthority = currentHasStateAuthority;
            _previousIsRoomHost = IsRoomHost;
        }
    }

    private void OnStateChanged()
    {
        string hostStatus = IsRoomHost ? "ROOM HOST" : (Object.HasStateAuthority ? "SERVER" : "CLIENT");
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
        AllPlayers.Add(this);

        Debug.Log($"[PlayerState] Spawned - Object.HasInputAuthority: {Object.HasInputAuthority}");
        Debug.Log($"[PlayerState] Spawned - Object.InputAuthority: {Object.InputAuthority}");
        Debug.Log($"[PlayerState] Spawned - Runner.LocalPlayer: {Runner.LocalPlayer}");
        Debug.Log($"[PlayerState] Spawned - Object.HasStateAuthority: {Object.HasStateAuthority}");

        if (Object.HasInputAuthority)
        {
            string nickname = PlayerInfoProvider.GetUserNickname();
            Debug.Log($"[Spawned] Setting Nickname: {nickname}");
            
            // 서버에게 초기화 요청 (RPC 사용)
            RPC_RequestInitialization(nickname);
        }
        else
        {
            Debug.Log($"[Spawned] No InputAuthority - skipping nickname setup");
        }

        Debug.Log($"[PlayerState] Spawned Complete: {Nickname} (Ready: {IsReady}, IsRoomHost: {IsRoomHost})");
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        AllPlayers.Remove(this);
        
        // 방장이 나갔을 때는 서버가 자동으로 새로운 방장을 선정
        // (StateAuthority를 가진 서버에서만 실행됨)
        if (Object.HasStateAuthority && IsRoomHost && AllPlayers.Count > 0)
        {
            AssignNewRoomHost();
        }
    }

    /// <summary>
    /// 방장 권한을 체크하고 필요시 할당 (서버에서만 실행)
    /// </summary>
    private void CheckAndAssignRoomHost()
    {
        if (!Object.HasStateAuthority) return; // 서버에서만 실행
        
        // 현재 방장이 있는지 확인
        bool hasRoomHost = false;
        foreach (var player in AllPlayers)
        {
            if (player.IsRoomHost)
            {
                hasRoomHost = true;
                break;
            }
        }
        
        // 방장이 없으면 가장 먼저 들어온 플레이어를 방장으로 설정
        if (!hasRoomHost)
        {
            PlayerStateController earliestPlayer = GetEarliestPlayer();
            if (earliestPlayer != null)
            {
                earliestPlayer.IsRoomHost = true;
                Debug.Log($"[RoomHost] Assigned room host to: {earliestPlayer.Nickname} (Join Order: {earliestPlayer.JoinOrder})");
            }
        }
    }
    
    /// <summary>
    /// 새로운 방장 선정 (서버에서만 실행)
    /// </summary>
    private void AssignNewRoomHost()
    {
        if (!Object.HasStateAuthority) return; // 서버에서만 실행
        
        PlayerStateController newHost = GetEarliestPlayer();
        if (newHost != null)
        {
            newHost.IsRoomHost = true;
            Debug.Log($"[RoomHost] New room host assigned: {newHost.Nickname} (Join Order: {newHost.JoinOrder})");
        }
    }
    
    /// <summary>
    /// 가장 먼저 들어온 플레이어 찾기
    /// </summary>
    private PlayerStateController GetEarliestPlayer()
    {
        PlayerStateController earliest = null;
        int earliestJoinOrder = int.MaxValue;
        
        foreach (var player in AllPlayers)
        {
            if (player.JoinOrder < earliestJoinOrder)
            {
                earliestJoinOrder = player.JoinOrder;
                earliest = player;
            }
        }
        
        return earliest;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestInitialization(string nickname)
    {
        Debug.Log($"[RPC_RequestInitialization] Received from {Object.InputAuthority}: {nickname}");
        
        // 닉네임 설정
        Nickname = nickname;
        
        // 입장 순서 설정
        JoinOrder = AllPlayers.Count;
        Debug.Log($"[RPC_RequestInitialization] Join Order set to: {JoinOrder}");
        
        // 방장 권한 체크 및 설정
        CheckAndAssignRoomHost();
        
        Debug.Log($"[RPC_RequestInitialization] Initialized: {Nickname} (JoinOrder: {JoinOrder}, IsRoomHost: {IsRoomHost})");
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
        IsReady = !IsReady;
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestGameStart()
    {
        if (!Object.HasStateAuthority) return; // 서버에서만 실행
        
        // 방장인지 확인
        if (!IsRoomHost)
        {
            Debug.LogWarning($"[GameStart] {Nickname} is not room host, cannot start game");
            return;
        }
        
        Debug.Log($"[GameStart] Game start requested by room host: {Nickname}");
        
        // 게임 시작 로직 - MultiPlayManager에게 알림
        NotifyGameStart();
    }
    
    /// <summary>
    /// 게임 시작을 MultiPlayManager에게 알림
    /// </summary>
    private void NotifyGameStart()
    {
        Debug.Log("[GameStart] Attempting to start game...");
        
        // 씬 전환이나 게임 시작 로직을 여기에 구현
        // 예: SceneManager.LoadScene("GameScene");
        
        // 임시로 Debug 로그만 출력
        Debug.Log("[GameStart] Game starting initiated by room host!");
        
        // TODO: 실제 게임 시작 로직 구현 필요
    }
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_TransferRoomHost(PlayerRef targetPlayer)
    {
        if (!Object.HasStateAuthority) return; // 서버에서만 실행
        
        // 현재 플레이어가 방장인지 확인
        if (!IsRoomHost)
        {
            Debug.LogWarning($"[RoomHost] {Nickname} is not room host, cannot transfer host");
            return;
        }
        
        // 대상 플레이어 찾기
        PlayerStateController targetPlayerController = null;
        foreach (var player in AllPlayers)
        {
            if (player.Object.InputAuthority == targetPlayer)
            {
                targetPlayerController = player;
                break;
            }
        }
        
        if (targetPlayerController != null)
        {
            // 방장 권한 이전
            IsRoomHost = false;
            targetPlayerController.IsRoomHost = true;
            Debug.Log($"[RoomHost] Host transferred from {Nickname} to {targetPlayerController.Nickname}");
        }
        else
        {
            Debug.LogWarning($"[RoomHost] Target player not found for host transfer");
        }
    }
}
