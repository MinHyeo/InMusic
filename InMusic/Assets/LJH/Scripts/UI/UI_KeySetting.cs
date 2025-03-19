using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_KeySetting : UI_Base
{
    private static UI_KeySetting _instance;

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

    private int _currentSelection = 0; // ���� ���� �ε��� (0~3)
    private float _inputDelay = 0.15f;
    private float _lastInputTime = 0f;

    private bool _isChangingKey = false; // Ű ���� ����

    private KeyCode[] _assignedKeys = new KeyCode[4]; // ���� ������ Ű

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));

        // ��ư Ŭ�� �� Ű ���� ��� ����
        for (int i = 0; i < 4; i++)
        {
            int index = i;
            GetButton(index).onClick.AddListener(() => EnterChangeMode(index));
        }

        // �ڷΰ��� ��ư
        GetButton((int)Buttons.BackButton).onClick.AddListener(ClosePopupUI);

        // ���� �⺻ Ű �Ҵ�
        _assignedKeys[0] = KeyCode.A;
        _assignedKeys[1] = KeyCode.S;
        _assignedKeys[2] = KeyCode.D;
        _assignedKeys[3] = KeyCode.F;

        UpdateVisuals(); // �ʱ� ���� �ݿ�
    }

    private void Start()
    {
        Init(); 
        Managers.Input.OnKeyPressed += HandleKeyPress;
        _instance = this;

        // Setting UI�� �浹 ����
        UIManager.ToggleComponentInput<UI_Setting>(this.gameObject, false);
    }

    private void OnDestroy()
    {
        ClosePopupUI();
    }

    /// <summary>
    /// Ű���� �Է� ó��
    /// </summary>
    private void HandleKeyPress(KeyCode key)
    {
        if (Time.time - _lastInputTime < _inputDelay) return;
        _lastInputTime = Time.time;

        if (_isChangingKey)
        {
            // �ߺ� Ű Ȯ��
            if (Array.Exists(_assignedKeys, assignedKey => assignedKey == key))
            {
                Debug.LogWarning($"�̹� ��� ���� Ű�Դϴ�: {key}");
                return; // �ߺ� Ű ����
            }

            // Ű ���Ҵ�
            _assignedKeys[_currentSelection] = key;
            Debug.Log($"Note{_currentSelection + 1} Ű�� {key}�� �����");
            ExitChangeMode(); // ���� ��� ����
        }
        else
        {
            // �⺻ ��忡�� �̵� �� ���� ��� ����
            if (key == KeyCode.UpArrow) MoveSelection(-1);
            else if (key == KeyCode.DownArrow) MoveSelection(1);
            else if (key == KeyCode.Return) EnterChangeMode(_currentSelection); // Enter�� ���� ��� ����
            else if (key == KeyCode.Escape) ClosePopupUI();
        }
    }

    /// <summary>
    /// ���� �̵�
    /// </summary>
    private void MoveSelection(int direction)
    {
        _currentSelection += direction;
        _currentSelection = Mathf.Clamp(_currentSelection, 0, 3);
        UpdateVisuals();
    }

    /// <summary>
    /// Ű ���� ��� ����
    /// </summary>
    private void EnterChangeMode(int index)
    {
        _currentSelection = index;
        _isChangingKey = true;
        UpdateVisuals();
        Debug.Log($"Note{_currentSelection + 1} Ű ���� ��� ��...");
    }

    /// <summary>
    /// Ű ���� ��� ����
    /// </summary>
    private void ExitChangeMode()
    {
        _isChangingKey = false;
        UpdateVisuals();
    }

    /// <summary>
    /// ȭ�� �ð� ȿ�� ����
    /// </summary>
    private void UpdateVisuals()
    {
        // ���� ǥ�� (Back �̹���)
        for (int i = 0; i < 4; i++)
        {
            GetImage(i).gameObject.SetActive(i == _currentSelection && !_isChangingKey); // �⺻ ����� ����
        }

        // �ؽ�Ʈ ���� (�⺻: ����, ���� ���: ������)
        for (int i = 0; i < 4; i++)
        {
            GetText(i).color = (_isChangingKey && i == _currentSelection) ? Color.red : Color.black;
        }

        // ����(Notice) Ȱ��ȭ ����
        GetText((int)Texts.Notice).gameObject.SetActive(_isChangingKey);

        // ���� �Ҵ�� Ű�� �ؽ�Ʈ�� ǥ��
        for (int i = 0; i < 4; i++)
        {
            GetText(i).text = _assignedKeys[i].ToString();
        }
    }

    /// <summary>
    /// �ݱ�
    /// </summary>
    public override void ClosePopupUI()
    {
        if (_instance == this)
        {
            Managers.Input.OnKeyPressed -= HandleKeyPress;
            UIManager.ToggleComponentInput<UI_Setting>(this.gameObject, true);
            _instance = null;
            Destroy(gameObject);
        }
    }
}
