using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            PauseManager.Instance.Restart();
            PlayManager.Instance.ReStart();
        }

        public void MusicSelect()
        {
            Time.timeScale = 1;

            PauseManager.Instance.DestroyKeyEvent();
            //PlayManager.Instance.DestroyKeyEvent();

            SoundManager.Instance.End();
            GameManager.Instance.SetGameState(GameState.MusicSelect);
            GameManager.Instance.SelectSong(PlayManager.Instance.SongTitle);
        }

        public void Exit()
        {
            
        }
    }
}