using Unity.VisualScripting;
using UnityEngine;

public class Note : MonoBehaviour
{
    int noteIndex; //노트 판정 정보를 주고받기 위해 인덱스 추가, 노트 생성 시 순서대로 1부터 부여
    // 노래의 총 노트가 100개라면 첫 노트는 1, 마지막은 100
    //노트 판정 정보는 노트인덱스, 판정(good, bad 등)으로 필요 시 입력 시간 추가

    float ScrollSpeed;
    float perfectThresholdDistance;
    float greatThresholdDistance;
    float goodThresholdDistance; 
    float badThresholdDistance;
    //private bool isJudged = false;
    float distance;
    public Transform line;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        line = NoteManager.Instance.judgeLinePos;
        ScrollSpeed = NoteManager.Instance.baseScrollSpeed;
        greatThresholdDistance = ScrollSpeed * 0.1041f; // 104.1ms
        goodThresholdDistance = ScrollSpeed * 0.1873f;  // 187.3ms
        badThresholdDistance = ScrollSpeed * 0.2289f;     // 228.9ms (예시)
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JudgmentNote()
    {
        distance = Mathf.Abs(transform.position.y - line.position.y);
        if (distance <= greatThresholdDistance)
        {
            Debug.Log($"Great");
            GameManagerProvider.Instance.AddScore("Great");
        }
        else if (distance <= goodThresholdDistance)
        {
            Debug.Log($"Good");
            GameManagerProvider.Instance.AddScore("Good");
        }
        else if (distance <= badThresholdDistance)
        {
            Debug.Log($"Bad");
            GameManagerProvider.Instance.AddScore("Bad");
        }
        else
        {
            Debug.Log($"Miss");
            GameManagerProvider.Instance.AddScore("Miss");
        }
    }

    public void MissNote()
    {
        Debug.Log($"Miss");
        GameManagerProvider.Instance.AddScore("Miss");
    }
}
