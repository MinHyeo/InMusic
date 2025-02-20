using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public GameObject loadingPanel; // 로딩 UI 패널
    public Slider progressBar; // 로딩 바 (슬라이더)
    private BMSData musicBMS;
    private LodingUI loadingUI;
    private MusicData musicData;

    public void LoadScene(string sceneName)
    {
        loadingPanel.SetActive(true); // 로딩 UI 활성화
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public void LoadScene(string sceneName, BMSData data)
    {
        loadingPanel.SetActive(true); // 로딩 UI 활성화
        SetLodingScreen(data);
        StartCoroutine(LoadSceneAsync(sceneName));
    }
    public void LoadScene(string sceneName, MusicData data)
    {
        loadingPanel.SetActive(true); // 로딩 UI 활성화
        SetLodingScreen(data);
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // 씬 전환을 수동으로 제어

        float displayedProgress = 0f;

        while (!operation.isDone)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f); // 로딩 진행도 (0~1)

            if (targetProgress < 0.9f)
            {
                // 로딩 중: 90%까지 로딩 바 업데이트
                displayedProgress = targetProgress;
                loadingUI.loadingRate.text = $"{Mathf.RoundToInt(displayedProgress * 100)}%";
            }
            else
            {
                // 로딩 완료 상태: 90%에서 100%까지 서서히 증가 (1초 동안)
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

                // 100%가 된 후 1초 대기 후 씬 전환
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
