using Unity.VisualScripting;
using UnityEngine;

public class Note : MonoBehaviour
{
    int noteIndex; //��Ʈ ���� ������ �ְ�ޱ� ���� �ε��� �߰�, ��Ʈ ���� �� ������� 1���� �ο�
    // �뷡�� �� ��Ʈ�� 100����� ù ��Ʈ�� 1, �������� 100
    //��Ʈ ���� ������ ��Ʈ�ε���, ����(good, bad ��)���� �ʿ� �� �Է� �ð� �߰�

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
        badThresholdDistance = ScrollSpeed * 0.2289f;     // 228.9ms (����)
        
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
