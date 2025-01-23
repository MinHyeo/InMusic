using UnityEngine;
using UnityEngine.UI;
using UI_BASE_PSH;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class Single_Lobby_UI : UI_Base
{
    [SerializeField] private GameObject[] musicItems = new GameObject[17];
    [SerializeField] private GameObject curMusicItem;
    [Tooltip("선택한 음악의 정보: 앨범, 제목, 아티스트, 길이")]
    [SerializeField] private GameObject[] curMusicData = new GameObject[4];
    [Tooltip("선택한 음악의 기록: 점수, 정확도, 콤보, 랭크")]
    [SerializeField] private Text[] logData = new Text[4];
    [Tooltip("스크롤 관련 변수")]
    [SerializeField] private RectTransform contentPos;
    [SerializeField] List<Music_Item> musicList = new List<Music_Item>();
    private float itemGap = 40.0f;
    private int numOfitems;
    [SerializeField] private int startIndex = 0;
    Vector2 dest;
    float duration = 0.3f;
    bool isScrolling = false;

    void Start()
    {
        //음악 목록 Load하기
        musicList = GameManager_PSH.Resource.GetMusicList();

        if (musicList == null) {
            Debug.Log("음악 목록 Load 실패");
            return;
        }

        numOfitems = musicList.Count;
        ContentDown();

        GameManager_PSH.Input.SetUIKeyEvent(SingleLobbyKeyEvent);
    }

    void Update()
    {
        //목록을 휠로 조작 처리
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0) //휠을 위로 돌렸을 때
        {
            ScrollUp();
        }

        else if (scroll < 0)  //휠을 아래로 돌렸을 때
        {
            ScrollDown();
        }
    }

    #region ItemDetect
    void OnTriggerEnter2D(Collider2D listItem)
    {
        curMusicItem = listItem.gameObject;
        curMusicItem.GetComponent<Music_Item>().ItemSelect();
        UpdateInfo();
    }

    private void OnTriggerExit2D(Collider2D listItem)
    {
        listItem.gameObject.GetComponent<Music_Item>().ItemUnselect();
    }

    #endregion

    public void ButtonEvent(string type) {
        switch (type)
        {
            case "Up":
                ScrollUp();
                break;
            case "Down":
                ScrollDown();
                break;
            case "Exit":
                //키 입력 이벤트 제거
                GameManager_PSH.Input.RemoveUIKeyEvent(SingleLobbyKeyEvent);
                //SceneManager.LoadScene("로비 씬");
                break;
            case "Enter":
                //키 입력 이벤트 제거
                GameManager_PSH.Input.RemoveUIKeyEvent(SingleLobbyKeyEvent);
                SceneManager.LoadScene(1);
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

    void UpdateInfo()
    {
        //Debug.Log("Change Item");
        Music_Item newData = curMusicItem.GetComponent<Music_Item>();
        //음악 정보 업데이트
        curMusicData[0].GetComponent<Image>().sprite = newData.Album.sprite;
        curMusicData[1].GetComponent<Text>().text = newData.Title.text;
        curMusicData[2].GetComponent<Text>().text = newData.Artist.text;
        curMusicData[3].GetComponent<Text>().text = newData.Length;
        //기록 정보 업데이트
        logData[0].text = newData.Score;
        logData[1].text = newData.Accuracy;
        logData[2].text = newData.Combo;
        logData[3].text = newData.Rank.text;
    }

    void ContentDown()
    {
        //Content 이동
        contentPos.localPosition = new Vector2(0, 200.0f);
        //목록 갱신
        startIndex -= 5;
        if (startIndex < 0) {
            startIndex = numOfitems + startIndex;
        }
        for (int i = 0; i < musicItems.Length; i++) {
            UpdateItems(musicItems[i].GetComponent<Music_Item>(), musicList[startIndex++]);
            if (startIndex >= numOfitems) {
                startIndex = 0;
            }
        }
    }

    void ContentUp() {
        //Content 이동
        contentPos.localPosition = new Vector2(0, 200.0f);
        //목록 갱신
        startIndex -= 12; // 12 = 17 - 5
        if (startIndex < 0) {
            startIndex = numOfitems + startIndex;
        }
        for (int i = 0; i < musicItems.Length; i++)
        {
            UpdateItems(musicItems[i].GetComponent<Music_Item>(), musicList[startIndex++]);
            if (startIndex >= numOfitems) {
                startIndex = 0;
            }
        }
    }

    void ScrollDown() {
        dest = contentPos.localPosition;
        dest += new Vector2(0, itemGap);
        StartCoroutine(SmoothScrollMove());
    }

    void ScrollUp(){
        dest = contentPos.localPosition;
        dest -= new Vector2(0, itemGap);
        StartCoroutine(SmoothScrollMove());
    }

    void UpdateItems(Music_Item oldItem, Music_Item newItem) {
        oldItem.Title.text = newItem.Title.text;
        oldItem.Artist.text = newItem.Artist.text;
        oldItem.Length = newItem.Length;
        oldItem.Album.sprite = newItem.Album.sprite;
        oldItem.Audio = newItem.Audio;
        oldItem.MuVi = newItem.MuVi;
        oldItem.Score = newItem.Score;
        oldItem.Accuracy = newItem.Accuracy + "%";
        oldItem.Combo = newItem.Combo;
        oldItem.Rank.text = newItem.Rank.text;
    }

    //부드럽게 이동: 스크롤/마우스 조작
    IEnumerator SmoothScrollMove() {
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
}
