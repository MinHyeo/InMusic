using System;
using UnityEngine;

namespace SSW
{
    public class SingleMenuInputManager
    {
        public Action keyAction = null;

        public void OnUpdate()
        {
            if(Input.anyKey == false) return;

            if(keyAction != null)
            {
                keyAction.Invoke();
            }
        }

        //MonoBehaviour가 아니므로, 게임오브젝트를 별도로 생성하지 않아도 됨
    }
}