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
    [Tooltip("������ ������ ����: �ٹ�, ����, ��Ƽ��Ʈ, ����")]
    [SerializeField] private GameObject[] curMusicData = new GameObject[4];
    [Tooltip("������ ������ ���: ����, ��Ȯ��, �޺�, ��ũ")]
    [SerializeField] private Text[] logData = new Text[4];
    [Tooltip("��ũ�� ���� ����")]
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
        //���� ��� Load�ϱ�
        musicList = GameManager_PSH.Resource.GetMusicList();

        if (musicList == null) {
            Debug.Log("���� ��� Load ����");
            return;
        }

        numOfitems = musicList.Count;
        ContentDown();

        GameManager_PSH.Input.SetUIKeyEvent(SingleLobbyKeyEvent);
    }

    void Update()
    {
        //����� �ٷ� ���� ó��
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0) //���� ���� ������ ��
        {
            ScrollUp();
        }

        else if (scroll < 0)  //���� �Ʒ��� ������ ��
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
                //Ű �Է� �̺�Ʈ ����
                GameManager_PSH.Input.RemoveUIKeyEvent(SingleLobbyKeyEvent);
                //SceneManager.LoadScene("�κ� ��");
                break;
            case "Enter":
                //Ű �Է� �̺�Ʈ ����
                GameManager_PSH.Input.RemoveUIKeyEvent(SingleLobbyKeyEvent);
                SceneManager.LoadScene(1);
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

    void UpdateInfo()
    {
        //Debug.Log("Change Item");
        Music_Item newData = curMusicItem.GetComponent<Music_Item>();
        //���� ���� ������Ʈ
        curMusicData[0].GetComponent<Image>().sprite = newData.Album.sprite;
        curMusicData[1].GetComponent<Text>().text = newData.Title.text;
        curMusicData[2].GetComponent<Text>().text = newData.Artist.text;
        curMusicData[3].GetComponent<Text>().text = newData.Length;
        //��� ���� ������Ʈ
        logData[0].text = newData.Score;
        logData[1].text = newData.Accuracy;
        logData[2].text = newData.Combo;
        logData[3].text = newData.Rank.text;
    }

    void ContentDown()
    {
        //Content �̵�
        contentPos.localPosition = new Vector2(0, 200.0f);
        //��� ����
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
        //Content �̵�
        contentPos.localPosition = new Vector2(0, 200.0f);
        //��� ����
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

    //�ε巴�� �̵�: ��ũ��/���콺 ����
    IEnumerator SmoothScrollMove() {
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
}
