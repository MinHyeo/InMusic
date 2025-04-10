using Steamworks;
using System;
using UnityEngine;

public class SteamWebProto_UI : MonoBehaviour
{
    public string userID;
    public string userName;
    void Start()
    {
        //스팀에서 유저 정보 가져오기
        GetSteamUserINFO();
        //서버와 유니티 음악 리소스랑 동기화
        GameManager_PSH.Resource.CheckMusic();
        //로그인
        LoginToWeb();
    }

    void GetSteamUserINFO()
    {
        if (SteamManager.Initialized)
        {
            userID = Convert.ToString(SteamUser.GetSteamID().m_SteamID);
            userName = SteamFriends.GetPersonaName();
        }
        else
        {
            Debug.LogError("Steam is not initialized.");
            return;
        }
        
        GameManager_PSH.Data.SetPlayerData(userID, userName);
    }

    public void LoginToWeb()
    {
        GameManager_PSH.Web.UserLogin(userID, userName);
    }

    //회원가입 테스트용 메서드
    public void SignUpToWeb()
    {
        //GameManager_PSH.Web.UserSignUp(userID, userName);
    }
}
