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

    [SerializeField] BMSData testBMS; //�׽�Ʈ������ �������� �巡�׷� �Ҵ��ؼ� ���� ��
    [SerializeField] BMSData curBMS; //�׽�Ʈ������ �������� �巡�׷� �Ҵ��ؼ� ���� ��

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
        //testBMS = GameManager_PSH.Instance.GetComponent<MusicData>().BMS; //���߿� �� ����Ǹ� ���õ� �� BMS ��������
        curBMS = GameManager_PSH.Instance.GetComponent<MusicData>().BMS;
        //string fileName = "Music/Heya2/HeyaBMS";
        //testBMS = bmsManager.ParseBMS(fileName);
        if (curBMS != null)
        {
            playManager = GetComponent<PlayManager>();
            playManager.SetResources(); // �뷡 ���ҽ� ����
            NoteManager.Instance.SetNote(curBMS);
            MultiNoteManager.Instance.SetNote(curBMS);
            Debug.Log("BMS ������ �Ҵ� �Ϸ�");
        }
        else
        {
            Debug.LogError("BMS ������ �Ҵ� ����");
        }

    }

    public void InitializeGame()
    {
        if (playManager.musicSound.clip.loadState == AudioDataLoadState.Unloaded)
        {
            playManager.musicSound.clip.LoadAudioData(); // ����� �����͸� �̸� �ε�
        }
        if (playManager.videoPlayer.clip != null)
        {
            playManager.videoPlayer.Prepare();
        }
        // �ʱ�ȭ (��Ʈ ������ ��������)
        totalNotes = NoteManager.Instance.totalNotes; // NoteManager���� �� ��Ʈ �� ��������
        maxScorePerNote = 1000000f / totalNotes; // ��Ʈ �ϳ��� �ִ� ���� ���
        totalScore = 0;
        totalNotesPlayed = 0;
        accuracy = 100f; // �ʱ� ��Ȯ��
        curHP = maxHP;
        greatCount = 0; //Great Ƚ��
        goodCount = 0;  //Good Ƚ��
        badCount = 0;   //bad ȹ��
        missCount = 0;  //miss Ƚ��
        maxCombo = 0;   //�ִ� �޺���
        combo = 0; //���� ����
        //LoadingScreen.Instance.SceneReady();
        Debug.Log("Game Initialized");

        StartCoroutine(Ready());
    }


    public void StartGame()
    {
        isGameActive = true;
        Debug.Log("���ӽ���");
        //1�� �� �ùķ��̼ǵ� ����
        StartCoroutine(StartSimulationAfterDelay(0.5f));
    }
    private IEnumerator StartSimulationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // delay��ŭ ��ٸ�
        if (MultiNoteManager.Instance != null)
        {
            Debug.Log("1�� �� �ù� ����");
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
        // ������ ��Ʈ���� ���� ����
        if (totalNotesPlayed == totalNotes)
        {
            totalScore = Mathf.Round(totalScore); // ���� ���� �ݿø�
            accuracy = Mathf.Round(accuracy * 100f) / 100f;
            EndGame();
        }
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
        //���â ����
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
        Debug.Log("���� ��ٸ�����");
        yield return new WaitUntil(() => MultPlayManager.Instance != null);
        MultPlayManager.Instance.RPC_CheckReady();
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
