using UnityEngine;

public class ScoreData
{
    public float curHp;
    public float totalScore;
    public float accuracy;
    public int combo;
    public int missCount;
    public string judgement;


    public ScoreData(float curHp = 0f, float totalScore = 0f, float accuracy = 0f, int combo = 0, int missCount = 0, string judgement = "")
    {
        this.curHp = curHp;
        this.totalScore = totalScore;
        this.accuracy = accuracy;
        this.combo = combo;
        this.missCount = missCount;
        this.judgement = judgement;
    }
}
