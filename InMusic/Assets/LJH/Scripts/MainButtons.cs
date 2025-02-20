using UnityEngine;

public class MainButtons : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject SettingPanel;
    public GameObject keyGuidePanel;
    public GameObject keySettingPanel;

    public GameObject mainCanvas;
    public GameObject soloCanvas;
    public GameObject multiCanvas;

    private void Start()
    {
        // 처음에는 설정 및 키가이드 패널 비활성화
        MainPanel.SetActive(true);
        SettingPanel.SetActive(false);
        keyGuidePanel.SetActive(false);
        keySettingPanel.SetActive(false);
    }

    private void Update()
    {
        // F10을 누르면 옵션 패널 열기/닫기
        if (Input.GetKeyDown(KeyCode.F10))
        {
            TogglePanel(SettingPanel);
        }

        // F1을 누르면 키가이드 패널 열기/닫기
        if (Input.GetKeyDown(KeyCode.F1))
        {
            TogglePanel(keyGuidePanel);
        }
    }

    public void OpenSettingPanel()
    {
        SettingPanel.SetActive(true);
        MainPanel.SetActive(false);
    }

    public void OpenKeyGuidePanel()
    {
        keyGuidePanel.SetActive(true);
        MainPanel.SetActive(false);
    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
        MainPanel.SetActive(true);
    }

    public void OpenKeySettingPanel()
    {
        keySettingPanel.SetActive(true);
    }

    public void OpenSoloCanvas()
    {
        mainCanvas.SetActive(false);
        soloCanvas.SetActive(true);
    }

    public void OpenMultiCanvas()
    {
        mainCanvas.SetActive(false);
        multiCanvas.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Game Exiting...");
    }

    private void TogglePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }
}
