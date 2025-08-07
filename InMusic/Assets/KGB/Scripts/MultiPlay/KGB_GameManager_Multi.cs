using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KGB_GameManager_Multi : MonoBehaviour, IGameManager
{

    public static KGB_GameManager_Multi Instance { get; private set; }

    [SerializeField] BMSData testBMS; //테스트용으로 폴더에서 드래그로 할당해서 쓰는 중

    public int totalNotes; // 총 노트 개수
    private float maxScorePerNote; // 노트 하나당 최대 점수
    private int totalNotesPlayed = 0; // 플레이된 총 노트 개수
    public float totalScore = 0; // 최종 점수
    public float accuracy = 100f; // 현재 정확도 (100%)
    public int greatCount = 0; //Great 횟수
    public int goodCount = 0;  //Good 횟수
    public int badCount = 0;   //bad 획수
    public int missCount = 0;  //miss 횟수
    public int maxCombo = 0;   //최대 콤보수
    public int combo = 0; //현재 콤포

    public float curHP;
    public float maxHP = 100;

    public bool isGameActive = false; // 게임 상태

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
        //testBMS = GameManager_PSH.Instance.GetComponent<MusicData>().BMS; //나중에 씬 연결되면 선택된 곡 BMS 가져오기

        string fileName = "Music/Heya2/HeyaBMS";
        testBMS = bmsManager.ParseBMS(fileName);
        if (testBMS != null)
        {
            NoteManager.Instance.SetNote(testBMS);
            Debug.Log("BMS 데이터 할당 완료");
        }
        else
        {
            Debug.LogError("BMS 데이터 파싱 실패");
        }

    }

    public void InitializeGame()
    {
        // 초기화 (노트 데이터 가져오기)
        totalNotes = NoteManager.Instance.totalNotes; // NoteManager에서 총 노트 수 가져오기
        maxScorePerNote = 1000000f / totalNotes; // 노트 하나당 최대 점수 계산
        totalScore = 0;
        totalNotesPlayed = 0;
        accuracy = 100f; // 초기 정확도
        greatCount = 0; //Great 횟수
        goodCount = 0;  //Good 횟수
        badCount = 0;   //bad 획수
        missCount = 0;  //miss 횟수
        maxCombo = 0;   //최대 콤보수
        combo = 0; //현재 콤포
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
        Debug.Log("일시정지");
    }
    public void ResumeGame()
    {
        Debug.Log("일시정지 해제");

    }

    public void StartMusic()
    {
    }


    // ====== 인터페이스 구현 ======

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
