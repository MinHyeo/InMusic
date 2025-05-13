using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    MusicData executeData;
    List<MusicLog> logList;
    PlayerData pData;
    public bool isLogReady = false;
    public void Init() {
        executeData = GameManager_PSH.Instance.GetComponent<MusicData>();
        pData = GameManager_PSH.Instance.GetComponent<PlayerData>();

        if (executeData == null)
        {
            Debug.LogError("GameManager에 MusicData가 없습니다.");
        }

        if (pData == null)
        {
            Debug.LogError("GameManager에 Player가 없습니다.");
        }
    }

    /// <summary>
    /// 스팀에서 받은 값들 넘겨 주기
    /// </summary>
    /// <param name="playerID">스팀ID</param>
    /// <param name="playerName">스팀 프로필 닉네임</param>
    public void SetPlayerData(string playerID, string playerName)
    {
        pData.PlayerID = playerID;
        pData.PlayerName = playerName;
    }

    public string GetPlayerID()
    {
        return pData.PlayerID;
    }

    public string GetPlayerName()
    {
        return pData.PlayerName;
    }

    /// <summary>
    /// 기존 MusicData의 값과 새로 저장할 MusicLog의 값을 비교해서 저장
    /// </summary>
    public void SaveData(MusicLog newLog, string userId = "")
    {
        //신기록 확인(최종 점수만 비교)
        if (int.Parse(executeData.Score) >= int.Parse(newLog.Score)){
            Debug.Log("기록 더 안 좋음");
            return;
        }

        #region Json으로 저장
        /*//경로 설정
        string filePath = executeData.DirPath + "/" + executeData.Title + "Log.json";
        //Json형태로 변환
        string JsonData = JsonUtility.ToJson(newLog, true);
        //저장(기록이 없으면 생성, 있으면 덮어씀)
        File.WriteAllText(filePath, JsonData);
        Debug.Log("Json 저장 완료");*/
        #endregion

        #region 서버에 저장
        //GameManager_PSH.Web.UpdateLog(newLog, userId);
        #endregion
    }

    /// <summary>
    /// musicItem을 주면 musicData로 변환(?)해서 가져감
    /// </summary>
    /// <param name="executeItem"></param>
    public void SetData(MusicItem executeItem) {
        executeData.LogID = executeItem.LogID;
        executeData.MusicID = executeItem.MusicID;
        executeData.DirPath = executeItem.DirPath;
        executeData.BMS = executeItem.Data.BMS;
        executeData.Album = executeItem.Album.sprite;
        executeData.Audio = executeItem.Audio;
        executeData.MuVi = executeItem.MuVi;
    }

    public MusicData GetData() { 
        return executeData;
    }

    public void SetLogDataList(List<MusicLog> serverLog)
    {
        logList = serverLog;
        isLogReady = true;
    }

    public List<MusicLog> getLogDataList()
    {
        return logList;
    }
}
