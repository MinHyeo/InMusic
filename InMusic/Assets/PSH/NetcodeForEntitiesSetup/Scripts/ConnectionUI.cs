using System;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem.UI;
#endif
namespace Unity.Multiplayer.Center.NetcodeForEntitiesSetup
{
    /// <summary>
    /// A basic example of a UI to start a host or client.
    /// If you want to modify this Script please copy it into your own project and add it to your copied UI Prefab.
    /// </summary>
    public class ConnectionUI : MonoBehaviour
    {
        /// <summary>
        /// The network port to use.
        /// </summary>
        public ushort NetworkPort = 7979;

        /// <summary>
        /// The Ipv4 address to use.
        /// </summary>
        public string Address = "127.0.0.1";

        /// <summary>
        /// The name of the scene to load (without extension).
        /// </summary>
        public string SceneToLoad;
        
        public Text ConnectionLabel;
        
        public Button StartHostButton;
        
        public Button StartClientButton;

        /// <summary>
        /// Stores the old name of the local world (create by initial bootstrap).
        /// It is reused later when the local world is created when coming back from game to the menu.
        /// </summary>
        internal static string OldFrontendWorldName = string.Empty;

        /// <summary>
        /// Gets or sets the Connection status text in the UI.
        /// </summary>
        public string ConnectionStatus
        {
            get => ConnectionLabel.text;
            set => ConnectionLabel.text = value;
        }

        /// <summary>
        /// Called before a connection is established.
        /// </summary>
        public void OnBeforeConnect()
        {
            // Prevent disconnection on focus loss.
            Application.runInBackground = true;
        }
        
        /// <summary>
        /// Called when the connection is established.
        /// </summary>
        public void OnConnected()
        {
            //Destroy the local simulation world to avoid the game scene to be loaded into it
            //This prevent rendering (rendering from multiple world with presentation is not greatly supported)
            //and other issues.
            DestroyLocalSimulationWorld();
            
            // Prevent loading the scene twice. This can happen when several widgets call OnConnect when a Session is created.
            var scene = SceneManager.GetSceneByName(SceneToLoad);
            if (scene.IsValid())
                return;
            
            SceneManager.LoadSceneAsync(SceneToLoad, LoadSceneMode.Additive);
        }

        void Awake()
        {   
            StartHostButton.onClick.AddListener(StartClientServer);
            StartClientButton.onClick.AddListener(StartClient);
            
            if (!FindAnyObjectByType<EventSystem>())
            {
                var inputType = typeof(StandaloneInputModule);
#if ENABLE_INPUT_SYSTEM
                inputType = typeof(InputSystemUIInputModule);                
#endif
                var eventSystem = new GameObject("EventSystem", typeof(EventSystem), inputType);
                eventSystem.transform.SetParent(transform);
            }
        }

        void AddConnectionUISystemToUpdateList()
        {
            foreach (var world in World.All)
            {
                if (world.IsClient() && !world.IsThinClient())
                {
                    var sys = world.GetOrCreateSystemManaged<ConnectionUISystem>();
                    sys.UIBehaviour = this;
                    var simGroup = world.GetExistingSystemManaged<SimulationSystemGroup>();
                    simGroup.AddSystemToUpdateList(sys);
                }
            }
        }
        
        void StartClientServer()
        {
            if (ClientServerBootstrap.RequestedPlayType != ClientServerBootstrap.PlayType.ClientAndServer)
            {
                Debug.LogError($"Creating client/server worlds is not allowed if playmode is set to {ClientServerBootstrap.RequestedPlayType}");
                return;
            }

            OnBeforeConnect();
            DisableButtons();
           
            var server = ClientServerBootstrap.CreateServerWorld("ServerWorld");
            var client = ClientServerBootstrap.CreateClientWorld("ClientWorld");
            
            if (World.DefaultGameObjectInjectionWorld == null)
                World.DefaultGameObjectInjectionWorld = server;

            OnConnected();
            
            NetworkEndpoint ep = NetworkEndpoint.AnyIpv4.WithPort(NetworkPort);
            {
                using var drvQuery = server.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
                drvQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(ep);
            }

            ep = NetworkEndpoint.LoopbackIpv4.WithPort(NetworkPort);
            {
                using var drvQuery = client.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
                drvQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(client.EntityManager, ep);
            }
            AddConnectionUISystemToUpdateList();
        }
        
        void StartClient()
        {
            OnBeforeConnect();
            DisableButtons(); 
            var client = ClientServerBootstrap.CreateClientWorld("ClientWorld");
            
            if (World.DefaultGameObjectInjectionWorld == null)
                World.DefaultGameObjectInjectionWorld = client;
            
            OnConnected();

            var ep = NetworkEndpoint.Parse(Address, NetworkPort);
            {
                using var drvQuery = client.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
                drvQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(client.EntityManager, ep);
            }
            AddConnectionUISystemToUpdateList();
        }

        static void DestroyLocalSimulationWorld()
        {
            foreach (var world in World.All)
            {
                if (world.Flags == WorldFlags.Game)
                {
                    OldFrontendWorldName = world.Name;
                    world.Dispose();
                    break;
                }
            }
        }

        void DisableButtons()
        {
            StartHostButton.interactable = false;
            StartClientButton.interactable = false;
        }
    }
    
    /// <summary>
    /// System making the link between the UI and the network connection status.
    /// </summary>
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [DisableAutoCreation]
    public partial class ConnectionUISystem : SystemBase
    {
        /// <summary>
        /// The UI behaviour to update.
        /// </summary>
        public ConnectionUI UIBehaviour;
        string m_PingText;

        /// <summary>
        /// Updates the UI with the connection status.
        /// </summary>
        protected override void OnUpdate()
        {
            CompleteDependency();
            if (!SystemAPI.TryGetSingletonEntity<NetworkStreamConnection>(out var connectionEntity))
            {
                UIBehaviour.ConnectionStatus = "Not connected!";
                m_PingText = default;
                return;
            }

            var connection = EntityManager.GetComponentData<NetworkStreamConnection>(connectionEntity);
            var address = SystemAPI.GetSingletonRW<NetworkStreamDriver>().ValueRO.GetRemoteEndPoint(connection).Address;
            if (EntityManager.HasComponent<NetworkId>(connectionEntity))
            {
                if (string.IsNullOrEmpty(m_PingText) || UnityEngine.Time.frameCount % 30 == 0)
                {
                    var networkSnapshotAck = EntityManager.GetComponentData<NetworkSnapshotAck>(connectionEntity);
                    m_PingText = networkSnapshotAck.EstimatedRTT > 0 ? $"{(int) networkSnapshotAck.EstimatedRTT}ms" : "Connected";
                }

                UIBehaviour.ConnectionStatus = $"{address} | {m_PingText}";
            }
            else
            {
                UIBehaviour.ConnectionStatus = $"{address} | Connecting";
            }
        }
    }
}
