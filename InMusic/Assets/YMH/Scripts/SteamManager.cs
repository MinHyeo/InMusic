using Steamworks;
using UnityEngine;

namespace YMH 
{
    public class SteamManager : SingleTon<SteamManager>
    {
        public bool Initialized { get; private set; } = false;

        public string steamName;
        public ulong steamId;

        protected override void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            //Steam API �ʱ�ȭ
            try
            {
                if (!SteamAPI.Init())
                {
                    Debug.LogError("Steam API �ʱ�ȭ ����!");
                    return;
                }
                Initialized = true; // Steam API �ʱ�ȭ ����
                Debug.Log("Steam API �ʱ�ȭ ����!");

                steamName = SteamFriends.GetPersonaName();
                Debug.Log("Steam ����� �̸�: " + steamName);

                steamId = SteamUser.GetSteamID().m_SteamID;
                Debug.Log($"Steam ID : {steamId}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Steam API �ʱ�ȭ �� ���� �߻�: {e.Message}");
            }
        }

        private void OnApplicationQuit()
        {
            if (Initialized)
            {
                SteamAPI.Shutdown(); // Steam API ����
                Initialized = false;
            }
        }
    }
}