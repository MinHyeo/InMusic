using Unity.VisualScripting;
using UnityEngine;

public class Note : MonoBehaviour
{
    //��Ʈ ���� ������ �ְ�ޱ� ���� �ε��� �߰�, ��Ʈ ���� �� ������� 1���� �ο�
    // �뷡�� �� ��Ʈ�� 100����� ù ��Ʈ�� 1, �������� 100
    //��Ʈ ���� ������ ��Ʈ�ε���, ����(good, bad ��)���� �ʿ� �� �Է� �ð� �߰�

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
            GameManagerProvider.Instance.AddScore("Great", index);
        }
        else if (distance <= goodThresholdDistance)
        {
            Debug.Log($"Good");
            GameManagerProvider.Instance.AddScore("Good", index);
        }
        else if (distance <= badThresholdDistance)
        {
            Debug.Log($"Bad");
            GameManagerProvider.Instance.AddScore("Bad", index);
        }
        else
        {
            Debug.Log($"Miss");
            GameManagerProvider.Instance.AddScore("Miss", index);
        }
    }

    public void MissNote()
    {
        Debug.Log($"Miss");
        GameManagerProvider.Instance.AddScore("Miss", noteIndex);
    }
}
