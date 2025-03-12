using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;

    public GameObject loadingPanel; // 로딩 UI 패널
    public Slider progressBar; // 로딩 바 (슬라이더)
    [SerializeField] GameObject inMusic;
    [SerializeField] GameObject musicInfoPanel;
    [SerializeField] GameObject loadingBarUI;
    [SerializeField] Sprite backImage;
    private BMSData musicBMS;
    private LodingUI loadingUI;
    private MusicData musicData;

    private CanvasGroup canvasGroup; // 페이드 효과를 위한 CanvasGroup

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // `CanvasGroup` 가져오거나 없으면 자동 추가
        canvasGroup = loadingPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = loadingPanel.AddComponent<CanvasGroup>();
        }
        loadingUI = GetComponent<LodingUI>();

        // 씬이 로드된 후에 페이드아웃을 실행하기 위해 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadScene(string sceneName) // 기본 로딩 화면
    {
        StartCoroutine(LoadSceneProcess(sceneName, null));
    }

    public void LoadScene(string sceneName, MusicData data) // 플레이 씬으로 가는 로딩 화면
    {
        StartCoroutine(LoadSceneProcess(sceneName, data));
    }

    private IEnumerator LoadSceneProcess(string sceneName, MusicData data)
    {
        if (data != null)
        {
            inMusic.SetActive(false);
            musicInfoPanel.SetActive(true);
            SetLoadingBarPos(-430);
            SetLodingScreen(data);

        }
        else
        {
            loadingPanel.GetComponent<Image>().sprite = backImage;
            inMusic.SetActive(true);
            musicInfoPanel.SetActive(false);
            SetLoadingBarPos(0);
        }
        yield return StartCoroutine(FadeIn()); // 로딩 화면 페이드 인
        loadingPanel.SetActive(true); // 로딩 UI 활성화

        

        yield return StartCoroutine(LoadSceneAsync(sceneName)); // 씬 로드 시작
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        float elapsed = 0f;
        float displayedProgress = 0f;
        progressBar.value = displayedProgress;
        loadingUI.loadingRate.text = $"{Mathf.RoundToInt(displayedProgress * 100)}%";
        loadingUI.UpdateTextPosition(displayedProgress);
        float loadingStartTime = Time.time;

        // 0%에서 1.5초 대기
        yield return new WaitForSeconds(1.5f);

        // 0~10%까지 2초 동안 증가
        loadingStartTime = Time.time; // 다시 시작 시간 초기화
        while (displayedProgress < 0.1f)
        {
            displayedProgress = (Time.time - loadingStartTime) / 2f; // 2초에 걸쳐 증가
            displayedProgress = Mathf.Clamp01(displayedProgress); // 0~1 사이로
            progressBar.value = displayedProgress;
            loadingUI.loadingRate.text = $"{Mathf.RoundToInt(displayedProgress * 100)}%";
            yield return null;
        }

        // 이후 진행도 업데이트
        while (!operation.isDone)
        {
            Debug.Log(operation.progress.ToString());
            if (operation.progress>=1){
                break;
            }
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f); // 로딩 진행도 (0~1)

            if (targetProgress < 0.9f)
            {
                // 로딩 중: 10%에서 90%까지 진행도 업데이트
                displayedProgress = Mathf.Lerp(0.1f, targetProgress, Time.time - loadingStartTime); // 10%에서 진행
                loadingUI.loadingRate.text = $"{Mathf.RoundToInt(displayedProgress * 100)}%";
            }
            else
            {
                // 로딩 완료 상태: 90%에서 100%까지 서서히 증가 (1초 동안)

                while (elapsed < 1f)
                {
                    elapsed += Time.deltaTime;
                    displayedProgress = Mathf.Lerp(0.9f, 1f, elapsed / 1f);
                    progressBar.value = displayedProgress;
                    loadingUI.loadingRate.text = $"{Mathf.RoundToInt(displayedProgress * 100)}%";
                    loadingUI.UpdateTextPosition(displayedProgress);
                    yield return null;
                }

                // 100%가 된 후 1초 대기 후 씬 전환
                yield return new WaitForSeconds(1f);
                operation.allowSceneActivation = true;
            }

            progressBar.value = displayedProgress;
            yield return null;
        }
    }

    // 씬이 변경되면 자동으로 호출되는 함수
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //StartCoroutine(FadeOut()); // 새로운 씬에서 페이드아웃 실행
        Debug.Log("씬 로드됨");
        if (scene.name == "Single_Lobby_PSH")
        {
            Debug.Log("싱글 로비 씬");
            return;
        }
        else if(scene.name == "KGB_SinglePlay")
        {
            Debug.Log("플레이 씬");
            return;
        }
        else
        {
            Debug.Log("바로 씬 준비 끝");
            StartCoroutine(FadeOut());
        }
        
    }

    private void SetLodingScreen(MusicData data)
    {
        loadingUI.artistText.text = data.BMS.header.artist;
        loadingUI.titleText.text = data.BMS.header.title;
        loadingUI.musicImage.sprite = data.Album;
    }

    private void SetLoadingBarPos(float newY)
    {
        Vector3 newPosition = loadingBarUI.transform.localPosition;
        newPosition.y = newY;
        loadingBarUI.transform.localPosition = newPosition;
    }

    private IEnumerator FadeIn()
    {
        Debug.Log("페이드 인");
        canvasGroup.alpha = 0;
        loadingPanel.SetActive(true);

        float duration = 0.5f; // 페이드인 시간 (0.5초)
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    private IEnumerator FadeOut()
    {
        Debug.Log("페이드 아웃");
        yield return new WaitForSeconds(0.5f); // 새로운 씬에서 약간 기다린 후 페이드 아웃 시작
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * 2; // 0.5초 정도 걸려서 사라짐
            yield return null;
        }
        canvasGroup.alpha = 0;
        progressBar.value = 0;
        loadingUI.loadingRate.text = "0%";
        loadingUI.UpdateTextPosition(0);
        loadingPanel.SetActive(false); // 완전히 사라지면 UI 비활성화
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SceneReady()
    {
        Debug.Log("씬 준비 완료");
        StartCoroutine(FadeOut());
    }
}
