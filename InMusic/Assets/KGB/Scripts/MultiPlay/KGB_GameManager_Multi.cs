using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KGB_GameManager_Multi : MonoBehaviour, IGameManager
{

    public static KGB_GameManager_Multi Instance { get; private set; }

    [SerializeField] BMSData testBMS; //�׽�Ʈ������ �������� �巡�׷� �Ҵ��ؼ� ���� ��

    public int totalNotes; // �� ��Ʈ ����
    private float maxScorePerNote; // ��Ʈ �ϳ��� �ִ� ����
    private int totalNotesPlayed = 0; // �÷��̵� �� ��Ʈ ����
    public float totalScore = 0; // ���� ����
    public float accuracy = 100f; // ���� ��Ȯ�� (100%)
    public int greatCount = 0; //Great Ƚ��
    public int goodCount = 0;  //Good Ƚ��
    public int badCount = 0;   //bad ȹ��
    public int missCount = 0;  //miss Ƚ��
    public int maxCombo = 0;   //�ִ� �޺���
    public int combo = 0; //���� ����

    public float curHP;
    public float maxHP = 100;

    public bool isGameActive = false; // ���� ����

    BMSManager bmsManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
        bmsManager = new BMSManager();
        SetBMS();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBMS()
    {
        //testBMS = GameManager_PSH.Instance.GetComponent<MusicData>().BMS; //���߿� �� ����Ǹ� ���õ� �� BMS ��������

        string fileName = "Music/Heya2/HeyaBMS";
        testBMS = bmsManager.ParseBMS(fileName);
        if (testBMS != null)
        {
            NoteManager.Instance.SetNote(testBMS);
            Debug.Log("BMS ������ �Ҵ� �Ϸ�");
        }
        else
        {
            Debug.LogError("BMS ������ �Ľ� ����");
        }

    }

    public void InitializeGame()
    {
        // �ʱ�ȭ (��Ʈ ������ ��������)
        totalNotes = NoteManager.Instance.totalNotes; // NoteManager���� �� ��Ʈ �� ��������
        maxScorePerNote = 1000000f / totalNotes; // ��Ʈ �ϳ��� �ִ� ���� ���
        totalScore = 0;
        totalNotesPlayed = 0;
        accuracy = 100f; // �ʱ� ��Ȯ��
        greatCount = 0; //Great Ƚ��
        goodCount = 0;  //Good Ƚ��
        badCount = 0;   //bad ȹ��
        missCount = 0;  //miss Ƚ��
        maxCombo = 0;   //�ִ� �޺���
        combo = 0; //���� ����
        //LoadingScreen.Instance.SceneReady();

        StartGame();
        Debug.Log("Game Initialized");
    }

    public void StartGame()
    {
        isGameActive = true;
    }

    public void AddScore(string judgement, int noteIndex)
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
                break;
            case "Good":
                scoreToAdd = maxScorePerNote * 0.8f;
                accuracyPenalty = 20f / totalNotes;
                combo++;
                goodCount++;
                break;
            case "Bad":
                scoreToAdd = maxScorePerNote * 0.5f;
                accuracyPenalty = 50f / totalNotes;
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
        //playUI.JudgeTextUpdate(judgement);
        if (combo > maxCombo)
            maxCombo = combo;

        //playUI.UpdatePlayUI();
        totalScore += scoreToAdd;
        totalNotesPlayed++;
        accuracy = Mathf.Clamp(accuracy - accuracyPenalty, 0f, 100f);

        MultPlayManager.Instance.RPC_SendNoteJudgement(noteIndex, judgement, 1, accuracy);
    }
    public void PauseGame()
    {
        Debug.Log("�Ͻ�����");
    }
    public void ResumeGame()
    {
        Debug.Log("�Ͻ����� ����");

    }

    public void StartMusic()
    {
    }


    // ====== �������̽� ���� ======

    public bool IsGameActive => isGameActive;
    public int TotalNotes => totalNotes;
    public float TotalScore => totalScore;
    public float Accuracy => accuracy;
    public int GreatCount => greatCount;
    public int GoodCount => goodCount;
    public int BadCount => badCount;
    public int MissCount => missCount;
    public int MaxCombo => maxCombo;
    public int Combo => combo;
    public float CurHP => curHP;
    public float MaxHP => maxHP;

}
