using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;

namespace SongList {
    public class LoadManager : Singleton<LoadManager> {

        public List<SongInfo> Songs { get; private set; }
        protected override void Awake() {
            base.Awake();
        }

        private void LoadAllSongs() {
            Songs = new List<SongInfo>();

            foreach(Song song in Enum.GetValues(typeof(Song))) {
                SongInfo info = BmsLoader.Instance.SelectSong(song);
                if (info != null) {
                    Songs.Add(info);
                    Debug.Log("Load Song: " + song);
                } else {
                    Debug.Log("Failed to load song: " + song);
                }
            }

            SceneManager.LoadScene("test_SSW");
        }

        private void OnGUI() {
            if (GUI.Button(new Rect(10, 10, 150, 100), "Load Songs")) {
                LoadAllSongs();
            }
        }
    }
}