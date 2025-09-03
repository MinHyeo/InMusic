using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
using SSW.DB;

namespace SongList {
    public class LoadManager : Managers.Singleton<LoadManager> {
        [Header("Song Management")]
        [SerializeField] private SongTitleList songTitleList;
        
        [Header("UI for Splash/Fade")]
        [SerializeField] private CanvasGroup logoCanvasGroup;
        [SerializeField] private CanvasGroup fmodCanvasGroup;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private float logoDisplayDuration = 1.5f;

        [Header("Debug Info (Read Only)")]
        [SerializeField] private bool isLoaded = false;
        [SerializeField] private int totalSongsCount = 0;

        //ddd

        public List<SongInfo> Songs { get; private set; }
        protected override void Awake() {
            base.Awake();
        }

        private void Start() {
            StartCoroutine(GameResourcesLoad());
        }

        private IEnumerator GameResourcesLoad()
        {
            yield return StartCoroutine(FadeCanvas(fmodCanvasGroup, 0f, 1f, fadeDuration));
            yield return new WaitForSeconds(logoDisplayDuration);
            yield return StartCoroutine(LoadAllSongs());
            yield return StartCoroutine(FadeCanvas(fmodCanvasGroup, 1f, 0f, fadeDuration));
            yield return StartCoroutine(FadeCanvas(logoCanvasGroup, 0f, 1f, fadeDuration));
            yield return new WaitForSeconds(logoDisplayDuration);
            yield return StartCoroutine(FadeCanvas(logoCanvasGroup, 1f, 0f, fadeDuration));
            GameManager.Instance.SetGameState(GameState.MainMenu);
            SceneManager.LoadScene("MainLobbyScene_InMusic");
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
            DBService db = FindFirstObjectByType<DBService>();
            
            if (songTitleList == null) {
                Debug.LogError("SongTitleList is not assigned!");
                yield break;
            }

            if (db == null) {
                Debug.LogError("DBService not found in the scene.");
                yield break;
            }

            Debug.Log($"[LoadManager] Starting to load {songTitleList.GetSongCount()} songs...");

            foreach(string songTitle in songTitleList.GetAllSongTitles()) {
                SongInfo info = BmsLoader.Instance.SelectSongByTitle(songTitle);
                if (info != null) {
                    Songs.Add(info);
                    Debug.Log($"[LoadManager] Loaded: {songTitle} - {info.Title} by {info.Artist} ({info.Duration:F1}s)");

                    // DBService에 곡 정보를 저장하는 부분
                    if (db != null) {
                        string musicId = info.Title + "_" + info.Artist;
                        db.SaveSongToDB(musicId, info.Title, info.Artist);
                    }
                } else {
                    Debug.LogError($"[LoadManager] Failed to load song: {songTitle}");
                }
                yield return null;
            }
            
            Debug.Log($"[LoadManager] Total songs loaded: {Songs.Count}/{songTitleList.GetSongCount()}");
            
            // 에디터에서 확인할 수 있도록 상태 업데이트
            isLoaded = true;
            totalSongsCount = Songs.Count;
        }
    }
}