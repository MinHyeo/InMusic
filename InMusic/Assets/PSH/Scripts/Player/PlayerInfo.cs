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

    // �÷��̾� �غ� ����
    // [Networked(OnChanged = nameof(OnPlayerReadyStatusChanged))] // IsReady ���� �� ȣ��� �ݹ� �Լ� ����
    public bool IsReady { get; set; }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            //PlayerName = GameManager_PSH.Data.GetPlayerName();
            Debug.Log($"���� �÷��̾�({Object.InputAuthority.PlayerId}) �̸� ����: {PlayerName}");

            // �ʱ� �غ� ���� ���� (��: �⺻������ false)
            IsReady = false;
            PlayerType = GameManager_PSH.PlayerRole;
        }
        OnPlayerObjectInitialized?.Invoke(Object.InputAuthority, Object);
    }
    /*
    public static void OnPlayerReadyStatusChanged(Changed<PlayerInfo> changed)
    {
        PlayerInfo playerInfo = changed.Behaviour;
        Debug.Log($"�÷��̾� �غ� ���� ���� ����: {playerInfo.PlayerName} IsReady: {playerInfo.IsReady}");
        //TODO: UI���� ���� ���� �ѱ��
    }*/
}