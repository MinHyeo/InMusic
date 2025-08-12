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
        badThresholdDistance = ScrollSpeed * 0.2289f;     // 228.9ms (¿¹½Ã)

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
    public void JudgmentNote()
    {
        distance = Mathf.Abs(transform.position.y - line.position.y);
        if (distance <= greatThresholdDistance)
        {
            Debug.Log($"Great");
        }
        else if (distance <= goodThresholdDistance)
        {
            Debug.Log($"Good");
        }
        else if (distance <= badThresholdDistance)
        {
            Debug.Log($"Bad");
        }
        else
        {
            Debug.Log($"Miss");
        }
    }

    public void MissNote()
    {
        Debug.Log($"Miss");
    }
}
