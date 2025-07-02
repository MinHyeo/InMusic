using NUnit.Framework.Constraints;
using Steamworks;
using System;
using UnityEngine;

public class SteamConnect : MonoBehaviour
{
    public GameObject messageBox;
    public string userID;
    public string userName;
    void Start()
    {
        InitializeGame();
    }

    //스팀 정보 -> 리소스 동기화 -> 로그인 로직 함수
    public void InitializeGame()
    {
        //서버와 유니티 음악 리소스랑 동기화
        GameManager_PSH.Resource.CheckMusic();

        if (GameManager_PSH.SteamCheck)
        {
            Debug.Log("스팀 연동 실패");
            return;
        }

        //스팀에서 유저 정보 가져오기
        if (GetSteamUserInfo()) {

            //로그인
            LoginToWeb();

            messageBox.SetActive(false);
        }
    }

    bool GetSteamUserInfo()
    {
        if (SteamManager.Initialized)
        {
            GameManager_PSH.SteamCheck = true;
            userID = Convert.ToString(SteamUser.GetSteamID().m_SteamID);
            userName = SteamFriends.GetPersonaName();
        }
        else
        {
            Debug.LogError("스팀 초기화 실패.");
            messageBox.SetActive(true);
            return false;
        }

        GameManager_PSH.Data.SetPlayerData(userID, userName);
        return true;
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

    public void PlayWithdoutSteam()
    {
        GameManager_PSH.SteamCheck = true;
        GameManager_PSH.Resource.CheckMusic();
        Destroy(gameObject);
    }
}
