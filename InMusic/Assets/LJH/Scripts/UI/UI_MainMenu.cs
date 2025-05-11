using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenu : UI_Base
{
    enum Buttons
    {
        SoloPlayButton,
        MultiPlayButton,
        SettingsButton,
        KeyGuideButton,
        ExitButton,
        LeftButton,
        RightButton
    }

    public enum SelectionState
    {
        Solo,
        Multi
    }

    private SelectionState _currentState = SelectionState.Solo;
    private bool _isMoving = false;

    private Animator _soloGroupAnimator;
    private Animator _multiGroupAnimator;

    private Button _leftButton;
    private Button _rightButton;

    private void Start()
    {
        Init();
        SetupRoot();

        // 키 입력 등록
        Managers.Instance.Input.OnKeyPressed -= HandleKeyPress;
        Managers.Instance.Input.OnKeyPressed += HandleKeyPress;

        // Animator 가져오기
        _soloGroupAnimator = Managers.Instance.FindChild<Animator>(gameObject, "SoloPlayButton", true);
        _multiGroupAnimator = Managers.Instance.FindChild<Animator>(gameObject, "MultiPlayButton", true);

        _leftButton = GetButton((int)Buttons.LeftButton);
        _rightButton = GetButton((int)Buttons.RightButton);

        UpdateButtonInteractable();
    }


    public override void Init()
    {
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.SoloPlayButton).onClick.AddListener(() => StartGame(false));
        GetButton((int)Buttons.MultiPlayButton).onClick.AddListener(() => StartGame(true));
        GetButton((int)Buttons.SettingsButton).onClick.AddListener(OpenSettings);
        GetButton((int)Buttons.KeyGuideButton).onClick.AddListener(OpenKeyGuide);
        GetButton((int)Buttons.ExitButton).onClick.AddListener(ExitGame);
        GetButton((int)Buttons.LeftButton).onClick.AddListener(() => ChangeSelection(false));
        GetButton((int)Buttons.RightButton).onClick.AddListener(() => ChangeSelection(true));
    }

    private void SetupRoot()
    {
        GameObject root = GameObject.Find("@Root");
        if (root == null)
        {
            root = new GameObject("@Root");
            root.transform.SetParent(transform.parent);
            root.transform.localPosition = Vector3.zero;
        }
    }

    public void HandleKeyPress(KeyCode key)
    {
        if (_isMoving) return;

        Debug.Log($"Key Pressed: {key}");

        switch (key)
        {
            case KeyCode.F1: OpenKeyGuide(); break;
            case KeyCode.F10: OpenSettings(); break;
            case KeyCode.LeftArrow: ChangeSelection(false); break;
            case KeyCode.RightArrow: ChangeSelection(true); break;
            case KeyCode.Return: StartGame(_currentState == SelectionState.Multi); break;
        }
    }

    private void ChangeSelection(bool moveRight)
    {
        if (_isMoving) return;

        if (moveRight && _currentState == SelectionState.Solo)
            StartCoroutine(SwitchTo(SelectionState.Multi, true));
        else if (!moveRight && _currentState == SelectionState.Multi)
            StartCoroutine(SwitchTo(SelectionState.Solo, false));
    }

    private IEnumerator SwitchTo(SelectionState newState, bool rightMove)
    {
        _isMoving = true;

        if (rightMove)
        {
            _soloGroupAnimator.SetTrigger("RightMove");
            _multiGroupAnimator.SetTrigger("RightMove");
        }
        else
        {
            _soloGroupAnimator.SetTrigger("LeftMove");
            _multiGroupAnimator.SetTrigger("LeftMove");
        }

        yield return new WaitForSeconds(0.5f);

        _currentState = newState;
        UpdateButtonInteractable();
        _isMoving = false;
    }

    private void UpdateButtonInteractable()
    {
        _leftButton.interactable = _currentState != SelectionState.Solo;
        _rightButton.interactable = _currentState != SelectionState.Multi;
    }

    private void StartGame(bool isMultiplayer)
    {
        Debug.Log(isMultiplayer ? "Starting Multiplayer Mode" : "Starting Solo Mode");
        GameManager_PSH.Resource.CheckMusic();

        if (isMultiplayer)
        {
            // Multiplayer scene load logic here
        }
        else
        {
            LoadingScreen.Instance.LoadScene("Single_Lobby_PSH");
        }
    }

    private void OpenSettings()
    {
        Debug.Log("Opening Settings");
        GameObject settingPanel = Resources.Load<GameObject>("Prefabs/UI/SettingPanel");
        if (settingPanel == null)
        {
            Debug.LogError("SettingPanel prefab not found!");
            return;
        }

        Managers.Instance.UI.ShowPopup(settingPanel);
    }

    private void OpenKeyGuide()
    {
        Debug.Log("Opening Key Guide");
        GameObject keyGuidePanel = Resources.Load<GameObject>("Prefabs/UI/KeyGuidePanel");
        if (keyGuidePanel == null)
        {
            Debug.LogError("KeyGuidePanel prefab not found!");
            return;
        }

        Managers.Instance.UI.ShowPopup(keyGuidePanel);
    }

    private void ExitGame()
    {
        Debug.Log("Exiting Game");
        Application.Quit();
    }

    private void OnDestroy()
    {
        if (Managers.Instance != null)
            Managers.Instance.Input.OnKeyPressed -= HandleKeyPress;
    }



}