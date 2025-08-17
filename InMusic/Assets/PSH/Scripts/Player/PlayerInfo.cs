using Fusion;
using UnityEngine;
using System; // Action�� ����ϱ� ���� �߰�

public class PlayerInfo : NetworkBehaviour
{
    public enum PlayerType{
        Host = 0,
        Client = 1
    }
    public static event Action<PlayerRef, NetworkObject> OnPlayerObjectInitialized;

    [Networked]
    public NetworkString<_16> PlayerName { get; set; } // 16�� ����
    
    [Networked]
    public PlayerType PlayerRole{get; set;}

    [Networked, OnChangedRender(nameof(OnLoadStateChangedRender))]
    public bool IsLoaded { get; set; } = false;

    [Networked, OnChangedRender(nameof(OnStateChangedRender))]
    public bool IsOwner { get; set; } = false;

    [Networked ,OnChangedRender(nameof(OnStateChangedRender))]
    // �÷��̾� �غ� ����
    public bool IsReady { get; set; }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_SetReady(bool readyState)
    {
        //������ IsReady [Networked] ������ ������Ʈ, �� ������ Fusion�� ���� �ڵ����� ����ȭ
        IsReady = readyState;
        Debug.Log($"�������� {PlayerName.ToString()}�� �غ� ���¸� {IsReady}�� ����.");
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_SetOwner(bool setOwner)
    {
        IsOwner = setOwner;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_InitReady()
    {
        IsReady = false;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void Rpc_CheckLoad()
    {
        GameObject gamePlayeySceneRespawner= GameObject.Find("PlayerRespawner");
        if (gamePlayeySceneRespawner == null) {
            Debug.Log("���� �ٸ� ��");
            return;
        }
        Debug.Log("���� �÷��� ��");
        gamePlayeySceneRespawner.GetComponent<PlayerRespawner>().CheckPlayerLoad();
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            //PlayerName = GameManager_PSH.Data.GetPlayerName();
            Debug.Log($"���� �÷��̾�({Object.InputAuthority.PlayerId}) �̸� ����: {PlayerName}");
            //�ʱ� �غ� ���� ����
            IsReady = false;
            if (GameManager_PSH.PlayerRole) {
                PlayerRole = PlayerType.Host;
                IsOwner = true;
            }
            else
            {
                PlayerRole = PlayerType.Client;
                IsOwner = false;
            }
        }
        OnPlayerObjectInitialized?.Invoke(Object.InputAuthority, Object);

        IsLoaded = true;
    }

    public void InitReady() {
        Rpc_InitReady();
    }

    public void OnStateChangedRender() 
    {
        GameObject waitingRoomUIManager = GameObject.Find("Waiting_Room_UI"); 
        if (waitingRoomUIManager != null)
        {
            waitingRoomUIManager.GetComponent<Waiting_Room_UI>().UpdateAllPlayerStatus();
        }
        else
        {
            Debug.LogWarning("Waiting_Room_UI GameObject�� ã�� �� �����ϴ�.");
        }
    }

    public void OnLoadStateChangedRender() {
        Rpc_CheckLoad();
    }

}