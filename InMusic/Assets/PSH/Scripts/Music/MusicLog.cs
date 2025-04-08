using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DB의 log 테이블과 통신용 음악 로그 Model
/// </summary>
[Serializable]
public class MusicLog
{
    [Header("기록 관련 정보")]
    [SerializeField] private string logID;
    [SerializeField] private string musicID; //맨 처음 로그랑 음악이랑 확인할 때 사용할 변수
    [SerializeField] private string musicScore; 
    [SerializeField] private string musicAccuracy;
    [SerializeField] private string musicCombo;
    [SerializeField] private string musicRank;

    public string LogID { get { return logID; } set { logID = value; } }
    public string MusicID { get { return musicID; } set { musicID = value; } }
    public string Score { get { return musicScore; } set { musicScore = value; } }
    public string Accuracy { get { return musicAccuracy; } set { musicAccuracy = value; } }
    public string Combo { get { return musicCombo; } set { musicCombo = value; } }
    public string Rank { get { return musicRank; } set { musicRank = value; } }

    //기본 생성자
    public MusicLog() {
        logID = "";
        musicID = "";
        musicScore = "0";
        musicAccuracy = "0";
        musicCombo = "0";
        musicRank = "-";
    }
}

[SerializeField]
class MusicLogList
{
    public List<MusicLog> logs;

    public List<MusicLog> GetLogs() { return logs; }
}