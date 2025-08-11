using UnityEngine;
using System.Collections;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Linq;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkRunner runnerInstance;

    public string lobbyName = "default";

    public Transform sessionListContentParent;
    public GameObject sessionListEntryPrefab;
    public Dictionary<string, GameObject> sessionListUiDictionnary = new Dictionary<string, GameObject>();

    private string lobbyScene = "KGB_Multi_Lobby";
    private string waitngScene = "Waiting_Room_PSH";
    private string gamePlayScene = "KGB_MultiPlay";
    //public SceneAsset gameplaySceneAsset;
    //public SceneAsset lobbySceneAsset;
    public GameObject playerPrefab;
    public GameObject passwordPanel;

    public List<SessionListEntry> sessionListEntries = new List<SessionListEntry>();
    public event Action OnSessionListChanged;

    public static event Action<PlayerRef> OnPlayerLeftEvt;

    private HashSet<PlayerRef> loadedPlayers = new HashSet<PlayerRef>();

    private void Awake()
    {
        

        runnerInstance = gameObject.GetComponent<NetworkRunner>();

        if (runnerInstance == null)
        {
            runnerInstance = gameObject.AddComponent<NetworkRunner>();
        }
        DontDestroyOnLoad(gameObject);
        runnerInstance.AddCallbacks(this);

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
            Scene = SceneRef.FromIndex(GetSceneIndex(waitngScene)),
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
            Scene = SceneRef.FromIndex(GetSceneIndex(waitngScene)),
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
            Scene = SceneRef.FromIndex(GetSceneIndex(waitngScene)), //임시
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
        if(player == runnerInstance.LocalPlayer)
        {
            Debug.Log($"플레이어 입장: {player.PlayerId}");
            NetworkObject playerObject = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);
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
        sessionListEntries.Add(entryScript);
        if (passwordPanel != null)
        {
            entryScript.SetPasswordPanel(passwordPanel);
        }
        //entryScript.roomName.text = session.Name;
        //entryScript.playerCount.text = session.PlayerCount.ToString() +"/"+ session.MaxPlayers.ToString();
        //entryScript.joinButton.interactable = session.IsOpen;

        newEntry.SetActive(session.IsVisible);
        OnSessionListChanged?.Invoke();
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
        OnSessionListChanged?.Invoke();
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
        Vector2 direction = Vector2.zero;
        //Debug.Log("온 인풋");
        if (Input.GetKey(KeyCode.W)) direction.y += 1;
        if (Input.GetKey(KeyCode.S)) direction.y -= 1;
        if (Input.GetKey(KeyCode.A)) direction.x -= 1;
        if (Input.GetKey(KeyCode.D)) direction.x += 1;

        if (direction != Vector2.zero)
        {
            input.Set(new NetworkInputData
            {
                moveDirection = direction
            });
        }
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
        Debug.Log($"Netwrok: 플레이어 나감 ({player.PlayerId})");
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

        Debug.Log($"씬 로딩 끝: {runner.LocalPlayer}");
        // 여기서 호스트에 "나 로딩 끝남"을 알림
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log($"씬 로딩 시작: {runner.LocalPlayer}");
    }








    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }

    public List<SessionListEntry> GetSessionEntries()
    {
        return sessionListEntries;
    }

}
