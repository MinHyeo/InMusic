using Fusion;
using UnityEngine;

/// <summary>
/// SharedModeMasterClient를 추적하는 시스템
/// 이 오브젝트는 MasterClientObject 플래그가 설정되어야 함
/// </summary>
public class SharedModeMasterClientTracker : NetworkBehaviour
{
    static SharedModeMasterClientTracker LocalInstance;
    private bool _wasLocalPlayerMasterClient;

    public override void Spawned()
    {
        LocalInstance = this;
        _wasLocalPlayerMasterClient = Runner.IsSharedModeMasterClient;
        Debug.Log($"[SharedModeMasterClientTracker] Spawned - LocalPlayer: {Runner.LocalPlayer}, IsSharedModeMasterClient: {Runner.IsSharedModeMasterClient}");
        
        // 로컬 플레이어가 SharedModeMasterClient인지 즉시 확인
        if (Runner.IsSharedModeMasterClient)
        {
            Debug.Log($"[SharedModeMasterClientTracker] 🔴 LOCAL PLAYER IS SHARED MODE MASTER CLIENT!");
        }
    }

    public override void FixedUpdateNetwork()
    {
        // SharedModeMasterClient 상태 변경 감지 (로컬 플레이어 기준)
        bool isCurrentlyMasterClient = Runner.IsSharedModeMasterClient;
        
        if (_wasLocalPlayerMasterClient != isCurrentlyMasterClient)
        {
            Debug.Log($"[SharedModeMasterClientTracker] Local SharedModeMasterClient status changed: {_wasLocalPlayerMasterClient} → {isCurrentlyMasterClient}");
            _wasLocalPlayerMasterClient = isCurrentlyMasterClient;
            
            // UI 업데이트를 약간 지연시켜 네트워크 동기화 완료 후 실행
            Invoke(nameof(NotifyMasterClientChanged), 0.1f);
        }
    }

    private void NotifyMasterClientChanged()
    {
        Debug.Log("[SharedModeMasterClientTracker] Notifying UI of MasterClient change");
        
        // PlayerUIController 강제 업데이트
        PlayerUIController uiController = FindFirstObjectByType<PlayerUIController>();
        if (uiController != null)
        {
            uiController.ForceRefreshAllSlots();
        }
    }

    private void OnDestroy()
    {
        if (LocalInstance == this)
            LocalInstance = null;
    }

    /// <summary>
    /// 해당 플레이어가 SharedModeMasterClient인지 확인
    /// </summary>
    public static bool IsPlayerSharedModeMasterClient(PlayerRef player)
    {
        if (LocalInstance == null)
            return false;

        return LocalInstance.Object.StateAuthority == player;
    }

    /// <summary>
    /// 현재 SharedModeMasterClient의 PlayerRef 반환
    /// </summary>
    public static PlayerRef? GetSharedModeMasterClientPlayerRef()
    {
        if (LocalInstance == null)
            return null;

        return LocalInstance.Object.StateAuthority;
    }

    /// <summary>
    /// 로컬 플레이어가 SharedModeMasterClient인지 확인하는 편의 메서드
    /// </summary>
    public static bool IsLocalPlayerSharedModeMasterClient()
    {
        if (LocalInstance?.Runner == null)
            return false;

        return IsPlayerSharedModeMasterClient(LocalInstance.Runner.LocalPlayer);
    }
}
