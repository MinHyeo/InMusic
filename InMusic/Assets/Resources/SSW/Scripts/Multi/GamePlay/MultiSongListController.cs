using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using SSW.DB;
using SongList;

public class MultiSongListController : MonoBehaviour
{
    #region Variables
    [Header("ScrollRect Settings")]
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _contentRect;

    [Header("Slot Settings")]
    [SerializeField] private GameObject _songItemPrefab;
    [SerializeField] private int _bufferItems = 10;

    [Header("Background Sprites")]
    [SerializeField] private Sprite _selectedSprite;
    [SerializeField] private Sprite _defaultSprite;

    [Header("Scrolling Settings")]
    [SerializeField] private float _scrollDebounceTime = 0.05f;

    private bool _isScrolling = false;
    private float _lastScrollTime = 0f;

    private int _totalSongCount;
    private int _poolSize;
    private float _itemHeight;
    private int _visibleCount;
    private int _firstVisibleIndexCached = -10;

    private float upKeyTimer = 0f;
    private float downKeyTimer = 0f;
    private const float initialDelay = 0.3f;
    private const float repeatRate = 0.1f;

    private List<GameObject> _songList = new List<GameObject>();
    private List<SongInfo> _songs;
    private GameObject _selectedSlot;
    public event Action<SongInfo> OnHighlightedSongChanged;
    private SingleMenuController _singleMenuController;

