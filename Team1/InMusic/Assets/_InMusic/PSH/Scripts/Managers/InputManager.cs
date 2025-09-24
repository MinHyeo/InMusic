using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Tooltip("Observer ������ �ϴ� �Լ��� �� ���� �Ҵ�")]
    public Action<Define.UIControl> uIKeyPress;
    public Action<Define.NoteControl> noteKeyPress;
    public Action<Define.NoteControl> noteKeyRelease;

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

        //StartCoroutine(UIControl());
    }

    //Ű �Է� �� �ش� Ű�� �Է��� ��ٸ��� �Լ� ȣ��
    #region GetKeyPress

    /// <summary>
    /// Ű �Է� ���� �޼���
    /// </summary>
    IEnumerator UIControl()
    {
        if (isSetMode)
        {
            KeyCode newKey = FindKeyPress();
            if (newKey != KeyCode.None && newKey != KeyCode.KeypadEnter)
            {
                SetNewKey(targetNoteKey, newKey);
                uIKeyPress.Invoke(Define.UIControl.Any);
            }
            //���� ���
            else if (newKey == KeyCode.Escape)
            {
                uIKeyPress.Invoke(Define.UIControl.Esc);
            }
            yield return null;
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
        // if (Input.GetKeyDown(KeyCode.F10))
        // {
        //     uIKeyPress.Invoke(Define.UIControl.Setting);
        // }
        yield return null;
    }

    public void UIUpdate()
    {
        if (isSetMode) {
            KeyCode newKey = FindKeyPress();
            if (newKey != KeyCode.None && newKey != KeyCode.KeypadEnter) {
                SetNewKey(targetNoteKey, newKey);
                uIKeyPress.Invoke(Define.UIControl.Any);
            }
            //���� ���
            else if (newKey == KeyCode.Escape) {
                uIKeyPress.Invoke(Define.UIControl.Esc);
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
        // //F10: Setting
        // if (Input.GetKeyDown(KeyCode.F10))
        // {
        //     uIKeyPress.Invoke(Define.UIControl.Setting);
        // }
    }

    //�ǹ� ����
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

        if(noteKeyRelease != null)
        {
            foreach(var entry in keyMapping)
            {
                if (Input.GetKeyUp(entry.Value))
                {
                    noteKeyRelease.Invoke(entry.Key);
                }
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
                Debug.Log($"Find New Key: {keyCode}");
                return keyCode; //���� Ű�� ��ȯ
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
    /// UI ���۽� ����� Ű���� �Է� �޼ҵ�, Define.UIControl�� ����
    /// </summary>
    /// <param name="keyEventFunc"></param>
    public void SetUIKeyEvent(Action<Define.UIControl> keyEventFunc) {
        //Initialize
        RemoveUIKeyEvent(keyEventFunc);
        //SetKeyEvent
        uIKeyPress += keyEventFunc;
    }
    /// <summary>
    /// ������ Ű���� �Է� �Լ��� �����ϴ� �޼���
    /// </summary>
    /// <param name="keyEventFunc"></param>
    public void RemoveUIKeyEvent(Action<Define.UIControl> keyEventFunc) {
        uIKeyPress -= keyEventFunc;
    }

    /// <summary>
    /// ���� �÷��̽� ����� Ű���� �Է� �޼���, Define.NoteControl�� ������
    /// </summary>
    /// <param name="keyEventFunc"></param>
    public void SetNoteKeyPressEvent(Action<Define.NoteControl> keyEventFunc)
    {
        //Initialize
        RemoveNoteKeyEvent(keyEventFunc);
        //SetKeyEvent
        noteKeyPress += keyEventFunc;
    }

    public void SetNoteKeyReleaseEvent(Action<Define.NoteControl> keyEventFunc)
    {
        //Initialize
        RemoveNoteKeyEvent(keyEventFunc);
        //SetKeyEvent
        noteKeyRelease += keyEventFunc;
    }

    /// <summary>
    /// ������ Ű���� �Է� �Լ��� �����ϴ� �޼���
    /// </summary>
    /// <param name="keyEventFunc"></param>
    public void RemoveNoteKeyEvent(Action<Define.NoteControl> keyEventFunc)
    {
        noteKeyPress -= keyEventFunc;
    }

    public void RemoveNoteKeyReleaseEvent(Action<Define.NoteControl> keyEventFunc)
    {
        noteKeyRelease -= keyEventFunc;
    }
    #endregion
}
