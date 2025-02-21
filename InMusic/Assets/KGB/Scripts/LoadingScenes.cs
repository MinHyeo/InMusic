using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public GameObject loadingPanel; // �ε� UI �г�
    public Slider progressBar; // �ε� �� (�����̴�)
    [SerializeField] GameObject inMusic;
    [SerializeField] GameObject musicInfoPanel;
    [SerializeField] GameObject loadingBarUI;
    private BMSData musicBMS;
    private LodingUI loadingUI;
    private MusicData musicData;

    private void Start()
    {
        loadingUI = GetComponent<LodingUI>();
    }
    public void LoadScene(string sceneName) //�⺻ �ε�ȭ��
    {
        loadingPanel.SetActive(true); // �ε� UI Ȱ��ȭ
        inMusic.SetActive(true);
        musicInfoPanel.SetActive(false);
        SetLoadingBarPos(0);
        StartCoroutine(LoadSceneAsync(sceneName));
    }
    public void LoadScene(string sceneName, MusicData data) //�÷��̾� ���� ���� �ε�ȭ�� 
    {
        loadingPanel.SetActive(true); // �ε� UI Ȱ��ȭ
        inMusic.SetActive(false);
        SetLoadingBarPos(-430);

        SetLodingScreen(data);
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // �� ��ȯ�� �������� ����

        float displayedProgress = 0f;

        while (!operation.isDone)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f); // �ε� ���൵ (0~1)

            if (targetProgress < 0.9f)
            {
                // �ε� ��: 90%���� �ε� �� ������Ʈ
                displayedProgress = targetProgress;
                loadingUI.loadingRate.text = $"{Mathf.RoundToInt(displayedProgress * 100)}%";
            }
            else
            {
                // �ε� �Ϸ� ����: 90%���� 100%���� ������ ���� (1�� ����)
                float elapsed = 0f;
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
}
