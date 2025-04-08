using Steamworks;
using System;
using UnityEngine;

public class SteamWebProto_UI : MonoBehaviour
{
    public string userID;
    public string userName;
    void Start()
    {
        //�������� ���� ���� ��������
        GetSteamUserINFO();
        //������ ����Ƽ ���� ���ҽ��� ����ȭ
        GameManager_PSH.Resource.CheckMusic();
        //�α���
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
    }

    public void LoginToWeb()
    {
        GameManager_PSH.Web.UserLogin(userID, userName);
    }

    //ȸ������ �׽�Ʈ�� �޼���
    public void SignUpToWeb()
    {
        //GameManager_PSH.Web.UserSignUp(userID, userName);
    }
}
