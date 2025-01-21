// using System.Collections.Generic;
// using System.Collections;
// using UnityEngine;
// using UnityEngine.UI;
// using System;

// namespace SongList {
//     public class SongScrollView : MonoBehaviour {

//         [Header("ScrollRect Settings")]
//         [SerializeField] private ScrollRect _scrollRect;  // 스크롤 뷰
//         [SerializeField] private RectTransform _viewportRect;  // 스크롤 뷰의 뷰포트
//         [SerializeField] private RectTransform _contentRect;  // 슬롯들이 배치될 부모 Rect

//         [Header("Slot Settings")]
//         [SerializeField] private GameObject[] _songItems = new GameObject[17];
//         [SerializeField] private List<ScrollSlot> _slotList = new List<ScrollSlot>();  // 슬롯 리스트
//         [SerializeField] private float _slotHeight = 93;  // 슬롯의 높이
//         [SerializeField] private float _snapThreshold = 46.5f;  // 스냅 임계값

//         private float _lastSnapY = 0;  // 마지막 스냅 위치

//         // 곡 목록
//         private List<SongInfo> _songList;
//         private int _songCount = 0;
//         private int _steps = 0;



//         private void Awake() {
//             _contentRect.anchoredPosition = new Vector2(0, 93 * 5);
//             _songList = LoadManager.Instance.Songs;
//             _songCount = _songList.Count;
//             Debug.Log($"[SongScrollView] Awake: 곡 개수 {_songCount}");
//             _contentRect.SetAsFirstSibling();
//             _scrollRect.scrollSensitivity = 0;
//         }

//         private void Start() {
//             _scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
//         }

//         private void OnScrollValueChanged (Vector2 value) {
//             SnapCheck();
//         }

//         private void SnapCheck() {
//             float currentY = _contentRect.anchoredPosition.y;
//             float deltaY = currentY - _lastSnapY;
//             if (deltaY < _snapThreshold) {
//                 _steps = Mathf.FloorToInt((deltaY + _snapThreshold) / _slotHeight);
//                 if(_steps > 0) {
//                     MoveSlotsUp(_steps);
//                     _lastSnapY += _steps * _slotHeight;
//                 }
//             } else if (deltaY > _snapThreshold) {
//                 _steps = Mathf.FloorToInt((-deltaY + _snapThreshold) / _slotHeight);
//                 if(_steps > 0) {
//                     //MoveSlotsDown(_steps);
//                     _lastSnapY -= _steps * _slotHeight;
//                 }
//             }
//         }

//         private void MoveSlotsUp(int steps) {
//             for(int s = 0; s < steps; s++) {
//                 ScrollSlot topSlot = _slotList[0];
//                 _slotList.RemoveAt(0);
//                 _slotList.Add(topSlot);

//                 ScrollSlot lastSlot = _slotList[_slotList.Count - 1];
//                 int newIndex = lastSlot.GetIndex() + 1;
//                 topSlot.SetData(newIndex, _songList[newIndex]);

//                 RectTransform bottomRect = lastSlot.GetComponent<RectTransform>();
//                 float newY = bottomRect.anchoredPosition.y - _slotHeight;
//                 topSlot.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, newY);

//                 _slotList.Add(topSlot);
//             }
//         }
        
//     }
// }