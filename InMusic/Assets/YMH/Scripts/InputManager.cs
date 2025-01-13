using Unity.VisualScripting;
using UnityEngine;

namespace Play 
{
    enum Key
    {
        D = 11,
        F = 12,
        J = 13,
        K = 14,
    }

    public class InputManager : MonoBehaviour
    {
        [Header("Key Objects")]
        [SerializeField]
        private GameObject[] keyObjects;
        private bool[] isKeyDown = new bool[4] { false, false, false, false };

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                InputKey(true, Key.D);
                PlayManager.Instance.OnKeyPress((int)Key.D, Time.time);
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                InputKey(false, Key.D);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                InputKey(true, Key.F);
                PlayManager.Instance.OnKeyPress((int)Key.F, Time.time);
            }
            if (Input.GetKeyUp(KeyCode.F))
            {
                InputKey(false, Key.F);
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                InputKey(true, Key.J);
                PlayManager.Instance.OnKeyPress((int)Key.J, Time.time);
            }
            if (Input.GetKeyUp(KeyCode.J))
            {
                InputKey(false, Key.J);
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                InputKey(true, Key.K);
                PlayManager.Instance.OnKeyPress((int)Key.K, Time.time);
            }
            if (Input.GetKeyUp(KeyCode.K))
            {
                InputKey(false, Key.K);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PlayManager.Instance.OnPause();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                PauseManager.Instance.OnKeyPress(KeyCode.DownArrow);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                PauseManager.Instance.OnKeyPress(KeyCode.UpArrow);
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                PauseManager.Instance.OnKeyPress(KeyCode.Return);
            }
        }

        private void InputKey(bool isDown, Key key)
        {
            isKeyDown[(int)key - (int)Key.D] = isDown;
            keyObjects[(int)key - (int)Key.D].SetActive(isKeyDown[(int)key - (int)Key.D]);
        }
    }
}