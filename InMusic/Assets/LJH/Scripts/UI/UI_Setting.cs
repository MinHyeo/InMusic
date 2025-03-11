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

        // UI_MainMenu�� Ű �Է� ����
        UIManager.ToggleComponentInput<UI_MainMenu>(this.gameObject, false);
    }

    public override void Init()
    {
        //?
        Bind<Button>(typeof(Buttons));
        Bind<Scrollbar>(typeof(Scrollbars));
        Bind<Image>(typeof(Images));

        // ��ư�� ConfirmSelection �߰�
        GetButton((int)Buttons.BeatOffsetButton).onClick.AddListener(() =>
        {
            SelectOption(SettingOption.BeatOffset);
            ConfirmSelection(); // Ŭ�� �� �ٷ� ����
        });

        GetButton((int)Buttons.KeySettingButton).onClick.AddListener(() =>
        {
            SelectOption(SettingOption.KeySetting);
            ConfirmSelection(); // Ŭ�� �� �ٷ� ����
        });

        GetButton((int)Buttons.BackButton).onClick.AddListener(ClosePopupUI);

        // �����̴� ���� �� ���� ����ȭ
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
        Debug.Log($"Ű����� ���õ�: {_currentSelection}");
        UpdateOutlines();
    }

    private void SelectOption(SettingOption option)
    {
        _currentSelection = option;
        //MoveSelection(0); // ���� ���� ����ȭ �� �׵θ� ����
        UpdateOutlines();
        Debug.Log($"���콺�� ���õ�: {_currentSelection}");
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
        // 1. ��� ��� �̹��� ���� (�ʱ�ȭ)
        for (int i = 0; i < Enum.GetValues(typeof(Images)).Length; i++)
        {
            Get<Image>(i).gameObject.SetActive(false);
        }

        // 2. ���� ���õ� �ɼǿ� �´� ��� �̹��� �ѱ�
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
