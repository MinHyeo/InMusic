using System;
using UnityEngine;

[Serializable]
public class MusicLog
{
    [Header("��� ���� ����")]
    [SerializeField] private string score;
    [SerializeField] private string accuracy;
    [SerializeField] private string combo;
    public string Score { get { return score; } set { score = value; } }
    public string Accuracy { get { return accuracy; } set { accuracy = value; } }
    public string Combo { get { return combo; } set {combo = value; } }
}
