using System.IO;
using UnityEngine;

public class DataManager
{
    MusicData executeData;

    public void Init() {
        executeData = GameManager_PSH.Instance.GetComponent<MusicData>();
        if (executeData == null)
        {
            Debug.LogError("GameManager에 MusicData가 없습니다.");
        }
    }

    /// <summary>
    /// 기존 MusicData의 값과 새로 저장할 MusicLog의 값을 비교해서 저장
    /// </summary>
    public void SaveData(MusicLog newLog)
    {
        //신기록 확인(최종 점수만 비교)
        if (int.Parse(executeData.Score) >= int.Parse(newLog.Score)){
            Debug.Log("기록 더 안 좋음");
            return;
        }
        //경로 설정
        string filePath = executeData.DirPath + "/" + executeData.Title + "Log.json";
        //Json형태로 변환
        string JsonData = JsonUtility.ToJson(newLog, true);
        //저장(기록이 없으면 생성, 있으면 덮어씀)
        File.WriteAllText(filePath, JsonData);
        Debug.Log("Json 저장 완료");
    }

    public void SetData(MusicItem executeItem) {
        executeData.DirPath = executeItem.DirPath;
        executeData.BMS = executeItem.Data.BMS;
        executeData.Album = executeItem.Album.sprite;
        executeData.Audio = executeItem.Audio;
        executeData.MuVi = executeItem.MuVi;
    }

    public MusicData GetData() { 
        return mData;
    }
}
