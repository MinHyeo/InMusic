using Steamworks;
using System;
using UnityEngine;

public class SteamWebProto_UI : MonoBehaviour
{
    public string userID;
    public string userName;
    void Start()
    {
        GetSteamUserINFO();
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

        LoginToWeb();
    }

    public void LoginToWeb()
    {
        GameManager_PSH.Web.UserLogin(userID, userName);
    }

    public void SignUpToWeb()
    {
        GameManager_PSH.Web.UserSignUp(userID, userName);
    }
}
