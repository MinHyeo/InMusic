using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;


namespace SongList {
    public class ScrollSlot : MonoBehaviour {
        [Header("UI References")]
        [SerializeField] private Image _songImage;       // 곡 이미지
        [SerializeField] private Text _songTitle;        // 곡 제목
        [SerializeField] private Text _songArtist;       // 곡 아티스트
        [SerializeField] private Image _highlightImage;  // 하이라이트 표시용 UI (테두리나 배경 등)

        [Header("Sprites")]
        [SerializeField] private Sprite _defaultSprite;   // 비선택 상태
        [SerializeField] private Sprite _highlightSprite; // 선택(하이라이트) 상태

        private SongInfo _currentData;
        private int _currentIndex;

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

        /// <summary>
        /// 슬롯이 현재 하이라이트(선택) 상태인지 여부에 따른 UI 처리
        /// </summary>
        public void SetHighlight(bool isHighlight) {
            if (_highlightImage == null) return;

            _highlightImage.sprite = isHighlight ? _highlightSprite : _defaultSprite;
        }
    }
}