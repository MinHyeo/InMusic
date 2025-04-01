using System.IO;
using UnityEngine;

public class DataManager
{
    MusicData executeData;

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
    public void SaveData(MusicLog newLog)
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

        #endregion
    }

    /// <summary>
    /// musicItem�� �ָ� musicData�� ��ȯ(?)�ؼ� ������
    /// </summary>
    /// <param name="executeItem"></param>
    public void SetData(MusicItem executeItem) {
        executeData.LogID = executeItem.LogID;
        executeData.DirPath = executeItem.DirPath;
        executeData.BMS = executeItem.Data.BMS;
        executeData.Album = executeItem.Album.sprite;
        executeData.Audio = executeItem.Audio;
        executeData.MuVi = executeItem.MuVi;
    }

    public MusicData GetData() { 
        return executeData;
    }
}
