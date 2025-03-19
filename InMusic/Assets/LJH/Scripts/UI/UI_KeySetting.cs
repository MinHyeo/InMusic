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

    private int _currentSelection = 0; // 현재 선택 인덱스 (0~3)
    private float _inputDelay = 0.15f;
    private float _lastInputTime = 0f;

    private bool _isChangingKey = false; // 키 변경 상태

    private KeyCode[] _assignedKeys = new KeyCode[4]; // 현재 설정된 키

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));

        // 버튼 클릭 시 키 변경 모드 진입
        for (int i = 0; i < 4; i++)
        {
            int index = i;
            GetButton(index).onClick.AddListener(() => EnterChangeMode(index));
        }

        // 뒤로가기 버튼
        GetButton((int)Buttons.BackButton).onClick.AddListener(ClosePopupUI);

        // 예시 기본 키 할당
        _assignedKeys[0] = KeyCode.A;
        _assignedKeys[1] = KeyCode.S;
        _assignedKeys[2] = KeyCode.D;
        _assignedKeys[3] = KeyCode.F;

        UpdateVisuals(); // 초기 상태 반영
    }

    private void Start()
    {
        Init(); 
        Managers.Input.OnKeyPressed += HandleKeyPress;
        _instance = this;

        // Setting UI와 충돌 방지
        UIManager.ToggleComponentInput<UI_Setting>(this.gameObject, false);
    }

    private void OnDestroy()
    {
        ClosePopupUI();
    }

    /// <summary>
    /// 키보드 입력 처리
    /// </summary>
    private void HandleKeyPress(KeyCode key)
    {
        if (Time.time - _lastInputTime < _inputDelay) return;
        _lastInputTime = Time.time;

        if (_isChangingKey)
        {
            // 중복 키 확인
            if (Array.Exists(_assignedKeys, assignedKey => assignedKey == key))
            {
                Debug.LogWarning($"이미 사용 중인 키입니다: {key}");
                return; // 중복 키 방지
            }

            // 키 재할당
            _assignedKeys[_currentSelection] = key;
            Debug.Log($"Note{_currentSelection + 1} 키가 {key}로 변경됨");
            ExitChangeMode(); // 변경 모드 종료
        }
        else
        {
            // 기본 모드에서 이동 및 변경 모드 진입
            if (key == KeyCode.UpArrow) MoveSelection(-1);
            else if (key == KeyCode.DownArrow) MoveSelection(1);
            else if (key == KeyCode.Return) EnterChangeMode(_currentSelection); // Enter로 변경 모드 진입
            else if (key == KeyCode.Escape) ClosePopupUI();
        }
    }

    /// <summary>
    /// 선택 이동
    /// </summary>
    private void MoveSelection(int direction)
    {
        _currentSelection += direction;
        _currentSelection = Mathf.Clamp(_currentSelection, 0, 3);
        UpdateVisuals();
    }

    /// <summary>
    /// 키 변경 모드 진입
    /// </summary>
    private void EnterChangeMode(int index)
    {
        _currentSelection = index;
        _isChangingKey = true;
        UpdateVisuals();
        Debug.Log($"Note{_currentSelection + 1} 키 변경 대기 중...");
    }

    /// <summary>
    /// 키 변경 모드 종료
    /// </summary>
    private void ExitChangeMode()
    {
        _isChangingKey = false;
        UpdateVisuals();
    }

    /// <summary>
    /// 화면 시각 효과 갱신
    /// </summary>
    private void UpdateVisuals()
    {
        // 선택 표시 (Back 이미지)
        for (int i = 0; i < 4; i++)
        {
            GetImage(i).gameObject.SetActive(i == _currentSelection && !_isChangingKey); // 기본 모드일 때만
        }

        // 텍스트 색상 (기본: 검정, 변경 모드: 빨간색)
        for (int i = 0; i < 4; i++)
        {
            GetText(i).color = (_isChangingKey && i == _currentSelection) ? Color.red : Color.black;
        }

        // 공지(Notice) 활성화 여부
        GetText((int)Texts.Notice).gameObject.SetActive(_isChangingKey);

        // 현재 할당된 키를 텍스트로 표시
        for (int i = 0; i < 4; i++)
        {
            GetText(i).text = _assignedKeys[i].ToString();
        }
    }

    /// <summary>
    /// 닫기
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
