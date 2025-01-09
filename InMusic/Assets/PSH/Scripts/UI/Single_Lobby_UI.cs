using UnityEngine;
using UnityEngine.UI;
using UI_BASE_PSH;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections;

public class Single_Lobby_UI : UI_Base
{
    [SerializeField] private GameObject[] musicItems = new GameObject[17];
    [SerializeField] private GameObject curMusicItem;
    [SerializeField] private GameObject[] curMusicData = new GameObject[4];
    [SerializeField] private GameObject[] curMusic = new GameObject[3];
    [SerializeField] private Text[] logData = new Text[4];
    //스크롤 관련
    [SerializeField]private RectTransform contentPos;
    private float itemGap = 40.0f;
    [SerializeField] List<string> musicList = new List<string>();
    private int  numOfitems;
    [SerializeField] private int startIndex = 0;
    Vector2 dest;

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
        else if (contentPos.localPosition.y >= 415.0f) {
            //Content 위치 올리기 -> 스크롤은 내려감
            ScrollDown();
        }
    }

    public void ButtonEvent(string type) {
        switch (type)
        {
            case "Up":
                //TODO
                dest = contentPos.localPosition;
                dest += new Vector2 (0, itemGap);
                break;
            case "Down":
                //TODO
                dest = contentPos.localPosition;
                dest -= new Vector2(0, itemGap);
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

}
