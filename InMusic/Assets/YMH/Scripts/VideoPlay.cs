using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace Play
{
    public class VideoPlay : MonoBehaviour
    {
        private VideoPlayer videoPlayer;
        private VideoClip videoClip;
        [SerializeField]
        private RenderTexture renderTexture;

        public void GetVideoClip(Song songName)
        {
            videoPlayer = GetComponent<VideoPlayer>();

            string part = "Song/" + songName + "/" + songName;
            videoClip = Resources.Load<VideoClip>(part);
            videoPlayer.clip = videoClip;
        }

        public void Play()
        {
            videoPlayer.Play();
        }

        public void Pause()
        {
            videoPlayer.Pause();
        }

        public void Restart()
        {
            videoPlayer.Stop();
            videoPlayer.Play();
        }

        public void End()
        {
            videoPlayer.Stop();

            // RenderTexture 초기화 (검은 화면으로 덮기)
            RenderTexture activeRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = activeRenderTexture;
        }
    }
}