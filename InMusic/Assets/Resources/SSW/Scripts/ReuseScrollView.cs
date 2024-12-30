using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ReuseScrollView : MonoBehaviour
{
    public GameObject orgItemPrefab;
    public float itemHeight = 200.0f;
    public List<int> dataList;

    private ScrollRect _scroll;
    private List<GameObject> itemList;

    private void Awake() {
        _scroll = GetComponent<ScrollRect>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dataList.Clear();
        for(int i = 0; i < 10000; i++) {
            dataList.Add(i);
        }
    }

    private void CreateItem() {
        RectTransform scrollRect = _scroll.GetComponent<RectTransform>();
        itemList = new List<GameObject>();

        int itemCount = (int)(scrollRect.rect.height / itemHeight) + 3;
        for(int i = 0; i < itemCount; i++) {
            GameObject item = Instantiate(orgItemPrefab, _scroll.content);
            item.transform.localPosition = new Vector3(0, -i * itemHeight, 0);
            item.SetActive(true);
            itemList.Add(item);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
