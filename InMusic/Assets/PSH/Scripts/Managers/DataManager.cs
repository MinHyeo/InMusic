using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager
{
    MusicData executeData;
    List<MusicLog> logList;

    public void Init() {
        executeData = GameManager_PSH.Instance.GetComponent<MusicData>();
        if (executeData == null)
        {
            Debug.LogError("GameManager�� MusicData�� �����ϴ�.");
        }
    }

    /// <summary>
    /// ���� MusicData�� ���� ���� ������ MusicLog�� ���� ���ؼ� ����
    /// </summary>
    public void SaveData(MusicLog newLog, string userId = "")
    {
        //�ű�� Ȯ��(���� ������ ��)
        if (int.Parse(executeData.Score) >= int.Parse(newLog.Score)){
            Debug.Log("��� �� �� ����");
            return;
        }

        #region Json���� ����
        /*//��� ����
        string filePath = executeData.DirPath + "/" + executeData.Title + "Log.json";
        //Json���·� ��ȯ
        string JsonData = JsonUtility.ToJson(newLog, true);
        //����(����� ������ ����, ������ ���)
        File.WriteAllText(filePath, JsonData);
        Debug.Log("Json ���� �Ϸ�");*/
        #endregion

        #region ������ ����
        //GameManager_PSH.Web.UpdateLog(newLog, userId);
        #endregion
    }

    /// <summary>
    /// musicItem�� �ָ� musicData�� ��ȯ(?)�ؼ� ������
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

    }

    public List<MusicLog> getLogDataList()
    {
        return logList;
    }
}
