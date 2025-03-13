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

    public int totalNotes; // �� ��Ʈ ����
    private float maxScorePerNote; // ��Ʈ �ϳ��� �ִ� ����
    private int totalNotesPlayed = 0; // �÷��̵� �� ��Ʈ ����

    public float curHP;
    public float maxHP = 100;



    public float totalScore = 0; // ���� ����
    public float accuracy = 100f; // ���� ��Ȯ�� (100%)
    public int greatCount = 0; //Great Ƚ��
    public int goodCount = 0;  //Good Ƚ��
    public int badCount = 0;   //bad ȹ��
    public int missCount = 0;  //miss Ƚ��
    public int maxCombo = 0;   //�ִ� �޺���

    public int combo = 0; //���� ����


    [SerializeField] PlayUI playUI;
    [SerializeField] SinglePlayResultUI resultUI;
    [SerializeField] GameObject pauseUI;
    [SerializeField] GameObject gameoverUI;
    [SerializeField] PlayManager playManager;

    public bool isGameActive = false; // ���� ����
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
        //string musicName = GameManager_PSH.Instance.GetSelectedMusic(); //�뷡 ���� ������ ������ �뷡�̸� �޾ƿ�.
        //Debug.Log(musicName);
        //string path = "Music_KGB/Heya"; //�ӽð��, ��θ� �޾����� ���� 
        //SetResourcePath(path);
        SetBMS(); //������ �뷡 //BMS������ ���鶧 �뷡�� ���۵Ǵ� ������ WAV02 ��Ʈ�� ��������.(��Ʈ������ �ʿ�)

        playManager = GetComponent<PlayManager>();
        playManager.SetResources(); // �뷡 ���ҽ� ����
        NoteManager.Instance.SetNote(curBMS); //��Ʈ����
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
        ////curBMS = BMSManager.Instance.ParseBMS(music); //BMS �Ľ� //�뷡 �̸� ������ ����
        //Debug.Log(path + "/BMS");
        //curBMS = BMSManager.Instance.ParseBMS(path+"/BMS");
        curBMS = GameManager_PSH.Instance.GetComponent<MusicData>().BMS;
        
    }
    public void SetResourcePath(string path) //������ �뷡 ��ι޾Ƽ� ����
    {
        resourcePath = path;
    }

   public void InitializeGame()
    {

        pauseUI.SetActive(false);
        gameoverUI.SetActive(false);
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

        // ������ ��Ʈ���� ���� ����
        if (totalNotesPlayed == totalNotes)
        {
            totalScore = Mathf.Round(totalScore); // ���� ���� �ݿø�
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
        Debug.Log("�Ͻ�����");
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        playManager.musicSound.Pause();
        if (playManager.videoPlayer.clip != null)
            playManager.videoPlayer.Pause();
    }
    public void ResumeGame()
    {
        Debug.Log("�Ͻ����� ����");
        pauseUI.SetActive(false);
        ResumCount();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f; // �� �ε� �� Ÿ�ӽ����� �ʱ�ȭ

    }

    private void OnDestroy()
    {
        // �̺�Ʈ ����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
