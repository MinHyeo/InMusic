using UnityEngine;

public class MainButtons: MonoBehaviour
{
    public GameObject SettingPanel;
    public GameObject keyGuidePanel;
    public GameObject mainCanvas;
    public GameObject soloCanvas;
    public GameObject multiCanvas;

    public void OpenSettingPanel()
    {
        SettingPanel.SetActive(true);
    }

    // Open KeyGuide Panel
    public void OpenKeyGuidePanel()
    {
        keyGuidePanel.SetActive(true);
    }

    // ClosePannel
    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    // Open SoloPLay Canvas
    public void OpenSoloCanvas()
    {
        mainCanvas.SetActive(false); 
        soloCanvas.SetActive(true);
    }

    // Open MultiPLay Canvas
    public void OpenMultiCanvas()
    {
        mainCanvas.SetActive(false); 
        multiCanvas.SetActive(true);
    }

    // Quit
    public void QuitGame()
    {
        Debug.Log("Game Exiting...");
    }
}
