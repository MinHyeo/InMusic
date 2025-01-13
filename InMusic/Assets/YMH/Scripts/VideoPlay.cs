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

        private void Start()
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        public void GetVideoClip(Song songName)
        {
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
    }
}