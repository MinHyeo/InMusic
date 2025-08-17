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
        LoadingScreen.Instance.LoadScene("KGB_MultiPlay");
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_GameSet(RpcInfo info = default)
    {
        if (info.Source == NetworkManager.runnerInstance.LocalPlayer)
        {
            return;
        }
        GameObject waitingRoomUIManager = GameObject.Find("Waiting_Room_UI");
        waitingRoomUIManager.GetComponent<Waiting_Room_UI>().ButtonEvent("Enter");
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
