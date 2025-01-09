using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace SongList
{
    public class SongListManager : MonoBehaviour
    {
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
        [SerializeField] private float _scrollDebounceTime = 0.05f; 
        // 스크롤 멈춘 뒤 50ms 경과 시 스냅

        private bool _isScrolling = false;
        private float _lastScrollTime = 0f;

        // 내부 변수
        private int _totalSongCount;
        private int _poolSize;
        private float _itemHeight;
        private int _visibleCount;
        private int _firstVisibleIndexCached = 0;

        private LinkedList<GameObject> _songList = new LinkedList<GameObject>();
        private List<SongInfo> songs;
        private GameObject _selectedSlot;
        public event Action<string> OnHighlightedSongChanged;

        private void Awake() {
            // 예: 실제 곡 목록 불러오기
            songs = LoadManager.Instance.Songs; 
            _totalSongCount = songs.Count;

            // 프리팹 높이
            _itemHeight = _songItemPrefab.GetComponent<RectTransform>().sizeDelta.y;

            // 스크롤 이벤트 연결
            _scrollRect.onValueChanged.AddListener(OnScrolled);
        }

        private IEnumerator Start() {
            // 1프레임 대기 후, 뷰포트 사이즈를 정확히 계산
            yield return null;

            float viewportHeight = _scrollRect.viewport.rect.height;
            _visibleCount = Mathf.CeilToInt(viewportHeight / _itemHeight);

            // 풀 사이즈 결정 (화면에 표시될 개수 + 버퍼)
            // 단, 총 곡 수보다 많을 수 없으므로 Clamp
            _poolSize = Mathf.Min(_visibleCount + _bufferItems * 2, _totalSongCount);

            // contentRect의 높이
            float contentHeight = _totalSongCount * _itemHeight;
            _contentRect.sizeDelta = new Vector2(_contentRect.sizeDelta.x, contentHeight);

            // 아이템 풀 생성
            for (int i = 0; i < _poolSize; i++) {
                GameObject slotSong = Instantiate(_songItemPrefab, _contentRect);
                slotSong.name = $"SongItem_{i}";

                RectTransform rt = slotSong.GetComponent<RectTransform>();
                float yPos = -(i * _itemHeight);
                rt.anchoredPosition = new Vector2(0, yPos);

                // 초기 텍스트 및 배경
                UpdateSlotData(slotSong, i);
                _songList.AddLast(slotSong);
            }

            // 초기 스냅 + 재배치
            SnapToNearestSlot();
            OnScroll();
            // 스냅 직후, 가운데 슬롯 하이라이트
            HighlightCenterSlotByPosition();
        }

        private void LateUpdate() {
            if (_isScrolling) {
                if (Time.time - _lastScrollTime > _scrollDebounceTime) {
                    // 스크롤 멈췄다고 판단 -> 칸 정렬
                    _isScrolling = false;
                    SnapToNearestSlot();
                    OnScroll();

                    // 스냅 후 가운데 하이라이트
                    HighlightCenterSlotByPosition();
                }
            }
        }

        // 스크롤 중: 일단 부드럽게 이동만 (스냅은 늦게)
        private void OnScrolled(Vector2 pos) {
            _isScrolling = true;
            _lastScrollTime = Time.time;

            // HighlightCenterSlotByPosition();
        }

        // 칸 단위 스냅 + 끝 범위 Clamp
        private void SnapToNearestSlot() {
            float contentY = _contentRect.anchoredPosition.y;
            int nearestIndex = Mathf.RoundToInt(contentY / _itemHeight);

            // 최대 인덱스 = (총곡 - 화면에서 보이는 슬롯 수)
            // 곡이 적으면 음수가 될 수 있으니 0으로 Clamp
            int maxIndex = Mathf.Max(0, _totalSongCount - _visibleCount);
            nearestIndex = Mathf.Clamp(nearestIndex, 0, maxIndex);

            float newY = nearestIndex * _itemHeight;
            _contentRect.anchoredPosition = new Vector2(0, newY);
        }

        // 스크롤 위치 기반으로 슬롯 재배치
        
        private void OnScroll() {
            float contentY = _contentRect.anchoredPosition.y;
            // 한계치
            int newFirstIndex = Mathf.FloorToInt(contentY / _itemHeight) - _bufferItems;
            if (newFirstIndex < 0) newFirstIndex = 0;

            if (newFirstIndex != _firstVisibleIndexCached) {
                int diffIndex = newFirstIndex - _firstVisibleIndexCached;
                bool scrollDown = (diffIndex > 0);
                int shiftCount = Mathf.Abs(diffIndex);

                ShiftSlots(shiftCount, scrollDown);
                _firstVisibleIndexCached = newFirstIndex;
            }
        }

        private void ShiftSlots(int shiftCount, bool scrollDown) {
            for (int i = 0; i < shiftCount; i++) {
                if (scrollDown) {
                    // 맨 앞 슬롯 빼서 맨 뒤로
                    GameObject slot = _songList.First.Value;
                    _songList.RemoveFirst();
                    _songList.AddLast(slot);

                    int newIndex = _firstVisibleIndexCached + _poolSize + i;
                    if (newIndex >= _totalSongCount) {
                        slot.SetActive(false);
                        continue;
                    }
                    slot.SetActive(true);

                    RectTransform rt = slot.GetComponent<RectTransform>();
                    rt.anchoredPosition = CalculateSlotPosition(newIndex);
                    UpdateSlotData(slot, newIndex);

                } else {
                    // 맨 뒤 슬롯 빼서 맨 앞으로
                    GameObject slot = _songList.Last.Value;
                    _songList.RemoveLast();
                    _songList.AddFirst(slot);

                    int newIndex = _firstVisibleIndexCached - 1 - i;
                    if (newIndex < 0) {
                        slot.SetActive(false);
                        continue;
                    }
                    slot.SetActive(true);

                    RectTransform rt = slot.GetComponent<RectTransform>();
                    rt.anchoredPosition = CalculateSlotPosition(newIndex);
                    UpdateSlotData(slot, newIndex);
                }
            }
        }

        private Vector2 CalculateSlotPosition(int dataIndex) {
            float y = -(dataIndex * _itemHeight);
            return new Vector2(0, y);
        }

        private void HighlightCenterSlotByPosition() {

            // 1) 이전 슬롯 해제
            if (_selectedSlot != null) {
                var prevImg = _selectedSlot.GetComponentInChildren<Image>();
                if (prevImg == null) {
                    Debug.Log($"[Highlight] Could not find BG Image in {_selectedSlot.name}");
                } else {
                    prevImg.sprite = _defaultSprite;
                }
                _selectedSlot = null;
            }

            // 2) 화면 중앙 계산
            float contentY = _contentRect.anchoredPosition.y;
            float viewportH = _scrollRect.viewport.rect.height;
            float viewportCenterY = -(viewportH / 2f);

            GameObject closestSlot = null;
            float closestDist = float.MaxValue;

            // 3) 활성화된 슬롯 중에서 "뷰포트 중앙"과의 거리 계산 → 가장 작은 것
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

            // 4) 새 슬롯 하이라이트
            if (closestSlot != null) {
                var img = closestSlot.GetComponentInChildren<Image>();
                if (img == null) {
                } else {
                    img.sprite = _selectedSprite;
                }
                _selectedSlot = closestSlot;

                // 슬롯의 Text 컴포넌트에서 곡 제목 추출
                Text txt = closestSlot.GetComponentInChildren<Text>();
                string highlightedSongTitle = txt != null ? txt.text : string.Empty;

                if (!string.IsNullOrEmpty(highlightedSongTitle)) {
                    Debug.Log($"[SongListManager] Highlighted Song Title: {highlightedSongTitle}");
                    // 이벤트을 곡 제목으로 변경 (새로운 이벤트 생성)
                    OnHighlightedSongChanged?.Invoke(highlightedSongTitle);
                }
            }

        }


        // 슬롯에 표시할 텍스트/데이터
        private void UpdateSlotData(GameObject slot, int dataIndex) {
            if (dataIndex < 0 || dataIndex >= _totalSongCount) {
                slot.SetActive(false);
                return;
            }

            // 디버그용으로 이름 붙이기
            slot.name = $"SongItem_{dataIndex}"; 

            // 텍스트
            Text txt = slot.GetComponentInChildren<Text>();
            if (txt != null) {
                txt.text = songs[dataIndex].Title;
            }
            // 필요하다면 slot.name = $"Slot_{dataIndex}" 등으로 식별도 가능
        }
    }
}



