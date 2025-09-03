using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_KeySetting : UI_Base
{
    private enum Texts { Note1, Note2, Note3, Note4, Notice }
    private enum Buttons { Button1, Button2, Button3, Button4, BackButton }
    private enum Images { Back1, Back2, Back3, Back4 }

    private int _currentSelection = 0;
    private float _inputDelay = 0.15f;
    private float _lastInputTime = 0f;
    private bool _isChangingKey = false;

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
        Managers.Instance.Key.OnKeyBindingsChanged += UpdateVisuals;
        UpdateVisuals();
    }

    private void Start()
    {
        Init();
        Managers.Instance.Input.OnKeyPressed += HandleKeyPress;
        Managers.Instance.UI.ToggleComponentInput<UI_Setting>(gameObject, false);
        EventSystem.current?.SetSelectedGameObject(null);
    }

    private void HandleKeyPress(KeyCode key)
    {
        if (Time.time - _lastInputTime < _inputDelay) return;
        _lastInputTime = Time.time;

        if (_isChangingKey)
        {
            if (Managers.Instance.Key.IsKeyInUse(key))
            {
                Debug.LogWarning("이미 사용 중인 키입니다: " + key);
                return;
            }

            Managers.Instance.Key.SetKey((Define.RhythmKey)_currentSelection, key);
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
        Debug.Log($"Note{_currentSelection + 1} 키 변경 대기 중...");
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
            GetText(i).text = Managers.Instance.Key.GetKey((Define.RhythmKey)i).ToString();
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
