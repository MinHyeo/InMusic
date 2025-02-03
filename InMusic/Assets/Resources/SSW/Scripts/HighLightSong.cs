using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace SongList{
    public class HighlightSong : MonoBehaviour
    {
        #region Variables
        [Header("References")]
        [SerializeField] private SongListManager _songListManager; // SongListManager 참조

        [Header("Detail UI")]
        [SerializeField] private Text _detailTitleText; // 좌측에 표시할 제목 Text
        [SerializeField] private Text _detailArtistText; // 좌측에 표시할 아티스트 Text
        //[SerializeField] private Text _detailPlayTimeText; // 좌측에 표시할 플레이 시간 Text
        [SerializeField] private Image _detailImage; // 좌측에 표시할 이미지
        [SerializeField] private LoadingSong loadingSongObj;
        // [Header("Score UI")]
        // [SerializeField] private Text _songHighScoreText; // 좌측 하단에 표시할 최고 점수 Text
        // [SerializeField] private Text _songMaxComboText; // 좌측 하단에 표시할 최대 콤보 Text
        // [SerializeField] private Text _songAccuracy; // 좌측 하단에 표시할 정확도 Text


        [Header("Button")]
        [SerializeField] private Button startButton;
        #endregion

        #region Unity Methods
        private void OnEnable() {
            if (_songListManager != null) {
                _songListManager.OnHighlightedSongChanged += HandleHighlightedSongChanged;
                Debug.Log("[HighlightSong] Event subscribed.");
            }
        }

        private void OnDisable() {
            if (_songListManager != null) {
                _songListManager.OnHighlightedSongChanged -= HandleHighlightedSongChanged;
                Debug.Log("[HighlightSong] Event unsubscribed.");
            }
        }
        #endregion

        #region UI Methods
        private void HandleHighlightedSongChanged(SongInfo songInfo) {
            Debug.Log($"[HighlightSong] HandleHighlightedSongChanged 호출됨: {songInfo.Title}");

            // 곡 제목이 비어있으면 UI 초기화
            if (songInfo == null) {
                ClearUI();
                return;
            }

            // 제목 / 아티스트 갱신
            if (_detailTitleText != null)  _detailTitleText.text  = songInfo.Title;
            if (_detailArtistText != null) _detailArtistText.text = songInfo.Artist;

            // 이미지 로드
            if (_detailImage != null) {
                Sprite songSprite = Resources.Load<Sprite>($"Song/{songInfo.Title}/{songInfo.Title}");
                _detailImage.sprite = songSprite;
            }

            // 버튼 리스너 세팅
            if (startButton != null) {
                startButton.onClick.RemoveAllListeners();
                string playSceneName = "YMH";
                startButton.onClick.AddListener(() => loadingSongObj.LoadPlay(playSceneName, songInfo.Title, _detailArtistText.text, _detailImage.sprite));
            }
        }

        /// <summary>
        /// 곡 정보를 찾지 못했거나, 제목이 비어있을 때 UI를 초기화/정리
        /// </summary>
        private void ClearUI() {
            if (_detailTitleText != null)   _detailTitleText.text   = string.Empty;
            if (_detailArtistText != null)  _detailArtistText.text  = string.Empty;
            if (_detailImage != null)       _detailImage.sprite     = null;

            if (startButton != null) {
                // 버튼 리스너 제거
                startButton.onClick.RemoveAllListeners();
            }
        }
        #endregion
    }
}
