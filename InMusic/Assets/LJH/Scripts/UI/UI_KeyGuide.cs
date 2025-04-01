using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_KeyGuide : UI_Base
{
    private void Start()
    {
        Init();
        Managers.Instance.Input.OnKeyPressed += HandleKeyPress;

        // UI_MainMenu의 키 입력 해제
        Managers.Instance.UI.ToggleComponentInput<UI_MainMenu>(this.gameObject, false);
    }

    public override void Init()
    {
        GetComponentInChildren<Button>()?.onClick.AddListener(ClosePopupUI);
    }

    private void HandleKeyPress(KeyCode key)
    {
        if (key == KeyCode.Escape)
        {
            ClosePopupUI();
        }
    }

    public override void ClosePopupUI()
    {
        Managers.Instance.Input.OnKeyPressed -= HandleKeyPress;
        Managers.Instance.UI.ToggleComponentInput<UI_MainMenu>(this.gameObject, true);

        Managers.Instance.UI.CloseCurrentPopup();
    }

}