// using System.Collections.Generic;
// using System.Collections;
// using UnityEngine;
// using UnityEngine.UI;

// namespace SongList
// {
//     public class SongListManager : MonoBehaviour
//     {
//         [Header("ScrollRect Settings")]
//         [SerializeField] private ScrollRect _scrollRect;
//         [SerializeField] private RectTransform _contentRect;

//         [Header("Slot Settings")]
//         [SerializeField] private GameObject _songItemPrefab;
//         [SerializeField] private int _totalSongCount;
//         [SerializeField] private int _bufferItems = 5;

//         [Header("Background Sprites")]
//         [SerializeField] private Sprite _selectedSprite;
//         [SerializeField] private Sprite _defaultSprite;

//         // 스크롤 중 여부 & 디바운스
//         private bool _isScrolling = false;
//         private float _lastScrollTime = 0f;
//         [SerializeField] private float _scrollDebounceTime = 0.05f; // 멈춘 뒤 50ms 후 스냅

//         private LinkedList<GameObject> _songList = new LinkedList<GameObject>();
//         private List<SongInfo> songs;
//         private int _poolSize;
//         private float _itemHeight;
//         private int _firstVisibleIndexCached = 0;
//         private int _visibleCount;
//         private GameObject _selectedSlot;

//         private void Awake() {
//             _itemHeight = _songItemPrefab.GetComponent<RectTransform>().sizeDelta.y;
//             songs = LoadManager.Instance.Songs;
//             _totalSongCount = songs.Count;

