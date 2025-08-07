using Fusion;
using UnityEngine;

public class PlayerUIController : NetworkBehaviour
{
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void Rpc_ScrollDown()
    {
        GameObject waitingRoomUIManager = GameObject.Find("Waiting_Room_UI");
        waitingRoomUIManager.GetComponent<Waiting_Room_UI>().ButtonEvent("Down");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void Rpc_ScrollUp()
    {
        GameObject waitingRoomUIManager = GameObject.Find("Waiting_Room_UI");
        waitingRoomUIManager.GetComponent<Waiting_Room_UI>().ButtonEvent("Up");
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void Rpc_GameStart()
    {
        //TODO: 게임 플레이 씬으로 이동
    }

    public void BroadScrollUp() {
        Rpc_ScrollUp();
    }

    public void BroadScrollDown()
    {
        Rpc_ScrollDown();
    }
    public void BroadGameStart()
    {
        Rpc_GameStart();
    }
}
