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
    [SerializeField] private string score;
    [SerializeField] private string accuracy;
    [SerializeField] private string combo;
    [SerializeField] private string rank;

    public string LogID { get { return logID; } set { logID = value; } }
    public string Score { get { return score; } set { score = value; } }
    public string Accuracy { get { return accuracy; } set { accuracy = value; } }
    public string Combo { get { return combo; } set {combo = value; } }
    public string Rank { get { return rank; } set { rank = value; } }

    //�⺻ ������
    public MusicLog() {
        logID = "";
        score = "0";
        accuracy = "0";
        combo = "0";
        rank = "-";
    }
}

[SerializeField]
class MusicLogList
{
    public List<MusicLog> musicLogs;
}