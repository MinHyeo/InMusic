using System;
using UnityEngine;

namespace SSW {
    public class UIInputManager {
        public Action keyAction = null;

        public void OnUpdate() {
            if(GlobalInputControl.IsInputEnabled == false) return;
            if(GlobalInputControl.CurrentInputMode != InputMode.UI) return;
            if(Input.anyKey == false) return;

            if(keyAction != null) {
                keyAction.Invoke();
            }
        }    
    }
}