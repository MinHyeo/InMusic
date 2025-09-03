using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Play;
using System.Collections.Generic;
using SSW.DB;

namespace SongList{
    public class MultiHighlightSong : MonoBehaviour
    {
        #region Variables
        [Header("References")]
        [SerializeField] private MultiSongListController _multiSongListController; // SongListController

        [Header("Detail Information")]
        [SerializeField] private string _detailTitleText; // 기존 single Text 형식에서 string으로 변경하여 사용(UI 표시 안함)
        [SerializeField] private string _detailArtistText; // 기존 single Text 형식에서 string으로 변경하여 사용(UI 표시 안함)
        [SerializeField] private string _detailPlayTime; // 기존 single Text 형식에서 string으로 변경하여 사용(UI 표시 안함)
        [SerializeField] private Image _detailImage; // 이미지 정보는 그대로 필요함

        [Header("Score Information")]
        [SerializeField] private string _songHighestScoreText; // 기존 single Text 형식에서 string으로 변경하여 사용(UI 표시 안함)
        [SerializeField] private string _songMaxComboText; // 기존 single Text 형식에서 string으로 변경하여 사용(UI 표시 안함)
        [SerializeField] private string _songHighestAccuracyText; // 기존 single Text 형식에서 string으로 변경하여 사용(UI 표시 안함)
        [SerializeField] private string _songHighestRankText; // 기존 single Text 형식에서 string으로 변경하여 사용(UI 표시 안함)
        #endregion

        #region Unity Methods
        private void OnEnable() {
            if (_multiSongListController != null) {
                _multiSongListController.OnHighlightedSongChanged += HandleHighlightedSongChanged;
                Debug.Log("[HighlightSong] Event subscribed.");
            }
        }

        private void OnDisable() {
            if (_multiSongListController != null) {
                _multiSongListController.OnHighlightedSongChanged -= HandleHighlightedSongChanged;
                Debug.Log("[HighlightSong] Event unsubscribed.");
            }
        }
        #endregion

        #region UI Methods
        private void HandleHighlightedSongChanged(SongInfo songInfo) {
            Debug.Log($"[HighlightSong] HandleHighlightedSongChanged 호출됨: {songInfo.Title}");

            // 곡 제목이 비어있으면 UI 초기화
            if (songInfo == null) {
                ClearData();
                return;
            }

            // 제목 / 아티스트  / 재생 시간 갱신
            if (_detailTitleText != null)  _detailTitleText  = songInfo.Title;
            if (_detailArtistText != null) _detailArtistText = songInfo.Artist;
            if (_detailPlayTime != null) {
                // Duration을 MM:SS 형태로 포맷팅해서 저장
                int totalSeconds = Mathf.RoundToInt(songInfo.Duration);
                int minutes = totalSeconds / 60;
                int seconds = totalSeconds % 60;
                _detailPlayTime = $"{minutes:00}:{seconds:00}";
            }

            // 이미지 로드
            if (_detailImage != null)
            {
                Sprite songSprite = Resources.Load<Sprite>($"Song/{songInfo.Title}/{songInfo.Title}");
                _detailImage.sprite = songSprite;
            }


        }

        
        public (string title, string artist, string duration, Sprite sprite) GetSelectedSongInfo()
        {
            return (_detailTitleText, _detailArtistText, _detailPlayTime, _detailImage?.sprite);
        }

        /// <summary>
        /// 곡 정보를 찾지 못했거나, 제목이 비어있을 때 UI를 초기화/정리
        /// </summary>
        private void ClearData()
        {
            if (_detailTitleText != null) _detailTitleText = string.Empty;
            if (_detailArtistText != null) _detailArtistText = string.Empty;
            if (_detailImage != null) _detailImage = null;
        }
        #endregion
    }
}
