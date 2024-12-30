using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace SongList
{
    public class SongListManager : MonoBehaviour
    {
        [Header("ScrollRect Settings")]
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _contentRect;


        [Header("Slot Settings")]
        [SerializeField] private GameObject _songItemPrefab;
        [SerializeField] private int _totalSongCount = 50;
        [SerializeField] private int _bufferItems = 2;
        // [SerializeField] private float _itemHeight = 85f;

        private LinkedList<GameObject> _songList = new LinkedList<GameObject>();
        private int _poolSize;
        private float _itemHeight;
        private int _firstVisibleIndexCached = 0;
        private bool _isScrolling = false;
        private float _scrollDebounceTime = 0.05f; // 50ms
        private float _lastScrollTime = 0f;
        private int _visibleCount;
        private void Awake() {
            // contentRect의 RectTransform 컴포넌트를 가져옴
            //_contentRect = _contentRect.GetComponent<RectTransform>();
            
            // itemPrefab의 높이를 계산
            _itemHeight = _songItemPrefab.GetComponent<RectTransform>().sizeDelta.y;
        }

        // UI의 초기화를 위한 코루틴
        private IEnumerator Start() {
            yield return null;
            // viewport의 높이를 계산
            float viewportHeight = _scrollRect.viewport.rect.height;

            // 한 번에 보여지는 아이템의 개수 계산
            _visibleCount = Mathf.CeilToInt(viewportHeight / _itemHeight);

            // 메모리에 생성할 아이템의 개수 계산
            _poolSize = _visibleCount + _bufferItems * 2;

            // contentRect의 높이 설정
            float contentHeight = _totalSongCount * _itemHeight;
            _contentRect.sizeDelta = new Vector2(_contentRect.sizeDelta.x, contentHeight);

            // 아이템 풀 초기화
            for (int i = 0; i < _poolSize; i++) {
                GameObject slotSong = Instantiate(_songItemPrefab, _contentRect);

                RectTransform rt = slotSong.GetComponent<RectTransform>();
                float yPos = -(i * _itemHeight);
                rt.anchoredPosition = new Vector2(0, yPos);

                Text txt = slotSong.GetComponentInChildren<Text>();
                if (txt != null) { 
                    txt.text = $"Item #{i + 1}";
                }
                _songList.AddLast(slotSong);
            }

            // 스크롤 이벤트 플래그 체크
            _scrollRect.onValueChanged.AddListener(SetScrollDirty);

            Debug.Log($"Viewport height = {viewportHeight}");
            Debug.Log($"ItemHeight = {_itemHeight}");
            Debug.Log($"=> visibleCount = {_visibleCount}, poolSize={_poolSize}");
        }

        // private void Start() {
        //     // viewport의 높이를 계산
        //     float viewportHeight = _scrollRect.viewport.rect.height;

        //     // 한 번에 보여지는 아이템의 개수를 계산
        //     int visibleCount = Mathf.CeilToInt(viewportHeight / _itemHeight);

        //     // 메모리에 생성할 아이템의 개수를 계산
        //     _poolSize = visibleCount + _bufferItems * 2;

        //     // contentRect의 높이를 계산
        //     float contentHeight = _totalSongCount * _itemHeight;

        //     // contentRect의 높이를 설정
        //     _contentRect.sizeDelta = new Vector2(_contentRect.sizeDelta.x, contentHeight);

        //     // 아이템 풀 초기화
        //     for (int i = 0; i < _poolSize; i++) {
        //         GameObject slotSong = Instantiate(_songItemPrefab, _contentRect);

        //         RectTransform rt = slotSong.GetComponent<RectTransform>();
        //         float yPos = -(i * _itemHeight);
        //         rt.anchoredPosition = new Vector2(0, yPos);

        //         Text txt = slotSong.GetComponentInChildren<Text>();
        //         if (txt != null) { 
        //             txt.text = $"Item #{i + 1}";
        //         }
        //         _songList.AddLast(slotSong);
        //     }
        //     // SlotInit();
        //     // 스크롤 이벤트 플래그 체크
        //     _scrollRect.onValueChanged.AddListener(SetScrollDirty);

        //     Debug.Log($"Viewport height = {viewportHeight}"); 
        //     Debug.Log($"ItemHeight = {_itemHeight}");
        //     Debug.Log($"=> visibleCount = {visibleCount}, poolSize={_poolSize}");

        // }

        private void LateUpdate() {
            if(_isScrolling) {
                if(Time.time - _lastScrollTime > _scrollDebounceTime) {
                    _isScrolling = false;
                    SnapToNearestSlot();
                    OnScroll();
                }
            }
        }

        private void SnapToNearestSlot() {
            float contentY = _contentRect.anchoredPosition.y;
            int nearestIndex = Mathf.RoundToInt(contentY / _itemHeight);
            float newY = nearestIndex * _itemHeight;
            _contentRect.anchoredPosition = new Vector2(0, newY);
        }

        private void SetScrollDirty(Vector2 pos) {
            _lastScrollTime = Time.time;
            _isScrolling = true;
        }

        private void OnScroll() {
            float contentY = _contentRect.anchoredPosition.y;

            // ShiftSlots에서 Clamp로 제한하고 있지만, 오류 탐색을 위해 여기서도 한 번 더 제한
            int newFirstIndex = Mathf.Max(0, Mathf.FloorToInt(contentY / _itemHeight) - _bufferItems); // 새로 보여줘야 할 첫 인덱스 계산
            
            if (newFirstIndex < 0) {
                newFirstIndex = 0;
            }

            if (newFirstIndex != _firstVisibleIndexCached) {
                int diffIndex = _firstVisibleIndexCached - newFirstIndex;
                bool scrollDown = (diffIndex < 0);

                int shiftCount = Mathf.Abs(diffIndex);
                ShiftSlots(shiftCount, scrollDown, newFirstIndex);

                _firstVisibleIndexCached = newFirstIndex;
            }
        }

        private void ShiftSlots(int shiftCount, bool scrollDown, int newFirstIndex) {

            for (int i = 0; i < shiftCount; i++) {
                if (scrollDown) {
                    GameObject song = _songList.First.Value;
                    song.SetActive(true);
                    _songList.RemoveFirst();
                    _songList.AddLast(song);

                    int newIndex = _firstVisibleIndexCached + _poolSize + i;
                    if (newIndex >= _totalSongCount) {
                        song.SetActive(false);
                        break;
                    }

                    // int newIndex = newFirstIndex + _visibleCount + i;
                    newIndex = Mathf.Clamp(newIndex, 0, _totalSongCount - 1);
                    Debug.Log($"new Index = {newIndex}");

                    RectTransform rt = song.GetComponent<RectTransform>();
                    rt.anchoredPosition = CalculateSlotPoisition(newIndex);

                    UpdateSlotData(song, newIndex);
                } else {
                    GameObject song = _songList.Last.Value;
                    _songList.RemoveLast();
                    _songList.AddFirst(song);

                    int newIndex = _firstVisibleIndexCached - i;

                    if (newIndex < 0) {
                        song.SetActive(false);
                        break;
                    }
                    // int newIndex = newFirstIndex - i;
                    newIndex = Mathf.Clamp(newIndex, 0, _totalSongCount - 1);

                    RectTransform rt = song.GetComponent<RectTransform>();
                    rt.anchoredPosition = CalculateSlotPoisition(newIndex);

                    UpdateSlotData(song, newIndex);
                }
            }
            // HideSlotsOutsideViewport();


            // LinkedListNode<GameObject> firstNode = _songList.First;
            // LinkedListNode<GameObject> lastNode = _songList.Last;

            // if (scrollDown) {
            //     for (int i = 0; i < shiftCount; i++) {
            //         GameObject slotSong = firstNode.Value;
            //         RectTransform rt = slotSong.GetComponent<RectTransform>();
            //         float yPos = lastNode.Value.GetComponent<RectTransform>().anchoredPosition.y - _itemHeight;
            //         rt.anchoredPosition = new Vector2(0, yPos);

            //         Text txt = slotSong.GetComponentInChildren<Text>();
            //         if (txt != null) {
            //             txt.text = $"Item #{newFirstIndex + _poolSize + i + 1}";
            //         }

            //         _songList.RemoveFirst();
            //         _songList.AddLast(slotSong);
            //         firstNode = _songList.First;
            //         lastNode = _songList.Last;
            //     }
            // } else {
            //     for (int i = 0; i < shiftCount; i++) {
            //         GameObject slotSong = lastNode.Value;
            //         RectTransform rt = slotSong.GetComponent<RectTransform>();
            //         float yPos = firstNode.Value.GetComponent<RectTransform>().anchoredPosition.y + _itemHeight;
            //         rt.anchoredPosition = new Vector2(0, yPos);

            //         Text txt = slotSong.GetComponentInChildren<Text>();
            //         if (txt != null) {
            //             txt.text = $"Item #{newFirstIndex - i}";
            //         }

            //         _songList.RemoveLast();
            //         _songList.AddFirst(slotSong);
            //         firstNode = _songList.First;
            //         lastNode = _songList.Last;
            //     }
            // }
        }

        // private void HideSlotsOutsideViewport() {
        //     float viewportHeight = _scrollRect.viewport.rect.height;
        //     float contentY = _contentRect.anchoredPosition.y;

        //     foreach (var slot in _songList) {
        //         RectTransform rt = slot.GetComponent<RectTransform>();
        //         float slotY = rt.anchoredPosition.y;

        //         if (slotY < 0f || slotY > viewportHeight) {
        //             slot.SetActive(false);
        //         } else {
        //             slot.SetActive(true);
        //         }
        //     }
        // }

        private Vector2 CalculateSlotPoisition(int index) {
            float y = -(index * _itemHeight);
            return new Vector2(0, y);
        }

        private void UpdateSlotData(GameObject slot, int dataIndex) {
            if (dataIndex < 0 || dataIndex >= _totalSongCount) {
                slot.SetActive(false);
            } else {
                slot.SetActive(true);
                Text txt = slot.GetComponentInChildren<Text>();
                if (txt != null) {
                    txt.text = $"Item #{dataIndex + 1}";
                }
            }
        }





        // Onscroll 아직 수정안했음(나중에 고쳐야 함)
        // 스크롤 이벤트가 발생할 때마다 호출되는 함수
        // private void OnScroll(Vector2 scrollPosition) {
        //     float contentY = _contentRect.anchoredPosition.y;
        //     int firstVisibleIndex = Mathf.Max(0, Mathf.FloorToInt(contentY / _itemHeight) - _bufferItems);
        //     float currentScrollIndex = Mathf.FloorToInt(contentY / _itemHeight);

        //     int startIndex = (int)currentScrollIndex;
        //     int endIndex = startIndex + _bufferItems;

        //     int index = 0;
        //     foreach (GameObject song in _songList) {
        //         if (index >= startIndex && index < endIndex) {
        //             song.SetActive(true);
        //         } else {
        //             song.SetActive(false);
        //         }
        //         index++;
        //     }
        // }


        // private void SlotInit() {
        //     int index = 0;
        //     foreach (GameObject song in _songList) {
        //         song.transform.localPosition = new Vector3(0, (-index * _itemHeight) -_itemHeight, 0);
        //         song.GetComponentInChildren<Text>().text = $"Item #{index}";
        //         index++;
        //     }
        // }
    }
}

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
