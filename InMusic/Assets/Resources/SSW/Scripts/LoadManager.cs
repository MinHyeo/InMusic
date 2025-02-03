using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

namespace SongList {
    public class LoadManager : Singleton<LoadManager> {
        [Header("UI for Splash/Fade")]
        [SerializeField] private CanvasGroup logoCanvasGroup; 
        [SerializeField] private float fadeDuration = 1f;
        public List<SongInfo> Songs { get; private set; }
        protected override void Awake() {
            base.Awake();
        }

        private void Start() {
            StartCoroutine(GameResourcesLoad());
        }

        private IEnumerator GameResourcesLoad()
        {
            yield return StartCoroutine(FadeCanvas(logoCanvasGroup, 0f, 1f, fadeDuration));
            yield return StartCoroutine(LoadAllSongs());
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(FadeCanvas(logoCanvasGroup, 1f, 0f, fadeDuration));
            SceneManager.LoadScene("Main_Lobby_PSH");
        }

        /// <summary>
        /// CanvasGroup alpha를 부드럽게 바꾸는 페이드 코루틴
        /// </summary>
        private IEnumerator FadeCanvas(CanvasGroup cg, float from, float to, float duration)
        {
            float elapsed = 0f;
            cg.alpha = from;

            // 페이드 중에는 interactable/blocksRaycasts를 제어할 수도 있음
            cg.interactable = false;
            cg.blocksRaycasts = true;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                cg.alpha = Mathf.Lerp(from, to, t);
                yield return null;
            }
            cg.alpha = to;
        }

        private IEnumerator LoadAllSongs() {
            Songs = new List<SongInfo>();

            foreach(Song song in Enum.GetValues(typeof(Song))) {
                SongInfo info = BmsLoader.Instance.SelectSong(song);
                if (info != null) {
                    Songs.Add(info);
                    Debug.Log("Load Song: " + song);
                } else {
                    Debug.Log("Failed to load song: " + song);
                }
                yield return null;
            }
        }
    }
}