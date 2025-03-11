using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_KeyGuide : UI_Base
{
    private static UI_KeyGuide _instance;

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
        if (_instance == this)
        {
            Managers.Input.OnKeyPressed -= HandleKeyPress;
            UIManager.ToggleComponentInput<UI_MainMenu>(this.gameObject, true);
            _instance = null;
            Destroy(gameObject);
        }
    }
}
