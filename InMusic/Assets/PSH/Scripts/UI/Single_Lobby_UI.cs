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
    //��ũ�� ����
    [SerializeField]private RectTransform contentPos;
    private float itemGap = 40.0f;
    [SerializeField] List<string> musicList = new List<string>();
    private int  numOfitems;
    [SerializeField] private int startIndex = 0;
    Vector2 dest;

    void Start()
    {
        //���� ��� Load�ϱ�
        musicList = GameManager.Resource.GetMusicList();
        numOfitems = musicList.Count;
        ScrollUp();

        GameManager.Input.SetUIKeyEvent(SingleLobbyKeyEvent);
    }

    void Update()
    {
        //��ũ���� �ø���
        if (contentPos.localPosition.y <= 1.45f) {
            //Content ��ġ ������ -> ��ũ���� �ö�
            ScrollUp();
        }
        //��ũ���� ������
        else if (contentPos.localPosition.y >= 415.0f) {
            //Content ��ġ �ø��� -> ��ũ���� ������
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
                //�κ� ȭ�� Load�ϱ�
                break;
            case "KeyGuide":
                Guide();
                break;
            default:
                Debug.Log("���� ����� ���ų� �߸� �Է�");
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
        //Content �̵�
        contentPos.localPosition = new Vector2(0, 200.0f);
        //��� ����
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
        //Content �̵�
        contentPos.localPosition = new Vector2(0, 200.0f);
        //��� ����
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
