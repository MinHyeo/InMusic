using Fusion;
using UnityEngine;
using System; // Action을 사용하기 위해 추가

public class PlayerInfo : NetworkBehaviour
{
    public enum PlayerType{
        Host = 0,
        Client = 1
    }
    public static event Action<PlayerRef, NetworkObject> OnPlayerObjectInitialized;

    [Networked]
    public NetworkString<_16> PlayerName { get; set; } // 16자 제한
    
    [Networked]
    public PlayerType PlayerRole{get; set;}

    [Networked, OnChangedRender(nameof(OnOwnerStateChangedRender))]
    public bool IsOwner { get; set; } = false;

    [Networked ,OnChangedRender(nameof(OnReadyStateChangedRender))]
    // 플레이어 준비 상태
    public bool IsReady { get; set; }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_SetReady(bool readyState)
    {
        //서버는 IsReady [Networked] 변수를 업데이트, 이 변경은 Fusion에 의해 자동으로 동기화
        IsReady = readyState;
        Debug.Log($"서버에서 {PlayerName.ToString()}의 준비 상태를 {IsReady}로 설정.");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_InitReady()
    {
        IsReady = false;
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            //PlayerName = GameManager_PSH.Data.GetPlayerName();
            Debug.Log($"로컬 플레이어({Object.InputAuthority.PlayerId}) 이름 설정: {PlayerName}");
            //초기 준비 상태 설정
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
    }

    public void InitReady() {
        Rpc_InitReady();
    }

    public void OnOwnerStateChangedRender() {
        GameObject waitingRoomUIManager = GameObject.Find("Waiting_Room_UI");
        if (waitingRoomUIManager != null)
        {
            waitingRoomUIManager.GetComponent<Waiting_Room_UI>().UpdateAllPlayerReady();
        }
        else
        {
            Debug.LogWarning("Waiting_Room_UI GameObject를 찾을 수 없습니다.");
        }
    }
    public void OnReadyStateChangedRender() 
    {
        GameObject waitingRoomUIManager = GameObject.Find("Waiting_Room_UI"); 
        if (waitingRoomUIManager != null)
        {
            waitingRoomUIManager.GetComponent<Waiting_Room_UI>().UpdateAllPlayerReady();
        }
        else
        {
            Debug.LogWarning("Waiting_Room_UI GameObject를 찾을 수 없습니다.");
        }
    }

}