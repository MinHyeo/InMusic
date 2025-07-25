using UnityEngine;
using Fusion;
using System.Collections.Generic;

public class PlayerStateController : NetworkBehaviour
{
    [Networked] 
    public string Nickname { get; set; }
    
    [Networked] 
    public bool IsReady { get; set; }

    public static List<PlayerStateController> AllPlayers = new();

    private string _previousNickname;
    private bool _previousIsReady;

    public override void FixedUpdateNetwork()
    {
        // 네트워크 속성 변경 감지
        if (Nickname != _previousNickname || IsReady != _previousIsReady)
        {
            OnStateChanged();
            _previousNickname = Nickname;
            _previousIsReady = IsReady;
        }
    }

    private void OnStateChanged()
    {
        Debug.Log($"[PlayerState] Updated: {Nickname} (Ready: {IsReady})");
        
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
            RPC_SetNickname(nickname);
        }
        else
        {
            Debug.Log($"[Spawned] No InputAuthority - skipping nickname setup");
        }

        Debug.Log($"[PlayerState] Spawned Complete: {Nickname} (Ready: {IsReady})");
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        AllPlayers.Remove(this);
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
}
