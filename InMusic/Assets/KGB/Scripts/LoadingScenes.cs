using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public GameObject loadingPanel; // �ε� UI �г�
    public Slider progressBar; // �ε� �� (�����̴�)
    private BMSData musicBMS;
    private LodingUI loadingUI;
    private MusicData musicData;

    public void LoadScene(string sceneName)
    {
        loadingPanel.SetActive(true); // �ε� UI Ȱ��ȭ
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public void LoadScene(string sceneName, BMSData data)
    {
        loadingPanel.SetActive(true); // �ε� UI Ȱ��ȭ
        SetLodingScreen(data);
        StartCoroutine(LoadSceneAsync(sceneName));
    }
    public void LoadScene(string sceneName, MusicData data)
    {
        loadingPanel.SetActive(true); // �ε� UI Ȱ��ȭ
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

    private void SetLodingScreen(BMSData data)
    {
        loadingUI = GetComponent<LodingUI>();
        loadingUI.artistText.text = data.header.artist;
        loadingUI.titleText.text = data.header.title;
    }
    private void SetLodingScreen(MusicData data)
    {
        loadingUI = GetComponent<LodingUI>();
        loadingUI.artistText.text = data.BMS.header.artist;
        loadingUI.titleText.text = data.BMS.header.title;
        loadingUI.musicImage.sprite = data.Album;
    }
}
