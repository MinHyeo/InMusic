using System.IO;
using UnityEngine;

public class DataManager
{
    MusicData mData;

    public void Init() {
        mData = GameManager_PSH.Instance.GetComponent<MusicData>();
        if (mData == null)
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
        if (int.Parse(mData.Score) >= int.Parse(newLog.Score)){
            Debug.Log("��� �� �� ����");
            return;
        }
        //��� ����
        string filePath = mData.DirPath + "/" + mData.Title + "Log.json";
        //Json���·� ��ȯ
        string JsonData = JsonUtility.ToJson(newLog, true);
        //����(����� ������ ����, ������ ���)
        File.WriteAllText(filePath, JsonData);
        Debug.Log("Json ���� �Ϸ�");
    }

    public void SetData(Music_Item item) {
        mData.DirPath = item.DirPath;
        mData.BMS = item.Data.BMS;
        mData.Album = item.Album.sprite;
        mData.Audio = item.Audio;
        mData.MuVi = item.MuVi;
    }
}
