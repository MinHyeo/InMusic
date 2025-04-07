using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyManager
{
    private Dictionary<Define.RhythmKey, KeyCode> _keyBindings = new();

    public Action OnKeyBindingsChanged;

    public KeyManager()
    {
        LoadKeyBindings();
    }

    public KeyCode GetKey(Define.RhythmKey key)
    {
        return _keyBindings.TryGetValue(key, out var code) ? code : KeyCode.None;
    }

    public void SetKey(Define.RhythmKey key, KeyCode code)
    {
        _keyBindings[key] = code;
        SaveKeyBindings();
        OnKeyBindingsChanged?.Invoke();
    }

    public bool IsKeyInUse(KeyCode code)
    {
        return _keyBindings.ContainsValue(code);
    }

    private void LoadKeyBindings()
    {
        foreach (Define.RhythmKey key in Enum.GetValues(typeof(Define.RhythmKey)))
        {
            string keyName = key.ToString();
            string saved = PlayerPrefs.GetString($"KeyBind_{keyName}", "");

            if (Enum.TryParse(saved, out KeyCode result))
                _keyBindings[key] = result;
            else
                _keyBindings[key] = GetDefaultKey(key);
        }
    }

    private void SaveKeyBindings()
    {
        foreach (var pair in _keyBindings)
        {
            PlayerPrefs.SetString($"KeyBind_{pair.Key}", pair.Value.ToString());
        }

        PlayerPrefs.Save();
    }

    private KeyCode GetDefaultKey(Define.RhythmKey key) => key switch
    {
        Define.RhythmKey.Key1 => KeyCode.D,
        Define.RhythmKey.Key2 => KeyCode.F,
        Define.RhythmKey.Key3 => KeyCode.J,
        Define.RhythmKey.Key4 => KeyCode.K,
        _ => KeyCode.None,
    };
}
