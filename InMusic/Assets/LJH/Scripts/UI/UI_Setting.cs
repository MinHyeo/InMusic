using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Setting : UI_Base
{
    private static UI_Setting _instance;

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

        // UI_MainMenu의 키 입력 해제
        UIManager.ToggleComponentInput<UI_MainMenu>(this.gameObject, false);
    }

    public override void Init()
    {
        //?
        Bind<Button>(typeof(Buttons));
        Bind<Scrollbar>(typeof(Scrollbars));
        Bind<Image>(typeof(Images));

        // 버튼에 ConfirmSelection 추가
        GetButton((int)Buttons.BeatOffsetButton).onClick.AddListener(() =>
        {
            SelectOption(SettingOption.BeatOffset);
            ConfirmSelection(); // 클릭 시 바로 실행
        });

        GetButton((int)Buttons.KeySettingButton).onClick.AddListener(() =>
        {
            SelectOption(SettingOption.KeySetting);
            ConfirmSelection(); // 클릭 시 바로 실행
        });

        GetButton((int)Buttons.BackButton).onClick.AddListener(ClosePopupUI);

        // 슬라이더 조정 시 선택 동기화
        Get<Scrollbar>((int)Scrollbars.MasterVolumeScrollbar).onValueChanged.AddListener((value) =>
        {
            SelectOption(SettingOption.MasterVolume);
        });

        Get<Scrollbar>((int)Scrollbars.BGMScrollbar).onValueChanged.AddListener((value) =>
        {
            SelectOption(SettingOption.BGM);
        });

        Get<Scrollbar>((int)Scrollbars.SFXScrollbar).onValueChanged.AddListener((value) =>
        {
            SelectOption(SettingOption.SFX);
        });

        _currentSelection = SettingOption.MasterVolume;
        UpdateOutlines();
    }

    private void HandleKeyPress(KeyCode key)
    {
        Debug.Log(key.ToString());
        switch (key)
        {
            case KeyCode.Escape:
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
        Debug.Log($"키보드로 선택됨: {_currentSelection}");
        UpdateOutlines();
    }

    private void SelectOption(SettingOption option)
    {
        _currentSelection = option;
        //MoveSelection(0); // 내부 상태 동기화 및 테두리 갱신
        UpdateOutlines();
        Debug.Log($"마우스로 선택됨: {_currentSelection}");
    }

    private void AdjustScrollbar(float value)
    {
        Debug.Log(value);
        if (_currentSelection == SettingOption.MasterVolume)
        {
            var scrollbar = Get<Scrollbar>((int)Scrollbars.MasterVolumeScrollbar);
            scrollbar.value = Mathf.Clamp01(scrollbar.value + value);
        }
        else if (_currentSelection == SettingOption.BGM)
        {
            var scrollbar = Get<Scrollbar>((int)Scrollbars.BGMScrollbar);
            scrollbar.value = Mathf.Clamp01(scrollbar.value + value);
        }
        else if (_currentSelection == SettingOption.SFX)
        {
            var scrollbar = Get<Scrollbar>((int)Scrollbars.SFXScrollbar);
            scrollbar.value = Mathf.Clamp01(scrollbar.value + value);
        }
    }

    private void ConfirmSelection()
    {
        if (_currentSelection == SettingOption.BeatOffset)
        {
            OpenTimingAdjustmentScene();
        }
        else if (_currentSelection == SettingOption.KeySetting)
        {
            OpenKeySettingPopup();
        }
    }

    private void OpenTimingAdjustmentScene()
    {
        Debug.Log("Opening Timing Adjustment Scene");
    }

    private void OpenKeySettingPopup()
    {
        Debug.Log("Opening Key Setting Popup");

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
        // 1. 모든 배경 이미지 끄기 (초기화)
        for (int i = 0; i < Enum.GetValues(typeof(Images)).Length; i++)
        {
            Get<Image>(i).gameObject.SetActive(false);
        }

        // 2. 현재 선택된 옵션에 맞는 배경 이미지 켜기
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
