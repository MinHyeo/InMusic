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

    //���� ���� -> ���ҽ� ����ȭ -> �α��� ���� �Լ�
    public void InitializeGame()
    {
        //������ ����Ƽ ���� ���ҽ��� ����ȭ
        GameManager_PSH.Resource.CheckMusic();

        if (GameManager_PSH.SteamCheck)
        {
            Debug.Log("���� ���� ����");
            return;
        }

        //�������� ���� ���� ��������
        if (GetSteamUserInfo()) {

            //�α���
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
            Debug.LogError("���� �ʱ�ȭ ����.");
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


    //ȸ������ �׽�Ʈ�� �޼���
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