//             // onValueChanged에서 "SetScrollDirty" 대신 직접 메소드 연결
//             _scrollRect.onValueChanged.AddListener(OnScrolled);
//         }

//         private IEnumerator Start() {
//             yield return null;
//             float viewportHeight = _scrollRect.viewport.rect.height;
//             _visibleCount = Mathf.CeilToInt(viewportHeight / _itemHeight);
//             _poolSize = Mathf.Min(_visibleCount + _bufferItems * 2, _totalSongCount);

//             float contentHeight = _totalSongCount * _itemHeight;
//             _contentRect.sizeDelta = new Vector2(_contentRect.sizeDelta.x, contentHeight);

//             // 아이템 풀 생성
//             for (int i = 0; i < _poolSize; i++) {
//                 GameObject slotSong = Instantiate(_songItemPrefab, _contentRect);

//                 RectTransform rt = slotSong.GetComponent<RectTransform>();
//                 float yPos = -(i * _itemHeight);
//                 rt.anchoredPosition = new Vector2(0, yPos);

//                 Text txt = slotSong.GetComponentInChildren<Text>();
//                 Image bgImage = slotSong.GetComponentInChildren<Image>();

//                 if (txt != null) {
//                     if (i < songs.Count) {
//                         txt.text = songs[i].Title;
//                     } else {
//                         txt.text = $"Item #{i + 1}";
//                     }
//                 }
//                 if (bgImage != null) {
//                     bgImage.sprite = _defaultSprite;
//                 }
//                 _songList.AddLast(slotSong);
//             }

//             // 초기 상태에서 스냅 or 업데이트
//             SnapToNearestSlot();
//             OnScroll();
//         }

//         // 매 프레임 후반에 "스크롤 멈춤" 체크
//         private void LateUpdate() {
//             if (_isScrolling) {
//                 if (Time.time - _lastScrollTime > _scrollDebounceTime) {
//                     // 스크롤이 멈춘 것으로 판단 -> 스냅
//                     _isScrolling = false;
//                     SnapToNearestSlot();
//                     OnScroll();
//                 }
//             }
//         }

//         // 스크롤 값 변경 시(드래그 중 실시간)
//         private void OnScrolled(Vector2 pos) {
//             _isScrolling = true;
//             _lastScrollTime = Time.time;

//             // 즉시 스냅은 하지 않는다(부드러운 스크롤을 위해)
//             // 대신 실시간으로 Slot 재배치 + 가운데 슬롯 업데이트
//             OnScroll();
//         }

//         // 실제 "칸" 위치로 이동(드래그 멈춘 뒤 한 번 호출됨)
//         private void SnapToNearestSlot() {
//             float contentY = _contentRect.anchoredPosition.y;
//             int nearestIndex = Mathf.RoundToInt(contentY / _itemHeight);

//             float newY = nearestIndex * _itemHeight;
//             _contentRect.anchoredPosition = new Vector2(0, newY);
//         }

