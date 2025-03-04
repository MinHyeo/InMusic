using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Play;

namespace SongList{
    public class HighlightSong : MonoBehaviour
    {
        #region Variables
        [Header("References")]
        [SerializeField] private SongListController _songListController; // SongListController

        [Header("Detail UI")]
        [SerializeField] private Text _detailTitleText; // 좌측에 표시할 제목 Text
        [SerializeField] private Text _detailArtistText; // 좌측에 표시할 아티스트 Text
        //[SerializeField] private Text _detailPlayTimeText; // 좌측에 표시할 플레이 시간 Text
        [SerializeField] private Image _detailImage; // 좌측에 표시할 이미지
        [SerializeField] private LoadingSong loadingSongObj;

        [Header("Score UI")]
        [SerializeField] private Text _songHighestScoreText; // 좌측 하단에 표시할 최고 점수 Text
        [SerializeField] private Text _songMaxComboText; // 좌측 하단에 표시할 최대 콤보 Text
        [SerializeField] private Text _songHighestAccuracyText; // 좌측 하단에 표시할 정확도 Text
        [SerializeField] private Text _songHighestRankText; // 좌측 하단에 표시할 최고 등급 Text


        [Header("Button")]
        [SerializeField] private Button startButton;
        #endregion

        #region Unity Methods
        private void OnEnable() {
            if (_songListController != null) {
                _songListController.OnHighlightedSongChanged += HandleHighlightedSongChanged;
                Debug.Log("[HighlightSong] Event subscribed.");
            }
        }

        private void OnDisable() {
            if (_songListController != null) {
                _songListController.OnHighlightedSongChanged -= HandleHighlightedSongChanged;
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

            // 플레이 기록 불러오기
            // 고유 키 생성 (예: "Heya_ArtistA")
            string uniqueKey = $"{songInfo.Title}_{songInfo.Artist}";

            SavePlayData spd = SavePlayData.Instance;
            if (spd != null) {
                ScoreData record = spd.GetSongScoreByKey(uniqueKey);
                if (record != null) {
                    // 최고 점수, 최대 콤보, 정확도, 랭크를 업데이트
                    if (_songHighestScoreText != null)
                        _songHighestScoreText.text = $"{record.score}";
                    if (_songMaxComboText != null)
                        _songMaxComboText.text = $"{record.maxCombo}";
                    if (_songHighestAccuracyText != null)
                        _songHighestAccuracyText.text = $"{record.accuracy:F2}%";
                    if (_songHighestRankText != null)
                        _songHighestRankText.text = $"{CalculateRank(record.score, record.accuracy)}";
                }
                else {
                    // 기록이 없는 경우 기본값 표시
                    if (_songHighestScoreText != null)
                        _songHighestScoreText.text = "-";
                    if (_songMaxComboText != null)
                        _songMaxComboText.text = "-";
                    if (_songHighestAccuracyText != null)
                        _songHighestAccuracyText.text = "-";
                    if (_songHighestRankText != null)
                        _songHighestRankText.text = "-";
                }
            }
            else {
                Debug.LogWarning("[HighlightSong] SavePlayData 객체를 찾지 못했습니다.");
            }

            // // TODO: 버튼 클릭 부분 리팩토링 필요 (버튼 관련 코드는 다른 클래스로 분리하여야 함)
            // // 버튼 리스너 세팅
            // if (startButton != null) {
            //     startButton.onClick.RemoveAllListeners();
            //     string playSceneName = "YMH";
            //     startButton.onClick.AddListener(() =>
            //     {
            //         // 1. 효과음 재생
            //         SoundManager.Instance.PlaySFX(SFXType.PlayStart);
            //         // 2. 씬 로드
            //         loadingSongObj = LoadingSong.Instance;
            //         loadingSongObj.LoadPlay(playSceneName, songInfo.Title, _detailArtistText.text, _detailImage.sprite);
            //     });

            // }
        }

        public void StartButtonAction()
        {
            //SoundManager.Instance.PlaySFX(SFXType.PlayStart);
            //Play.SoundManager.Instance.PlayBGMHighLight()
            loadingSongObj = LoadingSong.Instance;
            loadingSongObj.LoadPlay("YMH", _detailTitleText.text, _detailArtistText.text, _detailImage.sprite);
        }

        /// <summary>
        /// 점수와 정확도를 기준으로 랭크 계산
        /// 테스트를 위해 선언
        /// </summary>
        private string CalculateRank(int score, float accuracy)
        {
            if (score > 1000000 && accuracy > 95f) return "S";
            else if (score > 500000) return "A";
            else if (score > 300000) return "B";
            else return "C";
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
            if (_songHighestScoreText != null) _songHighestScoreText.text = "";
            if (_songMaxComboText != null)  _songMaxComboText.text  = "";
            if (_songHighestAccuracyText != null)  _songHighestAccuracyText.text  = "";
            if (_songHighestRankText != null)      _songHighestRankText.text      = "";
        }
        #endregion
    }
}
