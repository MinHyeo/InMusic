using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace SongList {
    public class SongScrollView : MonoBehaviour {

        [Header("ScrollRect Settings")]
        [SerializeField] private RectTransform _contentRect;  // 슬롯들이 배치될 부모 Rect

        [Header("Slot Settings")]
        [SerializeField] private GameObject _songItemPrefab; // SongItem 프리팹

        [SerializeField] private int _visibleCount = 7;    // 화면에 보이는 슬롯 개수
        [SerializeField] private int _bufferCount = 5;     // 위/아래 버퍼 개수

        // 곡 목록
        private List<SongInfo> _songList = new List<SongInfo>();
        private int _songCount = 0;



        private void Awake() {
            _songList = LoadManager.Instance.Songs;
            _songCount = _songList.Count;
            Debug.Log($"[SongScrollView] Awake: 곡 개수 {_songCount}");
        }
    }
}