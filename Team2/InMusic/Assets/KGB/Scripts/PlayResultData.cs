using TMPro;
using UnityEngine;

public class PlayResultData
{
    [SerializeField] private int score;
    [SerializeField] private int great;
    [SerializeField] private int good;
    [SerializeField] private int bad;
    [SerializeField] private int miss;
    [SerializeField] private float accuracy;
    [SerializeField] private int combo;
    [SerializeField] private float rank;
    [SerializeField] private bool fullCombo;

    public int Score { get => score; set => score = value; }
    public int Great { get => great; set => great = value; }
    public int Good { get => good; set => good = value; }
    public int Bad { get => bad; set => bad = value; }
    public int Miss { get => miss; set => miss = value; }
    public float Accuracy { get => accuracy; set => accuracy = value; }
    public int Combo { get => combo; set => combo = value; }
    public float Rank { get => rank; set => rank = value; }
    public bool FullCombo { get => fullCombo; set => fullCombo = value; }

    public PlayResultData() { }
}

