using UnityEngine;

public class SettingButton : MonoBehaviour
{
    public GameObject KeySettingPanel;
    public GameObject TimeSettingPanel;


    public void OpenKeySettingPanel()
    {
        KeySettingPanel.SetActive(true);
    }

    public void OpneTimeSettingPanel()
    {
        TimeSettingPanel.SetActive(true);
    }



}