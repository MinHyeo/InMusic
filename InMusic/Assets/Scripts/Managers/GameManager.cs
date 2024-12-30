using UnityEngine;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int totalNotes; // �� ��Ʈ ����
    public float totalScore = 0; // ���� ����
    private float maxScorePerNote; // ��Ʈ �ϳ��� �ִ� ����
    private int totalNotesPlayed = 0; // �÷��̵� �� ��Ʈ ����
    public float accuracy = 100f; // ���� ��Ȯ�� (100%)

    public bool isGameActive = false; // ���� ����
    //public AudioClip hitSound;
    [SerializeField] private AudioSource audioSource;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

   public void InitializeGame()
    {
        // �ʱ�ȭ (��Ʈ ������ ��������)
        totalNotes = NoteManager.Instance.totalNotes; // NoteManager���� �� ��Ʈ �� ��������
        maxScorePerNote = 1000000f / totalNotes; // ��Ʈ �ϳ��� �ִ� ���� ���
        totalScore = 0;
        totalNotesPlayed = 0;
        accuracy = 100f; // �ʱ� ��Ȯ��
        isGameActive = true;

        Debug.Log("Game Initialized");
    }

    public void AddScore(string judgement)
    {
        float scoreToAdd = 0;
        float accuracyPenalty = 0;

        switch (judgement)
        {
            case "Great":
                scoreToAdd = maxScorePerNote * 1.0f;
                accuracyPenalty = 0f;
                audioSource.Play();
                break;
            case "Good":
                scoreToAdd = maxScorePerNote * 0.8f;
                accuracyPenalty = 20f / totalNotes;
                audioSource.Play();
                break;
            case "Bad":
                scoreToAdd = maxScorePerNote * 0.5f;
                accuracyPenalty = 50f / totalNotes;
                audioSource.Play();
                break;
            case "Miss":
                scoreToAdd = 0;
                accuracyPenalty = 100f / totalNotes;
                audioSource.Play();
                break;
        }

        totalScore += scoreToAdd;
        totalNotesPlayed++;
        accuracy = Mathf.Clamp(accuracy - accuracyPenalty, 0f, 100f);
        Debug.Log($"+: {scoreToAdd} || combo: {totalNotesPlayed}");

        // ������ ��Ʈ���� ���� ����
        if (totalNotesPlayed == totalNotes)
        {
            totalScore = Mathf.Round(totalScore); // ���� ���� �ݿø�
            EndGame();
        }
    }

    void EndGame()
    {
        isGameActive = false;

        Debug.Log($"Game Over! Final Score: {totalScore}");
    }
}
