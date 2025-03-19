using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_Setting : UI_Base
{
    private static UI_Setting _instance;
    private bool isKeyboardAdjusting = false; // 키보드 입력 중인지 확인하는 변수

    private enum Images
    {
        MasterVolumeBack,
        SFXVolumeBack,
        BGMVolumeBack,
        BeatOffsetBack,
        KeySettingBack
    }

    private enum SettingOption
    {
        MasterVolume,
        SFX,
        BGM,
        BeatOffset,
        KeySetting
    }

    private enum Scrollbars
    {
        MasterVolumeScrollbar,
        BGMScrollbar,
        SFXScrollbar
    }

    private enum Buttons
    {
        BeatOffsetButton,
        KeySettingButton,
        BackButton
    }

    private enum Texts
    {
        MainVText,
        SFXVText,
        BGMVText
    }

    private SettingOption _currentSelection;
    private float _scrollStep = 0.01f;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void Start()
    {
        Init();
        Managers.Input.OnKeyPressed += HandleKeyPress;
        UIManager.ToggleComponentInput<UI_MainMenu>(this.gameObject, false);
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Scrollbar>(typeof(Scrollbars));
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));

        GetButton((int)Buttons.BeatOffsetButton).onClick.AddListener(() => { SelectOption(SettingOption.BeatOffset); ConfirmSelection(); });
        GetButton((int)Buttons.KeySettingButton).onClick.AddListener(() => { SelectOption(SettingOption.KeySetting); ConfirmSelection(); });
        GetButton((int)Buttons.BackButton).onClick.AddListener(() => { SoundManager.Instance.SaveVolumeSettings(); ClosePopupUI(); });

        Get<Scrollbar>((int)Scrollbars.MasterVolumeScrollbar).onValueChanged.AddListener((value) =>
        {
            if (isKeyboardAdjusting) return; // 키보드 조정 중이면 무시
            SelectOption(SettingOption.MasterVolume);
            SoundManager.Instance.SetMasterVolume(value);
            UpdateVolumeText();
        });

        Get<Scrollbar>((int)Scrollbars.BGMScrollbar).onValueChanged.AddListener((value) =>
        {
            if (isKeyboardAdjusting) return; // 키보드 조정 중이면 무시
            SelectOption(SettingOption.BGM);
            SoundManager.Instance.SetBGMVolume(value);
            UpdateVolumeText();
        });

        Get<Scrollbar>((int)Scrollbars.SFXScrollbar).onValueChanged.AddListener((value) =>
        {
            if (isKeyboardAdjusting) return; // 키보드 조정 중이면 무시
            SelectOption(SettingOption.SFX);
            SoundManager.Instance.SetSFXVolume(value);
            UpdateVolumeText();
        });

        // 스크롤바 초기값 로드
        Get<Scrollbar>((int)Scrollbars.MasterVolumeScrollbar).value = SoundManager.Instance.MasterVolume;
        Get<Scrollbar>((int)Scrollbars.BGMScrollbar).value = SoundManager.Instance.BGMVolume;
        Get<Scrollbar>((int)Scrollbars.SFXScrollbar).value = SoundManager.Instance.SFXVolume;

        UpdateVolumeText();
        _currentSelection = SettingOption.MasterVolume;
        UpdateOutlines();
    }

    private void HandleKeyPress(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Escape:
                SoundManager.Instance.SaveVolumeSettings();
                ClosePopupUI();
                break;
            case KeyCode.UpArrow:
                MoveSelection(-1);
                break;
            case KeyCode.DownArrow:
                MoveSelection(1);
                break;
            case KeyCode.LeftArrow:
                AdjustScrollbar(-_scrollStep);
                break;
            case KeyCode.RightArrow:
                AdjustScrollbar(_scrollStep);
                break;
            case KeyCode.Return:
                ConfirmSelection();
                break;
        }
    }

    private void MoveSelection(int direction)
    {
        int newSelection = (int)_currentSelection + direction;
        newSelection = Mathf.Clamp(newSelection, 0, Enum.GetValues(typeof(SettingOption)).Length - 1);
        _currentSelection = (SettingOption)newSelection;
        UpdateOutlines();
    }

    private void SelectOption(SettingOption option)
    {
        _currentSelection = option;
        UpdateOutlines();
    }

    private void AdjustScrollbar(float value)
    {
        float fixedStep = 0.01f; // 항상 0.01씩 변경
        isKeyboardAdjusting = true; // 키보드 조정 활성화

        switch (_currentSelection)
        {
            case SettingOption.MasterVolume:
                float newMasterVolume = Mathf.Clamp01(SoundManager.Instance.MasterVolume + (value > 0 ? fixedStep : -fixedStep));
                SoundManager.Instance.SetMasterVolume(newMasterVolume);
                Get<Scrollbar>((int)Scrollbars.MasterVolumeScrollbar).value = newMasterVolume;
                break;

            case SettingOption.BGM:
                float newBGMVolume = Mathf.Clamp01(SoundManager.Instance.BGMVolume + (value > 0 ? fixedStep : -fixedStep));
                SoundManager.Instance.SetBGMVolume(newBGMVolume);
                Get<Scrollbar>((int)Scrollbars.BGMScrollbar).value = newBGMVolume;
                break;

            case SettingOption.SFX:
                float newSFXVolume = Mathf.Clamp01(SoundManager.Instance.SFXVolume + (value > 0 ? fixedStep : -fixedStep));
                SoundManager.Instance.SetSFXVolume(newSFXVolume);
                Get<Scrollbar>((int)Scrollbars.SFXScrollbar).value = newSFXVolume;
                break;
        }

        UpdateVolumeText();

        // 일정 시간 후 isKeyboardAdjusting을 다시 false로 설정
        StartCoroutine(ResetKeyboardAdjusting());
    }

    private IEnumerator ResetKeyboardAdjusting()
    {
        yield return new WaitForSeconds(0.2f);
        isKeyboardAdjusting = false; // 키보드 입력 해제
    }

    private void UpdateVolumeText()
    {
        Get<Text>((int)Texts.MainVText).text = Mathf.RoundToInt(SoundManager.Instance.MasterVolume * 100) + "%";
        Get<Text>((int)Texts.BGMVText).text = Mathf.RoundToInt(SoundManager.Instance.BGMVolume * 100) + "%";
        Get<Text>((int)Texts.SFXVText).text = Mathf.RoundToInt(SoundManager.Instance.SFXVolume * 100) + "%";
    }

    private void ConfirmSelection()
    {
        if (_currentSelection == SettingOption.BeatOffset) OpenTimingAdjustmentScene();
        else if (_currentSelection == SettingOption.KeySetting) OpenKeySettingPopup();
    }

    private void OpenTimingAdjustmentScene()
    {
        Debug.Log("Opening Timing Adjustment Scene");
    }

    private void OpenKeySettingPopup()
    {
        GameObject keySettingPanel = Resources.Load<GameObject>("Prefabs/UI/KeySettingPanel");
        if (keySettingPanel == null)
        {
            Debug.LogError("KeySettingPanel prefab not found!");
            return;
        }
        Managers.UI.ShowPopup(keySettingPanel);
    }

    private void UpdateOutlines()
    {
        for (int i = 0; i < Enum.GetValues(typeof(Images)).Length; i++)
        {
            Get<Image>(i).gameObject.SetActive(false);
        }
        Get<Image>((int)_currentSelection).gameObject.SetActive(true);
    }

    public override void ClosePopupUI()
    {
        if (_instance == this)
        {
            Managers.Input.OnKeyPressed -= HandleKeyPress;
            UIManager.ToggleComponentInput<UI_MainMenu>(this.gameObject, true);
            _instance = null;
            Destroy(gameObject);
        }
    }
}
