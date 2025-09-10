using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Play;

public class NetworkManager : Singleton<NetworkManager>, INetworkRunnerCallbacks
{
    public static NetworkRunner runnerInstance;
    public static bool _isSessionLobbyReady = false; // 세션 로비 연결 상태 추적

    public string lobbyName = "default";

    public Transform sessionListContentParent;
    public GameObject sessionListEntryPrefab;
    public Dictionary<string, GameObject> sessionListUIDictionary = new Dictionary<string, GameObject>();

    //public Scene gameplayScene;
    public GameObject playerPrefab;

    // Event Handlers
    public static event Action OnGamePlayLoadingStart;
    public static event Action OnGamePlayLoadingCompleted;

    protected override void Awake()
    {
        base.Awake();

        if (runnerInstance != null && runnerInstance.IsRunning)
        {
            Debug.LogWarning("[Fusion] Found existing runner. Forcing shutdown.");
            runnerInstance.Shutdown();
            runnerInstance = null;
        }

        // runnerInstance 초기화
        runnerInstance = gameObject.GetComponent<NetworkRunner>();
        if (runnerInstance == null)
        {
            runnerInstance = gameObject.AddComponent<NetworkRunner>();
        }
    }

    private void Start()
    {
        runnerInstance.JoinSessionLobby(SessionLobby.Shared, lobbyName);
    }

    public async Task CreateRoom(string roomName, string password, bool isPassword)
    {
        var sessionProps = new Dictionary<string, SessionProperty>();
        SessionProperty isLockedProp = isPassword;

        if (isPassword)
        {
            string hashedPassword = HashUtils.GetSha256(password);
            SessionProperty passwordProp = hashedPassword;

            sessionProps.Add("isLocked", isLockedProp);   // 방 잠김 여부
            sessionProps.Add("password", passwordProp);   // 실제 비밀번호
        }
        else
        {
            sessionProps.Add("isLocked", isLockedProp);
        }
        sessionProps.Add("maxPlayers", 2); // 방 이름

        int index = SceneUtility.GetBuildIndexByScenePath("Assets/_InMusic/Scenes/MultiRoomScene_InMusic.unity");
        Debug.Log(index);

        await runnerInstance.JoinSessionLobby(SessionLobby.Shared);

        await runnerInstance.StartGame(new StartGameArgs()
        {
            Scene = SceneRef.FromIndex(index),
            SessionName = roomName,
            GameMode = GameMode.Shared,  // Photon Cloud Server 방식으로 변경
            SessionProperties = sessionProps,
        });
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("[Fusion] Connected to server.");
        //throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        //throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        //throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"[Fusion] Disconnected from server: {reason}");
        //throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        //throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        //throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        //throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        //throw new NotImplementedException();
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        //throw new NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Fusion] Player Joined: {player}, PlayerId: {player.PlayerId}, LocalPlayer = {runner.LocalPlayer}");
        if (player == runnerInstance.LocalPlayer)
        {
            Debug.Log("[Fusion] --> This is ME. Spawning my PlayerObject.");
            NetworkObject playerObject = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);
            runner.SetPlayerObject(player, playerObject);
            runner.MoveToRunnerScene(playerObject.gameObject);
        }
        else
        {
            Debug.Log("[Fusion] --> This is NOT me. Waiting for the player to spawn their PlayerObject.");
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[Fusion] Player Left: {player}");

        // MultiRoomManager에 플레이어 퇴장 알림
        if (MultiRoomManager.Instance != null)
        {
            MultiRoomManager.Instance.NotifyPlayerLeft(player);
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "MultiPlay_InMusic")
        {
            if (MatchController.Instance != null)
            {
                Debug.Log("[NetworkManager] Notifying MatchController that a player has left.");
                MatchController.Instance.OnPlayerLeft();
            }
        }
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        //throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        //throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Scene loadedScene = SceneManager.GetActiveScene();
        if (loadedScene.name == "MultiRoomScene_InMusic")
        {
            if (runner.IsSharedModeMasterClient)
            {
                if (!runner.SessionInfo.IsOpen)
                {
                    runner.SessionInfo.IsOpen = true;
                    Debug.Log("[NetworkManager] Session is now OPEN again on scene load.");
                }
            }
        }
        OnGamePlayLoadingCompleted?.Invoke();
        //throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        OnGamePlayLoadingStart?.Invoke();
        //throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        // 세션 로비에 연결되었음을 표시
        if (!_isSessionLobbyReady)
        {
            _isSessionLobbyReady = true;
            Debug.Log("[NetworkManager] Session lobby connected! Ready to create rooms.");
        }

        // 세션 목록 업데이트
        DeleteOldSessionsFromUI(sessionList);
        
        Debug.Log("Session List Count: " + sessionList.Count);
        CompareLists(sessionList);
    }

    //
    private void DeleteOldSessionsFromUI(List<SessionInfo> sessionList)
    {
        bool isContained = false;
        GameObject uiToDelete = null;

        foreach (KeyValuePair<string, GameObject> kvp in sessionListUIDictionary)
        {
            string sessionKey = kvp.Key;

            foreach (SessionInfo sessionInfo in sessionList)
            {
                if (sessionInfo.Name == sessionKey)
                {
                    isContained = true;
                    break;
                }
            }

            if (!isContained)
            {
                uiToDelete = kvp.Value;
                sessionListUIDictionary.Remove(sessionKey);
                Destroy(uiToDelete);
            }
        }
    }

    private void CompareLists(List<SessionInfo> sessionList)
    {
        foreach (SessionInfo session in sessionList)
        {
            if (sessionListUIDictionary.ContainsKey(session.Name))
            {
                UpdateEntryUI(session);
            }
            else
            {
                CreateEntryUI(session);
            }
        }
    }

    private void UpdateEntryUI(SessionInfo session)
    {
        sessionListUIDictionary.TryGetValue(session.Name, out GameObject newEntry);
        SessionListEntry entryScript = newEntry.GetComponent<SessionListEntry>();

        entryScript.UpdateRoom(session);

        newEntry.SetActive(session.IsVisible);
    }

    private void CreateEntryUI(SessionInfo session)
    {
        GameObject newEntry = GameObject.Instantiate(sessionListEntryPrefab);
        
        // UI 크기 문제 해결: RectTransform 설정
        RectTransform rectTransform = newEntry.GetComponent<RectTransform>();
        newEntry.transform.SetParent(sessionListContentParent, false); // worldPositionStays = false로 설정
        
        if (rectTransform != null)
        {
            // 원본 크기 유지
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition = Vector2.zero;
        }
        
        SessionListEntry entryScript = newEntry.GetComponent<SessionListEntry>();
        sessionListUIDictionary.Add(session.Name, newEntry);
        entryScript.CreateRoom(session);

        newEntry.SetActive(session.IsVisible);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        //throw new NotImplementedException();

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        //throw new NotImplementedException();
    }

    private void OnApplicationQuit()
    {
        if (runnerInstance != null && runnerInstance.IsRunning)
        {
            Debug.Log("[Fusion] Shutting down runner on quit.");
            runnerInstance.Shutdown();
            runnerInstance = null;
        }
    }
    
    private void OnDestroy()
    {
        if (runnerInstance != null && runnerInstance.IsRunning)
        {
            Debug.Log("[Fusion] Shutting down runner in OnDestroy.");
            runnerInstance.Shutdown();
            runnerInstance = null;
        }
    }

}
