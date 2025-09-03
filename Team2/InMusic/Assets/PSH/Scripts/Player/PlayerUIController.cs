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

    [Rpc(RpcSources.InputAuthority, RpcTargets.InputAuthority)]
    public void Rpc_GameStart()
    {
        //LoadingScreen.Instance.LoadScene("KGB_MultiPlay");    //로딩씬: 모든 클라이언트가 별도로 실행해야 됨
        Runner.LoadScene(SceneRef.FromIndex(5));                      //Runner.LoadScene:  모든 클라이언트가 실행 되므로, 한 명만 호출하도록 주의
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.InputAuthority)]
    public void Rpc_GameEnd()
    {
        Runner.LoadScene(SceneRef.FromIndex(6));    
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

    public void BroadGameEnd()
    {
        Rpc_GameEnd();
    }
}
