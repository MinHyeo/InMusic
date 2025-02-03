using System;
using UnityEngine;

[Serializable]
public class MusicLog
{
    [Header("기록 관련 정보")]
    [SerializeField] private string score;
    [SerializeField] private string accuracy;
    [SerializeField] private string combo;
    [SerializeField] private string rank;
    public string Score { get { return score; } set { score = value; } }
    public string Accuracy { get { return accuracy; } set { accuracy = value; } }
    public string Combo { get { return combo; } set {combo = value; } }
    public string Rank { get { return rank; } set { rank = value; } }

    //기본 생성자
    public MusicLog() { }
}
