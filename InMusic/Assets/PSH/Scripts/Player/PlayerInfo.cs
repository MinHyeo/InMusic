using Fusion;
using UnityEngine;
using System; // Action�� ����ϱ� ���� �߰�

public class PlayerInfo : NetworkBehaviour
{
    public static event Action<PlayerRef, NetworkObject> OnPlayerObjectInitialized;

    [Networked]
    public NetworkString<_16> PlayerName { get; set; } // 16�� ����

    
    [Networked]
    public bool PlayerType{get; set;}

    [Networked /**/]
    // �÷��̾� �غ� ����
    public bool IsReady { get; set; }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            //PlayerName = GameManager_PSH.Data.GetPlayerName();
            Debug.Log($"���� �÷��̾�({Object.InputAuthority.PlayerId}) �̸� ����: {PlayerName}");
            //�ʱ� �غ� ���� ����
            IsReady = false;
            PlayerType = GameManager_PSH.PlayerRole;
        }
        OnPlayerObjectInitialized?.Invoke(Object.InputAuthority, Object);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_SetReady(bool readyState)
    {
        //������ IsReady [Networked] ������ ������Ʈ, �� ������ Fusion�� ���� �ڵ����� ����ȭ
        IsReady = readyState;
        Debug.Log($"�������� {PlayerName.ToString()}�� �غ� ���¸� {IsReady}�� ����.");
    }
}