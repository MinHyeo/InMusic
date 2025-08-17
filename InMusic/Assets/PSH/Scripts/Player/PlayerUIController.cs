using Fusion;
using System.Collections;
using UnityEngine;
using static Unity.Collections.Unicode;

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
        GameObject waitingRoomUIManager = GameObject.Find("Waiting_Room_UI");

        //waitingRoomUIManager.GetComponent<Waiting_Room_UI>().SetBMS();
        //Runner.LoadScene(SceneRef.FromIndex(5));
        //LoadingScreen.Instance.LoadScene("KGB_MultiPlay");

        if (waitingRoomUIManager != null)
        {
            // SetBMS가 끝난 뒤 씬 전환
            StartCoroutine(SetBMSAndLoadScene(waitingRoomUIManager.GetComponent<Waiting_Room_UI>()));
        }
    }

    private IEnumerator SetBMSAndLoadScene(Waiting_Room_UI ui)
    {
        ui.SetBMS(); // 데이터 세팅
        yield return null; // 혹은 SetBMS 내부 코루틴이면 끝날 때까지 yield

        PlayerRef firstPlayer = default;
        foreach (var player in Runner.ActivePlayers)
        {
            firstPlayer = player;
            break; // 첫 번째만
        }

        if (Runner.LocalPlayer == firstPlayer)
        {
            Runner.LoadScene(SceneRef.FromIndex(5));
        }
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
