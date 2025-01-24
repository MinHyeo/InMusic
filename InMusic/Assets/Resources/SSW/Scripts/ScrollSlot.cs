using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;


namespace SongList {
    public class ScrollSlot : MonoBehaviour {
        #region Variables
        [Header("UI References")]
        [SerializeField] private Image _songImage;       // 곡 이미지
        [SerializeField] private Text _songTitle;        // 곡 제목
        [SerializeField] private Text _songArtist;       // 곡 아티스트
        [SerializeField] private Image _backgroundImage;   // 항상 기본 스프라이트가 표시되는 배경 이미지
        [SerializeField] private Image _highlightImage;  // 하이라이트 표시용 UI (테두리나 배경 등)
        [SerializeField] private bool _isHighlighted;    // 현재 하이라이트 상태인지 여부

        [Header("Sprites")]
        [SerializeField] private Sprite _defaultSprite;   // 비선택 상태
        [SerializeField] private Sprite _highlightSprite; // 선택(하이라이트) 상태

        private SongInfo _currentData;
        private int _currentIndex;
        private Coroutine _highlightCoroutine;
        #endregion

        #region Slot Data Setting
        private void Awake() {
            // 초기에 배경은 기본 스프라이트, 하이라이트는 투명( fillAmount=0 )
            if (_backgroundImage != null) {
                _backgroundImage.sprite = _defaultSprite;
                _backgroundImage.type = Image.Type.Simple;  // 또는 Filled, fillAmount=1
            }
            if (_highlightImage != null) {
                _highlightImage.sprite = _highlightSprite;
                _highlightImage.type = Image.Type.Filled;
                _highlightImage.fillMethod = Image.FillMethod.Horizontal;
                _highlightImage.fillOrigin = 0;   // 왼쪽부터 채움
                _highlightImage.fillAmount = 0f;  // 완전히 투명 상태
            }
        }

        /// <summary>
        /// 슬롯에 표시할 곡 데이터를 외부에서 전달받아 UI를 갱신
        /// </summary>
        public void SetData(SongInfo data, int index) {
            _currentData = data;
            _currentIndex = index;

            UpdateUI();
        }

        /// <summary>
        /// 슬롯 UI 업데이트. 제목, 아티스트, 이미지 등 갱신
        /// </summary>
        private void UpdateUI() {
            if (_currentData == null) return;

            // 곡 제목/아티스트 표시
            if (_songTitle  != null) _songTitle.text  = _currentData.Title;
            if (_songArtist != null) _songArtist.text = _currentData.Artist;

            // 곡 이미지를 Resources 폴더에서 "Song/{Title}/{Title}" 경로로 로드
            if (_songImage != null && !string.IsNullOrEmpty(_currentData.Title)) {
                // 예: Song/ShapeOfYou/ShapeOfYou
                string resourcePath = $"Song/{_currentData.Title}/{_currentData.Title}";
                Sprite loadedSprite = Resources.Load<Sprite>(resourcePath);

                if (loadedSprite != null) {
                    _songImage.sprite = loadedSprite;
                }
                else {
                    Debug.LogWarning($"[ScrollSlot] Could not find sprite for title '{_currentData.Title}' at '{resourcePath}' in Resources.");
                    // 필요한 경우 기본 이미지나 null 설정
                    _songImage.sprite = null;
                }
            }
        }
        #endregion

        #region Highlight

        /// <summary>
        /// 슬롯이 현재 하이라이트(선택) 상태인지 여부에 따른 UI 처리
        /// </summary>
        /// <summary>
        /// 하이라이트 On/Off 설정 (게이지 애니메이션)
        /// </summary>
        public void SetHighlight(bool highlight, bool isImmediate = false) {
            if (_highlightImage == null) return;
            if(_isHighlighted == highlight) return;
            _isHighlighted = highlight;

            // 코루틴 중복 실행 방지
            if (_highlightCoroutine != null) {
                StopCoroutine(_highlightCoroutine);
                _highlightCoroutine = null;
            }

            if (isImmediate) {
                // 애니메이션 없이 즉시 fillAmount = (highlight ? 1 : 0)
                _highlightImage.fillAmount = highlight ? 1f : 0f;
            } else {
                float target = highlight ? 1f : 0f;
                _highlightCoroutine = StartCoroutine(AnimateHighlightFill(target, 0.2f));
            }
        }

        /// <summary>
        /// 현재 fillAmount부터 toFill까지 Lerp 애니메이션
        /// </summary>
        private IEnumerator AnimateHighlightFill(float toFill, float duration) {
            float startFill = _highlightImage.fillAmount;
            float elapsed = 0f;

            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                _highlightImage.fillAmount = Mathf.Lerp(startFill, toFill, t);
                yield return null;
            }
            _highlightImage.fillAmount = toFill;

            _highlightCoroutine = null;
        }

        public SongInfo GetHighlightedSong() {
            if (_isHighlighted) {
                if (_currentData != null) {
                Debug.Log($"[ScrollSlot] GetHighlightedSong: {_currentData.Title}");
                IndexSaveTest.Instance.SelectSong(_currentIndex);
                Debug.Log($"[ScrollSlot] GetHighlightedSong: {_currentIndex}");
                return _currentData;
                }
            }
            return null;
        }
        #endregion
    }
}