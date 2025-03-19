using System;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

public class Managers
{

    private static UIManager _uiManager;
    private static InputManager _inputManager;
    public static SoundManager Sound => SoundManager.Instance;

    public static UIManager UI
        {
            get
            {
                if (_uiManager == null)
                    _uiManager = new UIManager();
                return _uiManager;
            }
        }

        public static InputManager Input
        {
            get
            {
                if (_inputManager == null)
                    _inputManager = new InputManager();
                return _inputManager;
            }
        }



    //  GetOrAddComponent: 중복 방지 및 최적화
    public static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        return component ? component : go.AddComponent<T>();
    }

    //  FindChild: UI 요소 탐색 기능 최적화
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (!recursive)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform child = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || child.name == name)
                {
                    T component = child.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }
        return null;
    }
}
