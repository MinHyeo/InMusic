using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicList : MonoBehaviour
{
    [Header("목록 관련")]
    [Tooltip("항목")]
    [SerializeField] private GameObject[] musicItems = new GameObject[17];
    [Tooltip("현재 선택한 항목")]
    [SerializeField] GameObject curItem;
    [Tooltip("항목 수, 간격, 시작 위치")]
    private int numOfitems;
    [Tooltip("간격, 시작 위치")]
    private float itemGap = 40.0f;
    [Tooltip("시작 위치(-6번째 항목에 들어갈 데이터의 index번호)")]
    [SerializeField] private int startIndex = 0;
    [Tooltip("각 항목에 할당할 데이터")]
    [SerializeField] List<MusicData> musicDataList = new List<MusicData>();
    [Header("스크롤 관련 변수")]
    [Tooltip("Content 위치")]
    [SerializeField] private RectTransform contentPos;
    [Tooltip("Content 이동 위치")]
    Vector2 dest;
    [Tooltip("Content 이동 속도")]
    float duration = 0.3f;
    [Tooltip("Content 이동 상태 여부")]
    bool isScrolling = false;

    public bool IsScrolling { get { return isScrolling; } }
    public GameObject SelectedItem { get { return curItem; } }

    public void SetData(List<MusicData> DataModel) {
        if (DataModel == null) {
            Debug.Log("음악 목록 넘겨받기 실패");
            return;
        }
        musicDataList = DataModel;
        numOfitems = musicDataList.Count;

        ContentDown();
    }

    public void UpdateItems(MusicItem oldItem, MusicData newItem)
    {
        oldItem.DirPath = newItem.DirPath;
        oldItem.IsDummy = false;
        //Debug.Log(newItem.BMS.header.title);
        if (newItem.HasBMS)
        {
            oldItem.Title.text = newItem.BMS.header.title;
            oldItem.Artist.text = newItem.BMS.header.artist;
        }
        else
        {
            oldItem.Title.text = "EmptyItem";
            oldItem.Artist.text = "Empty";
            oldItem.IsDummy = true;
        }
        oldItem.Length = newItem.Length;
        oldItem.Album.sprite = newItem.Album;
        oldItem.Audio = newItem.Audio;
        oldItem.MuVi = newItem.MuVi;
        oldItem.HasMV = newItem.HasMV;
        oldItem.HasBMS = newItem.HasBMS;
        oldItem.Score = newItem.Score;
        oldItem.Accuracy = newItem.Accuracy + "%";
        oldItem.Combo = newItem.Combo;
        oldItem.Rank.text = newItem.Rank;
        
        oldItem.Data = newItem;
        oldItem.MusicID = newItem.MusicID;
    }


    #region Detect
    /*
    void OnTriggerEnter2D(Collider2D listItem)
    {
        curItem = listItem.gameObject;
        if (curItem == null)
        {
            Debug.Log("");
        }
        curItem.GetComponent<Music_Item>().ItemSelect();
    }

    private void OnTriggerExit2D(Collider2D listItem)
    {
        listItem.gameObject.GetComponent<Music_Item>().ItemUnselect();
    }
    */
    #endregion

    #region ContectMoving
    public void ContentDown()
    {
        //Content 이동
        contentPos.localPosition = new Vector2(0, 200.0f);
        //목록 갱신
        startIndex -= 5;
        if (startIndex < 0)
        {
            startIndex = numOfitems + startIndex;
        }
        for (int i = 0; i < musicItems.Length; i++)
        {
            UpdateItems(musicItems[i].GetComponent<MusicItem>(), musicDataList[startIndex++]);
            if (startIndex >= numOfitems)
            {
                startIndex = 0;
            }
        }
    }

    public void ContentUp()
    {
        //Content 이동
        contentPos.localPosition = new Vector2(0, 200.0f);
        //목록 갱신
        startIndex -= 12; // 12 = 17 - 5
        if (startIndex < 0)
        {
            startIndex = numOfitems + startIndex;
        }
        for (int i = 0; i < musicItems.Length; i++)
        {
            UpdateItems(musicItems[i].GetComponent<MusicItem>(), musicDataList[startIndex++]);
            if (startIndex >= numOfitems)
            {
                startIndex = 0;
            }
        }
    }

    public void ScrollDown()
    {
        dest = contentPos.localPosition;
        dest += new Vector2(0, itemGap);
        StartCoroutine(SmoothScrollMove());
    }

    public void ScrollUp()
    {
        dest = contentPos.localPosition;
        dest -= new Vector2(0, itemGap);
        StartCoroutine(SmoothScrollMove());
    }

    //부드럽게 이동: 마우스(스크롤)/키보드 조작
    IEnumerator SmoothScrollMove()
    {
        isScrolling = true;

        //Content 위치 수정
        if (contentPos.localPosition.y < 40.0f)
        {
            ContentDown();
            dest = contentPos.localPosition;
            dest -= new Vector2(0, itemGap);
        }
        //스크롤을 내리면
        else if (contentPos.localPosition.y >= 400.0f)
        {
            ContentUp();
            dest = contentPos.localPosition;
            dest += new Vector2(0, itemGap);
        }

        //이동
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            contentPos.localPosition = Vector2.MoveTowards(contentPos.localPosition, dest, itemGap * (Time.deltaTime / duration));
            yield return null;
        }
        //위치 보정
        contentPos.localPosition = dest;

        isScrolling = false;
    }

    #endregion

}
