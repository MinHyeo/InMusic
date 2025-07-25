using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class MultiRoomTest : NetworkBehaviour
{
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_StartGame()
    {
        Debug.Log("RPC_StartGame called");
        // Here you can add logic to start the game, such as loading a scene or initializing game state.
        Dictionary<string, SessionProperty> newProps = new()
        {
            { "songName", "Heya" }
        };

        NetworkManager.runnerInstance.SessionInfo.UpdateCustomProperties(newProps);
        NetworkManager.runnerInstance.LoadScene("MultiPlay");
    }

    private void HandleSceneLoad(SceneRef sceneRef)
    {
        Debug.Log("Scene loaded: " + sceneRef);
        // Additional logic after the scene has been loaded can be added here.
    }
}
