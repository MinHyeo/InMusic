using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_KeySetting : UI_Base
{
    private enum Texts
    {
        Note1,
        Note2,
        Note3,
        Note4,
        Notice
    }

    private enum Buttons
    {
        Button1,
        Button2,
        Button3,
        Button4,
        BackButton
    }

    private enum Images
    {
        Back1,
        Back2,
        Back3,
        Back4
    }

    public enum DefKey // ��� �Ұ����� Ű
    {
        Return = KeyCode.Return,
        F10 = KeyCode.F10,
        F1 = KeyCode.F1,
        Escape = KeyCode.Escape
    }

    private int _currentSelection = 0;
    private float _inputDelay = 0.15f;
    private float _lastInputTime = 0f;
    private bool _isChangingKey = false;
    private readonly KeyCode[] _assignedKeys = { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F };

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));

        for (int i = 0; i < 4; i++)
        {
            int index = i;
            GetButton(index).onClick.AddListener(() => EnterChangeMode(index));
        }

        GetButton((int)Buttons.BackButton).onClick.AddListener(ClosePopupUI);
        UpdateVisuals();
    }

    private void Start()
    {
        Init();
        Managers.Instance.Input.OnKeyPressed -= HandleKeyPress;
        Managers.Instance.Input.OnKeyPressed += HandleKeyPress;
        Managers.Instance.UI.ToggleComponentInput<UI_Setting>(gameObject, false);

        // ��Ŀ�� ����
        EventSystem.current?.SetSelectedGameObject(null);
    }

    private void HandleKeyPress(KeyCode key)
    {
        if (Time.time - _lastInputTime < _inputDelay) return;
        _lastInputTime = Time.time;

        if (_isChangingKey)
        {
            if (Enum.GetValues(typeof(DefKey)).Cast<DefKey>().Any(dk => (KeyCode)dk == key))
            {
                Debug.LogWarning($"�⺻ Ű�� ����� �� �����ϴ�: {key}");
                return;
            }

            if (_assignedKeys.Contains(key))
            {
                Debug.LogWarning($"�̹� ��� ���� Ű�Դϴ�: {key}");
                return;
            }

            _assignedKeys[_currentSelection] = key;
            Debug.Log($"Note{_currentSelection + 1} Ű�� {key}�� �����");
            ExitChangeMode();
            return;
        }

        switch (key)
        {
            case KeyCode.UpArrow: MoveSelection(-1); break;
            case KeyCode.DownArrow: MoveSelection(1); break;
            case KeyCode.Return: EnterChangeMode(_currentSelection); break;
            case KeyCode.Escape: ClosePopupUI(); break;
        }
    }

    private void MoveSelection(int direction)
    {
        _currentSelection = Mathf.Clamp(_currentSelection + direction, 0, 3);
        UpdateVisuals();
    }

    private void EnterChangeMode(int index)
    {
        _currentSelection = index;
        _isChangingKey = true;
        UpdateVisuals();
        EventSystem.current?.SetSelectedGameObject(null);
        Debug.Log($"Note{_currentSelection + 1} Ű ���� ��� ��...");
    }

    private void ExitChangeMode()
    {
        _isChangingKey = false;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < 4; i++)
        {
            GetImage(i).gameObject.SetActive(i == _currentSelection && !_isChangingKey);
            GetText(i).color = (_isChangingKey && i == _currentSelection) ? Color.red : Color.black;
            GetText(i).text = _assignedKeys[i].ToString();
        }

        GetText((int)Texts.Notice).gameObject.SetActive(_isChangingKey);
    }

    public override void ClosePopupUI()
    {
        Managers.Instance.Input.OnKeyPressed -= HandleKeyPress;
        Managers.Instance.UI.ToggleComponentInput<UI_Setting>(gameObject, true);

        Managers.Instance.UI.CloseCurrentPopup();
    }
}