//         private void OnScroll() {
//             float contentY = _contentRect.anchoredPosition.y;
//             int newFirstIndex = Mathf.FloorToInt(contentY / _itemHeight) - _bufferItems;
//             if (newFirstIndex < 0) newFirstIndex = 0;

//             if (newFirstIndex != _firstVisibleIndexCached) {
//                 int diffIndex = newFirstIndex - _firstVisibleIndexCached;
//                 bool scrollDown = (diffIndex > 0);
//                 int shiftCount = Mathf.Abs(diffIndex);

//                 ShiftSlots(shiftCount, scrollDown);
//                 _firstVisibleIndexCached = newFirstIndex;
//             } else {
//                 // 인덱스 변화 없더라도 가운데 슬롯은 계속 업데이트
//                 UpdateSelectedSlot();
//             }
//         }

//         private void ShiftSlots(int shiftCount, bool scrollDown) {
//             for (int i = 0; i < shiftCount; i++) {
//                 if (scrollDown) {
//                     GameObject song = _songList.First.Value;
//                     _songList.RemoveFirst();
//                     _songList.AddLast(song);

//                     int newIndex = _firstVisibleIndexCached + _poolSize + i;
//                     if (newIndex >= _totalSongCount) {
//                         song.SetActive(false);
//                         continue;
//                     }
//                     newIndex = Mathf.Clamp(newIndex, 0, _totalSongCount - 1);

//                     RectTransform rt = song.GetComponent<RectTransform>();
//                     rt.anchoredPosition = CalculateSlotPosition(newIndex);

//                     UpdateSlotData(song, newIndex);
//                 } else {
//                     GameObject song = _songList.Last.Value;
//                     _songList.RemoveLast();
//                     _songList.AddFirst(song);

//                     int newIndex = _firstVisibleIndexCached - 1 - i;
//                     if (newIndex < 0) {
//                         song.SetActive(false);
//                         continue;
//                     }
//                     newIndex = Mathf.Clamp(newIndex, 0, _totalSongCount - 1);

//                     RectTransform rt = song.GetComponent<RectTransform>();
//                     rt.anchoredPosition = CalculateSlotPosition(newIndex);

//                     UpdateSlotData(song, newIndex);
//                 }
//             }
//             UpdateSelectedSlot();
//         }

//         private Vector2 CalculateSlotPosition(int index) {
//             float y = -(index * _itemHeight);
//             return new Vector2(0, y);
//         }

//         // 여기서 "4번째 슬롯(인덱스3)"을 선택
//         private void UpdateSelectedSlot() {
//             // 이전 selected 해제
//             if (_selectedSlot != null) {
//                 Image prevBg = _selectedSlot.GetComponentInChildren<Image>();
//                 if (prevBg != null) prevBg.sprite = _defaultSprite;
//                 _selectedSlot = null;
//             }

//             float contentY = _contentRect.anchoredPosition.y;

//             // "4번째 슬롯"이 실제로 viewport 상 얼마나 아래 있는지
//             // 예: 1번 슬롯이 y=0에 있다고 치면, 4번째 슬롯은 itemHeight * 3 만큼 내려간 지점
//             // (혹은 viewport 높이의 절반으로 계산할 수도 있음)
//             float centerPosY = contentY + _itemHeight * 3;

//             // 그 위치가 가리키는 "데이터 인덱스"
//             int centerDataIndex = Mathf.FloorToInt(centerPosY / _itemHeight);
//             centerDataIndex = Mathf.Clamp(centerDataIndex, 0, _totalSongCount - 1);

//             // 이제 "이 dataIndex를 실제로 표시 중인 슬롯"을 찾아야 함
//             // ShiftSlots()에서 "UpdateSlotData(song, newIndex)"로 할당했을 때,
//             // 예: slot.name = $"Slot_{newIndex}"; 또는 slot.GetComponent<SongSlot>().DataIndex = newIndex; 로 저장
//             // 여기선 예시로 slot.name 을 활용:
//             GameObject foundSlot = null;
//             var node = _songList.First;
//             while (node != null) {
//                 var slot = node.Value;
//                 if (slot.activeSelf && slot.name == $"Slot_{centerDataIndex}")
//                 {
//                     foundSlot = slot;
//                     break;
//                 }
//                 node = node.Next;
//             }

//             if (foundSlot != null) {
//                 Image bg = foundSlot.GetComponentInChildren<Image>();
//                 if (bg != null) bg.sprite = _selectedSprite;
//                 _selectedSlot = foundSlot;
//             }
//         }

