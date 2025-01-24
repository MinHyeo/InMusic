using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

namespace SongList
{
    public class SongListManager : MonoBehaviour
    {
        #region Variables
        [Header("ScrollRect Settings")]
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _contentRect;

        [Header("Slot Settings")]
        [SerializeField] private GameObject _songItemPrefab;
        [SerializeField] private int _bufferItems = 5;

        [Header("Background Sprites")]
        [SerializeField] private Sprite _selectedSprite;
        [SerializeField] private Sprite _defaultSprite;

        [Header("Scrolling Settings")]
        [SerializeField] private float _scrollDebounceTime = 0.05f; // 스크롤 멈춤 판정 시간

        // === 추가: 일정 범위 이상 넘어가면 텔레포트
        [SerializeField] private float _teleportThreshold = 2000f; 

        private bool _isScrolling = false;
        private float _lastScrollTime = 0f;

        private int _totalSongCount;
        private int _poolSize;
        private float _itemHeight;
        private int _visibleCount;
        private int _firstVisibleIndexCached = -5;

        private List<GameObject> _songList = new List<GameObject>();
        private List<SongInfo> _songs;
        private GameObject _selectedSlot;
        public event Action<SongInfo> OnHighlightedSongChanged;
        #endregion

        #region Unity Methods
        private void Awake() {
            _songs = LoadManager.Instance.Songs;
            _totalSongCount = _songs.Count;

            _itemHeight = _songItemPrefab.GetComponent<RectTransform>().sizeDelta.y;
            _scrollRect.onValueChanged.AddListener(OnScrolled);
        }

        private IEnumerator Start() {
            yield return null; // 1프레임 대기

            float viewportHeight = _scrollRect.viewport.rect.height;
            _visibleCount = Mathf.CeilToInt(viewportHeight / _itemHeight);

            _poolSize = _visibleCount + _bufferItems * 2;
            Debug.Log($"[SongListManager] Pool Size: {_poolSize}");

            float contentHeight = _visibleCount * _itemHeight;
            _contentRect.sizeDelta = new Vector2(_contentRect.sizeDelta.x, contentHeight);
            _contentRect.anchoredPosition = new Vector2(0, 0);

            // 아이템 풀 생성
            for (int i = 0; i < _poolSize; i++) {
                GameObject slotSong = Instantiate(_songItemPrefab, _contentRect);
                slotSong.name = $"SongItem_{i}";

                RectTransform rt = slotSong.GetComponent<RectTransform>();
                float yPos = -(i * _itemHeight);
                rt.anchoredPosition = new Vector2(0, yPos + (93 * 5));

                UpdateSlotData(slotSong, i);
                _songList.Add(slotSong);
            }

            int rememeberedIndex = IndexSaveTest.Instance.GetLastSelectedIndex();
            Debug.Log($"[SongListManager] Remembered Index: {rememeberedIndex}");

            //TODO: 아래 주석 풀고, 맨 밑에 있는 연산 식 확인
            // 현재 선택된 슬롯 위의 내용들이 사라지면서 그 다음항목부터 보이는 것으로 추정됨
            // 초기 스냅 + 재배치
            SnapToNearestSlot();
            OnScroll();

            // 중앙 슬롯 즉시 하이라이트
            HighlightCenterSlotByPosition(isImmediate: true);

            // if (rememeberedIndex >= 0) {
            //     ForceCenterAtIndex(rememeberedIndex);
            // } else {
            //     // 초기 스냅 + 재배치
            //     SnapToNearestSlot();
            //     OnScroll();

            //     // 중앙 슬롯 즉시 하이라이트
            //     HighlightCenterSlotByPosition(isImmediate: true);
            // }
        }
        
        private void Update() {
            if (_isScrolling) {
                if (Time.time - _lastScrollTime > _scrollDebounceTime) {
                    _isScrolling = false;
                    SnapToNearestSlot();
                    OnScroll();
                    HighlightCenterSlotByPosition();
                }
            }
        }
        #endregion

        #region Scroll Methods
        private void OnScrolled(Vector2 pos) {
            _isScrolling = true;
            _lastScrollTime = Time.time;

            // === 추가: 일정 임계값 벗어나면 텔레포트
            //TeleportIfNeeded();
        }

