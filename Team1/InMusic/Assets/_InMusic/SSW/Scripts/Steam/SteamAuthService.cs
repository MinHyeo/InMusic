using UnityEngine;
using Steamworks;
using System;
using SSW.DB;

namespace SSW.Steam
{
    public class SteamAuthService : MonoBehaviour
    {
        Callback<GetTicketForWebApiResponse_t> m_AuthTicketForWebApiResponseCallback;
        string m_SessionTicket;
        string identity = "unityauthenticationservice";

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if(!SteamManager.Initialized)
            {
                Debug.LogError("SteamManager not initialized. SteamAuthService will not work.");
                return;
            }
            SignInWithSteam();
        }

        void SignInWithSteam()
        {
            // GC로부터 보호하기 위해 콜백을 멤버 변수에 할당
            m_AuthTicketForWebApiResponseCallback = Callback<GetTicketForWebApiResponse_t>.Create(OnAuthCallback);

            // Steam에 웹 API용 티켓 발급 요청 (identity는 Unity Authentication 등에서 사용)
            SteamUser.GetAuthTicketForWebApi(identity);
        }

        void OnAuthCallback(GetTicketForWebApiResponse_t callback)
        {
            m_SessionTicket = BitConverter.ToString(callback.m_rgubTicket).Replace("-", string.Empty);

            // 콜백 사용 끝
            m_AuthTicketForWebApiResponseCallback.Dispose();
            m_AuthTicketForWebApiResponseCallback = null;

            Debug.Log("[SteamAuthService] Login success. Ticket: " + m_SessionTicket);

            // Steam ID (64비트)
            string steamId = SteamUser.GetSteamID().m_SteamID.ToString();
            Debug.Log("[SteamAuthService] Steam ID: " + steamId);

            // 스팀 닉네임 (원하면 UserName으로 DB에 저장 가능)
            string userName = SteamFriends.GetPersonaName();
            Debug.Log("[SteamAuthService] Steam Nickname: " + userName);

            // DBService 호출: 유저 등록/로그인 처리
            DBService db = FindFirstObjectByType<DBService>();
            if (db != null)
            {
                // userName에는 스팀 닉네임을 전달
                db.SaveOrLoadUser(steamId, userName);
            }
            else
            {
                Debug.LogWarning("DBService not found in scene. Skipping DB step.");
            }
        }
    }
}

