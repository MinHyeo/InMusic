using System;
using UnityEngine;

namespace SSW
{
    public static class GlobalInpoutControl {
        public static bool IsInputEnabled = true;
    }
    public class SingleMenuInputManager
    {
        public Action keyAction = null;

        public void OnUpdate()
        {
            if(GlobalInpoutControl.IsInputEnabled == false) return;
            if(Input.anyKey == false) return;

            if(keyAction != null)
            {
                keyAction.Invoke();
            }
        }

        //MonoBehaviour가 아니므로, 게임오브젝트를 별도로 생성하지 않아도 됨
    }
}