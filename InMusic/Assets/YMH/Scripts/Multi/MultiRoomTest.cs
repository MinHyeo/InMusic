using Fusion;
using UnityEngine;

public class MultiRoomTest : MonoBehaviour
{
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StartGame()
    {
        Debug.Log("RPC_StartGame called");
        // Here you can add logic to start the game, such as loading a scene or initializing game state.

        NetworkManager.runnerInstance.LoadScene("MultiPlay");
    }
}
