using UnityEngine;

public class Note_Multi : MonoBehaviour
{
    float ScrollSpeed;
    float perfectThresholdDistance;
    float greatThresholdDistance;
    float goodThresholdDistance;
    float badThresholdDistance;
    //private bool isJudged = false;
    float distance;
    public int index = -1;
    public Transform line;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        noteIndex = -1;
    }
    void Start()
    {

        line = MultiNoteManager.Instance.judgeLinePos;
        ScrollSpeed = MultiNoteManager.Instance.baseScrollSpeed;
        greatThresholdDistance = ScrollSpeed * 0.1041f; // 104.1ms
        goodThresholdDistance = ScrollSpeed * 0.1873f;  // 187.3ms
        badThresholdDistance = ScrollSpeed * 0.2289f;     // 228.9ms (예시)

    }

    // Update is called once per frame
    void Update()
    {

    }

    public int noteIndex
    {
        get => index;
        set => index = value;
    }
    public void JudgmentSimulateNote(string judgement, float percent)
    {
        Debug.Log($"[SimulateJudge] 판정: {judgement}, 정확도: {percent}");

        // 판정별 이펙트/애니메이션
        switch (judgement)
        {
            case "Perfect":
                break;
            case "Great":

                break;
            case "Good":

                break;
            case "Miss":

                break;
        }

        // 노트 제거
        gameObject.SetActive(false);
    }

    public void MissNote()
    {
        Debug.Log($"Miss");
    }
}
