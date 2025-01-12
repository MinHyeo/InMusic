using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int totalNotes; // 총 노트 개수
    public float totalScore = 0; // 현재 점수
    public float accuracy = 100f; // 현재 정확도 (100%)
    public float curHP;
    public float maxHP = 100;
    public int combo = 0;
    public int greatCount = 0;
    public int goodCount = 0;
    public int badCount = 0;
    public int missCount = 0;
    public int maxCombo = 0;

    private float maxScorePerNote; // 노트 하나당 최대 점수
    private int totalNotesPlayed = 0; // 플레이된 총 노트 개수
    

    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] PlayUI playUI;
    [SerializeField] SinglePlayResultUI resultUI;
    [SerializeField] GameObject pauseUI;

    public bool isGameActive = false; // 게임 상태
    //public AudioClip hitSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource SongSource;
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
        // 초기화 (노트 데이터 가져오기)
        totalNotes = NoteManager.Instance.totalNotes; // NoteManager에서 총 노트 수 가져오기
        maxScorePerNote = 1000000f / totalNotes; // 노트 하나당 최대 점수 계산
        totalScore = 0;
        totalNotesPlayed = 0;
        accuracy = 100f; // 초기 정확도
        curHP = maxHP;
        if (SongSource.clip.loadState == AudioDataLoadState.Unloaded)
        {
            SongSource.clip.LoadAudioData(); // 오디오 데이터를 미리 로드
        }
        videoPlayer.Prepare();
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
                playUI.UpdatePlayUI();
                break;
        }
        playUI.JudgeTextUpdate(judgement);
        if (combo > maxCombo)
            maxCombo = combo;

        playUI.UpdatePlayUI();
        totalScore += scoreToAdd;
        totalNotesPlayed++;
        accuracy = Mathf.Clamp(accuracy - accuracyPenalty, 0f, 100f);
        if(curHP <= 0)
        {
            GameOver();
        }

        // 마지막 노트에서 점수 조정
        if (totalNotesPlayed == totalNotes)
        {
            totalScore = Mathf.Round(totalScore); // 최종 점수 반올림
            accuracy = Mathf.Round(accuracy * 100f) / 100f;
            EndGame();
        }
    }

    void EndGame()
    {
        isGameActive = false;
        playUI.countText.text = "End";
        EndingGame();
    }

    public void StartGame()
    {
        isGameActive = true;
        videoPlayer.Play();
        SongSource.Play();
    }

    private void GameOver()
    {
        EndGame();
    }
    private async void EndingGame()
    {
        await Task.Delay(3000);
        resultUI.InitResult();
        
    }
    private async void ResumCount()
    {
        playUI.countText.text = "3";
        await Task.Delay(1000);
        playUI.countText.text = "2";
        await Task.Delay(1000);
        playUI.countText.text = "1";
        await Task.Delay(1000);
        playUI.countText.text = "";
        Time.timeScale = 1f;
        SongSource.Play();
        videoPlayer.Play();
     }

    public void PauseGame()
    {
        Debug.Log("일시정지");
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        SongSource.Pause();
        videoPlayer.Pause();
    }
    public void ResumeGame()
    {
        Debug.Log("일시정지 해제");
        pauseUI.SetActive(false);
        ResumCount();
    }

}
