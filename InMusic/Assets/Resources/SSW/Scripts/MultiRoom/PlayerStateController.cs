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
    }

    public override void Spawned()
    {
        AllPlayers.Add(this);

        if (Object.HasInputAuthority)
        {
            string nickname = PlayerInfoProvider.GetUserNickname();
            Debug.Log($"[Spawned] Setting Nickname: {nickname}");
            RPC_SetNickname(nickname);
        }

        Debug.Log($"[PlayerState] Spawned: {Nickname} (Ready: {IsReady})");
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        AllPlayers.Remove(this);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetNickname(string name)
    {
        Nickname = name;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_ToggleReady()
    {
        IsReady = !IsReady;
    }
}
