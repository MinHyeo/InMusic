using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    public Action<KeyCode> OnKeyPressed;

    private Dictionary<KeyCode, bool> _keyDownState = new();
    private Dictionary<KeyCode, bool> _keyHeldState = new();
    private Dictionary<KeyCode, bool> _keyUpState = new();

    public void Update()
    {
        _keyDownState.Clear();
        _keyHeldState.Clear();
        _keyUpState.Clear();

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                _keyDownState[key] = true;
                OnKeyPressed?.Invoke(key);
            }
            if (Input.GetKey(key))
                _keyHeldState[key] = true;
            if (Input.GetKeyUp(key))
                _keyUpState[key] = true;
        }
    }

    public bool GetKeyDown(KeyCode key) => _keyDownState.ContainsKey(key);
    public bool GetKey(KeyCode key) => _keyHeldState.ContainsKey(key);
    public bool GetKeyUp(KeyCode key) => _keyUpState.ContainsKey(key);
}
