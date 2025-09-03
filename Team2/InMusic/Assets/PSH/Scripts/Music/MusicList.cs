using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicList : MonoBehaviour
{
    [Header("��� ����")]
    [Tooltip("�׸�")]
    [SerializeField] private GameObject[] musicItems = new GameObject[17];
    [Tooltip("���� ������ �׸�")]
    [SerializeField] GameObject curItem;
    [Tooltip("�׸� ��, ����, ���� ��ġ")]
    private int numOfitems;
    [Tooltip("����, ���� ��ġ")]
    private float itemGap = 40.0f;
    [Tooltip("���� ��ġ(-6��° �׸� �� �������� index��ȣ)")]
    [SerializeField] private int startIndex = 0;
    [Tooltip("�� �׸� �Ҵ��� ������")]
    [SerializeField] List<MusicData> musicDataList = new List<MusicData>();
    [Header("��ũ�� ���� ����")]
    [Tooltip("Content ��ġ")]
    [SerializeField] private RectTransform contentPos;
    [Tooltip("Content �̵� ��ġ")]
    Vector2 dest;
    [Tooltip("Content �̵� �ӵ�")]
    float duration = 0.3f;
    [Tooltip("Content �̵� ���� ����")]
    bool isScrolling = false;

    public bool IsScrolling { get { return isScrolling; } }
    public GameObject SelectedItem { get { return curItem; } }

    public void SetData(List<MusicData> DataModel) {
        if (DataModel == null) {
            Debug.Log("���� ��� �Ѱܹޱ� ����");
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
        //Content �̵�
        contentPos.localPosition = new Vector2(0, 200.0f);
        //��� ����
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
        //Content �̵�
        contentPos.localPosition = new Vector2(0, 200.0f);
        //��� ����
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

    //�ε巴�� �̵�: ���콺(��ũ��)/Ű���� ����
    IEnumerator SmoothScrollMove()
    {
        isScrolling = true;

        //Content ��ġ ����
        if (contentPos.localPosition.y < 40.0f)
        {
            ContentDown();
            dest = contentPos.localPosition;
            dest -= new Vector2(0, itemGap);
        }
        //��ũ���� ������
        else if (contentPos.localPosition.y >= 400.0f)
        {
            ContentUp();
            dest = contentPos.localPosition;
            dest += new Vector2(0, itemGap);
        }

        //�̵�
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            contentPos.localPosition = Vector2.MoveTowards(contentPos.localPosition, dest, itemGap * (Time.deltaTime / duration));
            yield return null;
        }
        //��ġ ����
        contentPos.localPosition = dest;

        isScrolling = false;
    }

    #endregion

}
