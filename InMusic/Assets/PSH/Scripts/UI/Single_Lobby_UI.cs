using UnityEngine;
using UnityEngine.UI;
using UI_BASE_PSH;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class Single_Lobby_UI : UI_Base
{
    [SerializeField] private GameObject[] musicItems = new GameObject[17];
    [SerializeField] private GameObject curMusicItem;
    [SerializeField] private GameObject[] curMusicData = new GameObject[4];
    [SerializeField] private Text[] logData = new Text[4];
    //스크롤 관련
    [SerializeField]private RectTransform contentPos;
    [SerializeField] List<string> musicList = new List<string>();
    private float itemGap = 40.0f;
    private int  numOfitems;
    [SerializeField] private int startIndex = 0;
    Vector2 dest;
    float duration = 0.3f;

    void Start()
    {
        //음악 목록 Load하기
        musicList = GameManager.Resource.GetMusicList();
        numOfitems = musicList.Count;
        ScrollUp();

        GameManager.Input.SetUIKeyEvent(SingleLobbyKeyEvent);
    }

    void Update()
    {
        //스크롤을 올리면
        if (contentPos.localPosition.y <= 1.45f) {
            //Content 위치 내리기 -> 스크롤은 올라감
            ScrollUp();
        }
        //스크롤을 내리면
        else if (contentPos.localPosition.y >= 400.0f) {
            //Content 위치 올리기 -> 스크롤은 내려감
            ScrollDown();
        }
    }

    public void ButtonEvent(string type) {
        switch (type)
        {
            case "Up":
                /*
                dest = contentPos.localPosition;
                dest -= new Vector2 (0, itemGap);
                StartCoroutine(SmoothScrollMove());*/
                break;
            case "Down":
                /*
                dest = contentPos.localPosition;
                dest += new Vector2(0, itemGap);
                StartCoroutine(SmoothScrollMove());*/
                break;  
            case "Exit":
                //TODO
                //로비 화면 Load하기
                break;
            case "KeyGuide":
                Guide();
                break;
            default:
                Debug.Log("아직 기능이 없거나 잘못 입력");
                break;
        }
    }

    void SingleLobbyKeyEvent(Define.UIControl keyEvent) 
    {
        switch (keyEvent)
        {
            case Define.UIControl.Up:
                ButtonEvent("Up");
                break;
            case Define.UIControl.Down:
                ButtonEvent("Down");
                break;
            case Define.UIControl.Enter:
                ButtonEvent("Enter");
                break;
            case Define.UIControl.Esc:
                ButtonEvent("Exit");
                break;
            case Define.UIControl.Guide:
                Gear();
                break;
            case Define.UIControl.Setting:
                ButtonEvent("Gear");
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D listItem)
    {
        curMusicItem = listItem.gameObject;
        Debug.Log("Selected Item: " + curMusicItem.name);
        UpdateInfo();
    }

    void UpdateInfo()
    {
        Debug.Log("Change Item");
        Music_Item newData = curMusicItem.GetComponent<Music_Item>();
        curMusicData[0].GetComponent<Image>().sprite = newData.Album.sprite;
        curMusicData[1].GetComponent<Text>().text = newData.Title.text;
        curMusicData[2].GetComponent<Text>().text = newData.Artist.text;
        curMusicData[3].GetComponent<Text>().text = newData.Length.text;
    }

    void ScrollUp()
    {
        //Content 이동
        contentPos.localPosition = new Vector2(0, 200.0f);
        //목록 갱신
        startIndex -= 5;
        if (startIndex < 0) {
            startIndex = numOfitems + startIndex;
        }
        for (int i = 0; i < musicItems.Length; i++) {
            musicItems[i].GetComponent<Music_Item>().SetData(musicList[startIndex++]);
            if (startIndex >= numOfitems) {
                startIndex = 0;
            }
        }
    }

    void ScrollDown() {
        //Content 이동
        contentPos.localPosition = new Vector2(0, 200.0f);
        //목록 갱신
        startIndex -= 12; // 12 = 17 - 5
        if (startIndex < 0) {
            startIndex = numOfitems + startIndex;
        }
        for (int i = 0; i < musicItems.Length; i++)
        {
            musicItems[i].GetComponent<Music_Item>().SetData(musicList[startIndex++]);
            if (startIndex >= numOfitems) {
                startIndex = 0;
            }
        }
    }

    IEnumerator SmoothScrollMove() {
        //스크롤을 올리면
        if (contentPos.localPosition.y <= 1.45f)
        {
            //Content 위치 내리기 -> 스크롤은 올라감
            ScrollUp();
            dest = contentPos.localPosition;
            dest += new Vector2(0, itemGap);
        }
        //스크롤을 내리면
        else if (contentPos.localPosition.y >= 400.0f)
        {
            //Content 위치 올리기 -> 스크롤은 내려감
            ScrollDown();
            dest = contentPos.localPosition;
            dest -= new Vector2(0, itemGap);
        }

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            contentPos.localPosition = Vector2.Lerp(contentPos.localPosition, dest, t);
            yield return null;
        }
        contentPos.localPosition = dest;
    }
}