    private Dictionary<string, MusicLogRecord> _musicLogRecords;
    private bool _isRestoringPosition = false;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _itemHeight = _songItemPrefab.GetComponent<RectTransform>().sizeDelta.y;
        Debug.Log($"[SongListManager] Item Height: {_itemHeight}");
        _scrollRect.onValueChanged.AddListener(OnScrolled);
    }

    private IEnumerator Start()
    {
        yield return null;

        // LoadManager에서 이미 모든 곡을 로드했으므로 바로 사용
        _songs = LoadManager.Instance.Songs;
        _totalSongCount = _songs.Count;
        Debug.Log($"[MultiSongListController] Total Songs Count: {_totalSongCount}");

        // 플레이 기록은 별도로 로드 (DB가 있을 때만)
        DBService db = FindFirstObjectByType<DBService>();
        if (db != null)
        {
            string userId = Steamworks.SteamUser.GetSteamID().m_SteamID.ToString();
            bool logsLoaded = false;
            db.LoadAllMusicLogs(userId, (dict) =>
            {
                _musicLogRecords = dict;
                logsLoaded = true;
            });
            while (!logsLoaded)
                yield return null;
            if (_musicLogRecords != null)
            {
                Debug.Log($"[MultiSongListController] Loaded {_musicLogRecords.Count} music log records.");
            }
            else
            {
                Debug.LogWarning("[MultiSongListController] Failed to load music log records.");
            }
        }
        else
        {
            Debug.LogWarning("[MultiSongListController] DBService not found. Play records will be unavailable.");
            _musicLogRecords = new Dictionary<string, MusicLogRecord>();
        }

        float viewportHeight = _scrollRect.viewport.rect.height;
        _visibleCount = Mathf.CeilToInt(viewportHeight / _itemHeight);

        _poolSize = _visibleCount + _bufferItems * 2;
        Debug.Log($"[SongListManager] Pool Size: {_poolSize}");

        float contentHeight = _visibleCount * _itemHeight;
        _contentRect.sizeDelta = new Vector2(_contentRect.sizeDelta.x, contentHeight);
        _contentRect.anchoredPosition = new Vector2(0, 0);

        for (int i = 0; i < _poolSize; i++)
        {
            GameObject slotSong = Instantiate(_songItemPrefab, _contentRect);
            slotSong.name = $"SongItem_{i}";

            RectTransform rt = slotSong.GetComponent<RectTransform>();
            float yPos = -(i * _itemHeight);
            rt.anchoredPosition = new Vector2(0, yPos + (_bufferItems * _itemHeight));

            UpdateSlotData(slotSong, i);
            _songList.Add(slotSong);
        }

        int rememberedIndex = IndexSaveTest.Instance.GetLastSelectedIndex();
        Debug.Log($"[SongListManager] Remembered Index: {rememberedIndex}");

        Canvas.ForceUpdateCanvases();

        if (rememberedIndex >= 0)
        {
            ForceCenterAtIndex(rememberedIndex);
        }
        else
        {
            SnapToNearestSlot();
            _firstVisibleIndexCached = Mathf.FloorToInt(_contentRect.anchoredPosition.y / _itemHeight);
            OnScroll();
            HighlightCenterSlotByPosition(isImmediate: true);
        }
        _singleMenuController = FindFirstObjectByType<SingleMenuController>();
    }

    private void OnEnable()
    {
        GameManager.SingleMenuInput.keyAction += OnKeyPress;
        Debug.Log("SongList Input Enabled");
    }

    private void OnDisable()
    {
        GameManager.SingleMenuInput.keyAction -= OnKeyPress;
        Debug.Log("SongList Input Disabled");
    }

    private void Update()
    {
        if (_isScrolling)
        {
            // 마스터 클라이언트가 아니면 스크롤 강제 복원
            if (!SharedModeMasterClientTracker.IsLocalPlayerSharedModeMasterClient()) 
            {
                // 스크롤을 원래 위치로 되돌림
                if (!_isRestoringPosition)
                {
                    _isRestoringPosition = true;
                    StartCoroutine(RestoreScrollPosition());
                }
            }
            if (Time.time - _lastScrollTime > _scrollDebounceTime)
            {
                _isScrolling = false;
                SnapToNearestSlot();
                OnScroll();
                if (SharedModeMasterClientTracker.IsLocalPlayerSharedModeMasterClient())
                {
                    HighlightCenterSlotByPosition();
                }
            }
        }
    }
    #endregion

    #region Scroll Methods
    private void OnScrolled(Vector2 pos)
    {       
        _isScrolling = true;
        _lastScrollTime = Time.time;
    }

    private IEnumerator RestoreScrollPosition()
    {
        yield return new WaitForSeconds(3.0f);

        if (SongSelectionNetwork.Instance != null)
        {
            int networkIndex = SongSelectionNetwork.Instance.GetCurrentSongIndex();
            ForceCenterAtIndex(networkIndex);
        }
        
        _isRestoringPosition = false;
    }

    private void SnapToNearestSlot()
    {
        float topOffset = _bufferItems * _itemHeight;
        float contentY = _contentRect.anchoredPosition.y;
        float tempOffset = contentY - topOffset;
        Debug.Log($"[SongListManager] Snap to Nearest Slot: {tempOffset}");
        int nearestIndex = Mathf.RoundToInt(tempOffset / _itemHeight);

        float newY = nearestIndex * _itemHeight + topOffset;
        Debug.Log($"[SongListManager] Snap to Index: {nearestIndex}");
        _contentRect.anchoredPosition = new Vector2(0, newY);
    }

    private void OnScroll()
    {
        float contentY = _contentRect.anchoredPosition.y;
        int newFirstIndex = Mathf.FloorToInt(contentY / _itemHeight);
        Debug.Log($"New First Index: {newFirstIndex}");
        if (newFirstIndex != _firstVisibleIndexCached)
        {
            int diffIndex = newFirstIndex - _firstVisibleIndexCached;
            bool scrollDown = (diffIndex > 0);
            int shiftCount = Mathf.Abs(diffIndex);

            ShiftSlots(shiftCount, scrollDown);
            _firstVisibleIndexCached = newFirstIndex;
            Debug.Log($"[SongListManager] First Index: {_firstVisibleIndexCached}");
        }
    }

    private void ShiftSlots(int shiftCount, bool scrollDown)
    {
        for (int i = 0; i < shiftCount; i++)
        {
            if (scrollDown)
            {
                GameObject slot = _songList[0];
                _songList.RemoveAt(0);
                _songList.Add(slot);
                Debug.Log("_songList.Length: " + _songList.Count);

                int newIndex = _firstVisibleIndexCached + _poolSize + i;
                int realIndex = ((newIndex % _totalSongCount) + _totalSongCount) % _totalSongCount;

                slot.SetActive(true);
                RectTransform rt = slot.GetComponent<RectTransform>();
                rt.anchoredPosition = CalculateSlotPosition(newIndex);
                UpdateSlotData(slot, realIndex);
            }
            else
            {
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

    private Vector2 CalculateSlotPosition(int dataIndex)
    {
        float topOffset = _bufferItems * _itemHeight;
        float y = -(dataIndex * _itemHeight) + topOffset;
        Debug.Log($"[SongListManager] Calculate Slot Position: {dataIndex} -> {y}");
        return new Vector2(0, y);
    }
    #endregion

    #region Keyboard Input
    private void OnKeyPress()
    {
        if (!SharedModeMasterClientTracker.IsLocalPlayerSharedModeMasterClient()) return;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                ScrollUp();
                HighlightCenterSlotByPosition();
                upKeyTimer = 0f;
            }
            upKeyTimer += Time.deltaTime;
            if (upKeyTimer >= initialDelay)
            {
                ScrollUp();
                HighlightCenterSlotByPosition();
                upKeyTimer -= repeatRate;
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ScrollDown();
                HighlightCenterSlotByPosition();
                downKeyTimer = 0f;
            }
            downKeyTimer += Time.deltaTime;
            if (downKeyTimer >= initialDelay)
            {
                ScrollDown();
                HighlightCenterSlotByPosition();
                downKeyTimer -= repeatRate;
            }
        }
        else
        {
            upKeyTimer = 0f;
            downKeyTimer = 0f;
        }
    }

    private void ScrollUp()
    {
        Vector2 pos = _contentRect.anchoredPosition;
        pos.y -= _itemHeight;
        _contentRect.anchoredPosition = pos;
        OnScroll();
    }

    private void ScrollDown()
    {
        Vector2 pos = _contentRect.anchoredPosition;
        pos.y += _itemHeight;
        _contentRect.anchoredPosition = pos;
        OnScroll();
    }
    #endregion

    #region Highlighting
    private void HighlightCenterSlotByPosition(bool isImmediate = false)
    {
        if (_selectedSlot != null)
        {
            ScrollSlot oldSlotComp = _selectedSlot.GetComponent<ScrollSlot>();
            if (oldSlotComp != null)
            {
                oldSlotComp.SetHighlight(false, isImmediate);
            }
            _selectedSlot = null;
        }

        float contentY = _contentRect.anchoredPosition.y;
        float viewportH = _scrollRect.viewport.rect.height;
        float viewportCenterY = -(viewportH / 2f);

        GameObject closestSlot = null;
        float closestDist = float.MaxValue;

        foreach (var slotObj in _songList)
        {
            if (!slotObj.activeSelf) continue;

            RectTransform rt = slotObj.GetComponent<RectTransform>();
            float slotY = rt.anchoredPosition.y + contentY;
            float dist = Mathf.Abs(slotY - viewportCenterY);

            if (dist < closestDist)
            {
                closestDist = dist;
                closestSlot = slotObj;
            }
        }

        if (closestSlot != null)
        {
            if (closestSlot == _selectedSlot) return;
            _selectedSlot = closestSlot;

            ScrollSlot slotComponent = closestSlot.GetComponent<ScrollSlot>();
            if (slotComponent != null)
            {
                slotComponent.SetHighlight(true, isImmediate);

                SongInfo highlightSong = slotComponent.GetHighlightedSong();
                if (highlightSong != null)
                {
                    Debug.Log($"[SongListManager] Highlighted Song Title: {highlightSong.Title}");
                    OnHighlightedSongChanged?.Invoke(highlightSong);
                    
                    // 네트워크 동기화 추가
                    if (SharedModeMasterClientTracker.IsLocalPlayerSharedModeMasterClient())
                    {
                        int currentCenterIndex = GetCurrentCenterSongIndex();
                        if (SongSelectionNetwork.Instance != null)
                        {
                            SongSelectionNetwork.Instance.UpdateSongIndex(currentCenterIndex);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("[SongListManager] highlightSong is null");
                }
            }
            else
            {
                Debug.LogWarning("[SongListManager] closestSlot has no ScrollSlot component attached?");
            }
        }
    }

    public Dictionary<string, MusicLogRecord> MusicLogRecords
    {
        get { return _musicLogRecords; }
    }
    #endregion

    #region Slot Data Setting
    private void UpdateSlotData(GameObject slotObj, int dataIndex)
    {
        int songIndex = ((dataIndex % _totalSongCount) + _totalSongCount) % _totalSongCount;
        SongInfo currentSong = _songs[songIndex];

        ScrollSlot slot = slotObj.GetComponent<ScrollSlot>();
        if (slot != null)
        {
            slot.SetData(currentSong, songIndex, _musicLogRecords);
        }
    }

    public void ForceCenterAtIndex(int index)
    {
        // 중복 처리 방지
        int currentCenterIndex = GetCurrentCenterSongIndex();
        if (currentCenterIndex == index)
        {
            Debug.Log($"[MultiSongListController] Already centered at index {index}, skipping");
            return;
        }

        float topOffset = _bufferItems * _itemHeight;
        int centerSlotIndex = _visibleCount / 2;
        float targetContentY = (index - centerSlotIndex) * _itemHeight - topOffset;
        _contentRect.anchoredPosition = new Vector2(0, targetContentY);

        int newFirstIndex = Mathf.FloorToInt(targetContentY / _itemHeight);
        ReLayoutAllSlots(newFirstIndex);

        SnapToNearestSlot();
        HighlightCenterSlotByPosition(isImmediate: true);
    }

    private void ReLayoutAllSlots(int startIndex)
    {
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject slotObj = _songList[i];

            int dataIndex = startIndex + i;
            int realIndex = ((dataIndex % _totalSongCount) + _totalSongCount) % _totalSongCount;

            RectTransform rt = slotObj.GetComponent<RectTransform>();
            rt.anchoredPosition = CalculateSlotPosition(dataIndex);

            UpdateSlotData(slotObj, realIndex);
        }

        _firstVisibleIndexCached = startIndex;
    }

    #endregion

    #region Network Methods

    /// <summary>
    /// 현재 화면 중앙에 있는 곡의 인덱스를 계산
    /// </summary>
    private int GetCurrentCenterSongIndex()
    {
        int centerSlotIndex = _visibleCount / 2;
        int centerDataIndex = _firstVisibleIndexCached + centerSlotIndex;
        int realIndex = ((centerDataIndex % _totalSongCount) + _totalSongCount) % _totalSongCount;
        return realIndex;
    }

    /// <summary>
    /// 현재 하이라이트된 곡 정보 반환 (GameStartManager에서 사용)
    /// </summary>
    public SongInfo GetCurrentHighlightedSong()
    {
        if (_selectedSlot != null)
        {
            ScrollSlot slotComponent = _selectedSlot.GetComponent<ScrollSlot>();
            if (slotComponent != null)
            {
                return slotComponent.GetHighlightedSong();
            }
        }
        return null;
    }

    /// <summary>
    /// 현재 하이라이트된 곡 이름 반환 (GameStartManager에서 사용)
    /// </summary>
    public string GetCurrentHighlightedSongName()
    {
        var highlightedSong = GetCurrentHighlightedSong();
        return highlightedSong?.Title ?? "Unknown";
    }
    #endregion
}