
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Setting : UI_Base
{
    private bool isKeyboardAdjusting = false;

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

    private void Start()
    {
        Init();
        Managers.Instance.Input.OnKeyPressed += HandleKeyPress;
        Managers.Instance.UI.ToggleComponentInput<UI_MainMenu>(gameObject, false);
    }

    public override void Init()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Scrollbar>(typeof(Scrollbars));
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));

        // 버튼 바인딩
        GetButton((int)Buttons.BeatOffsetButton).onClick.AddListener(() =>
        {
            SelectOption(SettingOption.BeatOffset);
            ConfirmSelection();
        });
        GetButton((int)Buttons.KeySettingButton).onClick.AddListener(() =>
        {
            SelectOption(SettingOption.KeySetting);
            ConfirmSelection();
        });
        GetButton((int)Buttons.BackButton).onClick.AddListener(() =>
        {
            Managers.Instance.Sound.SaveVolumeSettings();
            ClosePopupUI();
        });

        // 스크롤바 변경 이벤트
        Get<Scrollbar>((int)Scrollbars.MasterVolumeScrollbar).onValueChanged.AddListener((value) =>
        {
            if (isKeyboardAdjusting) return;
            SelectOption(SettingOption.MasterVolume);
            Managers.Instance.Sound.SetMasterVolume(value);
            UpdateVolumeText();
        });

        Get<Scrollbar>((int)Scrollbars.BGMScrollbar).onValueChanged.AddListener((value) =>
        {
            if (isKeyboardAdjusting) return;
            SelectOption(SettingOption.BGM);
            Managers.Instance.Sound.SetBGMVolume(value);
            UpdateVolumeText();
        });

        Get<Scrollbar>((int)Scrollbars.SFXScrollbar).onValueChanged.AddListener((value) =>
        {
            if (isKeyboardAdjusting) return;
            SelectOption(SettingOption.SFX);
            Managers.Instance.Sound.SetSFXVolume(value);
            UpdateVolumeText();
        });

        // 초기값 설정
        Get<Scrollbar>((int)Scrollbars.MasterVolumeScrollbar).value = Managers.Instance.Sound.MasterVolume;
        Get<Scrollbar>((int)Scrollbars.BGMScrollbar).value = Managers.Instance.Sound.BGMVolume;
        Get<Scrollbar>((int)Scrollbars.SFXScrollbar).value = Managers.Instance.Sound.SFXVolume;

        UpdateVolumeText();
        _currentSelection = SettingOption.MasterVolume;
        UpdateOutlines();
    }

    private void HandleKeyPress(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.Escape:
                Managers.Instance.Sound.SaveVolumeSettings();
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
        int newSelection = Mathf.Clamp((int)_currentSelection + direction, 0, Enum.GetValues(typeof(SettingOption)).Length - 1);
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
        float fixedStep = 0.01f;
        isKeyboardAdjusting = true;

        switch (_currentSelection)
        {
            case SettingOption.MasterVolume:
                float mv = Mathf.Clamp01(Managers.Instance.Sound.MasterVolume + (value > 0 ? fixedStep : -fixedStep));
                Managers.Instance.Sound.SetMasterVolume(mv);
                Get<Scrollbar>((int)Scrollbars.MasterVolumeScrollbar).value = mv;
                break;

            case SettingOption.BGM:
                float bv = Mathf.Clamp01(Managers.Instance.Sound.BGMVolume + (value > 0 ? fixedStep : -fixedStep));
                Managers.Instance.Sound.SetBGMVolume(bv);
                Get<Scrollbar>((int)Scrollbars.BGMScrollbar).value = bv;
                break;

            case SettingOption.SFX:
                float sv = Mathf.Clamp01(Managers.Instance.Sound.SFXVolume + (value > 0 ? fixedStep : -fixedStep));
                Managers.Instance.Sound.SetSFXVolume(sv);
                Get<Scrollbar>((int)Scrollbars.SFXScrollbar).value = sv;
                break;
        }

        UpdateVolumeText();
        StartCoroutine(ResetKeyboardAdjusting());
    }

    private IEnumerator ResetKeyboardAdjusting()
    {
        yield return new WaitForSeconds(0.2f);
        isKeyboardAdjusting = false;
    }

    private void UpdateVolumeText()
    {
        GetText((int)Texts.MainVText).text = $"{Mathf.RoundToInt(Managers.Instance.Sound.MasterVolume * 100)}%";
        GetText((int)Texts.BGMVText).text = $"{Mathf.RoundToInt(Managers.Instance.Sound.BGMVolume * 100)}%";
        GetText((int)Texts.SFXVText).text = $"{Mathf.RoundToInt(Managers.Instance.Sound.SFXVolume * 100)}%";
    }

    private void ConfirmSelection()
    {
        if (_currentSelection == SettingOption.BeatOffset)
            OpenTimingAdjustmentScene();
        else if (_currentSelection == SettingOption.KeySetting)
            OpenKeySettingPopup();
    }

    private void OpenTimingAdjustmentScene()
    {
        Debug.Log("Opening Timing Adjustment Scene...");
    }

    private void OpenKeySettingPopup()
    {
        GameObject keySettingPanel = Resources.Load<GameObject>("Prefabs/UI/KeySettingPanel");
        if (keySettingPanel == null)
        {
            Debug.LogError("KeySettingPanel prefab not found!");
            return;
        }

        Managers.Instance.UI.ShowPopup(keySettingPanel);
    }

    private void UpdateOutlines()
    {
        for (int i = 0; i < Enum.GetValues(typeof(Images)).Length; i++)
            GetImage(i).gameObject.SetActive(false);

        GetImage((int)_currentSelection).gameObject.SetActive(true);
    }

    public override void ClosePopupUI()
    {
        Managers.Instance.Input.OnKeyPressed -= HandleKeyPress;
        Managers.Instance.UI.ToggleComponentInput<UI_MainMenu>(gameObject, true);

        EventSystem.current?.SetSelectedGameObject(null);

        Managers.Instance.UI.CloseCurrentPopup();
    }
}