        // === 추가 함수: contentRect를 ±2000 범위를 벗어날 때 중앙으로 되돌리기
        // TODO: 수정 필요
        private void TeleportIfNeeded()
        {
            float contentY = _contentRect.anchoredPosition.y;
            _teleportThreshold = _itemHeight * _poolSize * 2;
            if (contentY > _teleportThreshold)
            {
                // 1) ContentRect를 _teleportThreshold만큼 아래로 이동
                _contentRect.anchoredPosition -= new Vector2(0, _teleportThreshold);

                // 2) 슬롯들은 그만큼 위로 이동 (결과적으로 화면 위치 변화 X)
                foreach (var slotObj in _songList)
                {
                    var rt = slotObj.GetComponent<RectTransform>();
                    rt.anchoredPosition += new Vector2(0, _teleportThreshold);
                }

                // 3) firstVisibleIndexCached도 그만큼 "칸 수"만큼 변경
                int shiftCount = Mathf.RoundToInt(_teleportThreshold / _itemHeight);
                _firstVisibleIndexCached += shiftCount;
            }
            else if (contentY < -_teleportThreshold)
            {
                _contentRect.anchoredPosition += new Vector2(0, _teleportThreshold);

                foreach (var slotObj in _songList)
                {
                    var rt = slotObj.GetComponent<RectTransform>();
                    rt.anchoredPosition -= new Vector2(0, _teleportThreshold);
                }

                int shiftCount = Mathf.RoundToInt(_teleportThreshold / _itemHeight);
                _firstVisibleIndexCached -= shiftCount;
                if (_firstVisibleIndexCached < 0) _firstVisibleIndexCached = 0;
            }
        }

        private void SnapToNearestSlot() {
            float topOffset = 93f * _bufferItems; 
            float contentY = _contentRect.anchoredPosition.y;
            float tempOffset = contentY - topOffset;
            Debug.Log($"[SongListManager] Snap to Nearest Slot: {tempOffset}");
            int nearestIndex = Mathf.RoundToInt(tempOffset / _itemHeight);

            float newY = nearestIndex * _itemHeight + topOffset;
            Debug.Log($"[SongListManager] Snap to Index: {nearestIndex}");
            _contentRect.anchoredPosition = new Vector2(0, newY);
        }

        private void OnScroll() {
            float contentY = _contentRect.anchoredPosition.y;
            int newFirstIndex = Mathf.FloorToInt(contentY / _itemHeight);
            Debug.Log($"New First Index: {newFirstIndex}");
            if (newFirstIndex != _firstVisibleIndexCached) {
                int diffIndex = newFirstIndex - _firstVisibleIndexCached;
                bool scrollDown = (diffIndex > 0);
                int shiftCount = Mathf.Abs(diffIndex);

                ShiftSlots(shiftCount, scrollDown);
                _firstVisibleIndexCached = newFirstIndex;
                Debug.Log($"[SongListManager] First Index: {_firstVisibleIndexCached}");
            }
        }

        private void ShiftSlots(int shiftCount, bool scrollDown) {
            for (int i = 0; i < shiftCount; i++) {
                if (scrollDown) {
                    // 맨 앞 슬롯 -> 맨 뒤
                    GameObject slot = _songList[0];
                    _songList.RemoveAt(0);
                    _songList.Add(slot);
                    Debug.Log("_songList.Length: " + _songList.Count);

                    // === 수정: 무한 랩
                    int newIndex = _firstVisibleIndexCached + _poolSize + i;

                    int realIndex = ((newIndex % _totalSongCount) + _totalSongCount) % _totalSongCount;

                    slot.SetActive(true);
                    RectTransform rt = slot.GetComponent<RectTransform>();
                    rt.anchoredPosition = CalculateSlotPosition(newIndex); 
                    UpdateSlotData(slot, realIndex);

                } else {
                    // 맨 뒤 슬롯 -> 맨 앞으로
                    GameObject slot = _songList[_poolSize - 1];
                    _songList.RemoveAt(_poolSize - 1);
                    _songList.Insert(0, slot);

                    int newIndex = _firstVisibleIndexCached - 1 - i;

                    int realIndex = ((newIndex % _totalSongCount) + _totalSongCount) % _totalSongCount;

                    slot.SetActive(true);
                    RectTransform rt = slot.GetComponent<RectTransform>();
                    rt.anchoredPosition = CalculateSlotPosition(newIndex);
                    UpdateSlotData(slot, realIndex);
                }
            }
        }

