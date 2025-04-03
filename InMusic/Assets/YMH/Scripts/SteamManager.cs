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
            //Steam API 초기화
            try
            {
                if (!SteamAPI.Init())
                {
                    Debug.LogError("Steam API 초기화 실패!");
                    return;
                }
                Initialized = true; // Steam API 초기화 성공
                Debug.Log("Steam API 초기화 성공!");

                steamName = SteamFriends.GetPersonaName();
                Debug.Log("Steam 사용자 이름: " + steamName);

                steamId = SteamUser.GetSteamID().m_SteamID;
                Debug.Log($"Steam ID : {steamId}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Steam API 초기화 중 오류 발생: {e.Message}");
            }
        }

        private void OnApplicationQuit()
        {
            if (Initialized)
            {
                SteamAPI.Shutdown(); // Steam API 종료
                Initialized = false;
            }
        }
    }
}