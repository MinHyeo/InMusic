using Fusion;
using UnityEngine;
using System; // Action을 사용하기 위해 추가

public class PlayerInfo : NetworkBehaviour
{
    public static event Action<PlayerRef, NetworkObject> OnPlayerObjectInitialized;

    [Networked]
    public NetworkString<_16> PlayerName { get; set; } // 16자 제한

    
    [Networked]
    public bool PlayerType{get; set;}

    // 플레이어 준비 상태
    // [Networked(OnChanged = nameof(OnPlayerReadyStatusChanged))] // IsReady 변경 시 호출될 콜백 함수 지정
    public bool IsReady { get; set; }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            //PlayerName = GameManager_PSH.Data.GetPlayerName();
            Debug.Log($"로컬 플레이어({Object.InputAuthority.PlayerId}) 이름 설정: {PlayerName}");

            // 초기 준비 상태 설정 (예: 기본적으로 false)
            IsReady = false;
            PlayerType = GameManager_PSH.PlayerRole;
        }
        OnPlayerObjectInitialized?.Invoke(Object.InputAuthority, Object);
    }
    /*
    public static void OnPlayerReadyStatusChanged(Changed<PlayerInfo> changed)
    {
        PlayerInfo playerInfo = changed.Behaviour;
        Debug.Log($"플레이어 준비 상태 변경 감지: {playerInfo.PlayerName} IsReady: {playerInfo.IsReady}");
        //TODO: UI한테 상태 정보 넘기기
    }*/
}