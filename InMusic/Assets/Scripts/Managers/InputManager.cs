using System;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    [Tooltip("Observer ������ �ϴ� �Լ��� �� ���� �Ҵ�")]
    public Action<Define.UIControl> uIKeyPress;
    public Action<Define.NoteControl> noteKeyPress;

    [Header("Note Key Map")]
    [SerializeField]
    private Dictionary<Define.NoteControl, KeyCode> keyMapping;
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

    //Ű �Է� �� �ش� Ű�� �Է��� ��ٸ��� �Լ� ȣ��
    #region GetKeyPress
    //UI ������ �� ����� �޼���
    public void UIUpdate()
    {
        if (isSetMode) {
            KeyCode newKey = FindKeyPress();
            if (newKey != KeyCode.None) {
                SetNewKey(targetNoteKey, newKey);
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
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            uIKeyPress.Invoke(Define.UIControl.Up);
        }
        //DownArrow
        if (Input.GetKeyDown(KeyCode.DownArrow))
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

    //�ǹ� ������ �� ����� �޼���
    public void NoteUpdate() {
        foreach (var entry in keyMapping)
        {
            if (Input.GetKeyDown(entry.Value))
            {
                noteKeyPress.Invoke(entry.Key);
            }
        }
    }
    #endregion

    public KeyCode FindKeyPress() {
        //���� Ű ã��
        foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                return keyCode; // ���� Ű�� ��ȯ
            }
        }
        return KeyCode.None;
    }

    public void ChangeKey(Define.NoteControl noteKey) { 
        isSetMode = true;
        targetNoteKey = noteKey;
    }

    public void SetNewKey(Define.NoteControl noteKey, KeyCode newKey) {
        keyMapping[noteKey] = newKey;
        isSetMode = false;
    }


    public void SetUIKeyEvent(Action<Define.UIControl> keyEventFunc) {
        //Initialize
        RemoveUIKeyEvent(keyEventFunc);
        //SetKeyEvent
        uIKeyPress += keyEventFunc;
    }

    public void RemoveUIKeyEvent(Action<Define.UIControl> keyEventFunc) {
        uIKeyPress -= keyEventFunc;
    }

    public void SetNoteKeyEvent(Action<Define.NoteControl> keyEventFunc)
    {
        //Initialize
        RemoveNoteKeyEvent(keyEventFunc);
        //SetKeyEvent
        noteKeyPress += keyEventFunc;
    }

    public void RemoveNoteKeyEvent(Action<Define.NoteControl> keyEventFunc)
    {
        noteKeyPress -= keyEventFunc;
    }
}
