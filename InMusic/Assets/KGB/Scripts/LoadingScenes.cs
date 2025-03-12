using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;

    public GameObject loadingPanel; // �ε� UI �г�
    public Slider progressBar; // �ε� �� (�����̴�)
    [SerializeField] GameObject inMusic;
    [SerializeField] GameObject musicInfoPanel;
    [SerializeField] GameObject loadingBarUI;
    [SerializeField] Sprite backImage;
    private BMSData musicBMS;
    private LodingUI loadingUI;
    private MusicData musicData;

    private CanvasGroup canvasGroup; // ���̵� ȿ���� ���� CanvasGroup

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

        // `CanvasGroup` �������ų� ������ �ڵ� �߰�
        canvasGroup = loadingPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = loadingPanel.AddComponent<CanvasGroup>();
        }
        loadingUI = GetComponent<LodingUI>();

        // ���� �ε�� �Ŀ� ���̵�ƿ��� �����ϱ� ���� �̺�Ʈ ���
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadScene(string sceneName) // �⺻ �ε� ȭ��
    {
        StartCoroutine(LoadSceneProcess(sceneName, null));
    }

    public void LoadScene(string sceneName, MusicData data) // �÷��� ������ ���� �ε� ȭ��
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
        yield return StartCoroutine(FadeIn()); // �ε� ȭ�� ���̵� ��
        loadingPanel.SetActive(true); // �ε� UI Ȱ��ȭ

        

        yield return StartCoroutine(LoadSceneAsync(sceneName)); // �� �ε� ����
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

        // 0%���� 1.5�� ���
        yield return new WaitForSeconds(1.5f);

        // 0~10%���� 2�� ���� ����
        loadingStartTime = Time.time; // �ٽ� ���� �ð� �ʱ�ȭ
        while (displayedProgress < 0.1f)
        {
            displayedProgress = (Time.time - loadingStartTime) / 2f; // 2�ʿ� ���� ����
            displayedProgress = Mathf.Clamp01(displayedProgress); // 0~1 ���̷�
            progressBar.value = displayedProgress;
            loadingUI.loadingRate.text = $"{Mathf.RoundToInt(displayedProgress * 100)}%";
            yield return null;
        }

        // ���� ���൵ ������Ʈ
        while (!operation.isDone)
        {
            Debug.Log(operation.progress.ToString());
            if (operation.progress>=1){
                break;
            }
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f); // �ε� ���൵ (0~1)

            if (targetProgress < 0.9f)
            {
                // �ε� ��: 10%���� 90%���� ���൵ ������Ʈ
                displayedProgress = Mathf.Lerp(0.1f, targetProgress, Time.time - loadingStartTime); // 10%���� ����
                loadingUI.loadingRate.text = $"{Mathf.RoundToInt(displayedProgress * 100)}%";
            }
            else
            {
                // �ε� �Ϸ� ����: 90%���� 100%���� ������ ���� (1�� ����)

                while (elapsed < 1f)
                {
                    elapsed += Time.deltaTime;
                    displayedProgress = Mathf.Lerp(0.9f, 1f, elapsed / 1f);
                    progressBar.value = displayedProgress;
                    loadingUI.loadingRate.text = $"{Mathf.RoundToInt(displayedProgress * 100)}%";
                    loadingUI.UpdateTextPosition(displayedProgress);
                    yield return null;
                }

                // 100%�� �� �� 1�� ��� �� �� ��ȯ
                yield return new WaitForSeconds(1f);
                operation.allowSceneActivation = true;
            }

            progressBar.value = displayedProgress;
            yield return null;
        }
    }

    // ���� ����Ǹ� �ڵ����� ȣ��Ǵ� �Լ�
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //StartCoroutine(FadeOut()); // ���ο� ������ ���̵�ƿ� ����
        Debug.Log("�� �ε��");
        if (scene.name == "Single_Lobby_PSH")
        {
            Debug.Log("�̱� �κ� ��");
            return;
        }
        else if(scene.name == "KGB_SinglePlay")
        {
            Debug.Log("�÷��� ��");
            return;
        }
        else
        {
            Debug.Log("�ٷ� �� �غ� ��");
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
        Debug.Log("���̵� ��");
        canvasGroup.alpha = 0;
        loadingPanel.SetActive(true);

        float duration = 0.5f; // ���̵��� �ð� (0.5��)
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
        Debug.Log("���̵� �ƿ�");
        yield return new WaitForSeconds(0.5f); // ���ο� ������ �ణ ��ٸ� �� ���̵� �ƿ� ����
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * 2; // 0.5�� ���� �ɷ��� �����
            yield return null;
        }
        canvasGroup.alpha = 0;
        progressBar.value = 0;
        loadingUI.loadingRate.text = "0%";
        loadingUI.UpdateTextPosition(0);
        loadingPanel.SetActive(false); // ������ ������� UI ��Ȱ��ȭ
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SceneReady()
    {
        Debug.Log("�� �غ� �Ϸ�");
        StartCoroutine(FadeOut());
    }
}