        private Vector2 CalculateSlotPosition(int dataIndex) {
            // dataIndex = 논리 인덱스(계속 커질 수 있음), 실제 표시 위치는 - (dataIndex * slotHeight)
            float y = -(dataIndex * _itemHeight);
            Debug.Log($"[SongListManager] Calculate Slot Position: {dataIndex} -> {y}");
            return new Vector2(0, y);
        }
        #endregion

        #region Highlighting

        private void HighlightCenterSlotByPosition(bool isImmediate = false) {
            if (_selectedSlot != null) {
                ScrollSlot oldSlotComp = _selectedSlot.GetComponent<ScrollSlot>();
                if (oldSlotComp != null) {
                    // 기존 슬롯에 대해 '게이지가 줄어드는' 애니메이션을 실행
                    oldSlotComp.SetHighlight(false, isImmediate);
                }
                _selectedSlot = null;
            }

            float contentY = _contentRect.anchoredPosition.y;
            float viewportH = _scrollRect.viewport.rect.height;
            float viewportCenterY = -(viewportH / 2f);

            GameObject closestSlot = null;
            float closestDist = float.MaxValue;

            foreach (var slotObj in _songList) {
                if (!slotObj.activeSelf) continue;

                RectTransform rt = slotObj.GetComponent<RectTransform>();
                float slotY = rt.anchoredPosition.y + contentY;
                float dist = Mathf.Abs(slotY - viewportCenterY);

                if (dist < closestDist) {
                    closestDist = dist;
                    closestSlot = slotObj;
                }
            }

            if (closestSlot != null) {
                if(closestSlot == _selectedSlot) return;
                _selectedSlot = closestSlot;

                ScrollSlot slotComponent = closestSlot.GetComponent<ScrollSlot>();
                if (slotComponent != null) {
                    slotComponent.SetHighlight(true, isImmediate);

                    SongInfo highlightSong = slotComponent.GetHighlightedSong();
                    if (highlightSong != null) {
                        Debug.Log($"[SongListManager] Highlighted Song Title: {highlightSong.Title}");
                        OnHighlightedSongChanged?.Invoke(highlightSong);
                    } else {
                        Debug.LogWarning("[SongListManager] highlightSong is null - maybe slot.SetData() didn't set anything yet?");
                    }
                }
                else {
                    Debug.LogWarning("[SongListManager] closestSlot has no ScrollSlot component attached?");
                }
            }
        }
        #endregion

        #region Slot Data Setting
        private void UpdateSlotData(GameObject slotObj, int dataIndex) {
            int songIndex = ((dataIndex % _totalSongCount) + _totalSongCount) % _totalSongCount;
            
            // 곡 데이터
            SongInfo currentSong = _songs[songIndex];

            // ScrollSlot 컴포넌트를 가져와서 데이터 세팅
            ScrollSlot slot = slotObj.GetComponent<ScrollSlot>();
            if (slot != null) {
                slot.SetData(currentSong, songIndex);
            }
        }

        // private void ForceCenterAtIndex(int index) {
        //     int realIndex = ((index % _totalSongCount) + _totalSongCount) % _totalSongCount;
        //     float newY = realIndex * _itemHeight;
        //     _contentRect.anchoredPosition = new Vector2(0, -newY);
        //     OnScroll();
        //     HighlightCenterSlotByPosition(true);
        // }

        /// <summary>
        /// 특정 곡 인덱스를 "가운데"에 오도록 contentRect 위치를 강제 세팅
        /// 그리고 슬롯 재배치 + 하이라이트까지 연결
        /// </summary>
        // private void ForceCenterAtIndex(int index)
        // {
        //     // 1) “index”에 해당하는 슬롯이 화면 중앙에 오도록, contentRect를 이동
        //     //    아래는 예시 계산이며, 프로젝트 구조에 맞게 조정 필요
        //     float topOffset = 93f * _bufferItems; 
        //     float y = -(index * _itemHeight) + topOffset;

        //     // anchoredPosition 즉시 세팅
        //     _contentRect.anchoredPosition = new Vector2(0, y);
            
        //     // 2) 슬롯 재배치
        //     OnScroll();  
        //     // 내부에서 ShiftSlots()가 실행되어, 실제 배열 인덱스 → 슬롯 위치 등이 업데이트됨

        //     // 3) 즉시 하이라이트
        //     HighlightCenterSlotByPosition(true);
        // }
        #endregion
    }
}