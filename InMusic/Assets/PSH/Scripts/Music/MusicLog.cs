using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �α� ���� Model
/// </summary>
[Serializable]
public class MusicLog
{
    [Header("��� ���� ����")]
    [SerializeField] private string logID;
    [SerializeField] private string musicScore;
    [SerializeField] private string musicAccuracy;
    [SerializeField] private string musicCombo;
    [SerializeField] private string musicRank;

    public string LogID { get { return logID; } set { logID = value; } }
    public string Score { get { return musicScore; } set { musicScore = value; } }
    public string Accuracy { get { return musicAccuracy; } set { musicAccuracy = value; } }
    public string Combo { get { return musicCombo; } set { musicCombo = value; } }
    public string Rank { get { return musicRank; } set { musicRank = value; } }

    //�⺻ ������
    public MusicLog() {
        logID = "";
        musicScore = "0";
        musicAccuracy = "0";
        musicCombo = "0";
        musicRank = "-";
    }
}

[SerializeField]
class MusicLogList
{
    public List<MusicLog> musicLogs;
}