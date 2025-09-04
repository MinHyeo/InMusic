using SSW;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoading : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _loadingBar;
    [SerializeField] private Text _loadingText;
    [SerializeField] private Text _progressText;
    [SerializeField] private RectTransform _fillAreaRect;
    private string _loadSceneName;
    private Coroutine dotCoroutine;
    private float _defaultLoadingText;
    private static SceneLoading _instance;
    public static SceneLoading Instance
    {
        get
        {
            if (_instance == null)
            {
                SceneLoading instance = FindFirstObjectByType<SceneLoading>();
                if (instance != null)
                {
                    _instance = instance;
                }
                else
                {
                    _instance = CreateInstance();
                }
            }
            return _instance;
        }
    }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (_instance != this) {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    private static SceneLoading CreateInstance() {
        return Instantiate(Resources.Load<SceneLoading>("SSW/UI/Prefabs/Loading_Canvas"));
    }

    public void LoadScene(string sceneName)
    {
        gameObject.SetActive(true);
        if (dotCoroutine != null) {
            StopCoroutine(dotCoroutine);
            dotCoroutine = null;
        }

        _loadingBar.fillAmount = 0f;
        _progressText.text = "0%";
        _loadingText.text = "Loading";
        _defaultLoadingText = _loadingText.rectTransform.anchoredPosition.x;

        GlobalInputControl.IsInputEnabled = false;
        _loadSceneName = sceneName;
        StartCoroutine(LoadSceneProcess());
    }

    private IEnumerator LoadSceneProcess()
    {
        yield return StartCoroutine(Fade(true));

        dotCoroutine = StartCoroutine(Animate_LoadingText());

        AsyncOperation operation = SceneManager.LoadSceneAsync(_loadSceneName);
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
                    GlobalInputControl.IsInputEnabled = true;
                    gameObject.SetActive(false);
                    yield break;
                }
            }
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(arg0.name == _loadSceneName) {
            StartCoroutine(Fade(false));
            SceneManager.sceneLoaded -= OnSceneLoaded;

            if (dotCoroutine != null) {
                StopCoroutine(dotCoroutine);
                dotCoroutine = null;
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
        }
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
        _progressText.text = Mathf.RoundToInt(percentage) + "%";
        
        float fillWidth = _fillAreaRect.rect.width * _loadingBar.fillAmount;


        Vector2 pos = _progressText.rectTransform.anchoredPosition;
        pos.x = fillWidth;
        _progressText.rectTransform.anchoredPosition = pos;
    }
}
