using UnityEngine;
using UnityEngine.UI;

public class MusicInfo : MonoBehaviour
{
    [Header("������ ������ ����")]
    [Tooltip("�ٹ�, ����, ��Ƽ��Ʈ, ����")]
    [SerializeField] private GameObject[] curMusicData = new GameObject[4];
    [Header("������ ������ �÷��� ���")]
    [Tooltip("����, ��Ȯ��, �޺�, ��ũ")]
    [SerializeField] private Text[] logData = new Text[4];

    public void UpdateInfo(Music_Item newItem)
    {
        //���� ���� ������Ʈ
        curMusicData[0].GetComponent<Image>().sprite = newItem.Album.sprite;
        curMusicData[1].GetComponent<Text>().text = newItem.Title.text;
        curMusicData[2].GetComponent<Text>().text = newItem.Artist.text;
        curMusicData[3].GetComponent<Text>().text = newItem.Length;
        //��� ���� ������Ʈ
        logData[0].text = newItem.Score;
        logData[1].text = newItem.Accuracy;
        logData[2].text = newItem.Combo;
        logData[3].text = newItem.Rank.text;
    }

}
