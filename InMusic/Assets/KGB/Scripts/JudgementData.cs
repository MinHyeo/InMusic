using UnityEngine;

public class JudgementData
{
    public int noteId;
    public string judgement;
    public int keyIndex;
    public float percent;

    public JudgementData(int noteId, string judgement, int keyIndex, float percent)
    {
        this.noteId = noteId;
        this.judgement = judgement;
        this.keyIndex = keyIndex;
        this.percent = percent;
    }
}
