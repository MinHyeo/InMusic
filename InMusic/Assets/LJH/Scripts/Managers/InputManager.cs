using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    public Action<KeyCode> OnKeyPressed;

    public void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.anyKeyDown)
        {
            Debug.Log("키 입력");
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    Debug.Log("맞는 키 입력");
                    OnKeyPressed?.Invoke(key);
                    break;
                }
            }
        }
    }
}
