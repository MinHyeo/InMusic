using UnityEngine;
using UnityEngine.UI;

public class SettingPanelManager : MonoBehaviour
{
    private enum SettingOption
    {
        MasterVolume,
        SFX,
        BGM,
        BeatOffset,
        KeySetting
    }

    [Header("UI Elements")]
    public Scrollbar masterVolumeScrollbar;
    public Scrollbar sfxScrollbar;
    public Scrollbar bgmScrollbar;
    public GameObject beatOffsetPanel;
    public GameObject keySettingPanel;
    public Button backButton;

    [Header("Outlines")]
    public GameObject[] outlines; // ���õ� �ɼ��� �׵θ� �̹��� (�̸� ����)

    private SettingOption currentSelection = SettingOption.MasterVolume;
    private float scrollStep = 0.05f; // ��ũ�� ���� ����

    void Start()
    {
        UpdateOutlines(); // ���� �� �׵θ� �ʱ�ȭ
    }

    void Update()
    {
        HandleNavigation();
        HandleSelection();
    }

    private void HandleNavigation()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveSelection(-1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveSelection(1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            AdjustScrollbar(-scrollStep);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            AdjustScrollbar(scrollStep);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            backButton.onClick.Invoke(); // Esc Ű�� ������ �ڷ� ���� ��ư �ڵ� ����
        }
    }

    private void HandleSelection()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            switch (currentSelection)
            {
                case SettingOption.BeatOffset:
                    beatOffsetPanel.SetActive(true);
                    break;
                case SettingOption.KeySetting:
                    keySettingPanel.SetActive(true);
                    break;
            }
        }
    }

    private void MoveSelection(int direction)
    {
        currentSelection += direction;

        if (currentSelection < SettingOption.MasterVolume)
            currentSelection = SettingOption.KeySetting;
        else if (currentSelection > SettingOption.KeySetting)
            currentSelection = SettingOption.MasterVolume;

        UpdateOutlines(); // �׵θ� ������Ʈ
        Debug.Log($"���� ����: {currentSelection}");
    }

    private void AdjustScrollbar(float value)
    {
        switch (currentSelection)
        {
            case SettingOption.MasterVolume:
                masterVolumeScrollbar.value = Mathf.Clamp01(masterVolumeScrollbar.value + value);
                break;
            case SettingOption.BGM:
                bgmScrollbar.value = Mathf.Clamp01(bgmScrollbar.value + value);
                break;
            case SettingOption.SFX:
                sfxScrollbar.value = Mathf.Clamp01(sfxScrollbar.value + value);
                break;
            default:
                Debug.Log("�� �׸񿡼��� ��ũ�ѹ� ���� �Ұ���");
                break;
        }
    }

    private void UpdateOutlines()
    {
        for (int i = 0; i < outlines.Length; i++)
        {
            outlines[i].SetActive(i == (int)currentSelection); // ���� ���õ� �ɼǿ� �°� �׵θ� Ȱ��ȭ
        }
    }
}
