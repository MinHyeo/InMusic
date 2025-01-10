using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int totalNotes; // �� ��Ʈ ����
    public float totalScore = 0; // ���� ����
    public float accuracy = 100f; // ���� ��Ȯ�� (100%)
    public float curHP;
    public float maxHP = 100;
    public int combo = 0;
    public int greatCount = 0;
    public int goodCount = 0;
    public int badCount = 0;
    public int missCount = 0;
    public int maxCombo = 0;

    private float maxScorePerNote; // ��Ʈ �ϳ��� �ִ� ����
    private int totalNotesPlayed = 0; // �÷��̵� �� ��Ʈ ����
    

    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] PlayUI playUI;

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
        curHP = maxHP;
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
                combo++;
                greatCount++;
                audioSource.Play();
                break;
            case "Good":
                scoreToAdd = maxScorePerNote * 0.8f;
                accuracyPenalty = 20f / totalNotes;
                audioSource.Play();
                combo++;
                goodCount++;
                break;
            case "Bad":
                scoreToAdd = maxScorePerNote * 0.5f;
                accuracyPenalty = 50f / totalNotes;
                audioSource.Play();
                combo++;
                badCount++;
                break;
            case "Miss":
                scoreToAdd = 0;
                accuracyPenalty = 100f / totalNotes;
                curHP -= 10f;
                combo = 0;
                missCount++;
                break;
        }
        playUI.JudgeTextUpdate(judgement);
        if (combo > maxCombo)
            maxCombo = combo;

        playUI.UpdatePlayUI();
        totalScore += scoreToAdd;
        totalNotesPlayed++;
        accuracy = Mathf.Clamp(accuracy - accuracyPenalty, 0f, 100f);
        Debug.Log($"+: {scoreToAdd} || combo: {totalNotesPlayed}");
        if(curHP <= 0)
        {
            GameOver();
        }

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
    }

    public void StartGame()
    {
        isGameActive = true;
        NoteManager.Instance.isMoving = true;
        videoPlayer.Play();
    }

    private void GameOver()
    {
        EndGame();
    }
}