//         private GameObject GetNthSlot(int n) {
//             if (_songList.Count <= n) return null;
//             var current = _songList.First;
//             for (int i = 0; i < n; i++) {
//                 if (current.Next == null) return null;
//                 current = current.Next;
//             }
//             return current.Value;
//         }

//         private void UpdateSlotData(GameObject slot, int dataIndex) {
//             if (dataIndex < 0 || dataIndex >= _totalSongCount) {
//                 slot.SetActive(false);
//                 return;
//             }
//             slot.SetActive(true);

//             Text txt = slot.GetComponentInChildren<Text>();
//             if (txt != null) {
//                 txt.text = songs[dataIndex].Title;
//             }
//         }
//     }
// }


// public GameObject itemPrefab;
// public RectTransform content;
// public Sprite selectedSprite;
// public Sprite defaultSprite;

// public int totalItems = 30;
// private float itemHeight = 83f;
// private int visibleItems = 7;
// private Queue<GameObject> itemPool = new Queue<GameObject>();

// private List<string> numberTexts;

// private void Awake() {
//     numberTexts = new List<string>();
//     for(int i = 0; i < totalItems; i++) {
//         numberTexts.Add("Song " + (i + 1));
//     }
// }

// public string GetNumberText(int index) {
//     if (index >= 0 && index < numberTexts.Count)
//         return numberTexts[index];
//     return string.Empty;
// }

// public GameObject itemPrefab;
// public RectTransform content;
// public Sprite selectedSprite;
// public Sprite defaultSprite;

// public int totalItems = 20;
// private float itemHeight = 83f;
// private int visibleItems = 7;
// private Queue<GameObject> itemPool = new Queue<GameObject>();

// private int startIndex = 0;
// private int middleIndex; // 화면 중간에 위치한 아이템 인덱스

// void Start()
// {
//     InitializePool();
//     UpdateVisibleItems();
// }

// void InitializePool() {
//     int poolSize = visibleItems * 2;
//     for (int i = 0; i < visibleItems + 3; i++) {
//         GameObject newItem = Instantiate(itemPrefab, content);
//         itemPool.Enqueue(newItem);
//         newItem.SetActive(false);
//     }

//     content.sizeDelta = new Vector2(content.sizeDelta.x, itemHeight * totalItems);
// }

// void UpdateVisibleItems() {

//     // Pool에서 아이템 가져오기
//     int endIndex = Mathf.Min(startIndex + visibleItems, totalItems);
//     int poolIndex = 0;
//     middleIndex = startIndex + visibleItems / 2;

//     foreach (GameObject item in itemPool)
//     {
//         if (startIndex + poolIndex >= totalItems || poolIndex >= visibleItems)
//         {
//             item.SetActive(false);
//             continue;
//         }

//         // 아이템 활성화 및 위치 업데이트
//         item.SetActive(true);
//         RectTransform rect = item.GetComponent<RectTransform>();
//         rect.anchoredPosition = new Vector2(0, -(startIndex + poolIndex) * itemHeight);

//         // 텍스트 업데이트
//         Text titleText = item.GetComponentInChildren<Text>();
//         titleText.text = "Song " + (startIndex + poolIndex + 1);

//         // 선택 상태 업데이트
//         Image bgImage = item.GetComponentInChildren<Image>();     
//         bgImage.sprite = (startIndex + poolIndex == middleIndex) ? selectedSprite : defaultSprite;

//         poolIndex++;
//     }
// }


// void Update()
// {
//     float scrollOffset = content.anchoredPosition.y;
//     float currentScrollIndex = Mathf.FloorToInt(scrollOffset / itemHeight);


//     // 아래로 스크롤 (위쪽 아이템 재활용)
//     if (currentScrollIndex > startIndex + visibleItems)
//     {
//         if (startIndex + visibleItems < totalItems)
//         {
//             startIndex++;
//             GameObject item = itemPool.Dequeue();
//             itemPool.Enqueue(item);
//             UpdateVisibleItems();
//         }
//     }

//     // 위로 스크롤 (아래쪽 아이템 재활용)
//     else if (currentScrollIndex < startIndex - 1)
//     {
//         if (startIndex > 0)
//         {
//             startIndex--;
//             GameObject item = itemPool.Dequeue();
//             itemPool.Enqueue(item);
//             UpdateVisibleItems();
//         }
//     }
