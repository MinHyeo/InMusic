using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private PlayerSlotUI[] playerSlots;

    private void Update()
    {
        foreach (var player in PlayerStateController.AllPlayers.OrderBy(p => p.Object.InputAuthority.PlayerId))
        {
            PlayerSlotUI slot = FindSlotForPlayer(player);

            if (slot == null)
            {
                slot = FindAvailableSlot();
                if (slot != null)
                {
                    slot.Bind(player);
                }
            }
            else
            {
                slot.UpdateDisplay();
            }
        }

        // 4. 나간 플레이어는 슬롯에서 제거
        CleanupSlots();
    }

    private PlayerSlotUI FindSlotForPlayer(PlayerStateController player)
    {
        foreach (var slot in playerSlots)
        {
            if (slot.BoundPlayer == player)
                return slot;
        }
        return null;
    }

    private PlayerSlotUI FindAvailableSlot()
    {
        foreach (var slot in playerSlots)
        {
            if (slot.IsAvailable)
                return slot;
        }
        return null;
    }

    private void CleanupSlots()
    {
        foreach (var slot in playerSlots)
        {
            if (slot.BoundPlayer != null && !PlayerStateController.AllPlayers.Contains(slot.BoundPlayer))
                slot.Unbind();
        }
    }
}
