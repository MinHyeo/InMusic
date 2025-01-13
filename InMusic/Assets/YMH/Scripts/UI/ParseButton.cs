using System.Collections;
using UnityEngine;

namespace Play
{
    public class ParseButton : MonoBehaviour
    {
        public void Continue()
        {
            PauseManager.Instance.Continue();
        }

        public void Restart()
        {
            PlayManager.Instance.ReStart();
        }

        public void MusicSelect()
        {

        }

        public void Exit()
        {
            
        }
    }
}