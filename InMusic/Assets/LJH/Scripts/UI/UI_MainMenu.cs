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
    private bool _isMoving = false; // �ִϸ��̼� ��� �� �Է� ������ �÷���

    private Animator _soloGroupAnimator;
    private Animator _multiGroupAnimator;

    private Button _leftButton;
    private Button _rightButton;

    private void Start()
    {
        Init();
        SetupRoot();
        Managers.Input.OnKeyPressed -= HandleKeyPress;
        Managers.Input.OnKeyPressed += HandleKeyPress;

        // ���⼭ �׷� �ִϸ����͸� �����´� (�� ������Ʈ �׷�)
        _soloGroupAnimator = Managers.FindChild<Animator>(this.gameObject, "SoloPlayButton", true);
        _multiGroupAnimator = Managers.FindChild<Animator>(this.gameObject, "MultiPlayButton", true);

        _leftButton = GetButton((int)Buttons.LeftButton);
        _rightButton = GetButton((int)Buttons.RightButton);

        UpdateButtonInteractable(); // �ʱ� ��ư ���� ������Ʈ
    }

    private void Update()
    {
        Managers.Input.Update();
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
            root.transform.SetParent(this.transform.parent);
            root.transform.localPosition = Vector3.zero;
        }
    }

    public void HandleKeyPress(KeyCode key)
    {
        if (_isMoving) return; // �ִϸ��̼� �߿� �Է� ����

        Debug.Log($"Key Pressed: {key}");

        if (key == KeyCode.F1) OpenKeyGuide();
        else if (key == KeyCode.F10) OpenSettings();
        else if (key == KeyCode.LeftArrow) ChangeSelection(false);
        else if (key == KeyCode.RightArrow) ChangeSelection(true);
        else if (key == KeyCode.Return) StartGame(_currentState == SelectionState.Multi);
    }

    private void ChangeSelection(bool moveRight)
    {
        if (_isMoving) return; // �ִϸ��̼� �߿� ����

        if (moveRight && _currentState == SelectionState.Solo)
        {
            StartCoroutine(SwitchTo(SelectionState.Multi, true));
        }
        else if (!moveRight && _currentState == SelectionState.Multi)
        {
            StartCoroutine(SwitchTo(SelectionState.Solo, false));
        }
    }

    /// <summary>
    /// �ִϸ��̼��� ���� ��ȯ ó��
    /// </summary>
    private IEnumerator SwitchTo(SelectionState newState, bool rightMove)
    {
        _isMoving = true;

        // �׷� �ִϸ��̼� Ʈ���� �ߵ�
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

        // �ִϸ��̼� ���̸�ŭ ��� (�ʿ� �� ����)
        yield return new WaitForSeconds(0.5f);

        _currentState = newState;

        UpdateButtonInteractable(); // �̵� ���� ���� ����

        _isMoving = false;
    }

    /// <summary>
    /// ����, ������ ��ư Ȱ��ȭ ���� ����
    /// </summary>
    private void UpdateButtonInteractable()
    {
        _leftButton.interactable = _currentState != SelectionState.Solo;
        _rightButton.interactable = _currentState != SelectionState.Multi;
    }

    private void StartGame(bool isMultiplayer)
    {
        Debug.Log(isMultiplayer ? "Starting Multiplayer Mode" : "Starting Solo Mode");
        // �� ��ȯ �� ���� ���� ����
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
        Managers.UI.ShowPopup(settingPanel);
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
        Managers.UI.ShowPopup(keyGuidePanel);
    }

    private void ExitGame()
    {
        Debug.Log("Exiting Game");
        Application.Quit();
    }
}
