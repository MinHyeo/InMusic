using Fusion;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KGB_GameManager_Multi : MonoBehaviour, IGameManager
{
    [SerializeField] PlayerRespawner playerRespawner;
    public static KGB_GameManager_Multi Instance { get; private set; }

    [SerializeField] BMSData testBMS; //테스트용으로 폴더에서 드래그로 할당해서 쓰는 중
    [SerializeField] BMSData curBMS; //테스트용으로 폴더에서 드래그로 할당해서 쓰는 중

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
    private string curJudgement;

    BMSManager bmsManager;

    public PlayUI playUI;
    public PlayUI_Multi playUI_Multi;
    public ScoreBoardUI scoreBoardUI;
    public GameObject resultUI;
    public PlayManager playManager;

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

        DontDestroyOnLoad(gameObject);
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
        curBMS = GameManager_PSH.Instance.GetComponent<MusicData>().BMS;
        //string fileName = "Music/Heya2/HeyaBMS";
        //testBMS = bmsManager.ParseBMS(fileName);
        if (curBMS != null)
        {
            playManager = GetComponent<PlayManager>();
            playManager.SetResources(); // 노래 리소스 세팅
            NoteManager.Instance.SetNote(curBMS);
            MultiNoteManager.Instance.SetNote(curBMS);
            Debug.Log("BMS 데이터 할당 완료");
        }
        else
        {
            Debug.LogError("BMS 데이터 할당 실패");
        }

    }

    public void InitializeGame()
    {
        if (playManager.musicSound.clip.loadState == AudioDataLoadState.Unloaded)
        {
            playManager.musicSound.clip.LoadAudioData(); // 오디오 데이터를 미리 로드
        }
        if (playManager.videoPlayer.clip != null)
        {
            playManager.videoPlayer.Prepare();
        }
        // 초기화 (노트 데이터 가져오기)
        totalNotes = NoteManager.Instance.totalNotes; // NoteManager에서 총 노트 수 가져오기
        maxScorePerNote = 1000000f / totalNotes; // 노트 하나당 최대 점수 계산
        totalScore = 0;
        totalNotesPlayed = 0;
        accuracy = 100f; // 초기 정확도
        curHP = maxHP;
        greatCount = 0; //Great 횟수
        goodCount = 0;  //Good 횟수
        badCount = 0;   //bad 획수
        missCount = 0;  //miss 횟수
        maxCombo = 0;   //최대 콤보수
        combo = 0; //현재 콤포
        //LoadingScreen.Instance.SceneReady();
        Debug.Log("Game Initialized");

        StartCoroutine(Ready());
    }


    public void StartGame()
    {
        isGameActive = true;
        Debug.Log("게임시작");
        //1초 뒤 시뮬레이션도 시작
        StartCoroutine(StartSimulationAfterDelay(0.5f));
    }
    private IEnumerator StartSimulationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // delay만큼 기다림
        if (MultiNoteManager.Instance != null)
        {
            Debug.Log("1초 뒤 시뮬 시작");
            MultiNoteManager.Instance.isMoving = true;
        }
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
                playManager.hitSound.Play();
                break;
            case "Good":
                scoreToAdd = maxScorePerNote * 0.8f;
                accuracyPenalty = 20f / totalNotes;
                combo++;
                goodCount++;
                playManager.hitSound.Play();
                break;
            case "Bad":
                scoreToAdd = maxScorePerNote * 0.5f;
                accuracyPenalty = 50f / totalNotes;
                combo++;
                badCount++;
                playManager.hitSound.Play();
                break;
            case "Miss":
                scoreToAdd = 0;
                accuracyPenalty = 100f / totalNotes;
                curHP -= 10f;
                combo = 0;
                missCount++;
                playUI.UpdatePlayUI();
                break;
        }
        playUI.JudgeTextUpdate(judgement);
        curJudgement = judgement;
        if (combo > maxCombo)
            maxCombo = combo;

        playUI.UpdatePlayUI();
        totalScore += scoreToAdd;
        totalNotesPlayed++;
        accuracy = Mathf.Clamp(accuracy - accuracyPenalty, 0f, 100f);


        scoreBoardUI.UpdateScoreBoard_p1();
        MultPlayManager.Instance.RPC_SendNoteJudgement(noteIndex, 1, accuracy, curHP, totalScore, combo, missCount, judgement);
        //MultPlayManager.Instance.RPC_SendScoreData(curHP, totalScore, accuracy, combo, missCount, judgement);
        // 마지막 노트에서 점수 조정
        if (totalNotesPlayed == totalNotes)
        {
            totalScore = Mathf.Round(totalScore); // 최종 점수 반올림
            accuracy = Mathf.Round(accuracy * 100f) / 100f;
            EndGame();
        }
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
        if (playManager.videoPlayer.clip != null)
            playManager.videoPlayer.Play();
        playManager.musicSound.Play();
    }
    void EndGame()
    {
        //isGameActive = false;
        playManager.musicSound.Stop();
        playUI.countText.text = "End";
        EndingGame();
    }
    private async void EndingGame()
    {
        await Task.Delay(3000);
        //결과창 띄우기
        resultUI.SetActive(true);
        //SceneManager.LoadScene("MultiPlay_Result");
        //playerRespawner.P1.GetComponent<PlayerUIController>().BroadGameEnd();
    }

    public ScoreData GetScoreData()
    {
        ScoreData scoreData = new ScoreData(curHP, totalScore, accuracy, combo, missCount, curJudgement);
        return scoreData;
    }

    public IEnumerator Ready()
    {
        Debug.Log("레디 기다리는중");
        yield return new WaitUntil(() => MultPlayManager.Instance != null);
        MultPlayManager.Instance.RPC_CheckReady();
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
