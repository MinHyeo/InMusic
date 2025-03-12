using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public BMSData curBMS;
    public string resourcePath;

    public int totalNotes; // 총 노트 개수
    private float maxScorePerNote; // 노트 하나당 최대 점수
    private int totalNotesPlayed = 0; // 플레이된 총 노트 개수

    public float curHP;
    public float maxHP = 100;



    public float totalScore = 0; // 최종 점수
    public float accuracy = 100f; // 현재 정확도 (100%)
    public int greatCount = 0; //Great 횟수
    public int goodCount = 0;  //Good 횟수
    public int badCount = 0;   //bad 획수
    public int missCount = 0;  //miss 횟수
    public int maxCombo = 0;   //최대 콤보수

    public int combo = 0; //현재 콤포


    [SerializeField] PlayUI playUI;
    [SerializeField] SinglePlayResultUI resultUI;
    [SerializeField] GameObject pauseUI;
    [SerializeField] GameObject gameoverUI;
    [SerializeField] PlayManager playManager;

    public bool isGameActive = false; // 게임 상태
    //public AudioClip hitSound;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //string musicName = GameManager_PSH.Instance.GetSelectedMusic(); //노래 선택 씬에서 선택한 노래이름 받아옴.
        //Debug.Log(musicName);
        //string path = "Music_KGB/Heya"; //임시경로, 경로를 받았을때 세팅 
        //SetResourcePath(path);
        SetBMS(); //지정된 노래 //BMS파일을 만들때 노래가 시작되는 지점에 WAV02 노트로 찍어놔야함.(노트생성시 필요)

        playManager = GetComponent<PlayManager>();
        playManager.SetResources(); // 노래 리소스 세팅
        NoteManager.Instance.SetNote(curBMS); //노트생성
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!pauseUI.activeSelf)
                PauseGame();
        }
    }

    public void SetBMS()
    {
        ////curBMS = BMSManager.Instance.ParseBMS(music); //BMS 파싱 //노래 이름 받을때 쓴거
        //Debug.Log(path + "/BMS");
        //curBMS = BMSManager.Instance.ParseBMS(path+"/BMS");
        curBMS = GameManager_PSH.Instance.GetComponent<MusicData>().BMS;
        
    }
    public void SetResourcePath(string path) //선택한 노래 경로받아서 저장
    {
        resourcePath = path;
    }

   public void InitializeGame()
    {

        pauseUI.SetActive(false);
        gameoverUI.SetActive(false);
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
        LoadingScreen.Instance.SceneReady();
        

        StartGame();
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
                playManager.hitSound.Play();
                break;
            case "Good":
                scoreToAdd = maxScorePerNote * 0.8f;
                accuracyPenalty = 20f / totalNotes;
                playManager.hitSound.Play();
                combo++;
                goodCount++;
                break;
            case "Bad":
                scoreToAdd = maxScorePerNote * 0.5f;
                accuracyPenalty = 50f / totalNotes;
                playManager.hitSound.Play();
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
        playManager.musicSound.Stop();
        playUI.countText.text = "End";
        EndingGame();
    }
    public void StartGame()
    {
        isGameActive = true;
    }
    public void StartMusic()
    {
        if (playManager.videoPlayer.clip != null)
            playManager.videoPlayer.Play();
        playManager.musicSound.Play();
    }

    private void GameOver()
    {
        isGameActive = false;
        playManager.musicSound.Stop();
        gameoverUI.SetActive(true);
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
        playManager.musicSound.Play();
        if (playManager.videoPlayer.clip != null)
            playManager.videoPlayer.Play();
     }

    public void PauseGame()
    {
        Debug.Log("일시정지");
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        playManager.musicSound.Pause();
        if (playManager.videoPlayer.clip != null)
            playManager.videoPlayer.Pause();
    }
    public void ResumeGame()
    {
        Debug.Log("일시정지 해제");
        pauseUI.SetActive(false);
        ResumCount();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f; // 씬 로드 후 타임스케일 초기화

    }

    private void OnDestroy()
    {
        // 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
