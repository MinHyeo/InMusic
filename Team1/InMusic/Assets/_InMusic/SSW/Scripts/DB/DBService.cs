using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using SongList;
using Play;

namespace SSW.DB
{
    [System.Serializable]
    public class ServerResponse
    {
        public bool success;
        public bool newUser;
        public string message;
    }
    public class DBService : Managers.Singleton<DBService>
    {
        [Header("PHP Server URLs")]
        [SerializeField] private string _saveOrLoadUserURL = "http://localhost/InMusic/handleSteamLogin.php";
        [SerializeField] private string _updateMusicListURL = "http://localhost/InMusic/updateMusicList.php";
        [SerializeField] private string _handleGetAllSongsURL = "http://localhost/InMusic/handleGetAllSongs.php";
        [SerializeField] private string _getMusicLogURL = "http://localhost/InMusic/handleGetMusicLog.php";
        [SerializeField] private string _saveMusicLogURL = "http://localhost/InMusic/handleSaveMusicLog.php";
        #region User Data
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
        #endregion

        #region Music List Data

        /// <summary>
        /// Updates the music list on the server.
        /// </summary>
        public void SaveSongToDB(string musicId, string musicName, string musicArtist)
        {
            StartCoroutine(SendSongData(musicId, musicName, musicArtist));
        }
        

        private IEnumerator SendSongData(string musicId, string musicName, string musicArtist)
        {
            WWWForm form = new WWWForm();
            form.AddField("musicId", musicId);
            form.AddField("musicName", musicName);
            form.AddField("musicArtist", musicArtist);

            using (UnityWebRequest www = UnityWebRequest.Post(_updateMusicListURL, form))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string response = www.downloadHandler.text;
                    Debug.Log("[DBService] Response: " + response);

                    var result = JsonUtility.FromJson<ServerResponse>(response);
                    Debug.Log($"Success: {result.success}, Msg: {result.message}");
                }
                else
                {
                    Debug.LogError("[DBService] Server Error: " + www.error);
                }
            }
        }

        /// <summary>
        /// Gets the list of all songs from the server.
        /// </summary>
        public void LoadAllSongsFromDB(Action<List<MusicData>> onLoaded)
        {
            StartCoroutine(GetAllSongsCoroutine(onLoaded));
        }

        private IEnumerator GetAllSongsCoroutine(Action<List<MusicData>> onLoaded)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(_handleGetAllSongsURL))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string response = www.downloadHandler.text;
                    Debug.Log("[DBService] GetAllSongs response: " + response);

                    var result = JsonUtility.FromJson<GetAllSongsResponse>(response);
                    if (result != null && result.success)
                    {
                        onLoaded?.Invoke(result.songs);
                    }
                    else
                    {
                        // JSON 파싱 실패 or success=false
                        onLoaded?.Invoke(null);
                    }
                }
                else
                {
                    Debug.LogError("[DBService] Error loading songs: " + www.error);
                    onLoaded?.Invoke(null);
                }
            }
        }

        #endregion

        #region Get Music Log
        public void LoadAllMusicLogs(string userId, Action<Dictionary<string, MusicLogRecord>> onLoaded)
        {
            StartCoroutine(LoadAllMusicLogsCoroutine(userId, onLoaded));
        }

        private IEnumerator LoadAllMusicLogsCoroutine(string userId, Action<Dictionary<string, MusicLogRecord>> onLoaded)
        {
            WWWForm form = new WWWForm();
            form.AddField("userId", userId);

            using (UnityWebRequest www = UnityWebRequest.Post(_getMusicLogURL, form))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    string response = www.downloadHandler.text;
                    Debug.Log("[DBService] LoadAllMusicLogs response: " + response);

                    var result = JsonUtility.FromJson<GetAllMusicLogsResponse>(response);
                    if (result != null && result.success)
                    {
                        Dictionary<string, MusicLogRecord> dict = new Dictionary<string, MusicLogRecord>();
                        foreach (var log in result.logs)
                        {
                            if (!dict.ContainsKey(log.musicId))
                            {
                                dict.Add(log.musicId, new MusicLogRecord {
                                    musicScore = log.musicScore,
                                    musicCombo = log.musicCombo,
                                    musicAccuracy = log.musicAccuracy,
                                    musicRank = log.musicRank
                                });
                            }
                        }
                        onLoaded?.Invoke(dict);
                    }
                    else
                    {
                        onLoaded?.Invoke(null);
                    }
                }
                else
                {
                    Debug.LogError("[DBService] LoadAllMusicLogs Error: " + www.error);
                    onLoaded?.Invoke(null);
                }
            }
        }
        #endregion

        #region Save Music Log
        public void SaveMusicLog(string userId, ScoreData scoreData)
        {
            StartCoroutine(SaveMusicLogCoroutine(userId, scoreData));
        }

        private IEnumerator SaveMusicLogCoroutine(string userId, ScoreData scoreData)
        {
            //폼 생성
            WWWForm form = new WWWForm();
            form.AddField("steam_id", userId);
            form.AddField("music_name", scoreData.songName);
            form.AddField("music_score", scoreData.score);
            form.AddField("music_combo", scoreData.maxCombo);
            form.AddField("music_accuracy", scoreData.accuracy.ToString("F2"));
            form.AddField("music_rank", scoreData.rank);

            //서버에 데이터 전송
            using (UnityWebRequest webRequest = UnityWebRequest.Post(_saveMusicLogURL, form))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"서버 오류: {webRequest.error}");
                }
                else
                {
                    Debug.Log(webRequest.downloadHandler.text);
                }
            }
        }
        #endregion
    }
}

[System.Serializable]
public class MusicResponse
{
    public bool success;
    public string message;
}

[Serializable]
public class GetMusicLogResponse
{
    public bool success;
    public bool hasRecord;
    public int musicScore;
    public int musicCombo;
    public string musicAccuracy;
    public string musicRank;
}

[Serializable]
public class MusicLogRecord
{
    public int musicScore;
    public int musicCombo;
    public string musicAccuracy;
    public string musicRank;
}

[System.Serializable]
public class GetAllSongsResponse
{
    public bool success;
    public List<MusicData> songs;
}

[System.Serializable]
public class MusicData
{
    public string musicId;
    public string musicName;
    public string musicArtist;
}

[Serializable]
public class GetAllMusicLogsResponse
{
    public bool success;
    public List<MusicLogData> logs;
}

[Serializable]
public class MusicLogData
{
    public string musicId;
    public int musicScore;
    public int musicCombo;
    public string musicAccuracy;
    public string musicRank;
}