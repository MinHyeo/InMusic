using UnityEngine;
using System.Collections.Generic;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private PlayerSlotUI[] playerSlots; // Player slots, always fixed to 2
    
    private void Start()
    {
        RefreshAllSlots();
    }
    
    private void Update()
    {
        // 플레이어 수가 변경되었을 때만 전체 슬롯 재구성
        if (HasPlayerListChanged())
        {
            RefreshAllSlots();
        }
    }
    
    private bool HasPlayerListChanged()
    {
        // MultiRoomManager를 통해 플레이어 수 확인
        int activePlayerCount = MultiRoomManager.Instance?.GetPlayerCount() ?? 0;
        int activeSlotsCount = 0;
        
        foreach (var slot in playerSlots)
        {
            if (!slot.IsAvailable) activeSlotsCount++;
        }
        
        return activePlayerCount != activeSlotsCount;
    }
    
    private void RefreshAllSlots()
    {
        // 모든 슬롯 초기화
        foreach (var slot in playerSlots)
        {
            slot.Unbind();
        }
        
        // PlayerId 순서로 정렬 (모든 클라이언트에서 동일한 순서 보장)
        var players = MultiRoomManager.Instance?.GetAllPlayers() ?? new List<PlayerStateController>();
        var sortedPlayers = new List<PlayerStateController>(players);
        sortedPlayers.Sort((a, b) => a.Object.InputAuthority.PlayerId.CompareTo(b.Object.InputAuthority.PlayerId));
        
        // 정렬된 순서대로 슬롯 0, 1에 배치
        for (int i = 0; i < sortedPlayers.Count && i < 2; i++)
        {
            playerSlots[i].Bind(sortedPlayers[i]);
        }
    }
    
    // 특정 플레이어의 슬롯만 업데이트 (PlayerStateController에서 호출)
    public void UpdatePlayerSlot(PlayerStateController player)
    {
        foreach (var slot in playerSlots)
        {
            if (slot.BoundPlayer == player)
            {
                slot.ChangeDisplay();
                break;
            }
        }
    }

    // SharedModeMasterClient 변경 시 강제로 모든 슬롯 새로고침
    public void ForceRefreshAllSlots()
    {
        Debug.Log("[PlayerUIController] Force refreshing all slots due to MasterClient change");
        RefreshAllSlots();
    }
}
