using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class SongListManager : MonoBehaviour
{

    [Header("UI References")]
    [SerializeField] private ScrollRect _songScrollRect;     // 스크롤 Rect
    [SerializeField] private RectTransform _contentRect;     // 콘텐츠 영역 (Vertical Layout Group 등 부착 가능)
    [SerializeField] private GameObject _songItemPrefab;     // 노래 아이템 프리팹

    [Header("Settings")]
    [SerializeField] private int _totalSongCount = 30;       // 실제 데이터(노래) 총 개수
    [SerializeField] private int _slotCount = 13;            // 화면에 유지할 슬롯(아이템) 개수
    [SerializeField] private float _itemHeight = 83f;        // 아이템 하나의 높이

    // 내부에서 활용될 배열 (슬롯 오브젝트 & 텍스트)
    private GameObject[] _slotObjects;  
    private Text[] _slotTexts;

    private void Start() {
        _contentRect.sizeDelta = new Vector2(
            _contentRect.sizeDelta.x,
            _itemHeight * _totalSongCount
        );

        _slotObjects = new GameObject[_slotCount];
        _slotTexts   = new Text[_slotCount];

        for (int i = 0; i < _slotCount; i++) {
            // 슬롯 프리팹 생성 (Content 하위)
            GameObject slotObj = Instantiate(_songItemPrefab, _contentRect);
            _slotObjects[i] = slotObj;

            // 슬롯 내부 Text 컴포넌트 참조 (노래 제목/정보 표시용)
            Text slotText = slotObj.GetComponentInChildren<Text>();
            _slotTexts[i] = slotText;

            // Step1에선 아직 스크롤 인덱스 계산 X,
            // 그냥 "Item #i (Step1)" 정도로 표시해둔다.
            if (slotText != null) {
                slotText.text = $"Item #{i + 1} (Step1)";
            }
        }
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
