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
    public GameObject[] outlines; // 선택된 옵션의 테두리 이미지 (미리 세팅)

    private SettingOption currentSelection = SettingOption.MasterVolume;
    private float scrollStep = 0.05f; // 스크롤 조정 단위

    void Start()
    {
        UpdateOutlines(); // 시작 시 테두리 초기화
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
            backButton.onClick.Invoke(); // Esc 키를 누르면 뒤로 가기 버튼 자동 실행
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

        UpdateOutlines(); // 테두리 업데이트
        Debug.Log($"현재 선택: {currentSelection}");
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
                Debug.Log("이 항목에서는 스크롤바 조정 불가능");
                break;
        }
    }

    private void UpdateOutlines()
    {
        for (int i = 0; i < outlines.Length; i++)
        {
            outlines[i].SetActive(i == (int)currentSelection); // 현재 선택된 옵션에 맞게 테두리 활성화
        }
    }
}
