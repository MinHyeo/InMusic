using UnityEngine;
using UnityEngine.UI;

public class MultiMiss : MonoBehaviour
{
    [SerializeField]
    private Text myMissText;
    [SerializeField]
    private Text matchMissText;

    public void Init()
    {
        if (myMissText == null || matchMissText == null)
        {
            Debug.LogError("Miss Text components are not assigned in MultiMiss.");
            return;
        }

        myMissText.text = "0";
        matchMissText.text = "0";
    }

    public void UpdateMissCount(int myMissCount, int matchMissCount)
    {
        if (myMissText == null || matchMissText == null)
        {
            Debug.LogError("Miss Text components are not assigned in MultiMiss.");
            return;
        }

        myMissText.text = $"{myMissCount}";
        matchMissText.text = $"{matchMissCount}";
    }
}
