using UnityEngine;
using System.Collections;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkRunner runnerInstance;

    public string lobbyName = "default";

    public Transform sessionListContentParent;
    public GameObject sessionListEntryPrefab;
    public Dictionary<string, GameObject> sessionListUiDictionnary = new Dictionary<string, GameObject>();

    private string lobbyScene = "KGB_Multi_Lobby";
    private string gameplayScene = "SampleScene";
    private string waitingRoomScene = "Waiting_Room_PSH";
    //public SceneAsset gameplaySceneAsset;
    //public SceneAsset lobbySceneAsset;
    public GameObject playerPrefab;
    public GameObject passwordPanel;

    public static event Action<PlayerRef> OnPlayerLeftEvt;
    private void Awake()
    {
        

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

    public static void  ReturnToLobby()
    {
        NetworkManager.runnerInstance.Despawn(runnerInstance.GetPlayerObject(runnerInstance.LocalPlayer));
        NetworkManager.runnerInstance.Shutdown(true, ShutdownReason.Ok);
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        SceneManager.LoadScene(lobbyScene);
    }

    public void CreateRandomSession()
    {
        int randomInt = UnityEngine.Random.Range(1000, 9999);
        string randomSessionName = "Room-"+randomInt.ToString();

        runnerInstance.StartGame(new StartGameArgs()
        {
            Scene = SceneRef.FromIndex(GetSceneIndex(waitingRoomScene)),
            SessionName = randomSessionName,
            GameMode = GameMode.Shared,
        });

        GameManager_PSH.PlayerRole = true;
    } 

    public void CreateSession(string roomName)
    {
        string newSessionName = roomName;
        runnerInstance.StartGame(new StartGameArgs()
        {
            Scene = SceneRef.FromIndex(GetSceneIndex(waitingRoomScene)),
            SessionName = newSessionName,
            GameMode = GameMode.Shared,
        });

        GameManager_PSH.PlayerRole = true;

    }
    public void CreateSession(string roomName, string password)
    {
        string newSessionName = roomName;
        runnerInstance.StartGame(new StartGameArgs()
        {
            Scene = SceneRef.FromIndex(GetSceneIndex(waitingRoomScene)), //임시
            SessionName = newSessionName,
            GameMode = GameMode.Shared,
            SessionProperties = new Dictionary<string, SessionProperty>
            {
                {"pw", password}
            }
        });

        GameManager_PSH.PlayerRole = true;
    }

    public int GetSceneIndex(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if(name == sceneName)
            {
                return i;
            }
        }
        return -1;
    }


    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"플레이어 참가: {player.PlayerId}");
        if(player == runnerInstance.LocalPlayer)
        {
            NetworkObject playerObject = runner.Spawn(playerPrefab, Vector3.zero);
            runner.SetPlayerObject(player, playerObject);
        }
    }


    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

        DeleteOldSessionsFromUI(sessionList);

        CompareLists(sessionList);

        
    }

    private void CompareLists(List<SessionInfo> sessionList)
    {
        foreach (SessionInfo session in sessionList)
        {
            if (sessionListUiDictionnary.ContainsKey(session.Name))
            {
                UpdateEntryUI(session);
            }
            else
            {
                CreateEntryUI(session);
            }
        }
    }

    private void CreateEntryUI(SessionInfo session)
    {
        GameObject newEntry = GameObject.Instantiate(sessionListEntryPrefab);
        newEntry.transform.parent = sessionListContentParent;

        SessionListEntry entryScript = newEntry.GetComponentInChildren<SessionListEntry>();
        entryScript.Initialize(session);
        sessionListUiDictionnary.Add(session.Name, newEntry);
        if (passwordPanel != null)
        {
            entryScript.SetPasswordPanel(passwordPanel);
        }
        //entryScript.roomName.text = session.Name;
        //entryScript.playerCount.text = session.PlayerCount.ToString() +"/"+ session.MaxPlayers.ToString();
        //entryScript.joinButton.interactable = session.IsOpen;

        newEntry.SetActive(session.IsVisible);
    }

    private void UpdateEntryUI(SessionInfo session)
    {
        sessionListUiDictionnary.TryGetValue(session.Name, out GameObject newEntry);

        SessionListEntry entryScript = newEntry.GetComponent<SessionListEntry>();

        entryScript.roomName.text = session.Name;
        entryScript.playerCount.text = session.PlayerCount.ToString() + "/" + session.MaxPlayers.ToString();
        entryScript.joinButton.interactable = session.IsOpen;

        newEntry.SetActive(session.IsVisible);
    }



    private void DeleteOldSessionsFromUI(List<SessionInfo> sessionList)
    {
        bool isContained = false;
        GameObject uiToDelete = null;

        foreach (KeyValuePair<string, GameObject> kvp in sessionListUiDictionnary)
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

            if (isContained)
            {
                uiToDelete = kvp.Value;
                sessionListUiDictionnary.Remove(sessionKey);
                Destroy(uiToDelete);
            }
        }
    }


    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }



    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"플레이어 나감: {player.PlayerId}");
        // Waiting_Room_UI에 해당 플레이어가 나갔음을 알립니다.
        OnPlayerLeftEvt?.Invoke(player);
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }




    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }
}
