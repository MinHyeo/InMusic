using System;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    [Tooltip("Observer 역할을 하는 함수에 이 변수 할당")]
    public Action<Define.UIControl> uIKeyPress;
    public Action<Define.NoteControl> noteKeyPress;

    [Header("Note Key Map")]
    [SerializeField]
    private Dictionary<Define.NoteControl, KeyCode> keyMapping= new Dictionary<Define.NoteControl, KeyCode>();
    [Header("InputManager Mode")]
    [SerializeField] 
    private bool isSetMode = false;
    private Define.NoteControl targetNoteKey;

    public void Init() {
        //Default Key
        keyMapping.Add(Define.NoteControl.Key1, KeyCode.D);
        keyMapping.Add(Define.NoteControl.Key2, KeyCode.F);
        keyMapping.Add(Define.NoteControl.Key3, KeyCode.J);
        keyMapping.Add(Define.NoteControl.Key4, KeyCode.K);
    }

    //키 입력 시 해당 키의 입력을 기다리는 함수 호출
    #region GetKeyPress

    /// <summary>
    /// 키 입력 감지 메서드
    /// </summary>
    public void UIUpdate()
    {
        if (uIKeyPress == null) {
            return;
        }

        if (isSetMode) {
            KeyCode newKey = FindKeyPress();
            if (newKey != KeyCode.None && newKey != KeyCode.Escape && newKey != KeyCode.KeypadEnter) {
                SetNewKey(targetNoteKey, newKey);
                uIKeyPress.Invoke(Define.UIControl.Any);
            }
            return;
        }
        //LeftArrow
        if (Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            uIKeyPress.Invoke(Define.UIControl.Left);
        }
        //RightArrow
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            uIKeyPress.Invoke(Define.UIControl.Right);
        }
        //UpArrow
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKey(KeyCode.UpArrow))
        {
            uIKeyPress.Invoke(Define.UIControl.Up);
        }
        //DownArrow
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            uIKeyPress.Invoke(Define.UIControl.Down);
        }
        //Enter
        if (Input.GetKeyDown(KeyCode.Return))
        {
            uIKeyPress.Invoke(Define.UIControl.Enter);
        }
        //ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uIKeyPress.Invoke(Define.UIControl.Esc);
        }
        //F1: KeyGuide
        if (Input.GetKeyDown(KeyCode.F1))
        {
            uIKeyPress.Invoke(Define.UIControl.Guide);
        }
        //F10: Setting
        if (Input.GetKeyDown(KeyCode.F10))
        {
            uIKeyPress.Invoke(Define.UIControl.Setting);
        }
    }

    //건반 조작할 때 사용할 메서드
    public void NoteUpdate() {
        if (noteKeyPress != null)
        {
            foreach (var entry in keyMapping)
            {
                if (Input.GetKeyDown(entry.Value))
                {
                    noteKeyPress.Invoke(entry.Key);
                }
            }
        }
    }
    #endregion

    public KeyCode FindKeyPress() {
        //누른 키 찾기
        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                Debug.Log($"New Key: {keyCode}");
                return keyCode; //누른 키를 반환
            }
        }
        return KeyCode.None;
    }

    public string GetKey(Define.NoteControl noteKey) {
        return keyMapping[noteKey].ToString();
    
    }

    public void ChangeKey(Define.NoteControl noteKey) { 
        isSetMode = true;
        targetNoteKey = noteKey;
    }

    public void SetNewKey(Define.NoteControl noteKey, KeyCode newKey) {
        keyMapping[noteKey] = newKey;
        isSetMode = false;
    }

    #region KeyEventSet
    /// <summary>
    /// UI 조작시 사용할 키보드 입력 메소드, Define.UIControl을 전달
    /// </summary>
    /// <param name="keyEventFunc"></param>
    public void SetUIKeyEvent(Action<Define.UIControl> keyEventFunc) {
        //Initialize
        RemoveUIKeyEvent(keyEventFunc);
        //SetKeyEvent
        uIKeyPress += keyEventFunc;
    }
    /// <summary>
    /// 연결한 키보드 입력 함수를 제거하는 메서드
    /// </summary>
    /// <param name="keyEventFunc"></param>
    public void RemoveUIKeyEvent(Action<Define.UIControl> keyEventFunc) {
        uIKeyPress -= keyEventFunc;
    }

    /// <summary>
    /// 게임 플레이시 사용할 키보드 입력 메서드, Define.NoteControl를 전달함
    /// </summary>
    /// <param name="keyEventFunc"></param>
    public void SetNoteKeyEvent(Action<Define.NoteControl> keyEventFunc)
    {
        //Initialize
        RemoveNoteKeyEvent(keyEventFunc);
        //SetKeyEvent
        noteKeyPress += keyEventFunc;
    }
    /// <summary>
    /// 연결한 키보드 입력 함수를 제거하는 메서드
    /// </summary>
    /// <param name="keyEventFunc"></param>
    public void RemoveNoteKeyEvent(Action<Define.NoteControl> keyEventFunc)
    {
        noteKeyPress -= keyEventFunc;
    }
    #endregion
}
