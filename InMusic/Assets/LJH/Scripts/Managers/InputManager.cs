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
            Debug.Log("Ű �Է�");
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    Debug.Log("�´� Ű �Է�");
                    OnKeyPressed?.Invoke(key);
                    break;
                }
            }
        }
    }
}
