using System;
using System.Collections;
using Play;
using SSW;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class LoadingSong : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _loadingBar;
    [SerializeField] private Text _loadingText;
    [SerializeField] private Text progressText;
    [SerializeField] private RectTransform _fillAreaRect;

    [Header("UI Settings")]
    [SerializeField] private Image _songImage;
    [SerializeField] private Text _songTitle;
    [SerializeField] private Text _songArtist;

    [Header("Background Settings")]
    [SerializeField] private BackgroundController _bgController;
    [SerializeField] private VideoPlayer _bgVideo;
    [SerializeField] private AudioSource _bgAudio;
    private string loadSceneName;
    private Coroutine dotCoroutine;
    private float _defaultLoadingText;
    private Song songTitle;
    private string artist;
    private static LoadingSong _instance;

    private bool isDataReady = false;
    
    public static LoadingSong Instance {
        get {
            if (_instance == null) {
                LoadingSong instance = FindFirstObjectByType<LoadingSong>();
                if(instance == null) {
                    Debug.LogError("LoadingSong이 씬에 배치되어 있지 않음");
                } else {
                    _instance = instance;
                }
            }
            return _instance;
        }
    }

    private void Awake() {
        if (_instance == null) {
        _instance = this;
        DontDestroyOnLoad(gameObject);
        _bgController = BackgroundController.Instance;

        } else if (_instance != this) {
            Destroy(gameObject);
            return;
        }
    }

    private void Start() {
        // 시작할 때 투명/클릭불가 처리
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }


    /// <summary>
    /// 외부에서 호출하여 씬을 로드하는 함수
    /// </summary>
    public void LoadPlay(string sceneName, string SongTitle, string Artist, Sprite songSprite) {
        gameObject.SetActive(true);
        Enum.TryParse(SongTitle, out songTitle);
        artist = Artist;

        if (dotCoroutine != null) {
            StopCoroutine(dotCoroutine);
            dotCoroutine = null;
        }

        _loadingBar.fillAmount = 0f;
        progressText.text = "0%";
        _loadingText.text = "Loading";
        _songImage.sprite = songSprite;
        _songTitle.text = SongTitle;
        _songArtist.text = Artist;
        _defaultLoadingText = _loadingText.rectTransform.anchoredPosition.x;
        GlobalInputControl.IsInputEnabled = false;
        _bgController = BackgroundController.Instance;
        _bgAudio = _bgController.GetAudioSource();
        _bgVideo = _bgController.GetVideoPlayer();
        BackgroundController.Instance.StopHighlight();

        SceneManager.sceneLoaded += OnSceneLoaded;
        loadSceneName = sceneName;
        StartCoroutine(LoadSceneProcess(songTitle));
    }

    private IEnumerator LoadSceneProcess(Song songTitle)
    {
        yield return StartCoroutine(Fade(true));

        dotCoroutine = StartCoroutine(Animate_LoadingText());

        AsyncOperation operation = SceneManager.LoadSceneAsync(loadSceneName);
        operation.allowSceneActivation = false; // 씬 로딩 끝나도 자동 전환 x

        float timer = 0f;
        while(!operation.isDone) {
            yield return null;
            UpdateProgressTextAndPosition();

            if(operation.progress < 0.87f) {
                _loadingBar.fillAmount = operation.progress;
            } else {
                timer += Time.unscaledDeltaTime / 3;
                _loadingBar.fillAmount = Mathf.Lerp(0.87f, 1f, timer);

                if(_loadingBar.fillAmount >= 1f) {
                    operation.allowSceneActivation = true;

                    //씬 활성화될 때까지 대기
                    yield return new WaitUntil(() => PlayManager.Instance != null);
                    Debug.Log("PlayManager 인스턴스 확인");

                    yield return StartCoroutine(WaitForPlayManagerAndStartGame());

                    GlobalInputControl.IsInputEnabled = true;

                    //게임 시작
                    PlayManager.Instance.StartGame();
                    yield break;
                }
            }
        }
    }

    public IEnumerator WaitForPlayManagerAndStartGame()
    {
        Debug.Log("PlayManager 불러오기 시작");
        // PlayManager가 존재할 때까지 대기
        while (PlayManager.Instance == null)
        {
            yield return null;  // 다음 프레임까지 대기
            Debug.Log("PlayManager가 안 불러와짐");
        }
        Debug.Log("PlayManager 불러오기 완");

        PlayManager.Instance.Init(songTitle, artist);
        yield return new WaitUntil(() => PlayManager.Instance.IsDataLoaded());
        Debug.Log("준비 완");

        isDataReady = true;
        // PlayManager가 초기화되면 메서드 호출
        //PlayManager.Instance.StartGame(songTitle, artist);
    }
    //>


    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(arg0.name == loadSceneName) {
            StartCoroutine(Fade(false));
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (dotCoroutine != null) {
                StopCoroutine(dotCoroutine);
                dotCoroutine = null;
                isDataReady = false;
            }
        }
    }

    private IEnumerator Fade(bool isFadeIn)
    {
        if (isFadeIn)
        {
            // 보이게 할 때는 우선 인터랙션/레이캐스트 허용
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
        
        float timer = 0f;
        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime; // unscaledDeltaTime: The timeScale-independent interval in seconds from the last frame to the current one
            _canvasGroup.alpha = isFadeIn ? Mathf.Lerp(0, 1, timer) : Mathf.Lerp(1, 0, timer);
            //_bgAudio.volume = Mathf.Lerp(_bgAudio.volume, 0f, timer);
            //_bgVideo.Pause();
        }

        if (!isFadeIn)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
        Play.SoundManager.Instance.End();
    }

    /// <summary>
    /// "Loading"에 점(`.`)을 1개~3개까지 반복해서 붙이는 코루틴
    /// </summary>
    private IEnumerator Animate_LoadingText()
    {
        int dotCount = 0;
        while (true)
        {
            // 1 ~ 3개 점 반복
            dotCount = (dotCount % 3) + 1;
            _loadingText.text = "Loading" + new string('.', dotCount);
            yield return new WaitForSeconds(0.4f); 
        }
    }

    /// <summary>
    /// 진행도(%) 텍스트 갱신 및 텍스트 위치 이동
    /// </summary>
    private void UpdateProgressTextAndPosition()
    {
        // 퍼센트 표기 (예: "75%")
        float percentage = _loadingBar.fillAmount * 100f;
        progressText.text = Mathf.RoundToInt(percentage) + "%";
        
        float fillWidth = _fillAreaRect.rect.width * _loadingBar.fillAmount;


        Vector2 pos = progressText.rectTransform.anchoredPosition;
        pos.x = fillWidth;
        progressText.rectTransform.anchoredPosition = pos;
    }
}
