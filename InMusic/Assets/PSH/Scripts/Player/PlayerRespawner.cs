using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public GameObject playerPrefab;

    [SerializeField] NetworkObject P1;
    [SerializeField]  bool IsP1Loaded = false;
    [SerializeField]  NetworkObject P2;
    [SerializeField] bool IsP2Loaded = false;

    private void Awake()
    {
        NetworkRunner runner = NetworkManager.runnerInstance; // 싱글톤 접근
        if (runner != null)
        {
            runner.AddCallbacks(this);
        }
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        if (SceneManager.GetActiveScene().name == "KGB_MultiPlay" || SceneManager.GetActiveScene().name == "MultiPlay_Result")
        {
            Debug.Log("씬 로딩 완료!");

            if (runner.GetPlayerObject(runner.LocalPlayer) == null)
            {
                NetworkObject playerObject = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, runner.LocalPlayer);
                runner.SetPlayerObject(runner.LocalPlayer, playerObject);
            }
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        throw new NotImplementedException();
    }

    public void CheckPlayerLoad()
    {
        Debug.Log("플레이어 프리팹 로드 상태 확인");
        foreach (var playerRef in NetworkManager.runnerInstance.ActivePlayers)
        {
            NetworkObject pObject = NetworkManager.runnerInstance.GetPlayerObject(playerRef);
            PlayerInfo pInfo = pObject.GetComponent<PlayerInfo>();
            if (pInfo.PlayerRole == PlayerInfo.PlayerType.Host)
            {
                IsP1Loaded = true;
                Debug.Log("P1 로드 완료");
            }
            //P2기준 본인 UI 조작
            else
            {
                IsP2Loaded = true;
                Debug.Log("P2 로드 완료");
            }
        }
        if (IsP1Loaded && IsP2Loaded) {
            Debug.Log("모든 플레이어 로드 완료");
            LoadingScreen.Instance.SceneReady();
        }
    }
}
