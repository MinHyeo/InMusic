using UnityEngine;

namespace SSW
{
    public enum InputMode
    {
        GamePlay,
        UI,
    }
    public static class GlobalInputControl {
        public static bool IsInputEnabled = true;
        public static InputMode CurrentInputMode = InputMode.GamePlay;
    }
}
