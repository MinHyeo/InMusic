using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using SongList;

namespace SSW.DB
{
    [System.Serializable]
    public class ServerResponse
    {
        public bool success;
        public bool newUser;
        public string message;
    }
    public class DBService : Singleton<DBService>
    {
        [Header("PHP Server URLs")]
        [SerializeField] private string _saveOrLoadUserURL = "http://localhost/InMusic/handleSteamLogin.php";

        public void SaveOrLoadUser(string steamId, string nickname)
        {
            StartCoroutine(SendUserData(steamId, nickname));
        }

        private IEnumerator SendUserData(string userId, string userName)
        {
            WWWForm form = new WWWForm();
            form.AddField("userId", userId);
            form.AddField("userName", userName);

            using (UnityWebRequest www = UnityWebRequest.Post(_saveOrLoadUserURL, form))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string response = www.downloadHandler.text;
                    Debug.Log("[DBService] Response: " + response);

                    var result = JsonUtility.FromJson<ServerResponse>(response);
                    Debug.Log($"Success: {result.success}, newUser: {result.newUser}, Msg: {result.message}");
                }
                else
                {
                    Debug.LogError("[DBService] Server Error: " + www.error);
                }
            }
        }
    }
}